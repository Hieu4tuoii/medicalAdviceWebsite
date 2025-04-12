using Microsoft.AspNetCore.Mvc;
using WebsiteTuVan.Models;
using WebsiteTuVan.Repositories;

namespace WebsiteTuVan.Controllers
{
    [Route("Article")]
    public class ArticleController : Controller
    {
        private readonly IArticlesRepository _repository;
        private readonly IUsersRepository _userRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        public ArticleController(IArticlesRepository repository, IUsersRepository userRepository, ICategoriesRepository _categoriesRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            this._categoriesRepository = _categoriesRepository;
        }
        
        //truy cap trang index
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            //get role from session
            var role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = role;
            return View("Index");
        }

        private async Task<User> checkAuthen(string roleCheck)
        {
            //kiem tra quyen truy cap
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return null;
            }
            var role = HttpContext.Session.GetString("Role");
            if (role == null || role != roleCheck)
            {
                return null;
            }
            //get user by email
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        //truy cap trang add bai viet
        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            //kiem tra quyen truy cap
            var user = await checkAuthen("doctor");
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("AddArticle");
        }

        //truy cap trang sửa bài iết
        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Add(int id)
        {
            //kiem tra quyen truy cap
            var user = await checkAuthen("doctor");
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //truyền viewdata cho view
            ViewData["ArticleId"] = id;

            return View("UpdateArticle");
        }

        //truy cập trang dashboard
        [HttpGet("Dashboard/{status}")]
        public async Task<IActionResult> Dashboard(string status)
        {
            //kiem tra quyen truy cap
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }
            var role = HttpContext.Session.GetString("Role");
            if (role == null || role != "doctor")
            {
                return RedirectToAction("Index", "Home");
            }

            //get user by email
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            ViewData["Status"] = status;

            return View("Dashboard");
        }

        //truy cập trang chi tiết bài viết
        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            ViewData["ArticleId"] = id;
            return View("ArticleDetail");
        }

        [HttpPost("Add")]
        public async Task<JsonResult> SaveArticle([FromBody] Article request)
        {
            // Kiểm tra quyền truy cập
            var user = await checkAuthen("doctor");
            if (user == null)
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập." });
            }

            //lấy doctorId từ session
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin bác sĩ." });
            }

            // Tạo ra article
            var article = new Article
            {
                Title = request.Title,
                Content = request.Content,
                CategoryId = request.CategoryId,
                DoctorId = doctorId ?? 0,
                Image = request.Image,
                Status = request.Status,
            };
            // Lưu article vào cơ sở dữ liệu
            await _repository.SaveAsync(article);
            return Json(new { success = true, message = "Thêm bài viết thành công", data = article.Id });
        }

        //get ds bài viết của bác sĩ theo status
        [HttpGet("DoctorArticles")]
        public async Task<JsonResult> getArticleList(string status, string keyword)
        {
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin bác sĩ." });
            }
            var articles = await _repository.FindByDoctorId(doctorId ?? 0, status, keyword);
            if (articles == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài viết." });
            }
            return Json(new { success = true, message = "Lấy danh sách bài viết thành công", data = articles });
        }

        //get bài viết đã publish
        [HttpGet("Published")]
        public async Task<JsonResult> getAllPublishedArticleList(int? categoryId, string? keyword, int? size)
        {
            var articles = await _repository.FindAllPublished(categoryId, keyword, size);
            if (articles == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài viết." });
            }
            return Json(new { success = true, message = "Lấy danh sách bài viết thành công", data = articles });
        }

        //get bài viết theo id
        [HttpGet("{id}")]
        public async Task<JsonResult> getArticleById(int id)
        {
            var article = await _repository.FindById(id);
            if (article == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài viết." });
            }
            return Json(new { success = true, message = "Lấy bài viết thành công", data = article });
        }

        //xử lý update bài viết
        [HttpPut("Update")]
        public async Task<JsonResult> UpdateArticle([FromBody] Article request)
        {
            // Kiểm tra quyền truy cập
            var user = await checkAuthen("doctor");
            if (user == null)
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập." });
            }

            //lấy doctorId từ session
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin bác sĩ." });
            }

            //kiêm tra bài viết tồn tại ko
            var existingArticle = await _repository.FindById(request.Id);
            if (existingArticle == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài viết." });
            }

            //kiểm tra xem người dùng hiện tại có phải là người tạo bài viết không
            if (existingArticle.DoctorId != doctorId)
            {
                return Json(new { success = false, message = "Bạn không có quyền sửa bài viết này." });
            }

            //cập nhật thông tin bài viết
            existingArticle.Title = request.Title;
            existingArticle.Content = request.Content;
            existingArticle.CategoryId = request.CategoryId;
            existingArticle.Image = request.Image;
            existingArticle.Status = request.Status;
            existingArticle.DoctorId = doctorId ?? 0;
            // Lưu bài viết vào cơ sở dữ liệu
            await _repository.UpdateAsync(existingArticle);
            return Json(new { success = true, message = "Cập nhật bài viết thành công", data = existingArticle.Id });
        }

        //xu ly xoa bai viet
        [HttpDelete("Delete/{id}")]
        public async Task<JsonResult> DeleteArticle(int id)
        {
            // Kiểm tra quyền truy cập
            var user = await checkAuthen("doctor");
            if (user == null)
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập." });
            }

            //lấy doctorId từ session
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin bác sĩ." });
            }

            //kiêm tra bài viết tồn tại ko
            var existingArticle = await _repository.FindById(id);
            if (existingArticle == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài viết." });
            }

            //kiểm tra xem người dùng hiện tại có phải là người tạo bài viết không
            if (existingArticle.DoctorId != doctorId)
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa bài viết này." });
            }

            //xoa bai viet
            await _repository.DeleteAsync(id);
            return Json(new { success = true, message = "Xóa bài viết thành công" });
        }
    }
}