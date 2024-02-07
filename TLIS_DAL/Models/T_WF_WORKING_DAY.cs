using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public class T_WF_WORKING_DAY
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name between 3 and 50 characters")]
        public string Name { get; set; }
        public bool IsHoliday { get; set; }
        public int? StartWorkTime { get; set; }
        public int? EndWorkTime { get; set; }
    }
}
