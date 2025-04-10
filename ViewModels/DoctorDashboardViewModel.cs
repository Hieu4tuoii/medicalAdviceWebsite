namespace WebsiteTuVan.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public string DoctorName { get; set; } = string.Empty; // Tên bác sĩ đang đăng nhập
        public string CurrentFilter { get; set; } = "pending"; // Filter mặc định ("all", "pending", "answered")
        public List<QuestionSummaryViewModel> Questions { get; set; } = new List<QuestionSummaryViewModel>(); // Danh sách câu hỏi tóm tắt
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
    // ViewModel con để chứa thông tin tóm tắt của mỗi câu hỏi trong danh sách
    public class QuestionSummaryViewModel
    {
        public int Id { get; set; }
        public string? PatientName { get; set; } // Tên người hỏi (có thể null nếu User bị xóa)
        public string Title { get; set; } = string.Empty;
        public string ContentSnippet { get; set; } = string.Empty; // Nội dung rút gọn
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
