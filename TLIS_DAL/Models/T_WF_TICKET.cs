using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public class T_WF_TICKET
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public int CreatedById { get; set; }
        public string? SiteCode { get; set; }
        public string? RegionName { get; set; }
        public string? AreaName { get; set; }
        public string? CityName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string  Status { get; set; }
        public virtual ICollection<T_WF_TASK> WF_TASKS { get; set; }
    }
}
