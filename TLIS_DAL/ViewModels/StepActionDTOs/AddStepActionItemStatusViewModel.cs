using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.ItemStatusDTOs;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionItemStatusViewModel
    {
        public int IncomingItemStatusId { get; set; }
        public int OutgoingItemStatusId { get; set; }
    }
    public class AddStepActionItemStatusWithNameViewModel
    {
        public ListStepActionItemStatusWiNameViewModel OutgoingItemStatus { get; set; }
        public ListStepActionItemStatusWiNameViewModel IncomingItemStatus { get; set; }
    }
}
