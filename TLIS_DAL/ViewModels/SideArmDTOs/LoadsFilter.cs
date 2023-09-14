using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.SideArmDTOs
{
    public class LoadsFilter
    {
        [Required]
        public string SiteCode { get; set; }
        public int CivilId { get; set; }
        public int SideArmId { get; set; }
        public int ItemStatusId{ get; set; }
        public int TicketId { get; set; }
    }
}
