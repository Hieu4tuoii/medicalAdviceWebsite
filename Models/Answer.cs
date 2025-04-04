using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace WebsiteTuVan.Models{
public class Answer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    [Required]
    public int QuestionId { get; set; }
    public virtual Question Question { get; set; }
    
    [Required]
    public int DoctorId { get; set; }
    public virtual Doctor Doctor { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
}