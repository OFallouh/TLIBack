using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.LegDTOs
{
   public class EditLegViewModel
    {
        public int Id { get; set; }
        public string CiviLegName { get; set; }
        public string LegLetter { get; set; }
        public float LegAzimuth { get; set; }
        public string Notes { get; set; }
        public int CivilWithLegInstId { get; set; }
    }
}
