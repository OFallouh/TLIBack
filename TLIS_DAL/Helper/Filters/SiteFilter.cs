using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.Helper.Filters
{
    public class SiteFilter
    {
        [Required]
        public string siteCode { get; set; }
        
        
    }
}
