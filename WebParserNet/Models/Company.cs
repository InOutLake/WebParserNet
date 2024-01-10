using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebParserNet.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        [InverseProperty("Company")]
        public List<Vacancy> Vacancies { get; set; }

        public override string ToString()
        {
            return $"ID: {CompanyId}, Name: {CompanyName}";
        }
    }
}
