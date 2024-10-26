using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormAPI.DTO
{
    public class ResponseDto
    {
        [Required]
        public int ResponseId { get; set; }
        [Required]
        public int FormId { get; set; }
        [Required]
        public int StaffId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        [BindNever]
        public string FormSubject { get; set; }
        [BindNever]
        public string FormContent { get; set; }
        [BindNever]
        public string StaffEmail { get; set; }
    }
}
