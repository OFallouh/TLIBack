using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.OwnerDTOs;

namespace TLIS_Service.IService
{
    public interface IOwnerService
    {
        Task<Response<IEnumerable<OwnerViewModel>>> GetOwners(ParameterPagination parameterPagination, List<FilterObjectList> filters = null);
        Response<OwnerViewModel> GetOwner(int Id);
        Task<Response<OwnerViewModel>> AddOwner(AddOwnerViewModel addOwner);
        Task<Response<OwnerViewModel>> EditOwner(EditOwnerViewModel editOwner);
    }
}
