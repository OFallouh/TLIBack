using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.AllCivilInstDTOs
{
    public class CivilWithLegView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? SITECODE { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }
        public float? WindMaxLoadm2 { get; set; }
        public double? LocationHeight { get; set; }
        public string? PoType { get; set; }
        public string? PoNo { get; set; }
        public DateTime? PoDate { get; set; }
        public double? HeightImplemented { get; set; }
        public float? BuildingMaxLoad { get; set; }
        public float? SupportMaxLoadAfterInforcement { get; set; }
        public double? CurrentLoads { get; set; }
        public double? warningpercentageloads { get; set; }
        public string? VisiableStatus { get; set; }
        public string? VerticalMeasured { get; set; }
        public string? OtherBaseType { get; set; }
        public bool IsEnforeced { get; set; }
        public double H2height { get; set; }
        public float HeightBase { get; set; }
        public string? DimensionsLeg { get; set; }
        public string? DiagonalMemberSection { get; set; }
        public string? DiagonalMemberDimensions { get; set; }
        public int? BoltHoles { get; set; }
        public string? BasePlatethickness { get; set; }
        public string? BasePlateShape { get; set; }
        public string? BasePlateDimensions { get; set; }
        public string? BaseNote { get; set; }
        public string LOCATIONTYPE { get; set; }
        public string BASETYPE { get; set; }
        public string? VerticalMeasurement { get; set; }
        public string? SteelCrossSection { get; set; }
        public string? DiagonalMemberPrefix { get; set; }
        public float EnforcementHeightBase { get; set; }
        public float Enforcementlevel { get; set; }
        public StructureTypeCompatibleWithDesign StructureType { get; set; }
        public SectionsLegTypeCompatibleWithDesign SectionsLegType { get; set; }
        public float TotalHeight { get; set; }
        public float SpaceInstallation { get; set; }
        public string? OWNER { get; set; }
        public string CIVILWITHLEGSLIB { get; set; }
        public string BASECIVILWITHLEGTYPE { get; set; }
        public string? GUYLINETYPE { get; set; }
        public string SUPPORTTYPEIMPLEMENTED { get; set; }
        public float? CenterHigh { get; set; }
        public float? HBA { get; set; }
        public float? HieghFromLand { get; set; }
        public float? EquivalentSpace { get; set; }
        public float? Support_Limited_Load { get; set; }
        public string? ENFORCMENTCATEGORY { get; set; }


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
            outputData.Add("WindMaxLoadm2", WindMaxLoadm2);
            outputData.Add("LocationHeight", LocationHeight);
            outputData.Add("PoType", PoType);
            outputData.Add("PoNo", PoNo);
            outputData.Add("PoDate", PoDate);
            outputData.Add("HeightImplemented", HeightImplemented);
            outputData.Add("BuildingMaxLoad", BuildingMaxLoad);
            outputData.Add("SupportMaxLoadAfterInforcement", SupportMaxLoadAfterInforcement);
            outputData.Add("CurrentLoads", CurrentLoads);
            outputData.Add("warningpercentageloads", warningpercentageloads);
            outputData.Add("VisiableStatus", VisiableStatus);
            outputData.Add("VerticalMeasured", VerticalMeasured);
            outputData.Add("OtherBaseType", OtherBaseType);
            outputData.Add("IsEnforeced", IsEnforeced);
            outputData.Add("H2height", H2height);
            outputData.Add("HeightBase", HeightBase);
            outputData.Add("DimensionsLeg", DimensionsLeg);
            outputData.Add("DiagonalMemberSection", DiagonalMemberSection);
            outputData.Add("DiagonalMemberDimensions", DiagonalMemberDimensions);
            outputData.Add("BoltHoles", BoltHoles);
            outputData.Add("BasePlatethickness", BasePlatethickness);
            outputData.Add("BasePlateShape", BasePlateShape);
            outputData.Add("BasePlateDimensions", BasePlateDimensions);
            outputData.Add("BaseNote", BaseNote);
            outputData.Add("locationType", LOCATIONTYPE);
            outputData.Add("baseType", BASETYPE);
            outputData.Add("VerticalMeasurement", VerticalMeasurement);
            outputData.Add("SteelCrossSection", SteelCrossSection);
            outputData.Add("DiagonalMemberPrefix", DiagonalMemberPrefix);
            outputData.Add("EnforcementHeightBase", EnforcementHeightBase);
            outputData.Add("Enforcementlevel", Enforcementlevel);
            outputData.Add("StructureType", StructureType);
            outputData.Add("SectionsLegType", SectionsLegType);
            outputData.Add("TotalHeight", TotalHeight);
            outputData.Add("SpaceInstallation", SpaceInstallation);
            outputData.Add("Owner", OWNER);
            outputData.Add("CivilWithLegsLib", CIVILWITHLEGSLIB);
            outputData.Add("BaseCivilWithLegType", BASECIVILWITHLEGTYPE);
            outputData.Add("GuyLineType", GUYLINETYPE);
            outputData.Add("SupportTypeImplemented", SUPPORTTYPEIMPLEMENTED);
            outputData.Add("CenterHigh", CenterHigh);
            outputData.Add("HBA", HBA);
            outputData.Add("HieghFromLand", HieghFromLand);
            outputData.Add("EquivalentSpace", EquivalentSpace);
            outputData.Add("Support_Limited_Load", Support_Limited_Load);

            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}
    

