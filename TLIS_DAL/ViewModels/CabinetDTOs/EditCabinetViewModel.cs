using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
using TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs;

namespace TLIS_DAL.ViewModels.CabinetDTOs
{
    public class EditCabinetViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TPVersion { get; set; }
        public int? RenewableCabinetNumberOfBatteries { get; set; }
        public int? NUmberOfPSU { get; set; }
        public float SpaceInstallation { get; set; }
        public string VisibleStatus { get; set; }
        public int? CabinetPowerLibraryId { get; set; }
        public int? CabinetTelecomLibraryId { get; set; }
        public int? RenewableCabinetTypeId { get; set; } = 0;
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public EditOtherInSiteViewModel TLIotherInSite { get; set; }
        public EditOtherInventoryDistanceViewModel TLIotherInventoryDistance { get; set; }
        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }
    }
}
