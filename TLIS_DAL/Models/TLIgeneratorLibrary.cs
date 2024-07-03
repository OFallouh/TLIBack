using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIgeneratorLibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Model { get; set; }
        public float Width { get; set; }
        public float Weight { get; set; }
        public float Length { get; set; }
        public string? LayoutCode { get; set; }
        public float Height { get; set; }
        public float SpaceLibrary { get; set; }
        public TLIcapacity Capacity { get; set; }
        public int? CapacityId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public IEnumerable<TLIgenerator> Generators { get; set; }
    }
}
