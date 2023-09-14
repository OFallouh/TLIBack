using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;

namespace TLIS_Service.IService
{
    public interface ITablesHistoryService
    {
        Response<TablesHistoryViewModel> GetTablesHistory();
        Response<TablesHistoryViewModel> AddTableHistory(AddTablesHistoryViewModel addTablesHistorym, int UserId);
    }
}
