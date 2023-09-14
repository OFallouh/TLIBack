using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class MW_PortService : IMW_PortService
    {
        private readonly IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public MW_PortService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        public async Task<Response<AddMW_PortViewModel>> Create(AddMW_PortViewModel Port)
        {
            try
            {
                var Entity = _mapper.Map<TLImwPort>(Port);
                await _unitOfWork.MW_PortRepository.AddAsync(Entity);
                await _unitOfWork.SaveChangesAsync();
                return new Response<AddMW_PortViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<AddMW_PortViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            
        }

        public async Task<Response<MW_PortViewModel>> GetPortById(int Id)
        {
            try
            {
                var Entity = await _unitOfWork.MW_PortRepository.GetAsync(Id);
                var ViewModel = _mapper.Map<MW_PortViewModel>(Entity);
                return new Response<MW_PortViewModel>(true, ViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<MW_PortViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public async Task<Response<IEnumerable<MW_PortViewModel>>> GetPorts()
        {
            try
            {
                var Ports = await _unitOfWork.MW_PortRepository.GetAllAsync();
                var result = _mapper.Map<IEnumerable<MW_PortViewModel>>(Ports);
                return new Response<IEnumerable<MW_PortViewModel>>(true, result, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<IEnumerable<MW_PortViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public async Task<Response<EditMW_PortViewModel>> Update(EditMW_PortViewModel Port)
        {
            try
            {
                var Entity = _mapper.Map<TLImwPort>(Port);
                await _unitOfWork.MW_PortRepository.UpdateItem(Entity);
                await _unitOfWork.SaveChangesAsync();
                return new Response<EditMW_PortViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<EditMW_PortViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
    }
}
