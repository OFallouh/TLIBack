using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilSiteDateDTOs;
using TLIS_DAL.ViewModels.CivilSupportDistanceDTOs;
using TLIS_DAL.ViewModels.LegDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegsDTOs
{
    public class EditCivilWithLegsViewModelIntegration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float SpaceInstallation { get; set; }
        public double? CurrentLoads { get; set; }
        public double? HeightImplemented { get; set; }
        public float? WindMaxLoadm2 { get; set; }
        public double? LocationHeight { get; set; }
        public string PoType { get; set; }
        public string PoNo { get; set; }
        public DateTime? PoDate { get; set; }
        public float? BuildingMaxLoad { get; set; }
        public float? SupportMaxLoadAfterInforcement { get; set; }
        public double? warningpercentageloads { get; set; }
        public string VisiableStatus { get; set; }
        public string VerticalMeasured { get; set; }
        public string OtherBaseType { get; set; }
        public bool IsEnforeced { get; set; }
        public double H2height { get; set; }
        public float HeightBase { get; set; }
        public string DimensionsLeg { get; set; }
        public string DiagonalMemberSection { get; set; }
        public string DiagonalMemberDimensions { get; set; }
        public int? BoltHoles { get; set; }
        public string BasePlatethickness { get; set; }
        public string BasePlateShape { get; set; }
        public string BasePlateDimensions { get; set; }
        public string BaseNote { get; set; }
        public float TotalHeight { get; set; }
        public int? OwnerId { get; set; } = 0;
        public int CivilWithLegsLibId { get; set; }
        public int BaseCivilWithLegTypeId { get; set; }
        public int GuylineTypeId { get; set; }
        public int SupportTypeImplementedId { get; set; }
        public int? enforcmentCategoryId { get; set; } = 0;
        public int? locationTypeId { get; set; } = 0;
        public int baseTypeId { get; set; }
        public string VerticalMeasurement { get; set; }
        public string SteelCrossSection { get; set; }
        public string DiagonalMemberPrefix { get; set; }
        public float EnforcementHeightBase { get; set; }
        public float Enforcementlevel { get; set; }
        public float Support_Limited_Load { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public StructureTypeCompatibleWithDesign StructureType { get; set; }
        public SectionsLegTypeCompatibleWithDesign SectionsLegType { get; set; }
        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }
        public EditCivilSiteDateViewModel TLIcivilSiteDate { get; set; }
        public EditCivilSupportDistanceViewModel TLIcivilSupportDistance { get; set; }
        public List<EditLegViewModel> Legs { get; set; }
    }
}
