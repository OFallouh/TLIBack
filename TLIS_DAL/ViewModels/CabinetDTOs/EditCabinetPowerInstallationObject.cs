using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.SolarDTOs.AddSolarInstallationObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
using TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs;
using TLIS_DAL.Models;
using static TLIS_DAL.ViewModels.CabinetDTOs.AddCabinetPowerInstallation;

namespace TLIS_DAL.ViewModels.CabinetDTOs
{
    public class EditCabinetPowerInstallationObject
    {
        public LibraryAttributesCabinetPower CabinetPowerType { get; set; }
        public EditinstallationAttributesCabinetPower installationAttributes { get; set; }
        public AddOtherInSiteViewModel OtherInSite { get; set; }
        public AddOtherInventoryDistanceViewModel OtherInventoryDistance { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
    }
    public class EditinstallationAttributesCabinetPower
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public float Weight { get; set; } = 0;
        public int NumberOfBatteries { get; set; } = 0;
        public string? LayoutCode { get; set; } = " ";
        public string? Dimension_W_D_H { get; set; } = " ";
        public float BatteryWeight { get; set; } = 0;
        public string? BatteryType { get; set; } = " ";
        public string? BatteryDimension_W_D_H { get; set; } = " ";
        public float Depth { get; set; } = 0;
        public float Width { get; set; } = 0;
        public float Height { get; set; } = 0;
        public float SpaceLibrary { get; set; } = 0;
        public int CabinetPowerTypeId { get; set; }
        public bool Active { get; set; } = true;
        public bool Deleted { get; set; } = false;
        public bool PowerIntegrated { get; set; }
        public IntegratedWith? IntegratedWith { get; set; }

    }
}
