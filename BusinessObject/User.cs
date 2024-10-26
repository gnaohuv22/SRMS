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
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // New navigation property for categories (when user is department)
        public virtual ICollection<Category> DepartmentCategories { get; set; }
        public virtual ICollection<Response> Responses { get; set; }
        public virtual ICollection<Form> Forms { get; set; }
    }
}