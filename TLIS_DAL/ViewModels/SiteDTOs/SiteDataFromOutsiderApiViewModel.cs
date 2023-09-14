using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class SiteDataFromOutsiderApiViewModel
    {
        //public DateTime STATUS_DATE { get; set; }
        //public DateTime CREATE_DATE { get; set; }
        public string SITE_CODE { get; set; }
        public string SITE_NAME { get; set; }
        public float LATITUDE { get; set; }
        public float LONGITUDE { get; set; }
        public string REGION { get; set; } // Object
        public string AREA { get; set; } // Object
        public string ZONE { get; set; }
        public string SUBAREA { get; set; }
        public string SITE_STATUS { get; set; } // Object
    }
}
