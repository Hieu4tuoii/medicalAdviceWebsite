using System.ComponentModel.DataAnnotations;

namespace WebsiteTuVan.ViewModels
{
    public class DoctorAnswerViewModel
    {
        // --- Thông tin hiển thị của câu hỏi ---
        public int QuestionId { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public string QuestionContent { get; set; } = string.Empty;
        public string? PatientName { get; set; } // Tên người hỏi
        public DateTime QuestionCreatedAt { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>(); // File đính kèm của bệnh nhân

        // --- Dữ liệu cho Form trả lời ---
        [Required(ErrorMessage = "Vui lòng nhập nội dung trả lời.")]
        [Display(Name = "Nội dung trả lời")]
        public string AnswerContent { get; set; } = string.Empty; // Bác sĩ nhập vào đây
    }
    // ViewModel con (hoặc record) cho file đính kèm (có thể dùng lại nếu đã tạo)
    public record AttachmentViewModel(
        string FileName,
        string FilePath, // Đường dẫn tương đối để hiển thị/tải file
        string ContentType // Ví dụ: "image/jpeg", "application/pdf"
    );
}
