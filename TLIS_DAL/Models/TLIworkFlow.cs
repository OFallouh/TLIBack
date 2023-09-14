using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIworkFlow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        [ForeignKey("TLIsiteStatus")]
        public int SiteStatusId { get; set; }
        public TLIsiteStatus SiteStatus { get; set; }
        public List<TLIstepAction> StepActions { get; set; }
        public List<TLIworkFlowType> WorkFlowTypes { get; set; }
        public List<TLIworkFlowGroup> WorkFlowGroups { get; set; }
        public List<TLIticket> Tickets { get; set; }

    }
}
