namespace WebsiteTuVan.Models
{
    public class QuestionDetailViewModel
    {
        public Question Question { get; set; } = null!; // Câu hỏi chính, không nên null ở trang chi tiết
        public Answer? Answer { get; set; } // Câu trả lời (có thể null)
        public Doctor? AnsweringDoctor { get; set; } // Bác sĩ trả lời (có thể null)
        public User? DoctorUser { get; set; } // Thông tin User của bác sĩ (có thể null)

        // Danh sách các câu hỏi liên quan (đã công khai)
        public List<RelatedQuestionInfo> RelatedQuestions { get; set; } = new List<RelatedQuestionInfo>();
    }
    public record RelatedQuestionInfo(
        int Id,
        string Title,
        string? DoctorName, // Tên bác sĩ trả lời câu hỏi liên quan
        int? DoctorExperienceYears // Số năm kinh nghiệm
                                   // Thêm ImageUrl nếu bạn lưu ảnh đại diện cho câu hỏi/bác sĩ
    );
}
