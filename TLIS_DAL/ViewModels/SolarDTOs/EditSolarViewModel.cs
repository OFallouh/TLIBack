using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
using TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs;

namespace TLIS_DAL.ViewModels.SolarDTOs
{
    public class EditSolarViewModel
    {
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
        public int? SolarLibraryId { get; set; }
        public int? CabinetId { get; set; } = 0;
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public EditOtherInSiteViewModel TLIotherInSite { get; set; }
        public EditOtherInventoryDistanceViewModel TLIotherInventoryDistance { get; set; }
        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }
    }
}
