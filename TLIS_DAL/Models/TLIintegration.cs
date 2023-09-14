using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIintegration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        public List<TLIworkFlowGroup> WorkFlowGroups { get; set; }
        public List<TLIstepActionGroup> StepActionGroups { get; set; }
        //public List<TLIstepActionMail> StepActionMailFrom { get; set; }
        //public List<TLIstepActionMail> StepActionMailTo { get; set; }
        public List<TLIstepActionMail> StepActionMailFrom { get; set; } // both of from and to
        public List<TLIstepActionMailTo> StepActionMailTo { get; set; } // both of from and to
        public List<TLIstepActionPartGroup> StepActionPartGroup { get; set; }
        public List<TLIstepActionFileGroup> StepActionFileGroup { get; set; }
        public List<TLIagendaGroup> AgendaGroups { get; set; }
    }
}
