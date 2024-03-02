using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilSiteDateDTOs;
using TLIS_DAL.ViewModels.CivilSupportDistanceDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.CivilNonSteelDTOs
{
    public class AddCivilNonSteelViewModel
    {
        public CivilNonSteelegsLibraryAttributes civilType { get; set; }
        public installationAttributesCivilNonSteelLegs installationAttributes { get; set; }
        public AddCivilSiteDateViewModel civilSiteDate { get; set; }
        public AddCivilSupportDistanceViewModel civilSupportDistance { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
    }
    public class CivilNonSteelegsLibraryAttributes
    {
        public int civilNonSteelLegsLibId { get; set; }
    }
    public class installationAttributesCivilNonSteelLegs
    {
        public string Name { get; set; }
        public float SpaceInstallation { get; set; }
        public int CivilNonSteelLibraryId { get; set; }
        public int? ownerId { get; set; } = 0;
        public int? supportTypeImplementedId { get; set; } = 0;
        public int? locationTypeId { get; set; } = 0;
        public float locationHeight { get; set; }
        public float BuildingMaxLoad { get; set; }
        public float CivilSupportCurrentLoad { get; set; }
        public float H2Height { get; set; }
        public float HieghFromLand { get; set; }
        public float Support_Limited_Load { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float EquivalentSpace { get; set; }
}
}


