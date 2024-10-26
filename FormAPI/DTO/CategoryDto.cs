using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormAPI.DTO
{
    public class CategoryDto
    {
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int DepartmentId { get; set; }
    }
}
