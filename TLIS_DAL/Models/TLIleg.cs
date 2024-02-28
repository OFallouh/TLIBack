using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIleg
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CiviLegName { get; set; }
        public string LegLetter { get; set; }
        public float LegAzimuth { get; set; } 
        public string? Notes { get; set; }
        public TLIcivilWithLegs CivilWithLegInst { get; set; }
        public int CivilWithLegInstId { get; set; }
        public IEnumerable<TLIcivilLoadLegs> civilLoadsLegs { get; set; }
        public IEnumerable<TLIcivilLoads> civilLoads { get; set; }
    }
}
