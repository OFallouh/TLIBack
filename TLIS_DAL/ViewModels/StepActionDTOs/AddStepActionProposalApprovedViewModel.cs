using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.ItemStatusDTOs;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionProposalApprovedViewModel
    {
        public int WorkflowId { get; set; }
        public string label { get; set; }
        public int Period { get; set; }
        public List<ListStepActionItemStatusWiNameViewModel> IncomItemStatus { get; set; }
        public StepActionGroupViewModel StepActionGroup { get; set; }
    }
}
