using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.Models
{
    public enum ODUConnections
    {
        SeparateMount,
        DirectMount
    }
    public class TLImwODU
    {
        public int Id { get; set; }
        public string Serial_Number { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public float? Height { get; set; }
        public ODUConnections ODUConnections { get; set; }
        public string Visiable_Status { get; set; }
        public float SpaceInstallation { get; set; }
        public TLIowner Owner { get; set; }
        public int? OwnerId { get; set; }
        public TLImwDish  Mw_Dish { get; set; }
        public int? Mw_DishId { get; set; }
        public TLIoduInstallationType OduInstallationType { get; set; }
        public int? OduInstallationTypeId { get; set; }
        public TLImwODULibrary MwODULibrary { get; set; }
        public int MwODULibraryId { get; set; }

        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }

        public IEnumerable<TLIallLoadInst> allLoadInsts { get; set; }
    }
}
