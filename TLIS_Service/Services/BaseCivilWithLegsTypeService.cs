using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
using TLIS_DAL.ViewModels.BaseCivilWithLegsTypeDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class BaseCivilWithLegsTypeService : IBaseCivilWithLegsTypeService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        ServiceProvider _serviceProvider;
        private IMapper _mapper;

        public BaseCivilWithLegsTypeService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        //First Map viewModel to Entity then add the Entity to database
        public async Task<Response<BaseCivilWithLegsTypeViewModel>> AddBaseCivilWithLegsType(AddBaseCivilWithLegsTypeViewModel addBaseCivil)
        {
            try
            {
                TLIbaseCivilWithLegsType civilWithLegsTypeEntity = _mapper.Map<TLIbaseCivilWithLegsType>(addBaseCivil);
                await _unitOfWork.BaseCivilWithLegsTypeRepository.AddAsync(civilWithLegsTypeEntity);
                await _unitOfWork.SaveChangesAsync();
                return new Response<BaseCivilWithLegsTypeViewModel>();
            }
            catch(Exception err)
            {
                return new Response<BaseCivilWithLegsTypeViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            
        }
        //First Map ViewModel to Entity then Update the Entity
        public async Task<Response<BaseCivilWithLegsTypeViewModel>> EditBaseCivilWithLegsType(EditBaseCivilWithLegsTypeViewModel editBaseCivil)
        {
            try
            {
                TLIbaseCivilWithLegsType civilWithLegsTypeEntity = _mapper.Map<TLIbaseCivilWithLegsType>(editBaseCivil);
                _unitOfWork.BaseCivilWithLegsTypeRepository.Update(civilWithLegsTypeEntity);
                await _unitOfWork.SaveChangesAsync();
                return new Response<BaseCivilWithLegsTypeViewModel>();
            }
            catch(Exception err)
            {
                return new Response<BaseCivilWithLegsTypeViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Get BaseCivilWithLegsType by Id
        public async Task<Response<BaseCivilWithLegsTypeViewModel>> GetBaseCivilWithLegsType(int Id)
        {
            try
            {
                var BaseCivilWithLegsType = await _unitOfWork.BaseCivilWithLegsTypeRepository.GetAsync(Id);
                return new Response<BaseCivilWithLegsTypeViewModel>(true, BaseCivilWithLegsType, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                return new Response<BaseCivilWithLegsTypeViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //First get BaseCivilWithLegsType Entities then map to BaseCivilWithLegsTypeViewModel lists
        public async Task<Response<IEnumerable<BaseCivilWithLegsTypeViewModel>>> GetBaseCivilWithLegsTypes(ParameterPagination parameterPagination, List<FilterObjectList> filter = null)
        {
            try
            {
                int count = 0;
                var BaseCivilWithLegsTypesEntities = await _unitOfWork.BaseCivilWithLegsTypeRepository.GetAllIncludeMultiple(parameterPagination, filter, out count, null).ToListAsync();
                var BaseCivilWithLegsTypesModel = _mapper.Map<List<BaseCivilWithLegsTypeViewModel>>(BaseCivilWithLegsTypesEntities);
                return new Response<IEnumerable<BaseCivilWithLegsTypeViewModel>>(true, BaseCivilWithLegsTypesModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                return new Response<IEnumerable<BaseCivilWithLegsTypeViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            
        }
    }
}
