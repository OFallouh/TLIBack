using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIorderStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        [DefaultValue(false)]
        public bool IsDefault { get; set; }
        public bool IsFinish { get; set; }
        public List<TLIstepAction> StepActions { get; set; }
        public List<TLIstepActionOption> StepActionOptions { get; set; }
        public List<TLIstepActionItemOption> StepActionItemOptions { get; set; }
        public List<TLIticket> Tickets { get; set; }
    }
}
