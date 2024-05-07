using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLImwDish
    {
        [Key]
        public int Id { get; set; }
        public float Azimuth { get; set; }
        public string? Notes { get; set; }
        public string? Far_End_Site_Code { get; set; }
        public string HBA_Surface { get; set; }
        public bool Is_Repeator { get; set; }
        public string Serial_Number { get; set; }
        public string DishName { get; set; }
        public int? MW_LinkId { get; set; }
        public string? Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public string Temp { get; set; }
        public TLIowner owner { get; set; }
        public int? ownerId { get; set; }
        public TLIrepeaterType RepeaterType { get; set; }
        public int? RepeaterTypeId { get; set; }
        public TLIpolarityOnLocation PolarityOnLocation { get; set; }
        public int PolarityOnLocationId { get; set; }
        public TLIitemConnectTo ItemConnectTo { get; set; }
        public int ItemConnectToId { get; set; }
        public TLImwDishLibrary MwDishLibrary { get; set; }
        public int MwDishLibraryId { get; set; }
        public TLIinstallationPlace InstallationPlace { get; set; }
        public int InstallationPlaceId { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public IEnumerable<TLIallLoadInst> allLoadInsts { get; set; }
        public IEnumerable<TLImwBU> mwBUs { get; set; }

        public IEnumerable<TLImwODU> mwODUs { get; set; }
    }
}
