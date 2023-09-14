using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;

namespace TLIS_DAL.ViewModels.LoadOtherDTOs
{
    public class LoadsOtherDisplayedOnTableViewModel
    {
        public LoadOtherViewModel LoadsOtherInstallation { get; set; }
        public LoadOtherLibraryViewModel LoadsOtherLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }
    }
}
