using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_BUDTOs
{
    public class MV_MWRFU_LIBRARY_VIEW
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public string? Note { get; set; }
        public float Weight { get; set; }
        public string? L_W_H { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string? size { get; set; }
        public bool tx_parity { get; set; }
        public string? frequency_band { get; set; }
        public string? FrequencyRange { get; set; }
        public RFUType RFUType { get; set; }
        public string? VenferBoardName { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string DIVERSITYTYPE { get; set; }
        public string BOARDTYPE { get; set; }

        public Dictionary<string, object> GenerateOutputData()
        {
            Dictionary<string, object> outputData = new Dictionary<string, object>();

            outputData.Add("dynamicKeyProperties", null);
            outputData.Add("dynamicValueProperties", null);
            outputData.Add("key", Key);
            outputData.Add("value", INPUTVALUE);
            outputData.Add("id", Id);
            outputData.Add("Model", Model);
            outputData.Add("Weight", Weight);
            outputData.Add("Note", Note);
            outputData.Add("L_W_H", L_W_H);
            outputData.Add("Length", Length);
            outputData.Add("Width", Width);
            outputData.Add("Height", Height);
            outputData.Add("size", size);
            outputData.Add("frequency_band", frequency_band);
            outputData.Add("SpaceLibrary", SpaceLibrary);
            outputData.Add("Active", Active);
            outputData.Add("Deleted", Deleted);
            outputData.Add("tx_parity", tx_parity);
            outputData.Add("FrequencyRange", FrequencyRange);
            outputData.Add("RFUType", RFUType);
            outputData.Add("DIVERSITYTYPE", DIVERSITYTYPE);
            outputData.Add("BOARDTYPE", BOARDTYPE);


            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}

