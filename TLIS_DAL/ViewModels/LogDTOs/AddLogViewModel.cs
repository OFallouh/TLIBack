using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.LogDTOs
{
   public class AddLogViewModel
    {
  
        public int? UserId { get; set; }
        public string Action { get; set; }
        public string Table { get; set; }
        public int? RecordId { get; set; }
    }
}
