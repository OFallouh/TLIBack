using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.PowerLibraryDTOs
{
    public class MV_POWER_LIBRARY_VIEW
    {
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public int Id { get; set; }
        public string Model { get; set; }
        public string? Note { get; set; }
        public string? FrequencyRange { get; set; }
        public string? BandWidth { get; set; }
        public string? ChannelBandWidth { get; set; }
        public string? Type { get; set; }
        public float Size { get; set; }
        public string? L_W_H { get; set; }
        public float Weight { get; set; }
        public float width { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public float Depth { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }

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
            outputData.Add("FrequencyRange", FrequencyRange);
            outputData.Add("BandWidth", BandWidth);
            outputData.Add("ChannelBandWidth", ChannelBandWidth);
            outputData.Add("Type", Type);
            outputData.Add("Active", Active);
            outputData.Add("Deleted", Deleted);
            outputData.Add("Size", Size);
            outputData.Add("L_W_H", L_W_H);
            outputData.Add("Weight", Weight);
            outputData.Add("width", width);
            outputData.Add("Length", Length);
            outputData.Add("Height", Height);
            outputData.Add("Depth", Depth);
            outputData.Add("SpaceLibrary", SpaceLibrary);

            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}