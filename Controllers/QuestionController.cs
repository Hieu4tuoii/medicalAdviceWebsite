using Microsoft.AspNetCore.Mvc;
using WebsiteTuVan.Repositories;
using WebsiteTuVan.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteTuVan.Data;

namespace WebsiteTuVan.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionsRepository _repository;
        private readonly ICategoriesRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IAnswersRepository _answersRepository;
        private readonly IUsersRepository _usersRepository;
        //private readonly UserManager<User> _userManager;
        // Gi?i h?n c?u hình
        private const int MaxAttachmentCount = 4;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" }; // Ch? cho phép ?nh

        public QuestionController(IQuestionsRepository repository, ICategoriesRepository categoryRepository, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, IAnswersRepository answersRepository, IUsersRepository usersRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _answersRepository = answersRepository;
            _usersRepository = usersRepository;
            //_userManager = userManager;
        }

        // --- HÀM HELPER KIỂM TRA VÀ LẤY USER TỪ SESSION ---
        private async Task<(bool isAuthenticated, int userId, User? user)> GetCurrentUserFromSessionAsync()
        {
            var userEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(userEmail))
            {
                return (false, 0, null); // Chưa đăng nhập
            }

            var user = await _usersRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                // Email trong Session không hợp lệ hoặc user đã bị xóa
                HttpContext.Session.Clear(); // Xóa session cũ
                return (false, 0, null);
            }

            return (true, user.Id, user); // Đã đăng nhập, trả về ID và User object
        }
        // --- KẾT THÚC HÀM HELPER ---
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5)
        {
            // 1. Kiểm tra đăng nhập và lấy thông tin user
            var authResult = await GetCurrentUserFromSessionAsync();
            if (!authResult.isAuthenticated)
            {
                return RedirectToAction("Login", "User"); // Chuyển hướng nếu chưa đăng nhập
            }
            int patientId = authResult.userId;
            // 2. Lấy tất cả câu hỏi của người dùng từ Repository
            var allUserQuestions = await _repository.GetByPatientIdAsync(patientId);

            // 3. Thực hiện phân trang thủ công
            var totalItems = allUserQuestions.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Đảm bảo pageNumber hợp lệ
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

            var pagedQuestions = allUserQuestions
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

            // 4. Tạo ViewModel
            var viewModel = new QuestionListViewModel
            {
                Questions = pagedQuestions,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = totalPages
            };

            // 5. Trả về View với ViewModel
            return View(viewModel);
        }

        // [Authorize] // Đảm bảo người dùng đã đăng nhập
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // 1. Kiểm tra đăng nhập
            var authResult = await GetCurrentUserFromSessionAsync();
            if (!authResult.isAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var viewModel = new CreateQuestionViewModel
            {
                // Lấy danh sách chuyên khoa từ Repository và chuyển thành SelectListItem
                Categories = (await _categoryRepository.GetAllAsync())
                                         .Select(c => new SelectListItem
                                         {
                                             Value = c.Id.ToString(),
                                             Text = c.Name
                                         }).ToList()
            };
            return View(viewModel);
        }

        // [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken] // Ngăn chặn tấn công CSRF
        public async Task<IActionResult> Create(CreateQuestionViewModel viewModel /*, List<IFormFile> attachments */)
        {
            // Kiểm tra đăng nhập và lấy thông tin user
            var authResult = await GetCurrentUserFromSessionAsync();
            if (!authResult.isAuthenticated)
            {
                // Mặc dù form không nên hiển thị nếu chưa login, kiểm tra lại ở đây cho chắc chắn
                return RedirectToAction("Login", "User");
            }
            int patientId = authResult.userId;
            await LoadCategoriesIntoViewModel(viewModel);

            // --- **1. Validate File Attachments** ---
            if (viewModel.Attachments != null && viewModel.Attachments.Count > 0)
            {
                if (viewModel.Attachments.Count > MaxAttachmentCount)
                {
                    ModelState.AddModelError("Attachments", $"B?n ch? ???c phép t?i lên t?i ?a {MaxAttachmentCount} file.");
                }

                foreach (var file in viewModel.Attachments)
                {
                    // Kiểm tra kích thước
                    if (file.Length > _maxFileSize)
                    {
                        ModelState.AddModelError("Attachments", $"File '{file.FileName}' quá l?n. Kích th??c t?i ?a cho phép là {_maxFileSize / 1024 / 1024} MB.");
                    }

                    // Kiểm tra phần mở rộng (đuôi file)
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Attachments", $"File '{file.FileName}' có định dạng không hợp lệ. Chỉ chấp nhận các định dạng: {string.Join(", ", _allowedExtensions)}");
                    }
                }
            }

            // --- **2. Kiểm tra ModelState tổng thể** ---
            if (ModelState.IsValid)
            {
                // L?y User ID c?a ng??i dùng ?ang ??ng nh?p (c?n inject UserManager)
                // var userId = _userManager.GetUserId(User);
                // if (userId == null)
                // {
                //     // X? lý tr??ng h?p không tìm th?y user (ch?a ??ng nh?p?)
                //     ModelState.AddModelError("", "Không th? xác ??nh ng??i dùng. Vui lòng ??ng nh?p l?i.");
                //     return View(viewModel);
                // }
                // --- **3. Tạo và Lưu Question** ---
                var question = new Question
                {
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    CategoryId = viewModel.CategoryId,
                    PatientId = patientId, // <<< --- !!! THAY BẰNG USER ID THỰC TẾ CỦA NGƯỜI DÙNG ĐANG ĐĂNG NHẬP
                    Status = "pending", // Trạng thái mặc định khi mới tạo
                    CreatedAt = DateTime.Now
                };


                await _repository.AddAsync(question);
                // --- **4. Xử lý và Lưu Files (Nếu có)** ---
                var savedAttachments = new List<Attachment>();
                if (viewModel.Attachments != null && viewModel.Attachments.Count > 0)
                {
                    // Đường dẫn đến thư mục lưu file
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "questions");
                    // Tạo thư mục nếu chưa tồn tại
                    Directory.CreateDirectory(uploadsFolder);

                    foreach (var file in viewModel.Attachments)
                    {
                        try
                        {
                            // Tạo tên file duy nhất để tránh trùng lặp và vấn đề bảo mật
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            // Lưu file vào server
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Tạo đối tượng Attachment để lưu vào DB
                            var attachment = new Attachment
                            {
                                QuestionId = question.Id, // Gán ID của câu hỏi vừa tạo
                                FileName = file.FileName,
                                StoredFileName = uniqueFileName,
                                FilePath = "/uploads/questions/" + uniqueFileName, // Lưu đường dẫn tương đối
                                ContentType = file.ContentType,
                                FileSize = file.Length,
                                CreatedAt = DateTime.UtcNow
                            };
                            savedAttachments.Add(attachment);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"Lỗi khi tải lên file '{file.FileName}'. Vui lòng thử lại.");
                            return View(viewModel); // Trả về view với lỗi
                        }
                    }

                    // Lưu thông tin các file vào DB
                    if (savedAttachments.Any())
                    {
                        await _context.Attachments.AddRangeAsync(savedAttachments);
                        await _context.SaveChangesAsync(); // Lưu các attachment
                    }
                }

                // --- **5. Chuyển hướng sau khi thành công** ---
                TempData["SuccessMessage"] = "Gửi câu hỏi và file đính kèm thành công!";
                return RedirectToAction("Index", "Question"); 
            }

            // Nếu ModelState không hợp lệ (bao gồm cả lỗi file), hiển thị lại form
            return View(viewModel);
        }
        // Hàm helper để load Categories (tránh lặp code)
        private async Task LoadCategoriesIntoViewModel(CreateQuestionViewModel viewModel)
        {
            if (viewModel.Categories == null || !viewModel.Categories.Any())
            {
                viewModel.Categories = (await _categoryRepository.GetAllAsync())
                                       .Select(c => new SelectListItem
                                       {
                                           Value = c.Id.ToString(),
                                           Text = c.Name
                                       }).ToList();
            }
        }

        // --- Action Details ---
        [HttpGet]
        // Route có thể là "/cau-hoi/{id}/{slug?}" nếu muốn URL thân thiện
        public async Task<IActionResult> Details(int id)
        {
            // 1. Lấy chi tiết câu hỏi chính
            var question = await _repository.GetDetailsByIdAsync(id);
            if (question == null)
            {
                return NotFound(); // Không tìm thấy câu hỏi
            }

            // 2. Kiểm tra xem người dùng có quyền xem câu hỏi này không
            var authResult = await GetCurrentUserFromSessionAsync(); // Lấy thông tin người xem hiện tại

            // Logic kiểm tra quyền: Nếu câu hỏi đang chờ duyệt (pending) thì chỉ chủ nhân câu hỏi mới được xem
            if (question.Status == "pending")
            {
                if (!authResult.isAuthenticated || question.PatientId != authResult.userId)
                {
                    // Nếu chưa đăng nhập HOẶC đã đăng nhập nhưng không phải chủ câu hỏi -> không cho xem
                    // return Forbid(); // Trả về 403 Forbidden
                    return NotFound(); // Hoặc 404 để giấu sự tồn tại của câu hỏi
                }
            }

            // (Tùy chọn) Kiểm tra quyền truy cập:
            // var currentUserId = _userManager.GetUserId(User);
            // if (question.Status == "pending" && question.PatientId.ToString() != currentUserId)
            // {
            //     // Nếu câu hỏi đang chờ mà không phải của user hiện tại -> không cho xem
            //     return Forbid(); // Hoặc NotFound()
            // }

            // 2. Lấy câu trả lời (nếu có)
            Answer? answer = null;
            if (question.Status == "answered") // Chỉ lấy câu trả lời nếu status là answered
            {
                answer = await _answersRepository.GetByQuestionIdAsync(id);
            }

            // 3. Lấy các câu hỏi liên quan (đã công khai)
            int relatedQuestionsCount = 6; // Số lượng câu hỏi liên quan muốn hiển thị
            var relatedQuestionsData = await _repository.GetPublicAnsweredQuestionsAsync(question.CategoryId, id, relatedQuestionsCount);

            // 4. Chuẩn bị ViewModel
            var viewModel = new QuestionDetailViewModel
            {
                Question = question,
                Answer = answer,
                AnsweringDoctor = answer?.Doctor, // Lấy Doctor từ Answer (nếu answer không null)
                DoctorUser = answer?.Doctor?.User // Lấy User từ Doctor (nếu answer và doctor không null)
            };

            // Xử lý mapping cho danh sách câu hỏi liên quan
            foreach (var relatedQ in relatedQuestionsData)
            {
                // Cần lấy thông tin bác sĩ trả lời cho câu hỏi liên quan này
                // Cách lấy tùy thuộc vào cấu trúc DB và dữ liệu đã Include ở bước 3
                // Giả định là đã Include Answer -> Doctor -> User cho relatedQ
                var relatedAnswerDoctor = relatedQ.Answers?.FirstOrDefault()?.Doctor; // Lấy bác sĩ từ câu trả lời đầu tiên (nếu có)
                var relatedDoctorUser = relatedAnswerDoctor?.User;
                int? experienceYears = null;
                if (relatedAnswerDoctor != null)
                {
                    // Tính số năm kinh nghiệm, xử lý nếu YearOfStartingWork = 0 hoặc không hợp lệ
                    if (relatedAnswerDoctor.YearOfStartingWork > 1900 && relatedAnswerDoctor.YearOfStartingWork <= DateTime.Now.Year)
                    {
                        experienceYears = DateTime.Now.Year - relatedAnswerDoctor.YearOfStartingWork;
                    }
                }

                viewModel.RelatedQuestions.Add(new RelatedQuestionInfo(
                    relatedQ.Id,
                    relatedQ.Title,
                    relatedDoctorUser?.Name, // Lấy tên bác sĩ (có thể null)
                    experienceYears // Số năm kinh nghiệm (có thể null)
                ));
            }


            // 5. Trả về View với ViewModel
            return View(viewModel);
        }

    }
}