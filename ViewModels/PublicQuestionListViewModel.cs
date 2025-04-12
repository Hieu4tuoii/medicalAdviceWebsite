using WebsiteTuVan.Models;

namespace WebsiteTuVan.ViewModels
{
    public class PublicQuestionListViewModel
    {
        // Danh sách câu hỏi tóm tắt hiển thị trên trang
        public List<PublicQuestionSummary> Questions { get; set; } = new List<PublicQuestionSummary>();

        // Danh sách tất cả chuyên mục để hiển thị ở sidebar
        public List<Category> AllCategories { get; set; } = new List<Category>();

        // Thông tin tìm kiếm và lọc hiện tại
        public string? SearchTerm { get; set; } // Từ khóa tìm kiếm
        public int? CategoryId { get; set; } // ID chuyên mục đang lọc (null nếu không lọc)
        public string? CategoryName { get; set; } // Tên chuyên mục đang lọc (để hiển thị)

        // Thông tin phân trang
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
    // Record hoặc Class chứa thông tin tóm tắt cho mỗi card câu hỏi
    public record PublicQuestionSummary(
        int Id,
        string Title,
        string? AnsweringDoctorName, // Tên bác sĩ trả lời
        int? DoctorExperienceYears, // Số năm kinh nghiệm
        string? ImageUrl // Đường dẫn ảnh placeholder hoặc ảnh đại diện (tùy chọn)
    );
}
