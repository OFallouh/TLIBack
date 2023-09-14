using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.ItemStatusDTOs;
using TLIS_DAL.ViewModels.OrderStatusDTOs;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class ListStepActionOptionViewModel
    {
        public int Id { get; set; }
        public int StepActionId { get; set; }
        //public int? NextStepActionId { get; set; }
        public List<int> NextStepActions { get; set; }
        public int ActionOptionId { get; set; }
        public int? ItemStatusId { get; set; }
        public int? OrderStatusId { get; set; }
        public bool AllowNote { get; set; }
        public bool NoteIsMandatory { get; set; }
    }
    public class ListStepActionOptionWithNameViewModel 
    {
        public int Id { get; set; }
        public int StepActionId { get; set; }
        //public int? NextStepActionId { get; set; }
        public List<int> NextStepActions { get; set; }
        public int ActionOptionId { get; set; }
        public string ActionOptionName { get; set; }
        public ListItemStatusViewModel ItemStatus { get; set; }
        public OrderStatusViewModel OrderStatus { get; set; }
        public bool AllowNote { get; set; }
        public bool NoteIsMandatory { get; set; }
    }
}
