using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLImwBULibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // [Index(IsUnique = true)]
        public string Model { get; set; }

        public string Type { get; set; }

        public string Note { get; set; }
        public string L_W_H { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public string BUSize { get; set; }
        public int NumOfRFU { get; set; }
        public string frequency_band { get; set; }
        public float? channel_bandwidth { get; set; }
        public string FreqChannel { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }

        [ForeignKey("TLIdiversityType")]
        public int? diversityTypeId { get; set; }
        public TLIdiversityType diversityType { get; set; }
        public IEnumerable<TLImwBU> Mw_Bu { get; set; }
        public IEnumerable<TLImwPort> MwPort { get; set; }
    }
}
