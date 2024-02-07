using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public class T_WF_FORM_ELEMENT
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
        public int ColSpan { get; set; }
        public string Label { get; set; }
        public string? Hint { get; set; }
        //public bool IsDeleted { get; set; }
        public int ActionId { get; set; }
        public bool IsReturn { get; set; }
        public virtual T_WF_ACTION Action { get; set; }
        public virtual ICollection<T_WF_TASK_VALUE> WF_PHASE_ACTION_TICKET_VALUES { get; set; }

    }
}
