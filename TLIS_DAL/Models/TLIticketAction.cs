using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIticketAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIticket")]
        public int TicketId { get; set; }
        public TLIticket Ticket { get; set; }
        [ForeignKey("TLIticket")]
        public int StepActionId { get; set; }
        public TLIstepAction StepAction { get; set; }
        [ForeignKey("TLIuser")]
        public int? ExecuterId { get; set; }
        public TLIuser Executer { get; set; }
        public DateTime? ExecutionDate { get; set; }
        [ForeignKey("TLIuser")]
        public int? AssignedToId { get; set; }
        public TLIuser AssignedTo { get; set; }
        //public string FileName { get; set; }
        //public string MIMEType { get; set; }
        /// <summary>
        /// this column has to be BLOB type or something like that
        /// </summary>
        //public byte[] FileBody { get; set; }
        public List<TLIticketEquipmentAction> TicketEquipmentActions { get; set; }
        public List<TLIticketOptionNote> TicketOptionNotes { get; set; }
        public List<TLIagenda> Agenda { get; set; }

    }
}
