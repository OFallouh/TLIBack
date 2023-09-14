using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.MW_PortDTOs
{
    public class EditMW_PortViewModel
    {
        public int Id { get; set; }
        public string Port_Name { get; set; }
        public string TX_Frequency { get; set; }
        public int MwBUId { get; set; }
    }
}
