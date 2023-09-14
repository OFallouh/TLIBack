using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.DismantleDto
{
   public class ListOfCivilLoadDto
    {
        public string SiteName { get; set; }
        public string SiteCode { get; set; }
        public int CivilId { get; set; }
        public string CivilName { get; set; }
        public List<LoadOnSideArm> CivilLoadWithSideArm { get; set; }
        public List<LoadOnCivil> LoadDirOnCivil { get; set; }

    }

    public class LoadOnSideArm
    {
        public int SideArmId { get; set; }
        public string SideArmName { get; set; }

        public List<LoadOnCivil> LoadRelatedSide { get; set; }
    }


    public class LoadOnCivil
    {
        public int LoadId { get; set; }
        public string LoadName { get; set; }
    }
}
