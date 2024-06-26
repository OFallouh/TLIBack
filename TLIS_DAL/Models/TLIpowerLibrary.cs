using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIpowerLibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // [Index(IsUnique = true)]
        public string Model { get; set; }
        public string? Note { get; set; }
        public string? FrequencyRange { get; set; }
        public string? BandWidth { get; set; }
        public string? ChannelBandWidth { get; set; }
        public string? Type { get; set; }
        public float Size { get; set; }
        public string? L_W_H { get; set; }
        public float Weight { get; set; }
        public float width { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public float Depth { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public IEnumerable<TLIpower> powers { get; set; }
    }
}
