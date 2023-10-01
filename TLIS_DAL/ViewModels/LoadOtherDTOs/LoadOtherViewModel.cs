using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.LoadOtherDTOs
{
    public class LoadOtherViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public float EquivalentSpace { get; set; }
        public int loadOtherLibraryId { get; set; }
        public string loadOtherLibrary_Name { get; set; }
        public float SpaceInstallation { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
    }
}
