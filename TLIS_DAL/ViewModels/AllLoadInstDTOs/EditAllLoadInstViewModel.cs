using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AllLoadInstDTOs
{
    public class EditAllLoadInstViewModel
    {
        public int Id { get; set; }
        public int mwBUId { get; set; }
        public int mwDishId { get; set; }
        public int mwODUId { get; set; }
        public int mwRFUId { get; set; }
        public int mwOtherId { get; set; }
        public int radioAntennaId { get; set; }
        public int radioRRUId { get; set; }
        public int radioOtherId { get; set; }
        public int powerId { get; set; }
        public int loadOtherId { get; set; }
    }
}
