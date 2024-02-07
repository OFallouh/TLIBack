using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public enum Task_Status_Enum
    {
        Open,
        End,
        OnProgress,
        SubmitedByTLI
    }
    public class T_WF_TASK
    {
        [Key]
        public int Id { get; set; }
        public int PhaseActionId { get; set; }
        public virtual T_WF_PHASE_ACTION PhaseAction { get; set; }
        public int TicketId { get; set; }
        public virtual T_WF_TICKET Ticket { get; set; }
        public Task_Status_Enum Status { get; set; }
        public DateTime StratDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UserId { get; set; }
        public int LinkId { get; set; }
        public virtual T_WF_LINK Link { get; set; }
        public Conditions? Condition { get; set; }
        public virtual ICollection<T_WF_TASK_VALUE> WF_ACTION_TASKS { get; set; }
        public virtual ICollection<T_WF_DELEGATION> WF_DELEGATIONS { get; set; }

    }
}
