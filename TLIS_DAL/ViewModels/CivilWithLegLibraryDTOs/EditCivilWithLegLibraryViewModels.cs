using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LegDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using TLIS_DAL.ViewModels.LogisticalitemDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegDTOs
{
    public class EditCivilWithLegLibraryViewModels
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Note { get; set; }
        public string Prefix { get; set; }
        public float Height_Designed { get; set; }
        public float? Max_load_M2 { get; set; }
       // public bool Active { get; set; }
       // public bool Deleted { get; set; }
        public int supportTypeDesignedId { get; set; }
        public int sectionsLegTypeId { get; set; }
        public float SpaceLibrary { get; set; }
        public int structureTypeId { get; set; }
        public int civilSteelSupportCategoryId { get; set; }
        // public IEnumerable<EditLegViewModel> LegsViewModel { get; set; }
        // public List<DynamicAttLibViewModel> DynamicAtts { get; set; }
        // public List<DynaminAttInstViewModel> DynamicAttInst { get; set; }
        public float Manufactured_Max_Load { get; set; }
        public int NumberOfLegs { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<EditLegViewModel> LegsViewModel { get; set; }
        public List<DynamicAttLibViewModel> DynamicAtts { get; set; }
    }
  
}
