using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.MW_OtherLibraryDTOs
{
    public class MV_MWOTHER_LIBRARY_VIEW
    {
        public int Id { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public string Model { get; set; }
        public string? Note { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string? L_W_H { get; set; }
        public string? frequency_band { get; set; }
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
            outputData.Add("L_W_H", L_W_H);
            outputData.Add("Length", Length);
            outputData.Add("Width", Width);
            outputData.Add("Height", Height);
            outputData.Add("frequency_band", frequency_band);
            outputData.Add("SpaceLibrary", SpaceLibrary);
            outputData.Add("Active", Active);
            outputData.Add("Deleted", Deleted);


            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}


