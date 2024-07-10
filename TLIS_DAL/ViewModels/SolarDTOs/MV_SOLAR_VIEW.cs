using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.SolarDTOs
{
    public class MV_SOLAR_VIEW
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SITECODE { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public string? PVPanelBrandAndWattage { get; set; }
        public string? PVArrayAzimuth { get; set; }
        public string? PVArrayAngel { get; set; }
        public string? Prefix { get; set; }
        public string? PowerLossRatio { get; set; }
        public int NumberOfSSU { get; set; }
        public int NumberOfLightingRod { get; set; }
        public int NumberOfInstallPVs { get; set; }
        public string? LocationDescription { get; set; }
        public string? ExtenstionDimension { get; set; }
        public string? Extension { get; set; }
        public float SpaceInstallation { get; set; }
        public string? VisibleStatus { get; set; }
        public string SOLARLIBRARY { get; set; }
        public string CABINET { get; set; }
        public bool Dismantle { get; set; }


        public Dictionary<string, object> GenerateOutputData()
        {
            Dictionary<string, object> outputData = new Dictionary<string, object>();

            // Add original properties
            outputData.Add("sitecode", SITECODE);
            outputData.Add("dynamicKeyProperties", null);
            outputData.Add("dynamicValueProperties", null);
            outputData.Add("key", Key);
            outputData.Add("value", INPUTVALUE);
            outputData.Add("id", Id);
            outputData.Add("Name", Name);
            outputData.Add("PVPanelBrandAndWattage", PVPanelBrandAndWattage);
            outputData.Add("PVArrayAzimuth", PVArrayAzimuth);
            outputData.Add("PVArrayAngel", PVArrayAngel);
            outputData.Add("Prefix", Prefix);
            outputData.Add("PowerLossRatio", PowerLossRatio);
            outputData.Add("NumberOfSSU", NumberOfSSU);
            outputData.Add("NumberOfLightingRod", NumberOfLightingRod);
            outputData.Add("NumberOfInstallPVs", NumberOfInstallPVs);
            outputData.Add("LocationDescription", LocationDescription);
            outputData.Add("ExtenstionDimension", ExtenstionDimension);
            outputData.Add("Extension", Extension);
            outputData.Add("SpaceInstallation", SpaceInstallation);
            outputData.Add("VisibleStatus", VisibleStatus);
            outputData.Add("SOLARLIBRARY", SOLARLIBRARY);
            outputData.Add("CABINET", CABINET);
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


