using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIcabinetTelecomLibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // [Index(IsUnique = true)]
        public string Model { get; set; }
        public float? MaxWeight { get; set; }
        public string LayoutCode { get; set; }
        public string Dimension_W_D_H { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }
        public float Height { get; set; }
        public float SpaceLibrary { get; set; }
        public TLItelecomType TelecomType { get; set; }
        public int? TelecomTypeId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public IEnumerable<TLIcabinet> Cabinets { get; set; }
    }
}
