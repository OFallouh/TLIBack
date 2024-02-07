using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WF_API.Model
{
    public class T_WF_PHASE_ACTION
    {
        [Key]
        public int Id { get; set; }
        public int ActionId { get; set; }
        public virtual T_WF_ACTION Action { get; set; }
        public int PhaseId { get; set; }
        public virtual T_WF_PHASE Phase { get; set; }
        public int? UserAssignToId { get; set; }
        public int? GroupAssignToId { get; set; }
        public int? ExternalSysAssignToId { get; set; }
        public virtual ICollection<T_WF_TASK> WF_TASKS { get; set; }
        public virtual ICollection<T_WF_CONDITION> WF_CONDITIONS { get; set; }



    }
}
