using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class ListStepActionPartViewModel
    {
        public int Id { get; set; }
        public int StepActionId { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public StepActionGroupsViewModel StepActionPartGroup { get; set; }
        public bool AllowUploadFile { get; set; }
        public bool UploadFileIsMandatory { get; set; }
    }
}
