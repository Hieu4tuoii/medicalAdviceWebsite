using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebsiteTuVan.Models{
public class Category{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set ;}

    [Required]
    [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
    public string Name { get; set; }
}
}