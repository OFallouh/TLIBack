using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.CivilSupportDistanceDTOs
{
    public class CivilSupportDistanceViewModel
    {
        public int Id { get; set; }
        public float? Distance { get; set; }
        public float? Azimuth { get; set; }
        public int CivilInstId { get; set; }
        public int ReferenceCivilId { get; set; }
        public string SiteCode { get; set; }
    }
}
