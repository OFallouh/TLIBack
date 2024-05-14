using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_DishDTOs
{
    public class MWDISH_VIEW
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DishName { get; set; }
        public string SiteCode { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public float Azimuth { get; set; }
        public string? Notes { get; set; }
        public string? Far_End_Site_Code { get; set; }
        public float HBA_Surface { get; set; }
        public string Serial_Number { get; set; }
        public string? MW_LINK { get; set; }
        public string? Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public string? Temp { get; set; }
        public string? OWNER { get; set; }
        public string? REPEATERTYPE { get; set; }
        public string POLARITYONLOCATION { get; set; }
        public string ITEMCONNECTTO { get; set; }
        public string MWDISHLIBRARY { get; set; }
        public string INSTALLATIONPLACE { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public bool Dismantle { get; set; }
        public string? LEG_NAME { get; set; }
        public string CIVILNAME { get; set; }
        public int CIVIL_ID { get; set; }
        public string? SIDEARMNAME { get; set; }
        public int? SIDEARM_ID { get; set; }
        public int ALLCIVILINST_ID { get; set; }
        public int? LEG_ID { get; set; }        
        public int ODU_COUNT { get; set; }
        public string POLARITYTYPE { get; set; }
 

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
            outputData.Add("Azimuth", Azimuth);
            outputData.Add("Notes", Notes);
            outputData.Add("Far_End_Site_Code", Far_End_Site_Code);
            outputData.Add("HBA_Surface", HBA_Surface);
            outputData.Add("Serial_Number", Serial_Number);
            outputData.Add("DishName", DishName);
            outputData.Add("MW_LINK", MW_LINK);
            outputData.Add("Visiable_Status", Visiable_Status);
            outputData.Add("SpaceInstallation", SpaceInstallation);
            outputData.Add("HeightBase", HeightBase);
            outputData.Add("HeightLand", HeightLand);
            outputData.Add("Temp", Temp);
            outputData.Add("CenterHigh", CenterHigh);
            outputData.Add("HBA", HBA);
            outputData.Add("HieghFromLand", HieghFromLand);
            outputData.Add("EquivalentSpace", EquivalentSpace);
            outputData.Add("OWNER", OWNER);
            outputData.Add("REPEATERTYPE", REPEATERTYPE);
            outputData.Add("POLARITYONLOCATION", POLARITYONLOCATION);
            outputData.Add("ITEMCONNECTTO", ITEMCONNECTTO);
            outputData.Add("INSTALLATIONPLACE", INSTALLATIONPLACE);
            outputData.Add("MWDISHLIBRARY", MWDISHLIBRARY);
            outputData.Add("LEG_NAME", LEG_NAME);
            outputData.Add("CIVILNAME", CIVILNAME);
            outputData.Add("CIVIL_ID", CIVIL_ID);      
            outputData.Add("SIDEARMNAME", SIDEARMNAME);      
            outputData.Add("SIDEARM_ID", SIDEARM_ID);      
            outputData.Add("ALLCIVILINST_ID", ALLCIVILINST_ID);      
            outputData.Add("Dismantle", Dismantle);      
            outputData.Add("LEG_ID", LEG_ID);      
            outputData.Add("ODU_COUNT", ODU_COUNT);           
            outputData.Add("POLARITYTYPE", POLARITYTYPE);           
            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}


