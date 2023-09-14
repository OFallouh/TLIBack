using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.WorkFlowTypeDTOs;

namespace TLIS_DAL.ViewModels.WorkFlowDTOs
{
    public class AddWorkFlowViewModel
    {
        public string Name { get; set; }
        public int SiteStatusId { get; set; }
        //public List<AddWorkFlowTypeViewModel> WorkFlowTypes { get; set; }
        //public List<TLIworkFlowGroup> WorkFlowGroups { get; set; }
    }
}
