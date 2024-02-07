using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public enum Conditions
    {
        Yes,
        No,
        Any,
    }
    public class T_WF_CONDITION
    {
        [Key]
        public int Id { get; set; }
        public int PhaseActionId { get; set; }
        public virtual T_WF_PHASE_ACTION PhaseAction { get; set; }
        public int LinkId { get; set; }
        public virtual T_WF_LINK Link { get; set; }
        public Conditions Condition { get; set; }
    }
}
