using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
   
    public class SiteDetailsView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string PlanType { get; set; }
        public string CollectDataStatus { get; set; }
        public string MWMDStatus { get; set; }
        public string RadioMDStatus { get; set; }
        public string PowerMDStatus { get; set; }
        public string CollectDataPendingType { get; set; }
        public string MWMDPendingType { get; set; }
        public string RadioMDPendingType { get; set; }
        public string PowerMDPendingType { get; set; }
        public string MWValidationRemark { get; set; }
        public string RadioValidationRemark { get; set; }
        public string PowerValidationRemark { get; set; }
        public string userName { get; set; }
    }
}
