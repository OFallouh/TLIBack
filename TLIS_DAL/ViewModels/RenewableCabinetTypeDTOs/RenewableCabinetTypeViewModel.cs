using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.RenewableCabinetTypeDTOs
{
    public class RenewableCabinetTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public bool Disable { get; set; }
    }
}
