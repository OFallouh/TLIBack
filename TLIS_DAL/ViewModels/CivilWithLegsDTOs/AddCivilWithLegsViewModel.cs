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
        public string Name { get; set; }
        public float? WindMaxLoadm2 { get; set; }
        public double? LocationHeight { get; set; }
        public string PoType { get; set; }
        public string PoNo { get; set; }
        public DateTime? PoDate { get; set; }
        public double? HeightImplemented { get; set; }
        public float? BuildingMaxLoad { get; set; }
        public float? SupportMaxLoadAfterInforcement { get; set; }
        public double? warningpercentageloads { get; set; }
        public int baseTypeId { get; set; } 
        public int CivilWithLegsLibId { get; set; }
        public string VisiableStatus { get; set; }
        public string VerticalMeasured { get; set; }
        public string OtherBaseType { get; set; }
        public bool IsEnforeced { get; set; } = false;
        public double H2height { get; set; }
        public string DimensionsLeg { get; set; }
        public string DiagonalMemberSection { get; set; }
        public string DiagonalMemberDimensions { get; set; }
        public int? BoltHoles { get; set; }
        public float EnforcementHeightBase { get; set; }
        public float Enforcementlevel { get; set; }
        public string BasePlatethickness { get; set; }
        public string BasePlateShape { get; set; }
        public string BasePlateDimensions { get; set; }
        public float HeightBase { get; set; }
        public float TotalHeight { get; set; }
        public string BaseNote { get; set; }
        public float SpaceInstallation { get; set; }
        public int? OwnerId { get; set; } = 0;
        public string VerticalMeasurement { get; set; }
        public string SteelCrossSection { get; set; }
        public string DiagonalMemberPrefix { get; set; }
        public int BaseCivilWithLegTypeId { get; set; }
        public int GuylineTypeId { get; set; }
        public int SupportTypeImplementedId { get; set; }
        public int? enforcmentCategoryId { get; set; } = 0;
        public int? locationTypeId { get; set; } = 0;
        public float Support_Limited_Load { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public StructureTypeCompatibleWithDesign StructureType { get; set; }
        public SectionsLegTypeCompatibleWithDesign SectionsLegType { get; set; }
        public AddCivilSiteDateViewModel TLIcivilSiteDate { get; set; }
        public AddCivilSupportDistanceViewModel TLIcivilSupportDistance { get; set; }
        public List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue { get; set; }
        public TicketAttributes ticketAtt{get; set;}
        public List<AddLegViewModel> Legs { get; set; }
    }
}
