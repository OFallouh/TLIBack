using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_ODUDTOs
{
    public class MW_ODULibInst
    {
        public MW_ODUViewModel MW_ODU { get; set; }
        public MW_ODULibraryViewModel MW_ODULibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }
    }
}
