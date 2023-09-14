using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.WorkFlowDTOs
{
    public class PermissionWorkFlowViewModel
    {
        public int Id { get; set; }
        public List<TLIworkFlowGroup> WorkFlowGroups { get; set; }
    }
}
