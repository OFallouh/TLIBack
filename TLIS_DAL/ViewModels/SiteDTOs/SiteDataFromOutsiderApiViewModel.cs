using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class SiteDataFromOutsiderApiViewModel
    {
        public string Sitename { get; set; }
        public string Sitecode { get; set; }
        public string LocationType { get; set; }
        public float Latitude { get; set; }
        public float Longtude { get; set; }
        public string Area { get; set; } // Object
        public string Zone { get; set; }
        public string Subarea { get; set; }
        public string Statusdate { get; set; }
        public string Createddate { get; set; }
        public string Rejoncode { get; set; } // Object
        public float LocationHieght { get; set; }
    }
}
