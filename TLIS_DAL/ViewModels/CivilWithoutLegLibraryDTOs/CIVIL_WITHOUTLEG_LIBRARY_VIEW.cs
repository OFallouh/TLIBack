using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs
{
    public class MV_CIVIL_WITHOUTLEG_LIBRARY_VIEW
    {
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Model { get; set; }
        public string? Note { get; set; }
        public float Height_Designed { get; set; }
        public float Max_Load { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public float HeightBase { get; set; }
        public string Prefix { get; set; }
        public float Manufactured_Max_Load { get; set; }
        public string? WidthVariation { get; set; }
        public string CIVILWITHOUTLEGCATEGORY { get; set; }
        public string? INSTCIVILWITHOUTLEGSTYPE { get; set; }
        public string? CIVILSTEELSUPPORTCATEGORY { get; set; }
        public string STRUCTURETYPE { get; set; }

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
            outputData.Add("Height_Designed", Height_Designed);
            outputData.Add("Max_Load", Max_Load);
            outputData.Add("SpaceLibrary", SpaceLibrary);
            outputData.Add("Active", Active);
            outputData.Add("Deleted", Deleted);
            outputData.Add("HeightBase", HeightBase);
            outputData.Add("CIVILWITHOUTLEGCATEGORY", CIVILWITHOUTLEGCATEGORY);
            outputData.Add("INSTCIVILWITHOUTLEGSTYPE", INSTCIVILWITHOUTLEGSTYPE);
            outputData.Add("STRUCTURETYPE", STRUCTURETYPE);
            outputData.Add("CIVILSTEELSUPPORTCATEGORY", CIVILSTEELSUPPORTCATEGORY);
            outputData.Add("STRUCTURETYPE", STRUCTURETYPE);
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

