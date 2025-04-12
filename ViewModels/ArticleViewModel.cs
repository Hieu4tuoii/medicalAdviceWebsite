namespace WebsiteTuVan.ViewModels
{
    public class ArticleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } // tiêu đề bài viết
        public string Content { get; set; } // nội dung bài viết
        public string Status { get; set; } = "draft"; //draft, published 
        public string Image { get; set; } // hình ảnh đại diện cho bài viết
        public string CategoryName;
        public DateTime CreatedAt { get; set; } = DateTime.Now; // thời gian tạo bài viết

    }
}