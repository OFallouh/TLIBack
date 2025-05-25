using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIcabinet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? TPVersion { get; set; }
        public int RenewableCabinetNumberOfBatteries { get; set; }
        public int NUmberOfPSU { get; set; }
        public float SpaceInstallation { get; set; }
        public string? VisibleStatus { get; set; }
        public TLIcabinetPowerLibrary CabinetPowerLibrary { get; set; }
        public int? CabinetPowerLibraryId { get; set; }
        public TLIcabinetTelecomLibrary CabinetTelecomLibrary { get; set; }
        public int? CabinetTelecomLibraryId { get; set; }
        public TLIrenewableCabinetType RenewableCabinetType { get; set; }
        public int? RenewableCabinetTypeId { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }

        public TLIBatteryType BatterryType { get; set; }
        public int? BatterryTypeId { get; set; }

        public IEnumerable<TLIsolar> Solars { get; set; }
        public IEnumerable<TLIallOtherInventoryInst> allOtherInventoryInsts { get; set; }
    }
}
