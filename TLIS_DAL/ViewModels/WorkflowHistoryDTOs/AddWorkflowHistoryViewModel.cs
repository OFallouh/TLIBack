using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.WorkflowHistoryDTOs
{
    public class AddWorkflowHistoryViewModel
    {
       
        public int RecordId { get; set; }
        public int TablesNameId { get; set; }
      

        public int UserId { get; set; }

        public int TicketId { get; set; }

        public int TicketActionId { get; set; }
        public int CivilSupportTargetID { get; set; }
        public string Proposal { get; set; }
        public int ItemStatusId { get; set; }
        public int PartId { get; set; }
        public int? PreviousHistoryId { get; set; }

        public int? HistoryTypeId { get; set; }
        public DateTime Date { get; set; }
        public List<Tuple<string, string, string>> values { get; set; } = null;

       
    }
}
