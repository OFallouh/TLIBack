using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs
{
    public class MV_RADIO_ANTENNA_LIBRARY_VIEW
    {
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public int Id { get; set; }
        public string Model { get; set; }
        public string FrequencyBand { get; set; }
        public float? Weight { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }
        public float Length { get; set; }
        public string Notes { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }

        public Dictionary<string, object> GenerateOutputData()
        {
            Dictionary<string, object> outputData = new Dictionary<string, object>();
            outputData.Add("key", Key);
            outputData.Add("value", INPUTVALUE);
            outputData.Add("id", Id);
            outputData.Add("Model", Model);
            outputData.Add("FrequencyBand", FrequencyBand);
            outputData.Add("Weight", Weight);
            outputData.Add("Width", Width);
            outputData.Add("Depth", Depth);
            outputData.Add("Length", Length);
            outputData.Add("Active", Active);
            outputData.Add("Deleted", Deleted);
            outputData.Add("Notes", Notes);
            outputData.Add("SpaceLibrary", SpaceLibrary);

            if (!string.IsNullOrEmpty(Key) && !outputData.ContainsKey(Key))  // Check if Key already exists
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }

    }
}
