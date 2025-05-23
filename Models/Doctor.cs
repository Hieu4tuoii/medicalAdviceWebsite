using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuVan.Models{
public class Doctor{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set ;}

    [Required]
    public string Specialization { get; set; }

    //năm bắt đầu làm việc
    [Required]
    public int YearOfStartingWork { get; set; } = DateTime.Now.Year; //mặc định là năm hiện tại

    public string? Image { get; set; }

    //bio
    [Required]
    [StringLength(500, ErrorMessage = "Bio không được vượt quá 500 ký tự")]
    public string Bio { get; set;}

    //liên kết đến bảng Users
    [Required]
    public int UserId { get; set; }
    public virtual User User { get; set; }
}
}