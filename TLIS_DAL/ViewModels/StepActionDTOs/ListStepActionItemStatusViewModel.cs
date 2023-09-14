using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class ListStepActionItemStatusViewModel
    {
        public int Id { get; set; }
        public int StepActionId { get; set; }
        public int ItemStatusId { get; set; }
    }

    public class ListStepActionItemStatusWiNameViewModel 
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
