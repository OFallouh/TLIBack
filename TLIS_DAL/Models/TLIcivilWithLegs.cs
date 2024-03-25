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
    public enum BasePlateShape
    {
        Circular,
        Rectangular,
        Square,
        NotMeasurable
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
        [Required]
        public string Name { get; set; }
        public float WindMaxLoadm2 { get; set; } = 0;
        public double LocationHeight { get; set; } = 0;
        public string? PoType { get; set; }
        public string? PoNo { get; set; }
        public DateTime? PoDate { get; set; } = DateTime.Now;
        [Required]
        public double HeightImplemented { get; set; } = 0;
        [Required]
        public float BuildingMaxLoad { get; set; } = 0;
        public float SupportMaxLoadAfterInforcement { get; set; } = 0;
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
        public TLIlocationType locationType { get; set; }
        public int locationTypeId { get; set; }
        public TLIbaseType baseType { get; set; }
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
        public TLIowner Owner { get; set; }
        public int CivilWithLegsLibId { get; set; }
        public TLIcivilWithLegLibrary CivilWithLegsLib { get; set; }
        public int? BaseCivilWithLegTypeId { get; set; }
        public TLIbaseCivilWithLegsType? BaseCivilWithLegType { get; set; }
        public int? GuylineTypeId { get; set; }
        public TLIguyLineType? GuyLineType { get; set; }
        public int SupportTypeImplementedId { get; set; }
        public TLIsupportTypeImplemented SupportTypeImplemented { get; set; }
        public int? enforcmentCategoryId { get; set; }
        public float CenterHigh { get; set; } = 0;
        public float HBA { get; set; } = 0;
        public float HieghFromLand { get; set; } = 0;
        public float EquivalentSpace { get; set; } = 0;
        public string? SpecialEnforcementCategory { get; set; }
        public string? Remark { get; set; }
        public float Support_Limited_Load { get; set; } = 0;
        public TLIenforcmentCategory? enforcmentCategory { get; set; }
        public IEnumerable<TLIallCivilInst> allCivilInsts { get; set; }
        public IEnumerable<TLIleg> Legs { get; set; }
    }
}
