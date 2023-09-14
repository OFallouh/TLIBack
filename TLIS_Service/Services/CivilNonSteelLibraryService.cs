using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class CivilNonSteelLibraryService : ICivilNonSteelLibraryService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public CivilNonSteelLibraryService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        public async Task<IEnumerable<TLIcivilNonSteelLibrary>> GetCivilNonSteelLibrary(ParameterPagination parameterPagination, CivilNonSteelLibraryFilter civilNonSteelLibraryFilter)
        {
            return await _unitOfWork.CivilNonSteelLibraryRepository.GetAllAsync();
        }

        public async Task<CivilNonSteelLibraryViewModel> GetById(int Id)
        {
            return await _unitOfWork.CivilNonSteelLibraryRepository.GetAsync(Id);

        }

        public async Task AddCivilNonSteelLibrary(AddCivilNonSteelLibraryViewModel addCivilNonSteelLibraryViewModel)
        {
            TLIcivilNonSteelLibrary CivilNonSteelEntites = _mapper.Map<TLIcivilNonSteelLibrary>(addCivilNonSteelLibraryViewModel);

            await _unitOfWork.CivilNonSteelLibraryRepository.AddAsync(CivilNonSteelEntites);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task EditCivilNonSteelLibrary(EditCivilNonSteelLibraryViewModel editCivilNonSteelLibraryViewModel)
        {
            TLIcivilNonSteelLibrary CivilNonSteelEntites = _mapper.Map<TLIcivilNonSteelLibrary>(editCivilNonSteelLibraryViewModel);

            _unitOfWork.CivilNonSteelLibraryRepository.Update(CivilNonSteelEntites);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Disable_EnableCivilNonSteelLibrary(int id, bool active, CivilNonSteelLibraryViewModel civilNonSteelLibraryViewModel)
        {
           
            TLIcivilNonSteelLibrary CivilNonSteelEntites =  _unitOfWork.CivilNonSteelLibraryRepository.GetByID(id);

            CivilNonSteelEntites.Active = active;

            await _unitOfWork.CivilNonSteelLibraryRepository.Disable_Enable(id, active, CivilNonSteelEntites);

            await _unitOfWork.SaveChangesAsync();
        }

    }
}
