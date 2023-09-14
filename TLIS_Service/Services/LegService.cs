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
using TLIS_DAL.ViewModels.LegDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class LegService : ILegService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public LegService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        public async Task<IEnumerable<TLIleg>> GetCivilNonSteelLibrary(ParameterPagination parameterPagination, LegFilter LegFilter)
        {
            return await _unitOfWork.LegRepository.GetAllAsync();
        }

        public async Task<LegViewModel> GetById(int Id)
        {
            return await _unitOfWork.LegRepository.GetAsync(Id);

        }
        public async Task AddLeg(AddLegViewModel addLegViewModel)
        {
            TLIleg LegEntites = _mapper.Map<TLIleg>(addLegViewModel);

            await _unitOfWork.LegRepository.AddAsync(LegEntites);
            await _unitOfWork.SaveChangesAsync();

        }
        public async Task EditLeg(EditLegViewModel editLegViewModel)
        {
            TLIleg LegEntites = _mapper.Map<TLIleg>(editLegViewModel);

            await _unitOfWork.LegRepository.UpdateItem(LegEntites);
            await _unitOfWork.SaveChangesAsync();
        }

        public Response<List<LegViewModel>> GetLegsByCivilId(int CivilId)
        {
            try
            {
                List<LegViewModel> Legs = _mapper.Map<List<LegViewModel>>(_unitOfWork.LegRepository.GetWhere(x => x.CivilWithLegInstId == CivilId).ToList());
                return new Response<List<LegViewModel>>(true, Legs, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<LegViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<LegViewModel>> GetLegsByAllCivilInstId(int AllCivilInstId)
        {
            try
            {

                int CivilWithLegInstallationId = _unitOfWork.AllCivilInstRepository.GetIncludeWhereFirst(x => x.Id == AllCivilInstId && x.civilWithLegsId != null).civilWithLegsId.Value;

                List<LegViewModel> LegsViewModel = _mapper.Map<List<LegViewModel>>(_unitOfWork.LegRepository.GetWhere(x => x.CivilWithLegInstId == CivilWithLegInstallationId));

                return new Response<List<LegViewModel>>(true, LegsViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<LegViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
    }
}
