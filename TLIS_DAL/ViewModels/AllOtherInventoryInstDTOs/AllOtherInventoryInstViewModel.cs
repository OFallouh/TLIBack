using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AllOtherInventoryInstDTOs
{
    public class AllOtherInventoryInstViewModel
    {
        public int Id { get; set; }
        public int? cabinetId { get; set; }
        public int? solarId { get; set; }
        public int? generatorId { get; set; }
    }
}
