using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionOptionViewModel
    {
        public int ActionOptionId { get; set; }
        //public int? NextStepActionId { get; set; }
        public List<int> NextStepActions { get; set; }
        public int? ItemStatusId { get; set; }
        public int? OrderStatusId { get; set; }
        public bool AllowNote { get; set; }
        public bool NoteIsMandatory { get; set; }
    }
}
