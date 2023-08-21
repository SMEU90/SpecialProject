using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GrpcServiceSpecialProjectTest.Models
{
    public class Questionnaire
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
    }
}
