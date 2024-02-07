using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WF_API.Model
{
    public class T_WF_PHASE
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<T_WF_PHASE_ACTION> WF_PHASE_ACTIONS { get; set; }
        public int TemplateId { get; set; }
        public virtual T_WF_TEMPLATE Template { get; set; }
        public virtual ICollection<T_WF_LINK> WF_LINKCURRENTS { get; set; }
        public virtual ICollection<T_WF_LINK> WF_LINKNEXTS { get; set; }
       
        
    }
   
}