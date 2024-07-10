using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIsolarLibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Model { get; set; }
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public string? TotaPanelsDimensions { get; set; }
        public string? StructureDesign { get; set; }
        public string? LayoutCode { get; set; }
        public float HeightFromFront { get; set; }
        public float HeightFromBack { get; set; }
        public string? BasePlateDimension { get; set; }
        public float SpaceLibrary { get; set; }
        public TLIcapacity Capacity { get; set; }
        public int? CapacityId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public IEnumerable<TLIsolar> Solars { get; set; }
    }
}
