using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.MW_BUDTOs
{
    public class MW_BUViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string Serial_Number { get; set; }
        public float Height { get; set; }
        public string Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public int? OwnerId { get; set; }
        public float EquivalentSpace { get; set; }
        public string Owner_Name { get; set; }
        public int MwBULibraryId { get; set; }
        public string MwBULibrary_Name { get; set; }
        public int? MainDishId { get; set; }
        public string MainDish_Name { get; set; }
        public int? SdDishId { get; set; }
        public string SdDish_Name { get; set; }
        public float Azimuth { get; set; }
        public int BUNumber { get; set; }
        public bool Active { get; set; }
        public string BaseBU_Name { get; set; }
        public int BaseBUId { get; set; }
        public string InstallationPlace_Name { get; set; }
        public int InstallationPlaceId { get; set; }
    }
}
