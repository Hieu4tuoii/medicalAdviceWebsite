using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuVan.Models{
public class Comment {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string Content { get; set; }

    [Required]
    public int UserId { get; set; }
    public virtual User User { get; set; }

    [Required]
    public int ArticleId { get; set; }
    public virtual Article Article { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.Now; // thời gian tạo bình luận


}
}