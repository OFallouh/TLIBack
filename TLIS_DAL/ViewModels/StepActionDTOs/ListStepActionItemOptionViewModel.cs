using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class ListStepActionItemOptionViewModel
    {
        public int Id { get; set; }
        public int StepActionId { get; set; }
        public int ActionItemOptionId { get; set; }
        //public int? NextStepActionId { get; set; }
        public List<int> NextStepActions { get; set; }
        //public int? ItemStatusId { get; set; }
        public int? OrderStatusId { get; set; }
        public List<AddStepActionItemStatusViewModel> StepActionItemStatus { get; set; }
        public bool? AllowNote { get; set; }
        public bool? NoteIsMandatory { get; set; }
    }
    public class ListStepActionItemOptionWithNameViewModel
    {
        //public int Id { get; set; }
        //public int StepActionId { get; set; }
        public int ActionItemOptionId { get; set; }
        //public int? NextStepActionId { get; set; }
        public List<int> NextStepActions { get; set; }
        //public int? ItemStatusId { get; set; }
        public int? OrderStatusId { get; set; }
        public List<AddStepActionItemStatusWithNameViewModel> StepActionItemStatus { get; set; }
        public bool? AllowNote { get; set; }
        public bool? NoteIsMandatory { get; set; }
        public string ActionItemOptionName { get; set; }
        public string OrderStatusName { get; set; }
    }
}
