using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class GetAllsiteOnMultiRegion
    {       
         public string RegionCode { get; set; }
         public string RegionName { get; set; }
        public List<SiteForGetViewModel> siteForGetViewModels { get; set; }
    }
}
