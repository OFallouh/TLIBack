using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIgenerator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public int NumberOfFuelTanks { get; set; }
        public string? LocationDescription { get; set; }
        public bool BaseExisting { get; set; }
        public float SpaceInstallation { get; set; }
        public string? VisibleStatus { get; set; }
        public TLIbaseGeneratorType BaseGeneratorType { get; set; }
        public int? BaseGeneratorTypeId { get; set; }
        public TLIgeneratorLibrary GeneratorLibrary { get; set; }
        public int GeneratorLibraryId { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }

        public IEnumerable<TLIallOtherInventoryInst> allOtherInventoryInsts { get; set; }

        /////VisibleStatus////////
    }
}
