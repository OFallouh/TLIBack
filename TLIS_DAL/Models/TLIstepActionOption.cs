using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TLIS_DAL.Models
{
    /// <summary>
    /// contains all allowed options to be implemented on stepAction
    /// could be belong to a stepaction or a parent stepActionOption
    /// </summary>
    public class TLIstepActionOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [ForeignKey("TLIstepAction")]
        public int StepActionId { get; set; }
        public TLIstepAction StepAction { get; set; }
        public List<TLInextStepAction> NextStepActions { get; set; }
        /*
        [ForeignKey("TLIstepAction")]
        public int? NextStepActionId { get; set; }
        [NotMapped]
        public TLIstepAction NextStepAction { get; set; }
        //*/
        [ForeignKey("TLIactionOption")]
        public int ActionOptionId { get; set; }
        public TLIactionOption ActionOption { get; set; }
        //*
        [ForeignKey("TLIitemStatus")]
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        //*/
        [ForeignKey("TLIorderStatus")]
        public int? OrderStatusId { get; set; }
        public TLIorderStatus OrderStatus { get; set; }
        public bool AllowNote { get; set; }
        public bool NoteIsMandatory { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public List<TLIticketOptionNote> TicketOptionNotes { get; set; }
    }
}
