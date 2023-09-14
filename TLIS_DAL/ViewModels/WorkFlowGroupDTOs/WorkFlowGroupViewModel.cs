using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.WorkFlowGroupDTOs
{
    public class WorkFlowGroupViewModel
    {
        public List<int> Actors { get; set; }
        public List<int> Integrations { get; set; }
        public List<int> Users { get; set; }
        public List<int> Groups { get; set; }
    }
}
