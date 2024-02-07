using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public class T_WF_TEMPLATE
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int VersionNum { get; set; }
        public int? EscalationMailTemplateId { get; set; }
        public virtual T_WF_MAIL_TEMPLATE? EscalationMailTemplate { get; set; }
        public int? ReminderMailTemplateId { get; set; }
        public virtual T_WF_MAIL_TEMPLATE? ReminderMailTemplate { get; set; }
        public virtual ICollection<T_WF_PHASE> WF_PHASE { get; set; }
        public bool IsDraft { get; set; }
        public bool IsActive { get; set; }
        public bool IsTesting { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLive { get; set; }
        public int? ReminderAfter { get; set; }
        public int? EscalationLevels { get; set; }
        public int Duration { get; set; }

    }
}
