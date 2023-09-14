using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_RFUDTOs
{
    public class MW_RFULibInst
    {
        public MW_RFUViewModel MW_RFU { get; set; }
        public MW_RFULibraryViewModel MW_RFULibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }
    }
}
