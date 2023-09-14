using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.OrderStatusDTOs;
using TLIS_DAL.ViewModels.StepActionDTOs;

namespace TLIS_DAL.ViewModels.TicketActionDTOs
{
    public class PendingRequestViewModel
    {
        public int Id { get; set; }
        public int? AssignedToId { get; set; }
        public string Requester { get; set; }
        public string WorkFlowName { get; set; }
        public string WorkFlowType { get; set; }
        public string SiteName { get; set; }
        public string Sitecode { get; set; }
        public string NeededAction { get; set; }
        public StepActionGroupsViewModel Custody { get; set; }
        public DateTime TicketCreationDate { get; set; }
        public DateTime RequestCreationDate { get; set; }
        public OrderStatusViewModel TicketStatus { get; set; }

    }
}
