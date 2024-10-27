using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.GeneratorDTOs.AddGeneratorInstallationObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
using TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs;
using static TLIS_DAL.ViewModels.SolarDTOs.AddSolarInstallationObject;

namespace TLIS_DAL.ViewModels.SolarDTOs
{
    public class EditSolarInstallationObject
    {
        public LibraryAttributesSolar SolarType { get; set; }
        public EditinstallationAttributesSolar installationAttributes { get; set; }
        public AddOtherInSiteViewModel OtherInSite { get; set; }
        public AddOtherInventoryDistanceViewModel OtherInventoryDistance { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
    }
    public class EditinstallationAttributesSolar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? PVPanelBrandAndWattage { get; set; } = " ";
        public string? PVArrayAzimuth { get; set; } = " ";
        public string? PVArrayAngel { get; set; } = " ";
        public string? Prefix { get; set; } = " ";
        public string? PowerLossRatio { get; set; } = " ";
        public int NumberOfSSU { get; set; } = 0;
        public int NumberOfLightingRod { get; set; } = 0;
        public int NumberOfInstallPVs { get; set; } = 0;
        public string? LocationDescription { get; set; } = " ";
        public string? ExtenstionDimension { get; set; } = " ";
        public string? Extension { get; set; } = " ";
        public float SpaceInstallation { get; set; } = 0;
        public string? VisibleStatus { get; set; } = " ";
        public int? CabinetId { get; set; }

    }
}
