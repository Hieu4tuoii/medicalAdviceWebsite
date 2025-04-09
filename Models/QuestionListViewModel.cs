namespace WebsiteTuVan.Models
{
    public class QuestionListViewModel
    {
        public List<Question> Questions { get; set; } = new List<Question>(); // Danh sách câu hỏi trên trang hiện tại
        public int CurrentPage { get; set; } // Số trang hiện tại
        public int TotalPages { get; set; } // Tổng số trang
        public int PageSize { get; set; } // Số mục trên mỗi trang
        public int TotalCount { get; set; } // Tổng số câu hỏi
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
