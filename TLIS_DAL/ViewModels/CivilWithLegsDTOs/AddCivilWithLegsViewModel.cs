using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilSiteDateDTOs;
using TLIS_DAL.ViewModels.CivilSupportDistanceDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.LegDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegsDTOs
{
    public class AddCivilWithLegsViewModel
    {
        public LibraryAttributesCivilWithLegs civilType { get; set; }
        public installationAttributesCivilWithLegs installationAttributes { get; set; }
        public AddCivilSiteDateViewModel civilSiteDate { get; set; }
        public AddCivilSupportDistanceViewModel civilSupportDistance { get; set; }
        public List<AddLegViewModel> legsInfo { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
       
    }
    public class LibraryAttributesCivilWithLegs
    {
        public int civilWithLegsLibId { get; set; }
    }
    public class AddDdynamicAttributeInstallationValueViewModel
    {
        public int id { get; set; }
        public object value { get; set; }
    }

    public class installationAttributesCivilWithLegs
    {
        public string Name { get; set; }
        public float WindMaxLoadm2 { get; set; } = 0;
        public double LocationHeight { get; set; }
        public string? PoType { get; set; }
        public string? PoNo { get; set; }
        public DateTime? PoDate { get; set; } = DateTime.Now;
        [Required]
        public double HeightImplemented { get; set; }
        [Required]
        public float BuildingMaxLoad { get; set; }
        public float SupportMaxLoadAfterInforcement { get; set; }
        public double CurrentLoads { get; set; } = 0;
        public double warningpercentageloads { get; set; } = 0;
        public string? VisiableStatus { get; set; }
        public bool VerticalMeasured { get; set; }
        public string? OtherBaseType { get; set; }
        public bool IsEnforeced { get; set; } = false;
        public double H2height { get; set; } = 0;
        public float HeightBase { get; set; } = 0;
        public string? DimensionsLeg { get; set; }
        public string? DiagonalMemberSection { get; set; }
        public string? DiagonalMemberDimensions { get; set; }
        public int BoltHoles { get; set; } = 0;
        public string? BasePlatethickness { get; set; }
        public BasePlateShape BasePlateShape { get; set; }
        public string? BasePlateDimensions { get; set; }
        public string? BaseNote { get; set; }
        public int locationTypeId { get; set; }
        public int baseTypeId { get; set; }
        public string? VerticalMeasurement { get; set; }
        public string? SteelCrossSection { get; set; }
        public string? DiagonalMemberPrefix { get; set; }
        public float EnforcementHeightBase { get; set; } = 0;
        public float Enforcementlevel { get; set; } = 0;
        public bool StructureType { get; set; }
        public bool SectionsLegType { get; set; }
        public float TotalHeight { get; set; } = 0;
        public float SpaceInstallation { get; set; } = 0;
        public int OwnerId { get; set; }
        public int? BaseCivilWithLegTypeId { get; set; }
        public int? GuylineTypeId { get; set; }
        public int SupportTypeImplementedId { get; set; }
        public int? enforcmentCategoryId { get; set; }
        public string? SpecialEnforcementCategory { get; set; }
        public string? Remarks { get; set; }
        public float Support_Limited_Load { get; set; } = 0;

    }
}
