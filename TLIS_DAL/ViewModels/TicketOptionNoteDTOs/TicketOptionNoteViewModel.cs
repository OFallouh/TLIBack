using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.TicketOptionNoteDTOs
{
    public class TicketOptionNoteViewModel
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int TicketActionId { get; set; }
        public int? TicketEquipmentActionId { get; set; }
        public int? StepActionOptionId { get; set; }
        public string Note { get; set; }
    }
}
