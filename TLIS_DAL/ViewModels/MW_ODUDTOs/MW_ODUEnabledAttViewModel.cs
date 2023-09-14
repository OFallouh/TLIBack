using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CabinetDTOs;

namespace TLIS_DAL.ViewModels.MW_ODUDTOs
{
    public class MW_ODUEnabledAttViewModel
    {
        public dynamic MW_ODUViewModel { get; set; }
        public dynamic MW_ODULibraryViewModel { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }
    }
}
