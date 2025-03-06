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
        public string MWMdStatus { get; set; }
        public string RadioMdStatus { get; set; }
        public string PowerMdStatus { get; set; }
    }

}
