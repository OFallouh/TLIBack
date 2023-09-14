using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.AttributeViewManagmentDTOs;

namespace TLIS_Service.IService
{
    public interface IAttributeViewManagmentService
    {
        Response<List<AttributeViewManagmentViewModel>> GetAttributesForViewWithoutPagination(string ViewName, string Search);
        // Task<Response<List<AttributeViewManagmentViewModel>>> GetAllAttributes();
        Task<Response<AttributeViewManagmentViewModel>> UpdateAttributeStatus(int AttributeViewManagmentId);
        Response<List<AttributeViewManagmentViewModel>> GetAttributesForView(string ViewName, ParameterPagination parameterPagination, string AttributeName);
    }
}
