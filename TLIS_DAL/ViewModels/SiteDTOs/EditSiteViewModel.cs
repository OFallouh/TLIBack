using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
   public class EditSiteViewModel
    {
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public int LocationType { get; set; }
        public float LocationHieght { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int siteStatusId { get; set; }
        public float RentedSpace { get; set; }
        public float ReservedSpace { get; set; }
        public string Zone { get; set; }
        public string SubArea { get; set; }
        public string RegionCode { get; set; }
        public int AreaId { get; set; }
    }
}
