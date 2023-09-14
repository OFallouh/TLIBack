using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIticketTarget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIticket")]
        public int TicketId { get; set; }
        public TLIticket Ticket { get; set; }
        public String TargetTable { get; set; }
        public int TableId { get; set; }
    }
}
