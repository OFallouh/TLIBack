using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL.Helper;
using System.Collections;
using System.Globalization;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using AutoMapper;

namespace TLIS_Service.Services
{
    public class MWLibraryService : IMWLibraryService
    {
        private readonly IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public MWLibraryService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        public async Task MW_OtherLibrarySeedDataForTest()
        {
            try
            {
                List<TLImwOtherLibrary> SeedData = new List<TLImwOtherLibrary>
                {
                    new TLImwOtherLibrary
                    {
                        Id = 1,
                        Model = "MW_OtherLibrary1",
                        Note = "1",
                        Length = 1,
                        Width = 1,
                        Height = 1,
                        L_W_H = "1",
                        frequency_band = "1",
                        SpaceLibrary = 1,
                        Active = true,
                        Deleted = false
                    },
                    new TLImwOtherLibrary
                    {
                        Id = 2,
                        Model = "MW_OtherLibrary2",
                        Note = "2",
                        Length = 2,
                        Width = 2,
                        Height = 2,
                        L_W_H = "2",
                        frequency_band = "2",
                        SpaceLibrary = 2,
                        Active = true,
                        Deleted = false
                    },
                    new TLImwOtherLibrary
                    {
                        Id = 3,
                        Model = "MW_OtherLibrary3",
                        Note = "3",
                        Length = 3,
                        Width = 3,
                        Height = 3,
                        L_W_H = "3",
                        frequency_band = "3",
                        SpaceLibrary = 3,
                        Active = true,
                        Deleted = false
                    },
                    new TLImwOtherLibrary
                    {
                        Id = 4,
                        Model = "MW_OtherLibrary4",
                        Note = "4",
                        Length = 4,
                        Width = 4,
                        Height = 4,
                        L_W_H = "4",
                        frequency_band = "4",
                        SpaceLibrary = 4,
                        Active = true,
                        Deleted = false
                    },
                    new TLImwOtherLibrary
                    {
                        Id = 5,
                        Model = "MW_OtherLibrary5",
                        Note = "5",
                        Length = 5,
                        Width = 5,
                        Height = 5,
                        L_W_H = "5",
                        frequency_band = "5",
                        SpaceLibrary = 5,
                        Active = true,
                        Deleted = false
                    }
                };
                await _unitOfWork.MW_OtherLibraryRepository.AddRangeAsync(SeedData);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task MW_ODULibrarySeedDataForTest()
        {
            try
            {
                List<TLIparity> ParitySeedData = new List<TLIparity>
                {
                    new TLIparity
                    {
                        Id = 1,
                        Name = "Parity1"
                    },
                    new TLIparity
                    {
                        Id = 2,
                        Name = "Parity2"
                    },
                    new TLIparity
                    {
                        Id = 3,
                        Name = "Parity3"
                    }
                };
                await _unitOfWork.ParityRepository.AddRangeAsync(ParitySeedData);
                await _unitOfWork.SaveChangesAsync();

                List<TLImwODULibrary> SeedData = new List<TLImwODULibrary>
                {
                    new TLImwODULibrary
                    {
                        Id = 1,
                        Model = "MW_ODULibraray1",
                        Note = "1",
                        Weight = 1,
                        H_W_D = "1",
                        Depth = 1,
                        Width = 1,
                        Height = 1,
                        frequency_range = "1",
                        frequency_band = "1",
                        SpaceLibrary = 1,
                        Active = true,
                        Deleted = false,
                        parityId = 1
                    },
                    new TLImwODULibrary
                    {
                        Id = 2,
                        Model = "MW_ODULibraray2",
                        Note = "2",
                        Weight = 2,
                        H_W_D = "2",
                        Depth = 2,
                        Width = 2,
                        Height = 2,
                        frequency_range = "2",
                        frequency_band = "2",
                        SpaceLibrary = 2,
                        Active = true,
                        Deleted = false,
                        parityId = 1
                    },
                    new TLImwODULibrary
                    {
                        Id = 3,
                        Model = "MW_ODULibraray3",
                        Note = "3",
                        Weight = 3,
                        H_W_D = "3",
                        Depth = 3,
                        Width = 3,
                        Height = 3,
                        frequency_range = "3",
                        frequency_band = "3",
                        SpaceLibrary = 3,
                        Active = true,
                        Deleted = false,
                        parityId = 2
                    },
                    new TLImwODULibrary
                    {
                        Id = 4,
                        Model = "MW_ODULibraray4",
                        Note = "4",
                        Weight = 4,
                        H_W_D = "4",
                        Depth = 4,
                        Width = 4,
                        Height = 4,
                        frequency_range = "4",
                        frequency_band = "4",
                        SpaceLibrary = 4,
                        Active = true,
                        Deleted = false,
                        parityId = 2
                    },
                    new TLImwODULibrary
                    {
                        Id = 5,
                        Model = "MW_ODULibraray5",
                        Note = "5",
                        Weight = 5,
                        H_W_D = "5",
                        Depth = 5,
                        Width = 5,
                        Height = 5,
                        frequency_range = "5",
                        frequency_band = "5",
                        SpaceLibrary = 5,
                        Active = true,
                        Deleted = false,
                        parityId = 3
                    }
                };
                await _unitOfWork.MW_ODULibraryRepository.AddRangeAsync(SeedData);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task MW_DishLibrarySeedDataForTest()
        {
            try
            {
                List<TLIpolarityType> PolarityTypeSeedData = new List<TLIpolarityType>
                {
                    new TLIpolarityType
                    {
                        Id = 1,
                        Name = "PolarityType1"
                    },
                    new TLIpolarityType
                    {
                        Id = 2,
                        Name = "PolarityType2"
                    },
                    new TLIpolarityType
                    {
                        Id = 3,
                        Name = "PolarityType3"
                    }
                };
                await _unitOfWork.PolarityTypeRepository.AddRangeAsync(PolarityTypeSeedData);
                await _unitOfWork.SaveChangesAsync();

                List<TLImwDishLibrary> SeedData = new List<TLImwDishLibrary>
                {
                    new TLImwDishLibrary
                    {
                        Id = 1,
                        Model = "MW_DishLibraray1",
                        Description = "1",
                        Note = "1",
                        Weight = 1,
                        dimensions = "1",
                        Length = 1,
                        Width = 1,
                        Height = 1,
                        diameter = 1,
                        frequency_band = "1",
                        SpaceLibrary = 1,
                        Active = true,
                        Deleted = false,
                        polarityTypeId = 1
                    },
                    new TLImwDishLibrary
                    {
                        Id = 2,
                        Model = "MW_DishLibraray2",
                        Description = "2",
                        Note = "2",
                        Weight = 2,
                        dimensions = "2",
                        Length = 2,
                        Width = 2,
                        Height = 2,
                        diameter = 2,
                        frequency_band = "2",
                        SpaceLibrary = 2,
                        Active = true,
                        Deleted = false,
                        polarityTypeId = 1
                    },
                    new TLImwDishLibrary
                    {
                        Id = 3,
                        Model = "MW_DishLibraray3",
                        Description = "3",
                        Note = "3",
                        Weight = 3,
                        dimensions = "3",
                        Length = 3,
                        Width = 3,
                        Height = 3,
                        diameter = 3,
                        frequency_band = "3",
                        SpaceLibrary = 3,
                        Active = true,
                        Deleted = false,
                        polarityTypeId = 2
                    },
                    new TLImwDishLibrary
                    {
                        Id = 4,
                        Model = "MW_DishLibraray4",
                        Description = "4",
                        Note = "4",
                        Weight = 4,
                        dimensions = "4",
                        Length = 4,
                        Width = 4,
                        Height = 4,
                        diameter = 4,
                        frequency_band = "4",
                        SpaceLibrary = 4,
                        Active = true,
                        Deleted = false,
                        polarityTypeId = 2
                    },
                    new TLImwDishLibrary
                    {
                        Id = 5,
                        Model = "MW_DishLibraray5",
                        Description = "5",
                        Note = "5",
                        Weight = 5,
                        dimensions = "5",
                        Length = 5,
                        Width = 5,
                        Height = 5,
                        diameter = 5,
                        frequency_band = "5",
                        SpaceLibrary = 5,
                        Active = true,
                        Deleted = false,
                        polarityTypeId = 3
                    }
                };
                await _unitOfWork.MW_DishLibraryRepository.AddRangeAsync(SeedData);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Function take 2 parameters Id, TableName
        //First get table name Entity depened on TableName
        //Second specify the table i deal with
        //Get the record by Id
        //Get activated attributes and values
        //Get dynamic attributes
        public Response<AllItemAttributes> GetById(int Id, string TableName)
        {
            try
            {
                AllItemAttributes attributes = new AllItemAttributes();

                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c =>
                    c.TableName == TableName);

                List<BaseAttView> ListAttributesActivated = new List<BaseAttView>();

                if (LoadSubType.TLImwBULibrary.ToString() == TableName)
                {
                    TLImwBULibrary MWBULibrary = _unitOfWork.MW_BULibraryRepository.GetIncludeWhereFirst(x =>
                        x.Id == Id, x => x.diversityType);

                    object FK_DiversityType_Name = MWBULibrary.diversityType != null ? MWBULibrary.diversityType.Name : null;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, MWBULibrary, null).ToList();

                    foreach (BaseAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Label.ToLower() == "diversitytype_name")
                        {
                            if (FK_DiversityType_Name == null)
                                FKitem.Value = _unitOfWork.DiversityTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_DiversityType_Name;
                        }
                    }
                }
                else if (LoadSubType.TLImwDishLibrary.ToString() == TableName)
                {
                    TLImwDishLibrary MWDishLibrary = _unitOfWork.MW_DishLibraryRepository.GetIncludeWhereFirst(x =>
                        x.Id == Id, x => x.asType, x => x.polarityType);

                    object FK_AsType_Name = MWDishLibrary.asType != null ? MWDishLibrary.asType.Name : null;

                    object FK_PolarityType_Name = MWDishLibrary.polarityType != null ? MWDishLibrary.polarityType.Name : null;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, MWDishLibrary, null).ToList();

                    foreach (BaseAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Label.ToLower() == "polaritytype_name")
                        {
                            if (FK_PolarityType_Name == null)
                                FKitem.Value = _unitOfWork.PolarityTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_PolarityType_Name;
                        }
                        else if (FKitem.Label.ToLower() == "astype_name")
                        {
                            if (FK_AsType_Name == null)
                                FKitem.Value = _unitOfWork.AsTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_AsType_Name;
                        }
                    }
                }
                else if (LoadSubType.TLImwODULibrary.ToString() == TableName)
                {
                    TLImwODULibrary MWODUULibrary = _unitOfWork.MW_ODULibraryRepository.GetIncludeWhereFirst(x =>
                        x.Id == Id, x => x.parity);

                    object FK_Parity_Name = MWODUULibrary.parity != null ? MWODUULibrary.parity.Name : null;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, MWODUULibrary, null).ToList();

                    foreach (BaseAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Label.ToLower() == "parity_name")
                        {
                            if (FK_Parity_Name == null)
                                FKitem.Value = _unitOfWork.ParityRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_Parity_Name;
                        }
                    }
                }
                else if (LoadSubType.TLImwRFULibrary.ToString() == TableName)
                {
                    TLImwRFULibrary MWRFULibrary = _unitOfWork.MW_RFULibraryRepository.GetIncludeWhereFirst(x =>
                        x.Id == Id, x => x.boardType, x => x.diversityType);

                    object FK_BoardType_Name = MWRFULibrary.boardType != null ? MWRFULibrary.boardType.Name : null;

                    object FK_DiversityType_Name = MWRFULibrary.diversityType != null ? MWRFULibrary.diversityType.Name : null;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, MWRFULibrary, null).ToList();

                    foreach (BaseAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Label.ToLower() == "boardtype_name")
                        {
                            if (FK_BoardType_Name == null)
                                FKitem.Value = _unitOfWork.BoardTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_BoardType_Name;
                        }
                        else if (FKitem.Label.ToLower() == "diversitytype_name")
                        {
                            if (FK_DiversityType_Name == null)
                                FKitem.Value = _unitOfWork.DiversityTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_DiversityType_Name;
                        }
                    }
                }
                else if (LoadSubType.TLImwOtherLibrary.ToString() == TableName)
                {
                    TLImwOtherLibrary MWOtherLibrary = _unitOfWork.MW_OtherLibraryRepository.GetWhereFirst(x =>
                        x.Id == Id);

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, MWOtherLibrary, null).ToList();
                }

                ListAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.MW.ToString(), TableName, Id));
                attributes.AttributesActivated = ListAttributesActivated;
                attributes.DynamicAtts = _unitOfWork.DynamicAttLibRepository.GetDynamicLibAtts(TableNameEntity.Id, Id, null);
                attributes.DynamicAttInst = null;

                List<BaseAttView> Test = attributes.AttributesActivated.ToList();
                BaseAttView NameAttribute = Test.FirstOrDefault(x => x.Key.ToLower() == "Model".ToLower());
                if (NameAttribute != null)
                {
                    BaseAttView Swap = Test.ToList()[0];
                    Test[Test.IndexOf(NameAttribute)] = Swap;
                    Test[0] = NameAttribute;
                    attributes.AttributesActivated = Test;
                }

                return new Response<AllItemAttributes>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function return list of records depened on filters and parameters
        //If WithFilterData is true then return related tables
        public Response<ReturnWithFilters<MW_BULibraryViewModel>> get_MW_BU_LibrariesAsync(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                IEnumerable<TLImwBULibrary> MW_BU_LibrariesList;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                MW_BU_LibrariesList = _unitOfWork.MW_BULibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, M => M.diversityType).OrderBy(x => x.Id).ToList();
                var FilteredMW_BULibraryViewModel = _mapper.Map<IEnumerable<MW_BULibraryViewModel>>(MW_BU_LibrariesList);
                ReturnWithFilters<MW_BULibraryViewModel> MW_BULibrary = new ReturnWithFilters<MW_BULibraryViewModel>();
                MW_BULibrary.Model = FilteredMW_BULibraryViewModel.ToList();
                if (WithFilterData.Equals(true))
                {
                    MW_BULibrary.filters = _unitOfWork.MW_BULibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<MW_BULibraryViewModel>>(true, MW_BULibrary, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<MW_BULibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function return list of records depened on filters and parameters
        //If WithFilterData is true then return related tables
        public Response<ReturnWithFilters<MW_DishLibraryViewModel>> get_MW_Dish_LibrariesAsync(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                IEnumerable<TLImwDishLibrary> Mw_Dish_LibrariesList;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                Mw_Dish_LibrariesList = _unitOfWork.MW_DishLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, m => m.asType, m => m.polarityType).OrderBy(x => x.Id).ToList();
                var FilteredMW_DishLibraryViewModel = _mapper.Map<IEnumerable<MW_DishLibraryViewModel>>(Mw_Dish_LibrariesList);
                ReturnWithFilters<MW_DishLibraryViewModel> MW_DishLibrary = new ReturnWithFilters<MW_DishLibraryViewModel>();
                MW_DishLibrary.Model = FilteredMW_DishLibraryViewModel.ToList();
                if (WithFilterData.Equals(true))
                {
                    MW_DishLibrary.filters = _unitOfWork.MW_DishLibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<MW_DishLibraryViewModel>>(true, MW_DishLibrary, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<MW_DishLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function return list of records depened on filters and parameters
        //If WithFilterData is true then return related tables
        public Response<ReturnWithFilters<MW_ODULibraryViewModel>> get_MW_ODU_LibrariesAsync(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                IEnumerable<TLImwODULibrary> MW_ODU_LibrariesList;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                // condition.Add(new FilterObject("Deleted", false));
                MW_ODU_LibrariesList = _unitOfWork.MW_ODULibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, m => m.parity).OrderBy(x => x.Id).ToList();
                var FilteredMW_ODULibraryViewModel = _mapper.Map<IEnumerable<MW_ODULibraryViewModel>>(MW_ODU_LibrariesList);
                ReturnWithFilters<MW_ODULibraryViewModel> MW_ODULibrary = new ReturnWithFilters<MW_ODULibraryViewModel>();
                MW_ODULibrary.Model = FilteredMW_ODULibraryViewModel.ToList();
                if (WithFilterData.Equals(true))
                {
                    MW_ODULibrary.filters = _unitOfWork.MW_ODULibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<MW_ODULibraryViewModel>>(true, MW_ODULibrary, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<MW_ODULibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function return list of records depened on filters and parameters
        //If WithFilterData is true then return related tables
        public Response<ReturnWithFilters<MW_RFULibraryViewModel>> get_MW_RFU_LibrariesAsync(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                IEnumerable<TLImwRFULibrary> MW_RFU_LibrariesList;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                MW_RFU_LibrariesList = _unitOfWork.MW_RFULibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, m => m.diversityType).OrderBy(x => x.Id).ToList();
                var FilteredMW_RFULibrary = _mapper.Map<IEnumerable<MW_RFULibraryViewModel>>(MW_RFU_LibrariesList);
                ReturnWithFilters<MW_RFULibraryViewModel> MW_RFULibrary = new ReturnWithFilters<MW_RFULibraryViewModel>();
                MW_RFULibrary.Model = FilteredMW_RFULibrary.ToList();
                if (WithFilterData.Equals(true))
                {
                    MW_RFULibrary.filters = _unitOfWork.MW_RFULibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<MW_RFULibraryViewModel>>(true, MW_RFULibrary, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<MW_RFULibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters filters, parameters
        //Function return list of records depened on filters and parameters
        public Response<ReturnWithFilters<MW_OtherLibraryViewModel>> get_MW_Other_LibrariesAsync(List<FilterObjectList> filters, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                IEnumerable<TLImwOtherLibrary> MW_Other_LibrariesList;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                MW_Other_LibrariesList = _unitOfWork.MW_OtherLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, null).OrderBy(x => x.Id).ToList();
                var FilteredMW_OtherLibraryViewModel = _mapper.Map<IEnumerable<MW_OtherLibraryViewModel>>(MW_Other_LibrariesList);
                ReturnWithFilters<MW_OtherLibraryViewModel> MW_OtherLibrary = new ReturnWithFilters<MW_OtherLibraryViewModel>();
                MW_OtherLibrary.Model = FilteredMW_OtherLibraryViewModel.ToList();
                MW_OtherLibrary.filters = null;
                return new Response<ReturnWithFilters<MW_OtherLibraryViewModel>>(true, MW_OtherLibrary, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<MW_OtherLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        #region Get Enabled Attributes Only With Dynamic Objects (Libraries Only)...
        #region Helper Methods
        public void GetInventoriesIdsFromDynamicAttributes(out List<int> DynamicLibValueListIds, List<TLIdynamicAtt> LibDynamicAttListIds, List<StringFilterObjectList> AttributeFilters)
        {
            try
            {
                List<StringFilterObjectList> DynamicLibAttributeFilters = AttributeFilters.Where(x =>
                    LibDynamicAttListIds.Exists(y => y.Key.ToLower() == x.key.ToLower())).ToList();

                DynamicLibValueListIds = new List<int>();

                List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                    LibDynamicAttListIds.Exists(y => y.Id == x.DynamicAttId) && !x.disable).ToList();

                List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                foreach (int InventoryId in InventoriesIds)
                {
                    List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x => x.InventoryId == InventoryId).ToList();

                    if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Exists(x =>
                         (x.ValueBoolean != null) ?
                            (y.value.Any(z => x.ValueBoolean.ToString().ToLower().StartsWith(z.ToLower()))) :

                         (x.ValueDateTime != null ?
                            (y.value.Any(z => z.ToLower() == x.ValueDateTime.ToString().ToLower())) :

                         (x.ValueDouble != null ?
                            (y.value.Any(z => z.ToLower() == x.ValueDouble.ToString().ToLower())) :

                         (!string.IsNullOrEmpty(x.ValueString) ?
                            (y.value.Any(z => x.ValueString.ToLower().StartsWith(z.ToLower()))) : (false)))))))
                    {
                        DynamicLibValueListIds.Add(InventoryId);
                    }
                }
                return;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public Response<ReturnWithFilters<object>> GetMW_BULibraries(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> MW_BUTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<MW_BULibraryViewModel> MW_BULibraries = new List<MW_BULibraryViewModel>();
                List<MW_BULibraryViewModel> WithoutDateFilterMW_BULibraries = new List<MW_BULibraryViewModel>();
                List<MW_BULibraryViewModel> WithDateFilterMW_BULibraries = new List<MW_BULibraryViewModel>();

                List<TLIattributeActivated> MW_BULibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    MW_BULibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_BULibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == TablesNames.TLImwBULibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateMW_BULibraryAttribute = MW_BULibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateMW_BULibraryAttribute.FirstOrDefault(x =>
                            x.Label.ToLower() == item.key.ToLower());

                        string Key = "";

                        if (AttributeKey != null)
                            Key = AttributeKey.Key;

                        else
                            Key = item.key;

                        AttributeFilters.Add(new StringFilterObjectList
                        {
                            key = Key,
                            value = value
                        });
                    }
                }
                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> LibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLImwBULibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (LibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        GetInventoriesIdsFromDynamicAttributes(out DynamicLibValueListIds, LibDynamicAttListIds, AttributeFilters);
                    }

                    //
                    // Library Attribute Activated...
                    //
                    bool AttrLibExist = typeof(MW_BULibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower() && y.key.ToLower() != "id"));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(MW_BULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Exists(y =>
                                y.key.ToLower() == x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(MW_BULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Exists(y =>
                                y.key.ToLower() == x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower()) ||
                            StringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                        LibraryAttributeActivatedIds = _unitOfWork.MW_BULibraryRepository.GetWhere(x =>
                             LibraryPropsAttributeFilters.All(z =>
                                NonStringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<MW_BULibraryViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<MW_BULibraryViewModel>(x), null).ToString().ToLower()) : false)) ||
                                StringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                                     y.GetValue(_mapper.Map<MW_BULibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<MW_BULibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                         ).Select(i => i.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithoutDateFilterMW_BULibraries = _mapper.Map<List<MW_BULibraryViewModel>>(_unitOfWork.MW_BULibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.diversityType).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateMW_BULibraryAttribute = MW_BULibraryAttribute.Where(x =>
                        x.DataType.ToLower() == "datetime").ToList();

                    foreach (DateFilterViewModel item in DateFilter)
                    {
                        DateTime DateFrom = Convert.ToDateTime(item.DateFrom);
                        DateTime DateTo = Convert.ToDateTime(item.DateTo);

                        if (DateFrom > DateTo)
                        {
                            DateTime Replacer = DateFrom;
                            DateFrom = DateTo;
                            DateTo = Replacer;
                        }

                        TLIattributeActivated AttributeKey = DateMW_BULibraryAttribute.FirstOrDefault(x =>
                            x.Label.ToLower() == item.key.ToLower());
                        string Key = "";

                        if (AttributeKey != null)
                            Key = AttributeKey.Key;
                        else
                            Key = item.key;

                        AfterConvertDateFilters.Add(new DateFilterViewModel
                        {
                            key = Key,
                            DateFrom = DateFrom,
                            DateTo = DateTo
                        });
                    }
                }
                if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> DateTimeLibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLImwBULibrary.ToString(), x => x.tablesNames).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (DateTimeLibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeLibDynamicAttListIds.Exists(y => y.Key.ToLower() == x.key.ToLower())).ToList();

                        DynamicLibValueListIds = new List<int>();

                        List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            DateTimeLibDynamicAttListIds.Exists(y => y.Id == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Exists(x =>
                                 (x.ValueDateTime != null ?
                                    (x.ValueDateTime >= y.DateFrom && x.ValueDateTime <= y.DateTo) : (false)))))
                            {
                                DynamicLibValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Library Attribute Activated...
                    //
                    List<PropertyInfo> LibraryProps = typeof(MW_BULibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                        LibraryAttributeActivatedIds = _unitOfWork.MW_BULibraryRepository.GetIncludeWhere(x =>
                            LibraryPropsAttributeFilters.All(z =>
                                (LibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<MW_BULibraryViewModel>(x), null) != null) ?
                                    ((z.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_BULibraryViewModel>(x), null))) &&
                                     (z.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_BULibraryViewModel>(x), null)))) : (false)))))
                        ).Select(i => i.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithDateFilterMW_BULibraries = _mapper.Map<List<MW_BULibraryViewModel>>(_unitOfWork.MW_BULibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.diversityType).ToList());
                }

                //
                // Intersect Between WithoutDateFilterMW_BULibraries + WithDateFilterMW_BULibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    MW_BULibraries = _mapper.Map<List<MW_BULibraryViewModel>>(_unitOfWork.MW_BULibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted, x => x.diversityType).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> MicrowaveIds = WithoutDateFilterMW_BULibraries.Select(x => x.Id).Intersect(WithDateFilterMW_BULibraries.Select(x => x.Id)).ToList();
                    MW_BULibraries = _mapper.Map<List<MW_BULibraryViewModel>>(_unitOfWork.MW_BULibraryRepository.GetWhere(x =>
                        MicrowaveIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    MW_BULibraries = WithoutDateFilterMW_BULibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    MW_BULibraries = WithDateFilterMW_BULibraries;
                }

                Count = MW_BULibraries.Count();

                MW_BULibraries = MW_BULibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_BULibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == TablesNames.TLImwBULibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == TablesNames.TLImwBULibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == TablesNames.TLImwBULibrary.ToString()) : false),
                       x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
                       x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

                List<TLIattributeViewManagment> NotDateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime") : false).ToList();

                List<TLIattributeViewManagment> NotDateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() != "datetime" : false).ToList();

                List<TLIattributeViewManagment> DateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

                List<TLIattributeViewManagment> DateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() == "datetime" : false).ToList();

                foreach (MW_BULibraryViewModel MW_BULibraryViewModel in MW_BULibraries)
                {
                    dynamic DynamicMW_BULibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(MW_BULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in LibraryProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeLibraryAttributesViewModel.Exists(x =>
                                    x.AttributeActivated.Label.ToLower() == prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(MW_BULibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicMW_BULibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Exists(x =>
                                 x.AttributeActivated.Key.ToLower() == prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == TablesNames.TLImwBULibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(MW_BULibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicMW_BULibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(MW_BULibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicMW_BULibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLImwBULibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == MW_BULibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();

                            if (DynamicAttLibValue.ValueString != null)
                                DynamicAttValue = DynamicAttLibValue.ValueString;

                            else if (DynamicAttLibValue.ValueDouble != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDouble;

                            else if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            else if (DynamicAttLibValue.ValueBoolean != null)
                                DynamicAttValue = DynamicAttLibValue.ValueBoolean;

                            ((IDictionary<String, Object>)DynamicMW_BULibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicMW_BULibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(MW_BULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == TablesNames.TLImwBULibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(MW_BULibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLImwBULibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == MW_BULibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();
                            if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    ((IDictionary<String, Object>)DynamicMW_BULibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicMW_BULibrary);
                }

                MW_BUTableDisplay.Model = OutPutList;

                if (WithFilterData)
                {
                    MW_BUTableDisplay.filters = _unitOfWork.MW_BULibraryRepository.GetRelatedTables();
                }
                else
                {
                    MW_BUTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, MW_BUTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetMW_DishLibraries(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> MW_DishTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<MW_DishLibraryViewModel> MW_DishLibraries = new List<MW_DishLibraryViewModel>();
                List<MW_DishLibraryViewModel> WithoutDateFilterMW_DishLibraries = new List<MW_DishLibraryViewModel>();
                List<MW_DishLibraryViewModel> WithDateFilterMW_DishLibraries = new List<MW_DishLibraryViewModel>();

                List<TLIattributeActivated> MW_DishLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    MW_DishLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_DishLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == TablesNames.TLImwDishLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateMW_DishLibraryAttribute = MW_DishLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateMW_DishLibraryAttribute.FirstOrDefault(x =>
                            x.Label.ToLower() == item.key.ToLower());

                        string Key = "";

                        if (AttributeKey != null)
                            Key = AttributeKey.Key;

                        else
                            Key = item.key;

                        AttributeFilters.Add(new StringFilterObjectList
                        {
                            key = Key,
                            value = value
                        });
                    }
                }
                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> LibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLImwDishLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (LibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        GetInventoriesIdsFromDynamicAttributes(out DynamicLibValueListIds, LibDynamicAttListIds, AttributeFilters);
                    }

                    //
                    // Library Attribute Activated...
                    //
                    bool AttrLibExist = typeof(MW_DishLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower() && y.key.ToLower() != "id"));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(MW_DishLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Exists(y =>
                                y.key.ToLower() == x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(MW_DishLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Exists(y =>
                                y.key.ToLower() == x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower()) ||
                            StringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                        LibraryAttributeActivatedIds = _unitOfWork.MW_DishLibraryRepository.GetWhere(x =>
                             LibraryPropsAttributeFilters.All(z =>
                                NonStringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<MW_DishLibraryViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<MW_DishLibraryViewModel>(x), null).ToString().ToLower()) : false)) ||
                                StringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                                     y.GetValue(_mapper.Map<MW_DishLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<MW_DishLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                         ).Select(i => i.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithoutDateFilterMW_DishLibraries = _mapper.Map<List<MW_DishLibraryViewModel>>(_unitOfWork.MW_DishLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.asType, x => x.polarityType).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateMW_DishLibraryAttribute = MW_DishLibraryAttribute.Where(x =>
                        x.DataType.ToLower() == "datetime").ToList();

                    foreach (DateFilterViewModel item in DateFilter)
                    {
                        DateTime DateFrom = Convert.ToDateTime(item.DateFrom);
                        DateTime DateTo = Convert.ToDateTime(item.DateTo);

                        if (DateFrom > DateTo)
                        {
                            DateTime Replacer = DateFrom;
                            DateFrom = DateTo;
                            DateTo = Replacer;
                        }

                        TLIattributeActivated AttributeKey = DateMW_DishLibraryAttribute.FirstOrDefault(x =>
                            x.Label.ToLower() == item.key.ToLower());
                        string Key = "";

                        if (AttributeKey != null)
                            Key = AttributeKey.Key;
                        else
                            Key = item.key;

                        AfterConvertDateFilters.Add(new DateFilterViewModel
                        {
                            key = Key,
                            DateFrom = DateFrom,
                            DateTo = DateTo
                        });
                    }
                }
                if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> DateTimeLibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLImwDishLibrary.ToString(), x => x.tablesNames).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (DateTimeLibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeLibDynamicAttListIds.Exists(y => y.Key.ToLower() == x.key.ToLower())).ToList();

                        DynamicLibValueListIds = new List<int>();

                        List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            DateTimeLibDynamicAttListIds.Exists(y => y.Id == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Exists(x =>
                                 (x.ValueDateTime != null ?
                                    (x.ValueDateTime >= y.DateFrom && x.ValueDateTime <= y.DateTo) : (false)))))
                            {
                                DynamicLibValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Library Attribute Activated...
                    //
                    List<PropertyInfo> LibraryProps = typeof(MW_DishLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                        LibraryAttributeActivatedIds = _unitOfWork.MW_DishLibraryRepository.GetIncludeWhere(x =>
                            LibraryPropsAttributeFilters.All(z =>
                                (LibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<MW_DishLibraryViewModel>(x), null) != null) ?
                                    ((z.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_DishLibraryViewModel>(x), null))) &&
                                     (z.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_DishLibraryViewModel>(x), null)))) : (false)))))
                        ).Select(i => i.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithDateFilterMW_DishLibraries = _mapper.Map<List<MW_DishLibraryViewModel>>(_unitOfWork.MW_DishLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.asType, x => x.polarityType).ToList());
                }

                //
                // Intersect Between WithoutDateFilterMW_DishLibraries + WithDateFilterMW_DishLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    MW_DishLibraries = _mapper.Map<List<MW_DishLibraryViewModel>>(_unitOfWork.MW_DishLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted, x => x.asType, x => x.polarityType).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> MicrowaveIds = WithoutDateFilterMW_DishLibraries.Select(x => x.Id).Intersect(WithDateFilterMW_DishLibraries.Select(x => x.Id)).ToList();
                    MW_DishLibraries = _mapper.Map<List<MW_DishLibraryViewModel>>(_unitOfWork.MW_DishLibraryRepository.GetWhere(x =>
                        MicrowaveIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    MW_DishLibraries = WithoutDateFilterMW_DishLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    MW_DishLibraries = WithDateFilterMW_DishLibraries;
                }

                Count = MW_DishLibraries.Count();

                MW_DishLibraries = MW_DishLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_DishLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == TablesNames.TLImwDishLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == TablesNames.TLImwDishLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == TablesNames.TLImwDishLibrary.ToString()) : false),
                       x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
                       x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

                List<TLIattributeViewManagment> NotDateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime") : false).ToList();

                List<TLIattributeViewManagment> NotDateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() != "datetime" : false).ToList();

                List<TLIattributeViewManagment> DateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

                List<TLIattributeViewManagment> DateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() == "datetime" : false).ToList();

                foreach (MW_DishLibraryViewModel MW_DishLibraryViewModel in MW_DishLibraries)
                {
                    dynamic DynamicMW_DishLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(MW_DishLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in LibraryProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeLibraryAttributesViewModel.Exists(x =>
                                    x.AttributeActivated.Label.ToLower() == prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(MW_DishLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicMW_DishLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Exists(x =>
                                 x.AttributeActivated.Key.ToLower() == prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == TablesNames.TLImwDishLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(MW_DishLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicMW_DishLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(MW_DishLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicMW_DishLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLImwDishLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == MW_DishLibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();

                            if (DynamicAttLibValue.ValueString != null)
                                DynamicAttValue = DynamicAttLibValue.ValueString;

                            else if (DynamicAttLibValue.ValueDouble != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDouble;

                            else if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            else if (DynamicAttLibValue.ValueBoolean != null)
                                DynamicAttValue = DynamicAttLibValue.ValueBoolean;

                            ((IDictionary<String, Object>)DynamicMW_DishLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicMW_DishLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(MW_DishLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == TablesNames.TLImwDishLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(MW_DishLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLImwDishLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == MW_DishLibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();
                            if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    ((IDictionary<String, Object>)DynamicMW_DishLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicMW_DishLibrary);
                }

                MW_DishTableDisplay.Model = OutPutList;

                if (WithFilterData)
                {
                    MW_DishTableDisplay.filters = _unitOfWork.MW_DishLibraryRepository.GetRelatedTables();
                }
                else
                {
                    MW_DishTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, MW_DishTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetMW_ODULibraries(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> MW_ODUTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<MW_ODULibraryViewModel> MW_ODULibraries = new List<MW_ODULibraryViewModel>();
                List<MW_ODULibraryViewModel> WithoutDateFilterMW_ODULibraries = new List<MW_ODULibraryViewModel>();
                List<MW_ODULibraryViewModel> WithDateFilterMW_ODULibraries = new List<MW_ODULibraryViewModel>();

                List<TLIattributeActivated> MW_ODULibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    MW_ODULibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_ODULibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == TablesNames.TLImwODULibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateMW_ODULibraryAttribute = MW_ODULibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateMW_ODULibraryAttribute.FirstOrDefault(x =>
                            x.Label.ToLower() == item.key.ToLower());

                        string Key = "";

                        if (AttributeKey != null)
                            Key = AttributeKey.Key;

                        else
                            Key = item.key;

                        AttributeFilters.Add(new StringFilterObjectList
                        {
                            key = Key,
                            value = value
                        });
                    }
                }
                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> LibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLImwODULibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (LibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        GetInventoriesIdsFromDynamicAttributes(out DynamicLibValueListIds, LibDynamicAttListIds, AttributeFilters);
                    }

                    //
                    // Library Attribute Activated...
                    //
                    bool AttrLibExist = typeof(MW_ODULibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower() && y.key.ToLower() != "id"));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(MW_ODULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Exists(y =>
                                y.key.ToLower() == x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(MW_ODULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Exists(y =>
                                y.key.ToLower() == x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower()) ||
                            StringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                        LibraryAttributeActivatedIds = _unitOfWork.MW_ODULibraryRepository.GetWhere(x =>
                             LibraryPropsAttributeFilters.All(z =>
                                NonStringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<MW_ODULibraryViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<MW_ODULibraryViewModel>(x), null).ToString().ToLower()) : false)) ||
                                StringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                                     y.GetValue(_mapper.Map<MW_ODULibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<MW_ODULibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                         ).Select(i => i.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithoutDateFilterMW_ODULibraries = _mapper.Map<List<MW_ODULibraryViewModel>>(_unitOfWork.MW_ODULibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.parity).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateMW_ODULibraryAttribute = MW_ODULibraryAttribute.Where(x =>
                        x.DataType.ToLower() == "datetime").ToList();

                    foreach (DateFilterViewModel item in DateFilter)
                    {
                        DateTime DateFrom = Convert.ToDateTime(item.DateFrom);
                        DateTime DateTo = Convert.ToDateTime(item.DateTo);

                        if (DateFrom > DateTo)
                        {
                            DateTime Replacer = DateFrom;
                            DateFrom = DateTo;
                            DateTo = Replacer;
                        }

                        TLIattributeActivated AttributeKey = DateMW_ODULibraryAttribute.FirstOrDefault(x =>
                            x.Label.ToLower() == item.key.ToLower());
                        string Key = "";

                        if (AttributeKey != null)
                            Key = AttributeKey.Key;
                        else
                            Key = item.key;

                        AfterConvertDateFilters.Add(new DateFilterViewModel
                        {
                            key = Key,
                            DateFrom = DateFrom,
                            DateTo = DateTo
                        });
                    }
                }
                if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> DateTimeLibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLImwODULibrary.ToString(), x => x.tablesNames).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (DateTimeLibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeLibDynamicAttListIds.Exists(y => y.Key.ToLower() == x.key.ToLower())).ToList();

                        DynamicLibValueListIds = new List<int>();

                        List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            DateTimeLibDynamicAttListIds.Exists(y => y.Id == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Exists(x =>
                                 (x.ValueDateTime != null ?
                                    (x.ValueDateTime >= y.DateFrom && x.ValueDateTime <= y.DateTo) : (false)))))
                            {
                                DynamicLibValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Library Attribute Activated...
                    //
                    List<PropertyInfo> LibraryProps = typeof(MW_ODULibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                        LibraryAttributeActivatedIds = _unitOfWork.MW_ODULibraryRepository.GetIncludeWhere(x =>
                            LibraryPropsAttributeFilters.All(z =>
                                (LibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<MW_ODULibraryViewModel>(x), null) != null) ?
                                    ((z.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_ODULibraryViewModel>(x), null))) &&
                                     (z.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_ODULibraryViewModel>(x), null)))) : (false)))))
                        ).Select(i => i.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithDateFilterMW_ODULibraries = _mapper.Map<List<MW_ODULibraryViewModel>>(_unitOfWork.MW_ODULibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.parity).ToList());
                }

                //
                // Intersect Between WithoutDateFilterMW_ODULibraries + WithDateFilterMW_ODULibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    MW_ODULibraries = _mapper.Map<List<MW_ODULibraryViewModel>>(_unitOfWork.MW_ODULibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted, x => x.parity).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> MicrowaveIds = WithoutDateFilterMW_ODULibraries.Select(x => x.Id).Intersect(WithDateFilterMW_ODULibraries.Select(x => x.Id)).ToList();
                    MW_ODULibraries = _mapper.Map<List<MW_ODULibraryViewModel>>(_unitOfWork.MW_ODULibraryRepository.GetWhere(x =>
                        MicrowaveIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    MW_ODULibraries = WithoutDateFilterMW_ODULibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    MW_ODULibraries = WithDateFilterMW_ODULibraries;
                }

                Count = MW_ODULibraries.Count();

                MW_ODULibraries = MW_ODULibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_ODULibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == TablesNames.TLImwODULibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == TablesNames.TLImwODULibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == TablesNames.TLImwODULibrary.ToString()) : false),
                       x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
                       x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

                List<TLIattributeViewManagment> NotDateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime") : false).ToList();

                List<TLIattributeViewManagment> NotDateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() != "datetime" : false).ToList();

                List<TLIattributeViewManagment> DateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

                List<TLIattributeViewManagment> DateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() == "datetime" : false).ToList();

                foreach (MW_ODULibraryViewModel MW_ODULibraryViewModel in MW_ODULibraries)
                {
                    dynamic DynamicMW_ODULibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(MW_ODULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in LibraryProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeLibraryAttributesViewModel.Exists(x =>
                                    x.AttributeActivated.Label.ToLower() == prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(MW_ODULibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicMW_ODULibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Exists(x =>
                                 x.AttributeActivated.Key.ToLower() == prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == TablesNames.TLImwODULibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(MW_ODULibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicMW_ODULibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(MW_ODULibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicMW_ODULibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLImwODULibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == MW_ODULibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();

                            if (DynamicAttLibValue.ValueString != null)
                                DynamicAttValue = DynamicAttLibValue.ValueString;

                            else if (DynamicAttLibValue.ValueDouble != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDouble;

                            else if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            else if (DynamicAttLibValue.ValueBoolean != null)
                                DynamicAttValue = DynamicAttLibValue.ValueBoolean;

                            ((IDictionary<String, Object>)DynamicMW_ODULibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicMW_ODULibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(MW_ODULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == TablesNames.TLImwODULibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(MW_ODULibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLImwODULibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == MW_ODULibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();
                            if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    ((IDictionary<String, Object>)DynamicMW_ODULibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicMW_ODULibrary);
                }

                MW_ODUTableDisplay.Model = OutPutList;

                if (WithFilterData)
                {
                    MW_ODUTableDisplay.filters = _unitOfWork.MW_ODULibraryRepository.GetRelatedTables();
                }
                else
                {
                    MW_ODUTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, MW_ODUTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetMW_RFULibraries(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> NW_RFUTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<MW_RFULibraryViewModel> MW_RFULibraries = new List<MW_RFULibraryViewModel>();
                List<MW_RFULibraryViewModel> WithoutDateFilterMW_RFULibraries = new List<MW_RFULibraryViewModel>();
                List<MW_RFULibraryViewModel> WithDateFilterMW_RFULibraries = new List<MW_RFULibraryViewModel>();

                List<TLIattributeActivated> MW_RFULibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    MW_RFULibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_RFULibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == TablesNames.TLImwRFULibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateMW_RFULibraryAttribute = MW_RFULibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateMW_RFULibraryAttribute.FirstOrDefault(x =>
                            x.Label.ToLower() == item.key.ToLower());

                        string Key = "";

                        if (AttributeKey != null)
                            Key = AttributeKey.Key;

                        else
                            Key = item.key;

                        AttributeFilters.Add(new StringFilterObjectList
                        {
                            key = Key,
                            value = value
                        });
                    }
                }
                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> LibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLImwRFULibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (LibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        GetInventoriesIdsFromDynamicAttributes(out DynamicLibValueListIds, LibDynamicAttListIds, AttributeFilters);
                    }

                    //
                    // Library Attribute Activated...
                    //
                    bool AttrLibExist = typeof(MW_RFULibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower() && y.key.ToLower() != "id"));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(MW_RFULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Exists(y =>
                                y.key.ToLower() == x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(MW_RFULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Exists(y =>
                                y.key.ToLower() == x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower()) ||
                            StringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                        LibraryAttributeActivatedIds = _unitOfWork.MW_RFULibraryRepository.GetWhere(x =>
                             LibraryPropsAttributeFilters.All(z =>
                                NonStringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<MW_RFULibraryViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<MW_RFULibraryViewModel>(x), null).ToString().ToLower()) : false)) ||
                                StringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                                     y.GetValue(_mapper.Map<MW_RFULibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<MW_RFULibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                         ).Select(i => i.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithoutDateFilterMW_RFULibraries = _mapper.Map<List<MW_RFULibraryViewModel>>(_unitOfWork.MW_RFULibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.boardType, x => x.diversityType).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateMW_RFULibraryAttribute = MW_RFULibraryAttribute.Where(x =>
                        x.DataType.ToLower() == "datetime").ToList();

                    foreach (DateFilterViewModel item in DateFilter)
                    {
                        DateTime DateFrom = Convert.ToDateTime(item.DateFrom);
                        DateTime DateTo = Convert.ToDateTime(item.DateTo);

                        if (DateFrom > DateTo)
                        {
                            DateTime Replacer = DateFrom;
                            DateFrom = DateTo;
                            DateTo = Replacer;
                        }

                        TLIattributeActivated AttributeKey = DateMW_RFULibraryAttribute.FirstOrDefault(x =>
                            x.Label.ToLower() == item.key.ToLower());
                        string Key = "";

                        if (AttributeKey != null)
                            Key = AttributeKey.Key;
                        else
                            Key = item.key;

                        AfterConvertDateFilters.Add(new DateFilterViewModel
                        {
                            key = Key,
                            DateFrom = DateFrom,
                            DateTo = DateTo
                        });
                    }
                }
                if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> DateTimeLibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLImwRFULibrary.ToString(), x => x.tablesNames).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (DateTimeLibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeLibDynamicAttListIds.Exists(y => y.Key.ToLower() == x.key.ToLower())).ToList();

                        DynamicLibValueListIds = new List<int>();

                        List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            DateTimeLibDynamicAttListIds.Exists(y => y.Id == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Exists(x =>
                                 (x.ValueDateTime != null ?
                                    (x.ValueDateTime >= y.DateFrom && x.ValueDateTime <= y.DateTo) : (false)))))
                            {
                                DynamicLibValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Library Attribute Activated...
                    //
                    List<PropertyInfo> LibraryProps = typeof(MW_RFULibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                        LibraryAttributeActivatedIds = _unitOfWork.MW_RFULibraryRepository.GetIncludeWhere(x =>
                            LibraryPropsAttributeFilters.All(z =>
                                (LibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<MW_RFULibraryViewModel>(x), null) != null) ?
                                    ((z.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_RFULibraryViewModel>(x), null))) &&
                                     (z.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_RFULibraryViewModel>(x), null)))) : (false)))))
                        ).Select(i => i.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithDateFilterMW_RFULibraries = _mapper.Map<List<MW_RFULibraryViewModel>>(_unitOfWork.MW_RFULibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.boardType, x => x.diversityType).ToList());
                }

                //
                // Intersect Between WithoutDateFilterMW_RFULibraries + WithDateFilterMW_RFULibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    MW_RFULibraries = _mapper.Map<List<MW_RFULibraryViewModel>>(_unitOfWork.MW_RFULibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted, x => x.boardType, x => x.diversityType).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> MicrowaveIds = WithoutDateFilterMW_RFULibraries.Select(x => x.Id).Intersect(WithDateFilterMW_RFULibraries.Select(x => x.Id)).ToList();
                    MW_RFULibraries = _mapper.Map<List<MW_RFULibraryViewModel>>(_unitOfWork.MW_RFULibraryRepository.GetWhere(x =>
                        MicrowaveIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    MW_RFULibraries = WithoutDateFilterMW_RFULibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    MW_RFULibraries = WithDateFilterMW_RFULibraries;
                }

                Count = MW_RFULibraries.Count();

                MW_RFULibraries = MW_RFULibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_RFULibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == TablesNames.TLImwRFULibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == TablesNames.TLImwRFULibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == TablesNames.TLImwRFULibrary.ToString()) : false),
                       x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
                       x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

                List<TLIattributeViewManagment> NotDateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime") : false).ToList();

                List<TLIattributeViewManagment> NotDateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() != "datetime" : false).ToList();

                List<TLIattributeViewManagment> DateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

                List<TLIattributeViewManagment> DateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() == "datetime" : false).ToList();

                foreach (MW_RFULibraryViewModel MW_RFULibraryViewModel in MW_RFULibraries)
                {
                    dynamic DynamicMW_RFULibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(MW_RFULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in LibraryProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeLibraryAttributesViewModel.Exists(x =>
                                    x.AttributeActivated.Label.ToLower() == prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(MW_RFULibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicMW_RFULibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Exists(x =>
                                 x.AttributeActivated.Key.ToLower() == prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == TablesNames.TLImwRFULibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(MW_RFULibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicMW_RFULibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(MW_RFULibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicMW_RFULibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLImwRFULibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == MW_RFULibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();

                            if (DynamicAttLibValue.ValueString != null)
                                DynamicAttValue = DynamicAttLibValue.ValueString;

                            else if (DynamicAttLibValue.ValueDouble != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDouble;

                            else if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            else if (DynamicAttLibValue.ValueBoolean != null)
                                DynamicAttValue = DynamicAttLibValue.ValueBoolean;

                            ((IDictionary<String, Object>)DynamicMW_RFULibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicMW_RFULibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(MW_RFULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == TablesNames.TLImwRFULibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(MW_RFULibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLImwRFULibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == MW_RFULibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();
                            if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    ((IDictionary<String, Object>)DynamicMW_RFULibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicMW_RFULibrary);
                }

                NW_RFUTableDisplay.Model = OutPutList;

                if (WithFilterData)
                {
                    NW_RFUTableDisplay.filters = _unitOfWork.MW_RFULibraryRepository.GetRelatedTables();
                }
                else
                {
                    NW_RFUTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, NW_RFUTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetMW_OtherLibraries(CombineFilters CombineFilters, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> MW_OtherTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<MW_OtherLibraryViewModel> MW_OtherLibraries = new List<MW_OtherLibraryViewModel>();
                List<MW_OtherLibraryViewModel> WithoutDateFilterMW_OtherLibraries = new List<MW_OtherLibraryViewModel>();
                List<MW_OtherLibraryViewModel> WithDateFilterMW_OtherLibraries = new List<MW_OtherLibraryViewModel>();

                List<TLIattributeActivated> MW_OtherLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    MW_OtherLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.OtherMWLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == TablesNames.TLImwOtherLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateMW_OtherLibraryAttribute = MW_OtherLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateMW_OtherLibraryAttribute.FirstOrDefault(x =>
                            x.Label.ToLower() == item.key.ToLower());

                        string Key = "";

                        if (AttributeKey != null)
                            Key = AttributeKey.Key;

                        else
                            Key = item.key;

                        AttributeFilters.Add(new StringFilterObjectList
                        {
                            key = Key,
                            value = value
                        });
                    }
                }
                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> LibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLImwOtherLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (LibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        GetInventoriesIdsFromDynamicAttributes(out DynamicLibValueListIds, LibDynamicAttListIds, AttributeFilters);
                    }

                    //
                    // Library Attribute Activated...
                    //
                    bool AttrLibExist = typeof(MW_OtherLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower() && y.key.ToLower() != "id"));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(MW_OtherLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Exists(y =>
                                y.key.ToLower() == x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(MW_OtherLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Exists(y =>
                                y.key.ToLower() == x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower()) ||
                            StringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                        LibraryAttributeActivatedIds = _unitOfWork.MW_OtherLibraryRepository.GetWhere(x =>
                             LibraryPropsAttributeFilters.All(z =>
                                NonStringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<MW_OtherLibraryViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<MW_OtherLibraryViewModel>(x), null).ToString().ToLower()) : false)) ||
                                StringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                                     y.GetValue(_mapper.Map<MW_OtherLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<MW_OtherLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                         ).Select(i => i.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithoutDateFilterMW_OtherLibraries = _mapper.Map<List<MW_OtherLibraryViewModel>>(_unitOfWork.MW_OtherLibraryRepository.GetWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateMW_OtherLibraryAttribute = MW_OtherLibraryAttribute.Where(x =>
                        x.DataType.ToLower() == "datetime").ToList();

                    foreach (DateFilterViewModel item in DateFilter)
                    {
                        DateTime DateFrom = Convert.ToDateTime(item.DateFrom);
                        DateTime DateTo = Convert.ToDateTime(item.DateTo);

                        if (DateFrom > DateTo)
                        {
                            DateTime Replacer = DateFrom;
                            DateFrom = DateTo;
                            DateTo = Replacer;
                        }

                        TLIattributeActivated AttributeKey = DateMW_OtherLibraryAttribute.FirstOrDefault(x =>
                            x.Label.ToLower() == item.key.ToLower());
                        string Key = "";

                        if (AttributeKey != null)
                            Key = AttributeKey.Key;
                        else
                            Key = item.key;

                        AfterConvertDateFilters.Add(new DateFilterViewModel
                        {
                            key = Key,
                            DateFrom = DateFrom,
                            DateTo = DateTo
                        });
                    }
                }
                if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> DateTimeLibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLImwOtherLibrary.ToString(), x => x.tablesNames).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (DateTimeLibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeLibDynamicAttListIds.Exists(y => y.Key.ToLower() == x.key.ToLower())).ToList();

                        DynamicLibValueListIds = new List<int>();

                        List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            DateTimeLibDynamicAttListIds.Exists(y => y.Id == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Exists(x =>
                                 (x.ValueDateTime != null ?
                                    (x.ValueDateTime >= y.DateFrom && x.ValueDateTime <= y.DateTo) : (false)))))
                            {
                                DynamicLibValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Library Attribute Activated...
                    //
                    List<PropertyInfo> LibraryProps = typeof(MW_OtherLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                        LibraryAttributeActivatedIds = _unitOfWork.MW_OtherLibraryRepository.GetIncludeWhere(x =>
                            LibraryPropsAttributeFilters.All(z =>
                                (LibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<MW_OtherLibraryViewModel>(x), null) != null) ?
                                    ((z.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_OtherLibraryViewModel>(x), null))) &&
                                     (z.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_OtherLibraryViewModel>(x), null)))) : (false)))))
                        ).Select(i => i.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithDateFilterMW_OtherLibraries = _mapper.Map<List<MW_OtherLibraryViewModel>>(_unitOfWork.MW_OtherLibraryRepository.GetWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // Intersect Between WithoutDateFilterMW_OtherLibraries + WithDateFilterMW_OtherLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    MW_OtherLibraries = _mapper.Map<List<MW_OtherLibraryViewModel>>(_unitOfWork.MW_OtherLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> MicrowaveIds = WithoutDateFilterMW_OtherLibraries.Select(x => x.Id).Intersect(WithDateFilterMW_OtherLibraries.Select(x => x.Id)).ToList();
                    MW_OtherLibraries = _mapper.Map<List<MW_OtherLibraryViewModel>>(_unitOfWork.MW_OtherLibraryRepository.GetWhere(x =>
                        MicrowaveIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    MW_OtherLibraries = WithoutDateFilterMW_OtherLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    MW_OtherLibraries = WithDateFilterMW_OtherLibraries;
                }

                Count = MW_OtherLibraries.Count();

                MW_OtherLibraries = MW_OtherLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.OtherMWLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == TablesNames.TLImwOtherLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == TablesNames.TLImwOtherLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == TablesNames.TLImwOtherLibrary.ToString()) : false),
                       x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
                       x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

                List<TLIattributeViewManagment> NotDateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime") : false).ToList();

                List<TLIattributeViewManagment> NotDateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() != "datetime" : false).ToList();

                List<TLIattributeViewManagment> DateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

                List<TLIattributeViewManagment> DateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() == "datetime" : false).ToList();

                foreach (MW_OtherLibraryViewModel MW_OtherLibraryViewModel in MW_OtherLibraries)
                {
                    dynamic DynamicMW_OtherLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(MW_OtherLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in LibraryProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeLibraryAttributesViewModel.Exists(x =>
                                    x.AttributeActivated.Label.ToLower() == prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(MW_OtherLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicMW_OtherLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Exists(x =>
                                 x.AttributeActivated.Key.ToLower() == prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == TablesNames.TLImwOtherLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(MW_OtherLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicMW_OtherLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(MW_OtherLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicMW_OtherLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLImwOtherLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == MW_OtherLibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();

                            if (DynamicAttLibValue.ValueString != null)
                                DynamicAttValue = DynamicAttLibValue.ValueString;

                            else if (DynamicAttLibValue.ValueDouble != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDouble;

                            else if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            else if (DynamicAttLibValue.ValueBoolean != null)
                                DynamicAttValue = DynamicAttLibValue.ValueBoolean;

                            ((IDictionary<String, Object>)DynamicMW_OtherLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicMW_OtherLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(MW_OtherLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == TablesNames.TLImwOtherLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(MW_OtherLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLImwOtherLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == MW_OtherLibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();
                            if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    ((IDictionary<String, Object>)DynamicMW_OtherLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicMW_OtherLibrary);
                }

                MW_OtherTableDisplay.Model = OutPutList;

                MW_OtherTableDisplay.filters = _unitOfWork.MW_OtherLibraryRepository.GetRelatedTables();

                return new Response<ReturnWithFilters<object>>(true, MW_OtherTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        #endregion
        //Function take 2 parameters TableName, LoadLibraryViewModel
        //First get table name Entity by TableName
        //Second specify the table i deal with
        //Map from LoadLibraryViewModel object to ViewModel
        //Map from ViewModel To Entity
        //Check validation
        //Add Entity
        //Add dynamic attributes values
        public Response<AllItemAttributes> AddMWLibrary(string TableName, object LoadLibraryViewModel, string connectionString)
        {
            using (var con = new OracleConnection(connectionString))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        try
                        {
                            string ErrorMessage = string.Empty;
                            //   var TableNameEntity = _unitOfWork.TablesNamesRepository.GetAllAsQueryable().AsNoTracking().Where(l => l.TableName == TableName).FirstOrDefault();
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == TableName);
                            if (LoadSubType.TLImwBULibrary.ToString() == TableName)
                            {
                                AddMW_BULibraryViewModel MW_BULibraryViewModel = _mapper.Map<AddMW_BULibraryViewModel>(LoadLibraryViewModel);
                                TLImwBULibrary MW_BULibraryEntity = _mapper.Map<TLImwBULibrary>(LoadLibraryViewModel);
                                bool test = true;
                                string CheckDependencyValidation = CheckDependencyValidationForMWTypes(LoadLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(MW_BULibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                if (test == true)
                                {
                                    var CheckModel = _unitOfWork.MW_BULibraryRepository.GetWhereFirst(x => x.Model == MW_BULibraryEntity.Model && !x.Deleted);
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {MW_BULibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                    //else if (MW_BULibraryEntity.Length <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Length Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_BULibraryEntity.Width <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Width Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_BULibraryEntity.Height <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Height Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_BULibraryEntity.SpaceLibrary <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "SpaceLibrary Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    _unitOfWork.MW_BULibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, MW_BULibraryEntity);
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = LoadLibraryViewModel;

                                    AddLogisticalItemWithMW(LogisticalItemIds, MW_BULibraryEntity, TableNameEntity.Id);


                                    if (MW_BULibraryViewModel.TLIdynamicAttLibValue.Count > 0)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(MW_BULibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.Id, MW_BULibraryEntity.Id);
                                    }
                                    _unitOfWork.TablesHistoryRepository.AddHistory(MW_BULibraryEntity.Id, Helpers.Constants.HistoryType.Add.ToString().ToLower(), TablesNames.TLImwBULibrary.ToString().ToLower());
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            else if (LoadSubType.TLImwDishLibrary.ToString() == TableName)
                            {
                                AddMW_DishLibraryViewModel MW_DishLibraryViewModel = _mapper.Map<AddMW_DishLibraryViewModel>(LoadLibraryViewModel);
                                TLImwDishLibrary MW_DishLibraryEntity = _mapper.Map<TLImwDishLibrary>(LoadLibraryViewModel);
                                bool test = true;
                                string CheckDependencyValidation = CheckDependencyValidationForMWTypes(LoadLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(MW_DishLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                if (test == true)
                                {
                                    var CheckModel = _unitOfWork.MW_DishLibraryRepository.GetWhereFirst(x => x.Model == MW_DishLibraryEntity.Model && !x.Deleted);
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {MW_DishLibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                    //else if (MW_DishLibraryEntity.Weight <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Weight Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_DishLibraryEntity.Length <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Length Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_DishLibraryEntity.Width <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Width Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_DishLibraryEntity.Height <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Height Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_DishLibraryEntity.diameter <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "diameter Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_DishLibraryEntity.SpaceLibrary <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "SpaceLibrary Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    _unitOfWork.MW_DishLibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, MW_DishLibraryEntity);
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = LoadLibraryViewModel;

                                    AddLogisticalItemWithMW(LogisticalItemIds, MW_DishLibraryEntity, TableNameEntity.Id);

                                    if (MW_DishLibraryViewModel.TLIdynamicAttLibValue.Count > 0)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(MW_DishLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.Id, MW_DishLibraryEntity.Id);
                                    }
                                    _unitOfWork.TablesHistoryRepository.AddHistory(MW_DishLibraryEntity.Id, Helpers.Constants.HistoryType.Add.ToString().ToLower(), TablesNames.TLImwDishLibrary.ToString().ToLower());
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            else if (LoadSubType.TLImwODULibrary.ToString() == TableName)
                            {
                                AddMW_ODULibraryViewModel AddMW_ODULibrary = _mapper.Map<AddMW_ODULibraryViewModel>(LoadLibraryViewModel);
                                TLImwODULibrary MW_ODULibraryEntity = _mapper.Map<TLImwODULibrary>(LoadLibraryViewModel);
                                bool test = true;
                                string CheckDependencyValidation = CheckDependencyValidationForMWTypes(LoadLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(AddMW_ODULibrary.TLIdynamicAttLibValue, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                if (test == true)
                                {
                                    var CheckModel = _unitOfWork.MW_ODULibraryRepository.GetWhereFirst(x => x.Model == MW_ODULibraryEntity.Model && !x.Deleted);
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {MW_ODULibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                    //else if (MW_ODULibraryEntity.Depth <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Depth Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_ODULibraryEntity.Width <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Width Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_ODULibraryEntity.Height <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Height Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_ODULibraryEntity.SpaceLibrary <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "SpaceLibrary Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    _unitOfWork.MW_ODULibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, MW_ODULibraryEntity);
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = LoadLibraryViewModel;

                                    AddLogisticalItemWithMW(LogisticalItemIds, MW_ODULibraryEntity, TableNameEntity.Id);

                                    if (AddMW_ODULibrary.TLIdynamicAttLibValue.Count > 0)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(AddMW_ODULibrary.TLIdynamicAttLibValue, TableNameEntity.Id, MW_ODULibraryEntity.Id);
                                    }
                                    _unitOfWork.TablesHistoryRepository.AddHistory(MW_ODULibraryEntity.Id, Helpers.Constants.HistoryType.Add.ToString().ToLower(), "TLImwODULibrary");
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            else if (LoadSubType.TLImwRFULibrary.ToString() == TableName)
                            {
                                AddMW_RFULibraryViewModel MW_RFULibraryViewModel = _mapper.Map<AddMW_RFULibraryViewModel>(LoadLibraryViewModel);
                                TLImwRFULibrary MW_RFULibraryEntity = _mapper.Map<TLImwRFULibrary>(LoadLibraryViewModel);
                                bool test = true;
                                string CheckDependencyValidation = CheckDependencyValidationForMWTypes(LoadLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(MW_RFULibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                if (test == true)
                                {
                                    var CheckModel = _unitOfWork.MW_RFULibraryRepository.GetWhereFirst(x => x.Model == MW_RFULibraryEntity.Model && !x.Deleted);
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {MW_RFULibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                    //else if (MW_RFULibraryEntity.Length <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Length Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_RFULibraryEntity.Width <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Width Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_RFULibraryEntity.Height <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Height Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_RFULibraryEntity.SpaceLibrary <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "SpaceLibrary Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    _unitOfWork.MW_RFULibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, MW_RFULibraryEntity);
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = LoadLibraryViewModel;

                                    AddLogisticalItemWithMW(LogisticalItemIds, MW_RFULibraryEntity, TableNameEntity.Id);

                                    if (MW_RFULibraryViewModel.TLIdynamicAttLibValue.Count > 0)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(MW_RFULibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.Id, MW_RFULibraryEntity.Id);
                                    }
                                    //  _unitOfWork.TablesHistoryRepository.AddHistory(MW_RFULibraryEntity.Id, "Add", "TLImwRFULibrary");
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            else if (LoadSubType.TLImwOtherLibrary.ToString() == TableName)
                            {
                                AddMW_OtherLibraryViewModel MW_OtherLibraryViewModel = _mapper.Map<AddMW_OtherLibraryViewModel>(LoadLibraryViewModel);
                                TLImwOtherLibrary MW_OtherLibraryEntity = _mapper.Map<TLImwOtherLibrary>(LoadLibraryViewModel);
                                bool test = true;
                                string CheckDependencyValidation = CheckDependencyValidationForMWTypes(LoadLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(MW_OtherLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                if (test == true)
                                {
                                    var CheckModel = _unitOfWork.MW_OtherLibraryRepository.GetWhereFirst(x => x.Model == MW_OtherLibraryEntity.Model && !x.Deleted);
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {MW_OtherLibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                    //else if (MW_OtherLibraryEntity.Length <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Length Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_OtherLibraryEntity.Width <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Width Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_OtherLibraryEntity.Height <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Height Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    //else if (MW_OtherLibraryEntity.SpaceLibrary <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "SpaceLibrary Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    _unitOfWork.MW_OtherLibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, MW_OtherLibraryEntity);
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = LoadLibraryViewModel;

                                    AddLogisticalItemWithMW(LogisticalItemIds, MW_OtherLibraryEntity, TableNameEntity.Id);

                                    if (MW_OtherLibraryViewModel.TLIdynamicAttLibValue.Count > 0)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(MW_OtherLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.Id, MW_OtherLibraryEntity.Id);
                                    }
                                    //  _unitOfWork.TablesHistoryRepository.AddHistory(MW_OtherLibraryEntity.Id, Helpers.Constants.HistoryType.Add.ToString().ToLower(), TablesNames.TLImwOtherLibrary.ToString().ToLower());
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.fail);
                                }

                            }
                            transaction.Complete();
                            return new Response<AllItemAttributes>();
                        }
                        catch (Exception err)
                        {
                            return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }
            }
        }
        #region Helper Methods
        public string CheckDependencyValidationForMWTypes(object Input, string MWType)
        {
            if (MWType.ToLower() == TablesNames.TLImwDishLibrary.ToString().ToLower())
            {
                AddMW_DishLibraryViewModel AddMWLibraryViewModel = _mapper.Map<AddMW_DishLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MWType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddMWLibraryViewModel.TLIdynamicAttLibValue
                            .FirstOrDefault(x => x.DynamicAttId == DynamicAttribute.Id);

                        if (InsertedDynamicAttributeValue == null)
                            return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                        List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                        foreach (int RowId in RowsIds)
                        {
                            List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                                , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                            int Succed = 0;

                            foreach (TLIrule Rule in Rules)
                            {
                                string RuleOperation = Rule.Operation.Name;
                                object RuleValue = new object();

                                if (Rule.OperationValueBoolean != null)
                                    RuleValue = Rule.OperationValueBoolean;

                                else if (Rule.OperationValueDateTime != null)
                                    RuleValue = Rule.OperationValueDateTime;

                                else if (Rule.OperationValueDouble != null)
                                    RuleValue = Rule.OperationValueDouble;

                                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                    RuleValue = Rule.OperationValueString;

                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    InsertedValue = AddMWLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddMWLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddMWLibraryViewModel.TLIdynamicAttLibValue
                                        .FirstOrDefault(x => x.DynamicAttId == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    if (DynamicObject.ValueBoolean != null)
                                        InsertedValue = DynamicObject.ValueBoolean;

                                    else if (DynamicObject.ValueDateTime != null)
                                        InsertedValue = DynamicObject.ValueDateTime;

                                    else if (DynamicObject.ValueDouble != null)
                                        InsertedValue = DynamicObject.ValueDouble;

                                    else if (!string.IsNullOrEmpty(DynamicObject.ValueString))
                                        InsertedValue = DynamicObject.ValueString;
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                                {
                                    Succed++;
                                }
                            }
                            if (Rules.Count() == Succed)
                            {
                                string DependencyValidationOperation = Dependency.Operation.Name;

                                object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                    Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                    Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                    !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                                object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.ValueBoolean != null ? InsertedDynamicAttributeValue.ValueBoolean :
                                    InsertedDynamicAttributeValue.ValueDateTime != null ? InsertedDynamicAttributeValue.ValueDateTime :
                                    InsertedDynamicAttributeValue.ValueDouble != null ? InsertedDynamicAttributeValue.ValueDouble :
                                    !string.IsNullOrEmpty(InsertedDynamicAttributeValue.ValueString) ? InsertedDynamicAttributeValue.ValueString : null;

                                if (Dependency.ValueDateTime != null && InsertedDynamicAttributeValue.ValueDateTime != null)
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = new DateTime(InsertedDynamicAttributeValue.ValueDateTime.Value.Year,
                                        InsertedDynamicAttributeValue.ValueDateTime.Value.Month, InsertedDynamicAttributeValue.ValueDateTime.Value.Day);

                                    InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                                }

                                if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                                {
                                    if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                         DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                         DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                         DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                    {
                                        string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                            (DependencyValidationOperation == "!=" ? "not equal to" :
                                            (DependencyValidationOperation == ">" ? "bigger than" :
                                            (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                            (DependencyValidationOperation == "<" ? "smaller than" :
                                            (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                        return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (MWType.ToLower() == TablesNames.TLImwRFULibrary.ToString().ToLower())
            {
                AddMW_RFULibraryViewModel AddMWLibraryViewModel = _mapper.Map<AddMW_RFULibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MWType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddMWLibraryViewModel.TLIdynamicAttLibValue
                            .FirstOrDefault(x => x.DynamicAttId == DynamicAttribute.Id);

                        if (InsertedDynamicAttributeValue == null)
                            return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                        List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                        foreach (int RowId in RowsIds)
                        {
                            List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                                , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                            int Succed = 0;

                            foreach (TLIrule Rule in Rules)
                            {
                                string RuleOperation = Rule.Operation.Name;
                                object RuleValue = new object();

                                if (Rule.OperationValueBoolean != null)
                                    RuleValue = Rule.OperationValueBoolean;

                                else if (Rule.OperationValueDateTime != null)
                                    RuleValue = Rule.OperationValueDateTime;

                                else if (Rule.OperationValueDouble != null)
                                    RuleValue = Rule.OperationValueDouble;

                                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                    RuleValue = Rule.OperationValueString;

                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    InsertedValue = AddMWLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddMWLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddMWLibraryViewModel.TLIdynamicAttLibValue
                                        .FirstOrDefault(x => x.DynamicAttId == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    if (DynamicObject.ValueBoolean != null)
                                        InsertedValue = DynamicObject.ValueBoolean;

                                    else if (DynamicObject.ValueDateTime != null)
                                        InsertedValue = DynamicObject.ValueDateTime;

                                    else if (DynamicObject.ValueDouble != null)
                                        InsertedValue = DynamicObject.ValueDouble;

                                    else if (!string.IsNullOrEmpty(DynamicObject.ValueString))
                                        InsertedValue = DynamicObject.ValueString;
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                                {
                                    Succed++;
                                }
                            }
                            if (Rules.Count() == Succed)
                            {
                                string DependencyValidationOperation = Dependency.Operation.Name;

                                object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                    Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                    Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                    !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                                object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.ValueBoolean != null ? InsertedDynamicAttributeValue.ValueBoolean :
                                    InsertedDynamicAttributeValue.ValueDateTime != null ? InsertedDynamicAttributeValue.ValueDateTime :
                                    InsertedDynamicAttributeValue.ValueDouble != null ? InsertedDynamicAttributeValue.ValueDouble :
                                    !string.IsNullOrEmpty(InsertedDynamicAttributeValue.ValueString) ? InsertedDynamicAttributeValue.ValueString : null;

                                if (Dependency.ValueDateTime != null && InsertedDynamicAttributeValue.ValueDateTime != null)
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = new DateTime(InsertedDynamicAttributeValue.ValueDateTime.Value.Year,
                                        InsertedDynamicAttributeValue.ValueDateTime.Value.Month, InsertedDynamicAttributeValue.ValueDateTime.Value.Day);

                                    InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                                }
                                if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                                {
                                    if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                         DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                         DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                         DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                    {
                                        string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                            (DependencyValidationOperation == "!=" ? "not equal to" :
                                            (DependencyValidationOperation == ">" ? "bigger than" :
                                            (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                            (DependencyValidationOperation == "<" ? "smaller than" :
                                            (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                        return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (MWType.ToLower() == TablesNames.TLImwBULibrary.ToString().ToLower())
            {
                AddMW_BULibraryViewModel AddMWLibraryViewModel = _mapper.Map<AddMW_BULibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MWType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddMWLibraryViewModel.TLIdynamicAttLibValue
                            .FirstOrDefault(x => x.DynamicAttId == DynamicAttribute.Id);

                        if (InsertedDynamicAttributeValue == null)
                            return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                        List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                        foreach (int RowId in RowsIds)
                        {
                            List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                                , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                            int Succed = 0;

                            foreach (TLIrule Rule in Rules)
                            {
                                string RuleOperation = Rule.Operation.Name;
                                object RuleValue = new object();

                                if (Rule.OperationValueBoolean != null)
                                    RuleValue = Rule.OperationValueBoolean;

                                else if (Rule.OperationValueDateTime != null)
                                    RuleValue = Rule.OperationValueDateTime;

                                else if (Rule.OperationValueDouble != null)
                                    RuleValue = Rule.OperationValueDouble;

                                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                    RuleValue = Rule.OperationValueString;

                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    InsertedValue = AddMWLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddMWLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddMWLibraryViewModel.TLIdynamicAttLibValue
                                        .FirstOrDefault(x => x.DynamicAttId == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    if (DynamicObject.ValueBoolean != null)
                                        InsertedValue = DynamicObject.ValueBoolean;

                                    else if (DynamicObject.ValueDateTime != null)
                                        InsertedValue = DynamicObject.ValueDateTime;

                                    else if (DynamicObject.ValueDouble != null)
                                        InsertedValue = DynamicObject.ValueDouble;

                                    else if (!string.IsNullOrEmpty(DynamicObject.ValueString))
                                        InsertedValue = DynamicObject.ValueString;
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                                {
                                    Succed++;
                                }
                            }
                            if (Rules.Count() == Succed)
                            {
                                string DependencyValidationOperation = Dependency.Operation.Name;

                                object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                    Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                    Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                    !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                                object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.ValueBoolean != null ? InsertedDynamicAttributeValue.ValueBoolean :
                                    InsertedDynamicAttributeValue.ValueDateTime != null ? InsertedDynamicAttributeValue.ValueDateTime :
                                    InsertedDynamicAttributeValue.ValueDouble != null ? InsertedDynamicAttributeValue.ValueDouble :
                                    !string.IsNullOrEmpty(InsertedDynamicAttributeValue.ValueString) ? InsertedDynamicAttributeValue.ValueString : null;

                                if (Dependency.ValueDateTime != null && InsertedDynamicAttributeValue.ValueDateTime != null)
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = new DateTime(InsertedDynamicAttributeValue.ValueDateTime.Value.Year,
                                        InsertedDynamicAttributeValue.ValueDateTime.Value.Month, InsertedDynamicAttributeValue.ValueDateTime.Value.Day);

                                    InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                                }

                                if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                                {
                                    if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                         DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                         DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                         DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                    {
                                        string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                            (DependencyValidationOperation == "!=" ? "not equal to" :
                                            (DependencyValidationOperation == ">" ? "bigger than" :
                                            (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                            (DependencyValidationOperation == "<" ? "smaller than" :
                                            (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                        return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (MWType.ToLower() == TablesNames.TLImwODULibrary.ToString().ToLower())
            {
                AddMW_ODULibraryViewModel AddMWLibraryViewModel = _mapper.Map<AddMW_ODULibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MWType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddMWLibraryViewModel.TLIdynamicAttLibValue
                            .FirstOrDefault(x => x.DynamicAttId == DynamicAttribute.Id);

                        if (InsertedDynamicAttributeValue == null)
                            return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                        List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                        foreach (int RowId in RowsIds)
                        {
                            List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                                , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                            int Succed = 0;

                            foreach (TLIrule Rule in Rules)
                            {
                                string RuleOperation = Rule.Operation.Name;
                                object RuleValue = new object();

                                if (Rule.OperationValueBoolean != null)
                                    RuleValue = Rule.OperationValueBoolean;

                                else if (Rule.OperationValueDateTime != null)
                                    RuleValue = Rule.OperationValueDateTime;

                                else if (Rule.OperationValueDouble != null)
                                    RuleValue = Rule.OperationValueDouble;

                                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                    RuleValue = Rule.OperationValueString;

                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    InsertedValue = AddMWLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddMWLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddMWLibraryViewModel.TLIdynamicAttLibValue
                                        .FirstOrDefault(x => x.DynamicAttId == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    if (DynamicObject.ValueBoolean != null)
                                        InsertedValue = DynamicObject.ValueBoolean;

                                    else if (DynamicObject.ValueDateTime != null)
                                        InsertedValue = DynamicObject.ValueDateTime;

                                    else if (DynamicObject.ValueDouble != null)
                                        InsertedValue = DynamicObject.ValueDouble;

                                    else if (!string.IsNullOrEmpty(DynamicObject.ValueString))
                                        InsertedValue = DynamicObject.ValueString;
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                                {
                                    Succed++;
                                }
                            }
                            if (Rules.Count() == Succed)
                            {
                                string DependencyValidationOperation = Dependency.Operation.Name;

                                object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                    Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                    Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                    !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                                object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.ValueBoolean != null ? InsertedDynamicAttributeValue.ValueBoolean :
                                    InsertedDynamicAttributeValue.ValueDateTime != null ? InsertedDynamicAttributeValue.ValueDateTime :
                                    InsertedDynamicAttributeValue.ValueDouble != null ? InsertedDynamicAttributeValue.ValueDouble :
                                    !string.IsNullOrEmpty(InsertedDynamicAttributeValue.ValueString) ? InsertedDynamicAttributeValue.ValueString : null;

                                if (Dependency.ValueDateTime != null && InsertedDynamicAttributeValue.ValueDateTime != null)
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = new DateTime(InsertedDynamicAttributeValue.ValueDateTime.Value.Year,
                                        InsertedDynamicAttributeValue.ValueDateTime.Value.Month, InsertedDynamicAttributeValue.ValueDateTime.Value.Day);

                                    InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                                }

                                if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                                {
                                    if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                         DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                         DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                         DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                    {
                                        string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                            (DependencyValidationOperation == "!=" ? "not equal to" :
                                            (DependencyValidationOperation == ">" ? "bigger than" :
                                            (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                            (DependencyValidationOperation == "<" ? "smaller than" :
                                            (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                        return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (MWType.ToLower() == TablesNames.TLImwOtherLibrary.ToString().ToLower())
            {
                AddMW_OtherLibraryViewModel AddMWLibraryViewModel = _mapper.Map<AddMW_OtherLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MWType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddMWLibraryViewModel.TLIdynamicAttLibValue
                            .FirstOrDefault(x => x.DynamicAttId == DynamicAttribute.Id);

                        if (InsertedDynamicAttributeValue == null)
                            return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                        List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                        foreach (int RowId in RowsIds)
                        {
                            List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                                , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                            int Succed = 0;

                            foreach (TLIrule Rule in Rules)
                            {
                                string RuleOperation = Rule.Operation.Name;
                                object RuleValue = new object();

                                if (Rule.OperationValueBoolean != null)
                                    RuleValue = Rule.OperationValueBoolean;

                                else if (Rule.OperationValueDateTime != null)
                                    RuleValue = Rule.OperationValueDateTime;

                                else if (Rule.OperationValueDouble != null)
                                    RuleValue = Rule.OperationValueDouble;

                                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                    RuleValue = Rule.OperationValueString;

                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    InsertedValue = AddMWLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddMWLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddMWLibraryViewModel.TLIdynamicAttLibValue
                                        .FirstOrDefault(x => x.DynamicAttId == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    if (DynamicObject.ValueBoolean != null)
                                        InsertedValue = DynamicObject.ValueBoolean;

                                    else if (DynamicObject.ValueDateTime != null)
                                        InsertedValue = DynamicObject.ValueDateTime;

                                    else if (DynamicObject.ValueDouble != null)
                                        InsertedValue = DynamicObject.ValueDouble;

                                    else if (!string.IsNullOrEmpty(DynamicObject.ValueString))
                                        InsertedValue = DynamicObject.ValueString;
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                                {
                                    Succed++;
                                }
                            }
                            if (Rules.Count() == Succed)
                            {
                                string DependencyValidationOperation = Dependency.Operation.Name;

                                object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                    Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                    Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                    !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                                object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.ValueBoolean != null ? InsertedDynamicAttributeValue.ValueBoolean :
                                    InsertedDynamicAttributeValue.ValueDateTime != null ? InsertedDynamicAttributeValue.ValueDateTime :
                                    InsertedDynamicAttributeValue.ValueDouble != null ? InsertedDynamicAttributeValue.ValueDouble :
                                    !string.IsNullOrEmpty(InsertedDynamicAttributeValue.ValueString) ? InsertedDynamicAttributeValue.ValueString : null;

                                if (Dependency.ValueDateTime != null && InsertedDynamicAttributeValue.ValueDateTime != null)
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = new DateTime(InsertedDynamicAttributeValue.ValueDateTime.Value.Year,
                                        InsertedDynamicAttributeValue.ValueDateTime.Value.Month, InsertedDynamicAttributeValue.ValueDateTime.Value.Day);

                                    InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                                }

                                if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                                {
                                    if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                         DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                         DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                         DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                    {
                                        string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                            (DependencyValidationOperation == "!=" ? "not equal to" :
                                            (DependencyValidationOperation == ">" ? "bigger than" :
                                            (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                            (DependencyValidationOperation == "<" ? "smaller than" :
                                            (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                        return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
        public string CheckGeneralValidationFunction(List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue, string TableName)
        {
            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable
                    , x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttributeEntity in DynamicAttributes)
            {
                TLIvalidation Validation = _unitOfWork.ValidationRepository
                    .GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttributeEntity.Id, x => x.Operation, x => x.DynamicAtt);

                if (Validation != null)
                {
                    AddDynamicLibAttValueViewModel DynmaicAttributeValue = TLIdynamicAttLibValue.FirstOrDefault(x => x.DynamicAttId == DynamicAttributeEntity.Id);

                    if (DynmaicAttributeValue == null)
                        return $"({Validation.DynamicAtt.Key}) value can't be null and must be inserted";

                    string OperationName = Validation.Operation.Name;

                    object InputDynamicValue = new object();

                    if (DynmaicAttributeValue.ValueBoolean != null)
                        InputDynamicValue = DynmaicAttributeValue.ValueBoolean;

                    else if (DynmaicAttributeValue.ValueDateTime != null)
                        InputDynamicValue = DynmaicAttributeValue.ValueDateTime;

                    else if (DynmaicAttributeValue.ValueDouble != null)
                        InputDynamicValue = DynmaicAttributeValue.ValueDouble;

                    else if (!string.IsNullOrEmpty(DynmaicAttributeValue.ValueString))
                        InputDynamicValue = DynmaicAttributeValue.ValueString;

                    object ValidationValue = new object();

                    if (Validation.ValueBoolean != null)
                        ValidationValue = Validation.ValueBoolean;

                    else if (Validation.ValueDateTime != null)
                        ValidationValue = Validation.ValueDateTime;

                    else if (Validation.ValueDouble != null)
                        ValidationValue = Validation.ValueDouble;

                    else if (!string.IsNullOrEmpty(Validation.ValueString))
                        ValidationValue = Validation.ValueString;

                    if (!(OperationName == "==" ? InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower() :
                        OperationName == "!=" ? InputDynamicValue.ToString().ToLower() != ValidationValue.ToString().ToLower() :
                        OperationName == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 :
                        OperationName == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) :
                        OperationName == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 :
                        OperationName == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) : false))
                    {
                        string DynamicAttributeName = _unitOfWork.DynamicAttRepository
                            .GetWhereFirst(x => x.Id == Validation.DynamicAttId).Key;

                        string ReturnOperation = (OperationName == "==" ? "equal to" :
                            (OperationName == "!=" ? "not equal to" :
                            (OperationName == ">" ? "bigger than" :
                            (OperationName == ">=" ? "bigger than or equal to" :
                            (OperationName == "<" ? "smaller than" :
                            (OperationName == "<=" ? "smaller than or equal to" : ""))))));

                        return $"({DynamicAttributeName}) value must be {ReturnOperation} {ValidationValue}";
                    }
                }
            }

            return string.Empty;
        }
        public void AddLogisticalItemWithMW(dynamic LogisticalItemIds, dynamic MWLibraryEntity, int TableNameEntityId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (LogisticalItemIds.LogisticalItems != null)
                    {
                        if (LogisticalItemIds.LogisticalItems.VendorId != null && LogisticalItemIds.LogisticalItems.VendorId != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.LogisticalItems.VendorId);
                            TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                            {
                                Name = "",
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = MWLibraryEntity.Id,
                                tablesNamesId = TableNameEntityId
                            };
                            _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                            _unitOfWork.SaveChangesAsync();
                        }
                        if (LogisticalItemIds.LogisticalItems.SupplierId != null && LogisticalItemIds.LogisticalItems.SupplierId != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.LogisticalItems.SupplierId);
                            TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                            {
                                Name = "",
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = MWLibraryEntity.Id,
                                tablesNamesId = TableNameEntityId
                            };
                            _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                            _unitOfWork.SaveChangesAsync();
                        }
                        if (LogisticalItemIds.LogisticalItems.DesignerId != null && LogisticalItemIds.LogisticalItems.DesignerId != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.LogisticalItems.DesignerId);
                            TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                            {
                                Name = "",
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = MWLibraryEntity.Id,
                                tablesNamesId = TableNameEntityId
                            };
                            _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                            _unitOfWork.SaveChangesAsync();
                        }
                        if (LogisticalItemIds.LogisticalItems.ManufacturerId != null && LogisticalItemIds.LogisticalItems.ManufacturerId != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.LogisticalItems.ManufacturerId);
                            TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                            {
                                Name = "",
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = MWLibraryEntity.Id,
                                tablesNamesId = TableNameEntityId
                            };
                            _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                            _unitOfWork.SaveChangesAsync();
                        }
                    }

                    transaction.Complete();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #endregion
        //Function take 2 parameters TableName, LoadLibraryViewModel
        //First get table name Entity by TableName
        //Second specify the table i deal with
        //Map from LoadLibraryViewModel object to ViewModel
        //Map from ViewModel To Entity
        //Check validation
        //Update Entity
        //Update dynamic attribute svalues
        public async Task<Response<AllItemAttributes>> EditMWLibrary(string TableName, object LoadLibraryViewModel)
        {
            using (TransactionScope transaction =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    int resultId = 0;
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == TableName);
                    if (LoadSubType.TLImwBULibrary.ToString() == TableName)
                    {
                        EditMW_BULibraryViewModel editMW_BULibraryView = _mapper.Map<EditMW_BULibraryViewModel>(LoadLibraryViewModel);
                        TLImwBULibrary MW_BULibraryEntity = _mapper.Map<TLImwBULibrary>(LoadLibraryViewModel);
                        var MWBU = _unitOfWork.MW_BULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editMW_BULibraryView.Id);
                        MW_BULibraryEntity.Active = MWBU.Active;
                        MW_BULibraryEntity.Deleted = MWBU.Deleted;
                        var CheckModel = _unitOfWork.MW_BULibraryRepository.GetWhereFirst(x => x.Model.ToLower() == MW_BULibraryEntity.Model.ToLower() &&
                            x.Id != MW_BULibraryEntity.Id && !x.Deleted);
                        if (CheckModel != null)
                        {
                            return new Response<AllItemAttributes>(true, null, null, $"This model {MW_BULibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        //_unitOfWork.MW_BULibraryRepository.Update(MW_BULibraryEntity);
                        //if (editMW_BULibraryView.DynamicAtts.Count > 0)
                        //{
                        //    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAtts(editMW_BULibraryView.DynamicAtts, TableNameEntity.Id, MW_BULibraryEntity.Id);

                        //}

                        _unitOfWork.MW_BULibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MWBU, MW_BULibraryEntity);
                        //resultId = _unitOfWork.TablesHistoryRepository.AddHistoryForEdit(MW_BULibraryEntity.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());


                        string CheckDependency = CheckDependencyValidationEditApiVersion(LoadLibraryViewModel, TableName);
                        if (!string.IsNullOrEmpty(CheckDependency))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersion(editMW_BULibraryView.DynamicAtts, TableNameEntity.TableName);
                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        dynamic LogisticalItemIds = new ExpandoObject();
                        LogisticalItemIds = LoadLibraryViewModel;

                        AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                        var CheckVendorId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_BULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckVendorId != null)
                            OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

                        else
                            OldLogisticalItemIds.VendorId = 0;

                        var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_BULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckSupplierId != null)
                            OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

                        else
                            OldLogisticalItemIds.SupplierId = 0;

                        var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_BULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckDesignerId != null)
                            OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

                        else
                            OldLogisticalItemIds.DesignerId = 0;

                        var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_BULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckManufacturerId != null)
                            OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

                        else
                            OldLogisticalItemIds.ManufacturerId = 0;

                        EditLogisticalItem(LogisticalItemIds, MW_BULibraryEntity, TableNameEntity.Id, OldLogisticalItemIds);

                        if (editMW_BULibraryView.DynamicAtts != null ? editMW_BULibraryView.DynamicAtts.Count > 0 : false)
                        {
                            _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editMW_BULibraryView.DynamicAtts, TableNameEntity.Id, MW_BULibraryEntity.Id, Helpers.LogFilterAttribute.UserId, resultId, MWBU.Id);
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (LoadSubType.TLImwDishLibrary.ToString() == TableName)
                    {
                        EditMW_DishLibraryViewModel editMW_Dish = _mapper.Map<EditMW_DishLibraryViewModel>(LoadLibraryViewModel);
                        TLImwDishLibrary MW_DishLibraryEntity = _mapper.Map<TLImwDishLibrary>(LoadLibraryViewModel);
                        var MWDish = _unitOfWork.MW_DishLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editMW_Dish.Id);
                        MW_DishLibraryEntity.Active = MWDish.Active;
                        MW_DishLibraryEntity.Deleted = MWDish.Deleted;
                        var CheckModel = _unitOfWork.MW_DishLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == MW_DishLibraryEntity.Model.ToLower() &&
                            x.Id != MW_DishLibraryEntity.Id && !x.Deleted);
                        if (CheckModel != null)
                        {
                            return new Response<AllItemAttributes>(true, null, null, $"This model {MW_DishLibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        //_unitOfWork.MW_DishLibraryRepository.Update(MW_DishLibraryEntity);
                        //if (editMW_Dish.DynamicAtts.Count > 0)
                        //{
                        //    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAtts(editMW_Dish.DynamicAtts, TableNameEntity.Id, MW_DishLibraryEntity.Id);

                        //}
                        //var testUpdate = _unitOfWork.TablesHistoryRepository.CheckUpdateObject(MWDish, MW_DishLibraryEntity);
                        //if (testUpdate.Details.Count != 0)
                        //{
                        _unitOfWork.MW_DishLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MWDish, MW_DishLibraryEntity);
                        //    resultId = _unitOfWork.TablesHistoryRepository.AddHistoryForEdit(MW_DishLibraryEntity.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
                        //}

                        string CheckDependency = CheckDependencyValidationEditApiVersion(LoadLibraryViewModel, TableName);
                        if (!string.IsNullOrEmpty(CheckDependency))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersion(editMW_Dish.DynamicAtts, TableNameEntity.TableName);
                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        dynamic LogisticalItemIds = new ExpandoObject();
                        LogisticalItemIds = LoadLibraryViewModel;

                        AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                        var CheckVendorId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_DishLibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckVendorId != null)
                            OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

                        else
                            OldLogisticalItemIds.VendorId = 0;

                        var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_DishLibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckSupplierId != null)
                            OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

                        else
                            OldLogisticalItemIds.SupplierId = 0;

                        var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_DishLibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckDesignerId != null)
                            OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

                        else
                            OldLogisticalItemIds.DesignerId = 0;

                        var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_DishLibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckManufacturerId != null)
                            OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

                        else
                            OldLogisticalItemIds.ManufacturerId = 0;

                        EditLogisticalItem(LogisticalItemIds, MW_DishLibraryEntity, TableNameEntity.Id, OldLogisticalItemIds);

                        if (editMW_Dish.DynamicAtts != null ? editMW_Dish.DynamicAtts.Count() > 0 : false)
                        {
                            _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editMW_Dish.DynamicAtts, TableNameEntity.Id, MW_DishLibraryEntity.Id, Helpers.LogFilterAttribute.UserId, resultId, MWDish.Id);

                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (LoadSubType.TLImwODULibrary.ToString() == TableName)
                    {
                        EditMW_ODULibraryViewModel editMW_ODU = _mapper.Map<EditMW_ODULibraryViewModel>(LoadLibraryViewModel);
                        TLImwODULibrary MW_ODULibraryEntity = _mapper.Map<TLImwODULibrary>(LoadLibraryViewModel);
                        var MW_ODU = _unitOfWork.MW_ODULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editMW_ODU.Id);

                        MW_ODULibraryEntity.Active = MW_ODU.Active;
                        MW_ODULibraryEntity.Deleted = MW_ODU.Deleted;
                        var CheckModel = _unitOfWork.MW_ODULibraryRepository.GetWhereFirst(x => x.Model.ToLower() == MW_ODULibraryEntity.Model.ToLower() &&
                            x.Id != MW_ODULibraryEntity.Id && !x.Deleted);
                        if (CheckModel != null)
                        {
                            return new Response<AllItemAttributes>(true, null, null, $"This model {MW_ODULibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        //var testUpdate = _unitOfWork.TablesHistoryRepository.CheckUpdateObject(MW_ODU, MW_ODULibraryEntity);
                        //if (testUpdate.Details.Count != 0)
                        //{
                        _unitOfWork.MW_ODULibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MW_ODU, MW_ODULibraryEntity);
                        //  resultId = _unitOfWork.TablesHistoryRepository.AddHistoryForEdit(MW_ODULibraryEntity.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
                        // }

                        string CheckDependency = CheckDependencyValidationEditApiVersion(LoadLibraryViewModel, TableName);
                        if (!string.IsNullOrEmpty(CheckDependency))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersion(editMW_ODU.DynamicAtts, TableNameEntity.TableName);
                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        dynamic LogisticalItemIds = new ExpandoObject();
                        LogisticalItemIds = LoadLibraryViewModel;

                        AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                        var CheckVendorId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_ODULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckVendorId != null)
                            OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

                        else
                            OldLogisticalItemIds.VendorId = 0;

                        var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_ODULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckSupplierId != null)
                            OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

                        else
                            OldLogisticalItemIds.SupplierId = 0;

                        var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_ODULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckDesignerId != null)
                            OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

                        else
                            OldLogisticalItemIds.DesignerId = 0;

                        var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_ODULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckManufacturerId != null)
                            OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

                        else
                            OldLogisticalItemIds.ManufacturerId = 0;

                        EditLogisticalItem(LogisticalItemIds, MW_ODULibraryEntity, TableNameEntity.Id, OldLogisticalItemIds);

                        if (editMW_ODU.DynamicAtts != null ? editMW_ODU.DynamicAtts.Count > 0 : false)
                        {
                            _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editMW_ODU.DynamicAtts, TableNameEntity.Id, MW_ODULibraryEntity.Id, Helpers.LogFilterAttribute.UserId, resultId, MW_ODU.Id);

                        }
                        //_unitOfWork.MW_ODULibraryRepository.Update(MW_ODULibraryEntity);
                        //if (editMW_ODU.DynamicAtts.Count > 0)
                        //{
                        //    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAtts(editMW_ODU.DynamicAtts, TableNameEntity.Id, MW_ODULibraryEntity.Id);

                        //}
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (LoadSubType.TLImwRFULibrary.ToString() == TableName)
                    {
                        EditMW_RFULibraryViewModel editMW_RFU = _mapper.Map<EditMW_RFULibraryViewModel>(LoadLibraryViewModel);
                        TLImwRFULibrary MW_RFULibraryEntity = _mapper.Map<TLImwRFULibrary>(LoadLibraryViewModel);
                        var MW_RFU = _unitOfWork.MW_RFULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editMW_RFU.Id);

                        MW_RFULibraryEntity.Active = MW_RFU.Active;
                        MW_RFULibraryEntity.Deleted = MW_RFU.Deleted;
                        var CheckModel = _unitOfWork.MW_RFULibraryRepository.GetWhereFirst(x => x.Model.ToLower() == MW_RFULibraryEntity.Model.ToLower() &&
                            x.Id != MW_RFULibraryEntity.Id && !x.Deleted);
                        if (CheckModel != null)
                        {
                            return new Response<AllItemAttributes>(true, null, null, $"This model {MW_RFULibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        //_unitOfWork.MW_RFULibraryRepository.Update(MW_RFULibraryEntity);
                        //if (editMW_RFU.DynamicAtts.Count > 0)
                        //{
                        //    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAtts(editMW_RFU.DynamicAtts, TableNameEntity.Id, MW_RFULibraryEntity.Id);

                        //}
                        _unitOfWork.MW_RFULibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MW_RFU, MW_RFULibraryEntity);


                        string CheckDependency = CheckDependencyValidationEditApiVersion(LoadLibraryViewModel, TableName);
                        if (!string.IsNullOrEmpty(CheckDependency))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersion(editMW_RFU.DynamicAtts, TableNameEntity.TableName);
                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        dynamic LogisticalItemIds = new ExpandoObject();
                        LogisticalItemIds = LoadLibraryViewModel;

                        AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                        var CheckVendorId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_RFULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckVendorId != null)
                            OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

                        else
                            OldLogisticalItemIds.VendorId = 0;

                        var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_RFULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckSupplierId != null)
                            OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

                        else
                            OldLogisticalItemIds.SupplierId = 0;

                        var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_RFULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckDesignerId != null)
                            OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

                        else
                            OldLogisticalItemIds.DesignerId = 0;

                        var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_RFULibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckManufacturerId != null)
                            OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

                        else
                            OldLogisticalItemIds.ManufacturerId = 0;

                        EditLogisticalItem(LogisticalItemIds, MW_RFULibraryEntity, TableNameEntity.Id, OldLogisticalItemIds);

                        if (editMW_RFU.DynamicAtts != null ? editMW_RFU.DynamicAtts.Count > 0 : false)
                        {
                            _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editMW_RFU.DynamicAtts, TableNameEntity.Id, MW_RFULibraryEntity.Id, Helpers.LogFilterAttribute.UserId, resultId, MW_RFU.Id);

                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (LoadSubType.TLImwOtherLibrary.ToString() == TableName)
                    {
                        EditMW_OtherLibraryViewModel editMW_Other = _mapper.Map<EditMW_OtherLibraryViewModel>(LoadLibraryViewModel);
                        TLImwOtherLibrary MW_OtherLibraryEntity = _mapper.Map<TLImwOtherLibrary>(LoadLibraryViewModel);
                        var MWOther = _unitOfWork.MW_OtherLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editMW_Other.Id);

                        MW_OtherLibraryEntity.Active = MWOther.Active;
                        MW_OtherLibraryEntity.Deleted = MWOther.Deleted;
                        var CheckModel = _unitOfWork.MW_OtherLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == MW_OtherLibraryEntity.Model.ToLower() &&
                            x.Id != MW_OtherLibraryEntity.Id && !x.Deleted);
                        if (CheckModel != null)
                        {
                            return new Response<AllItemAttributes>(true, null, null, $"This model {MW_OtherLibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        //_unitOfWork.MW_OtherLibraryRepository.Update(MW_OtherLibraryEntity);
                        //if (editMW_Other.DynamicAtts.Count > 0)
                        //{
                        //    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAtts(editMW_Other.DynamicAtts, TableNameEntity.Id, MW_OtherLibraryEntity.Id);

                        //}
                        //await _unitOfWork.SaveChangesAsync();
                        //var testUpdate = _unitOfWork.TablesHistoryRepository.CheckUpdateObject(MWOther, MW_OtherLibraryEntity);
                        //if (testUpdate.Details.Count != 0)
                        //{
                        _unitOfWork.MW_OtherLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MWOther, MW_OtherLibraryEntity);
                        //  resultId = _unitOfWork.TablesHistoryRepository.AddHistoryForEdit(MW_OtherLibraryEntity.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
                        //}

                        string CheckDependency = CheckDependencyValidationEditApiVersion(LoadLibraryViewModel, TableName);
                        if (!string.IsNullOrEmpty(CheckDependency))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersion(editMW_Other.DynamicAtts, TableNameEntity.TableName);
                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        dynamic LogisticalItemIds = new ExpandoObject();
                        LogisticalItemIds = LoadLibraryViewModel;

                        AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                        var CheckVendorId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_OtherLibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckVendorId != null)
                            OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

                        else
                            OldLogisticalItemIds.VendorId = 0;

                        var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_OtherLibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckSupplierId != null)
                            OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

                        else
                            OldLogisticalItemIds.SupplierId = 0;

                        var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_OtherLibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckDesignerId != null)
                            OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

                        else
                            OldLogisticalItemIds.DesignerId = 0;

                        var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == MW_OtherLibraryEntity.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckManufacturerId != null)
                            OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

                        else
                            OldLogisticalItemIds.ManufacturerId = 0;

                        EditLogisticalItem(LogisticalItemIds, MW_OtherLibraryEntity, TableNameEntity.Id, OldLogisticalItemIds);

                        if (editMW_Other.DynamicAtts != null ? editMW_Other.DynamicAtts.Count > 0 : false)
                        {
                            _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editMW_Other.DynamicAtts, TableNameEntity.Id, MW_OtherLibraryEntity.Id, Helpers.LogFilterAttribute.UserId, resultId, MWOther.Id);

                        }
                    }
                    transaction.Complete();
                    return new Response<AllItemAttributes>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {
                    return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

        }
        #region Helper Methods..
        public void EditLogisticalItem(dynamic LogisticalItemIds, dynamic MainEntity, int TableNameEntityId, dynamic OldLogisticalItemIds)
        {
            using (TransactionScope transaction2 =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    if (LogisticalItemIds.LogisticalItems != null)
                    {
                        if (LogisticalItemIds.LogisticalItems.VendorId != null && LogisticalItemIds.LogisticalItems.VendorId != 0)
                        {
                            if (OldLogisticalItemIds.VendorId != null ? OldLogisticalItemIds.VendorId != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(OldLogisticalItemIds.VendorId);

                                int CivilId = MainEntity.Id;
                                int x = 1;
                                TLIlogisticalitem LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                //int NewLogisiticalId = LogisticalItemIds.LogisticalItems.VendorId;

                                LogisticalItem.logisticalId = LogisticalItemIds.LogisticalItems.VendorId;

                                _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(LogisticalItemIds.LogisticalItems.VendorId);

                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MainEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.LogisticalItems.SupplierId != null && LogisticalItemIds.LogisticalItems.SupplierId != 0)
                        {
                            if (OldLogisticalItemIds.SupplierId != null ? OldLogisticalItemIds.SupplierId != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(OldLogisticalItemIds.SupplierId);

                                int CivilId = MainEntity.Id;

                                TLIlogisticalitem LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                int NewLogisiticalId = LogisticalItemIds.LogisticalItems.SupplierId;

                                LogisticalItem.logisticalId = NewLogisiticalId;

                                _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(LogisticalItemIds.LogisticalItems.SupplierId);

                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MainEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.LogisticalItems.DesignerId != null && LogisticalItemIds.LogisticalItems.DesignerId != 0)
                        {
                            if (OldLogisticalItemIds.DesignerId != null ? OldLogisticalItemIds.DesignerId != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(OldLogisticalItemIds.DesignerId);

                                int CivilId = MainEntity.Id;

                                TLIlogisticalitem LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                int NewLogisiticalId = LogisticalItemIds.LogisticalItems.DesignerId;

                                LogisticalItem.logisticalId = NewLogisiticalId;

                                _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(LogisticalItemIds.LogisticalItems.DesignerId);

                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MainEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.LogisticalItems.ManufacturerId != null && LogisticalItemIds.LogisticalItems.ManufacturerId != 0)
                        {
                            if (OldLogisticalItemIds.ManufacturerId != null ? OldLogisticalItemIds.ManufacturerId != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(OldLogisticalItemIds.ManufacturerId);

                                int CivilId = MainEntity.Id;

                                TLIlogisticalitem LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                int NewLogisiticalId = LogisticalItemIds.LogisticalItems.ManufacturerId;

                                LogisticalItem.logisticalId = NewLogisiticalId;

                                _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(LogisticalItemIds.LogisticalItems.ManufacturerId);

                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MainEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }

                    transaction2.Complete();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public string CheckDependencyValidationEditApiVersion(object Input, string MWType)
        {
            if (MWType.ToLower() == TablesNames.TLImwDishLibrary.ToString().ToLower())
            {
                EditMW_DishLibraryViewModel EditMW_DishLibraryViewModel = _mapper.Map<EditMW_DishLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MWType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditMW_DishLibraryViewModel.DynamicAtts
                            .FirstOrDefault(x => x.Key.ToLower() == DynamicAttribute.Key.ToLower());

                        if (InsertedDynamicAttributeValue == null)
                            return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                        List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                        foreach (int RowId in RowsIds)
                        {
                            List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                                , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                            int Succed = 0;

                            foreach (TLIrule Rule in Rules)
                            {
                                string RuleOperation = Rule.Operation.Name;
                                object RuleValue = new object();

                                if (Rule.OperationValueBoolean != null)
                                    RuleValue = Rule.OperationValueBoolean;

                                else if (Rule.OperationValueDateTime != null)
                                    RuleValue = Rule.OperationValueDateTime;

                                else if (Rule.OperationValueDouble != null)
                                    RuleValue = Rule.OperationValueDouble;

                                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                    RuleValue = Rule.OperationValueString;

                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    InsertedValue = EditMW_DishLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditMW_DishLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditMW_DishLibraryViewModel.DynamicAtts
                                        .FirstOrDefault(x => x.Key.ToLower() == Rule.dynamicAtt.Key.ToLower());

                                    if (DynamicObject == null)
                                        break;

                                    InsertedValue = DynamicObject.Value;
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                                {
                                    Succed++;
                                }
                            }
                            if (Rules.Count() == Succed)
                            {
                                string DependencyValidationOperation = Dependency.Operation.Name;

                                object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                    Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                    Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                    !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                                object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.Value;

                                if (Dependency.ValueDateTime != null)
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = DateTime.Parse(InsertedDynamicAttributeValue.Value.ToString());

                                    InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                                }

                                if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                                {
                                    if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                         DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                         DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                         DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                    {
                                        string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                            (DependencyValidationOperation == "!=" ? "not equal to" :
                                            (DependencyValidationOperation == ">" ? "bigger than" :
                                            (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                            (DependencyValidationOperation == "<" ? "smaller than" :
                                            (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                        return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (MWType.ToLower() == TablesNames.TLImwODULibrary.ToString().ToLower())
            {
                EditMW_ODULibraryViewModel EditMW_ODULibraryViewModel = _mapper.Map<EditMW_ODULibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MWType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditMW_ODULibraryViewModel.DynamicAtts
                            .FirstOrDefault(x => x.Key.ToLower() == DynamicAttribute.Key.ToLower());

                        if (InsertedDynamicAttributeValue == null)
                            return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                        List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                        foreach (int RowId in RowsIds)
                        {
                            List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                                , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                            int Succed = 0;

                            foreach (TLIrule Rule in Rules)
                            {
                                string RuleOperation = Rule.Operation.Name;
                                object RuleValue = new object();

                                if (Rule.OperationValueBoolean != null)
                                    RuleValue = Rule.OperationValueBoolean;

                                else if (Rule.OperationValueDateTime != null)
                                    RuleValue = Rule.OperationValueDateTime;

                                else if (Rule.OperationValueDouble != null)
                                    RuleValue = Rule.OperationValueDouble;

                                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                    RuleValue = Rule.OperationValueString;

                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    InsertedValue = EditMW_ODULibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditMW_ODULibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditMW_ODULibraryViewModel.DynamicAtts
                                        .FirstOrDefault(x => x.Key.ToLower() == Rule.dynamicAtt.Key.ToLower());

                                    if (DynamicObject == null)
                                        break;

                                    InsertedValue = DynamicObject.Value;
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                                {
                                    Succed++;
                                }
                            }
                            if (Rules.Count() == Succed)
                            {
                                string DependencyValidationOperation = Dependency.Operation.Name;

                                object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                    Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                    Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                    !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                                object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.Value;

                                if (Dependency.ValueDateTime != null)
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = DateTime.Parse(InsertedDynamicAttributeValue.Value.ToString());

                                    InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                                }

                                if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                                {
                                    if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                         DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                         DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                         DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                    {
                                        string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                            (DependencyValidationOperation == "!=" ? "not equal to" :
                                            (DependencyValidationOperation == ">" ? "bigger than" :
                                            (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                            (DependencyValidationOperation == "<" ? "smaller than" :
                                            (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                        return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (MWType.ToLower() == TablesNames.TLImwRFULibrary.ToString().ToLower())
            {
                EditMW_RFULibraryViewModel EditMW_RFULibraryViewModel = _mapper.Map<EditMW_RFULibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MWType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditMW_RFULibraryViewModel.DynamicAtts
                            .FirstOrDefault(x => x.Key.ToLower() == DynamicAttribute.Key.ToLower());

                        if (InsertedDynamicAttributeValue == null)
                            return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                        List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                        foreach (int RowId in RowsIds)
                        {
                            List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                                , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                            int Succed = 0;

                            foreach (TLIrule Rule in Rules)
                            {
                                string RuleOperation = Rule.Operation.Name;
                                object RuleValue = new object();

                                if (Rule.OperationValueBoolean != null)
                                    RuleValue = Rule.OperationValueBoolean;

                                else if (Rule.OperationValueDateTime != null)
                                    RuleValue = Rule.OperationValueDateTime;

                                else if (Rule.OperationValueDouble != null)
                                    RuleValue = Rule.OperationValueDouble;

                                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                    RuleValue = Rule.OperationValueString;

                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    InsertedValue = EditMW_RFULibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditMW_RFULibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditMW_RFULibraryViewModel.DynamicAtts
                                        .FirstOrDefault(x => x.Key.ToLower() == Rule.dynamicAtt.Key.ToLower());

                                    if (DynamicObject == null)
                                        break;

                                    InsertedValue = DynamicObject.Value;
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                                {
                                    Succed++;
                                }
                            }
                            if (Rules.Count() == Succed)
                            {
                                string DependencyValidationOperation = Dependency.Operation.Name;

                                object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                    Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                    Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                    !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                                object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.Value;

                                if (Dependency.ValueDateTime != null)
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = DateTime.Parse(InsertedDynamicAttributeValue.Value.ToString());

                                    InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                                }

                                if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                                {
                                    if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                         DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                         DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                         DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                    {
                                        string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                            (DependencyValidationOperation == "!=" ? "not equal to" :
                                            (DependencyValidationOperation == ">" ? "bigger than" :
                                            (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                            (DependencyValidationOperation == "<" ? "smaller than" :
                                            (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                        return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (MWType.ToLower() == TablesNames.TLImwBULibrary.ToString().ToLower())
            {
                EditMW_BULibraryViewModel EditMW_BULibraryViewModel = _mapper.Map<EditMW_BULibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MWType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditMW_BULibraryViewModel.DynamicAtts
                            .FirstOrDefault(x => x.Key.ToLower() == DynamicAttribute.Key.ToLower());

                        if (InsertedDynamicAttributeValue == null)
                            return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                        List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                        foreach (int RowId in RowsIds)
                        {
                            List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                                , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                            int Succed = 0;

                            foreach (TLIrule Rule in Rules)
                            {
                                string RuleOperation = Rule.Operation.Name;
                                object RuleValue = new object();

                                if (Rule.OperationValueBoolean != null)
                                    RuleValue = Rule.OperationValueBoolean;

                                else if (Rule.OperationValueDateTime != null)
                                    RuleValue = Rule.OperationValueDateTime;

                                else if (Rule.OperationValueDouble != null)
                                    RuleValue = Rule.OperationValueDouble;

                                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                    RuleValue = Rule.OperationValueString;

                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    InsertedValue = EditMW_BULibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditMW_BULibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditMW_BULibraryViewModel.DynamicAtts
                                        .FirstOrDefault(x => x.Key.ToLower() == Rule.dynamicAtt.Key.ToLower());

                                    if (DynamicObject == null)
                                        break;

                                    InsertedValue = DynamicObject.Value;
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                                {
                                    Succed++;
                                }
                            }
                            if (Rules.Count() == Succed)
                            {
                                string DependencyValidationOperation = Dependency.Operation.Name;

                                object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                    Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                    Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                    !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                                object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.Value;

                                if (Dependency.ValueDateTime != null)
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = DateTime.Parse(InsertedDynamicAttributeValue.Value.ToString());

                                    InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                                }

                                if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                                {
                                    if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                         DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                         DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                         DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                    {
                                        string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                            (DependencyValidationOperation == "!=" ? "not equal to" :
                                            (DependencyValidationOperation == ">" ? "bigger than" :
                                            (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                            (DependencyValidationOperation == "<" ? "smaller than" :
                                            (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                        return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (MWType.ToLower() == TablesNames.TLImwOtherLibrary.ToString().ToLower())
            {
                EditMW_OtherLibraryViewModel EditMW_OtherLibraryViewModel = _mapper.Map<EditMW_OtherLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MWType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditMW_OtherLibraryViewModel.DynamicAtts
                            .FirstOrDefault(x => x.Key.ToLower() == DynamicAttribute.Key.ToLower());

                        if (InsertedDynamicAttributeValue == null)
                            return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                        List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                        foreach (int RowId in RowsIds)
                        {
                            List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                                , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                            int Succed = 0;

                            foreach (TLIrule Rule in Rules)
                            {
                                string RuleOperation = Rule.Operation.Name;
                                object RuleValue = new object();

                                if (Rule.OperationValueBoolean != null)
                                    RuleValue = Rule.OperationValueBoolean;

                                else if (Rule.OperationValueDateTime != null)
                                    RuleValue = Rule.OperationValueDateTime;

                                else if (Rule.OperationValueDouble != null)
                                    RuleValue = Rule.OperationValueDouble;

                                else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                    RuleValue = Rule.OperationValueString;

                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    InsertedValue = EditMW_OtherLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditMW_OtherLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditMW_OtherLibraryViewModel.DynamicAtts
                                        .FirstOrDefault(x => x.Key.ToLower() == Rule.dynamicAtt.Key.ToLower());

                                    if (DynamicObject == null)
                                        break;

                                    InsertedValue = DynamicObject.Value;
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                                {
                                    Succed++;
                                }
                            }
                            if (Rules.Count() == Succed)
                            {
                                string DependencyValidationOperation = Dependency.Operation.Name;

                                object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                    Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                    Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                    !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                                object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.Value;

                                if (Dependency.ValueDateTime != null)
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = DateTime.Parse(InsertedDynamicAttributeValue.Value.ToString());

                                    InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                                }

                                if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                                {
                                    if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                         DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                         DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                         DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                         DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                             Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                    {
                                        string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                            (DependencyValidationOperation == "!=" ? "not equal to" :
                                            (DependencyValidationOperation == ">" ? "bigger than" :
                                            (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                            (DependencyValidationOperation == "<" ? "smaller than" :
                                            (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                        return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
        public string CheckGeneralValidationFunctionEditApiVersion(List<DynamicAttLibViewModel> TLIdynamicAttLibValue, string TableName)
        {
            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable
                    , x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
            {
                TLIvalidation Validation = _unitOfWork.ValidationRepository
                    .GetIncludeWhereFirst(x => x.DynamicAtt.Key.ToLower() == DynamicAttribute.Key.ToLower(), x => x.Operation, x => x.DynamicAtt);

                if (Validation != null)
                {
                    string OperationName = Validation.Operation.Name;

                    DynamicAttLibViewModel TestValue = TLIdynamicAttLibValue.FirstOrDefault(x => x.Id == DynamicAttribute.Id);

                    if (TestValue == null)
                        return $"({Validation.DynamicAtt.Key}) value can't be null and must be inserted";

                    object InputDynamicValue = TestValue.Value;

                    object ValidationValue = new object();

                    if (Validation.ValueBoolean != null)
                    {
                        ValidationValue = Validation.ValueBoolean;
                        InputDynamicValue = bool.Parse(TestValue.Value.ToString());
                    }

                    else if (Validation.ValueDateTime != null)
                    {
                        ValidationValue = Validation.ValueDateTime;
                        InputDynamicValue = DateTime.Parse(TestValue.Value.ToString());
                    }

                    else if (Validation.ValueDouble != null)
                    {
                        ValidationValue = Validation.ValueDouble;
                        InputDynamicValue = double.Parse(TestValue.Value.ToString());
                    }

                    else if (!string.IsNullOrEmpty(Validation.ValueString))
                    {
                        ValidationValue = Validation.ValueString;
                        InputDynamicValue = TestValue.Value.ToString();
                    }

                    if (!(OperationName == "==" ? InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower() :
                        OperationName == "!=" ? InputDynamicValue.ToString().ToLower() != ValidationValue.ToString().ToLower() :
                        OperationName == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 :
                        OperationName == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) :
                        OperationName == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 :
                        OperationName == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) : false))
                    {
                        string DynamicAttributeName = _unitOfWork.DynamicAttRepository
                            .GetWhereFirst(x => x.Id == Validation.DynamicAttId).Key;

                        string ReturnOperation = (OperationName == "==" ? "equal to" :
                            (OperationName == "!=" ? "not equal to" :
                            (OperationName == ">" ? "bigger than" :
                            (OperationName == ">=" ? "bigger than or equal to" :
                            (OperationName == "<" ? "smaller than" :
                            (OperationName == "<=" ? "smaller than or equal to" : ""))))));

                        return $"({DynamicAttributeName}) value must be {ReturnOperation} {ValidationValue}";
                    }
                }
            }

            return string.Empty;
        }
        #endregion
        //Function take 2 parameters Id, TableName
        //Get table name Entity by TableName
        //specify the table i deal with
        //Get record by Id
        //disable or active record depened on record status
        //Update record 
        public async Task<Response<AllItemAttributes>> Disable(int Id, string TableName)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == TableName);
                    if (LoadSubType.TLImwBULibrary.ToString() == TableName)
                    {
                        TLImwBULibrary OldMW_BULibrary = _unitOfWork.MW_BULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);

                        TLImwBULibrary NewMW_BULibrary = _unitOfWork.MW_BULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewMW_BULibrary.Active = !(NewMW_BULibrary.Active);

                        _unitOfWork.MW_BULibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldMW_BULibrary, NewMW_BULibrary);

                        await _unitOfWork.SaveChangesAsync();

                    }
                    else if (LoadSubType.TLImwDishLibrary.ToString() == TableName)
                    {
                        var MW_DishLibrary = _unitOfWork.MW_DishLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        var NewMW_DishLibrary = _unitOfWork.MW_DishLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewMW_DishLibrary.Active = !(NewMW_DishLibrary.Active);

                        _unitOfWork.MW_DishLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MW_DishLibrary, NewMW_DishLibrary);
                        await _unitOfWork.SaveChangesAsync();

                    }
                    else if (LoadSubType.TLImwODULibrary.ToString() == TableName)
                    {
                        var MW_ODULibrary = _unitOfWork.MW_ODULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        var NewMW_ODULibrary = _unitOfWork.MW_ODULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewMW_ODULibrary.Active = !(NewMW_ODULibrary.Active);
                        _unitOfWork.MW_ODULibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MW_ODULibrary, NewMW_ODULibrary);
                        await _unitOfWork.SaveChangesAsync();

                    }
                    else if (LoadSubType.TLImwRFULibrary.ToString() == TableName)
                    {
                        var MW_RFULibrary = _unitOfWork.MW_RFULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        TLImwRFULibrary NewMw_RFULibrary = _unitOfWork.MW_RFULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewMw_RFULibrary.Active = !(NewMw_RFULibrary.Active);
                        _unitOfWork.MW_RFULibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MW_RFULibrary, NewMw_RFULibrary);
                        await _unitOfWork.SaveChangesAsync();

                    }
                    else if (LoadSubType.TLImwOtherLibrary.ToString() == TableName)
                    {
                        var MW_OtherLibrary = _unitOfWork.MW_OtherLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        var NewMW_OtherLibrary = _unitOfWork.MW_OtherLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewMW_OtherLibrary.Active = !(NewMW_OtherLibrary.Active);
                        _unitOfWork.MW_OtherLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MW_OtherLibrary, NewMW_OtherLibrary);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    transaction.Complete();
                    return new Response<AllItemAttributes>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {

                    return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

        }
        //Function take 1 parameter TableName
        //First get table name Entity by TableName
        //specify the table i deal with
        //get activate attributes depened on TableName
        //get dynamic attributes depened on TableNameId
        public Response<AllItemAttributes> GetForAdd(string TableName)
        {
            try
            {
                AllItemAttributes attributes = new AllItemAttributes();
                var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == TableName);
                if (LoadSubType.TLImwBULibrary.ToString() == TableName)
                {
                    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TablesNames.TLImwBULibrary.ToString(), null, null).ToList();
                    ListAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical("MW"));
                    attributes.AttributesActivated = ListAttributesActivated;
                    attributes.DynamicAtts = _unitOfWork.DynamicAttRepository.GetDynamicLibAtts(TableNameEntity.Id, null);
                    attributes.DynamicAttInst = null;
                }
                else if (LoadSubType.TLImwDishLibrary.ToString() == TableName)
                {
                    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TablesNames.TLImwDishLibrary.ToString(), null, null).ToList();
                    ListAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical("MW"));
                    attributes.AttributesActivated = ListAttributesActivated;
                    attributes.DynamicAtts = _unitOfWork.DynamicAttRepository.GetDynamicLibAtts(TableNameEntity.Id, null);
                    attributes.DynamicAttInst = null;
                }
                else if (LoadSubType.TLImwODULibrary.ToString() == TableName)
                {
                    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TablesNames.TLImwODULibrary.ToString(), null, null).ToList();
                    ListAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical("MW"));
                    attributes.AttributesActivated = ListAttributesActivated;
                    attributes.DynamicAtts = _unitOfWork.DynamicAttRepository.GetDynamicLibAtts(TableNameEntity.Id, null);
                    attributes.DynamicAttInst = null;
                }
                else if (LoadSubType.TLImwRFULibrary.ToString() == TableName)
                {
                    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TablesNames.TLImwRFULibrary.ToString(), null, null).ToList();
                    ListAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical("MW"));
                    attributes.AttributesActivated = ListAttributesActivated;
                    attributes.DynamicAtts = _unitOfWork.DynamicAttRepository.GetDynamicLibAtts(TableNameEntity.Id, null);
                    attributes.DynamicAttInst = null;
                }
                else if (LoadSubType.TLImwOtherLibrary.ToString() == TableName)
                {
                    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(LoadSubType.TLImwOtherLibrary.ToString(), null, null).ToList();
                    ListAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical("MW"));
                    attributes.AttributesActivated = ListAttributesActivated;
                    attributes.DynamicAtts = _unitOfWork.DynamicAttRepository.GetDynamicLibAtts(TableNameEntity.Id, null);
                    attributes.DynamicAttInst = null;
                }
                return new Response<AllItemAttributes>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters Id, TableName
        //First get table name Entity
        //specify the table i deal with
        //get record by Id
        //set Deleted to true
        //update record
        public async Task<Response<AllItemAttributes>> Delete(int Id, string TableName)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == TableName);
                    if (LoadSubType.TLImwBULibrary.ToString() == TableName)
                    {
                        TLImwBULibrary OldMW_BULibrary = _unitOfWork.MW_BULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);

                        TLImwBULibrary NewMW_BULibrary = _unitOfWork.MW_BULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);

                        NewMW_BULibrary.Deleted = true;
                        NewMW_BULibrary.Model = NewMW_BULibrary.Model + "_" + DateTime.Now.ToString();

                        _unitOfWork.MW_BULibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldMW_BULibrary, NewMW_BULibrary);
                        _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                        //AddHistory(MW_BULibrary.Id, Helpers.Constants.HistoryType.Delete.ToString(), TablesNames.TLImwBULibrary.ToString());
                    }
                    else if (LoadSubType.TLImwDishLibrary.ToString() == TableName)
                    {
                        var MW_DishLibrary = _unitOfWork.MW_DishLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        var NewMW_DishLibrary = _unitOfWork.MW_DishLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewMW_DishLibrary.Deleted = true;
                        NewMW_DishLibrary.Model = NewMW_DishLibrary.Model + "_" + DateTime.Now.ToString();

                        _unitOfWork.MW_DishLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MW_DishLibrary, NewMW_DishLibrary);
                        _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (LoadSubType.TLImwODULibrary.ToString() == TableName)
                    {
                        var MW_ODULibrary = _unitOfWork.MW_ODULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        var NewMW_ODULibrary = _unitOfWork.MW_ODULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewMW_ODULibrary.Deleted = true;
                        NewMW_ODULibrary.Model = NewMW_ODULibrary.Model + "_" + DateTime.Now.ToString();

                        _unitOfWork.MW_ODULibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MW_ODULibrary, NewMW_ODULibrary);
                        _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (LoadSubType.TLImwRFULibrary.ToString() == TableName)
                    {
                        var MW_RFULibrary = _unitOfWork.MW_RFULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        TLImwRFULibrary NewMW_RFULibrary = _unitOfWork.MW_RFULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewMW_RFULibrary.Deleted = true;
                        NewMW_RFULibrary.Model = NewMW_RFULibrary.Model + "_" + DateTime.Now.ToString();

                        _unitOfWork.MW_RFULibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MW_RFULibrary, NewMW_RFULibrary);
                        _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (LoadSubType.TLImwOtherLibrary.ToString() == TableName)
                    {
                        var MW_OtherLibrary = _unitOfWork.MW_OtherLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        var NewMW_OtherLibrary = _unitOfWork.MW_OtherLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewMW_OtherLibrary.Deleted = true;
                        NewMW_OtherLibrary.Model = NewMW_OtherLibrary.Model + "_" + DateTime.Now.ToString();

                        _unitOfWork.MW_OtherLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, MW_OtherLibrary, NewMW_OtherLibrary);
                        _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    transaction.Complete();
                    return new Response<AllItemAttributes>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {

                    return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        //#region Add History
        //public void AddHistory(int MW_lib_id, string historyType, string TableName)
        //{

        //    AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
        //    history.RecordId = MW_lib_id;
        //    history.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableName).Id;
        //    history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
        //    history.UserId = 261;
        //    _unitOfWork.TablesHistoryRepository.AddTableHistory(history);

        //}
        //#endregion
        //#region AddHistoryForEdit
        //public int AddHistoryForEdit(int RecordId, int TableNameid, string HistoryType, List<TLIhistoryDetails> details)
        //{
        //    AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
        //    history.RecordId = RecordId;
        //    history.TablesNameId = TableNameid;//_unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.Id == TableNameid).Id;
        //    history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == HistoryType, x => new { x.Id }).Id;
        //    history.UserId = 83;
        //    int? TableHistoryId = null;
        //    var CheckTableHistory = _unitOfWork.TablesHistoryRepository.GetWhereFirst(x => x.HistoryType.Name == HistoryType && x.RecordId == RecordId && x.TablesNameId == TableNameid);
        //    if (CheckTableHistory)
        //    {
        //        var TableHistory = _unitOfWork.TablesHistoryRepository.GetWhereAndSelect(x => x.HistoryType.Name == HistoryType && x.RecordId == RecordId && x.TablesNameId == TableNameid, x => new { x.Id }).ToList().Max(x => x.Id);
        //        if (TableHistory != null)
        //            TableHistoryId = TableHistory;
        //        if (TableHistoryId != null)
        //        {
        //            history.PreviousHistoryId = TableHistoryId;
        //        }
        //    }

        //    int HistoryId = _unitOfWork.TablesHistoryRepository.AddTableHistory(history, details);
        //    _unitOfWork.SaveChangesAsync();
        //    return HistoryId;
        //}

        //#endregion
        #region test
        public EditHistoryDetails CheckUpdateObject(object originalObj, object updateObj)
        {
            EditHistoryDetails result = new EditHistoryDetails();
            result.original = originalObj;
            result.Details = new List<TLIhistoryDetails>();
            foreach (var property in updateObj.GetType().GetProperties())
            {

                var x = property.GetValue(updateObj);
                var y = property.GetValue(originalObj);
                if (x != null && y != null)
                {
                    if (!x.Equals(y))
                    {
                        property.SetValue(result.original, x);
                        TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                        // historyDetails.AttType = "static";
                        historyDetails.AttName = property.Name;
                        historyDetails.OldValue = y.ToString();
                        historyDetails.NewValue = x.ToString();
                        result.Details.Add(historyDetails);

                    }
                }
            }
            return result;
        }
        #endregion
    }
}
