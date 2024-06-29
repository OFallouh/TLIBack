using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIpower
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index(IsUnique = true)]
        public string SerialNumber { get; set; }
        [Index(IsUnique = true)]
        public string Name { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public float SpaceInstallation { get; set; }
        public float Azimuth { get; set; }
        public float Height { get; set; }
        public string? Notes { get; set; }
        public TLIowner owner { get; set; }
        public int? ownerId { get; set; }
        public TLIinstallationPlace installationPlace { get; set; }
        public int installationPlaceId { get; set; }
        public TLIpowerLibrary powerLibrary { get; set; }
        public int powerLibraryId { get; set; }
        public TLIpowerType powerType { get; set; }
        public int? powerTypeId { get; set; }
        public string? VisibleStatus { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public IEnumerable<TLIallLoadInst> allLoadInsts { get; set; }
    }
}
