using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuVan.Models{
public class User{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set ;}

    [Required]
    [StringLength(255, ErrorMessage = "Tên không được vượt quá 255 ký tự")]
    public string Name { get; set; }

    [Required]
    [StringLength(255, ErrorMessage = "Địa chỉ email không được vượt quá 255 ký tự")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
    public string Email { get; set; }

    [Required]
    [StringLength(255, ErrorMessage = "Mật khẩu không được vượt quá 255 ký tự")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    //Role
    [Required]
    [StringLength(20)]
    public string Role { get; set; } = "user"; //mặc định là user

   //created
    public DateTime CreatedAt { get; set; } = DateTime.Now; //mặc định là tg khi tạo tài khoản
}
}