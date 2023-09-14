using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class AddDynamicAttInstViewModel
    {
        public string Key { get; set; }
        public bool LibraryAtt { get; set; }
        public int? DataTypeId { get; set; }
        //public int CiviId { get; set; }
        //public int CivilTypeId { get; set; }
        public List<ValidationViewModel> validations { get; set; } 
    }
}
