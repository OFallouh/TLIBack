using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AgendaDTOs
{
    public class AgendaViewModel
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        //private DateTime? dateCreated { get; set; }
        public int TicketActionId { get; set; }
        public int? period { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public List<AgendaGroupViewModel> AgendaGroups { get; set; }
    }
}
