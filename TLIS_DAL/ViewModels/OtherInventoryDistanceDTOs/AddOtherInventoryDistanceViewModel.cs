using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs
{
    public class AddOtherInventoryDistanceViewModel
    {
        public int? ReferenceOtherInventoryId { get; set; }
        public float Distance { get; set; } = 0;
        public float Azimuth { get; set; } = 0;
    }
}
