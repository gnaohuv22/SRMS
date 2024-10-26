using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormAPI.DTO
{
    public class DepartmentDto
    {
        [Required]
        public int DepartmentId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
