using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class SiteViewModel
    {
        public string SiteCode { get; set; }
        [Required]
        public string SiteName { get; set; }

        public int? LocationType { get; set; }

        public float LocationHieght { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public int siteStatusId { get; set; }

        public string Zone { get; set; }

        public int AreaId { get; set; }
        public string SubArea { get; set; }
        public string RegionCode { get; set; }

        public double RentedSpace { get; set; }
        public DateTime SiteVisiteDate { get; set; }

        public double ReservedSpace { get; set; }


    }
}
