using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class LogUsersActionsService: ILogUsersActionsService
    {
        private readonly IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        public LogUsersActionsService(IUnitOfWork unitOfWork, IServiceCollection services)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
        }
    }
}
