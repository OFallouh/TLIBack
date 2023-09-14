using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;

namespace TLIS_DAL.ViewModels.RadioRRUDTOs
{
    public class RadioRRULibInst
    {
        public RadioRRUViewModel RadioRRU { get; set; }
        public RadioRRULibraryViewModel RadioRRULibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }
    }
}
