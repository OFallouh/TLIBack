using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIcapacity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Disable { get; set; }
        public bool Delete { get; set; }

        public IEnumerable<TLIsolarLibrary> SolarLibraries { get; set; }
        public IEnumerable<TLIgeneratorLibrary> GeneratorLibrary { get; set; }
    }
}
