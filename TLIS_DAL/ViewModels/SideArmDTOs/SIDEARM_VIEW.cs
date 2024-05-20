using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.SideArmDTOs
{
    public class SIDEARM_VIEW
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SITECODE { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public string CIVILNAME { get; set; }
        public string CIVILID { get; set; }
        public string? FIRST_LEG { get; set; }
        public string? SECOND_LEG { get; set; }
        public int? FIRST_LEG_ID { get; set; }
        public int? SECOND_LEG_ID { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string? Notes { get; set; }
        public float HeightBase { get; set; }
        public float Azimuth { get; set; }
        public bool ReservedSpace { get; set; }
        public string? VisibleStatus { get; set; }
        public float SpaceInstallation { get; set; }
        public string SIDEARMLIBRARY { get; set; }
        public string SIDEARMINSTALLATIONPLACE { get; set; }
        public string? OWNER { get; set; }
        public string SIDEARMTYPE { get; set; }
        public string? ITEMSTATUS { get; set; }
        public bool Draft { get; set; }
        public bool Active { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set;}
        public float EquivalentSpace { get; set;}
        public bool Dismantle { get; set; }


        public Dictionary<string, object> GenerateOutputData()
        {
            Dictionary<string, object> outputData = new Dictionary<string, object>();

            // Add original properties
            outputData.Add("SITECODE", SITECODE);
            outputData.Add("key", Key);
            outputData.Add("INPUTVALUE", INPUTVALUE);
            outputData.Add("Id", Id);
            outputData.Add("Name", Name);
            outputData.Add("CIVILNAME", CIVILNAME);
            outputData.Add("CIVILID", CIVILID);
            outputData.Add("FIRST_LEG", FIRST_LEG);
            outputData.Add("SECOND_LEG", SECOND_LEG);
            outputData.Add("Notes", Notes);
            outputData.Add("HeightBase", HeightBase);
            outputData.Add("Azimuth", Azimuth);
            outputData.Add("ReservedSpace", ReservedSpace);
            outputData.Add("Active", Active);
            outputData.Add("VisibleStatus", VisibleStatus);
            outputData.Add("SpaceInstallation", SpaceInstallation);
            outputData.Add("SIDEARMINSTALLATIONPLACE", SIDEARMINSTALLATIONPLACE);
            outputData.Add("OWNER", OWNER);
            outputData.Add("SIDEARMTYPE", SIDEARMTYPE);
            outputData.Add("ITEMSTATUS", ITEMSTATUS);
            outputData.Add("Draft", Draft);
            outputData.Add("CenterHigh", CenterHigh);
            outputData.Add("HBA", HBA);
            outputData.Add("HieghFromLand", HieghFromLand);
            outputData.Add("EquivalentSpace", EquivalentSpace);
            outputData.Add("SIDEARMLIBRARY", SIDEARMLIBRARY);
            outputData.Add("SECOND_LEG_ID", SECOND_LEG_ID);
            outputData.Add("FIRST_LEG_ID", FIRST_LEG_ID);


            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }

}
