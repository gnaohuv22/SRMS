using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject
{
    public enum UserRole
    {
        Student,
        Department,
        Admin
    }
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public UserRole Role { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Response> Responses { get; set; }
        public virtual ICollection<Form> Forms { get; set; }
    }
}
