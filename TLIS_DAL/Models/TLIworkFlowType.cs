using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIworkFlowType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("TLIworkFlow")]
        public int WorkFlowId { get; set; }
        public TLIworkFlow WorkFlow { get; set; }
        [ForeignKey("TLIstepAction")]
        public int? nextStepActionId { get; set; }
        public TLIstepAction nextStepAction { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public List<TLIticket> Tickets { get; set; }
    }
}
