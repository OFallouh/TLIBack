using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.OrderStatusDTOs
{
    public class OrderStatusAddViewModel
    {
        public string Name { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsFinish { get; set; }
        //public bool Deleted { get; set; }
        //public DateTime? DateDeleted { get; set; }
    }
}
