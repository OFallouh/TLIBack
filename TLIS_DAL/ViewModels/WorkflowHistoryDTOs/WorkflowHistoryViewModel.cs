using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.WorkflowHistoryDTOs
{
    public class WorkflowHistoryViewModel
    {
       
        public int Id { get; set; }
        [Required]
        public int RecordId { get; set; }
        public int TablesNameId { get; set; }
        public TLItablesNames TablesName { get; set; }

        public int UserId { get; set; }

        public int TicketId { get; set; }
     
        public int TicketActionId { get; set; }
     
        public int ItemStatusId { get; set; }
        public int PartId { get; set; }    
        public int? PreviousHistoryId { get; set; }
       
        public int? HistoryTypeId { get; set; }
        public DateTime Date { get; set; }

       
    }
}
