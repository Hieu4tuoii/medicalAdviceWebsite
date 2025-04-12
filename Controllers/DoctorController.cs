using Microsoft.AspNetCore.Mvc;
using WebsiteTuVan.Data;
using WebsiteTuVan.Models;
using WebsiteTuVan.Repositories;
using WebsiteTuVan.ViewModels;

namespace WebsiteTuVan.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorsRepository _repository;
        private readonly IUsersRepository _usersRepository;
        private readonly IQuestionsRepository _questionRepository;
        private readonly IAnswersRepository _answersRepository;
        private readonly ApplicationDbContext _context;

        public DoctorController(IDoctorsRepository repository, IUsersRepository usersRepository, IQuestionsRepository questionRepository, IAnswersRepository answersRepository, ApplicationDbContext context)
        {
            _repository = repository;
            _usersRepository = usersRepository;
            _questionRepository = questionRepository;
            _answersRepository = answersRepository;
            _context = context;
        }
        private async Task<(bool isAuthenticated, int userId, User? user)> GetCurrentUserFromSessionAsync()
        {
            var userEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(userEmail)) { return (false, 0, null); }
            var user = await _usersRepository.GetUserByEmailAsync(userEmail); // Dùng repo đã inject
            if (user == null) { HttpContext.Session.Clear(); return (false, 0, null); }
            return (true, user.Id, user);
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        //get top doctor
        [HttpGet("Doctor/GetAll")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _repository.GetAllAsync();
            if (doctors == null || doctors.Count() == 0)
            {
                return Json(new { success = false, message = "Không tìm thấy bác sĩ." });
            }

            return Json(new { success = true, data = doctors });
        }

        // Action chính cho trang của bác sĩ
        public async Task<IActionResult> Dashboard(string statusFilter = "pending", int pageNumber = 1, int pageSize = 10)
        {
            // 1. KIỂM TRA ĐĂNG NHẬP VÀ VAI TRÒ DOCTOR
            var role = HttpContext.Session.GetString("Role");
            var doctorName = HttpContext.Session.GetString("Name");
            //var userId = HttpContext.Session.GetInt32("UserId"); // Lấy ID để có thể lọc câu hỏi của bác sĩ này nếu cần

            if (string.IsNullOrEmpty(role) || role.ToLower() != "doctor" || string.IsNullOrEmpty(doctorName))
            {
                // Nếu chưa đăng nhập HOẶC không phải doctor -> về trang login
                HttpContext.Session.Clear(); // Xóa session nếu có lỗi role
                return RedirectToAction("Login", "User");
            }

            // 2. Lấy tất cả câu hỏi (bao gồm Patient)
            var allQuestions = await _questionRepository.GetAllIncludingPatientAsync();

            // 3. Lọc theo trạng thái (statusFilter)
            IEnumerable<Question> filteredQuestions = allQuestions;
            string normalizedFilter = statusFilter?.ToLower() ?? "pending"; // Mặc định là pending nếu null

            if (normalizedFilter != "all")
            {
                filteredQuestions = allQuestions.Where(q => q.Status == normalizedFilter);
            }

            // 4. Phân trang
            var totalItems = filteredQuestions.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages)); // Đảm bảo pageNumber hợp lệ

            var questionsForCurrentPage = filteredQuestions
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();

            // 5. Tạo danh sách ViewModel con (QuestionSummaryViewModel)
            var questionSummaries = questionsForCurrentPage.Select(q => new QuestionSummaryViewModel
            {
                Id = q.Id,
                PatientName = q.Patient?.Name ?? "N/A", // Lấy tên từ Patient, nếu null thì hiển thị N/A
                Title = q.Title,
                // Tạo nội dung rút gọn (ví dụ: 150 ký tự)
                ContentSnippet = q.Content.Length > 150 ? q.Content.Substring(0, 150) + "..." : q.Content,
                CreatedAt = q.CreatedAt,
                Status = q.Status
            }).ToList();

            // 6. Tạo ViewModel chính
            var viewModel = new DoctorDashboardViewModel
            {
                DoctorName = doctorName,
                CurrentFilter = normalizedFilter, // Lưu filter hiện tại (đã chuẩn hóa)
                Questions = questionSummaries,
                CurrentPage = pageNumber,
                TotalPages = totalPages
                // Gán thêm các thuộc tính phân trang khác nếu cần
            };
            return View(viewModel);
        }

        // --- Action Answer (GET): Hiển thị trang trả lời ---
        [HttpGet]
        public async Task<IActionResult> Answer(int id) // id là QuestionId
        {
            // 1. Kiểm tra Role Doctor
            var authResult = await GetCurrentUserFromSessionAsync();
            if (!authResult.isAuthenticated || authResult.user?.Role?.ToLower() != "doctor")
            {
                return RedirectToAction("Login", "User");
            }

            // 2. Lấy chi tiết câu hỏi (bao gồm Patient và Attachments)
            var question = await _questionRepository.GetDetailsByIdIncludingPatientAndAttachmentsAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            // 3. Chỉ cho phép trả lời câu hỏi đang chờ
            if (question.Status != "pending")
            {
                TempData["ErrorMessage"] = "Câu hỏi này đã được trả lời hoặc không ở trạng thái chờ.";
                return RedirectToAction("Dashboard"); // Về trang danh sách của bác sĩ
            }

            // 4. Tạo ViewModel
            var viewModel = new DoctorAnswerViewModel
            {
                QuestionId = question.Id,
                QuestionTitle = question.Title,
                QuestionContent = question.Content,
                PatientName = question.Patient?.Name ?? "N/A",
                QuestionCreatedAt = question.CreatedAt,
                // Map danh sách Attachments (nếu có)
                Attachments = question.Attachments?.Select(att => new AttachmentViewModel(
                                                            att.FileName,
                                                            att.FilePath, // Đường dẫn đã lưu để hiển thị
                                                            att.ContentType ?? "application/octet-stream" // Kiểu file
                                                        )).ToList() ?? new List<AttachmentViewModel>()
            };

            // 5. Trả về View
            return View(viewModel);
        }

        // --- Action Answer (POST): Xử lý gửi câu trả lời ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(DoctorAnswerViewModel model)
        {
            // 1. Kiểm tra Role Doctor
            var authResult = await GetCurrentUserFromSessionAsync();
            if (!authResult.isAuthenticated || authResult.user?.Role?.ToLower() != "doctor")
            {
                return RedirectToAction("Login", "User"); // Hoặc trả về lỗi phù hợp
            }
            int doctorUserId = authResult.userId; // ID của User đang đăng nhập (là doctor)

            // 2. Kiểm tra ModelState (chủ yếu là AnswerContent)
            if (ModelState.IsValid)
            {
                // Bắt đầu Transaction để đảm bảo lưu Answer và cập nhật Question cùng lúc
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 3. Lấy thông tin Doctor tương ứng với User đang đăng nhập
                    var doctor = await _repository.GetDoctorByUserIdAsync(doctorUserId);
                    if (doctor == null)
                    {
                        ModelState.AddModelError("", "Không tìm thấy thông tin bác sĩ của tài khoản này.");
                        await transaction.RollbackAsync(); // Hoàn tác nếu có lỗi
                                                           // Cần load lại dữ liệu câu hỏi để hiển thị lại form
                                                           // (Code load lại dữ liệu giống action GET - nên tách ra hàm riêng)
                        return View(await ReloadAnswerViewModelOnError(model));
                    }

                    // 4. Lấy lại câu hỏi gốc để cập nhật status
                    var question = await _questionRepository.GetByIdAsync(model.QuestionId);
                    if (question == null || question.Status != "pending")
                    {
                        ModelState.AddModelError("", "Câu hỏi không tồn tại hoặc đã được trả lời.");
                        await transaction.RollbackAsync();
                        return View(await ReloadAnswerViewModelOnError(model));
                    }

                    // 5. Tạo đối tượng Answer mới
                    var newAnswer = new Answer
                    {
                        Content = model.AnswerContent,
                        QuestionId = model.QuestionId,
                        DoctorId = doctor.Id, // ID của Doctor (từ bảng Doctors)
                        CreatedAt = DateTime.UtcNow // Dùng UTC cho server
                    };

                    // 6. Lưu Answer vào DB
                    await _answersRepository.AddAsync(newAnswer); // Giả sử AddAsync có SaveChanges

                    // 7. Cập nhật trạng thái Question thành "answered"
                    question.Status = "answered";
                    await _questionRepository.UpdateAsync(question); // Giả sử UpdateAsync có SaveChanges

                    // 8. Commit Transaction
                    await transaction.CommitAsync();

                    // 9. Thông báo thành công và chuyển hướng
                    TempData["SuccessMessage"] = "Gửi câu trả lời thành công!";
                    return RedirectToAction("Dashboard"); // Về trang danh sách của bác sĩ
                }
                catch (Exception ex) // Bắt lỗi trong quá trình lưu
                {
                    await transaction.RollbackAsync(); // Hoàn tác tất cả thay đổi nếu có lỗi
                                                       // Log lỗi ex
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu câu trả lời. Vui lòng thử lại.");
                    return View(await ReloadAnswerViewModelOnError(model)); // Hiển thị lại form với lỗi
                }
            }

            // Nếu ModelState không hợp lệ ngay từ đầu
            return View(await ReloadAnswerViewModelOnError(model)); // Hiển thị lại form với lỗi validation
        }
        // --- Hàm Helper để load lại ViewModel khi có lỗi ---
        private async Task<DoctorAnswerViewModel> ReloadAnswerViewModelOnError(DoctorAnswerViewModel currentModel)
        {
            var question = await _questionRepository.GetDetailsByIdIncludingPatientAndAttachmentsAsync(currentModel.QuestionId);
            if (question != null)
            {
                currentModel.QuestionTitle = question.Title;
                currentModel.QuestionContent = question.Content;
                currentModel.PatientName = question.Patient?.Name ?? "N/A";
                currentModel.QuestionCreatedAt = question.CreatedAt;
                currentModel.Attachments = question.Attachments?.Select(att => new AttachmentViewModel(
                                                            att.FileName, att.FilePath, att.ContentType ?? "application/octet-stream"
                                                        )).ToList() ?? new List<AttachmentViewModel>();
            }
            // Giữ lại AnswerContent đã nhập nhưng bị lỗi validation
            return currentModel;
        }
    }
}