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
    }
}
