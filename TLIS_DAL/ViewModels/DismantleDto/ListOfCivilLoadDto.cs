using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;

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

        public List<CivilLoads> LoadRelatedSide { get; set; }
    }


    public class LoadOnCivil
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
