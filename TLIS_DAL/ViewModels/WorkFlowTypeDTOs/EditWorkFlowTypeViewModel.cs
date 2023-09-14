using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.WorkFlowTypeDTOs
{
    public class EditWorkFlowTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WorkFlowId { get; set; }
        //public bool Deleted { get; set; }
        //public DateTime? DateDeleted { get; set; }
        public int? nextStepActionId { get; set; }
        //public TLIstepAction nextStepAction { get; set; }
    }
}
