using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.Mw_OtherDTOs
{
    public class Mw_OtherViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public string VisibleStatus { get; set; }
        public float EquivalentSpace { get; set; }
        public int mwOtherLibraryId { get; set; }
        public string mwOtherLibrary_Name { get; set; }
    }
}
