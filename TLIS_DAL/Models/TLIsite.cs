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
        [Index(IsUnique = true)]
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
        public float RentedSpace { get; set; } = 0;
        public float ReservedSpace { get; set; } = 0;
        public DateTime SiteVisiteDate { get; set; }
        public string Zone { get; set; }
        public string SubArea { get; set; }
        public string RegionCode { get; set; }
        public string STATUS_DATE { get; set; }
        public string CREATE_DATE { get; set; }
        public TLIregion Region { get; set; }
        public int AreaId { get; set; }
        public TLIarea Area { get; set; }

        public int? PlanStatusCollectData { get; set; }
        public int? PlanStatusMWMd { get; set; }
        public int? PlanStatusPowerMd { get; set; }
        public int? PlanStatusRadioMd { get; set; }
        public string PlanType { get; set; }
        public int? pendingTypeCollectData { get; set; }
        public int? pendingTypeMWMd { get; set; }
        public int? pendingTypePowerMd { get; set; }
        public int? pendingTypeRadioMd { get; set; }
        public int? MWValidationStatusCollectDate { get; set; }
        public int? MWValidationStatusMWMd { get; set; }
        public string MWValidationRemarkCollectData { get; set; }
        public string MWValidationRemarkMWMd { get; set; }
        public int? RadioVStatusCollectData { get; set; }
        public int? RadioVStatusRadioMd { get; set; }
        public string RadioVRemarkCollectData { get; set; }
        public string RadioVRemarkRadioMd{ get; set; }
        public int? PowerVStatusCollectData { get; set; }
        public int? PowerVStatusPowerMd { get; set; }
        public string PowerVRemarkPowerMd { get; set; }
        public string PowerVRemarkCollectData { get; set; }
        public int? MdTypePowerMd { get; set; }
        public int? MdTypeMWMd { get; set; }
        public int? MdTypeRadioMd { get; set; }
        public string DescriptionRadioMd { get; set; }
        public string DescriptionPowerMd { get; set; }
        public string DescriptionMWMd { get; set; }
        public IEnumerable<TLIcivilSiteDate> CivilSiteDate { get; set; }
        public IEnumerable<TLIotherInSite> OthersInSite { get; set; }
        public IEnumerable<TLIcivilSupportDistance> civilSupportDistances { get; set; }
        public IEnumerable<TLIcivilLoads> CivilLoads { get; set; }
        public IEnumerable<TLIotherInventoryDistance> OtherInventoryDistances { get; set; }
        public IEnumerable<TLIticket> Tickets { get; set; }
    }
}
