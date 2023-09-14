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
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class MW_BULibraryService : IMW_BULibraryService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public MW_BULibraryService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        public async Task MW_BULibrarySeedDataForTest()
        {
            try
            {
                List<TLIdiversityType> SeedDataForDiversityType = new List<TLIdiversityType>
                {
                    new TLIdiversityType
                    {
                        Id = 1,
                        Name = "DiversityType1",
                        Deleted = false,
                        Disable = false
                    },
                    new TLIdiversityType
                    {
                        Id = 2,
                        Name = "DiversityType2",
                        Deleted = false,
                        Disable = false
                    },
                    new TLIdiversityType
                    {
                        Id = 3,
                        Name = "DiversityType3",
                        Deleted = false,
                        Disable = false
                    }
                };
                await _unitOfWork.DiversityTypeRepository.AddRangeAsync(SeedDataForDiversityType);
                await _unitOfWork.SaveChangesAsync();

                List<TLImwBULibrary> SeedData = new List<TLImwBULibrary>
                {
                    new TLImwBULibrary
                    {
                        Id = 1,
                        Model = "MW_BULibrary1",
                        Type = "1",
                        Note = "1",
                        L_W_H = "1",
                        Length = 1,
                        Width = 1,
                        Height = 1,
                        Weight = 1,
                        BUSize = "1",
                        NumOfRFU = 1,
                        frequency_band = "1",
                        channel_bandwidth = 1,
                        FreqChannel = "1",
                        SpaceLibrary = 1,
                        Active = true,
                        Deleted = false,
                        diversityTypeId = 1
                    },
                    new TLImwBULibrary
                    {
                        Id = 2,
                        Model = "MW_BULibrary2",
                        Type = "2",
                        Note = "2",
                        L_W_H = "2",
                        Length = 2,
                        Width = 2,
                        Height = 2,
                        Weight = 2,
                        BUSize = "2",
                        NumOfRFU = 2,
                        frequency_band = "2",
                        channel_bandwidth = 2,
                        FreqChannel = "2",
                        SpaceLibrary = 2,
                        Active = true,
                        Deleted = false,
                        diversityTypeId = 1
                    },
                    new TLImwBULibrary
                    {
                        Id = 3,
                        Model = "MW_BULibrary3",
                        Type = "3",
                        Note = "3",
                        L_W_H = "3",
                        Length = 3,
                        Width = 3,
                        Height = 3,
                        Weight = 3,
                        BUSize = "3",
                        NumOfRFU = 3,
                        frequency_band = "3",
                        channel_bandwidth = 3,
                        FreqChannel = "3",
                        SpaceLibrary = 3,
                        Active = true,
                        Deleted = false,
                        diversityTypeId = 2
                    },
                    new TLImwBULibrary
                    {
                        Id = 4,
                        Model = "MW_BULibrary4",
                        Type = "4",
                        Note = "4",
                        L_W_H = "4",
                        Length = 4,
                        Width = 4,
                        Height = 4,
                        Weight = 4,
                        BUSize = "4",
                        NumOfRFU = 4,
                        frequency_band = "4",
                        channel_bandwidth = 4,
                        FreqChannel = "4",
                        SpaceLibrary = 4,
                        Active = true,
                        Deleted = false,
                        diversityTypeId = 2
                    },
                    new TLImwBULibrary
                    {
                        Id = 5,
                        Model = "MW_BULibrary5",
                        Type = "5",
                        Note = "5",
                        L_W_H = "5",
                        Length = 5,
                        Width = 5,
                        Height = 5,
                        Weight = 5,
                        BUSize = "5",
                        NumOfRFU = 5,
                        frequency_band = "5",
                        channel_bandwidth = 5,
                        FreqChannel = "5",
                        SpaceLibrary = 5,
                        Active = true,
                        Deleted = false,
                        diversityTypeId = 3
                    }
                };
                await _unitOfWork.MW_BULibraryRepository.AddRangeAsync(SeedData);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<TLImwBULibrary>> GetMW_BULibrary(ParameterPagination parameterPagination, MW_BULibraryFilter MW_BULibraryFilter)
        {
            return await _unitOfWork.MW_BULibraryRepository.GetAllAsync();
        }

        public async Task<MW_BULibraryViewModel> GetById(int Id)
        {
            return await _unitOfWork.MW_BULibraryRepository.GetAsync(Id);

        }

        public async Task AddMW_BULibrary(AddMW_BULibraryViewModel addMW_BULibraryViewModel)
        {
            TLImwBULibrary MW_BULibraryEntites = _mapper.Map<TLImwBULibrary>(addMW_BULibraryViewModel);

            await _unitOfWork.MW_BULibraryRepository.AddAsync(MW_BULibraryEntites);
           await _unitOfWork.SaveChangesAsync();
        }

        public async Task EditMW_BULibrary(EditMW_BULibraryViewModel EditMW_BULibraryViewModel)
        {
            TLImwBULibrary MW_BULibraryEntites = _mapper.Map<TLImwBULibrary>(EditMW_BULibraryViewModel);

            await _unitOfWork.MW_BULibraryRepository.UpdateItem(MW_BULibraryEntites);
            await _unitOfWork.SaveChangesAsync();
        }

        
    }
}
