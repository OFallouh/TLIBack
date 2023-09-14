using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Helper.Filters
{
    public class MW_BULibraryFilter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double L_W_H { get; set; }
        public double frequency_band { get; set; }
        public double channel_bandwidth { get; set; }
        public string diversityType_Id { get; set; }
    }
}
