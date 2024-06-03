using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_ODULibraryDTOs
{
    public class MV_MWODU_LIBRARY_VIEW
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Model { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public string? Note { get; set; }
        public float Weight { get; set; }
        public string H_W_D { get; set; }
        public float Depth { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string? frequency_range { get; set; }
        public string? frequency_band { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string? PARITY { get; set; }
        public float Diameter { get; set; }
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
            outputData.Add("Weight", Weight);
            outputData.Add("H_W_D", H_W_D);
            outputData.Add("Depth", Depth);
            outputData.Add("Width", Width);
            outputData.Add("Height", Height);
            outputData.Add("frequency_range", frequency_range);
            outputData.Add("frequency_band", frequency_band);
            outputData.Add("frequency_band", frequency_band);
            outputData.Add("SpaceLibrary", SpaceLibrary);
            outputData.Add("Active", Active);
            outputData.Add("Deleted", Deleted);
            outputData.Add("PARITY", PARITY);
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}

