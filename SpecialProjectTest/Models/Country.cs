using System.Collections.Generic;

namespace SpecialProjectTest.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Questionnaire> Questionnaires { get; set; }
    }
}
