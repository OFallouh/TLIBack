using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.CabinetDTOs
{
    public class CabinetViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TPVersion { get; set; }
        public int? RenewableCabinetNumberOfBatteries { get; set; }
        public int? NUmberOfPSU { get; set; }
        public int? CabinetPowerLibraryId { get; set; }
        public string CabinetPowerLibrary_Name { get; set; }
        public int? CabinetTelecomLibraryId { get; set; }
        public string CabinetTelecomLibrary_Name { get; set; }
        public int? RenewableCabinetTypeId { get; set; }
        public string RenewableCabinetType_Name { get; set; }
        public float SpaceInstallation { get; set; }
        public string VisibleStatus { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
    }
}
