using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
      public class TLIcivilWithoutLegLibrary
      {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // [Index(IsUnique = true)]
        public string Model { get; set; }

        public string? Note { get; set; }
        public float Height_Designed { get; set; }
        public float Max_Load { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public float HeightBase { get; set; }
        public string? Prefix { get; set; }
        public TLIstructureType? structureType { get; set; }
        public int? structureTypeId { get; set; }             
        [ForeignKey("TLIcivilSteelSupportCategory")]
        public int? CivilSteelSupportCategoryId { get; set; }
        public TLIcivilSteelSupportCategory? CivilSteelSupportCategory { get; set; }
        [ForeignKey("TLIInstCivilwithoutLegsType")]
        public int? InstCivilwithoutLegsTypeId { get; set; }
        public TLIInstCivilwithoutLegsType? InstCivilwithoutLegsType { get; set; }
        [ForeignKey("TLIcivilWithoutLegCategory")]
        public int CivilWithoutLegCategoryId { get; set; }
        public float Manufactured_Max_Load { get; set; }

        public TLIcivilWithoutLegCategory? CivilWithoutLegCategory { get; set; }
        public IEnumerable<TLIcivilWithoutLeg> civilWithoutLegs { get; set; }

    }
}
