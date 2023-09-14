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
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class CivilWithoutLegLibraryService : ICivilWithoutLegLibraryService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public CivilWithoutLegLibraryService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        public async Task<Response<ReturnWithFilters<CivilWithoutLegLibraryViewModel>>> GetCivilWithoutLegLibrary(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                IEnumerable<TLIcivilWithoutLegLibrary> CivilWithoutLegLibrariesList;
                if (filters.Count != 0)
                {
                   
                    CivilWithoutLegLibrariesList = await _unitOfWork.CivilWithoutLegLibraryRepository.GetAllAsync(parameters, filters);
                }
                else
                {
                    CivilWithoutLegLibrariesList = await _unitOfWork.CivilWithoutLegLibraryRepository.GetAllAsync(parameters);
                }
                var FilteredCivilWithoutLegLibrariesModel = _mapper.Map<IEnumerable<CivilWithoutLegLibraryViewModel>>(CivilWithoutLegLibrariesList);
                ReturnWithFilters<CivilWithoutLegLibraryViewModel> CivilWithoutLeg = new ReturnWithFilters<CivilWithoutLegLibraryViewModel>();
                CivilWithoutLeg.Model = FilteredCivilWithoutLegLibrariesModel.ToList();
                CivilWithoutLeg.filters = _unitOfWork.CivilWithoutLegLibraryRepository.GetRelatedTables();
                return new Response<ReturnWithFilters<CivilWithoutLegLibraryViewModel>>(true, CivilWithoutLeg, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<ReturnWithFilters<CivilWithoutLegLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            
        }

        public async Task<CivilWithoutLegLibraryViewModel> GetById(int Id)
        {
            return await _unitOfWork.CivilWithoutLegLibraryRepository.GetAsync(Id);

        }


        public async Task AddCivilWithoutLegLibrary(AddCivilWithoutLegLibraryViewModel addCivilWithoutLegLibraryViewModel)
        {
            TLIcivilWithoutLegLibrary CivilWithOutLegEntites = _mapper.Map<TLIcivilWithoutLegLibrary>(addCivilWithoutLegLibraryViewModel);

            await _unitOfWork.CivilWithoutLegLibraryRepository.AddAsync(CivilWithOutLegEntites);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task EditCivilWithoutLegLibrary(EditCivilWithoutLegLibraryViewModel editCivilWithoutLegLibraryViewModel)
        {
            TLIcivilWithoutLegLibrary CivilWithOutLegEntites = _mapper.Map<TLIcivilWithoutLegLibrary>(editCivilWithoutLegLibraryViewModel);

            _unitOfWork.CivilWithoutLegLibraryRepository.Update(CivilWithOutLegEntites);
            await _unitOfWork.SaveChangesAsync();
        }


    }
}
