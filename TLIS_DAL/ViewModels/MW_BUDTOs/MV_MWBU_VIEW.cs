using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_BUDTOs
{
    public class MV_MWBU_VIEW
    {
        public int Id { get; set; }
        public string SiteCode { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public string Notes { get; set; }
        public string Name { get; set; }
        public string Serial_Number { get; set; }
        public float Height { get; set; }
        public float Azimuth { get; set; }
        public int BUNumber { get; set; }
        public bool Active { get; set; }
        public string? Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public string? BASEBU { get; set; }
        public string INSTALLATIONPLACE { get; set; }
        public string? OWNER { get; set; }
        public string MWBULIBRARY { get; set; }
        public string MAINDISH { get; set; }
        public string? SDDISH { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public string? PORTCASCADE { get; set; }
        public string CIVILNAME { get; set; }
        public string? SIDEARMNAME { get; set; }
        public string? SideArmSec_Name { get; set; }
        public string? LEG_NAME { get; set; }
        public int CIVIL_ID { get; set; }
        public int ALLCIVILINST_ID { get; set; }
        public int ALLLOAD_ID { get; set; }
        public int LEG_ID { get; set; }
        public bool Dismantle { get; set; }

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
            outputData.Add("Serial_Number", Serial_Number);
            outputData.Add("Name", Name);
            outputData.Add("Notes", Notes);
            outputData.Add("Height", Height);
            outputData.Add("Visiable_Status", Visiable_Status);
            outputData.Add("SpaceInstallation", SpaceInstallation);
            outputData.Add("CenterHigh", CenterHigh);
            outputData.Add("HBA", HBA);
            outputData.Add("HieghFromLand", HieghFromLand);
            outputData.Add("EquivalentSpace", EquivalentSpace);
            outputData.Add("Azimuth", Azimuth);
            outputData.Add("OWNER", OWNER);
            outputData.Add("BUNumber", BUNumber);
            outputData.Add("INSTALLATIONPLACE", INSTALLATIONPLACE);
            outputData.Add("BASEBU", BASEBU);
            outputData.Add("SDDISH", SDDISH);
            outputData.Add("MAINDISH", MAINDISH);
            outputData.Add("CIVILNAME", CIVILNAME);
            outputData.Add("CIVIL_ID", CIVIL_ID);
            outputData.Add("SIDEARMNAME", SIDEARMNAME);
            outputData.Add("MWBULIBRARY", MWBULIBRARY);
            outputData.Add("PORTCASCADE", PORTCASCADE);
            outputData.Add("SideArmSec_Name", SideArmSec_Name);
            outputData.Add("LEG_NAME", LEG_NAME);
            outputData.Add("ALLCIVILINST_ID", ALLCIVILINST_ID);
            outputData.Add("ALLLOAD_ID", ALLLOAD_ID);
            outputData.Add("LEG_ID", LEG_ID);
            outputData.Add("Dismantle", Dismantle);
            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}

