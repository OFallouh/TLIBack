using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CityDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class CityService: ICityService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public CityService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }

        public async Task AddCity(AddCityViewModel addCityViewModel)
        {
            //using (TransactionScope transaction = new TransactionScope())
            //{
            //    try
            //    {
            //        if(CheckNameForAdd(addCityViewModel.Name))
            //        {
            //            TLIcity cityEntites = _mapper.Map<TLIcity>(addCityViewModel);
            //            await _unitOfWork.CityRepository.AddAsync(cityEntites);
            //            await _unitOfWork.SaveChangesAsync();
            //            transaction.Complete();
            //        }
            //    }
            //    catch(Exception)
            //    {

            //    }
            //}            
        }

        public void GetFilter(List<FilterObjectList> filter)
        {
            
           // var testWhere = _unitOfWork.CivilWithLegLibraryRepository.WhereFiltersAllTypeList(filter).ToList();
            //
        }


        public async Task<IEnumerable<TLIcity>> GetCitiess(ParameterPagination parameterPagination)
        {
            return await _unitOfWork.CityRepository.GetAllAsync(parameterPagination);    
        }
        public async Task< IEnumerable<TLIcity>> GetCitiess(ParameterPagination parameterPagination,List<FilterObjectList> filters)
        {

             return await _unitOfWork.CityRepository.GetAllAsync(parameterPagination, filters);


        }

        public IEnumerable<TLIcity> GetCitiess2()
        {
            var cityList = _unitOfWork.CityRepository.GetAllWithoutCount();
            return cityList;
        }

        public async Task<IEnumerable<TLIcity>> GetCitiesRep(ParameterPagination parameterPagination)
        {
            return await _unitOfWork.CityRepository.GetAllAsync();
            //return _unitOfWork.ClassRepository.GetAll();
        }

        public IEnumerable<TLIcity> whereCity(List<FilterObjectList> filter)
        {
          //  var l = _unitOfWork.CityRepository.SelectExpression("Name");
            //List<FilterExperssion> filters = new List<FilterExperssion>();
          //  var x = new List<string>();
         //   x.Add("Damas");
          //  filters.Add(new FilterExperssion("Name", "==", x));
           
            GetFilter(filter);
            var s = _unitOfWork.CityRepository.GetAllWithoutCount();
            return s;
        }
        public bool CheckNameForAdd(string Name)
        {
            var city = _unitOfWork.CityRepository.GetWhereFirst(c => c.Name == Name);
            if(city == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckNameForUpdate(string Name, int Id)
        {
            var city = _unitOfWork.CityRepository.SingleOrDefaultAsync(x => x.Name == Name && x.Id != Id);
            if(city == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
