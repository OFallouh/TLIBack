using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.SupportTypeImplementedDTOs;

namespace TLIS_Service.IService
{
   public  interface ISupportTypeImplementedService
    {
        Task<Response<IEnumerable<SupportTypeImplementedViewModel>>> GetSupportTypesImplemented(ParameterPagination parameterPagination,List<FilterObjectList> filters = null);
        Task<Response<SupportTypeImplementedViewModel>> GetById(int Id);
        Task<Response<SupportTypeImplementedViewModel>> AddSupportTypeImplemented(AddSupportTypeImplementedViewModel addSupportTypeImplemented);
        Task<Response<SupportTypeImplementedViewModel>> EditSupportTypeImplemented(EditSupportTypeImplementedViewModel editSupportTypeImplemented);
    }
}
