using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.TicketDTOs
{
    public class ListTicketViewModel
    {

        public int Id { get; set; }
        public int WorkFlowId { get; set; }
        public int SiteId { get; set; }
        public DateTime CreationDate { get; set; }
        private DateTime? dateCreated;
        public int? CreatorId { get; set; }
        public int? IntegrationId { get; set; }
        public int? StatusId { get; set; }
        public int? TypeId { get; set; }
        //public List<TLIticketTarget> TicketTargets { get; set; }
        //public List<TLIticketEquipment> TicketEquipments { get; set; }
        //public List<TLIticketStep> TicketSteps { get; set; }
        //public List<TLIticketAction> Ticketactions { get; set; }
    }
}
