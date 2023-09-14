using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionItemOptionValidationViewModel
    {
        public int ActionItemOptionId { get; set; }
        //public int? NextStepActionId { get; set; }
        public List<int> NextStepActions { get; set; }
        //public int? ItemStatusId { get; set; }
        public List<AddStepActionItemStatusViewModel> StepActionItemStatus { get; set; }
        public bool? AllowNote { get; set; }
        public bool? NoteIsMandatory { get; set; }
        //public List<int> NextStepActions { get; set; }
    }
}
