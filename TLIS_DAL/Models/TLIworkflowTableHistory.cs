using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIworkflowTableHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int RecordId { get; set; }     
        public int TablesNameId { get; set; }
        public TLItablesNames TablesName { get; set; }
        [Required]
        public int UserId { get; set; }
        public TLIuser User { get; set; }

        [Required]
        public int TicketId { get; set; }
        public TLIticket Ticket { get; set; }
        [Required]
        public int TicketActionId { get; set; }
        public TLIticketAction TicketAction { get; set; }
        public int ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int PartId { get; set; }
        public TLIpart Part { get; set; }
        public int? PreviousHistoryId { get; set; }
        public TLIworkflowTableHistory PreviousHistory { get; set; }
      
        public int? HistoryTypeId { get; set; }
        public TLIhistoryType HistoryType { get; set; }
        [Required]
        public DateTime Date { get; set; }
        //add attributes target, proposal , propsal Type,Operation 

        public IEnumerable<TLIhistoryDetails> historyDetails { get; set; }

        public stepActionOperation? Operation { get; set; }

    }
}
