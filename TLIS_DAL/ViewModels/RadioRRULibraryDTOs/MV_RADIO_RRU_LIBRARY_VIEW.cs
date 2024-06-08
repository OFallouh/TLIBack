using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.RadioRRULibraryDTOs
{
    public class MV_RADIO_RRU_LIBRARY_VIEW
    {
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public int Id { get; set; }
        public string Model { get; set; }
        public string? Type { get; set; }
        public string? Band { get; set; }
        public float ChannelBandwidth { get; set; }
        public float Weight { get; set; }
        public string? L_W_H_cm3 { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string? Notes { get; set; }
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
            outputData.Add("Type", Type);
            outputData.Add("Weight", Weight);
            outputData.Add("Width", Width);
            outputData.Add("Depth", Depth);
            outputData.Add("Length", Length);
            outputData.Add("Active", Active);
            outputData.Add("Deleted", Deleted);
            outputData.Add("Notes", Notes);
            outputData.Add("SpaceLibrary", SpaceLibrary);
            outputData.Add("Band", Band);
            outputData.Add("ChannelBandwidth", ChannelBandwidth);
            outputData.Add("L_W_H_cm3", L_W_H_cm3);
            outputData.Add("Height", Height);

            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}
