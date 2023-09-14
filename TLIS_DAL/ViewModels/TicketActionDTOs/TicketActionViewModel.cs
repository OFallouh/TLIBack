using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.TicketActionDTOs
{
    public class TicketActionViewModel
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int StepActionId { get; set; }
        public int? ExecuterId { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public int? AssignedToId { get; set; }
        //public List<TLIticketEquipmentAction> TicketEquipmentActions { get; set; }
        //public List<TLIticketOptionNote> TicketOptionNotes { get; set; }
        //public List<TLIagenda> Agenda { get; set; }
    }
}
