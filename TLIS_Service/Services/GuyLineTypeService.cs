using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GuyLineTypeDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class GuyLineTypeService : IGuyLineTypeService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public GuyLineTypeService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        //Function take 1 parameter addGuyLine
        //Function check name for add then add GuyLine if the name not exist
        public async Task<Response<GuyLineTypeViewModel>> AddGuyLineType(AddGuyLineTypeViewModel addGuyLine)
        {
            try
            {
                if (CheckNameForAdd(addGuyLine.Name))
                {
                    var GuyLineTypeEntity = _mapper.Map<TLIguyLineType>(addGuyLine);
                    await _unitOfWork.GuyLineTypeRepository.AddAsync(GuyLineTypeEntity);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<GuyLineTypeViewModel>();
                }
                else
                {
                    return new Response<GuyLineTypeViewModel>(true, null, null, $"This GuyLine Type {addGuyLine.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch(Exception err)
            {
                
                return new Response<GuyLineTypeViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter 
        //Function check name for update if not exist then update
        public async Task<Response<GuyLineTypeViewModel>> EditGuyLineType(EditGuyLineTypeViewModel editGuyLine)
        {
            try
            {
                if (await CheckNameForUpdate(editGuyLine.Name,editGuyLine.Id))
                {
                    var GuyLineTypeEntity = _mapper.Map<TLIguyLineType>(editGuyLine);
                    _unitOfWork.GuyLineTypeRepository.Update(GuyLineTypeEntity);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<GuyLineTypeViewModel>();
                }
                else
                {
                    return new Response<GuyLineTypeViewModel>(true, null, null, $"this GuyLineType {editGuyLine.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<GuyLineTypeViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter Id
        //Function get Guy Line Type by Id
        public Response<GuyLineTypeViewModel> GetGuyLineType(int Id)
        {
            try
            {
                var GuyLineTypeEntity = _unitOfWork.GuyLineTypeRepository.GetByID(Id);
                var GuyLineTypeViewModel = _mapper.Map<GuyLineTypeViewModel>(GuyLineTypeEntity);
                return new Response<GuyLineTypeViewModel>(true, GuyLineTypeViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<GuyLineTypeViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters
        //First parameterPagination
        //Second filters
        //Function return list of Guy Line Types
        public async Task<Response<IEnumerable<GuyLineTypeViewModel>>> GetGuyLineTypes(ParameterPagination parameterPagination, List<FilterObjectList> filters = null)
        {
            try
            {
                var GuyLineTypesEntities = await _unitOfWork.GuyLineTypeRepository.GetAllAsync(parameterPagination, filters);
                var GuyLineTypesTypesModel = _mapper.Map<IEnumerable<GuyLineTypeViewModel>>(GuyLineTypesEntities);
                return new Response<IEnumerable<GuyLineTypeViewModel>>(true, GuyLineTypesTypesModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<IEnumerable<GuyLineTypeViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter Name
        //Function check the name for add in database return true or false
        private bool CheckNameForAdd(string Name)
        {
            //  var GuyLineType = _unitOfWork.GuyLineTypeRepository.GetAllAsQueryable().Where(g => g.Name == Name).FirstOrDefault();
            var GuyLineType = _unitOfWork.GuyLineTypeRepository.GetWhereFirst(g => g.Name == Name);
            if (GuyLineType == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Function take 2 parameters Name, Id
        //Function check if there is record has different Id and same name 
        //Function return true or false
        private async Task <bool> CheckNameForUpdate(string Name, int Id)
        {
            var GuyLineType =await _unitOfWork.GuyLineTypeRepository.SingleOrDefaultAsync(x => x.Name == Name && x.Id != Id);
            if (GuyLineType == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
