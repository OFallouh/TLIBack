using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using AutoMapper;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using Org.BouncyCastle.Asn1.Cms;
using TLIS_DAL.ViewModels.SectionsLegTypeDTOs;
using TLIS_DAL.ViewModels.StructureTypeDTOs;
using TLIS_DAL.ViewModels.SupportTypeDesignedDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using static TLIS_DAL.ViewModels.SideArmLibraryDTOs.EditSideArmLibraryObject;
using TLIS_DAL;

namespace TLIS_Service.Services
{
    public class SideArmLibraryService : ISideArmLibraryService
    {
        private readonly IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        ApplicationDbContext db;
        public SideArmLibraryService(IUnitOfWork unitOfWork, ApplicationDbContext _context, IServiceCollection services, IMapper mapper)
        {
            db = _context;
            _unitOfWork = unitOfWork;
            _services = services;
            _mapper = mapper;
        }
        //Function take 1 parameter
        //map ViewModel to Entity
        //check validation
        //add Entity
        //add dynamic library attributes values
        public void SeedDataForTest()
        {
            try
            {
                List<TLIsideArmLibrary> SeedData = new List<TLIsideArmLibrary>
                {
                    new TLIsideArmLibrary {
                        Id = 1,
                        Model = "SideArmLibrary1",
                        Width = 1,
                        Weight = 1,
                        Length = 1,
                        Height = 1,
                        SpaceLibrary = 1,
                        Note = "1",
                        Active = true,
                        Deleted = false
                    },
                    new TLIsideArmLibrary {
                        Id = 2,
                        Model = "SideArmLibrary1",
                        Width = 2,
                        Weight = 2,
                        Length = 2,
                        Height = 2,
                        SpaceLibrary = 2,
                        Note = "2",
                        Active = true,
                        Deleted = false
                    },
                    new TLIsideArmLibrary {
                        Id = 3,
                        Model = "SideArmLibrary3",
                        Width = 3,
                        Weight = 3,
                        Length = 3,
                        Height = 3,
                        SpaceLibrary = 3,
                        Note = "3",
                        Active = true,
                        Deleted = false
                    },
                    new TLIsideArmLibrary {
                        Id = 4,
                        Model = "SideArmLibrary4",
                        Width = 4,
                        Weight = 4,
                        Length = 4,
                        Height = 4,
                        SpaceLibrary = 4,
                        Note = "4",
                        Active = true,
                        Deleted = false
                    },
                    new TLIsideArmLibrary {
                        Id = 5,
                        Model = "SideArmLibrary5",
                        Width = 5,
                        Weight = 5,
                        Length = 5,
                        Height = 5,
                        SpaceLibrary = 5,
                        Note = "5",
                        Active = true,
                        Deleted = false
                    },
                };
                foreach (TLIsideArmLibrary item in SeedData)
                {
                    _unitOfWork.SideArmLibraryRepository.Add(item);
                }
                _unitOfWork.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Response<AddSideArmLibraryObject> AddSideArmLibrary(AddSideArmLibraryObject addSideArmLibraryViewModel, string connectionString,int UserId)
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
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLIsideArmLibrary.ToString().ToLower());
                            TLIsideArmLibrary tLIsideArmLibrary = _mapper.Map<TLIsideArmLibrary>(addSideArmLibraryViewModel.LibraryAttribute);
                            var CheckModel = _unitOfWork.SideArmLibraryRepository.GetWhereFirst(x => x.Model == tLIsideArmLibrary.Model && !x.Deleted);
                            if (CheckModel != null)
                            {
                                return new Response<AddSideArmLibraryObject>(false, null, null, $"This Model {tLIsideArmLibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                            if(tLIsideArmLibrary.SpaceLibrary== 0)
                            {
                                if(tLIsideArmLibrary .Length <= 0)
                                {
                                    return new Response<AddSideArmLibraryObject>(false, null, null, $"must length bigger from zero or must input spacelibrary", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                if (tLIsideArmLibrary.Width <= 0)
                                {
                                    return new Response<AddSideArmLibraryObject>(false, null, null, $"must width bigger from zero or must input spacelibrary", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                else
                                {
                                    var SpaceLibrary = tLIsideArmLibrary.Length * tLIsideArmLibrary.Width;
                                }
                            }
                            //string CheckDependencyValidation = CheckDependencyValidationForSideArm(addSideArmLibraryViewModel);

                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //    return new Response<AddSideArmLibraryObject>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                            //string CheckGeneralValidation = CheckGeneralValidationFunctionLib(addSideArmLibraryViewModel.dynamicAttribute, TableNameEntity.TableName);

                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //    return new Response<AddSideArmLibraryObject>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                            _unitOfWork.SideArmLibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, tLIsideArmLibrary);
                            _unitOfWork.SaveChanges();

                            dynamic LogisticalItemIds = new ExpandoObject();
                            LogisticalItemIds = addSideArmLibraryViewModel.LogisticalItems;

                            AddLogisticalItemWithSideArms(UserId,LogisticalItemIds, tLIsideArmLibrary, TableNameEntity.Id);

                            if (addSideArmLibraryViewModel.dynamicAttribute.Count > 0)
                            {
                                _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtt(UserId,addSideArmLibraryViewModel.dynamicAttribute, TableNameEntity.Id, tLIsideArmLibrary.Id);
                            }
                            transaction.Complete();
                            tran.Commit();
                            return new Response<AddSideArmLibraryObject>();
                        }
                        catch (Exception err)
                        {

                            tran.Rollback();
                            return new Response<AddSideArmLibraryObject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }
            }
        }
        #region Helper Methods
        public string CheckDependencyValidationForSideArm(object Input)
        {
            AddSideArmLibraryViewModel AddSideArmLibraryViewModel = _mapper.Map<AddSideArmLibraryViewModel>(Input);

            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TablesNames.TLIsideArmLibrary.ToString().ToLower() && !x.disable
                    , x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
            {
                TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                    x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                        x => x.Operation, x => x.DynamicAtt);

                if (Dependency != null)
                {
                    AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddSideArmLibraryViewModel.TLIdynamicAttLibValue
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

                                InsertedValue = AddSideArmLibraryViewModel.GetType().GetProperties()
                                    .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddSideArmLibraryViewModel, null);

                                if (InsertedValue == null)
                                    break;
                            }
                            else if (Rule.dynamicAttId != null)
                            {
                                AddDynamicLibAttValueViewModel DynamicObject = AddSideArmLibraryViewModel.TLIdynamicAttLibValue
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

                            if (RuleOperation == "==" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 0 :
                                RuleOperation == "!=" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) != 0 :
                                RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                    Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 0) :
                                RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                    Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 0) : false)
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
                                        DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                        DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                            Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) :
                                        DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                        DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                            Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) : false))
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
        public string CheckGeneralValidationFunctionLib(List<AddDdynamicAttributeInstallationValueViewModel> TLIdynamicAttLibValue, string TableName)
        {
            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable
                    , x => x.tablesNames).ToList());

            var invalidValidation = DynamicAttributes.Select(DynamicAttributeEntity =>
            {
                var Validation = _unitOfWork.ValidationRepository
                    .GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttributeEntity.Id, x => x.Operation, x => x.DynamicAtt);

                if (Validation != null)
                {
                    var DynmaicAttributeValue = TLIdynamicAttLibValue.FirstOrDefault(x => x.id == DynamicAttributeEntity.Id);

                    if (DynmaicAttributeValue == null)
                        return $"({Validation.DynamicAtt.Key}) value can't be null and must be inserted";

                    var OperationName = Validation.Operation.Name;

                    var InputDynamicValue = DynmaicAttributeValue.value;
                    var ValidationValue = Validation.ValueBoolean ?? Validation.ValueDateTime ?? Validation.ValueDouble ?? (object)Validation.ValueString;

                    if (!(OperationName == "==" ? InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower() :
                        OperationName == "!=" ? InputDynamicValue.ToString().ToLower() != ValidationValue.ToString().ToLower() :
                        OperationName == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 :
                        OperationName == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) :
                        OperationName == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 :
                        OperationName == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) : false))
                    {
                        var DynamicAttributeName = _unitOfWork.DynamicAttRepository
                            .GetWhereFirst(x => x.Id == Validation.DynamicAttId).Key;

                        var ReturnOperation = (OperationName == "==" ? "equal to" :
                            (OperationName == "!=" ? "not equal to" :
                            (OperationName == ">" ? "bigger than" :
                            (OperationName == ">=" ? "bigger than or equal to" :
                            (OperationName == "<" ? "smaller than" :
                            (OperationName == "<=" ? "smaller than or equal to" : ""))))));

                        return $"({DynamicAttributeName}) value must be {ReturnOperation} {ValidationValue}";
                    }
                }
                return null;
            }).FirstOrDefault(invalidValidation => invalidValidation != null);

            return invalidValidation ?? string.Empty;
        }
        public void AddLogisticalItemWithSideArm(dynamic LogisticalItemIds, dynamic SideArmEntity, int TableNameEntityId)
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
                                RecordId = SideArmEntity.Id,
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
                                RecordId = SideArmEntity.Id,
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
                                RecordId = SideArmEntity.Id,
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
                                RecordId = SideArmEntity.Id,
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
        public void AddLogisticalItemWithSideArms(int UserId,dynamic LogisticalItemIds, dynamic SideArmEntity, int TableNameEntityId)
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
                                    RecordId = SideArmEntity.Id,
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
                                    RecordId = SideArmEntity.Id,
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
                                    RecordId = SideArmEntity.Id,
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
                                    RecordId = SideArmEntity.Id,
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
                                    RecordId = SideArmEntity.Id,
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
                                    RecordId = SideArmEntity.Id,
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
        //Function take 1 parameter
        //get record by Id
        //set Deleted is true
        //update Entity
        public async Task<Response<SideArmLibraryViewModel>> Delete(int id)
        {
            try
            {
                TLIsideArmLibrary OldSideWithArm = _unitOfWork.SideArmLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == id);

                TLIsideArmLibrary NewSideWithArm = _unitOfWork.SideArmLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == id);
                NewSideWithArm.Deleted = true;
                NewSideWithArm.Model = NewSideWithArm.Model + "_" + DateTime.Now.ToString();

                _unitOfWork.SideArmLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldSideWithArm, NewSideWithArm);
                await _unitOfWork.SaveChangesAsync();
                return new Response<SideArmLibraryViewModel>();
            }
            catch (Exception err)
            {
                return new Response<SideArmLibraryViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter
        //get record by Id
        //enable or disable record depened on record status
        //update Entity
        public async Task<Response<SideArmLibraryViewModel>> Disable(int id)
        {
            try
            {
                TLIsideArmLibrary OldSideWithArm = _unitOfWork.SideArmLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == id);

                TLIsideArmLibrary NewSideWithArm = _unitOfWork.SideArmLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == id);
                NewSideWithArm.Active = !(NewSideWithArm.Active);

                _unitOfWork.SideArmLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldSideWithArm, NewSideWithArm);
                await _unitOfWork.SaveChangesAsync();
                return new Response<SideArmLibraryViewModel>();
            }
            catch (Exception err)
            {
                return new Response<SideArmLibraryViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter
        //map ViewModel to Entity
        //check validation
        //update Entity
        //update dynamic library attributes values
        public async Task<Response<EditSideArmLibraryObject>> EditSideArmLibrary(EditSideArmLibraryObject editSideArmLibraryViewModel,int UserId)
        {
            try
            {
                int resultId = 0;
                var TableNames = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLIsideArmLibrary.ToString().ToLower());
                TLIsideArmLibrary tLIsideArmLibrary = _mapper.Map<TLIsideArmLibrary>(editSideArmLibraryViewModel.attributesActivatedLibrary);
                var CheckModel = _unitOfWork.SideArmLibraryRepository.GetWhereFirst(x => x.Model == tLIsideArmLibrary.Model && x.Id != tLIsideArmLibrary.Id);
                var SidArm = _unitOfWork.SideArmLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == tLIsideArmLibrary.Id);
                if (CheckModel != null)
                {
                    return new Response<EditSideArmLibraryObject>(true, null, null, $"This Model {tLIsideArmLibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                if (tLIsideArmLibrary.SpaceLibrary == 0)
                {
                    if (tLIsideArmLibrary.Length <= 0)
                    {
                        return new Response<EditSideArmLibraryObject>(false, null, null, $"must length bigger from zero or must input spacelibrary", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    if (tLIsideArmLibrary.Width <= 0)
                    {
                        return new Response<EditSideArmLibraryObject>(false, null, null, $"must width bigger from zero or must input spacelibrary", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        var SpaceLibrary = tLIsideArmLibrary.Length * tLIsideArmLibrary.Width;
                    }
                }
                _unitOfWork.SideArmLibraryRepository.UpdateWithHistory(UserId, SidArm, tLIsideArmLibrary);
                //string CheckDependency = CheckDependencyValidationEditApiVersion(editSideArmLibraryViewModel);
                //if (!string.IsNullOrEmpty(CheckDependency))
                //{
                //    return new Response<EditSideArmLibraryObject>(true, null, null, CheckDependency, (int)Helpers.Constants.ApiReturnCode.fail);
                //}

                //string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersion(editSideArmLibraryViewModel.dynamicAttributes, TableNames.TableName);
                //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                //{
                //    return new Response<EditSideArmLibraryObject>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                //}

                LogisticalObject OldLogisticalItemIds = new LogisticalObject();

                var CheckVendorId = _unitOfWork.LogisticalitemRepository
                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                        x.IsLib && x.tablesNamesId == TableNames.Id && x.RecordId == tLIsideArmLibrary.Id, x => x.logistical,
                            x => x.logistical.logisticalType);

                if (CheckVendorId != null)
                    OldLogisticalItemIds.Vendor = CheckVendorId.logisticalId;

                else
                    OldLogisticalItemIds.Vendor = 0;

                var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                        x.IsLib && x.tablesNamesId == TableNames.Id && x.RecordId == tLIsideArmLibrary.Id, x => x.logistical,
                            x => x.logistical.logisticalType);

                if (CheckSupplierId != null)
                    OldLogisticalItemIds.Supplier = CheckSupplierId.logisticalId;

                else
                    OldLogisticalItemIds.Supplier = 0;

                var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                        x.IsLib && x.tablesNamesId == TableNames.Id && x.RecordId == tLIsideArmLibrary.Id, x => x.logistical,
                            x => x.logistical.logisticalType);

                if (CheckDesignerId != null)
                    OldLogisticalItemIds.Designer = CheckDesignerId.logisticalId;

                else
                    OldLogisticalItemIds.Designer = 0;

                var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == LogisticalType.Manufacturer.ToString().ToLower() &&
                        x.IsLib && x.tablesNamesId == TableNames.Id && x.RecordId == tLIsideArmLibrary.Id, x => x.logistical,
                            x => x.logistical.logisticalType);

                if (CheckManufacturerId != null)
                    OldLogisticalItemIds.Manufacturer = CheckManufacturerId.logisticalId;

                else
                    OldLogisticalItemIds.Manufacturer = 0;


                var CheckContractorId = _unitOfWork.LogisticalitemRepository
                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == LogisticalType.Contractor.ToString().ToLower() &&
                        x.IsLib && x.tablesNamesId == TableNames.Id && x.RecordId == tLIsideArmLibrary.Id, x => x.logistical,
                            x => x.logistical.logisticalType);

                if (CheckContractorId != null)
                    OldLogisticalItemIds.Contractor = CheckContractorId.logisticalId;

                else
                    OldLogisticalItemIds.Contractor = 0;

                var CheckConsultantId = _unitOfWork.LogisticalitemRepository
                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == LogisticalType.Consultant.ToString().ToLower() &&
                        x.IsLib && x.tablesNamesId == TableNames.Id && x.RecordId == tLIsideArmLibrary.Id, x => x.logistical,
                            x => x.logistical.logisticalType);

                if (CheckConsultantId != null)
                    OldLogisticalItemIds.Consultant = CheckConsultantId.logisticalId;

                else
                    OldLogisticalItemIds.Consultant = 0;

                EditLogisticalItems(UserId, editSideArmLibraryViewModel.logisticalItems, tLIsideArmLibrary, TableNames.Id, OldLogisticalItemIds);

                if (editSideArmLibraryViewModel.dynamicAttributes.Count > 0)
                {
                    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistorys(editSideArmLibraryViewModel.dynamicAttributes, TableNames.Id, tLIsideArmLibrary.Id, UserId, resultId, SidArm.Id);
                }
                await _unitOfWork.SaveChangesAsync();
                return new Response<EditSideArmLibraryObject>();
            }
            catch (Exception err)
            {
                return new Response<EditSideArmLibraryObject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
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
        public void EditLogisticalItems(int UserId, LogisticalObject LogisticalItemIds, dynamic MainEntity, int TableNameEntityId, LogisticalObject OldLogisticalItemIds)
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
                                    .GetWhereFirst(x => x.Id == LogisticalItemIds.Vendor);

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
                                    .GetWhereFirst(x=>x.Id==LogisticalItemIds.Vendor);

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
        public string CheckDependencyValidationEditApiVersion(object Input)
        {
            EditSideArmLibraryViewModel EditSideArmLibraryViewModels = _mapper.Map<EditSideArmLibraryViewModel>(Input);

            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString().ToLower() && !x.disable
                    , x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
            {
                TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                    x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                        x => x.Operation, x => x.DynamicAtt);

                if (Dependency != null)
                {
                    DynamicAttLibViewModel InsertedDynamicAttributeValue = EditSideArmLibraryViewModels.DynamicAtts
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

                                PropertyInfo PropertyInfoinType = EditSideArmLibraryViewModels.GetType().GetProperties()
                                    .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower());

                                if (PropertyInfoinType != null)
                                    InsertedValue = PropertyInfoinType.GetValue(EditSideArmLibraryViewModels, null);

                                else
                                    break;
                            }
                            else if (Rule.dynamicAttId != null)
                            {
                                DynamicAttLibViewModel DynamicObject = EditSideArmLibraryViewModels.DynamicAtts
                                    .FirstOrDefault(x => x.Key.ToLower() == Rule.dynamicAtt.Key.ToLower());

                                if (DynamicObject != null)
                                    InsertedValue = DynamicObject.Value;

                                else
                                    break;
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
        //Function take 2 parameters
        //Function return all records depened on filters and parameters
        public async Task<Response<IEnumerable<SideArmLibraryViewModel>>> GetSideArmLibraries(List<FilterObjectList> filters, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                var sideWithArm = await _unitOfWork.SideArmLibraryRepository.GetAllIncludeMultiple(parameters, filters, out count, null).AsNoTracking().Where(s => s.Deleted == false && s.Active == true).OrderBy(x => x.Id).ToListAsync();
                var sideWithArmViewModel = _mapper.Map<IEnumerable<SideArmLibraryViewModel>>(sideWithArm);
                return new Response<IEnumerable<SideArmLibraryViewModel>>(true, sideWithArmViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, sideWithArmViewModel.Count());
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<SideArmLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        #region Get Enabled Attributes Only With Dynamic Objects (Libraries Only)...
        #region Helper Methods
        public void GetInventoriesIdsFromDynamicAttributes(out List<int> DynamicLibValueListIds, List<TLIdynamicAtt> LibDynamicAttListIds, List<StringFilterObjectList> AttributeFilters)
        {
            try
            {
                List<StringFilterObjectList> DynamicLibAttributeFilters = AttributeFilters.Where(x =>
                    LibDynamicAttListIds.Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

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
        public Response<ReturnWithFilters<object>> GetSideArmLibrariesWithEnabledAttributes(CombineFilters CombineFilters, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> SideArmTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<SideArmLibraryViewModel> SideArmLibraries = new List<SideArmLibraryViewModel>();
                List<SideArmLibraryViewModel> WithoutDateFilterSideArmLibraries = new List<SideArmLibraryViewModel>();
                List<SideArmLibraryViewModel> WithDateFilterSideArmLibraries = new List<SideArmLibraryViewModel>();

                List<TLIattributeActivated> SideArmLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    SideArmLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.SideArmLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == TablesNames.TLIsideArmLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateSideArmLibraryAttribute = SideArmLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateSideArmLibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == TablesNames.TLIsideArmLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

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
                    bool AttrLibExist = typeof(SideArmLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(SideArmLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(SideArmLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIsideArmLibrary> Libraries = _unitOfWork.SideArmLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<SideArmLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<SideArmLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<SideArmLibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<SideArmLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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

                    WithoutDateFilterSideArmLibraries = _mapper.Map<List<SideArmLibraryViewModel>>(_unitOfWork.SideArmLibraryRepository.GetWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateSideArmLibraryAttribute = SideArmLibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = DateSideArmLibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == TablesNames.TLIsideArmLibrary.ToString(), x => x.tablesNames).ToList();

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
                    List<PropertyInfo> LibraryProps = typeof(SideArmLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIsideArmLibrary> Libraries = _unitOfWork.SideArmLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<SideArmLibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<SideArmLibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<SideArmLibraryViewModel>(x), null)))) : (false))));
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

                    WithDateFilterSideArmLibraries = _mapper.Map<List<SideArmLibraryViewModel>>(_unitOfWork.SideArmLibraryRepository.GetWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // Intersect Between WithoutDateFilterSideArmLibraries + WithDateFilterSideArmLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    SideArmLibraries = _mapper.Map<List<SideArmLibraryViewModel>>(_unitOfWork.SideArmLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> SideArmIds = WithoutDateFilterSideArmLibraries.Select(x => x.Id).Intersect(WithDateFilterSideArmLibraries.Select(x => x.Id)).ToList();
                    SideArmLibraries = _mapper.Map<List<SideArmLibraryViewModel>>(_unitOfWork.SideArmLibraryRepository.GetWhere(x =>
                        SideArmIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    SideArmLibraries = WithoutDateFilterSideArmLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    SideArmLibraries = WithDateFilterSideArmLibraries;
                }

                Count = SideArmLibraries.Count();

                SideArmLibraries = SideArmLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.SideArmLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == TablesNames.TLIsideArmLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == TablesNames.TLIsideArmLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == TablesNames.TLIsideArmLibrary.ToString()) : false),
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

                foreach (SideArmLibraryViewModel SideArmLibraryViewModel in SideArmLibraries)
                {
                    dynamic DynamicSideArmLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(SideArmLibraryViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(SideArmLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicSideArmLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == TablesNames.TLIsideArmLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(SideArmLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicSideArmLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(SideArmLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicSideArmLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLIsideArmLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == SideArmLibraryViewModel.Id && !x.disable &&
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

                            ((IDictionary<String, Object>)DynamicSideArmLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicSideArmLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(SideArmLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == TablesNames.TLIsideArmLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(SideArmLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == TablesNames.TLIsideArmLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == SideArmLibraryViewModel.Id && !x.disable &&
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

                    ((IDictionary<String, Object>)DynamicSideArmLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicSideArmLibrary);
                }

                SideArmTableDisplay.Model = OutPutList;
                SideArmTableDisplay.filters = _unitOfWork.SideArmLibraryRepository.GetRelatedTables();

                return new Response<ReturnWithFilters<object>>(true, SideArmTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        #endregion

        //Function take 1 parameter
        //get record by Id
        //get activated attributes with values
        //get table name Entity
        //get dynamic attributes
        public Response<GetForAddCivilLibrarybject> GetSideArmLibraryById(int Id)
        {
            try
            {
                GetForAddCivilLibrarybject attributes = new GetForAddCivilLibrarybject();

                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c =>
                    c.TableName == TablesNames.TLIsideArmLibrary.ToString());

                TLIsideArmLibrary SideArmLibrary = _unitOfWork.SideArmLibraryRepository.GetIncludeWhereFirst(x =>
                    x.Id == Id);

                List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TablesNames.TLIsideArmLibrary.ToString(), SideArmLibrary, null).ToList();
            
                var LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticals(Helpers.Constants.TablePartName.SideArm.ToString(), TablesNames.TLIsideArmLibrary.ToString(), Id);
                attributes.LogisticalItems = LogisticalItems;
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
                }

                return new Response<GetForAddCivilLibrarybject>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //get table name Entity by table name
        //get activated attributes 
        //get dynamic attributes
        public Response<GetForAddCivilLibrarybject> GetForAdd()
        {
            try
            {
                GetForAddCivilLibrarybject Attributes = new GetForAddCivilLibrarybject();
                var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TablesNames.TLIsideArmLibrary.ToString());
                List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TablesNames.TLIsideArmLibrary.ToString(), null, null, "Model").ToList();
                var LogisticalAttributes = _unitOfWork.LogistcalRepository.GetLogisticalLibrary(Helpers.Constants.TablePartName.SideArm.ToString());
                Attributes.LogisticalItems = LogisticalAttributes;
                Attributes.AttributesActivatedLibrary = listofAttributesActivated;

                IEnumerable<BaseInstAttViewDynamic> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                    .GetDynamicLibAtt(TableNameEntity.Id, null)
                    .AsParallel()
                    .Select(DynamicAttribute =>
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            DynamicAttribute.Value = DynamicAttribute.DataType.ToLower() switch
                            {
                                "string" => DynamicAttributeEntity.DefaultValue,
                                "int" => int.Parse(DynamicAttributeEntity.DefaultValue),
                                "double" => double.Parse(DynamicAttributeEntity.DefaultValue),
                                "boolean" => bool.Parse(DynamicAttributeEntity.DefaultValue),
                                "datetime" => DateTime.Parse(DynamicAttributeEntity.DefaultValue),
                                _ => DynamicAttribute.Value
                            };
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }

                        return DynamicAttribute;
                    });

                Attributes.DynamicAttributes = DynamicAttributesWithoutValue.ToList();


                return new Response<GetForAddCivilLibrarybject>(true, Attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<List<KeyValuePair<string, int>>> GetSideArmLibs()
        {
            try
            {
                int count = 0;
                var SideArmLibs = _unitOfWork.SideArmLibraryRepository.GetSelect(x => new { x.Model, x.Id }).Select(x => new KeyValuePair<string, int>(x.Model, x.Id)).ToList();
                return new Response<List<KeyValuePair<string, int>>>(true, SideArmLibs, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<List<KeyValuePair<string, int>>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
    }
}
