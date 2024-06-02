using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_ODUDTOs
{
    public class MWODU_VIEW
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SiteCode { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public string Serial_Number { get; set; }
        public string Name { get; set; }
        public string? Notes { get; set; }
        public float Height { get; set; }
        public ODUConnections? ODUConnections { get; set; }
        public string? Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public string? OWNER { get; set; }
        public string MW_DISH { get; set; }
        public string ODUINSTALLATIONTYPE { get; set; }
        public string MWODULIBRARY { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public float Azimuth { get; set; }
        public int? SIDEARMID { get; set; }
        public string CIVILNAME { get; set; }
        public int CIVIL_ID { get; set; }
        public int ALLCIVILID { get; set; }
        public string? SIDEARMNAME { get; set; }
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
            outputData.Add("ODUConnections", ODUConnections);
            outputData.Add("Visiable_Status", Visiable_Status);
            outputData.Add("SpaceInstallation", SpaceInstallation);
            outputData.Add("Visiable_Status", Visiable_Status);
            outputData.Add("SpaceInstallation", SpaceInstallation);
            outputData.Add("CenterHigh", CenterHigh);
            outputData.Add("HBA", HBA);
            outputData.Add("HieghFromLand", HieghFromLand);
            outputData.Add("EquivalentSpace", EquivalentSpace);
            outputData.Add("HBA", HBA);
            outputData.Add("HieghFromLand", HieghFromLand);
            outputData.Add("EquivalentSpace", EquivalentSpace);
            outputData.Add("Azimuth", Azimuth);
            outputData.Add("OWNER", OWNER);
            outputData.Add("MW_DISH", MW_DISH);
            outputData.Add("ODUINSTALLATIONTYPE", ODUINSTALLATIONTYPE);
            outputData.Add("MWODULIBRARY", MWODULIBRARY);
            outputData.Add("ALLCIVILID", ALLCIVILID);
            outputData.Add("SIDEARMID", SIDEARMID);
            outputData.Add("CIVILNAME", CIVILNAME);
            outputData.Add("CIVIL_ID", CIVIL_ID);
            outputData.Add("SIDEARMNAME", SIDEARMNAME);
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

