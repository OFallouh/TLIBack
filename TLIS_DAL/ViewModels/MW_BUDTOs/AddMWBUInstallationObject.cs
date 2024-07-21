using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using static TLIS_DAL.ViewModels.MW_DishDTOs.AddMWDishInstallationObject;
namespace TLIS_DAL.ViewModels.MW_BUDTOs
{
    public class AddMWBUInstallationObject
    {
        public InstallationMWBUConfigObject installationConfig { get; set; }
        public AddCivilLoad civilLoads { get; set; }
        public installationAttributesMWBU installationAttributes { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class installationAttributesMWBU
        {
            public string? Notes { get; set; } = " ";
            public string Name { get; set; }
            public string Serial_Number { get; set; }
            public float Height { get; set; } = 0;
            public float Azimuth { get; set; } = 0;
            public int BUNumber { get; set; } = 0;
            public bool Active { get; set; } = true;
            public string? Visiable_Status { get; set; } = " ";
            public float SpaceInstallation { get; set; } = 0;
            public int? BaseBUId { get; set; }
            public int? OwnerId { get; set; }
            public float CenterHigh { get; set; } = 0;
            public float HBA { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float EquivalentSpace { get; set; } = 0;


        }
        public class InstallationMWBUConfigObject
        {
            public int InstallationPlaceId { get; set; }
            public int? civilSteelType { get; set; }
            public int? civilWithLegId { get; set; }
            public int? civilWithoutLegId { get; set; }
            public int? civilNonSteelId { get; set; }
            public List<int>? sideArmId { get; set; }
            public int? legId { get; set; }
            public int mwBuLibraryId { get; set; }
            public int? sdDishId { get; set; }
            public int mainDishId { get; set; }
            public int? CascededBuId { get; set; }
        }
        
    }
}