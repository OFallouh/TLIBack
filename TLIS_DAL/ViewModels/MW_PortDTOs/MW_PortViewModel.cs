using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.MW_PortDTOs
{
    public class MW_PortViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TX_Frequency { get; set; }
        public int MwBUId { get; set; }

        public int Port_Type { get; set; }
    }
}
