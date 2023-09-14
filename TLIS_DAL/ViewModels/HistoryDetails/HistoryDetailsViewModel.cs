using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.HistoryDetails
{
    public class HistoryDetailsViewModel
    {
        public int Id { get; set; }
        public int TablesHistoryId { get; set; }
        public string AttName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
