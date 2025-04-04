
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebsiteTuVan.Models{
public class Article
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "tiêu đề không được vượt quá 100 ký tự")]
    public string Title { get; set; } // tiêu đề bài viết
    [Required]
    public string Content { get; set; } // nội dung bài viết
    public string Status { get; set; } =  "draft"; //draft, published 
    public DateTime CreatedAt { get; set; } = DateTime.Now; // thời gian tạo bài viết
    public string Image { get; set; } // hình ảnh đại diện cho bài viết
    [Required]
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } // danh mục bài viết
    [Required]
    public int DoctorId { get; set; } 
    public virtual Doctor Doctor { get; set; } // bác sĩ viết bài
}
}
