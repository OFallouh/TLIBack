using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class LoadsOnSiteFilter
    {
        [Required]
        public string SiteCode { get; set; }
        public int? ItemStatusId { get; set; }
        public int? TicketId { get; set; }
        public int? PartId { get; set; }
        public int? TablesNameId { get; set; }
        public int? SideArmId { get; set; }
        public int? AllCivilId { get; set; }
    }
}
