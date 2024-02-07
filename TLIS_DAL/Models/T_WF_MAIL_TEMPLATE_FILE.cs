
using System.ComponentModel.DataAnnotations.Schema;

namespace WF_API.Model
{
    public class T_WF_MAIL_TEMPLATE_FILE
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int MailTemplateId { get; set; }
        public virtual T_WF_MAIL_TEMPLATE MailTemplate { get; set; }
    }
}
