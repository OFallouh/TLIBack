using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModelBase
{
    public class SideArmAndLoadsOnCivil
    {
        public int PowersCount { get; set; }
        public int MW_RFUsCount { get; set; }
        public int MW_BUsCount { get; set; }
        public int MW_DishesCount { get; set; }
        public int MW_ODUsCount { get; set; }
        public int OtherMWsCount { get; set; }
        public int RadioAntennasCount { get; set; }
        public int RadioRRUsCount { get; set; }
        public int OtherRadiosCount { get; set; }
        public int OtherLoadsCount { get; set; }
        public int SideArmsCount { get; set; }
        public double Availablespace { get; set; }  
        public double? CurrentLoads { get; set; }
    }
}
