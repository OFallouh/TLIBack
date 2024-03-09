using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilSiteDateDTOs;
using TLIS_DAL.ViewModels.CivilSupportDistanceDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.LegDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using TLIS_DAL.ViewModels.LogisticalitemDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegDTOs
{
    public class AddCivilWithLegLibraryViewModel
    {
        public string Model { get; set; }
        public string Note { get; set; }
        public string Prefix { get; set; }
        public float? Height_Designed { get; set; }
        public float? Max_load_M2 { get; set; }
        public float? SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public int supportTypeDesignedId { get; set; }
        public int sectionsLegTypeId { get; set; }
        public int structureTypeId { get; set; }
        public int civilSteelSupportCategoryId { get; set; }
        public float Manufactured_Max_Load { get; set; }
        public int NumberOfLegs { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddLegViewModel> LegsViewModel { get; set; }
        public List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue { get; set; }
    }
      
}
