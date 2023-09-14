using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Helper.Filters
{
    public class MW_ODULibraryFilter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public double H_W_D { get; set; }
        public double frequency_range { get; set; }
        public double frequency_band { get; set; }
    }
}
