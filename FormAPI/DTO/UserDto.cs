using BusinessObject;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormAPI.DTO
{
    public class UserDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public UserRole Role { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
