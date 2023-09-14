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
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class OwnerService : IOwnerService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public OwnerService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        public async Task<Response<OwnerViewModel>> AddOwner(AddOwnerViewModel addOwner)
        {
            try
            {
                if(CheckNameForAdd(addOwner.OwnerName))
                {
                    var OwnerEntity = _mapper.Map<TLIowner>(addOwner);
                    await _unitOfWork.OwnerRepository.AddAsync(OwnerEntity);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<OwnerViewModel>();
                }
                else
                {
                    return new Response<OwnerViewModel>(true, null, null, $"This Owner {addOwner.OwnerName} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch(Exception err)
            {
                
                return new Response<OwnerViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public async Task<Response<OwnerViewModel>> EditOwner(EditOwnerViewModel editOwner)
        {
            try
            {
                if(CheckNameForUpdate(editOwner.OwnerName,editOwner.Id))
                {
                    var OwnerEntity = _mapper.Map<TLIowner>(editOwner);
                    await _unitOfWork.OwnerRepository.UpdateItem(OwnerEntity);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<OwnerViewModel>();
                }
                else
                {
                    return new Response<OwnerViewModel>(true, null, null, $"this Owner {editOwner.OwnerName} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch(Exception err)
            {
                
                return new Response<OwnerViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<OwnerViewModel> GetOwner(int Id)
        {
            try
            {
                var Owner = _unitOfWork.OwnerRepository.GetByID(Id);
                var OwnerViewModel = _mapper.Map<OwnerViewModel>(Owner);
                return new Response<OwnerViewModel>(true, OwnerViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<OwnerViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public async Task<Response<IEnumerable<OwnerViewModel>>> GetOwners(ParameterPagination parameterPagination, List<FilterObjectList> filters = null)
        {
            try
            {
                List<TLIowner> Owners = new List<TLIowner>();
                var OwnersEntities = await _unitOfWork.OwnerRepository.GetAllAsync(parameterPagination, filters);
                Owners = OwnersEntities.ToList();       
                var OwnersViewModel = _mapper.Map<IEnumerable<OwnerViewModel>>(Owners);
                return new Response<IEnumerable<OwnerViewModel>>(true, OwnersViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<IEnumerable<OwnerViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        private bool CheckNameForAdd(string Name)
        {
            var Owner = _unitOfWork.OwnerRepository.GetWhereFirst(o => o.OwnerName == Name);
            if(Owner == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckNameForUpdate(string Name, int Id)
        {
            var Owner = _unitOfWork.OwnerRepository.Any(x => x.OwnerName == Name && x.Id != Id);
            return !Owner;
        }
    }
}
