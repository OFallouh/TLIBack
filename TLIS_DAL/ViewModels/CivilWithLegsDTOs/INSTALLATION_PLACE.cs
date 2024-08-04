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
    

            return outputData;
        }
    }
}

