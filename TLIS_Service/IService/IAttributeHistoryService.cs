using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.StaticAttributesHistory;

namespace TLIS_Service.IService
{
    public interface IAttributeHistoryService
    {
        Response<ReturnWithFilters<StaticAttsHistoryViewModel>> GetStaticAttributesHistoryByTableName(List<FilterObjectList> ObjectAttributeFilters, string TableName, ParameterPagination parameters);
        Response<ReturnWithFilters<StaticAttsHistoryViewModel>> GetDynamicAttributesHistoryByTableName(List<FilterObjectList> ObjectAttributeFilters, string TableName, ParameterPagination parameters);
        Response<List<HistoryViewModel>> GetAttachedFileHistory( string TableName, int RecordId, ParameterPagination parameters);
    }
}
