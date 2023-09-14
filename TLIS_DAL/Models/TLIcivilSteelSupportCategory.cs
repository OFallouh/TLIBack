using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIcivilSteelSupportCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<TLIcivilWithLegLibrary> civilWithLeg { get; set; }
        public IEnumerable<TLIcivilWithoutLegLibrary> civilWithoutLeg { get; set; }
        public IEnumerable<TLIcivilLoads> civilLoads { get; set; }
    }
}
