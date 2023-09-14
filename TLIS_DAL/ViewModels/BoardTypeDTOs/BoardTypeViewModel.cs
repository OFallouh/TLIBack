using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.MW_RFUDTOs;

namespace TLIS_DAL.ViewModels.BoardTypeDTOs
{
    public class BoardTypeViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<MW_RFULibraryViewModel> MW_RFU { get; set; }
    }
}
