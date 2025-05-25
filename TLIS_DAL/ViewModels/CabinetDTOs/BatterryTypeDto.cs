using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.CabinetDTOs
{
    public class BatterryTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Delete { get; set; }
        public bool Disable { get; set; }
    }
}
