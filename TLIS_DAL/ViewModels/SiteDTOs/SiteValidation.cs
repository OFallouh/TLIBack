using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
   public class SiteValidation
    {
       
        
            public string SiteCode { get; set; }
            public string SiteName { get; set; }

            public SiteValidation(string code, string name)
            {
                SiteCode = code;
                SiteName = name;
            }
        
    }
}
