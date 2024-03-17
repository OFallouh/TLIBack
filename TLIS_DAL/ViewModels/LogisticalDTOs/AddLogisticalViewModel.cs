using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.LogisticalDTOs
{
    public class AddLogisticalViewModel
    {
        public int Vendor { get; set; }
        public int? Supplier { get; set; }
        public int? Designer { get; set; }
        public int? Manufacturer { get; set; }
    }
}
