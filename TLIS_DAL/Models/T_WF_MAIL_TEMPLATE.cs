using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WF_API.Model
{
    public enum Mail_Type_Enum
    {
        EscalationMailTemplate,
        ReminderMailTemplate,
        NotificationMailTemplate
    }
    public class T_WF_MAIL_TEMPLATE
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "CLOB")]
        public string Template { get; set; }
        public string Subject { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Mail_Type_Enum MailType { get; set; }
        public virtual ICollection<T_WF_TEMPLATE> REMINDER_TEMPLATES { get; set; }
        public virtual ICollection<T_WF_TEMPLATE> ESCALATION_TEMPLATES { get; set; }
        public virtual ICollection<T_WF_MAIL_TEMPLATE_FILE> WF_MAIL_TEMPLATE_FILES { get; set; }
       
    }
}
