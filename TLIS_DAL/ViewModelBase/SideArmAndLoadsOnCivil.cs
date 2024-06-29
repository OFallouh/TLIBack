using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModelBase
{
    public class SideArmAndLoadsOnCivil
    {
        public double Availablespace { get; set; }  
        public double? CurrentLoads { get; set; }

        public double? CivilMaxLoad { get; set; }
    }
}
