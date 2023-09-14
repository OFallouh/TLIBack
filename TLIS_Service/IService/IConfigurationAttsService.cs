using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;

namespace TLIS_Service.IService
{
    public interface IConfigurationAttsService
    {
        Response<List<ConfigurationListViewModel>> GetConfigrationTables(string filterName);
        Response<List<ConfigurationAttsViewModel>> GetAll(string TableName, ParameterPagination Pagination);
        Response<ConfigurationAttsViewModel> GetById(string TableName, int Id);
        Response<ConfigurationAttsViewModel> Add(AddConfigrationAttViewModel viewModel);
        Task<Response<ConfigurationAttsViewModel>> Update(ConfigurationAttsViewModel viewModel);
        Task<Response<ConfigurationAttsViewModel>> Disable(string TableName, int Id);
        Task<Response<ConfigurationAttsViewModel>> Delete(string TableName, int Id);
    }
}
