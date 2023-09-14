using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.LogisticalDTOs
{
    public class AddNewLogistical
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TablePartName { get; set; }
        public int LogisticalTypeId { get; set; }
    }
}
