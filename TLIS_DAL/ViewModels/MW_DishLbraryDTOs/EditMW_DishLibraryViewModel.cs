using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.MW_DishLbraryDTOs
{
    public class EditMW_DishLibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public float Weight { get; set; }
        public string dimensions { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float SpaceLibrary { get; set; }
        public float diameter { get; set; }
        public float Height { get; set; }
        public string frequency_band { get; set; }
        //public bool Active { get; set; }
        //public bool Deleted { get; set; }
        public int polarityTypeId { get; set; }
        public int asTypeId { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<DynamicAttLibViewModel> DynamicAtts { get; set; }
        //public List<DynaminAttInstViewModel> DynamicAttInst { get; set; }
    }
}
