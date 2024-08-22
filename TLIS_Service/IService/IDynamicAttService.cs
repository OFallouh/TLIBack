using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AttActivatedCategoryDTOs;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.DependencyDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.OperationDTOs;
using static TLIS_Service.Services.DynamicAttService;

namespace TLIS_Service.IService
{
    public interface IDynamicAttService
    {
        Response<DynamicAttributeValidations> GetDynamicAttributeValidation(int DynamicAttId);
        Task<Response<AddDynamicObject>> EditDynamicAttribute(int DynamicAttributeId, AddDynamicObject DynamicAttViewModel, int UserId, string connectionString);
        Response<AddDynamicObject> AddDynamic(AddDynamicObject addDynamicObject, string connectionString, string TabelName, int UserId, int? CategoryId);
        Response<GetForAddDynamicAttribute> GeStaticAttsAndDynamicAttsByTableName(string TabelName, bool IsLibrary, int? CategoryId);
        Response<IEnumerable<AttributeActivatedViewModel>> GetAttributes(string TableName);
        Response<AddDynamicLibAttValueViewModel> AddDynamicAttLibValue(AddDynamicLibAttValueViewModel addDynamicLibAttValueViewModel);
        Response<AddDependencyInstViewModel> AddDynamicAttInst(AddDependencyInstViewModel addDependencyInstViewModel, string ConnectionString);
        // Response<IEnumerable<DependencyColumn>> GetDependencyLib(string tableName, string RecordId);
        Response<AddDependencyViewModel> AddDynamicAtts(AddDependencyViewModel addDependencyViewModel, string ConnectionString);
        Response<DependencyColumnForAdd> GetDependencyLib(string tableName, int? CategoryId);
        Response<DynamicAttLibForAddViewModel> GetForAdd();
        Response<ReturnWithFilters<DynamicAttViewModel>> GetDynamicAtts(List<FilterObjectList> filters, ParameterPagination parameters);
        Response<ReturnWithFilters<DynamicAttViewModel>> GetDynamicAttsByTableName(List<FilterObjectList> filters, ParameterPagination parameters, string TableName, int? CategoryId);
        Response<DynamicAttViewModel> GetById(int Id);
        Task<Response<DynamicAttViewModel>> Edit(EditDynamicAttViewModel dynamicAttViewModel,string connectionString);
        Response<DependencyColumnForAdd> GetDependencyInst(string Layer, int? CategoryId, bool IsLibrary = false);
        //Response<DynamicAttLibForAddViewModel>
        Response<DynamicAttViewModel> Disable(int RecordId, string ConnectionString);
        Response<DynamicAttViewModel> RequiredNOTRequired(int DynamicAttId, string ConnectionString);
        Response<List<OutPutString>> GetLayers(string TableName);
        Response<FirstStepAddDependencyViewModel> GetForAddingDynamicAttribute(string TableName);
        Response<DynamicAttViewModel> CheckEditingDynamicAttDataType(int DynamicAttributeId, int NewDataTypeId);
        Response<AddDynamicObject> GetDynamicLibraryById(int id);
    }
}
