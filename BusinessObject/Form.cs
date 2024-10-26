using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject
{
    public enum FormStatus
    {
        Pending,
        Processing,
        Accepted,
        Rejected
    }
    public class Form
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FormId { get; set; }
        [ForeignKey("User")]
        public int StudentId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }
        [Required]
        public FormStatus Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Response> Responses { get; set; }
    }
}
