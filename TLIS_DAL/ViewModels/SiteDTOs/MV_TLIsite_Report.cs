using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class MV_TLIsite_Report
    {
       
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public int? PlanStatusCollectData { get; set; }
        public int? PlanStatusMWMd { get; set; }
        public int? PlanStatusPowerMd { get; set; }
        public int? PlanStatusRadioMd { get; set; }
        public string PlanType { get; set; }
        public int? PendingTypeCollectData { get; set; }
        public int? PendingTypeMWMd { get; set; }
        public int? PendingTypePowerMd { get; set; }
        public int? PendingTypeRadioMd { get; set; }
        public int? MWValidationStatusCollectDate { get; set; }
        public int? MWValidationStatusMWMd { get; set; }
        public string MWValidationRemarkCollectData { get; set; }
        public string MWValidationRemarkMWMd { get; set; }
        public int? RadioVStatusCollectData { get; set; }
        public int? RadioVStatusRadioMd { get; set; }
        public string RadioVRemarkCollectData { get; set; }
        public string RadioVRemarkRadioMd { get; set; }
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
        

    }
}
