using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.CivilNonSteelDTOs
{
    public class CivilNonSteelTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Disable { get; set; }
        public bool Deleted { get; set; }
    }
}
