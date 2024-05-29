using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;

namespace TLIS_Service.IService
{
    public interface IConfigurationAttsService
    {
        Response<List<ConfigurationListViewModel>> GetConfigrationTables(string filterName);
        Response<List<ConfigurationAttsViewModel>> GetAll(string TableName, ParameterPagination Pagination);
        Response<ConfigurationAttsViewModel> GetById(string TableName, int Id);
        Task<Response<ConfigurationAttsViewModel>> Add(string TabelName, string ListName, string NewName, int UserId);
        Task<Response<List<TableAffected>>> Delete(string TabelName, int RecordId, int UserId, string ListName);
        Task<Response<List<TableAffected>>> Disable(string TabelName, int RecordId, string ListName, int UserId);
        Task<Response<ConfigurationAttsViewModel>> Update(string TabelName, string ListName, int RecordId, string NewName, int UserId);
    }
}
