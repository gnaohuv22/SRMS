using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace assignment3_b3w.Settings
{
    public class Credential
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
