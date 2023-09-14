using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class StepActionWithNamesViewModel 
    {
        public int Id { get; set; }
        public int WorkflowId { get; set; }
        public int ActionId { get; set; }
        public int sequence { get; set; }
        public ActionType type { get; set; }
        public string label { get; set; }
        public int Period { get; set; }
        public stepActionOutputMode OutputMode { get; set; }
        public stepActionInputMode InputMode { get; set; }
        //public int? NextStepActionId { get; set; }
        public List<int> NextStepActions { get; set; }
        public bool AllowUploadFile { get; set; }
        public bool UploadFileIsMandatory { get; set; }
        public bool AllowNote { get; set; }
        public bool NoteIsMandatory { get; set; }
        public bool CalculateLandSpace { get; set; }
        public bool CalculateLoadSpace { get; set; }
        //public int? ItemStatusId { get; set; }
        public int? OrderStatusId { get; set; }
        public bool Active { get; set; }
        public stepActionOperation? Operation { get; set; }
        //public StepActionGroupViewModel StepActionFileGroup { get; set; }
        public StepActionGroupViewModel StepActionGroup { get; set; }
        public List<AddStepActionPartViewModel> StepActionPart { get; set; }
        public bool IsStepActionMail { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { set; get; }
        //public PermissionStepActionViewModel StepActionMailFrom { get; set; }
        public StepActionGroupsViewModel StepActionMailTo { get; set; }
        public StepActionGroupsViewModel StepActionMailCC { get; set; }
        public List<ListStepActionOptionWithNameViewModel> StepActionOption { get; set; }
        public List<ListStepActionItemOptionWithNameViewModel> StepActionItemOption { get; set; }
        //public List<AddStepActionItemStatusViewModel> ItemStatus { get; set; }
        //public string ItemStatusName { get; set; }
        public string OrderStatusName { get; set; }
        //public List<ListStepActionItemStatusViewModel> IncomItemStatus { get; set; }
        public List<ListStepActionItemStatusWiNameViewModel> IncomItemStatus { get; set; }
        //public List<ListStepActionPartViewModel> StepActionPart { get; set; }
        //public List<ItemOrderStatusNameViewModel> StepActionOptionName { get; set; }
        //public List<ItemOrderStatusNameViewModel> StepActionItemOptionName { get; set; }


    }
}
