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
        public LibraryAttributes civilType { get; set; }
        public installationAttributes installationAttributes { get; set; }
        public AddCivilSiteDateViewModel civilSiteDate { get; set; }
        public AddCivilSupportDistanceViewModel civilSupportDistance { get; set; }
        public List<AddLegViewModel> legsInfo { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
       
    }
    public class LibraryAttributes
    {
        public int civilWithLegsLibId { get; set; }
    }
    public class AddDdynamicAttributeInstallationValueViewModel
    {
        public int id { get; set; }
        public object value { get; set; }
    }

    public class installationAttributes
    {
        public string Name { get; set; }
        public float WindMaxLoadm2 { get; set; } = 0;
        public double LocationHeight { get; set; } = 0;
        public string? PoType { get; set; }
        public string? PoNo { get; set; }
        public DateTime? PoDate { get; set; } = DateTime.Now;
        public double HeightImplemented { get; set; } = 0;
        public float BuildingMaxLoad { get; set; } = 0;
        public float SupportMaxLoadAfterInforcement { get; set; } = 0;
        public double CurrentLoads { get; set; } = 0;
        public double warningpercentageloads { get; set; } = 0;
        public string? VisiableStatus { get; set; }
        public string? VerticalMeasured { get; set; }
        public string? OtherBaseType { get; set; }
        public bool IsEnforeced { get; set; } = false;
        public double H2height { get; set; } = 0;
        public float HeightBase { get; set; }
        public string? DimensionsLeg { get; set; }
        public string? DiagonalMemberSection { get; set; } 
        public string? DiagonalMemberDimensions { get; set; } 
        public int BoltHoles { get; set; } = 0;
        public string? BasePlatethickness { get; set; }
        public string? BasePlateShape { get; set; }
        public string? BasePlateDimensions { get; set; }
        public string? BaseNote { get; set; }
        public int? locationTypeId { get; set; }
        public int? baseTypeId { get; set; }
        public string? VerticalMeasurement { get; set; }
        public string? SteelCrossSection { get; set; }
        public string? DiagonalMemberPrefix { get; set; }
        public float EnforcementHeightBase { get; set; } = 0;
        public float Enforcementlevel { get; set; } = 0;
        public StructureTypeCompatibleWithDesign StructureType { get; set; }
        public SectionsLegTypeCompatibleWithDesign SectionsLegType { get; set; }
        public float TotalHeight { get; set; } = 0;
        public float SpaceInstallation { get; set; } = 0;
        public int OwnerId { get; set; }
        public int CivilWithLegsLibId { get; set; }
        public int? BaseCivilWithLegTypeId { get; set; }

        public int? GuylineTypeId { get; set; }

        public int? SupportTypeImplementedId { get; set; }
        public int? enforcmentCategoryId { get; set; }
        public float CenterHigh { get; set; } = 0;
        public float HBA { get; set; } = 0;
        public float HieghFromLand { get; set; } = 0;
        public float EquivalentSpace { get; set; } = 0;

        public float Support_Limited_Load { get; set; } = 0;

    }
}
