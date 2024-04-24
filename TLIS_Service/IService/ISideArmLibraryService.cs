using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;

namespace TLIS_Service.IService
{
    public interface ISideArmLibraryService
    {
        void SeedDataForTest();
        Task<Response<IEnumerable<SideArmLibraryViewModel>>> GetSideArmLibraries(List<FilterObjectList> filters, ParameterPagination parameters);
        Response<List<KeyValuePair<string, int>>> GetSideArmLibs();
        Response<AddSideArmLibraryObject> AddSideArmLibrary(AddSideArmLibraryObject addSideArmLibraryViewModel, string connectionString, int UserId);
        Task<Response<EditSideArmLibraryObject>> EditSideArmLibrary(EditSideArmLibraryObject editSideArmLibraryViewModel, int UserId);
        Response<GetForAddCivilLibrarybject> GetSideArmLibraryById(int Id);
        Task<Response<SideArmLibraryViewModel>> Disable(int id);
        Task<Response<SideArmLibraryViewModel>> Delete(int id);
        Response<GetForAddCivilLibrarybject> GetForAdd();
        Response<GetEnableAttribute> GetSideArmLibrariesEnabledAtt(string ConnectionString);
        Response<ReturnWithFilters<object>> GetSideArmLibrariesWithEnabledAttributes(CombineFilters CombineFilters, ParameterPagination parameterPagination);
    }
}
