using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    using Newtonsoft.Json;

    public class SiteDataFromOutsiderApiViewModel
    {
        [JsonProperty("ENGLISH_NAME")]
        public string Sitename { get; set; }

        [JsonProperty("CODE")]
        public string Sitecode { get; set; }

        [JsonProperty("SITE_LOCATION")]
        public string LocationType { get; set; }

        [JsonProperty("LATITUDE")]
        public float Latitude { get; set; }

        [JsonProperty("LONGITUDE")]
        public float Longitude { get; set; }

        [JsonProperty("Area")]
        public string Area { get; set; } // Object

        [JsonProperty("ZONE")]
        public string Zone { get; set; }

        [JsonProperty("SUBAREA")]
        public string Subarea { get; set; }

        [JsonProperty("STATUS_DATE")]
        public string Statusdate { get; set; }

        [JsonProperty("CREATE_DATE")]
        public string Createddate { get; set; }

        [JsonProperty("REGION_CODE")]
        public string RegionCode { get; set; } // Object

        [JsonProperty("siteStatus")]
        public string siteStatus { get; set; } // Object (يمكن تجاهل هذه الخاصية إذا لم تكن موجودة في الاستجابة)

        [JsonProperty("HIGHT")]
        public float? LocationHieght { get; set; }

        [JsonProperty("RentedSpace")]
        public float RentedSpace { get; set; } // (قد تكون غير موجودة في الاستجابة)

        [JsonProperty("ReservedSpace")]
        public float ReservedSpace { get; set; } // (قد تكون غير موجودة في الاستجابة)

        public DateTime SiteVisiteDate { get; set; } // حقل مضاف لأغراضك
    }

}
