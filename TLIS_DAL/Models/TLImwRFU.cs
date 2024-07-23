using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLImwRFU
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Azimuth { get; set; }
        public float heightBase { get; set; }
        public string? SerialNumber { get; set; }
        public string Note { get; set; }
        public TLIowner Owner { get; set; }
        public int? OwnerId { get; set; }
        public TLImwRFULibrary MwRFULibrary { get; set; }
        public int MwRFULibraryId { get; set; }
        public TLImwPort MwPort { get; set; }
        public int? MwPortId { get; set; }
        public float SpaceInstallation { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public IEnumerable<TLIallLoadInst> allLoadInsts { get; set; }
    }
}
