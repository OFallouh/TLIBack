using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Helper.Filters
{
    public class MW_RFULibraryFilter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public double L_W_H { get; set; }
        public double size { get; set; }
        public double tx_parity { get; set; }
        public double frequency_band { get; set; }
    }
}
