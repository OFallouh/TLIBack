﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.RadioAntennaDTOs
{
    public class MV_RADIO_ANTENNA_VIEW
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string SiteCode { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public float Azimuth { get; set; }
        public float MechanicalTilt { get; set; }
        public float ElectricalTilt { get; set; }
        public string SerialNumber { get; set; }
        public float HBASurface { get; set; }
        public string? Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public float SpaceInstallation { get; set; }
        public string? VisibleStatus { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public string INSTALLATIONPLACE { get; set; }
        public string RADIOANTENNALIBRARY { get; set; }
        public string? LEGNAME { get; set; }
        public string? OWNER { get; set; }
        public int? LEGID { get; set; }
        public string CIVILNAME { get; set; }
        public int CIVIL_ID { get; set; }
        public string? SIDEARMNAME { get; set; }
        public int? SIDEARM_ID { get; set; }
        public int ALLCIVILINST_ID { get; set; }
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
            outputData.Add("Azimuth", Azimuth);
            outputData.Add("Notes", Notes);
            outputData.Add("Name", Name);
            outputData.Add("MechanicalTilt", MechanicalTilt);
            outputData.Add("ElectricalTilt", ElectricalTilt);
            outputData.Add("SerialNumber", SerialNumber);
            outputData.Add("HBASurface", HBASurface);
            outputData.Add("HeightBase", HeightBase);
            outputData.Add("SpaceInstallation", SpaceInstallation);
            outputData.Add("HeightLand", HeightLand);
            outputData.Add("VisibleStatus", VisibleStatus);
            outputData.Add("CenterHigh", CenterHigh);
            outputData.Add("HBA", HBA);
            outputData.Add("HieghFromLand", HieghFromLand);
            outputData.Add("EquivalentSpace", EquivalentSpace);
            outputData.Add("RADIOANTENNALIBRARY", RADIOANTENNALIBRARY);
            outputData.Add("INSTALLATIONPLACE", INSTALLATIONPLACE);
            outputData.Add("LEGNAME", LEGNAME);
            outputData.Add("CIVILNAME", CIVILNAME);
            outputData.Add("CIVIL_ID", CIVIL_ID);
            outputData.Add("SIDEARMNAME", SIDEARMNAME);
            outputData.Add("LEGID", LEGID);
            outputData.Add("Dismantle", Dismantle);
            outputData.Add("SIDEARM_ID", SIDEARM_ID);
            outputData.Add("ALLCIVILINST_ID", ALLCIVILINST_ID);
            outputData.Add("OWNER ", OWNER);
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}


