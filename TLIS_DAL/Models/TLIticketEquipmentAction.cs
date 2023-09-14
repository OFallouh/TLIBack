using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIticketEquipmentAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIticket")]
        public int TicketId { get; set; }
        public TLIticketAction TicketAction { get; set; }
        [ForeignKey("TLIticketEquipment")]
        public int TicketEquipmentId { get; set; }
        public TLIticketEquipment TicketEquipment { get; set; }
        [ForeignKey("TLIuser")]
        public int ExecuterId { get; set; }
        public TLIuser Executer { get; set; }
        public DateTime? ExecutionDate { get; set; }
        [ForeignKey("TLIitemStatus")]
        public int ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public equipmentProposal ProposalType { get; set; }
        public string Proposal { get; set; }
        //public string FileName { get; set; }
        //public string MIMEType { get; set; }
        /// <summary>
        /// this column has to be BLOB type or something like that
        /// </summary>
        //public byte[] FileContent { get; set; }
        public List<TLIticketOptionNote> TicketOptionNotes { get; set; }
    }
}
