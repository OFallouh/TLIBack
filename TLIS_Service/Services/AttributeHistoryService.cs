using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.StaticAttributesHistory;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class AttributeHistory : IAttributeHistoryService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private readonly ILogger<AttributeHistory> _logger;
        private IMapper _mapper;
        public AttributeHistory(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _logger = serviceProvider.GetService<ILogger<AttributeHistory>>();
            _mapper = mapper;
        }
        public Response<ReturnWithFilters<StaticAttsHistoryViewModel>> GetStaticAttributesHistoryByTableName(List<FilterObjectList> ObjectAttributeFilters, string TableName, ParameterPagination parameters)
        {
            try
            {
                ReturnWithFilters<StaticAttsHistoryViewModel> OutPut = new ReturnWithFilters<StaticAttsHistoryViewModel>();
                List<StaticAttsHistoryViewModel> StaticAttsHistoryViewModelList = new List<StaticAttsHistoryViewModel>();
                var list = _unitOfWork.TablesHistoryRepository.GetStaticAttributesHistory(TableName, parameters);

                OutPut.Model = list;
                OutPut.filters = null;
                return new Response<ReturnWithFilters<StaticAttsHistoryViewModel>>(true, OutPut, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return new Response<ReturnWithFilters<StaticAttsHistoryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<StaticAttsHistoryViewModel>> GetDynamicAttributesHistoryByTableName(List<FilterObjectList> ObjectAttributeFilters, string TableName, ParameterPagination parameters)
        {
            try
            {
                ReturnWithFilters<StaticAttsHistoryViewModel> OutPut = new ReturnWithFilters<StaticAttsHistoryViewModel>();
                var list = _unitOfWork.TablesHistoryRepository.GetDynamicAttributesHistory(TableName, parameters);

                OutPut.Model = list;
                return new Response<ReturnWithFilters<StaticAttsHistoryViewModel>>(true, OutPut, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<StaticAttsHistoryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<HistoryViewModel>> GetAttachedFileHistory(string TableName, int RecordId, ParameterPagination parameters)
        {
            try
            {
                var list = _unitOfWork.TablesHistoryRepository.GetAttachedFileHistory(TableName, RecordId, parameters);

                return new Response<List<HistoryViewModel>>(true, list, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<HistoryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
    }
}