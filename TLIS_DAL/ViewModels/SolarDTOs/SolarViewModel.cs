using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.SolarDTOs
{
    public class SolarViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PVPanelBrandAndWattage { get; set; }
        public string PVArrayAzimuth { get; set; }
        public string PVArrayAngel { get; set; }
        public string Prefix { get; set; }
        public string PowerLossRatio { get; set; }
        public int NumberOfSSU { get; set; }
        public int NumberOfLightingRod { get; set; }
        public int NumberOfInstallPVs { get; set; }
        public string LocationDescription { get; set; }
        public string ExtenstionDimension { get; set; }
        public string Extension { get; set; }
        public float SpaceInstallation { get; set; }
        public int SolarLibraryId { get; set; }
        public string SolarLibrary_Name { get; set; }
        public int? CabinetId { get; set; }
        public string Cabinet_Name { get; set; }
        public string VisibleStatus { get; set; }
    }
}
