using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLImwOther
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public string VisibleStatus { get; set; }
        public TLImwOtherLibrary mwOtherLibrary { get; set; }
        public int mwOtherLibraryId { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public float Spaceinstallation { get; set; }
        public TLIinstallationPlace InstallationPlace { get; set; }
        public int InstallationPlaceId { get; set; }
        public IEnumerable<TLIallLoadInst> allLoadInsts { get; set; }
    }
}
