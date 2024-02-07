using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public class T_WF_TASK_VALUE
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
        public int FormElementId { get; set; }
        public virtual T_WF_FORM_ELEMENT FormElement { get; set; }
        public int TaskId { get; set; }
        public virtual T_WF_TASK Task { get; set; }

    }
}
