using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ItemStatusDTOs;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionViewModel
    {




        public int WorkflowId { get; set; }
        public int ActionId { get; set; }
        public ActionType type { get; set; }
        public string label { get; set; }
        public int Period { get; set; }
        public int sequence { get; set; }
        public stepActionOutputMode OutputMode { get; set; }
        public stepActionInputMode InputMode { get; set; }
        public bool AllowUploadFile { get; set; }
        public bool UploadFileIsMandatory { get; set; }
        public bool AllowNote { get; set; }
        public bool NoteIsMandatory { get; set; }
        public bool CalculateLandSpace { get; set; }
        public bool CalculateLoadSpace { get; set; }
        //public int? ItemStatusId { get; set; }
        public int? OrderStatusId { get; set; }
        public stepActionOperation? Operation { get; set; }
        public List<ListItemStatusViewModel> IncomItemStatus { get; set; }
        public bool IsStepActionMail { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { set; get; }
        //public PermissionStepActionViewModel StepActionMailFrom { get; set; }
        //public List<AddStepActionMailToViewModel> StepActionMailTo { get; set; }
        public StepActionGroupsViewModel StepActionMailTo { get; set; }
        public StepActionGroupsViewModel StepActionMailCC { get; set; }
        //public StepActionGroupViewModel StepActionFileGroup { get; set; }
        //public StepActionGroupsViewModel StepActionFileGroup { get; set; }
        public StepActionGroupViewModel StepActionGroup { get; set; }
        public List<AddStepActionPartViewModel> StepActionPart { get; set; }
        public List<ListStepActionOptionViewModel> StepActionOption { get; set; }
        public List<AddStepActionItemOptionViewModel> StepActionItemOption { get; set; }
        //public List<AddStepActionItemStatusViewModel> ItemStatus { get; set; }
        public List<int> NextStepActions { get; set; }
        //public int? NextStepActionId { get; set; }






    }
}
