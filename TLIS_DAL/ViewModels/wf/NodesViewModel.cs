using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WF_API.Model;

namespace TLIS_DAL.ViewModels.wf
{
    public class NodesViewModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public List<NodesViewModel>? Children { get; set; }
        public Relation_Node_Enum? Relation { get; set; }
        public List<OptionsBinding>? Options { get; set; }
    }
}
