using Microsoft.AspNetCore.Mvc;
using WebsiteTuVan.Repositories;
using WebsiteTuVan.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteTuVan.Data; // Add this for Question model

namespace WebsiteTuVan.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionsRepository _repository;
        private readonly ICategoriesRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        // Gi?i h?n c?u hình
        private const int MaxAttachmentCount = 4;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" }; // Ch? cho phép ?nh

        public QuestionController(IQuestionsRepository repository, ICategoriesRepository categoryRepository, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var questions = await _repository.GetAllAsync();
            return View(questions);
        }

        // [Authorize] // Đảm bảo người dùng đã đăng nhập
        [HttpGet]
        public async Task<IActionResult> Create()
        {
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
                    PatientId = 1, // <<< --- !!! THAY BẰNG USER ID THỰC TẾ CỦA NGƯỜI DÙNG ĐANG ĐĂNG NHẬP
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
                            ModelState.AddModelError("", $"L?i khi t?i lên file '{file.FileName}'. Vui lòng th? l?i.");
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
                // TempData["SuccessMessage"] = "Gửi câu hỏi và file đính kèm thành công!";
                return RedirectToAction("Index", "Home"); 
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
    }
}