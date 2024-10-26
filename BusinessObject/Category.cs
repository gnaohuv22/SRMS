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
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [ForeignKey("DepartmentUser")]
        public int DepartmentUserId { get; set; }

        public virtual User DepartmentUser { get; set; }
        public virtual ICollection<Form> Forms { get; set; }
    }
}