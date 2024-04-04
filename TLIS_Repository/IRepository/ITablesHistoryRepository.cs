using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.StaticAttributesHistory;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ITablesHistoryRepository:IRepositoryBase<TLItablesHistory, TablesHistoryViewModel, int>
    {
        void AddTableHistory(int RecordId, int TableNameId, int UserId, int HistoryTypeId, List<Tuple<string, string, string>> values = null);
        void AddTableHistory(AddTablesHistoryViewModel addModel, List<Tuple<string, string, string>> values = null);
        int AddTableHistory(AddTablesHistoryViewModel addModel, List<TLIhistoryDetails> details);
        //List<StaticAttsHistoryViewModel> GetDynamicAttributesHistory(string TableName, ParameterPagination parameterPagination);
        //List<StaticAttsHistoryViewModel> GetStaticAttributesHistory(string TableName, ParameterPagination parameterPagination);
        //List<HistoryViewModel> GetAttachedFileHistory( string TableName, int RecordId, ParameterPagination parameterPagination);
        EditHistoryDetails CheckUpdateObject(object originalObj, object updateObj);
        int AddHistoryForEdit(int RecordId, int TableNameid, string HistoryType, List<TLIhistoryDetails> details);
        void AddHistory(int MW_lib_id, string historyType, string TableName);
    }
}
