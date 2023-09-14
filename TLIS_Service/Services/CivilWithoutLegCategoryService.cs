using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithoutLegCategoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class CivilWithoutLegCategoryService : ICivilWithoutLegCategoryService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public CivilWithoutLegCategoryService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        //Function take 1 parameter
        //First Check if Civil WithoutLeg Category is already exists in database return true or false
        //if true return error message that the category is already exists in database
        //else add category to database
        public async Task<Response<CivilWithoutLegCategoryViewModel>> Add(AddCivilWithoutLegCategoryViewModel categoryViewModel)
        {
            try
            {
                if(CheckNameForAdd(categoryViewModel.Name))
                {
                    var ResultEntity = _mapper.Map<TLIcivilWithoutLegCategory>(categoryViewModel);
                    _unitOfWork.CivilWithoutLegCategoryRepository.Add(ResultEntity);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<CivilWithoutLegCategoryViewModel>();
                }
                else
                {
                    return new Response<CivilWithoutLegCategoryViewModel>(true, null, null, $"This CivilWithoutLegCategory {categoryViewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch(Exception err)
            {
                
                return new Response<CivilWithoutLegCategoryViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        //Function take 2 parameters 
        //First Id to specify the record i deal with
        //Second status to specify if enable or disable
        public async  Task<Response<CivilWithoutLegCategoryViewModel>>  Disable_EnableCivilWithoutLegCategory (int Id, bool status)
        {
            try
            {
                TLIcivilWithoutLegCategory civilWithoutLegCategory = _unitOfWork.CivilWithoutLegCategoryRepository.GetByID(Id);
                civilWithoutLegCategory.disable = status;
                _unitOfWork.CivilWithoutLegCategoryRepository.Update( civilWithoutLegCategory);
                await _unitOfWork.SaveChangesAsync();
                return new Response<CivilWithoutLegCategoryViewModel>(true,null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
              
            }
            catch(Exception err)
            {
                
                return new Response<CivilWithoutLegCategoryViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter CivilWithoutLegCategoryViewModel have the data to update the record
        //First Check that there is no other record have the same name return true or false
        //Second if true update the record
        //else return error message that the name is already exists
        public async Task<Response<CivilWithoutLegCategoryViewModel>> Edit(CivilWithoutLegCategoryViewModel CivilWithoutLegCategoryViewModel)
        {
            try
            {
                var test = await CheckNameForUpdate(CivilWithoutLegCategoryViewModel.Name, CivilWithoutLegCategoryViewModel.Id);
                if (test == true)
                {
                    var CategoryEntity = _mapper.Map<TLIcivilWithoutLegCategory>(CivilWithoutLegCategoryViewModel);
                    _unitOfWork.CivilWithoutLegCategoryRepository.Update(CategoryEntity);
                    await _unitOfWork.SaveChangesAsync();
                     return new Response<CivilWithoutLegCategoryViewModel>();
                }
                else
                {
                    return new Response<CivilWithoutLegCategoryViewModel>(true, null, null, $"this CivilWithoutLegCategory {CivilWithoutLegCategoryViewModel.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch(Exception err)
            {
                
                return new Response<CivilWithoutLegCategoryViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function get record by Id
        public async Task<Response<CivilWithoutLegCategoryViewModel>> GetCivilWithoutLegCategory(int Id)
        {
            try
            {
                var CivilWithoutLegCategory = await _unitOfWork.CivilWithoutLegCategoryRepository.GetAsync(Id);
                //var CivilWithoutLegCategoryViewModel = _mapper.Map<CivilWithoutLegCategoryViewModel>(CivilWithoutLegCategory);
                return new Response<CivilWithoutLegCategoryViewModel>(true, CivilWithoutLegCategory, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<CivilWithoutLegCategoryViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        // Function get record by Name
        public Response<CivilWithoutLegCategoryViewModel> GetCivilWithoutLegCategoryByName(string Name)
        {
            try
            {
                CivilWithoutLegCategoryViewModel CivilWithoutLegCategory = _mapper.Map<CivilWithoutLegCategoryViewModel>(_unitOfWork.CivilWithoutLegCategoryRepository.GetWhereFirst(x => 
                    x.Name.ToLower() == Name.ToLower()));

                return new Response<CivilWithoutLegCategoryViewModel>(true, CivilWithoutLegCategory, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<CivilWithoutLegCategoryViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function get civil without leg category list
        public async Task<Response<IEnumerable<CivilWithoutLegCategoryViewModel>>> GetList()
        {
            try
            {
                var result = await _unitOfWork.CivilWithoutLegCategoryRepository.GetAllAsync();
                var resultvm = _mapper.Map<IEnumerable<CivilWithoutLegCategoryViewModel>>(result);
                return new Response<IEnumerable<CivilWithoutLegCategoryViewModel>>(true, resultvm, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<IEnumerable<CivilWithoutLegCategoryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter name
        //Check the name is already exists in database return true or false
        private bool CheckNameForAdd(string Name)
        {
            var CivilWithoutLegCategory = _unitOfWork.CivilWithoutLegCategoryRepository.GetWhereFirst(c => c.Name == Name);
            if (CivilWithoutLegCategory == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Function take 2 parameter name and Id
        //Check if there is record have the same name and difference Id return true or false
        private async Task<bool> CheckNameForUpdate(string Name, int Id)
        {
            var CivilWithoutLegCategory = await _unitOfWork.CivilWithoutLegCategoryRepository.SingleOrDefaultAsync(x => x.Name == Name && x.Id != Id);
            if (CivilWithoutLegCategory == null)
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
