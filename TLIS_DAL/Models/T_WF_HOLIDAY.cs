using System;
using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public class T_WF_HOLIDAY
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name between 3 and 50 characters")]
        public string Name { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public T_WF_HOLIDAY() { }
        
    }
}
