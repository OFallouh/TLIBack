using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class MW_ODULibraryService : IMW_ODULibraryService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        public MW_ODULibraryService(IUnitOfWork unitOfWork, IServiceCollection services)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
        }
        public async Task<IEnumerable<TLImwODULibrary>> GetMW_ODULibrary(ParameterPagination parameterPagination, MW_ODULibraryFilter MW_ODULibraryFilter)
        {
            return await _unitOfWork.MW_ODULibraryRepository.GetAllAsync();
        }

        public void AddMW_ODULibrary(MW_ODULibraryViewModel addMW_ODULibraryViewModel)
        {
            _unitOfWork.MW_ODULibraryRepository.AddAsync(addMW_ODULibraryViewModel);
            _unitOfWork.SaveChanges();
        }
        public void EditMW_ODULibrary(MW_ODULibraryViewModel EditMW_ODULibraryViewModel)
        {
            _unitOfWork.MW_ODULibraryRepository.UpdateItem(EditMW_ODULibraryViewModel);
            _unitOfWork.SaveChanges();
        }

        
    }
}
