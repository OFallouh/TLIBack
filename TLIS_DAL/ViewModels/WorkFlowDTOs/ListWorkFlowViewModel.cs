using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.WorkFlowGroupDTOs;
using TLIS_DAL.ViewModels.WorkFlowTypeDTOs;

namespace TLIS_DAL.ViewModels.WorkFlowDTOs
{
    public class ListWorkFlowViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int SiteStatusId { get; set; }
        //public TLIsiteStatus SiteStatus { get; set; }
        public List<ListWorkFlowTypeViewModel> WorkFlowTypes { get; set; }
        public List<WorkFlowGroupVM> WorkFlowGroups { get; set; }
        //public List<TLIticket> Tickets { get; set; }
    }
}
