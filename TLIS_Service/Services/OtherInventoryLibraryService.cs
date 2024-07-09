using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using System.Collections;
using System.Globalization;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;
using Microsoft.EntityFrameworkCore;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using AutoMapper;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.AsTypeDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.PolarityTypeDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.MW_ODULibraryDTOs;
using TLIS_DAL.ViewModels.ParityDTOs;
using TLIS_DAL;
using System.Reflection.Emit;

namespace TLIS_Service.Services
{
    public class OtherInventoryLibraryService : IOtherInventoryLibraryService
    {
        private readonly IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        ApplicationDbContext db;
        public OtherInventoryLibraryService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper, ApplicationDbContext _context)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
            db = _context;
        }
        public async Task RadioRRULibrarySeedDataForTest()
        {
            try
            {
                List<TLIradioRRULibrary> SeedData = new List<TLIradioRRULibrary>
                {
                    new TLIradioRRULibrary
                    {
                        Id = 1,
                        Model = "RadioRRULibrary1",
                        Type = "1",
                        Band = "1",
                        ChannelBandwidth = 1,
                        Weight = 1,
                        L_W_H_cm3 = "1",
                        Length = 1,
                        Width = 1,
                        Height = 1,
                        Notes = "1",
                        SpaceLibrary = 1,
                        Active = true,
                        Deleted = false
                    },
                    new TLIradioRRULibrary
                    {
                        Id = 2,
                        Model = "RadioRRULibrary2",
                        Type = "2",
                        Band = "2",
                        ChannelBandwidth = 2,
                        Weight = 2,
                        L_W_H_cm3 = "2",
                        Length = 2,
                        Width = 2,
                        Height = 2,
                        Notes = "2",
                        SpaceLibrary = 2,
                        Active = true,
                        Deleted = false
                    },
                    new TLIradioRRULibrary
                    {
                        Id = 3,
                        Model = "RadioRRULibrary3",
                        Type = "3",
                        Band = "3",
                        ChannelBandwidth = 3,
                        Weight = 3,
                        L_W_H_cm3 = "3",
                        Length = 3,
                        Width = 3,
                        Height = 3,
                        Notes = "3",
                        SpaceLibrary = 3,
                        Active = true,
                        Deleted = false
                    },
                    new TLIradioRRULibrary
                    {
                        Id = 4,
                        Model = "RadioRRULibrary4",
                        Type = "4",
                        Band = "4",
                        ChannelBandwidth = 4,
                        Weight = 4,
                        L_W_H_cm3 = "4",
                        Length = 4,
                        Width = 4,
                        Height = 4,
                        Notes = "4",
                        SpaceLibrary = 4,
                        Active = true,
                        Deleted = false
                    },
                    new TLIradioRRULibrary
                    {
                        Id = 5,
                        Model = "RadioRRULibrary5",
                        Type = "5",
                        Band = "5",
                        ChannelBandwidth = 5,
                        Weight = 5,
                        L_W_H_cm3 = "5",
                        Length = 5,
                        Width = 5,
                        Height = 5,
                        Notes = "5",
                        SpaceLibrary = 5,
                        Active = true,
                        Deleted = false
                    }
                };
                await _unitOfWork.RadioRRULibraryRepository.AddRangeAsync(SeedData);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task GeneratorLibrarySeedDataForTest()
        {
            try
            {
                List<TLIcapacity> SeedDataForCapacity = new List<TLIcapacity>
                {
                    new TLIcapacity {
                        Id = 1,
                        Name = "Capacity1"
                    },
                    new TLIcapacity {
                        Id = 2,
                        Name = "Capacity2"
                    },
                    new TLIcapacity {
                        Id = 3,
                        Name = "Capacity3"
                    },
                };
                await _unitOfWork.CapacityRepository.AddRangeAsync(SeedDataForCapacity);
                await _unitOfWork.SaveChangesAsync();

                List<TLIgeneratorLibrary> SeedData = new List<TLIgeneratorLibrary>
                {
                    new TLIgeneratorLibrary
                    {
                        Id = 1,
                        Model = "GeneratorLibrary1",
                        Width = 1,
                        Weight = 1,
                        Length = 1,
                        LayoutCode = "1",
                        Height = 1,
                        SpaceLibrary = 1,
                        CapacityId = 1,
                        Active = true,
                        Deleted = false
                    },
                    new TLIgeneratorLibrary
                    {
                        Id = 2,
                        Model = "GeneratorLibrary2",
                        Width = 2,
                        Weight = 2,
                        Length = 2,
                        LayoutCode = "2",
                        Height = 2,
                        SpaceLibrary = 2,
                        CapacityId = 1,
                        Active = true,
                        Deleted = false
                    },
                    new TLIgeneratorLibrary
                    {
                        Id = 3,
                        Model = "GeneratorLibrary3",
                        Width = 3,
                        Weight = 3,
                        Length = 3,
                        LayoutCode = "3",
                        Height = 3,
                        SpaceLibrary = 3,
                        CapacityId = 2,
                        Active = true,
                        Deleted = false
                    },
                    new TLIgeneratorLibrary
                    {
                        Id = 4,
                        Model = "GeneratorLibrary4",
                        Width = 4,
                        Weight = 4,
                        Length = 4,
                        LayoutCode = "4",
                        Height = 4,
                        SpaceLibrary = 4,
                        CapacityId = 2,
                        Active = true,
                        Deleted = false
                    },
                    new TLIgeneratorLibrary
                    {
                        Id = 5,
                        Model = "GeneratorLibrary5",
                        Width = 5,
                        Weight = 5,
                        Length = 5,
                        LayoutCode = "5",
                        Height = 5,
                        SpaceLibrary = 5,
                        CapacityId = 3,
                        Active = true,
                        Deleted = false
                    }
                };
                await _unitOfWork.GeneratorLibraryRepository.AddRangeAsync(SeedData);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task SolarLibrarySeedDataForTest()
        {
            try
            {
                List<TLIsolarLibrary> SeedData = new List<TLIsolarLibrary>
                {
                    new TLIsolarLibrary
                    {
                        Id = 1,
                        Model = "SolarLibrary1",
                        Weight = 1,
                        TotaPanelsDimensions = "1",
                        StructureDesign = "1",
                        LayoutCode = "1",
                        HeightFromFront = 1,
                        HeightFromBack = 1,
                        BasePlateDimension = "1",
                        SpaceLibrary = 1,
                        CapacityId = 1,
                        Active = true,
                        Deleted = false
                    },
                    new TLIsolarLibrary
                    {
                        Id = 2,
                        Model = "SolarLibrary2",
                        Weight = 2,
                        TotaPanelsDimensions = "2",
                        StructureDesign = "2",
                        LayoutCode = "2",
                        HeightFromFront = 2,
                        HeightFromBack = 2,
                        BasePlateDimension = "2",
                        SpaceLibrary = 2,
                        CapacityId = 1,
                        Active = true,
                        Deleted = false
                    },
                    new TLIsolarLibrary
                    {
                        Id = 3,
                        Model = "SolarLibrary3",
                        Weight = 3,
                        TotaPanelsDimensions = "3",
                        StructureDesign = "3",
                        LayoutCode = "3",
                        HeightFromFront = 3,
                        HeightFromBack = 3,
                        BasePlateDimension = "3",
                        SpaceLibrary = 3,
                        CapacityId = 2,
                        Active = true,
                        Deleted = false
                    },
                    new TLIsolarLibrary
                    {
                        Id = 4,
                        Model = "SolarLibrary4",
                        Weight = 4,
                        TotaPanelsDimensions = "4",
                        StructureDesign = "4",
                        LayoutCode = "4",
                        HeightFromFront = 4,
                        HeightFromBack = 4,
                        BasePlateDimension = "4",
                        SpaceLibrary = 4,
                        CapacityId = 2,
                        Active = true,
                        Deleted = false
                    },
                    new TLIsolarLibrary
                    {
                        Id = 5,
                        Model = "SolarLibrary5",
                        Weight = 5,
                        TotaPanelsDimensions = "5",
                        StructureDesign = "5",
                        LayoutCode = "5",
                        HeightFromFront = 5,
                        HeightFromBack = 5,
                        BasePlateDimension = "5",
                        SpaceLibrary = 5,
                        CapacityId = 3,
                        Active = true,
                        Deleted = false
                    }
                };
                await _unitOfWork.SolarLibraryRepository.AddRangeAsync(SeedData);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Function take 3 parameters TableName, OtherInventoryLibraryViewModel
        //First get table name Entity 
        //specify the table i deal with
        //Map OtherInventoryLibraryViewModel object to ViewModel
        //Map to Entity
        //check validation
        //Add Entity
        //Add dynamic attribute values
        public Response<AllItemAttributes> AddOtherInventoryLibrary(string TableName, object OtherInventoryLibraryViewModel, string connectionString)
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
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(o => o.TableName == TableName);
                            if (OtherInventoryType.TLIcabinetPowerLibrary.ToString().ToLower() == TableName.ToLower())
                            {
                                AddCabinetPowerLibraryViewModel cabinetPowerLibraryViewModel = _mapper.Map<AddCabinetPowerLibraryViewModel>(OtherInventoryLibraryViewModel);
                                TLIcabinetPowerLibrary cabinetPowerLibrary = _mapper.Map<TLIcabinetPowerLibrary>(cabinetPowerLibraryViewModel);
                                bool test = false;
                                string CheckDependencyValidation = CheckDependencyValidationForOtherInventoryTypes(OtherInventoryLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(cabinetPowerLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                test = true;

                                if (test == true)
                                {
                                    var CheckModel = _unitOfWork.CabinetPowerLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == cabinetPowerLibrary.Model.ToLower()
                                        && !x.Deleted);
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {cabinetPowerLibrary.Model} is already exists", (int)ApiReturnCode.fail);
                                    }
                                    _unitOfWork.CabinetPowerLibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, cabinetPowerLibrary);
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = OtherInventoryLibraryViewModel;

                                    AddLogisticalItemWithOtherInventory(LogisticalItemIds, cabinetPowerLibrary, TableNameEntity.Id);

                                    if (cabinetPowerLibraryViewModel.TLIdynamicAttLibValue.Count > 0)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(cabinetPowerLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.Id, cabinetPowerLibrary.Id);
                                    }
                                    //AddHistory(cabinetPowerLibrary.Id, "Add", "TLIcabinetPowerLibrary");
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
                                }
                            }
                            else if (OtherInventoryType.TLIcabinetTelecomLibrary.ToString().ToLower() == TableName.ToLower())
                            {
                                AddCabinetTelecomLibraryViewModel cabinetTelecomLibraryViewModel = _mapper.Map<AddCabinetTelecomLibraryViewModel>(OtherInventoryLibraryViewModel);

                                TLIcabinetTelecomLibrary cabinetTelecomLibrary = _mapper.Map<TLIcabinetTelecomLibrary>(cabinetTelecomLibraryViewModel);
                                if (cabinetTelecomLibrary.Dimension_W_D_H == null || cabinetTelecomLibrary.Dimension_W_D_H == "")
                                {
                                    cabinetTelecomLibrary.Dimension_W_D_H = cabinetTelecomLibrary.Width + "_" + cabinetTelecomLibrary.Depth + "_" + cabinetTelecomLibrary.Height;
                                }
                                bool test = false;
                                string CheckDependencyValidation = CheckDependencyValidationForOtherInventoryTypes(OtherInventoryLibraryViewModel, TableName);


                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(cabinetTelecomLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                test = true;

                                if (test == true)
                                {
                                    var CheckModel = _unitOfWork.CabinetTelecomLibraryRepository.GetWhereFirst(x => x.Model == cabinetTelecomLibrary.Model && !x.Deleted);
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {cabinetTelecomLibrary.Model} is already exists", (int)ApiReturnCode.fail);
                                    }
                                    _unitOfWork.CabinetTelecomLibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, cabinetTelecomLibrary);
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = OtherInventoryLibraryViewModel;

                                    AddLogisticalItemWithOtherInventory(LogisticalItemIds, cabinetTelecomLibrary, TableNameEntity.Id);

                                    if (cabinetTelecomLibraryViewModel.TLIdynamicAttLibValue.Count > 0)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(cabinetTelecomLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.Id, cabinetTelecomLibrary.Id);
                                    }
                                    //  AddHistory(cabinetTelecomLibrary.Id, "Add", "TLIcabinetTelecomLibrary");
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
                                }
                            }
                            else if (OtherInventoryType.TLIgeneratorLibrary.ToString().ToLower() == TableName.ToLower())
                            {
                                AddGeneratorLibraryViewModel generatorLibraryViewModel = _mapper.Map<AddGeneratorLibraryViewModel>(OtherInventoryLibraryViewModel);
                                TLIgeneratorLibrary generatorLibrary = _mapper.Map<TLIgeneratorLibrary>(generatorLibraryViewModel);
                                bool test = false;
                                string CheckDependencyValidation = CheckDependencyValidationForOtherInventoryTypes(OtherInventoryLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(generatorLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                test = true;

                                if (test == true)
                                {
                                    var CheckModel = _unitOfWork.GeneratorLibraryRepository.GetWhereFirst(x => x.Model == generatorLibrary.Model && !x.Deleted);
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {generatorLibrary.Model} is already exists", (int)ApiReturnCode.fail);
                                    }
                                    _unitOfWork.GeneratorLibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, generatorLibrary);
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = OtherInventoryLibraryViewModel;

                                    AddLogisticalItemWithOtherInventory(LogisticalItemIds, generatorLibrary, TableNameEntity.Id);

                                    if (generatorLibraryViewModel.TLIdynamicAttLibValue.Count > 0)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(generatorLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.Id, generatorLibrary.Id);
                                    }
                                    //   AddHistory(generatorLibrary.Id, "Add", "TLIgeneratorLibrary");
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
                                }
                            }
                            else if (OtherInventoryType.TLIsolarLibrary.ToString().ToLower() == TableName.ToLower())
                            {
                                AddSolarLibraryViewModel solarLibraryViewModel = _mapper.Map<AddSolarLibraryViewModel>(OtherInventoryLibraryViewModel);
                                TLIsolarLibrary solarLibrary = _mapper.Map<TLIsolarLibrary>(solarLibraryViewModel);
                                bool test = false;
                                string CheckDependencyValidation = CheckDependencyValidationForOtherInventoryTypes(OtherInventoryLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(solarLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                test = true;

                                if (test == true)
                                {
                                    var CheckModel = _unitOfWork.SolarLibraryRepository.GetWhereFirst(x => x.Model == solarLibrary.Model && !x.Deleted);
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {solarLibrary.Model} is already exists", (int)ApiReturnCode.fail);
                                    }
                                    _unitOfWork.SolarLibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, solarLibrary);
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = OtherInventoryLibraryViewModel;

                                    AddLogisticalItemWithOtherInventory(LogisticalItemIds, solarLibrary, TableNameEntity.Id);

                                    if (solarLibraryViewModel.TLIdynamicAttLibValue.Count > 0)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(solarLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.Id, solarLibrary.Id);
                                    }
                                    //AddHistory(solarLibrary.Id, "Add", "TLIsolarLibrary");
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
                                }
                            }
                            transaction.Complete();
                            tran.Commit();
                            return new Response<AllItemAttributes>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<AllItemAttributes>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                        }
                    }
                }
            }

        }
        #region Helper Methods
        public string CheckDependencyValidationForOtherInventoryTypes(object Input, string OtherInventoryType)
        {
            if (OtherInventoryType.ToLower() == TablesNames.TLIcabinetTelecomLibrary.ToString().ToLower())
            {
                AddCabinetTelecomLibraryViewModel AddCabinetTelecomLibraryViewModel = _mapper.Map<AddCabinetTelecomLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == OtherInventoryType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddCabinetTelecomLibraryViewModel.TLIdynamicAttLibValue
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

                                    InsertedValue = AddCabinetTelecomLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddCabinetTelecomLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddCabinetTelecomLibraryViewModel.TLIdynamicAttLibValue
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
            else if (OtherInventoryType.ToLower() == TablesNames.TLIcabinetPowerLibrary.ToString().ToLower())
            {
                AddCabinetPowerLibraryViewModel AddCabinetTelecomLibraryViewModel = _mapper.Map<AddCabinetPowerLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == OtherInventoryType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddCabinetTelecomLibraryViewModel.TLIdynamicAttLibValue
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

                                    InsertedValue = AddCabinetTelecomLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddCabinetTelecomLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddCabinetTelecomLibraryViewModel.TLIdynamicAttLibValue
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
            else if (OtherInventoryType.ToLower() == TablesNames.TLIgeneratorLibrary.ToString().ToLower())
            {
                AddGeneratorLibraryViewModel AddGeneratorLibraryViewModel = _mapper.Map<AddGeneratorLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == OtherInventoryType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddGeneratorLibraryViewModel.TLIdynamicAttLibValue
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

                                    InsertedValue = AddGeneratorLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddGeneratorLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddGeneratorLibraryViewModel.TLIdynamicAttLibValue
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
            else if (OtherInventoryType.ToLower() == TablesNames.TLIsolarLibrary.ToString().ToLower())
            {
                AddSolarLibraryViewModel AddSolarLibraryViewModel = _mapper.Map<AddSolarLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == OtherInventoryType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddSolarLibraryViewModel.TLIdynamicAttLibValue
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

                                    InsertedValue = AddSolarLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddSolarLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddSolarLibraryViewModel.TLIdynamicAttLibValue
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
        public void AddLogisticalItemWithOtherInventory(dynamic LogisticalItemIds, dynamic OtherInventoryEntity, int TableNameEntityId)
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
                                Name = LogisticalObject.Name,
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = OtherInventoryEntity.Id,
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
                                Name = LogisticalObject.Name,
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = OtherInventoryEntity.Id,
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
                                Name = LogisticalObject.Name,
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = OtherInventoryEntity.Id,
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
                                Name = LogisticalObject.Name,
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = OtherInventoryEntity.Id,
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
        //Function take 2 parameters Id, TableName
        //get table name Entity by TableName
        //specify the table i deal with
        //get record by Id
        //disable or active record depened on record status
        //update Entity
        public async Task<Response<AllItemAttributes>> Disable(int Id, string TableName, string ConnectionString)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(o => o.TableName == TableName);
                    if (OtherInventoryType.TLIcabinetPowerLibrary.ToString() == TableName)
                    {
                        var UseCabinetPower = _unitOfWork.CabinetRepository.GetWhere(x => x.CabinetPowerLibraryId == Id).ToList();
                        if (UseCabinetPower != null && UseCabinetPower.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not change status this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);


                        var CabinetPowerLibrary = _unitOfWork.CabinetPowerLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        var NewCabinetPowerLibrary = _unitOfWork.CabinetPowerLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewCabinetPowerLibrary.Active = !(NewCabinetPowerLibrary.Active);
                        _unitOfWork.CabinetPowerLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, CabinetPowerLibrary, NewCabinetPowerLibrary);
                        //DisableDynamicAttLibValues(OtherInventoryTypeId.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (OtherInventoryType.TLIcabinetTelecomLibrary.ToString() == TableName)
                    {
                        var UseCabinetPower = _unitOfWork.CabinetRepository.GetWhere(x => x.CabinetTelecomLibraryId == Id).ToList();
                        if (UseCabinetPower != null && UseCabinetPower.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not change status this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                        var CabinetTelecomLibrary = _unitOfWork.CabinetTelecomLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        TLIcabinetTelecomLibrary NewCabinetTelecomLibrary = _unitOfWork.CabinetTelecomLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewCabinetTelecomLibrary.Active = !(NewCabinetTelecomLibrary.Active);
                        _unitOfWork.CabinetTelecomLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, CabinetTelecomLibrary, NewCabinetTelecomLibrary);
                        //DisableDynamicAttLibValues(OtherInventoryTypeId.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (OtherInventoryType.TLIgeneratorLibrary.ToString() == TableName)
                    {
                        var UseCabinetPower = _unitOfWork.GeneratorRepository.GetWhere(x => x.GeneratorLibraryId == Id).ToList();
                        if (UseCabinetPower != null && UseCabinetPower.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not change status this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                        TLIgeneratorLibrary OldGeneratorLibrary = _unitOfWork.GeneratorLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);

                        TLIgeneratorLibrary NewGeneratorLibrary = _unitOfWork.GeneratorLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);

                        NewGeneratorLibrary.Active = !(NewGeneratorLibrary.Active);
                        _unitOfWork.GeneratorLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldGeneratorLibrary, NewGeneratorLibrary);
                        //DisableDynamicAttLibValues(OtherInventoryTypeId.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (OtherInventoryType.TLIsolarLibrary.ToString() == TableName)
                    {
                        var UseCabinetPower = _unitOfWork.SolarRepository.GetWhere(x => x.SolarLibraryId == Id).ToList();
                        if (UseCabinetPower != null && UseCabinetPower.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not change status this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                        var SolarLibrary = _unitOfWork.SolarLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        TLIsolarLibrary NewSolarLibrary = _unitOfWork.SolarLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewSolarLibrary.Active = !(NewSolarLibrary.Active);
                        _unitOfWork.SolarLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, SolarLibrary, NewSolarLibrary);
                        //DisableDynamicAttLibValues(OtherInventoryTypeId.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    transaction.Complete();
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(ConnectionString));
                    return new Response<AllItemAttributes>();
                }
                catch (Exception err)
                {

                    return new Response<AllItemAttributes>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                }
            }
        }
        //Function take 2 parameters OtherInventoryLibraryViewModel, TableName
        //get table name Entity depened on TableName
        //specify the table i deal with
        //Map OtherInventoryLibraryViewModel object to ViewModel
        //Map to Entity
        //check validation
        //update Entity
        //Update dynamic attributes
        //public async Task<Response<AllItemAttributes>> EditOtherInventoryLibrary(object OtherInventoryLibraryViewModel, string TableName)
        //{
        //    using (TransactionScope transaction =
        //        new TransactionScope(TransactionScopeOption.Required,
        //                           new System.TimeSpan(0, 15, 0)))
        //    {
        //        try
        //        {
        //            int resultId = 0;
        //            TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(o => o.TableName.ToLower() == TableName.ToLower());
        //            if (OtherInventoryType.TLIcabinetPowerLibrary.ToString().ToLower() == TableName.ToLower())
        //            {
        //                EditCabinetPowerLibraryViewModel CabinetPowerLibraryViewModel = _mapper.Map<EditCabinetPowerLibraryViewModel>(OtherInventoryLibraryViewModel);
        //                TLIcabinetPowerLibrary CabinetPowerLibraryEntity = _mapper.Map<TLIcabinetPowerLibrary>(OtherInventoryLibraryViewModel);
        //                var Cabinet = _unitOfWork.CabinetPowerLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == CabinetPowerLibraryViewModel.Id);

        //                CabinetPowerLibraryEntity.Active = Cabinet.Active;
        //                CabinetPowerLibraryEntity.Deleted = Cabinet.Deleted;
        //                var CheckModel = _unitOfWork.CabinetPowerLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == CabinetPowerLibraryEntity.Model.ToLower() &&
        //                    x.Id != CabinetPowerLibraryEntity.Id && !x.Deleted);
        //                if (CheckModel != null)
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, $"This model {CabinetPowerLibraryEntity.Model} is already exists", (int)ApiReturnCode.fail);
        //                }
        //                //var testUpdate = _unitOfWork.TablesHistoryRepository.CheckUpdateObject(Cabinet, CabinetPowerLibraryEntity);
        //                //if (testUpdate.Details.Count != 0)
        //                //{
        //                _unitOfWork.CabinetPowerLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, Cabinet, CabinetPowerLibraryEntity);
        //                //  resultId = _unitOfWork.TablesHistoryRepository.AddHistoryForEdit(CabinetPowerLibraryEntity.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
        //                await _unitOfWork.SaveChangesAsync();
        //                // }

        //                string CheckDependency = CheckDependencyValidationForOtherInventoryTypesEditApiVersion(OtherInventoryLibraryViewModel, TableName);
        //                if (!string.IsNullOrEmpty(CheckDependency))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)ApiReturnCode.fail);
        //                }

        //                string CheckValidation = CheckGeneralValidationFunctionEditApiVersion(CabinetPowerLibraryViewModel.DynamicAtts, TableNameEntity.TableName);
        //                if (!string.IsNullOrEmpty(CheckValidation))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckValidation, (int)ApiReturnCode.fail);
        //                }

        //                dynamic LogisticalItemIds = new ExpandoObject();
        //                LogisticalItemIds = OtherInventoryLibraryViewModel;

        //                AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

        //                var CheckVendorId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CabinetPowerLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckVendorId != null)
        //                    OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.VendorId = 0;

        //                var CheckSupplierId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CabinetPowerLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckSupplierId != null)
        //                    OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.SupplierId = 0;

        //                var CheckDesignerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CabinetPowerLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckDesignerId != null)
        //                    OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.DesignerId = 0;

        //                var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CabinetPowerLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckManufacturerId != null)
        //                    OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.ManufacturerId = 0;

        //                EditLogisticalItem(LogisticalItemIds, CabinetPowerLibraryEntity, TableNameEntity.Id, OldLogisticalItemIds);

        //                if (CabinetPowerLibraryViewModel.DynamicAtts != null ? CabinetPowerLibraryViewModel.DynamicAtts.Count > 0 : false)
        //                {
        //                    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(CabinetPowerLibraryViewModel.DynamicAtts, TableNameEntity.Id, CabinetPowerLibraryEntity.Id, Helpers.LogFilterAttribute.UserId, resultId, Cabinet.Id);
        //                }
        //                await _unitOfWork.SaveChangesAsync();
        //            }
        //            else if (OtherInventoryType.TLIcabinetTelecomLibrary.ToString().ToLower() == TableName.ToLower())
        //            {
        //                EditCabinetTelecomLibraryViewModel CabinetTelecomLibraryViewModel = _mapper.Map<EditCabinetTelecomLibraryViewModel>(OtherInventoryLibraryViewModel);
        //                TLIcabinetTelecomLibrary CabinetTelecomLibraryEntity = _mapper.Map<TLIcabinetTelecomLibrary>(OtherInventoryLibraryViewModel);
        //                if (CabinetTelecomLibraryEntity.Dimension_W_D_H == null || CabinetTelecomLibraryEntity.Dimension_W_D_H == "")
        //                {
        //                    CabinetTelecomLibraryEntity.Dimension_W_D_H = CabinetTelecomLibraryEntity.Width + "_" + CabinetTelecomLibraryEntity.Depth + "_" + CabinetTelecomLibraryEntity.Height;
        //                }
        //                var CheckModel = _unitOfWork.CabinetTelecomLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == CabinetTelecomLibraryEntity.Model.ToLower() &&
        //                    x.Id != CabinetTelecomLibraryEntity.Id && !x.Deleted);
        //                var CabinetTelecom = _unitOfWork.CabinetTelecomLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == CabinetTelecomLibraryViewModel.Id);

        //                CabinetTelecomLibraryEntity.Active = CabinetTelecom.Active;
        //                CabinetTelecomLibraryEntity.Deleted = CabinetTelecom.Deleted;

        //                if (CheckModel != null)
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, $"This model {CabinetTelecomLibraryEntity.Model} is already exists", (int)ApiReturnCode.fail);
        //                }
        //                //_unitOfWork.CabinetTelecomLibraryRepository.Update(CabinetTelecomLibraryEntity);
        //                //if (CabinetTelecomLibraryViewModel.DynamicAtts.Count > 0)
        //                //{
        //                //    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAtts(CabinetTelecomLibraryViewModel.DynamicAtts, TableNameEntity.Id, CabinetTelecomLibraryEntity.Id);

        //                //}
        //                _unitOfWork.CabinetTelecomLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, CabinetTelecom, CabinetTelecomLibraryEntity);
        //                //if (testUpdate.Details.Count != 0)
        //                //{
        //                //    _unitOfWork.CabinetTelecomLibraryRepository.Update((TLIcabinetTelecomLibrary)testUpdate.original);
        //                //    resultId = _unitOfWork.TablesHistoryRepository.AddHistoryForEdit(CabinetTelecomLibraryEntity.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
        //                //    await _unitOfWork.SaveChangesAsync();
        //                //}

        //                string CheckDependency = CheckDependencyValidationForOtherInventoryTypesEditApiVersion(OtherInventoryLibraryViewModel, TableName);
        //                if (!string.IsNullOrEmpty(CheckDependency))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)ApiReturnCode.fail);
        //                }

        //                string CheckValidation = CheckGeneralValidationFunctionEditApiVersion(CabinetTelecomLibraryViewModel.DynamicAtts, TableNameEntity.TableName);
        //                if (!string.IsNullOrEmpty(CheckValidation))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckValidation, (int)ApiReturnCode.fail);
        //                }

        //                dynamic LogisticalItemIds = new ExpandoObject();
        //                LogisticalItemIds = OtherInventoryLibraryViewModel;

        //                AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

        //                var CheckVendorId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CabinetTelecomLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckVendorId != null)
        //                    OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.VendorId = 0;

        //                var CheckSupplierId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CabinetTelecomLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckSupplierId != null)
        //                    OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.SupplierId = 0;

        //                var CheckDesignerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CabinetTelecomLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckDesignerId != null)
        //                    OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.DesignerId = 0;

        //                var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CabinetTelecomLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckManufacturerId != null)
        //                    OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.ManufacturerId = 0;

        //                EditLogisticalItem(LogisticalItemIds, CabinetTelecomLibraryEntity, TableNameEntity.Id, OldLogisticalItemIds);

        //                if (CabinetTelecomLibraryViewModel.DynamicAtts != null ? CabinetTelecomLibraryViewModel.DynamicAtts.Count > 0 : false)
        //                {
        //                    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(CabinetTelecomLibraryViewModel.DynamicAtts, TableNameEntity.Id, CabinetTelecomLibraryEntity.Id, Helpers.LogFilterAttribute.UserId, resultId, CabinetTelecom.Id);
        //                }
        //                await _unitOfWork.SaveChangesAsync();
        //                //if (CabinetTelecomLibraryViewModel.DynamicAttInst.Count > 0)
        //                //{
        //                //    //await _unitOfWork.Dynam
        //                //}
        //            }
        //            else if (OtherInventoryType.TLIgeneratorLibrary.ToString().ToLower() == TableName.ToLower())
        //            {
        //                EditGeneratorLibraryViewModel GeneratorLibraryViewModel = _mapper.Map<EditGeneratorLibraryViewModel>(OtherInventoryLibraryViewModel);
        //                TLIgeneratorLibrary GeneratorLibraryEntity = _mapper.Map<TLIgeneratorLibrary>(OtherInventoryLibraryViewModel);
        //                var generator = _unitOfWork.GeneratorLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == GeneratorLibraryViewModel.Id);
        //                var CheckModel = _unitOfWork.GeneratorLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == GeneratorLibraryEntity.Model.ToLower() &&
        //                    x.Id != GeneratorLibraryEntity.Id && !x.Deleted);

        //                GeneratorLibraryEntity.Active = generator.Active;
        //                GeneratorLibraryEntity.Deleted = generator.Deleted;

        //                if (CheckModel != null)
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, $"This model {GeneratorLibraryEntity.Model} is already exists", (int)ApiReturnCode.fail);
        //                }
        //                //_unitOfWork.GeneratorLibraryRepository.Update(GeneratorLibraryEntity);
        //                //if (GeneratorLibraryViewModel.DynamicAtts.Count > 0)
        //                //{
        //                //    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAtts(GeneratorLibraryViewModel.DynamicAtts, TableNameEntity.Id, GeneratorLibraryEntity.Id);

        //                //}

        //                _unitOfWork.GeneratorLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, generator, GeneratorLibraryEntity);

        //                string CheckDependency = CheckDependencyValidationForOtherInventoryTypesEditApiVersion(OtherInventoryLibraryViewModel, TableName);
        //                if (!string.IsNullOrEmpty(CheckDependency))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)ApiReturnCode.fail);
        //                }

        //                string CheckValidation = CheckGeneralValidationFunctionEditApiVersion(GeneratorLibraryViewModel.DynamicAtts, TableNameEntity.TableName);
        //                if (!string.IsNullOrEmpty(CheckValidation))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckValidation, (int)ApiReturnCode.fail);
        //                }

        //                dynamic LogisticalItemIds = new ExpandoObject();
        //                LogisticalItemIds = OtherInventoryLibraryViewModel;

        //                AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

        //                var CheckVendorId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == GeneratorLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckVendorId != null)
        //                    OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.VendorId = 0;

        //                var CheckSupplierId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == GeneratorLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckSupplierId != null)
        //                    OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.SupplierId = 0;

        //                var CheckDesignerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == GeneratorLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckDesignerId != null)
        //                    OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.DesignerId = 0;

        //                var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == GeneratorLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckManufacturerId != null)
        //                    OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.ManufacturerId = 0;

        //                EditLogisticalItem(LogisticalItemIds, GeneratorLibraryEntity, TableNameEntity.Id, OldLogisticalItemIds);

        //                if (GeneratorLibraryViewModel.DynamicAtts != null ? GeneratorLibraryViewModel.DynamicAtts.Count > 0 : false)
        //                {
        //                    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(GeneratorLibraryViewModel.DynamicAtts, TableNameEntity.Id, GeneratorLibraryEntity.Id, Helpers.LogFilterAttribute.UserId, resultId, generator.Id);
        //                }
        //                await _unitOfWork.SaveChangesAsync();
        //                //if (GeneratorLibraryViewModel.DynamicAttInst.Count > 0)
        //                //{
        //                //    //await _unitOfWork.Dynam
        //                //}
        //            }
        //            else if (OtherInventoryType.TLIsolarLibrary.ToString().ToLower() == TableName.ToLower())
        //            {
        //                EditSolarLibraryViewModel SolarLibraryViewModel = _mapper.Map<EditSolarLibraryViewModel>(OtherInventoryLibraryViewModel);
        //                TLIsolarLibrary SolarLibraryEntity = _mapper.Map<TLIsolarLibrary>(OtherInventoryLibraryViewModel);
        //                var solar = _unitOfWork.SolarLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == SolarLibraryViewModel.Id);

        //                SolarLibraryEntity.Active = solar.Active;
        //                SolarLibraryEntity.Deleted = solar.Deleted;

        //                var CheckModel = _unitOfWork.SolarLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == SolarLibraryEntity.Model.ToLower() &&
        //                    x.Id != SolarLibraryEntity.Id && !x.Deleted);
        //                if (CheckModel != null)
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, $"This model {SolarLibraryEntity.Model} is already exists", (int)ApiReturnCode.fail);
        //                }
        //                //_unitOfWork.SolarLibraryRepository.Update(SolarLibraryEntity);
        //                //if (SolarLibraryViewModel.DynamicAtts.Count > 0)
        //                //{
        //                //    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAtts(SolarLibraryViewModel.DynamicAtts, TableNameEntity.Id, SolarLibraryEntity.Id);

        //                //}
        //                _unitOfWork.SolarLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, solar, SolarLibraryEntity);
        //                //if (testUpdate.Details.Count != 0)
        //                //{
        //                //    _unitOfWork.SolarLibraryRepository.Update((TLIsolarLibrary)testUpdate.original);
        //                //    resultId = _unitOfWork.TablesHistoryRepository.AddHistoryForEdit(SolarLibraryEntity.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
        //                //    await _unitOfWork.SaveChangesAsync();
        //                //}
        //                string CheckDependency = CheckDependencyValidationForOtherInventoryTypesEditApiVersion(OtherInventoryLibraryViewModel, TableName);
        //                if (!string.IsNullOrEmpty(CheckDependency))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)ApiReturnCode.fail);
        //                }

        //                string CheckValidation = CheckGeneralValidationFunctionEditApiVersion(SolarLibraryViewModel.DynamicAtts, TableNameEntity.TableName);
        //                if (!string.IsNullOrEmpty(CheckValidation))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckValidation, (int)ApiReturnCode.fail);
        //                }

        //                dynamic LogisticalItemIds = new ExpandoObject();
        //                LogisticalItemIds = OtherInventoryLibraryViewModel;

        //                AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

        //                var CheckVendorId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == SolarLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckVendorId != null)
        //                    OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.VendorId = 0;

        //                var CheckSupplierId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == SolarLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckSupplierId != null)
        //                    OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.SupplierId = 0;

        //                var CheckDesignerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == SolarLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckDesignerId != null)
        //                    OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.DesignerId = 0;

        //                var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == SolarLibraryEntity.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckManufacturerId != null)
        //                    OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.ManufacturerId = 0;

        //                EditLogisticalItem(LogisticalItemIds, SolarLibraryEntity, TableNameEntity.Id, OldLogisticalItemIds);

        //                if (SolarLibraryViewModel.DynamicAtts != null ? SolarLibraryViewModel.DynamicAtts.Count > 0 : false)
        //                {
        //                    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(SolarLibraryViewModel.DynamicAtts, TableNameEntity.Id, SolarLibraryEntity.Id, Helpers.LogFilterAttribute.UserId, resultId, solar.Id);
        //                }
        //                await _unitOfWork.SaveChangesAsync();
        //                //if (SolarLibraryViewModel.DynamicAttInst.Count > 0)
        //                //{
        //                //    //await _unitOfWork.Dynam
        //                //}
        //            }
        //            transaction.Complete();
        //            return new Response<AllItemAttributes>();
        //        }
        //        catch (Exception err)
        //        {
        //            return new Response<AllItemAttributes>(true, null, null, err.Message, (int)ApiReturnCode.fail);
        //        }
        //    }
        //}
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

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.LogisticalItems.VendorId;

                                    _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.LogisticalItems.VendorId,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.Add(NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
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

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.LogisticalItems.SupplierId;

                                    _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.LogisticalItems.SupplierId,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.Add(NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
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

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.LogisticalItems.DesignerId;

                                    _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.LogisticalItems.DesignerId,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.Add(NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
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

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.LogisticalItems.ManufacturerId;

                                    _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.LogisticalItems.ManufacturerId,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.Add(NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
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
        public string CheckDependencyValidationForOtherInventoryTypesEditApiVersion(object Input, string OtherInventoryType)
        {
            if (OtherInventoryType.ToLower() == TablesNames.TLIcabinetPowerLibrary.ToString().ToLower())
            {
                EditCabinetPowerLibraryViewModel EditCabinetPowerLibraryViewModel = _mapper.Map<EditCabinetPowerLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == OtherInventoryType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditCabinetPowerLibraryViewModel.DynamicAtts
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

                                    InsertedValue = EditCabinetPowerLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditCabinetPowerLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditCabinetPowerLibraryViewModel.DynamicAtts
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
            else if (OtherInventoryType.ToLower() == TablesNames.TLIcabinetTelecomLibrary.ToString().ToLower())
            {
                EditCabinetTelecomLibraryViewModel EditCabinetTelecomLibraryViewModel = _mapper.Map<EditCabinetTelecomLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == OtherInventoryType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditCabinetTelecomLibraryViewModel.DynamicAtts
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

                                    InsertedValue = EditCabinetTelecomLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditCabinetTelecomLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditCabinetTelecomLibraryViewModel.DynamicAtts
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
            else if (OtherInventoryType.ToLower() == TablesNames.TLIsolarLibrary.ToString().ToLower())
            {
                EditSolarLibraryViewModel EditSolarLibraryViewModel = _mapper.Map<EditSolarLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == OtherInventoryType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditSolarLibraryViewModel.DynamicAtts
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

                                    InsertedValue = EditSolarLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditSolarLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditSolarLibraryViewModel.DynamicAtts
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
            else if (OtherInventoryType.ToLower() == TablesNames.TLIgeneratorLibrary.ToString().ToLower())
            {
                EditGeneratorLibraryViewModel EditGeneratorLibraryViewModel = _mapper.Map<EditGeneratorLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == OtherInventoryType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditGeneratorLibraryViewModel.DynamicAtts
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

                                    InsertedValue = EditGeneratorLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditGeneratorLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditGeneratorLibraryViewModel.DynamicAtts
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
        //get table name Entity depened on TableName
        //specify the table i deal with
        //get record by Id
        //get activated attributed with values
        //get dynamic attributes by TableNameId
        public Response<GetForAddCivilLibrarybject> GetById(int Id, string TableName)
        {
            try
            {
                GetForAddCivilLibrarybject attributes = new GetForAddCivilLibrarybject();

                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c =>
                    c.TableName == TableName);

                List<BaseAttView> ListAttributesActivated = new List<BaseAttView>();

                //if (OtherInventoryType.TLIcabinetPowerLibrary.ToString() == TableName)
                //{
                //    TLIcabinetPowerLibrary CabinetPowerLibrary = _unitOfWork.CabinetPowerLibraryRepository.GetIncludeWhereFirst(x =>
                //        x.Id == Id, x => x.CabinetPowerType);

                //    object FK_CabinetPowerType_Name = CabinetPowerLibrary.CabinetPowerType != null ? CabinetPowerLibrary.CabinetPowerType.Name : null;

                //    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, CabinetPowerLibrary, null).ToList();

                //    foreach (BaseAttView FKitem in ListAttributesActivated)
                //    {
                //        if (FKitem.Label.ToLower() == "cabinetpowertype_name")
                //        {
                //            if (FK_CabinetPowerType_Name == null)
                //                FKitem.Value = _unitOfWork.CabinetPowerTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                //            else
                //                FKitem.Value = FK_CabinetPowerType_Name;
                //        }
                //    }
                //}
                //else if (OtherInventoryType.TLIcabinetTelecomLibrary.ToString() == TableName)
                //{
                //    TLIcabinetTelecomLibrary CabinetTelecomLibrary = _unitOfWork.CabinetTelecomLibraryRepository.GetIncludeWhereFirst(x =>
                //        x.Id == Id, x => x.TelecomType);

                //    object FK_TelecomType_Name = CabinetTelecomLibrary.TelecomType != null ? CabinetTelecomLibrary.TelecomType.Name : null;

                //    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, CabinetTelecomLibrary, null).ToList();

                //    foreach (BaseAttView FKitem in ListAttributesActivated)
                //    {
                //        if (FKitem.Label.ToLower() == "telecomtype_name")
                //        {
                //            if (FK_TelecomType_Name == null)
                //                FKitem.Value = _unitOfWork.TelecomTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                //            else
                //                FKitem.Value = FK_TelecomType_Name;
                //        }
                //    }
                //}
                if (OtherInventoryType.TLIgeneratorLibrary.ToString() == TableName)
                {
                    TLIgeneratorLibrary GeneratorLibrary = _unitOfWork.GeneratorLibraryRepository.GetIncludeWhereFirst(x =>
                        x.Id == Id && !x.Deleted, x => x.Capacity);
                    if (GeneratorLibrary != null)
                    {
                        List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TableName, GeneratorLibrary, null).ToList();
                        attributes.LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticalsNonSteel(Helpers.Constants.TablePartName.OtherInventory.ToString(), TableName, Id);
                        attributes.AttributesActivatedLibrary = listofAttributesActivated;
                        attributes.DynamicAttributes = _unitOfWork.DynamicAttLibRepository.GetDynamicLibAtt(TableNameEntity.Id, Id, null);
                        List<BaseInstAttViews> Test = attributes.AttributesActivatedLibrary.ToList();
                        BaseInstAttViews NameAttribute = Test.FirstOrDefault(x => x.Key.ToLower() == "Model".ToLower());
                        if (NameAttribute != null)
                        {
                            BaseInstAttViews Swap = Test.ToList()[0];
                            Test[Test.IndexOf(NameAttribute)] = Swap;
                            Test[0] = NameAttribute;
                            attributes.AttributesActivatedLibrary = Test;
                            NameAttribute.Value = db.MV_GENERATOR_LIBRARY_VIEW.FirstOrDefault(x => x.Id == Id)?.Model;
                        }
                    }
                    else
                    {
                        return new Response<GetForAddCivilLibrarybject>(false, null, null, "this generator is not  found", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                //else if (OtherInventoryType.TLIsolarLibrary.ToString() == TableName)
                //{
                //    TLIsolarLibrary SolarLibrary = _unitOfWork.SolarLibraryRepository.GetIncludeWhereFirst(x =>
                //        x.Id == Id, x => x.Capacity);

                //    object FK_Capacity_Name = SolarLibrary.Capacity != null ? SolarLibrary.Capacity.Name : null;

                //    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, SolarLibrary, null).ToList();

                //    foreach (BaseAttView FKitem in ListAttributesActivated)
                //    {
                //        if (FKitem.Label.ToLower() == "capacity_name")
                //        {
                //            if (FK_Capacity_Name == null)
                //                FKitem.Value = _unitOfWork.CapacityRepository.GetWhereFirst(x => x.Id == 0).Name;

                //            else
                //                FKitem.Value = FK_Capacity_Name;
                //        }
                //    }
                //}

               

                return new Response<GetForAddCivilLibrarybject>(true, attributes, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public async Task<Response<EditGeneratorLibraryObject>> EditGeneratorLibrary(int userId, EditGeneratorLibraryObject editGeneratorLibraryObject, string TableName, string connectionString)
        {
            using (TransactionScope transaction =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {

                    int resultId = 0;

                    TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);

                    TLIgeneratorLibrary GeneratorLibraryEntites = _mapper.Map<TLIgeneratorLibrary>(editGeneratorLibraryObject.AttributesActivatedLibrary);

                    TLIgeneratorLibrary GeneratorLegLib = _unitOfWork.GeneratorLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == GeneratorLibraryEntites.Id);


                    if (GeneratorLibraryEntites.SpaceLibrary <= 0)
                    {
                        if (GeneratorLibraryEntites.Height <= 0)
                        {
                            return new Response<EditGeneratorLibraryObject>(false, null, null, "Height must bigger of zero", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                        else if (GeneratorLibraryEntites.Width <= 0)
                        {
                            return new Response<EditGeneratorLibraryObject>(false, null, null, "Width must bigger of zero", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                        else
                        {
                            GeneratorLibraryEntites.SpaceLibrary = GeneratorLibraryEntites.Height * GeneratorLibraryEntites.Width;
                        }
                    }
                    var CheckModel = db.MV_GENERATOR_LIBRARY_VIEW
                             .FirstOrDefault(x => x.Model != null &&
                                         x.Model.ToLower() == GeneratorLibraryEntites.Model.ToLower() &&
                                         x.Id != GeneratorLibraryEntites.Id && !x.Deleted);
                    if (CheckModel != null)
                    {
                        return new Response<EditGeneratorLibraryObject>(false, null, null, $"This model {GeneratorLibraryEntites.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                    GeneratorLibraryEntites.Active = GeneratorLegLib.Active;
                    GeneratorLibraryEntites.Deleted = GeneratorLegLib.Deleted;

                    _unitOfWork.GeneratorLibraryRepository.UpdateWithHistory(userId, GeneratorLegLib, GeneratorLibraryEntites);


                    //string CheckDependencyValidation = CheckDependencyValidationForCivilTypesEditApiVersions(editCivilWithLegsLibrary, TableName);
                    //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                    //{
                    //    return new Response<EditCivilWithLegsLibraryObject>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                    //}

                    //string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersions(editCivilWithLegsLibrary.dynamicAttributes, TableNameEntity.TableName);
                    //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                    //{
                    //    return new Response<EditCivilWithLegsLibraryObject>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                    //}

                    AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                    var CheckVendorId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == GeneratorLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckVendorId != null)
                        OldLogisticalItemIds.Vendor = Convert.ToInt32(CheckVendorId.logisticalId);

                    var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == GeneratorLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckSupplierId != null)
                        OldLogisticalItemIds.Supplier = CheckSupplierId.logisticalId;

                    var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == GeneratorLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckDesignerId != null)
                        OldLogisticalItemIds.Designer = CheckDesignerId.logisticalId;


                    var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == GeneratorLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckManufacturerId != null)
                        OldLogisticalItemIds.Manufacturer = CheckManufacturerId.logisticalId;


                    var CheckContractorId = _unitOfWork.LogisticalitemRepository
                 .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Contractor.ToString().ToLower() &&
                     x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == GeneratorLibraryEntites.Id, x => x.logistical,
                         x => x.logistical.logisticalType);

                    if (CheckContractorId != null)
                        OldLogisticalItemIds.Contractor = CheckContractorId.logisticalId;


                    var CheckConsultantId = _unitOfWork.LogisticalitemRepository
                       .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Consultant.ToString().ToLower() &&
                           x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == GeneratorLibraryEntites.Id, x => x.logistical,
                               x => x.logistical.logisticalType);

                    if (CheckConsultantId != null)
                        OldLogisticalItemIds.Consultant = CheckConsultantId.logisticalId;


                    EditLogisticalItemss(userId, editGeneratorLibraryObject.LogisticalItems, GeneratorLibraryEntites, TableNameEntity.Id, OldLogisticalItemIds);

                    if (editGeneratorLibraryObject.DynamicAttributes != null ? editGeneratorLibraryObject.DynamicAttributes.Count > 0 : false)
                    {
                        _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistorys(editGeneratorLibraryObject.DynamicAttributes, connectionString, TableNameEntity.Id, GeneratorLibraryEntites.Id, userId, resultId, GeneratorLegLib.Id);
                    }

                    await _unitOfWork.SaveChangesAsync();


                    transaction.Complete();
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                    return new Response<EditGeneratorLibraryObject>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {
                    return new Response<EditGeneratorLibraryObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

        }
        public void EditLogisticalItemss(int UserId, AddLogisticalViewModel LogisticalItemIds, dynamic MainEntity, int TableNameEntityId, AddLogisticalViewModel OldLogisticalItemIds)
        {
            using (TransactionScope transaction2 =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    if (LogisticalItemIds != null)
                    {
                        if (LogisticalItemIds.Vendor != null && LogisticalItemIds.Vendor != 0)
                        {
                            if (OldLogisticalItemIds.Vendor != null ? OldLogisticalItemIds.Vendor != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(OldLogisticalItemIds.Vendor);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id
                                && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Vendor;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Vendor,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(LogisticalItemIds.Vendor);

                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MainEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.Supplier != null && LogisticalItemIds.Supplier != 0)
                        {
                            if (OldLogisticalItemIds.Supplier != null ? OldLogisticalItemIds.Supplier != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == OldLogisticalItemIds.Supplier);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id
                                && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Supplier;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Supplier,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = db.TLIlogistical.FirstOrDefault(x => x.Id == LogisticalItemIds.Supplier);
                                if (LogisticalObject != null)
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                    {
                                        Name = "",
                                        IsLib = true,
                                        logisticalId = LogisticalObject.Id,
                                        RecordId = MainEntity.Id,
                                        tablesNamesId = TableNameEntityId
                                    };
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Designer != null && LogisticalItemIds.Designer != 0)
                        {
                            if (OldLogisticalItemIds.Designer != null ? OldLogisticalItemIds.Designer != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == OldLogisticalItemIds.Designer);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id
                                && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Designer;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Designer,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == LogisticalItemIds.Designer);
                                if (LogisticalObject != null)
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                    {
                                        Name = "",
                                        IsLib = true,
                                        logisticalId = LogisticalObject.Id,
                                        RecordId = MainEntity.Id,
                                        tablesNamesId = TableNameEntityId
                                    };
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Manufacturer != null && LogisticalItemIds.Manufacturer != 0)
                        {
                            if (OldLogisticalItemIds.Manufacturer != null ? OldLogisticalItemIds.Manufacturer != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == OldLogisticalItemIds.Manufacturer);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id &&
                                x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Manufacturer;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Manufacturer,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = db.TLIlogistical.FirstOrDefault(x => x.Id == LogisticalItemIds.Manufacturer);
                                if (LogisticalObject != null)
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                    {
                                        Name = "",
                                        IsLib = true,
                                        logisticalId = LogisticalObject.Id,
                                        RecordId = MainEntity.Id,
                                        tablesNamesId = TableNameEntityId
                                    };
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Consultant != null && LogisticalItemIds.Consultant != 0)
                        {
                            if (OldLogisticalItemIds.Consultant != null ? OldLogisticalItemIds.Consultant != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == OldLogisticalItemIds.Consultant);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id &&
                                x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Consultant;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Manufacturer,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = db.TLIlogistical.FirstOrDefault(x => x.Id == LogisticalItemIds.Consultant);
                                if (LogisticalObject != null)
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                    {
                                        Name = "",
                                        IsLib = true,
                                        logisticalId = LogisticalObject.Id,
                                        RecordId = MainEntity.Id,
                                        tablesNamesId = TableNameEntityId
                                    };
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Contractor != null && LogisticalItemIds.Contractor != 0)
                        {
                            if (OldLogisticalItemIds.Contractor != null ? OldLogisticalItemIds.Contractor != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == OldLogisticalItemIds.Contractor);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id &&
                                x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Contractor;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Manufacturer,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = db.TLIlogistical.FirstOrDefault(x => x.Id == LogisticalItemIds.Contractor);
                                if (LogisticalObject != null)
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                    {
                                        Name = "",
                                        IsLib = true,
                                        logisticalId = LogisticalObject.Id,
                                        RecordId = MainEntity.Id,
                                        tablesNamesId = TableNameEntityId
                                    };
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
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
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function list of records depened on filters and parameters
        //If WithFilterData is true then return Related tables
        public Response<ReturnWithFilters<CabinetPowerLibraryViewModel>> GetCabinetPowerLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            int count = 0;
            try
            {
                IEnumerable<TLIcabinetPowerLibrary> cabinetPowerLibraries;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                cabinetPowerLibraries = _unitOfWork.CabinetPowerLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, c => c.CabinetPowerType).OrderBy(x => x.Id).ToList();

                IEnumerable<CabinetPowerLibraryViewModel> cabinetPowerLibraryViewModels = _mapper.Map<IEnumerable<CabinetPowerLibraryViewModel>>(cabinetPowerLibraries);
                ReturnWithFilters<CabinetPowerLibraryViewModel> returnWithFilters = new ReturnWithFilters<CabinetPowerLibraryViewModel>();
                returnWithFilters.Model = cabinetPowerLibraryViewModels.ToList();
                if (WithFilterData.Equals(true))
                {
                    returnWithFilters.filters = _unitOfWork.CabinetPowerLibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<CabinetPowerLibraryViewModel>>(true, returnWithFilters, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<CabinetPowerLibraryViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }

        }
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function list of records depened on filters and parameters
        //If WithFilterData is true then return Related tables
        public Response<ReturnWithFilters<CabinetTelecomLibraryViewModel>> GetCabinetTelecomLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            int count = 0;
            try
            {
                IEnumerable<TLIcabinetTelecomLibrary> CabinetTelecomLibraries;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                CabinetTelecomLibraries = _unitOfWork.CabinetTelecomLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, c => c.TelecomType).OrderBy(x => x.Id).ToList();
                IEnumerable<CabinetTelecomLibraryViewModel> cabinetTelecomLibraryViewModels = _mapper.Map<IEnumerable<CabinetTelecomLibraryViewModel>>(CabinetTelecomLibraries);
                ReturnWithFilters<CabinetTelecomLibraryViewModel> returnWithFilters = new ReturnWithFilters<CabinetTelecomLibraryViewModel>();
                returnWithFilters.Model = cabinetTelecomLibraryViewModels.ToList();
                if (WithFilterData.Equals(true))
                {
                    returnWithFilters.filters = _unitOfWork.CabinetTelecomLibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<CabinetTelecomLibraryViewModel>>(true, returnWithFilters, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<CabinetTelecomLibraryViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function list of records depened on filters and parameters
        //If WithFilterData is true then return Related tables
        public Response<ReturnWithFilters<GeneratorLibraryViewModel>> GetGeneratorLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            int count = 0;
            try
            {
                IEnumerable<TLIgeneratorLibrary> GeneratorLibraries;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                GeneratorLibraries = _unitOfWork.GeneratorLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, c => c.Capacity).OrderBy(x => x.Id).ToList();
                IEnumerable<GeneratorLibraryViewModel> generatorLibraryViewModels = _mapper.Map<IEnumerable<GeneratorLibraryViewModel>>(GeneratorLibraries);
                ReturnWithFilters<GeneratorLibraryViewModel> returnWithFilters = new ReturnWithFilters<GeneratorLibraryViewModel>();
                returnWithFilters.Model = generatorLibraryViewModels.ToList();
                if (WithFilterData.Equals(true))
                {
                    returnWithFilters.filters = _unitOfWork.GeneratorLibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<GeneratorLibraryViewModel>>(true, returnWithFilters, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<GeneratorLibraryViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function list of records depened on filters and parameters
        //If WithFilterData is true then return Related tables
        public Response<ReturnWithFilters<SolarLibraryViewModel>> GetSolarLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                IEnumerable<TLIsolarLibrary> solarLibraries;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                solarLibraries = _unitOfWork.SolarLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, c => c.Capacity).OrderBy(x => x.Id).ToList();
                IEnumerable<SolarLibraryViewModel> solarLibraryViewModels = _mapper.Map<IEnumerable<SolarLibraryViewModel>>(solarLibraries);
                ReturnWithFilters<SolarLibraryViewModel> returnWithFilters = new ReturnWithFilters<SolarLibraryViewModel>();
                returnWithFilters.Model = solarLibraryViewModels.ToList();
                if (WithFilterData.Equals(true))
                {
                    returnWithFilters.filters = _unitOfWork.SolarLibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<SolarLibraryViewModel>>(true, returnWithFilters, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<SolarLibraryViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter TableName
        //get table name Entity dpened on TableName
        //return list of activated attributes
        //return list of dynamic attributes
        public Response<GetForAddCivilLibrarybject> GetForAdd(string TableName)
        {
            try
            {
                GetForAddCivilLibrarybject attributes = new GetForAddCivilLibrarybject();
                var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(o => o.TableName == TableName);
                if (OtherInventoryType.TLIsolarLibrary.ToString() == TableName)
                {
                    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TablesNames.TLIsolarLibrary.ToString(), null, null).ToList();
                    var LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticalLibrary("MW");
                    attributes.LogisticalItems = LogisticalItems;
                    attributes.AttributesActivatedLibrary = ListAttributesActivated;
                    IEnumerable<BaseInstAttViewDynamic> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                   .GetDynamicLibAtt(TableNameEntity.Id, null)
                   .Select(DynamicAttribute =>
                   {
                       TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);
                       if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                       {
                           switch (DynamicAttribute.DataType.ToLower())
                           {
                               case "string":
                                   DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;
                                   break;
                               case "int":
                                   DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                               case "double":
                                   DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                               case "bool":
                                   DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                               case "datetime":
                                   DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                           }
                       }
                       else
                       {
                           DynamicAttribute.Value = " ".Split(' ')[0];
                       }
                       return DynamicAttribute;
                   });

                    attributes.DynamicAttributes = DynamicAttributesWithoutValue;
                }
                else if (OtherInventoryType.TLIgeneratorLibrary.ToString() == TableName)
                {
                    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TablesNames.TLIgeneratorLibrary.ToString(), null, null)
                      .ToList();
                    var LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticalLibrary("OtherInventory");
                    attributes.LogisticalItems = LogisticalItems;
                    attributes.AttributesActivatedLibrary = ListAttributesActivated;
                    IEnumerable<BaseInstAttViewDynamic> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                   .GetDynamicLibAtt(TableNameEntity.Id, null)
                   .Select(DynamicAttribute =>
                   {
                       TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);
                       if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                       {
                           switch (DynamicAttribute.DataType.ToLower())
                           {
                               case "string":
                                   DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;
                                   break;
                               case "int":
                                   DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                               case "double":
                                   DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                               case "bool":
                                   DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                               case "datetime":
                                   DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                           }
                       }
                       else
                       {
                           DynamicAttribute.Value = " ".Split(' ')[0];
                       }
                       return DynamicAttribute;
                   });

                    attributes.DynamicAttributes = DynamicAttributesWithoutValue;
                }
                //else if (OtherInventoryType.TLIcabinetPowerLibrary.ToString() == TableName)
                //{
                //    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();
                //    ListAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical("MW"));
                //    attributes.AttributesActivated = ListAttributesActivated;
                //    attributes.DynamicAtts = _unitOfWork.DynamicAttRepository.GetDynamicLibAtts(TableNameEntity.Id, null);
                //    attributes.DynamicAttInst = null;
                //}
                //else if (OtherInventoryType.TLIcabinetTelecomLibrary.ToString() == TableName)
                //{
                //    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();
                //    ListAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical("MW"));
                //    attributes.AttributesActivated = ListAttributesActivated;
                //    attributes.DynamicAtts = _unitOfWork.DynamicAttRepository.GetDynamicLibAtts(TableNameEntity.Id, null);
                //    attributes.DynamicAttInst = null;
                //}
                return new Response<GetForAddCivilLibrarybject>(true, attributes, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters Id, TableName
        //get table name Entity depened on TableName
        //specify the table i deal with
        //get record by Id
        //set Deleted to true
        //Update Entity
        //disable dynamic attributes related to that record
        public async Task<Response<AllItemAttributes>> Delete(int Id, string TableName,string ConnectionString)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(o => o.TableName == TableName);
                    if (OtherInventoryType.TLIcabinetPowerLibrary.ToString() == TableName)
                    {
                        var UseCabinetPower=_unitOfWork.CabinetRepository.GetWhere(x=>x.CabinetPowerLibraryId == Id).ToList();
                        if (UseCabinetPower != null && UseCabinetPower.Count>0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not delete this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                            var CabinetPowerLibrary = _unitOfWork.CabinetPowerLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                            var NeweCabinetPowerLibrary = _unitOfWork.CabinetPowerLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                            NeweCabinetPowerLibrary.Deleted = true;
                            NeweCabinetPowerLibrary.Model = NeweCabinetPowerLibrary.Model + "_" + DateTime.Now.ToString();

                            _unitOfWork.CabinetPowerLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, CabinetPowerLibrary, NeweCabinetPowerLibrary);
                            _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                            await _unitOfWork.SaveChangesAsync();
                       
                        //  AddHistory(CabinetPowerLibrary.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString());
                    }
                    else if (OtherInventoryType.TLIcabinetTelecomLibrary.ToString() == TableName)
                    {
                        var UseCabinetPower = _unitOfWork.CabinetRepository.GetWhere(x => x.CabinetTelecomLibraryId == Id).ToList();
                        if (UseCabinetPower != null && UseCabinetPower.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not delete this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                        var CabinetTelecomLibrary = _unitOfWork.CabinetTelecomLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        TLIcabinetTelecomLibrary NewCabinetTelecomLibrary = _unitOfWork.CabinetTelecomLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewCabinetTelecomLibrary.Deleted = true;
                        NewCabinetTelecomLibrary.Model = NewCabinetTelecomLibrary.Model + "_" + DateTime.Now.ToString();

                        _unitOfWork.CabinetTelecomLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, CabinetTelecomLibrary, NewCabinetTelecomLibrary);
                        _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                        // AddHistory(CabinetTelecomLibrary.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString());
                    }
                    else if (OtherInventoryType.TLIgeneratorLibrary.ToString() == TableName)
                    {
                        var UseCabinetPower = _unitOfWork.GeneratorRepository.GetWhere(x => x.GeneratorLibraryId == Id).ToList();
                        if (UseCabinetPower != null && UseCabinetPower.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not delete this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                        TLIgeneratorLibrary OldGeneratorLibrary = _unitOfWork.GeneratorLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);

                        TLIgeneratorLibrary NewGeneratorLibrary = _unitOfWork.GeneratorLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewGeneratorLibrary.Deleted = true;
                        NewGeneratorLibrary.Model = NewGeneratorLibrary.Model + "_" + DateTime.Now.ToString();

                        _unitOfWork.GeneratorLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldGeneratorLibrary, NewGeneratorLibrary);
                        _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                        //AddHistory(GeneratorLibrary.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString());
                    }
                    else if (OtherInventoryType.TLIsolarLibrary.ToString() == TableName)
                    {
                        var UseCabinetPower = _unitOfWork.SolarRepository.GetWhere(x => x.SolarLibraryId == Id).ToList();
                        if (UseCabinetPower != null && UseCabinetPower.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not delete this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                        var SolarLibrary = _unitOfWork.SolarLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        TLIsolarLibrary NewSolarLibrary = _unitOfWork.SolarLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewSolarLibrary.Deleted = true;
                        NewSolarLibrary.Model = NewSolarLibrary.Model + "_" + DateTime.Now.ToString();

                        _unitOfWork.SolarLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, SolarLibrary, NewSolarLibrary);
                        _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                        //  AddHistory(SolarLibrary.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIsolarLibrary.ToString());
                    }
                    transaction.Complete();
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(ConnectionString));
                    return new Response<AllItemAttributes>();
                }
                catch (Exception err)
                {

                    return new Response<AllItemAttributes>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                }
            }
        }

        #region Get Enabled Attributes Only With Dynamic Objects (Libraries Only)...
        #region Helper Methods
        public void GetInventoriesIdsFromDynamicAttributes(out List<int> DynamicLibValueListIds, List<TLIdynamicAtt> LibDynamicAttListIds, List<StringFilterObjectList> AttributeFilters)
        {
            try
            {
                List<StringFilterObjectList> DynamicLibAttributeFilters = AttributeFilters.Where(x =>
                    LibDynamicAttListIds.Select(x => x.Key.ToLower()).Contains(x.key.ToLower())).ToList();
                
                DynamicLibValueListIds = new List<int>();

                List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                    LibDynamicAttListIds.Select(y => y.Id).Contains(x.DynamicAttId) && !x.disable).ToList();

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
        public Response<ReturnWithFilters<object>> GetCabinetPowerLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> CabinetPowerTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<CabinetPowerLibraryViewModel> CabinetPowerLibraries = new List<CabinetPowerLibraryViewModel>();
                List<CabinetPowerLibraryViewModel> WithoutDateFilterCabinetPowerLibraries = new List<CabinetPowerLibraryViewModel>();
                List<CabinetPowerLibraryViewModel> WithDateFilterCabinetPowerLibraries = new List<CabinetPowerLibraryViewModel>();

                List<TLIattributeActivated> CabinetPowerLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    CabinetPowerLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CabinetPowerLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateCabinetPowerLibraryAttribute = CabinetPowerLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateCabinetPowerLibraryAttribute.FirstOrDefault(x =>
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
                    var temp = AttributeFilters.Select(x => x.key.ToLower()).ToList();
                    List<TLIdynamicAtt> LibDynamicAttListIds=_unitOfWork.DynamicAttRepository.GetIncludeWhere(x => x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();
                    LibDynamicAttListIds = LibDynamicAttListIds.FindAll(x => temp.Any(y => y == x.Key.ToLower()));

                    //List<TLIdynamicAtt> LibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x => temp.Any(y => y == x.Key) &&
                    //    x.LibraryAtt && !x.disable &&
                    //    x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();
                    

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
                    bool AttrLibExist = typeof(CabinetPowerLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(CabinetPowerLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(CabinetPowerLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIcabinetPowerLibrary> Libraries = _unitOfWork.CabinetPowerLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<CabinetPowerLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CabinetPowerLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CabinetPowerLibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<CabinetPowerLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
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

                    WithoutDateFilterCabinetPowerLibraries = _mapper.Map<List<CabinetPowerLibraryViewModel>>(_unitOfWork.CabinetPowerLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.CabinetPowerType).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateCabinetPowerLibraryAttribute = CabinetPowerLibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = DateCabinetPowerLibraryAttribute.FirstOrDefault(x =>
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
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString(), x => x.tablesNames).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (DateTimeLibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeLibDynamicAttListIds.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                        DynamicLibValueListIds = new List<int>();

                        List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            DateTimeLibDynamicAttListIds.Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Select(x => x.ValueDateTime).Any(x =>
                                 (x != null ?
                                    (x >= y.DateFrom && x <= y.DateTo) : (false)))))
                            {
                                DynamicLibValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Library Attribute Activated...
                    //
                    List<PropertyInfo> LibraryProps = typeof(CabinetPowerLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIcabinetPowerLibrary> Libraries = _unitOfWork.CabinetPowerLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CabinetPowerLibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<CabinetPowerLibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<CabinetPowerLibraryViewModel>(x), null)))) : (false))));
                        }

                        LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
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

                    WithDateFilterCabinetPowerLibraries = _mapper.Map<List<CabinetPowerLibraryViewModel>>(_unitOfWork.CabinetPowerLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.CabinetPowerType).ToList());
                }

                //
                // Intersect Between WithoutDateFilterCabinetPowerLibraries + WithDateFilterCabinetPowerLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    CabinetPowerLibraries = _mapper.Map<List<CabinetPowerLibraryViewModel>>(_unitOfWork.CabinetPowerLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted, x => x.CabinetPowerType).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> CabinetPowerIds = WithoutDateFilterCabinetPowerLibraries.Select(x => x.Id).Intersect(WithDateFilterCabinetPowerLibraries.Select(x => x.Id)).ToList();
                    CabinetPowerLibraries = _mapper.Map<List<CabinetPowerLibraryViewModel>>(_unitOfWork.CabinetPowerLibraryRepository.GetWhere(x =>
                        CabinetPowerIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    CabinetPowerLibraries = WithoutDateFilterCabinetPowerLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    CabinetPowerLibraries = WithDateFilterCabinetPowerLibraries;
                }

                Count = CabinetPowerLibraries.Count();

                CabinetPowerLibraries = CabinetPowerLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CabinetPowerLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString()) : false),
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

                foreach (CabinetPowerLibraryViewModel CabinetPowerLibraryViewModel in CabinetPowerLibraries)
                {
                    dynamic DynamicCabinetPowerLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(CabinetPowerLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in LibraryProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeLibraryAttributesViewModel.Select(x =>
                                    x.AttributeActivated.Label.ToLower()).Contains(prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(CabinetPowerLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicCabinetPowerLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(CabinetPowerLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicCabinetPowerLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(CabinetPowerLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicCabinetPowerLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    var temp = NotDateTimeDynamicLibraryAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        temp.Any(y => y == x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == CabinetPowerLibraryViewModel.Id && !x.disable &&
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

                            ((IDictionary<String, Object>)DynamicCabinetPowerLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicCabinetPowerLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(CabinetPowerLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(CabinetPowerLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    temp = DateTimeDynamicLibraryAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        temp.Any(y => y == x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == CabinetPowerLibraryViewModel.Id && !x.disable &&
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

                    ((IDictionary<String, Object>)DynamicCabinetPowerLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicCabinetPowerLibrary);
                }

                CabinetPowerTableDisplay.Model = OutPutList;

                if (WithFilterData)
                    CabinetPowerTableDisplay.filters = _unitOfWork.CabinetPowerLibraryRepository.GetRelatedTables();
                else
                    CabinetPowerTableDisplay.filters = null;

                return new Response<ReturnWithFilters<object>>(true, CabinetPowerTableDisplay, null, null, (int)ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetCabinetTelecomLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> CabinetTelecomTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<CabinetTelecomLibraryViewModel> CabinetTelecomLibraries = new List<CabinetTelecomLibraryViewModel>();
                List<CabinetTelecomLibraryViewModel> WithoutDateFilterCabinetTelecomLibraries = new List<CabinetTelecomLibraryViewModel>();
                List<CabinetTelecomLibraryViewModel> WithDateFilterCabinetTelecomLibraries = new List<CabinetTelecomLibraryViewModel>();

                List<TLIattributeActivated> CabinetTelecomLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    CabinetTelecomLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CabinetTelecomLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateCabinetTelecomLibraryAttribute = CabinetTelecomLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateCabinetTelecomLibraryAttribute.FirstOrDefault(x =>
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
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

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
                    bool AttrLibExist = typeof(CabinetTelecomLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(CabinetTelecomLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(CabinetTelecomLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIcabinetTelecomLibrary> Libraries = _unitOfWork.CabinetTelecomLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<CabinetTelecomLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CabinetTelecomLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CabinetTelecomLibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<CabinetTelecomLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
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

                    WithoutDateFilterCabinetTelecomLibraries = _mapper.Map<List<CabinetTelecomLibraryViewModel>>(_unitOfWork.CabinetTelecomLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.TelecomType).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateCabinetTelecomLibraryAttribute = CabinetTelecomLibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = DateCabinetTelecomLibraryAttribute.FirstOrDefault(x =>
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
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString(), x => x.tablesNames).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (DateTimeLibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeLibDynamicAttListIds.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                        DynamicLibValueListIds = new List<int>();

                        List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            DateTimeLibDynamicAttListIds.Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Select(x => x.ValueDateTime).Any(x =>
                                 (x != null ?
                                    (x >= y.DateFrom && x <= y.DateTo) : (false)))))
                            {
                                DynamicLibValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Library Attribute Activated...
                    //
                    List<PropertyInfo> LibraryProps = typeof(CabinetTelecomLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIcabinetTelecomLibrary> Libraries = _unitOfWork.CabinetTelecomLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CabinetTelecomLibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<CabinetTelecomLibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<CabinetTelecomLibraryViewModel>(x), null)))) : (false))));
                        }

                        LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
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

                    WithDateFilterCabinetTelecomLibraries = _mapper.Map<List<CabinetTelecomLibraryViewModel>>(_unitOfWork.CabinetTelecomLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.TelecomType).ToList());
                }

                //
                // Intersect Between WithoutDateFilterCabinetTelecomLibraries + WithDateFilterCabinetTelecomLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    CabinetTelecomLibraries = _mapper.Map<List<CabinetTelecomLibraryViewModel>>(_unitOfWork.CabinetTelecomLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted, x => x.TelecomType).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> CabinetTelecomIds = WithoutDateFilterCabinetTelecomLibraries.Select(x => x.Id).Intersect(WithDateFilterCabinetTelecomLibraries.Select(x => x.Id)).ToList();
                    CabinetTelecomLibraries = _mapper.Map<List<CabinetTelecomLibraryViewModel>>(_unitOfWork.CabinetTelecomLibraryRepository.GetWhere(x =>
                        CabinetTelecomIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    CabinetTelecomLibraries = WithoutDateFilterCabinetTelecomLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    CabinetTelecomLibraries = WithDateFilterCabinetTelecomLibraries;
                }

                Count = CabinetTelecomLibraries.Count();

                CabinetTelecomLibraries = CabinetTelecomLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CabinetTelecomLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString()) : false),
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

                foreach (CabinetTelecomLibraryViewModel CabinetTelecomLibraryViewModel in CabinetTelecomLibraries)
                {
                    dynamic DynamicCabinetTelecomLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(CabinetTelecomLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in LibraryProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeLibraryAttributesViewModel.Select(x =>
                                    x.AttributeActivated.Label.ToLower()).Contains(prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(CabinetTelecomLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicCabinetTelecomLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(CabinetTelecomLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicCabinetTelecomLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(CabinetTelecomLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicCabinetTelecomLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    //                   
                    var temp = NotDateTimeDynamicLibraryAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        temp.Any(y => y == x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == CabinetTelecomLibraryViewModel.Id && !x.disable &&
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

                            ((IDictionary<String, Object>)DynamicCabinetTelecomLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicCabinetTelecomLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(CabinetTelecomLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(CabinetTelecomLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    temp = DateTimeDynamicLibraryAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        temp.Any(y => y == x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == CabinetTelecomLibraryViewModel.Id && !x.disable &&
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

                    ((IDictionary<String, Object>)DynamicCabinetTelecomLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicCabinetTelecomLibrary);
                }

                CabinetTelecomTableDisplay.Model = OutPutList;

                if (WithFilterData)
                    CabinetTelecomTableDisplay.filters = _unitOfWork.CabinetTelecomLibraryRepository.GetRelatedTables();
                else
                    CabinetTelecomTableDisplay.filters = null;

                return new Response<ReturnWithFilters<object>>(true, CabinetTelecomTableDisplay, null, null, (int)ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetSolarLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> SolarTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<SolarLibraryViewModel> SolarLibraries = new List<SolarLibraryViewModel>();
                List<SolarLibraryViewModel> WithoutDateFilterSolarLibraries = new List<SolarLibraryViewModel>();
                List<SolarLibraryViewModel> WithDateFilterSolarLibraries = new List<SolarLibraryViewModel>();

                List<TLIattributeActivated> SolarLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    SolarLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.SolarLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateSolarLibraryAttribute = SolarLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateSolarLibraryAttribute.FirstOrDefault(x =>
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
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

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
                    bool AttrLibExist = typeof(SolarLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(SolarLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(SolarLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIsolarLibrary> Libraries = _unitOfWork.SolarLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<SolarLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<SolarLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<SolarLibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<SolarLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
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

                    WithoutDateFilterSolarLibraries = _mapper.Map<List<SolarLibraryViewModel>>(_unitOfWork.SolarLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.Capacity).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateSolarLibraryAttribute = SolarLibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = DateSolarLibraryAttribute.FirstOrDefault(x =>
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
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString(), x => x.tablesNames).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (DateTimeLibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeLibDynamicAttListIds.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                        DynamicLibValueListIds = new List<int>();

                        List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            DateTimeLibDynamicAttListIds.Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Select(x => x.ValueDateTime).Any(x =>
                                 (x != null ?
                                    (x >= y.DateFrom && x <= y.DateTo) : (false)))))
                            {
                                DynamicLibValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Library Attribute Activated...
                    //
                    List<PropertyInfo> LibraryProps = typeof(SolarLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIsolarLibrary> Libraries = _unitOfWork.SolarLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<SolarLibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<SolarLibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<SolarLibraryViewModel>(x), null)))) : (false))));
                        }

                        LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
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

                    WithDateFilterSolarLibraries = _mapper.Map<List<SolarLibraryViewModel>>(_unitOfWork.SolarLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.Capacity).ToList());
                }

                //
                // Intersect Between WithoutDateFilterSolarLibraries + WithDateFilterSolarLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    SolarLibraries = _mapper.Map<List<SolarLibraryViewModel>>(_unitOfWork.SolarLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted, x => x.Capacity).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> SolarIds = WithoutDateFilterSolarLibraries.Select(x => x.Id).Intersect(WithDateFilterSolarLibraries.Select(x => x.Id)).ToList();
                    SolarLibraries = _mapper.Map<List<SolarLibraryViewModel>>(_unitOfWork.SolarLibraryRepository.GetWhere(x =>
                        SolarIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    SolarLibraries = WithoutDateFilterSolarLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    SolarLibraries = WithDateFilterSolarLibraries;
                }

                Count = SolarLibraries.Count();

                SolarLibraries = SolarLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.SolarLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString()) : false),
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

                foreach (SolarLibraryViewModel SolarLibraryViewModel in SolarLibraries)
                {
                    dynamic DynamicSolarLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(SolarLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in LibraryProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeLibraryAttributesViewModel.Select(x =>
                                    x.AttributeActivated.Label.ToLower()).Contains(prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(SolarLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicSolarLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(SolarLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicSolarLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(SolarLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicSolarLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    var temp = NotDateTimeDynamicLibraryAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        temp.Any(y => y == x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == SolarLibraryViewModel.Id && !x.disable &&
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

                            ((IDictionary<String, Object>)DynamicSolarLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicSolarLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(SolarLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(SolarLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    temp = DateTimeDynamicLibraryAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        temp.Any(y => y == x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == SolarLibraryViewModel.Id && !x.disable &&
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

                    ((IDictionary<String, Object>)DynamicSolarLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicSolarLibrary);
                }

                SolarTableDisplay.Model = OutPutList;

                if (WithFilterData)
                    SolarTableDisplay.filters = _unitOfWork.SolarLibraryRepository.GetRelatedTables();
                else
                    SolarTableDisplay.filters = null;

                return new Response<ReturnWithFilters<object>>(true, SolarTableDisplay, null, null, (int)ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetGeneratorLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> GeneratorTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<GeneratorLibraryViewModel> GeneratorLibraries = new List<GeneratorLibraryViewModel>();
                List<GeneratorLibraryViewModel> WithoutDateFilterGeneratorLibraries = new List<GeneratorLibraryViewModel>();
                List<GeneratorLibraryViewModel> WithDateFilterGeneratorLibraries = new List<GeneratorLibraryViewModel>();

                List<TLIattributeActivated> GeneratorLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    GeneratorLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.GeneratorLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateGeneratorLibraryAttribute = GeneratorLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateGeneratorLibraryAttribute.FirstOrDefault(x =>
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
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

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
                    bool AttrLibExist = typeof(GeneratorLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(GeneratorLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(GeneratorLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIgeneratorLibrary> Libraries = _unitOfWork.GeneratorLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<GeneratorLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<GeneratorLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<GeneratorLibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<GeneratorLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
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

                    WithoutDateFilterGeneratorLibraries = _mapper.Map<List<GeneratorLibraryViewModel>>(_unitOfWork.GeneratorLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.Capacity).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateGeneratorLibraryAttribute = GeneratorLibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = DateGeneratorLibraryAttribute.FirstOrDefault(x =>
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
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString(), x => x.tablesNames).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (DateTimeLibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeLibDynamicAttListIds.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                        DynamicLibValueListIds = new List<int>();

                        List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            DateTimeLibDynamicAttListIds.Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Select(x => x.ValueDateTime).Any(x =>
                                 (x != null ?
                                    (x >= y.DateFrom && x <= y.DateTo) : (false)))))
                            {
                                DynamicLibValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Library Attribute Activated...
                    //
                    List<PropertyInfo> LibraryProps = typeof(GeneratorLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIgeneratorLibrary> Libraries = _unitOfWork.GeneratorLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<GeneratorLibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<GeneratorLibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<GeneratorLibraryViewModel>(x), null)))) : (false))));
                        }

                        LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
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

                    WithDateFilterGeneratorLibraries = _mapper.Map<List<GeneratorLibraryViewModel>>(_unitOfWork.GeneratorLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.Capacity).ToList());
                }

                //
                // Intersect Between WithoutDateFilterGeneratorLibraries + WithDateFilterGeneratorLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    GeneratorLibraries = _mapper.Map<List<GeneratorLibraryViewModel>>(_unitOfWork.GeneratorLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted, x => x.Capacity).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> GeneratorIds = WithoutDateFilterGeneratorLibraries.Select(x => x.Id).Intersect(WithDateFilterGeneratorLibraries.Select(x => x.Id)).ToList();
                    GeneratorLibraries = _mapper.Map<List<GeneratorLibraryViewModel>>(_unitOfWork.GeneratorLibraryRepository.GetWhere(x =>
                        GeneratorIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    GeneratorLibraries = WithoutDateFilterGeneratorLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    GeneratorLibraries = WithDateFilterGeneratorLibraries;
                }

                Count = GeneratorLibraries.Count();

                GeneratorLibraries = GeneratorLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.GeneratorLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString()) : false),
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

                foreach (GeneratorLibraryViewModel GeneratorLibraryViewModel in GeneratorLibraries)
                {
                    dynamic DynamicGeneratorLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(GeneratorLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in LibraryProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeLibraryAttributesViewModel.Select(x =>
                                    x.AttributeActivated.Label.ToLower()).Contains(prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(GeneratorLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicGeneratorLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(GeneratorLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicGeneratorLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(GeneratorLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicGeneratorLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    var temp = NotDateTimeDynamicLibraryAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        temp.Any(y => y == x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == GeneratorLibraryViewModel.Id && !x.disable &&
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

                            ((IDictionary<String, Object>)DynamicGeneratorLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicGeneratorLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(GeneratorLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(GeneratorLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    temp= DateTimeDynamicLibraryAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        temp.Any(y => y == x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == GeneratorLibraryViewModel.Id && !x.disable &&
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

                    ((IDictionary<String, Object>)DynamicGeneratorLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicGeneratorLibrary);
                }

                GeneratorTableDisplay.Model = OutPutList;

                if (WithFilterData)
                    GeneratorTableDisplay.filters = _unitOfWork.GeneratorLibraryRepository.GetRelatedTables();
                else
                    GeneratorTableDisplay.filters = null;

                return new Response<ReturnWithFilters<object>>(true, GeneratorTableDisplay, null, null, (int)ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<GetEnableAttribute> GetGeneratorLibrariesEnabledAtt(string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    //string storedProcedureName = "create_dynamic_pivot_withleg_library ";
                    //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    //{
                    //    procedureCommand.CommandType = CommandType.StoredProcedure;
                    //    procedureCommand.ExecuteNonQuery();
                    //}
                    var attActivated = db.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "GeneratorLibrary"
                        && ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
                        .Select(x => new { attribute = x.AttributeActivated.Key, dynamic = x.DynamicAtt.Key, dataType = x.DynamicAtt != null ? x.DynamicAtt.DataType.Name.ToString() : x.AttributeActivated.DataType.ToString() })
                          .OrderByDescending(x => x.attribute.ToLower().StartsWith("model"))
                            .ThenBy(x => x.attribute == null)
                            .ThenBy(x => x.attribute)
                            .ToList();
                    getEnableAttribute.Type = attActivated;
                    List<string> propertyNamesStatic = new List<string>();
                    Dictionary<string, string> propertyNamesDynamic = new Dictionary<string, string>();
                    foreach (var key in attActivated)
                    {
                        if (key.attribute != null)
                        {
                            string name = key.attribute;
                            if (name != "Id" && name.EndsWith("Id"))
                            {
                                string fk = name.Remove(name.Length - 2);
                                propertyNamesStatic.Add(fk);
                            }
                            else
                            {
                                propertyNamesStatic.Add(name);
                            }

                        }
                        else
                        {
                            string name = key.dynamic;
                            string datatype = key.dataType;
                            propertyNamesDynamic.Add(name, datatype);
                        }

                    }
                    if (propertyNamesDynamic.Count == 0)
                    {
                        var query = db.MV_GENERATOR_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();

                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = db.MV_GENERATOR_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                    .GroupBy(x => new
                    {
                        Id = x.Id,
                        Model = x.Model,
                        Width = x.Width,
                        Weight = x.Weight,
                        Length = x.Length,
                        LayoutCode = x.LayoutCode,
                        Height = x.Height,
                        Active = x.Active,
                        Deleted = x.Deleted,
                        SpaceLibrary = x.SpaceLibrary,
                        CAPACITY = x.CAPACITY
                       

                    }).OrderBy(x => x.Key.Model)
                    .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();

                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }

                }
                catch (Exception err)
                {
                    return new Response<GetEnableAttribute>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        public Response<AddGeneratorLibraryObject> AddGenertatoLibrary(int UserId, string TableName, AddGeneratorLibraryObject addGeneratorLibraryObject, string connectionString)
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
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == TableName);

                            TLIgeneratorLibrary GeneratorLibraryEntity = _mapper.Map<TLIgeneratorLibrary>(addGeneratorLibraryObject.AttributesActivatedLibrary);
                            if (GeneratorLibraryEntity.SpaceLibrary <= 0)
                            {
                                if (GeneratorLibraryEntity.Height <= 0)
                                {
                                    return new Response<AddGeneratorLibraryObject>(false, null, null, "Height must bigger of zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                else if (GeneratorLibraryEntity.Width <= 0)
                                {
                                    return new Response<AddGeneratorLibraryObject>(false, null, null, "Width must bigger of zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                else
                                {
                                    GeneratorLibraryEntity.SpaceLibrary = GeneratorLibraryEntity.Height * GeneratorLibraryEntity.Width;
                                }
                            }
                            //string CheckDependencyValidation = CheckDependencyValidationForMWTypes(addMWDishLibraryObject, TableName);

                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //    return new Response<AddMWDishLibraryObject>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                            //string CheckGeneralValidation = CheckGeneralValidationFunctionLib(addMWDishLibraryObject.dynamicAttribute, TableNameEntity.TableName);

                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //    return new Response<AddMWDishLibraryObject>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                             var CheckModel = db.MV_GENERATOR_LIBRARY_VIEW
                               .FirstOrDefault(x => x.Model != null &&
                                x.Model.ToLower() == GeneratorLibraryEntity.Model.ToLower()
                                && !x.Deleted);

                            if (CheckModel != null)
                            {
                                return new Response<AddGeneratorLibraryObject>(true, null, null, $"This model {GeneratorLibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                            _unitOfWork.GeneratorLibraryRepository.AddWithHistory(UserId, GeneratorLibraryEntity);
                            _unitOfWork.SaveChanges();

                            dynamic LogisticalItemIds = new ExpandoObject();
                            LogisticalItemIds = addGeneratorLibraryObject.LogisticalItems;

                            AddLogisticalItemWithCivil(UserId, LogisticalItemIds, GeneratorLibraryEntity, TableNameEntity.Id);

                            if (addGeneratorLibraryObject.DynamicAttributes.Count > 0)
                            {
                                _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtt(UserId, addGeneratorLibraryObject.DynamicAttributes, TableNameEntity.Id, GeneratorLibraryEntity.Id, connectionString);
                            }
                            _unitOfWork.TablesHistoryRepository.AddHistory(GeneratorLibraryEntity.Id, Helpers.Constants.HistoryType.Add.ToString().ToLower(), TablesNames.TLImwDishLibrary.ToString().ToLower());



                            transaction.Complete();
                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                            return new Response<AddGeneratorLibraryObject>();
                        }
                        catch (Exception err)
                        {
                            return new Response<AddGeneratorLibraryObject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }
            }
        }
        public void AddLogisticalItemWithCivil(int UserId, dynamic LogisticalItemIds, dynamic MWLibraryEntity, int TableNameEntityId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (LogisticalItemIds != null)
                    {
                        if (LogisticalItemIds.Vendor != null && LogisticalItemIds.Vendor != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Vendor);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.Supplier != null && LogisticalItemIds.Supplier != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Supplier);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.Designer != null && LogisticalItemIds.Designer != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Designer);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.Manufacturer != null && LogisticalItemIds.Manufacturer != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Manufacturer);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.Contractor != null && LogisticalItemIds.Contractor != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Contractor);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }

                        }
                        if (LogisticalItemIds.Consultant != null && LogisticalItemIds.Consultant != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Consultant);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
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
        //#region Add History
        //public void AddHistory(int Other_Inventory_Lib_Id, string historyType, string TableName)
        //{

        //    AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
        //    history.RecordId = Other_Inventory_Lib_Id;
        //    history.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableName).Id;
        //    history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
        //    history.UserId = 83;
        //    _unitOfWork.TablesHistoryRepository.AddTableHistory(history);

        //}
        //#endregion
    }
}
