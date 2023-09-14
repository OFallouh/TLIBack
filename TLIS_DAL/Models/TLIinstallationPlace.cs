using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIinstallationPlace
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public bool Disable { get; set; }
        public IEnumerable<TLIRadioRRU> RadioRRUs { get; set; }
        public IEnumerable<TLIradioAntenna> radioAntennas { get; set; }
        public IEnumerable<TLImwDish> Mw_Dishes { get; set; }
        public IEnumerable<TLIradioOther> radioOthers { get; set; }
        public IEnumerable<TLIpower> powers { get; set; }
        public IEnumerable <TLImwBU> mwBU { get; set; }
    }
}
