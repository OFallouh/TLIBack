using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{

    public class DicMod
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public bool ? Deleted { get; set; }
        public bool ? Disable { get; set; }

    }
    public enum StructureTypeCompatibleWithDesign
    {
        Yes,
        No,
    }
    public enum SectionsLegTypeCompatibleWithDesign
    {
        Yes,
        No,
    }
    public class TLIcivilWithLegs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public float? WindMaxLoadm2 { get; set; }
        public double? LocationHeight { get; set; }
        public string PoType { get; set; }
        public string PoNo { get; set; }
        public DateTime? PoDate { get; set; }
        public double? HeightImplemented { get; set; }
        public float? BuildingMaxLoad { get; set; }
        public float? SupportMaxLoadAfterInforcement { get; set; }
        public double? CurrentLoads { get; set; }
        public double? warningpercentageloads { get; set; }
        public string VisiableStatus { get; set; }
        public string VerticalMeasured { get; set; }
        public string OtherBaseType { get; set; }
        public bool? IsEnforeced { get; set; }
        public double H2height { get; set; }
        public float HeightBase { get; set; }
        public string DimensionsLeg { get; set; }
        public string DiagonalMemberSection { get; set; }
        public string DiagonalMemberDimensions { get; set; }
        // edit BoltHoles to string
        public int? BoltHoles { get; set; }
        public string BasePlatethickness { get; set; }
        public string BasePlateShape { get; set; }
        public string BasePlateDimensions { get; set; }
        public string BaseNote { get; set; }
        public TLIlocationType locationType { get; set; }
        public int? locationTypeId { get; set; }
        public TLIbaseType baseType { get; set; }
        public int baseTypeId { get; set; }
        public string VerticalMeasurement { get; set; }
        public string SteelCrossSection { get; set; }
        public string DiagonalMemberPrefix { get; set; }
        public float EnforcementHeightBase { get; set; }
        public float Enforcementlevel { get; set; }
        public StructureTypeCompatibleWithDesign StructureType { get; set; }
        public SectionsLegTypeCompatibleWithDesign SectionsLegType { get; set; }



        public float TotalHeight { get; set; }
        public float SpaceInstallation { get; set; }
        public int? OwnerId { get; set; }
        public TLIowner Owner { get; set; }
        public int CivilWithLegsLibId { get; set; }
        public TLIcivilWithLegLibrary CivilWithLegsLib { get; set; }
        public int BaseCivilWithLegTypeId { get; set; }
        public TLIbaseCivilWithLegsType BaseCivilWithLegType { get; set; }

        public int? GuylineTypeId { get; set; }
        public TLIguyLineType GuyLineType { get; set; }
        public int? SupportTypeImplementedId { get; set; }
        public TLIsupportTypeImplemented SupportTypeImplemented { get; set; }
        public int? enforcmentCategoryId { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }

        public float Support_Limited_Load { get; set; }
        public TLIenforcmentCategory enforcmentCategory { get; set; }
        public IEnumerable<TLIallCivilInst> allCivilInsts { get; set; }
        public IEnumerable<TLIleg> Legs { get; set; }
    }
}
