using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.LegDTOs;
using TLIS_DAL.ViewModels.SupportTypeDesignedDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegDTOs
{
    public class CivilWithLegLibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string? Note { get; set; }
        public string? Prefix { get; set; }
        public float Height_Designed { get; set; } 
        public float? Max_load_M2 { get; set; } 
        public float SpaceLibrary { get; set; } 
        public bool Active { get; set; } = false;
        public bool Deleted { get; set; } = false;
        public int? supportTypeDesignedId { get; set; }
        public string? SupportTypeDesigned_Name { get; set; }
        public int? sectionsLegTypeId { get; set; }
        public string? SectionsLegType_Name { get; set; }
        public int? structureTypeId { get; set; }
        public string? StructureType_Name { get; set; }
        public int? civilSteelSupportCategoryId { get; set; }
        public string? CivilSteelSupportCategory_Name { get; set; }
        public float Manufactured_Max_Load { get; set; } 
        public int NumberOfLegs { get; set; }
        public string? WidthVariation { get; set; }
        public IEnumerable<LegViewModel> LegsViewModel { get; set; }

    }
}
