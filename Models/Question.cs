using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteTuVan.Models{
    public class Question
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự")]
        public string Title { get; set; } 

        [Required]
        public string Content { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "pending";  // pending, answered

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Quan hệ với User
        public int PatientId { get; set; }
        public virtual User Patient { get; set; }  

        // Quan hệ với Category
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } 
    }
}
