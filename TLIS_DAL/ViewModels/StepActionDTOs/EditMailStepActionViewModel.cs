using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class EditMailStepActionViewModel
    {
        public int Id { get; set; }
        //public int WorkflowId { get; set; }
        public int Period { get; set; }
        public string label { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { set; get; }
        //public PermissionStepActionViewModel StepActionMailFrom { get; set; }
        public StepActionGroupsViewModel StepActionMailTo { get; set; }
        public StepActionGroupsViewModel StepActionMailCC { get; set; }
        public List<int> NextStepActions { get; set; }
    }
}
