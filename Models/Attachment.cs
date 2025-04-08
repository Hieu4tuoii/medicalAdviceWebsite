using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebsiteTuVan.Models
{
    public class Attachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; } // Foreign key đến bảng Question
        public virtual Question Question { get; set; } // Navigation property

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } // Tên file gốc

        [Required]
        [StringLength(255)]
        public string StoredFileName { get; set; } // Tên file duy nhất được lưu trên server

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } // Đường dẫn tương đối đến file đã lưu (ví dụ: /uploads/questions/...)

        [StringLength(100)]
        public string ContentType { get; set; } // Kiểu file (ví dụ: image/jpeg)

        public long FileSize { get; set; } // Kích thước file (bytes)

        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}
