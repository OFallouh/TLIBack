using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.GeneratorLibraryDTOs
{
    public class MV_GENERATOR_LIBRARY_VIEW
    {
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public int Id { get; set; }
        public string Model { get; set; }
        public float Width { get; set; }
        public float Weight { get; set; }
        public float Length { get; set; }
        public string? LayoutCode { get; set; }
        public float Height { get; set; }
        public float SpaceLibrary { get; set; }
        public string? CAPACITY { get; set; }
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
            outputData.Add("Width", Width);
            outputData.Add("Weight", Weight);
            outputData.Add("Length", Length);
            outputData.Add("LayoutCode", LayoutCode);
            outputData.Add("Height", Height);
            outputData.Add("Active", Active);
            outputData.Add("Deleted", Deleted);
            outputData.Add("SpaceLibrary", SpaceLibrary);
            outputData.Add("CAPACITY", CAPACITY);

            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}