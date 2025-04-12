using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebsiteTuVan.ViewModels
{
    public class ArticleAddViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài viết")]
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự")]
        public string Title { get; set; } // tiêu đề bài viết
        [Required(ErrorMessage = "Vui lòng nhập nội dung bài viết")]
        [DataType(DataType.MultilineText)]
        [StringLength(5000, ErrorMessage = "Nội dung không được vượt quá 5000 ký tự")]
        public string Content { get; set; } // nội dung bài viết
        public string Status { get; set; }  //draft, published 
        public string Image { get; set; } = ""; // hình ảnh đại diện cho bài viết
        public string CategoryId {get; set;} // danh mục bài viết;
        public string DoctorId { get; set; } // bác sĩ viết bài

        // Dùng để hiển thị danh sách chuyên khoa trong dropdown
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}