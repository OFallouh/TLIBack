using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.LogisticalTypeDTOs
{
    public class LogisticalTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
