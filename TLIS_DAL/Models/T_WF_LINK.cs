using System.Collections.Generic;

namespace WF_API.Model
{
    
    public class T_WF_LINK
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CurrentPhaseId { get; set; }
        public virtual T_WF_PHASE CurrentPhase { get; set; }
        public int? NextPhaseId { get; set; }
        public virtual T_WF_PHASE NextPhase { get; set; }
        public virtual ICollection<T_WF_TASK> WF_TASKS { get; set; }
        public virtual ICollection<T_WF_CONDITION> WF_CONDITIONS { get; set; }

   }
}
