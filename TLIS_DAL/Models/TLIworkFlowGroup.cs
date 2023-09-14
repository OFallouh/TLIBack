using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIworkFlowGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIworkFlow")]
        public int WorkFlowId { get; set; }
        public TLIworkFlow WorkFlow { get; set; }
        [ForeignKey("TLIactor")]
        public int? ActorId { get; set; }
        public TLIactor Actor { get; set; }
        [ForeignKey("TLIintegration")]
        public int? IntegrationId { get; set; }
        public TLIintegration Integration { get; set; }
        [ForeignKey("TLIuser")]
        public int? UserId { get; set; }
        public TLIuser User { get; set; }
        [ForeignKey("TLIgroup")]
        public int? GroupId { get; set; }
        public TLIgroup Group { get; set; }
    }
}
