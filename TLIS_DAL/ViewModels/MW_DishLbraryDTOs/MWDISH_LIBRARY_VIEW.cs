using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_DishLbraryDTOs
{
    public class MWDISH_LIBRARY_VIEW
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Model { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public float Weight { get; set; }
        public string? dimensions { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float diameter { get; set; }
        public string? frequency_band { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string ASTYPE { get; set; }
        public string POLARITYTYPE { get; set; }
       
        public Dictionary<string, object> GenerateOutputData()
        {
            Dictionary<string, object> outputData = new Dictionary<string, object>();

            outputData.Add("dynamicKeyProperties", null);
            outputData.Add("dynamicValueProperties", null);
            outputData.Add("key", Key);
            outputData.Add("value", INPUTVALUE);
            outputData.Add("id", Id);
            outputData.Add("Model", Model);
            outputData.Add("Description", Description);
            outputData.Add("Note", Note);
            outputData.Add("Weight", Weight);
            outputData.Add("dimensions", dimensions);
            outputData.Add("Length", Length);
            outputData.Add("Width", Width);
            outputData.Add("Height", Height);
            outputData.Add("diameter", diameter);
            outputData.Add("frequency_band", frequency_band);
            outputData.Add("SpaceLibrary", SpaceLibrary);
            outputData.Add("Active", Active);
            outputData.Add("Deleted", Deleted);
            outputData.Add("ASTYPE", ASTYPE);
            outputData.Add("POLARITYTYPE", POLARITYTYPE);
            
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}


