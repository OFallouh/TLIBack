using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class MW_RFULibraryService : IMW_RFULibraryService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public MW_RFULibraryService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        public async Task<IEnumerable<TLImwRFULibrary>> GetMW_RFULibrary(ParameterPagination parameterPagination, MW_RFULibraryFilter MW_RFULibraryFilter)
        {
            return await _unitOfWork.MW_RFULibraryRepository.GetAllAsync();
        }
        public void AddMW_RFULibrary(MW_RFULibraryViewModel addMW_RFULibraryViewModel)
        {
            _unitOfWork.MW_RFULibraryRepository.AddAsync(addMW_RFULibraryViewModel);
            _unitOfWork.SaveChanges();
        }
        public void EditMW_RFULibrary(MW_RFULibraryViewModel EditMW_RFULibraryViewModel)
        {
            _unitOfWork.MW_RFULibraryRepository.UpdateItem(EditMW_RFULibraryViewModel);
            _unitOfWork.SaveChanges();
        }
    }
}
