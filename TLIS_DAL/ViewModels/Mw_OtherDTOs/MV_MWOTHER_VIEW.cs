using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.Mw_OtherDTOs
{
    public class MV_MWOTHER_VIEW
    {
        public int Id { get; set; }
        public string SiteCode { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string? Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public string? VisibleStatus { get; set; }
        public string MWOTHERLIBRARY { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public float Azimuth { get; set; }
        public float Spaceinstallation { get; set; }
        public string INSTALLATIONPLACE { get; set; }
        public string CIVILNAME { get; set; }
        public string? SIDEARMNAME { get; set; }
        public string? SideArmSec_Name { get; set; }
        public string? LEG_NAME { get; set; }
        public int CIVIL_ID { get; set; }
        public int ALLCIVILINST_ID { get; set; }
        public int ALLLOAD_ID { get; set; }
        public int? LEG_ID { get; set; }
        public bool Dismantle { get; set; }
        public int? SIDEARM_ID { get; set; }
        public int? SideArmSec_Id { get; set; }

        public Dictionary<string, object> GenerateOutputData()
        {
            Dictionary<string, object> outputData = new Dictionary<string, object>();

            // Add original properties
            outputData.Add("SiteCode", SiteCode);
            outputData.Add("dynamicKeyProperties", null);
            outputData.Add("dynamicValueProperties", null);
            outputData.Add("key", Key);
            outputData.Add("value", INPUTVALUE);
            outputData.Add("id", Id);
            outputData.Add("SerialNumber", SerialNumber);
            outputData.Add("Name", Name);
            outputData.Add("Notes", Notes);
            outputData.Add("HeightBase", HeightBase);
            outputData.Add("HeightLand", HeightLand);
            outputData.Add("Spaceinstallation", Spaceinstallation);
            outputData.Add("CenterHigh", CenterHigh);
            outputData.Add("HBA", HBA);
            outputData.Add("VisibleStatus", VisibleStatus);
            outputData.Add("HieghFromLand", HieghFromLand);
            outputData.Add("EquivalentSpace", EquivalentSpace);
            outputData.Add("Azimuth", Azimuth);
            outputData.Add("HieghFromLand", HieghFromLand);
            outputData.Add("INSTALLATIONPLACE", INSTALLATIONPLACE);
            outputData.Add("CIVILNAME", CIVILNAME);
            outputData.Add("CIVIL_ID", CIVIL_ID);
            outputData.Add("SIDEARMNAME", SIDEARMNAME);
            outputData.Add("MWOTHERLIBRARY", MWOTHERLIBRARY);
            outputData.Add("SideArmSec_Name", SideArmSec_Name);
            outputData.Add("LEG_NAME", LEG_NAME);
            outputData.Add("ALLCIVILINST_ID", ALLCIVILINST_ID);
            outputData.Add("ALLLOAD_ID", ALLLOAD_ID);
            outputData.Add("LEG_ID", LEG_ID);
            outputData.Add("Dismantle", Dismantle);
            outputData.Add("SideArmSec_Id", SideArmSec_Id);
            outputData.Add("SIDEARM_ID", SIDEARM_ID);
            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}

