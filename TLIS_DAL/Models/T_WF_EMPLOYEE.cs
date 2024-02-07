using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public class T_WF_EMPLOYEE
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name between 3 and 50 characters")]
        public string UserName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name between 3 and 50 characters")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name between 3 and 50 characters")]
        public string LastName { get; set; }

        public string Mobile { get; set; }
        public string Title { get; set; }

        [ForeignKey("WF_DEPARTEMENT")]
        public int DepId { get; set; }

        [ForeignKey("WF_ESCALATION_LEVEL")]

        public int LevelId { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public virtual T_WF_DEPARTEMENT WF_DEPARTEMENT { get; set; }

        public virtual T_WF_ESCALATION_LEVEL WF_ESCALATION_LEVEL { get; set; }

       
    }
}
