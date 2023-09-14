using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ItemStatusDTOs;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionInsertDataViewModel
    {
        public int WorkflowId { get; set; }
        public string label { get; set; }
        public int Period { get; set; }
        public stepActionOperation? Operation { get; set; }
        public List<ListStepActionItemStatusWiNameViewModel> IncomItemStatus { get; set; }
        //public bool AllowUploadFile { get; set; }
        //public bool UploadFileIsMandatory { get; set; }
        //public List<AddStepActionGroupViewModel> StepActionFileGroup { get; set; }
        public List<AddStepActionPartViewModel> StepActionPart { get; set; }
        public StepActionGroupViewModel StepActionGroup { get; set; }
        public List<int> NextStepActions { get; set; }
    }
}
