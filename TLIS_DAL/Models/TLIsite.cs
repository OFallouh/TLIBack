using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIsite
    {
        [Key]
       //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string SiteCode { get; set; }
        [Index(IsUnique =true)]
        public string SiteName { get; set; }
       
        public string LocationType { get; set; }
        
        public float LocationHieght { get; set; }
        [Required]
        public float Latitude { get; set; }
        [Required]
        public float Longitude { get; set; }
        [Required]
        public int siteStatusId { get; set; }
        public TLIsiteStatus siteStatus { get; set; }
        [Required]
        public float RentedSpace { get; set; }
        public float ReservedSpace { get; set; }
        public DateTime SiteVisiteDate { get; set; }
        public string Zone { get; set; }
        public string SubArea { get; set; }
        public string RegionCode { get; set; }
        public string STATUS_DATE { get; set; }
        public string CREATE_DATE { get; set; }

        public TLIregion Region { get; set; }
        public int AreaId { get; set; }
        public TLIarea Area { get; set; }
        public IEnumerable<TLIcivilSiteDate> CivilSiteDate { get; set; }
        public IEnumerable<TLIotherInSite> OthersInSite { get; set; }
        public IEnumerable<TLIcivilSupportDistance> civilSupportDistances { get; set; }
        public IEnumerable<TLIcivilLoads> CivilLoads { get; set; }
        public IEnumerable<TLIotherInventoryDistance> OtherInventoryDistances { get; set; }
    }
}
