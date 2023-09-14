using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AllCivilInstDTOs
{
    public class AllCivilInstViewModel
    {
        public int Id { get; set; }
        public int? civilWithLegsId { get; set; }
        public int? civilWithoutLegId { get; set; }
        public int? civilNonSteelId { get; set; }
    }
}
