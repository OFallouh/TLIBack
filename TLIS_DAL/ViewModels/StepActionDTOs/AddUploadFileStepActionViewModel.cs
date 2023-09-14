using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddUploadFileStepActionViewModel
    {
        public int WorkflowId { get; set; }
        public string label { get; set; }
        public int Period { get; set; }
        //public bool AllowUploadFile { get; set; }
        //public bool UploadFileIsMandatory { get; set; }
        //public List<AddStepActionGroupViewModel> StepActionFileGroup { get; set; }
        public StepActionGroupViewModel StepActionGroup { get; set; }
        public List<AddStepActionPartFileViewModel> StepActionPart { get; set; }
        public List<int> NextStepActions { get; set; }
    }
}
