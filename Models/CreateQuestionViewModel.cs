using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTuVan.Models
{
    public class CreateQuestionViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự.")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chuyên khoa.")]
        [Display(Name = "Chuyên khoa")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả chi tiết.")]
        [Display(Name = "Mô tả chi tiết")]
        public string Content { get; set; }

        // Dùng để hiển thị danh sách chuyên khoa trong dropdown
        public IEnumerable<SelectListItem>? Categories { get; set; }

        // [Display(Name = "File đính kèm")]
         public List<IFormFile>? Attachments { get; set; } 
    }
}
