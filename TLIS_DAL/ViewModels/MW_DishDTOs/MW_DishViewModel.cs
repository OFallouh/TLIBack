using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_DishDTOs
{
    public class MW_DishViewModel
    {
        public int Id { get; set; }
        public string DishName { get; set; }
        public float Azimuth { get; set; }
        public string Notes { get; set; }
        public string Far_End_Site_Code { get; set; }
        public string HBA_Surface { get; set; }
        public bool Is_Repeator { get; set; }
        public string Serial_Number { get; set; }
        public string MW_LinkId { get; set; }
        public string Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public float EquivalentSpace { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public string Temp { get; set; }
        public int? ownerId { get; set; }
        public string owner_Name { get; set; }
        public int? RepeaterTypeId { get; set; }
        public string RepeaterType_Name { get; set; }
        public int? PolarityOnLocationId { get; set; }
        public string PolarityOnLocation_Name { get; set; }
        public int? ItemConnectToId { get; set; }
        public string ItemConnectTo_Name { get; set; }
        public int MwDishLibraryId { get; set; }
        public string MwDishLibrary_Name { get; set; }
        public int? InstallationPlaceId { get; set; }
        public string InstallationPlace_Name { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
    }
}
