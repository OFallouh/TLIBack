using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.WorkFlowTypeDTOs;

namespace TLIS_DAL.ViewModels.WorkFlowDTOs
{
    public class DeleteWorkFlowViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public int SiteStatusId { get; set; }
        //public TLIsiteStatus SiteStatus { get; set; }
        public List<TLIworkFlowType> WorkFlowTypes { get; set; }
        public List<TLIworkFlowGroup> WorkFlowGroups { get; set; }
        public List<TLIticket> Tickets { get; set; }
    }
}
