using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.WorkFlowGroupDTOs;

namespace TLIS_DAL.ViewModels.WorkFlowDTOs
{
    public class WorkFlowGroupsViewModel
    {
        public int Id { get; set; }
        public WorkFlowGroupViewModel WorkFlowGroups { get; set; }
    }
}
