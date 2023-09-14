using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.TicketDTOs
{
    public class AddTicketViewModel
    {
        public int WorkFlowId { get; set; }
        public string SiteCode { get; set; }
        //private DateTime? dateCreated;
        public int? TypeId { get; set; }
    }
}
