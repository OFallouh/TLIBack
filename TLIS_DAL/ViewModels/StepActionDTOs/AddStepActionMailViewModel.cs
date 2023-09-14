using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionMailViewModel
    {
        public int WorkflowId { get; set; }
        public int Period { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { set; get; }
        public PermissionStepActionViewModel StepActionMailFromId { get; set; }
        public List<AddStepActionMailToViewModel> StepActionMailTo { get; set; }
        public List<int> NextStepActions { get; set; }
    }
}
