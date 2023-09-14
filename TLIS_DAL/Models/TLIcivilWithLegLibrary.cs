using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIcivilWithLegLibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // [Index(IsUnique = true)]
        public string Model { get; set; }
        public string Note { get; set; }
        public string Prefix { get; set; }
        public float Height_Designed { get; set; }
        public float? Max_load_M2 { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public TLIsupportTypeDesigned supportTypeDesigned { get; set; }
        public int supportTypeDesignedId { get; set; } 
        public TLIsectionsLegType sectionsLegType { get; set; } 
        public int sectionsLegTypeId { get; set; } 
        public TLIstructureType structureType { get; set; }
        public int? structureTypeId { get; set; }
        public TLIcivilSteelSupportCategory civilSteelSupportCategory { get; set; } 
        public int civilSteelSupportCategoryId { get; set; }
        public float Manufactured_Max_Load { get; set; }
        public int NumberOfLegs { get; set; }   
        public IEnumerable<TLIcivilWithLegs> CivilWithLegs { get; set; }
    }
}
