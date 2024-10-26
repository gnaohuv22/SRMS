using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Form> Forms { get; set; }
    }
}
