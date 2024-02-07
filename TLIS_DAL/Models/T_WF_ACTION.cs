using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public class T_WF_ACTION
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Colspan { get; set; }
        public string? PageUrl { get; set; }
        public string? Description { get; set; }
        public int Duration { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool IsDraft { get; set; }
        public virtual T_WF_META_LINK MetaLink { get; set; }
        public virtual ICollection<T_WF_PHASE_ACTION> WF_PHASE_ACTIONS { get; set; }
        public virtual ICollection<T_WF_FORM_ELEMENT> FORMELEMENTS { get; set; }
        public T_WF_ACTION() { }
    }
}
