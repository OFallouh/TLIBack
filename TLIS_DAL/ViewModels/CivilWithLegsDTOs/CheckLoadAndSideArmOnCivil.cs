using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using static TLIS_DAL.ViewModels.CivilLoadsDTOs.CivilLoads;

namespace TLIS_DAL.ViewModels.CivilWithLegsDTOs
{
    public class CheckLoadAndSideArmOnCivil
    {
        public CivilLoads Loads { get; set; }
        public SideArmAndLoadsOnCivil Info { get; set; }
    }
}
