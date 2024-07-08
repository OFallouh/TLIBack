using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.GeneratorDTOs
{
    public class MV_GENERATOR_VIEW
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? SITECODE { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public string SerialNumber { get; set; }
        public int NumberOfFuelTanks { get; set; }
        public bool BaseExisting { get; set; }
        public float SpaceInstallation { get; set; }
        public string? VisibleStatus { get; set; }
        public string BASEGENERATORTYPE { get; set; }
        public string GENERATORLIBRARY { get; set; }
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
            outputData.Add("SerialNumber", SerialNumber);
            outputData.Add("NumberOfFuelTanks", NumberOfFuelTanks);
            outputData.Add("BaseExisting", BaseExisting);
            outputData.Add("SpaceInstallation", SpaceInstallation);
            outputData.Add("VisibleStatus", VisibleStatus);
            outputData.Add("BASEGENERATORTYPE", BASEGENERATORTYPE);
            outputData.Add("GENERATORLIBRARY", GENERATORLIBRARY);
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

