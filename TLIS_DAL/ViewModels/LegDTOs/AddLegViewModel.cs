using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.LegDTOs
{
   public class AddLegViewModel
    {
        public string CiviLegName { get; set; }
        public string LegLetter { get; set; }
        public float LegAzimuth { get; set; } = 0;
        public string? Notes { get; set; }
    }
}
