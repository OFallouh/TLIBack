using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TLIS_DAL.Models
{

    /// <summary>
    /// detect which allowed status for itme to be process through this action
    /// </summary>
    public class TLIstepActionIncomeItemStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [ForeignKey("TLIstepAction")]
        public int StepActionId { get; set; }
        public TLIstepAction StepAction { get; set; }
        [ForeignKey("TLIitemStatus")]
        public int ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }

    }
}
