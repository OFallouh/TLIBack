using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class GetAllsiteOnMultiAreaViewModel
    {
        public int Id { get; set; }
        public string AreaName { get; set; }
        public List<SiteForGetViewModel> siteForGetViewModels { get; set; }
    }
}
