using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.CivilWithLegsDTOs
{
    public class INSTALLATION_PLACE
    {
        public string? SITECODE { get; set; }
        public string? SITENAME { get; set; }
        public int? STATUS_NUMBER { get; set; }
        public float? AZIMUTH { get; set; }
        public float? HEIGHT { get; set; }
        public int? WITHLEG_ID { get; set; }
        public int? FIRST_LEG_ID { get; set; }
        public int? SECOND_LEG_ID { get; set; }
        public int? WITHOUTLEG_ID { get; set; }
        public int? NONSTEEL_ID { get; set; }
        public int? FIRST_SIDEARM_ID { get; set; }
        public int? SECOND_SIDEARM_ID { get; set; }
        public int? MWDISH_ID { get; set; }
        public int? MWODU_ID { get; set; }
        public int? MWBU_ID { get; set; }
        public int? MWRFU_ID { get; set; }
        public int? MWOTHER_ID { get; set; }
        public int? RADIO_ANTENNA_ID { get; set; }
        public int? RADIO_RRU_ID { get; set; }
        public int? RADIO_OTHER_ID { get; set; }
        public int? POWER_ID { get; set; }
        public int? LOAD_OTHER_ID { get; set; }
   
 

        public Dictionary<string, object> GenerateOutputData()
        {
            Dictionary<string, object> outputData = new Dictionary<string, object>();

            // Add original properties
            outputData.Add("SITECODE", SITECODE);
            outputData.Add("SITENAME", SITENAME);
            outputData.Add("STATUS_NUMBER", STATUS_NUMBER);
            outputData.Add("AZIMUTH", AZIMUTH);
            outputData.Add("HEIGHT", HEIGHT);
            outputData.Add("WITHLEG_ID", WITHLEG_ID);
            outputData.Add("FIRST_LEG_ID", FIRST_LEG_ID);
            outputData.Add("SECOND_LEG_ID", SECOND_LEG_ID);
            outputData.Add("WITHOUTLEG_ID", WITHOUTLEG_ID);
            outputData.Add("NONSTEEL_ID", NONSTEEL_ID);
            outputData.Add("FIRST_SIDEARM_ID", FIRST_SIDEARM_ID);
            outputData.Add("SECOND_SIDEARM_ID", SECOND_SIDEARM_ID);
            outputData.Add("MWDISH_ID", MWDISH_ID);
            outputData.Add("MWODU_ID", MWODU_ID);
            outputData.Add("MWBU_ID", MWBU_ID);
            outputData.Add("MWRFU_ID", MWRFU_ID);
            outputData.Add("MWOTHER_ID", MWOTHER_ID);
            outputData.Add("RADIO_ANTENNA_ID", RADIO_ANTENNA_ID);
            outputData.Add("RADIO_RRU_ID", RADIO_RRU_ID);
            outputData.Add("RADIO_OTHER_ID", RADIO_OTHER_ID);
            outputData.Add("POWER_ID", POWER_ID);
            outputData.Add("LOAD_OTHER_ID", LOAD_OTHER_ID);
    

            return outputData;
        }
    }
}

