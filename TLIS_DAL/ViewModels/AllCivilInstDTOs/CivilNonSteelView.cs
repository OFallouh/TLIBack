using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.AllCivilInstDTOs
{
    public class CivilNonSteelView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? SITECODE { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? CurrentLoads { get; set; }
        public float? SpaceInstallation { get; set; }
        public string? CIVILNONSTEELLIBRARY { get; set; }
        public string? OWNER { get; set; }
        public string? SUPPORTTYPEIMPLEMENTED { get; set; }
        public string? LOCATIONTYPE { get; set; }
        public float? locationHeight { get; set; }
        public float? BuildingMaxLoad { get; set; }
        public float? CivilSupportCurrentLoad { get; set; }
        public float? H2Height { get; set; }
        public float? CenterHigh { get; set; }
        public float? HBA { get; set; }
        public float? HieghFromLand { get; set; }
        public float? EquivalentSpace { get; set; }
        public float? Support_Limited_Load { get; set; }

    
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
        outputData.Add("name", Name);
        outputData.Add("currentLoads", CurrentLoads);
        outputData.Add("spaceInstallation", SpaceInstallation);
        outputData.Add("civilnonsteellibrary", CIVILNONSTEELLIBRARY);
        outputData.Add("owner", OWNER);
        outputData.Add("supporttypeimplemented", SUPPORTTYPEIMPLEMENTED);
        outputData.Add("locationtype", LOCATIONTYPE);
        outputData.Add("locationHeight", locationHeight);
        outputData.Add("buildingMaxLoad", BuildingMaxLoad);
        outputData.Add("civilSupportCurrentLoad", CivilSupportCurrentLoad);
        outputData.Add("h2Height", H2Height);
        outputData.Add("centerHigh", CenterHigh);
        outputData.Add("hba", HBA);
        outputData.Add("hieghFromLand", HieghFromLand);
        outputData.Add("equivalentSpace", EquivalentSpace);
        outputData.Add("support_Limited_Load", Support_Limited_Load);

        // Add dynamic property if "key" has a value
        if (!string.IsNullOrEmpty(Key))
        {
            outputData.Add(Key, INPUTVALUE);
        }

        return outputData;
    }
    }

}
