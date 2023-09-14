using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionPartViewModel: AddStepActionPartFileViewModel
    {
        public bool AllowUploadFile { get; set; }
        public bool UploadFileIsMandatory { get; set; }
    }
}
