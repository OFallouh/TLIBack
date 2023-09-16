using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class SiteViewModel
    {
        public string SiteCode { get; set; }
        [Required]
        public string SiteName { get; set; }

        public int LocationTypeId { get; set; }
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


    }
}
