using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIticket
    {

        //[DefaultValue(true)]

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIworkFlow")]
        public int WorkFlowId { get; set; }
        public TLIworkFlow WorkFlow { get; set; }
        [ForeignKey("TLIsite")]
        public string SiteCode { get; set; }
        public TLIsite Site { get; set; }
        public DateTime CreationDate
        {
            get
            {
                return this.dateCreated.HasValue
                   ? this.dateCreated.Value
                   : DateTime.Now;
            }

            set { this.dateCreated = value; }
        }
        private DateTime? dateCreated;
        //*
        [ForeignKey("TLIuser")]
        public int? CreatorId { get; set; }
        public TLIuser Creator { get; set; }
        [ForeignKey("TLIintegration")]
        public int? IntegrationId { get; set; }
        public TLIintegration Integration { get; set; }
        //*/
        /*
        [ForeignKey("TLIticketCreator")]
        public int TicketCreatorId { get; set; }
        public TLIticketCreator TicketCreator { get; set; }
        //*/
        [ForeignKey("TLIorderStatus")]
        public int? StatusId { get; set; }
        public TLIorderStatus Status { get; set; }
        [ForeignKey("TLIworkFlowType")]
        public int? TypeId { get; set; }
        public TLIworkFlowType Type { get; set; }
        public List<TLIticketTarget> TicketTargets { get; set; }
        public List<TLIticketEquipment> TicketEquipments { get; set; }
        public List<TLIticketStep> TicketSteps { get; set; }
        public List<TLIticketAction> Ticketactions { get; set; }
    }
}
