using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.CivilWithLegsDTOs
{
    public class SiteBaseFilter
    {
        [Required]
        public string SiteCode { get; set; }
        public int? ItemStatusId { get; set; }
        public int? TicketId { get; set; }
    }
}

