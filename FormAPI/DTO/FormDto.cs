using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormAPI.DTO
{
    public class FormDto
    {
        [Required]
        public int FormId { get; set; }
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public FormStatus Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        [BindNever]
        public string? CategoryName { get; set; }
        [BindNever]
        public string? StudentEmail { get; set; }
        [BindNever]
        public bool HasResponse { get; set; }
    }
}
