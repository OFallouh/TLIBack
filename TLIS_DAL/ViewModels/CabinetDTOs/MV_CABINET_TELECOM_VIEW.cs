using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.CabinetDTOs
{
    public class MV_CABINET_TELECOM_VIEW
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? SITECODE { get; set; }
        public string? Key { get; set; }
        public string? INPUTVALUE { get; set; }
        public string? TPVersion { get; set; }
        public int RenewableCabinetNumberOfBatteries { get; set; }
        public int NUmberOfPSU { get; set; }
        public float SpaceInstallation { get; set; }
        public string? VisibleStatus { get; set; }
        public string? CABINETTELECOMLIBRARY { get; set; }
        public string? RENEWABLECABINETTYPE { get; set; }
        public bool Dismantle { get; set; }


        public Dictionary<string, object> GenerateOutputData()
        {
            Dictionary<string, object> outputData = new Dictionary<string, object>();

            // Add original properties
            outputData.Add("sitecode", SITECODE);
            outputData.Add("dynamicKeyProperties", null);
            outputData.Add("dynamicValueProperties", null);
            outputData.Add("key", Key);
            outputData.Add("value", INPUTVALUE);
            outputData.Add("id", Id);
            outputData.Add("Name", Name);
            outputData.Add("TPVersion", TPVersion);
            outputData.Add("RenewableCabinetNumberOfBatteries", RenewableCabinetNumberOfBatteries);
            outputData.Add("NUmberOfPSU", NUmberOfPSU);
            outputData.Add("SpaceInstallation", SpaceInstallation);
            outputData.Add("VisibleStatus", VisibleStatus);
            outputData.Add("CABINETTELECOMLIBRARY", CABINETTELECOMLIBRARY);
            outputData.Add("RENEWABLECABINETTYPE", RENEWABLECABINETTYPE);
            outputData.Add("Dismantle", Dismantle);

            // Add dynamic property if "key" has a value
            if (!string.IsNullOrEmpty(Key))
            {
                outputData.Add(Key, INPUTVALUE);
            }

            return outputData;
        }
    }
}
