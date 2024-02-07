using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.wf
{
    public class MetaLinkViewDto
    {
        public int Id { get; set; }
        public string Api { get; set; }
        public string URL { get; set; }
        public string URLActive { get; set; }
        public string Permission { get; set; }
    }
}
