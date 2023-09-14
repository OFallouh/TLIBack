using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CabinetDTOs;

namespace TLIS_DAL.ViewModels.MW_BUDTOs
{
    public class MW_BUEnabledAttViewModel
    {
        public dynamic MW_BUViewModel { get; set; }
        public dynamic MW_BULibraryViewModel { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }
    }
}
