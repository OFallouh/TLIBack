using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.GroupDTOs
{
    public class EscalationWFViewModel
    {
        public GroupViewModel AllLevel1 { get; set; }
        public GroupViewModel AllLevel2 { get; set; }
        public GroupViewModel AllLevel3 { get; set; }
        public List<EscalationViewModel> Level1 { get; set; }

        public List<EscalationViewModel> Level2 { get; set; }

        public List<EscalationViewModel> Level3 { get; set; }
    }
}
