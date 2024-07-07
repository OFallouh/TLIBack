using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class RecalculatSpaceOnSite
    {
        public string AttributeName { get; set; }
        public string ItemOnSiteType { get; set; }
        public string ItemOnSiteName { get; set; }
        public string SiteName { get; set; }
        public string Type { get; set; }
        public bool ReservedSpaceInCivil { get; set; }
    }
}
