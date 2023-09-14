using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using TLIS_DAL.ViewModels.LogisticalitemDTOs;

namespace TLIS_DAL.ViewModels.CivilWithoutLegDTOs
{
   public class AddCivilWithoutLegLibraryViewModel
    {
        public string Model { get; set; }
        public string Note { get; set; }
        public float Height_Designed { get; set; }
        public float HeightBase { get; set; }
        public float? Max_Load { get; set; }
        public float SpaceLibrary { get; set; }
        public string Prefix { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public int CivilSteelSupportCategoryId { get; set; }
        public int? InstallationCivilwithoutLegsTypeId { get; set; }
        public int? CivilWithoutLegCategoryId { get; set; }
        public float Manufactured_Max_Load { get; set; }
        public int ? structureTypeId { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue { get; set; }
        //public List<AddDynamicAttInstViewModel> DynamicAttsInst { get; set; }
    }
}
