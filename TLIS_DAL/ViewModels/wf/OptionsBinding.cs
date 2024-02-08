using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WF_API.Model;

namespace TLIS_DAL.ViewModels.wf
{
    public class OptionsBinding
    {
        public List<string> Value { get; set; }
        public int FormElementId { get; set; }
        public Relation_Option_Enum Relation { get; set; }
    }
}
