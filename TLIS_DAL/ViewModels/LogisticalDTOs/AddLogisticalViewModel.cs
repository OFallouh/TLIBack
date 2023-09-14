using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.LogisticalDTOs
{
    public class AddLogisticalViewModel
    {
        public int? VendorId { get; set; }
        public int? SupplierId { get; set; }
        public int? DesignerId { get; set; }
        public int? ManufacturerId { get; set; }
    }
}
