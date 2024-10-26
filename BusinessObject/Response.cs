using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject
{
    public class Response
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResponseId { get; set; }
        [ForeignKey("Form")]
        public int FormId { get; set; }
        [ForeignKey("User")]
        public int StaffId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public virtual Form Form { get; set; }
        public virtual User User { get; set; }
    }
}
