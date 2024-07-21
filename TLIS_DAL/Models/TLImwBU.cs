using Castle.Components.DictionaryAdapter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLImwBU
    {
        public int Id { get; set; }
        public string? Notes { get; set; }
        public string Name { get; set; }
        public string Serial_Number { get; set; }
        public float Height { get; set; }
        public float Azimuth { get; set; }
        public int BUNumber { get; set; }    
        public bool Active { get; set; }
        public string? Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public TLIbaseBU baseBU { get; set; }
        public int? BaseBUId { get; set; }
        public TLIinstallationPlace InstallationPlace { get; set; }
        public int InstallationPlaceId { get; set; }
        public TLIowner Owner { get; set; }
        public int? OwnerId { get; set; }
        public TLImwBULibrary MwBULibrary { get; set; }
        public int MwBULibraryId { get; set; }
        public TLImwDish MainDish { get; set; }
        public int MainDishId { get; set; }
        public int? SdDishId { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public int? PortCascadeId { get; set; }
        public IEnumerable<TLImwPort> MwPort { get; set; }
        public IEnumerable<TLIallLoadInst> allLoadInsts { get; set; }
    }
}
