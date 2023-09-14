using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.BaseCivilWithLegsTypeDTOs;

namespace TLIS_Service.IService
{
    public interface IBaseCivilWithLegsTypeService
    {
        Task<Response<IEnumerable<BaseCivilWithLegsTypeViewModel>>> GetBaseCivilWithLegsTypes(ParameterPagination parameterPagination, List<FilterObjectList> filter = null);
        Task<Response<BaseCivilWithLegsTypeViewModel>> GetBaseCivilWithLegsType(int Id);
        Task<Response<BaseCivilWithLegsTypeViewModel>> AddBaseCivilWithLegsType(AddBaseCivilWithLegsTypeViewModel addBaseCivil);
        Task<Response<BaseCivilWithLegsTypeViewModel>> EditBaseCivilWithLegsType(EditBaseCivilWithLegsTypeViewModel editBaseCivil);
    }
}
