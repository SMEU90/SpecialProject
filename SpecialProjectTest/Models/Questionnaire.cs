using System;

namespace SpecialProjectTest.Models
{
    public class Questionnaire
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public int CountryId { get; set; }
        public virtual Country Country { get; set; } 
    }
}
