using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.GuyLineTypeDTOs;

namespace TLIS_Service.IService
{
    public interface IGuyLineTypeService
    {
        Task<Response<IEnumerable<GuyLineTypeViewModel>>> GetGuyLineTypes(ParameterPagination parameterPagination, List<FilterObjectList> filters = null);
        Response<GuyLineTypeViewModel> GetGuyLineType(int Id);
        Task<Response<GuyLineTypeViewModel>> AddGuyLineType(AddGuyLineTypeViewModel addGuyLine);
        Task<Response<GuyLineTypeViewModel>> EditGuyLineType(EditGuyLineTypeViewModel editGuyLine);
    }
}
