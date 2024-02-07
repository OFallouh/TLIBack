using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public class T_WF_DEPARTEMENT
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name between 3 and 50 characters")]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<T_WF_EMPLOYEE> WF_EMPLOYEES { get; set; }
       
    }
}
