using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TLIS_DAL.Models
{

    /// <summary>
    /// execute this action output on one equipment or all of them
    /// </summary>
    public enum stepActionOutputMode
    {
        /// <summary>
        /// here we can execute each equipement on its own
        /// </summary>
        single,
        /// <summary>
        /// all equipemtns will handeled by one shot
        /// </summary>
        entire
    }

    /// <summary>
    /// buffer input for one equipment or all of them
    /// </summary>
    public enum stepActionInputMode
    {
        /// <summary>
        /// recieve equipment without waiting for the rest
        /// </summary>
        stochastic,
        /// <summary>
        /// revcieve all equipemtns together
        /// </summary>
        batch
    }

    /// <summary>
    /// type of operation to be user later on in next actions, betterto reflect this value to ticket its self
    /// </summary>
    public enum stepActionOperation
    {
        /// <summary>
        /// add new items
        /// </summary>
        add,
        /// <summary>
        /// dismantel an exist item
        /// </summary>
        dismantel,
        /// <summary>
        /// both: add and dismintal
        /// </summary>
        both
    }


    public enum stepActionType
    {
        /// <summary>
        /// how options to choose one of them
        /// </summary>
        Conditional,
        /// <summary>
        /// send an email
        /// </summary>
        Mail,
        /// <summary>
        /// insert data regarding a site
        /// </summary>
        InsertData
    }



    /// <summary>
    /// actions belong to a step
    /// </summary>
    public class TLIstepAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIworkFlow")]
        public int WorkflowId { get; set; }
        public TLIworkFlow Workflow { get; set; }
        [ForeignKey("TLIaction")]
        public int ActionId { get; set; }
        public TLIaction Action { get; set; }
        public ActionType type { get; set; }
        public string label { get; set; }
        public int sequence { get; set; }
        public int Period { get; set; }
        public stepActionOutputMode OutputMode { get; set; }
        public stepActionInputMode InputMode { get; set; }
        /*
        [ForeignKey("TLIstepAction")]
        public int? NextStepActionId { get; set; }
        public TLIstepAction NextStepAction { get; set; }
        //*/
        /*
        [ForeignKey("TLIitemStatus")]
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        //*/
        [ForeignKey("TLIorderStatus")]
        public int? OrderStatusId { get; set; }
        public TLIorderStatus OrderStatus { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public stepActionOperation? Operation { get; set; }
        public bool AllowNote { get; set; }
        public bool NoteIsMandatory { get; set; }
        public bool AllowUploadFile { get; set; }
        public bool UploadFileIsMandatory { get; set; }
        public bool CalculateLandSpace { get; set; }
        public bool CalculateLoadSpace { get; set; }
        public List<TLInextStepAction> NextStepActions { get; set; }
        public List<TLIstepActionFileGroup> StepActionFileGroup { get; set; }
        public List<TLIstepActionGroup> StepActionGroup { get; set; }
        public List<TLIstepActionPart> StepActionPart { get; set; }
        public List<TLIstepActionIncomeItemStatus> IncomItemStatus { get; set; }
        public List<TLIworkFlowType> WorkFlowTypes { get; set; }
        public List<TLIstepActionOption> StepActionOption { get; set; }
        public List<TLIstepActionItemOption> StepActionItemOption { get; set; }
        public bool IsStepActionMail { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { set; get; }
      //  [ForeignKey("TLImailTemplate")]
      //  public int? MailTemplateId { get; set; }
      //  public TLImailTemplate MailTemplate { get; set; }
        [ForeignKey("TLIstepActionMail")]
        public int? StepActionMailFromId { get; set; }
        public TLIstepActionMail StepActionMailFrom { get; set; }
        public List<TLIstepActionMailTo> StepActionMailTo { get; set; }
        public List<TLIticketAction> TicketActions { get; set; }
        //public List<TLIstepActionItemStatus> ItemStatus { get; set; }
    }
}
