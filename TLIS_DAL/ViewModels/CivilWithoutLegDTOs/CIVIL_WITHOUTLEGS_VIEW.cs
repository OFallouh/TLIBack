using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.CivilWithoutLegDTOs
{
    public class MV_CIVIL_WITHOUTLEGS_VIEW
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string SITECODE { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
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
        public LadderSteps? ladderSteps { get; set; }
        public bool availabilityOfWorkPlatforms { get; set; }
        public EquipmentsLocation? equipmentsLocation { get; set; }
        public float HeightImplemented { get; set; }
        public float BuildingMaxLoad { get; set; }
        public float SupportMaxLoadAfterInforcement { get; set; }
        public float CurrentLoads { get; set; }
        public float BuildingHeightH3 { get; set; }
        public int WarningPercentageLoads { get; set; }
        public string? Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public string CIVILWITHOUTLEGSLIB { get; set; }
        public string CIVILWITHOUTLEGCATEGORY { get; set; }
        public string OWNER { get; set; }
        public string? SUBTYPE { get; set; }
        public float Support_Limited_Load { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }

        public bool Dismantle { get; set; }
        public Dictionary<string, object> GenerateOutputData()
        {
            Dictionary<string, object> outputData = new Dictionary<string, object>();

            // Add original properties
            outputData.Add("sitecode", SITECODE);
            outputData.Add("dynamicKeyProperties", null);
            outputData.Add("dynamicValueProperties", null);
            outputData.Add("key", Key);
            outputData.Add("value", INPUTVALUE);
            outputData.Add("id", Id);
            outputData.Add("Name", Name);
            outputData.Add("HeightBase", HeightBase);
            outputData.Add("UpperPartLengthm", UpperPartLengthm);
            outputData.Add("UpperPartDiameterm", UpperPartDiameterm);
            outputData.Add("BottomPartDiameterm", BottomPartDiameterm);
            outputData.Add("SpindlesBasePlateLengthcm", SpindlesBasePlateLengthcm);
            outputData.Add("SpindlesBasePlateWidthcm", SpindlesBasePlateWidthcm);
            outputData.Add("SpinBasePlateAnchorDiametercm", SpinBasePlateAnchorDiametercm);
            outputData.Add("NumberOfCivilParts", NumberOfCivilParts);
            outputData.Add("NumberOfLongitudinalSpindles", NumberOfLongitudinalSpindles);
            outputData.Add("NumberOfhorizontalSpindle", NumberOfhorizontalSpindle);
            outputData.Add("CivilLengthAboveEndOfSpindles", CivilLengthAboveEndOfSpindles);
            outputData.Add("CivilBaseLevelFromGround", CivilBaseLevelFromGround);
            outputData.Add("LongitudinalSpinDiameterrmm", LongitudinalSpinDiameterrmm);
            outputData.Add("HorizontalSpindlesHBAm", HorizontalSpindlesHBAm);
            outputData.Add("HorizontalSpindleDiametermm", HorizontalSpindleDiametermm);
            outputData.Add("FlangeThicknesscm", FlangeThicknesscm);
            outputData.Add("FlangeDiametercm", FlangeDiametercm);
            outputData.Add("FlangeBoltsDiametermm", FlangeBoltsDiametermm);
            outputData.Add("ConcreteBaseThicknessm", ConcreteBaseThicknessm);
            outputData.Add("ConcreteBaseLengthm", ConcreteBaseLengthm);
            outputData.Add("ConcreteBaseWidthm", ConcreteBaseWidthm);
            outputData.Add("Civil_Remarks", Civil_Remarks);
            outputData.Add("BottomPartLengthm", BottomPartLengthm);
            outputData.Add("BasePlateWidthcm", BasePlateWidthcm);
            outputData.Add("BasePlateThicknesscm", BasePlateThicknesscm);
            outputData.Add("BasePlateLengthcm", BasePlateLengthcm);
            outputData.Add("BPlateBoltsAnchorDiametermm", BPlateBoltsAnchorDiametermm);
            outputData.Add("BaseBeamSectionmm", BaseBeamSectionmm);
            outputData.Add("WindMaxLoadm2", WindMaxLoadm2);
            outputData.Add("Location_Height", Location_Height);
            outputData.Add("PoType", PoType);
            outputData.Add("PoNo", PoNo);
            outputData.Add("PoDate", PoDate);
            outputData.Add("reinforced", reinforced);
            outputData.Add("ladderSteps", ladderSteps);
            outputData.Add("availabilityOfWorkPlatforms", availabilityOfWorkPlatforms);
            outputData.Add("equipmentsLocation", equipmentsLocation);
            outputData.Add("HeightImplemented", HeightImplemented);
            outputData.Add("GuyLiBuildingMaxLoadneType", BuildingMaxLoad);
            outputData.Add("SupportMaxLoadAfterInforcement", SupportMaxLoadAfterInforcement);
            outputData.Add("CurrentLoads", CurrentLoads);
            outputData.Add("BuildingHeightH3", BuildingHeightH3);
            outputData.Add("WarningPercentageLoads", WarningPercentageLoads);
            outputData.Add("Visiable_Status", Visiable_Status);
            outputData.Add("SpaceInstallation", SpaceInstallation);
            outputData.Add("Support_Limited_Load", Support_Limited_Load);
            outputData.Add("CenterHigh", CenterHigh);
            outputData.Add("HBA", HBA);
            outputData.Add("HieghFromLand", HieghFromLand);
            outputData.Add("EquivalentSpace", EquivalentSpace);
            outputData.Add("CIVILWITHOUTLEGSLIB", CIVILWITHOUTLEGSLIB);
            outputData.Add("OWNER", OWNER);
            outputData.Add("SUBTYPE", SUBTYPE);
            outputData.Add("Dismantle", Dismantle);

            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}


