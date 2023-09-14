using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;

namespace TLIS_DAL.ViewModels.MW_ODUDTOs
{
    public class EditMW_ODUViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Serial_Number { get; set; }
        public string Notes { get; set; }
        public float? Height { get; set; }
        public ODUConnections ODUConnections { get; set; }
        public string Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public int? OwnerId { get; set; } = 0;
        public int? Mw_DishId { get; set; } = 0;
        public int? OduInstallationTypeId { get; set; } = 0;
        public int? MwODULibraryId { get; set; }
        public EditCivilLoadsViewModel TLIcivilLoads { get; set; }
        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }
    }
}
