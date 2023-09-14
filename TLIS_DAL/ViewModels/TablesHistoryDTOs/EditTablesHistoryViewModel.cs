using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.TablesHistoryDTOs
{
    public class EditTablesHistoryViewModel
    {
        public int Id { get; set; }
        public int HistoryTypeId { get; set; }
        public int UserId { get; set; }
        public int RecordId { get; set; }
        public string TableName { get; set; }
        public DateTime Date { get; set; }
    }
}
