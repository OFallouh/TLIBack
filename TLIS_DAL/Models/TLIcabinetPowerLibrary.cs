using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public enum IntegratedWith
    {
        Solar,
        Wind
       
    }
    public class TLIcabinetPowerLibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // [Index(IsUnique = true)]
        public string Model { get; set; }
        public float Weight { get; set; }
        public int NumberOfBatteries { get; set; }
        public string? LayoutCode { get; set; }
        public string? Dimension_W_D_H { get; set; }
        public float BatteryWeight { get; set; }
        public string? BatteryType { get; set; }
        public string? BatteryDimension_W_D_H { get; set; }
        public float Depth { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float SpaceLibrary { get; set; }
        public TLIcabinetPowerType CabinetPowerType { get; set; }
        public int CabinetPowerTypeId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool PowerIntegrated { get; set; }
        public IntegratedWith IntegratedWith { get; set; }
        public IEnumerable<TLIcabinet> Cabinets { get; set; }
    }
}
