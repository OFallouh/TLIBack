using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.DismantleDto
{
  public  class DismantleBinding
    {
        public int CivilId { get; set; }
        public string SiteCode { get; set; }
        public List<int> SidearmIds { get; set; }

        public List<int> Loadids { get; set; }

        public bool DismantleAll { get; set; }
        public bool RecalculatedReservedSpace { get; set; }
    }
}
