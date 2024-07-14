using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs
{
    public class EditCabinetPowerLibraryObject
    {
        public EditCabinetPowerLibraryAttributes AttributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public class EditCabinetPowerLibraryAttributes
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
            public IntegratedWith IntegratedWith { get; set; }

        }
    }
}
