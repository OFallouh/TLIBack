using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.TicketTargetDTOs
{
    public class TicketTargetViewModel
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public String TargetTable { get; set; }
        public int TableId { get; set; }
    }
}
