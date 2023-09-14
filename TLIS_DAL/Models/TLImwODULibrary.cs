using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
   public class TLImwODULibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // [Index(IsUnique = true)]
        public string Model { get; set; }
        public string Note { get; set; }
        public float? Weight { get; set; }
        public string H_W_D { get; set; }
        public float Depth { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string frequency_range { get; set; }
        public string frequency_band { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        [ForeignKey("TLIparity")]
        public int? parityId { get; set; }
        public float Diameter { get; set; }

        public TLIparity parity { get; set; }
        public IEnumerable<TLImwODU> Mw_ODU { get; set; }
    }
}
