using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_ODUDTOs
{
    public class MW_ODUViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Serial_Number { get; set; }
        public string Notes { get; set; }
        public float? Height { get; set; }
        public string Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public int? OwnerId { get; set; }
        public string Owner_Name { get; set; }
        public int? Mw_DishId { get; set; }
        public float EquivalentSpace { get; set; }
        public string Mw_Dish_Name { get; set; }
        public int? OduInstallationTypeId { get; set; }
        public string OduInstallationType_Name { get; set; }
        public int MwODULibraryId { get; set; }
        public string MwODULibrary_Name { get; set; }
        public ODUConnections ODUConnections { get; set; }

        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
    }
}
