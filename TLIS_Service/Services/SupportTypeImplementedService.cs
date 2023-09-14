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
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.SupportTypeImplementedDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
   public class SupportTypeImplementedService : ISupportTypeImplementedService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public SupportTypeImplementedService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        public async Task<Response<IEnumerable<SupportTypeImplementedViewModel>>> GetSupportTypesImplemented(ParameterPagination parameterPagination, List<FilterObjectList> filters = null)
        {
            try
            {
                int count = 0;
                var SupportTypes = await _unitOfWork.SupportTypeImplementedRepository.GetAllIncludeMultiple(parameterPagination, filters, out count, null).AsNoTracking().ToListAsync();
                var SupportTypesModel = _mapper.Map<IEnumerable<SupportTypeImplementedViewModel>>(SupportTypes);
                return new Response<IEnumerable<SupportTypeImplementedViewModel>>(true, SupportTypesModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch(Exception err)
            {
                
                return new Response<IEnumerable<SupportTypeImplementedViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            
        }

        public async Task<Response<SupportTypeImplementedViewModel>> GetById(int Id)
        {
            try
            {
                var SupportType = await _unitOfWork.SupportTypeImplementedRepository.GetAsync(Id);
                return new Response<SupportTypeImplementedViewModel>(true, SupportType, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<SupportTypeImplementedViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            

        }
        public async Task<Response<SupportTypeImplementedViewModel>> AddSupportTypeImplemented(AddSupportTypeImplementedViewModel addSupportTypeImplementedViewModel)
        {
            try
            {
                if(CheckNameForAdd(addSupportTypeImplementedViewModel.Name))
                {
                    TLIsupportTypeImplemented supportTypeImplementedEntity = _mapper.Map<TLIsupportTypeImplemented>(addSupportTypeImplementedViewModel);
                    await _unitOfWork.SupportTypeImplementedRepository.AddAsync(supportTypeImplementedEntity);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<SupportTypeImplementedViewModel>();
                }
                else
                {
                    return new Response<SupportTypeImplementedViewModel>(true, null, null, $"This supportTypeImplementedEntity {addSupportTypeImplementedViewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch(Exception err)
            {
                
                return new Response<SupportTypeImplementedViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            

        }

        public async Task<Response<SupportTypeImplementedViewModel>> EditSupportTypeImplemented(EditSupportTypeImplementedViewModel editSupportTypeImplementedViewModel)
        {
            
            try
            {

                if (await CheckNameForUpdate(editSupportTypeImplementedViewModel.Name, editSupportTypeImplementedViewModel.Id))
                {
                    TLIsupportTypeImplemented supportTypeImplementedEntity = _mapper.Map<TLIsupportTypeImplemented>(editSupportTypeImplementedViewModel);
                    _unitOfWork.SupportTypeImplementedRepository.Update(supportTypeImplementedEntity);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<SupportTypeImplementedViewModel>();
                }
                else
                {
                    return new Response<SupportTypeImplementedViewModel>(true, null, null, $"this supportTypeImplementedEntity {editSupportTypeImplementedViewModel.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch(Exception err)
            {
                
                return new Response<SupportTypeImplementedViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        private bool CheckNameForAdd(string Name)
        {
            var SupportTypeImplemented = _unitOfWork.SupportTypeImplementedRepository.GetAllAsQueryable().Where(s => s.Name == Name).FirstOrDefault();
            if (SupportTypeImplemented == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task <bool> CheckNameForUpdate(string Name, int Id)
        {
            var SupportTypeImplemented =await _unitOfWork.SupportTypeImplementedRepository.SingleOrDefaultAsync(x => x.Name == Name && x.Id != Id);
            if (SupportTypeImplemented == null)
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
