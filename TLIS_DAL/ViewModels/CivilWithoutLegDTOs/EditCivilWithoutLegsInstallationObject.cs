using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilSiteDateDTOs;
using TLIS_DAL.ViewModels.CivilSupportDistanceDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LegDTOs;

namespace TLIS_DAL.ViewModels.CivilWithoutLegDTOs
{
    public class EditCivilWithoutLegsInstallationObject
    {
        public CivilWithOutLegsLibraryAttributes civilType { get; set; }
        public EditinstallationAttributesCivilWithoutLegs installationAttributes { get; set; }
        public AddCivilSiteDateViewModel civilSiteDate { get; set; }
        public AddCivilSupportDistanceViewModel civilSupportDistance { get; set; }
        public List<AddLegViewModel> legsInfo { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class EditinstallationAttributesCivilWithoutLegs
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public float HeightBase { get; set; }
            public float UpperPartLengthm { get; set; }
            public float UpperPartDiameterm { get; set; }
            public float BottomPartDiameterm { get; set; }
            public float SpindlesBasePlateLengthcm { get; set; }
            public float SpindlesBasePlateWidthcm { get; set; }
            public float SpinBasePlateAnchorDiametercm { get; set; }
            public int NumberOfCivilParts { get; set; }
            public int NumberOfLongitudinalSpindles { get; set; }
            public int NumberOfhorizontalSpindle { get; set; }
            public float CivilLengthAboveEndOfSpindles { get; set; }
            public float CivilBaseLevelFromGround { get; set; }
            public float LongitudinalSpinDiameterrmm { get; set; }
            public float HorizontalSpindlesHBAm { get; set; }
            public float HorizontalSpindleDiametermm { get; set; }
            public float FlangeThicknesscm { get; set; }
            public float FlangeDiametercm { get; set; }
            public float FlangeBoltsDiametermm { get; set; }
            public float ConcreteBaseThicknessm { get; set; }
            public float ConcreteBaseLengthm { get; set; }
            public float ConcreteBaseWidthm { get; set; }
            public string? Civil_Remarks { get; set; }
            public float BottomPartLengthm { get; set; }
            public float BasePlateWidthcm { get; set; }
            public float BasePlateThicknesscm { get; set; }
            public float BasePlateLengthcm { get; set; }
            public float BPlateBoltsAnchorDiametermm { get; set; }
            public float BaseBeamSectionmm { get; set; }
            public float WindMaxLoadm2 { get; set; }
            public float Location_Height { get; set; }
            public string? PoType { get; set; }
            public string? PoNo { get; set; }
            public DateTime PoDate { get; set; }
            public bool reinforced { get; set; }
            [EnumDataType(typeof(LadderSteps))]
            public LadderSteps? ladderSteps { get; set; }
            public bool availabilityOfWorkPlatforms { get; set; }
            [EnumDataType(typeof(EquipmentsLocation))]
            public EquipmentsLocation? equipmentsLocation { get; set; }
            public float HeightImplemented { get; set; }
            public float BuildingMaxLoad { get; set; }
            public float SupportMaxLoadAfterInforcement { get; set; }
            public float CurrentLoads { get; set; }
            public float BuildingHeightH3 { get; set; }
            public int WarningPercentageLoads { get; set; }
            public string? Visiable_Status { get; set; }
            public float SpaceInstallation { get; set; }
            public int OwnerId { get; set; }
            public int? subTypeId { get; set; }
            public float Support_Limited_Load { get; set; }
            public float CenterHigh { get; set; }
            public float HBA { get; set; }
            public float HieghFromLand { get; set; }
            public float EquivalentSpace { get; set; }
        }
    } 
}
