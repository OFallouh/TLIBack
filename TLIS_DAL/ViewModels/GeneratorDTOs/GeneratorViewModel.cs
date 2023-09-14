using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.GeneratorDTOs
{
    public class GeneratorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public int? NumberOfFuelTanks { get; set; }
        public string LocationDescription { get; set; }
        public bool? BaseExisting { get; set; }
        public float SpaceInstallation { get; set; }
        public string VisibleStatus { get; set; }
        public int? BaseGeneratorTypeId { get; set; } = 0;
        public string BaseGeneratorType_Name { get; set; }
        public int GeneratorLibraryId { get; set; }
        public string GeneratorLibrary_Name { get; set; }
    }
}
