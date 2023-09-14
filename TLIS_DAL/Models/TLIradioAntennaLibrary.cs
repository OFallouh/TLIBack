using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIradioAntennaLibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // [Index(IsUnique = true)]
        public string Model { get; set; }
        public string FrequencyBand { get; set; }
        public float? Weight { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }
        public float Length { get; set; }
        public string Notes { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public IEnumerable<TLIradioAntenna> radioAntennas { get; set; }
    }
}
