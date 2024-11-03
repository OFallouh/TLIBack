using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class SiteDataFromOutsiderApiViewModel
    {
        public string Sitename { get; set; }
        public string Sitecode { get; set; }
        public string LocationType { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Area { get; set; } // Object
        public string Zone { get; set; }
        public string Subarea { get; set; }
        public string Statusdate { get; set; }
        public string Createddate { get; set; }
        public string RegionCode { get; set; } // Object
        public string siteStatus { get; set; } // Object
        public float LocationHieght { get; set; }
        public float RentedSpace { get; set; }
        public float ReservedSpace { get; set; }
        public DateTime SiteVisiteDate { get; set; }





    }
}
