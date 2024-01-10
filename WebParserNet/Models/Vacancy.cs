using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebParserNet.Models
{
    public class Vacancy
    {
        [Key]
        public int VacancyId { get; set; }
        public string? Name {  get; set; }
        public string? Meta {  get; set; }
        public string? Skills { get; set; }
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

        public override string ToString()
        {
            return $"Name: {Name},\nCompany: {Company?.CompanyName},\nMeta: {Meta},\nSkills: {Skills}";
        }

    }
}
