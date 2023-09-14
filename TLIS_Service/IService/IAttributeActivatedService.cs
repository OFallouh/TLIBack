using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.TablesNamesDTOs;

namespace TLIS_Service.IService
{
    public interface IAttributeActivatedService
    {
        Response<IEnumerable<TLIattributeActivated>> GetStaticAttsWithoutPagination(int? CivilWithoutLegCategoryId, string AttributeName, string TableName);
        Task<Response<AttributeActivatedViewModel>> Disable(int AttributeActivatedId, int? CivilWithoutLegCategoryId);
        Response<TablesNamesViewModel> GetTableNameByAttributeId(int AttributeId, bool IsDynamic);
        Task<Response<AttributeActivatedViewModel>> RequiredNOTRequired(int Id, int? CivilWithoutLegCategoryId);
        Response<IEnumerable<TLIattributeActivated>> GetStaticAtts(int? CivilWithoutLegCategoryId, string AttributeName, ParameterPagination parameters, string TableName);
        // IEnumerable<TLIattributeActivated> GetAttributeActivated(ParameterPagination parameterPagination, AttributeActivatedFilter AttributeActivatedFilter,string TableName);
        Response<AttributeActivatedViewModel> GetById(int Id);
        //Task AddAttributeActivated(AddAttributeActivatedViewModel addAttributeActivatedViewModel);
        Task<Response<AttributeActivatedViewModel>> EditAttributeActivated(EditAttributeActivatedViewModel editAttributeActivatedViewModel, int? CivilWithoutLegCategoryId);
        Task AddTablesActivatedAttributes();
    }
}
