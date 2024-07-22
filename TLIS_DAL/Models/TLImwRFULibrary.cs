using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public enum RFUType
    {
        Compact,
        Traditional

    }
    public class TLImwRFULibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Model { get; set; }
        public string? Note { get; set; }

        public float Weight { get; set; }
        public string? L_W_H { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string? size { get; set; }
        public bool tx_parity { get; set; }
        public string? frequency_band { get; set; }
        public string? FrequencyRange { get; set; }
        public RFUType RFUType { get; set; }
        public string? VenferBoardName { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        [ForeignKey("TLIdiversityType")]
        public int diversityTypeId { get; set; }
        public TLIdiversityType diversityType { get; set; }
        [ForeignKey("TLIboardType")]
        public int boardTypeId { get; set; }
        public TLIboardType boardType { get; set; }
        public IEnumerable<TLImwRFU> mwRFUs { get; set; }
    }
}
