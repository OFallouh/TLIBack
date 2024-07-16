using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.LogisticalDTOs
{
    public class AddLogisticalViewModel
    {
        [Required]
        public int Vendor { get; set; }
        public int? Supplier { get; set; }
        public int? Designer { get; set; }
        public int? Manufacturer { get; set; }
        public int? Consultant { get; set; }
        public int? Contractor { get; set; }
    }
    public class AddLogisticalViewModels
    {
        public int? Vendor { get; set; }
        public int? Supplier { get; set; }
        public int? Designer { get; set; }
        public int? Manufacturer { get; set; }
        public int? Consultant { get; set; }
        public int? Contractor { get; set; }
    }
    public class AddLogisticalViewModelSolar
    {
        public int? Vendor { get; set; }
        public int? Supplier { get; set; }
        public int Designer { get; set; }
        public int? Manufacturer { get; set; }
        public int? Consultant { get; set; }
        public int? Contractor { get; set; }
    }
}
