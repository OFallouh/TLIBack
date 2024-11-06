using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class SiteViewModelForGetAll
    {
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string LocationType { get; set; }
        public float LocationHieght { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Status { get; set; }
        public string CityName { get; set; }
        public string Area { get; set; }
        public string Region { get; set; }
        public double RentedSpace { get; set; }
        public double ReservedSpace { get; set; }
        public bool isUsed { get; set; }
        public DateTime SiteVisiteDate { get; set; }
        public ItemsOnSite ItemsOnSite { get; set; }
    }
}
