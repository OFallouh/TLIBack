using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs
{
    public class OtherInventoryDistanceViewModel
    {
        public int Id { get; set; }
        public int? OtherInventoryID1 { get; set; }
        public int? OtherInventoryID2 { get; set; }
        public float? Distance { get; set; }
        public float? Azimuth { get; set; }
    }
}
