using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs
{
    public class MV_CABINET_TELECOM_LIBRARY_VIEW
    {
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public int Id { get; set; }
        public string Model { get; set; }
        public float MaxWeight { get; set; }
        public string? LayoutCode { get; set; }
        public string? Dimension_W_D_H { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }
        public float Height { get; set; }
        public float SpaceLibrary { get; set; }
        public string TELECOMTYPE { get; set; }
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
            outputData.Add("MaxWeight", MaxWeight);
            outputData.Add("Dimension_W_D_H", Dimension_W_D_H);
            outputData.Add("LayoutCode", LayoutCode);
            outputData.Add("Height", Height);
            outputData.Add("Active", Active);
            outputData.Add("Deleted", Deleted);
            outputData.Add("SpaceLibrary", SpaceLibrary);
            outputData.Add("Dimension_W_D_H", Dimension_W_D_H);
            outputData.Add("Depth", Depth);
            outputData.Add("Width", Width);
            outputData.Add("TELECOMTYPE", TELECOMTYPE);


            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}