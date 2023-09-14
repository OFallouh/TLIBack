using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIticketOptionNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIticket")]
        public int TicketId { get; set; }
        public TLIticket Ticket { get; set; }
        [ForeignKey("TLIticketAction")]
        public int TicketActionId { get; set; }
        public TLIticketAction TicketAction { get; set; }
        [ForeignKey("TLIticketEquipmentAction")]
        public int? TicketEquipmentActionId { get; set; }
        public TLIticketEquipmentAction TicketEquipmentAction { get; set; }
        [ForeignKey("TLIstepActionOption")]
        public int? StepActionOptionId { get; set; }
        public TLIstepActionOption StepActionOption { get; set; }
        public string Note { get; set; }
    }
}
