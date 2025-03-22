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

        // New Columns
        public string CivilCollectDoneBy { get; set; }
        public string OMCollectDoneBy { get; set; }
        public string MWCollectPendingImplBy { get; set; }
        public string MWMDPendingImplBy { get; set; }
        public string PowerMDPendingOMBy { get; set; }
        public string RadioCollectPendingOMBy { get; set; }
        public string RadioMDPendingOMBy { get; set; }
        public string RadioCollectPendingCivilBy { get; set; }
        public string RadioMDPendingCivilBy { get; set; }
        public string MWMdImplDoneBy { get; set; }
        public string MWMdCivilDoneBy { get; set; }
        public string PowerMdOMDoneBy { get; set; }
        public string RadioMdOMDoneBy { get; set; }
    }
}
