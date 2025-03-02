using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class MV_TLISITE
    {
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string LocationType { get; set; }
        public float LocationHieght { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int siteStatusId { get; set; }
        public string SiteStatus { get; set; }
        public float RentedSpace { get; set; }
        public float ReservedSpace { get; set; }
        public DateTime SiteVisiteDate { get; set; }
        public string Zone { get; set; }
        public string SubArea { get; set; }
        public string RegionCode { get; set; }
        public string STATUS_DATE { get; set; }
        public string CREATE_DATE { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public string RegionName { get; set; }
    }
}
