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
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class CivilWithLegsService : ICivilWithLegsService
    {
        private readonly IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        public CivilWithLegsService(IUnitOfWork unitOfWork, IServiceCollection services)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
        }
        public Task<IEnumerable<TLIcivilWithLegs>> GetCivilWithLegs(ParameterPagination parameterPagination, CivilWithLegsFilter civilWithLegsFilter)
        {
            throw new NotImplementedException();
        }
        
    }
}
