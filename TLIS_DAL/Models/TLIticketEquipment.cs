using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public enum equipmentOperation
    {
        /// <summary>
        /// remove it from current location
        /// </summary>
        Dismantle,
        /// <summary>
        /// put it on this location
        /// </summary>
        Install
    }
    public enum equipmentProposal
    {
        /// <summary>
        /// there is no proposal for this item
        /// </summary>
        NoProposal,
        /// <summary>
        /// this item needs an action from requester side
        /// </summary>
        RequesterAction,
        /// <summary>
        /// Civil requests a new study for this item
        /// </summary>
        NewStudy
    }

    public class TLIticketEquipment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIticket")]
        public int TicketId { get; set; }
        public TLIticket Ticket { get; set; }
        [ForeignKey("TLIequipment")]
        public int EquipemntId { get; set; }
        public TLIequipment Equipemnt { get; set; }
        [ForeignKey("TLIitemStatus")]
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        [ForeignKey("TLIticketTarget")]
        public int? TicketTargetId { get; set; }
        public TLIticketTarget TicketTarget { get; set; }
        public equipmentOperation Operation { get; set; }
        public equipmentProposal Proposal { get; set; }
        public List<TLIticketEquipmentAction> TicketEqupmentActions { get; set; }
    }
}
