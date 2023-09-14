using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.SolarLibraryDTOs
{
    public class AddSolarLibraryViewModel
    {
        public string Model { get; set; }
        public float? Weight { get; set; }
        public string TotaPanelsDimensions { get; set; }
        public string StructureDesign { get; set; }
        public string LayoutCode { get; set; }
        public float? HeightFromFront { get; set; }
        public float? HeightFromBack { get; set; }
        public string BasePlateDimension { get; set; }
        public float SpaceLibrary { get; set; }
        public int? CapacityId { get; set; } = 0;
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue { get; set; }
        //public List<AddDynamicAttInstViewModel> DynamicAttsInst { get; set; }
    }
}
