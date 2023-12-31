using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.CivilLoadsDTOs
{
    public class ListOfCivilLoads
    {
        public List<CivilLoad> CivilLoad{ get; set; }
        public List<OtherInventories> OtherInventories { get; set; }
        public List<FileRelated> FileRelated { get; set; }

    }

    public class LoadOnSideArms
    {
        public int SideArmId { get; set; }
        public string SideArmName { get; set; }

        public List<LoadOnCivils> LoadRelatedSide { get; set; }
    }


    public class LoadOnCivils
    {
        public int LoadId { get; set; }
        public string LoadName { get; set; }
    }
    public class OtherInventories
    {
        public int OtherInventoriesId { get; set; }
        public string OtherInventoriesName { get; set; }
    }
    public class FileRelated
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
    }
    public class CivilLoad
    {
        public int CivilId { get; set; }
        public string CivilName { get; set; }
        public List<LoadOnSideArms> CivilLoadWithSideArm { get; set; }
        public List<LoadOnCivils> LoadDirOnCivil { get; set; }
    }

}

