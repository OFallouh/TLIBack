using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class EditStepActionTicketStatusViewModel
    {
        public int Id { get; set; }
        public int Period { get; set; }
        public string label { get; set; }
        public int OrderStatusId { get; set; }
        public List<int> NextStepActions { get; set; }
    }
}
