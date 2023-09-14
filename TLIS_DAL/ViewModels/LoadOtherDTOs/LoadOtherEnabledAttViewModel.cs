using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CabinetDTOs;

namespace TLIS_DAL.ViewModels.LoadOtherDTOs
{
    public class LoadOtherEnabledAttViewModel
    {
        public dynamic LoadOtherViewModel { get; set; }
        public dynamic LoadOtherLibraryViewModel { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }
    }
}
