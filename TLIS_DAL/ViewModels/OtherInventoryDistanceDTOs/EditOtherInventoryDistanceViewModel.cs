using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs
{
    public class EditOtherInventoryDistanceViewModel
    {
        //public int Id { get; set; }
        //public int? OtherInventoryID1 { get; set; }
        //public int? OtherInventoryID2 { get; set; }
        public int? ReferenceOtherInventoryId { get; set; } = 0;
        public float? Distance { get; set; } = 0;
        public float? Azimuth { get; set; } = 0;
    }
}
