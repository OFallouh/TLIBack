using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TLIS_DAL.Models
{

    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class TLIstepActionItemOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIstepAction")]
        public int StepActionId { get; set; }
        public TLIstepAction StepAction { get; set; }
        [ForeignKey("TLIactionItemOption")]
        public int ActionItemOptionId { get; set; }
        public TLIactionItemOption ActionItemOption { get; set; }
        /*
        [ForeignKey("TLIstepAction")]
        public int? NextStepActionId { get; set; }
        [NotMapped]
        public TLIstepAction NextStepAction { get; set; }
        //*/
        /*
        [ForeignKey("TLIitemStatus")]
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        //*/
        public List<TLInextStepAction> NextStepActions { get; set; }
        public List<TLIstepActionItemStatus> StepActionItemStatus { get; set; }
        [ForeignKey("TLIorderStatus")]
        public int? OrderStatusId { get; set; }
        public TLIorderStatus OrderStatus { get; set; }
        public bool? AllowNote { get; set; }
        public bool? NoteIsMandatory { get; set; }
    }
}
