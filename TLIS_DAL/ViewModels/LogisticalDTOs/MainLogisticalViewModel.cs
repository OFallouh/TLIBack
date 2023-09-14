using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.LogisticalDTOs
{
    public class MainLogisticalViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string tablePartName_Name { get; set; }
        public string logisticalType_Name { get; set; }
        public bool Active { get; set; }
    }
}
