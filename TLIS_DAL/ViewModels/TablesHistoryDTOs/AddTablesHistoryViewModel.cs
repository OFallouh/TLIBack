using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.HistoryDetails;

namespace TLIS_DAL.ViewModels.TablesHistoryDTOs
{
    public class AddTablesHistoryViewModel
    {
        public int Id { get; set; }
        public int HistoryTypeId { get; set; }
        public int UserId { get; set; }
        public int RecordId { get; set; }
        public int TablesNameId { get; set; }

        public DateTime Date { get; set; }
        public int? PreviousHistoryId { get; set; }
        public AddHistoryDetailsViewModel HistoryDetails { get; set; }
    }
}
