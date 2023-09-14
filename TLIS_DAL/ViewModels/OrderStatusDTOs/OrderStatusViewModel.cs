using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.OrderStatusDTOs
{
    public class OrderStatusViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsFinish { get; set; }
        //public bool Deleted { get; set; }
        //public DateTime? DateDeleted { get; set; }
        //public List<TLIstepAction> StepActions { get; set; }
        //public List<TLIstepActionOption> StepActionOptions { get; set; }
        //public List<TLIstepActionItemOption> StepActionItemOptions { get; set; }
        //public List<TLIticket> Tickets { get; set; }
    }
}
