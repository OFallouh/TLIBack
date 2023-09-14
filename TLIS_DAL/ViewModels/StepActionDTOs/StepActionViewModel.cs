using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class StepActionViewModel
    {
        public int Id { get; set; }
        public int WorkflowId { get; set; }
        public int ActionId { get; set; }
        public int sequence { get; set; }
        public int Period { get; set; }
        public ActionType type { get; set; }
        public string label { get; set; }
        public stepActionOutputMode OutputMode { get; set; }
        public stepActionInputMode InputMode { get; set; }
        public stepActionOperation? Operation { get; set; }
        public int? NextStepActionId { get; set; }
        public bool AllowUploadFile { get; set; }
        public bool UploadFileIsMandatory { get; set; }
        public bool AllowNote { get; set; }
        public bool NoteIsMandatory { get; set; }
        //public int? ItemStatusId { get; set; }
        public int? OrderStatusId { get; set; }
        public bool Active { get; set; }
        public bool IsStepActionMail { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { set; get; }
        public int? StepActionMailFromId { get; set; }
        public List<TLIstepActionIncomeItemStatus> IncomItemStatus { get; set; }
        public List<TLIworkFlowType> WorkFlowTypes { get; set; }
        public List<TLIstepActionFileGroup> StepActionFileGroup { get; set; }
        public List<TLIstepActionGroup> StepActionGroup { get; set; }
        public List<TLIstepActionPart> StepActionPart { get; set; }
        public List<TLIstepActionMailTo> StepActionMailTo { get; set; }
        public List<TLIticketAction> TicketActions { get; set; }
        public List<int> NextStepActions { get; set; }
        //public List<TLIstepActionItemStatus> ItemStatus { get; set; }
    }
}
