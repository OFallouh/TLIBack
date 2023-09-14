using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIsolar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PVPanelBrandAndWattage { get; set; }
        public string PVArrayAzimuth { get; set; }
        public string PVArrayAngel { get; set; }
        public string Prefix { get; set; }
        public string PowerLossRatio { get; set; }
        public int NumberOfSSU { get; set; }
        public int NumberOfLightingRod { get; set; }
        public int NumberOfInstallPVs { get; set; }
        public string LocationDescription { get; set; }
        public string ExtenstionDimension { get; set; }
        public string Extension { get; set; }
        public float SpaceInstallation { get; set; }
        public string VisibleStatus { get; set; }
        public TLIsolarLibrary SolarLibrary { get; set; }
        public int SolarLibraryId { get; set; }
        public TLIcabinet Cabinet { get; set; }
        public int? CabinetId { get; set; }

        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public IEnumerable<TLIallOtherInventoryInst> allOtherInventoryInsts { get; set; }
    }
}
