using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.StepDTOs
{
    public class StepListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public int? WorkFlowId { get; set; }
        public int? ParentStepId { get; set; }
        public StepType type { get; set; }
        public List<TLIstepAction> StepActions { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DeleteDate { get; set; }
        public List<TLIticketStep> TicketSteps { get; set; }

    }
}
