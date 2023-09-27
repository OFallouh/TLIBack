using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.CivilWithoutLegDTOs
{
    public class CivilWithoutLegViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float HeightBase { get; set; }
        public float? UpperPartLengthm { get; set; }
        public float? UpperPartDiameterm { get; set; }
        public float? SpindlesBasePlateLengthcm { get; set; }
        public float? SpindlesBasePlateWidthcm { get; set; }
        public float? SpinBasePlateAnchorDiametercm { get; set; }
        public int? NumberOfCivilParts { get; set; }
        public int? NumberOfLongitudinalSpindles { get; set; }
        public int? NumberOfhorizontalSpindle { get; set; }
        public float? CivilLengthAboveEndOfSpindles { get; set; }
        public float? CivilBaseLevelFromGround { get; set; }
        public float? LongitudinalSpinDiameterrmm { get; set; }
        public float? HorizontalSpindlesHBAm { get; set; }
        public float? HorizontalSpindleDiametermm { get; set; }
        public float? FlangeThicknesscm { get; set; }
        public float? FlangeDiametercm { get; set; }
        public float? FlangeBoltsDiametermm { get; set; }
        public float? ConcreteBaseThicknessm { get; set; }
        public float? ConcreteBaseLengthm { get; set; }
        public string Civil_Remarks { get; set; }
        public float? BottomPartLengthm { get; set; }
        public float? BottomPartDiametermm { get; set; }
        public float? BasePlateWidthcm { get; set; }
        public float? BasePlateThicknesscm { get; set; }
        public float? BasePlateLengthcm { get; set; }
        public float? BPlateBoltsAnchorDiametermm { get; set; }
        public float? BaseBeamSectionmm { get; set; }
        public float? WindMaxLoadm2 { get; set; }
        public float? Location_Height { get; set; }
        public string PoType { get; set; }
        public string PoNo { get; set; }
        public DateTime PoDate { get; set; }
        public float? HeightImplemented { get; set; }
        public float? BuildingMaxLoad { get; set; }
        public float? SupportMaxLoadAfterInforcement { get; set; }
        public float? CurrentLoads { get; set; }
        public int? WarningPercentageLoads { get; set; }
        public string Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public int CivilWithoutlegsLibId { get; set; }
        public string civilWithoutLegsLib_Name { get; set; }
        public int? OwnerId { get; set; }
        public string Owner_Name { get; set; }
        public float Support_Limited_Load { get; set; }
        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }


        [EnumDataType(typeof(Reinforced))]

        public Reinforced? reinforced { get; set; }
        [EnumDataType(typeof(LadderSteps))]

        public LadderSteps? ladderSteps { get; set; }
        [EnumDataType(typeof(AvailabilityOfWorkPlatforms))]

        public AvailabilityOfWorkPlatforms? availabilityOfWorkPlatforms { get; set; }
        [EnumDataType(typeof(EquipmentsLocation))]

        public EquipmentsLocation? equipmentsLocation { get; set; }
        //FROM BASE 
        public float? BuildingHeightH3 { get; set; }
        public int? subTypeId { get; set; }
        public string subType_Name { get; set; }
        public float HieghFromLand { get; set; }
        public float CenterHigh { get; set; }
    }
}
