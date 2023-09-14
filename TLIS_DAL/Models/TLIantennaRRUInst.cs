using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIantennaRRUInst
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TLIradioAntenna radioAntenna { get; set; }
        public int radioAntennaId { get; set; }
        public TLIRadioRRU radioRRU { get; set; }
        public int radioRRUId { get; set; }
    }
}
