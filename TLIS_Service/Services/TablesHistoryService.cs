using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class TablesHistoryService : ITablesHistoryService
    {
        //public Response<TablesHistoryViewModel> AddTableHistory(AddTablesHistoryViewModel addTablesHistory)
        //{
        //    throw new NotImplementedException();
        //}
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public TablesHistoryService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        public Response<TablesHistoryViewModel> GetTablesHistory()
        {
            throw new NotImplementedException();
        }
        public Response<TablesHistoryViewModel> AddTableHistory(AddTablesHistoryViewModel addTablesHistory, int UserId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    //int UserId = http.Headers.UserAgent.
                    var TablesHistory = _mapper.Map<TLItablesHistory>(addTablesHistory);
                    //var test = _httpContextAccessor.HttpContext.Request.Headers["cookie"].ToString();
                    //TablesHistory.UserId = Int32.Parse(test);
                    //var message = _session.GetString("Test");
                    _unitOfWork.TablesHistoryRepository.Add(TablesHistory);
                    _unitOfWork.SaveChanges();
                    return new Response<TablesHistoryViewModel>();
                }
                catch (Exception err)
                {
                    
                    return new Response<TablesHistoryViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
    }
}
