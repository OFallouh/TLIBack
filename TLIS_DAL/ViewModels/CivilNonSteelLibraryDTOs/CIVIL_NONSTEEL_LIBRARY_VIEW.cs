using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.CivilNonSteelLibraryDTOs
{
    public class MV_CIVIL_NONSTEEL_LIBRARY_VIEW
    {
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Model { get; set; }
        public string? Prefix { get; set; }
        public string? Note { get; set; }
        public float Hight { get; set; }
        public float SpaceLibrary { get; set; }
        public bool VerticalMeasured { get; set; }
        public string CIVILNONSTEELTYPE { get; set; }
        public float NumberofBoltHoles { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string? WidthVariation { get; set; }
        public float Manufactured_Max_Load { get; set; }

        public Dictionary<string, object> GenerateOutputData()
        {
            Dictionary<string, object> outputData = new Dictionary<string, object>();

            outputData.Add("dynamicKeyProperties", null);
            outputData.Add("dynamicValueProperties", null);
            outputData.Add("key", Key);
            outputData.Add("value", INPUTVALUE);
            outputData.Add("id", Id);
            outputData.Add("Model", Model);
            outputData.Add("Note", Note);
            outputData.Add("Prefix", Prefix);
            outputData.Add("Hight", Hight);
            outputData.Add("VerticalMeasured", VerticalMeasured);
            outputData.Add("SpaceLibrary", SpaceLibrary);
            outputData.Add("Active", Active);
            outputData.Add("Deleted", Deleted);
            outputData.Add("NumberofBoltHoles", NumberofBoltHoles);
            outputData.Add("CIVILNONSTEELTYPE", CIVILNONSTEELTYPE);
            outputData.Add("Manufactured_Max_Load", Manufactured_Max_Load);
            outputData.Add("WidthVariation", WidthVariation);

            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}
