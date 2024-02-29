using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.CivilSupportDistanceDTOs
{
    public class AddCivilSupportDistanceViewModel
    {
        public float Distance { get; set; } = 0;
        public float Azimuth { get; set; } = 0;
        public int? ReferenceCivilId { get; set; }
    }
}
