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
using TLIS_DAL;
using System.Collections;
using System.Globalization;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AsTypeDTOs;
using TLIS_DAL.ViewModels.AttActivatedCategoryDTOs;
using TLIS_DAL.ViewModels.BoardTypeDTOs;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CabinetPowerTypeDTOs;
using TLIS_DAL.ViewModels.CapacityDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilSteelSupportCategoryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegCategoryDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DiversityTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.InstCivilwithoutLegsTypeDTOs;
using TLIS_DAL.ViewModels.LogisticalitemDTOs;
using TLIS_DAL.ViewModels.ParityDTOs;
using TLIS_DAL.ViewModels.PolarityTypeDTOs;
using TLIS_DAL.ViewModels.SectionsLegTypeDTOs;
using TLIS_DAL.ViewModels.StructureTypeDTOs;
using TLIS_DAL.ViewModels.SupportTypeDesignedDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_DAL.ViewModels.TelecomTypeDTOs;
using TLIS_DAL.ViewModels.WorkflowHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using TLIS_Service.ServiceBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Org.BouncyCastle.Asn1.Cms;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using AutoMapper;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using System.Numerics;
using TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelLibraryDTOs;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using System.Data;
using Remotion;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static TLIS_DAL.ViewModels.SideArmLibraryDTOs.EditSideArmLibraryObject;

namespace TLIS_Service.Services
{
    public class CivilLibraryService : ICivilLibraryService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        ApplicationDbContext db;
        private IMapper _mapper;
        public CivilLibraryService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext _context, IMapper mapper)
        {
            db = _context;
            _unitOfWork = unitOfWork;
            _services = services;
            _mapper = mapper;
        }
        //Function accept 2 parameters
        //First TableName to specify the table i deal with
        //Second CivilLibraryViewModel object have data to add
        public Response<AddCivilWithLegsLibraryObject> AddCivilWithLegsLibrary(string TableName, AddCivilWithLegsLibraryObject AddCivilWithLegsLibraryObject, string connectionString, int UserId)
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
                           
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);
                            if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == TableName)
                            {
                                TLIcivilWithLegLibrary CivilWithLegEntites = _mapper.Map<TLIcivilWithLegLibrary>(AddCivilWithLegsLibraryObject.attributesActivatedLibrary);

                                
                                var logisticalObject = _unitOfWork.LogistcalRepository.GetByID(AddCivilWithLegsLibraryObject.logisticalItems.Vendor);
                                var vendor = logisticalObject?.Name;

                                var structureType = db.TLIstructureType.FirstOrDefault(x => x.Id == CivilWithLegEntites.structureTypeId);
                                var structureTypeName = structureType?.Name;
                                if (CivilWithLegEntites.SpaceLibrary == 0)
                                {
                                    return new Response<AddCivilWithLegsLibraryObject>(false, null, null, "spaceLibrary It must be greater than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                if(structureTypeName == null)
                                {
                                    return new Response<AddCivilWithLegsLibraryObject>(false, null, null, "structureType It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                if(vendor == null)
                                {
                                    return new Response<AddCivilWithLegsLibraryObject>(false, null, null, "Vendor It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                if (CivilWithLegEntites.Prefix == null)
                                {
                                    return new Response<AddCivilWithLegsLibraryObject>(false, null, null, $"Prefix It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                var model = vendor + ' ' + CivilWithLegEntites.Prefix + ' ' + structureTypeName + ' ' + CivilWithLegEntites.Height_Designed+"HE";

                                var CheckModel = db.MV_CIVIL_WITHLEG_LIBRARY_VIEW
                               .FirstOrDefault(x => x.Model != null &&
                                           x.Model.ToLower() == model.ToLower() &&
                                           !x.Deleted);


                                if (CheckModel != null)
                                    return new Response<AddCivilWithLegsLibraryObject>(false, null, null, $"The name {model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);


                                if (structureTypeName!=null && structureTypeName .ToLower()== "triangular")
                                    CivilWithLegEntites.NumberOfLegs = 3;

                                else if (structureTypeName != null && structureTypeName.ToLower() == "square")
                                    CivilWithLegEntites.NumberOfLegs = 4;

                                CivilWithLegEntites.Model = model;
                                
                                string CheckDependencyValidation = CheckDependencyValidationForCivilTypes(AddCivilWithLegsLibraryObject, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AddCivilWithLegsLibraryObject>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunctionLib(AddCivilWithLegsLibraryObject.dynamicAttributes, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AddCivilWithLegsLibraryObject>(false, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);


                               var HistoryId= _unitOfWork.CivilWithLegLibraryRepository.AddWithH(UserId,null, CivilWithLegEntites);
                                    
                                _unitOfWork.SaveChanges();

                                dynamic LogisticalItemIds = new ExpandoObject();
                                LogisticalItemIds = AddCivilWithLegsLibraryObject.logisticalItems;
                                AddLogisticalItemWithCivilH(UserId,LogisticalItemIds, CivilWithLegEntites, TableNameEntity.Id, HistoryId);

                                if (AddCivilWithLegsLibraryObject.dynamicAttributes != null ? AddCivilWithLegsLibraryObject.dynamicAttributes.Count > 0 : false)
                                {
                                    _unitOfWork.DynamicAttLibRepository.AddDynamicLibraryAtt(UserId, AddCivilWithLegsLibraryObject.dynamicAttributes, TableNameEntity.Id, CivilWithLegEntites.Id, connectionString, HistoryId);
                                }
                                
                                
                            }
                            
                            transaction.Complete();
                            tran.Commit();
                            Task.Run(() =>_unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                            return new Response<AddCivilWithLegsLibraryObject>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        catch (Exception err)
                        {
                            return new Response<AddCivilWithLegsLibraryObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }
            }
        }
        #region Helper Methods
        public Response<AddCivilWithoutLegsLibraryObject> AddCivilWithoutLegsLibrary(string TableName, AddCivilWithoutLegsLibraryObject AddCivilWithoutLegsLibraryObject, string connectionString, int UserId)
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
                            
                            TLIcivilWithoutLegLibrary CivilWithoutLegEntites = _mapper.Map<TLIcivilWithoutLegLibrary>(AddCivilWithoutLegsLibraryObject.attributesActivatedLibrary);
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);

                            var logisticalObject = _unitOfWork.LogistcalRepository.GetByID(AddCivilWithoutLegsLibraryObject.logisticalItems.Vendor);
                            var vendor = logisticalObject?.Name;

                            var structureType = db.TLIstructureType.FirstOrDefault(x => x.Id == CivilWithoutLegEntites.structureTypeId);
                            var structureTypeName = structureType?.Name;
                            if (CivilWithoutLegEntites.SpaceLibrary == 0)
                            {
                                return new Response<AddCivilWithoutLegsLibraryObject>(false, null, null, "spaceLibrary It must be greater than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                            if (structureTypeName == null)
                            {
                                return new Response<AddCivilWithoutLegsLibraryObject>(false, null, null, "structureType It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                            if (vendor == null)
                            {
                                return new Response<AddCivilWithoutLegsLibraryObject>(false, null, null, "Vendor It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);

                            }
                            if (CivilWithoutLegEntites.Prefix == null)
                            {
                                return new Response<AddCivilWithoutLegsLibraryObject>(false, null, null, $"Prefix It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);

                            }
                            var CivilCategoryName = _unitOfWork.CivilWithoutLegCategoryRepository.GetWhereFirst(x => x.Id == CivilWithoutLegEntites.CivilWithoutLegCategoryId) ?.Name;
                            var model = CivilCategoryName + ' ' + vendor + ' ' + CivilWithoutLegEntites.Prefix + ' ' + structureTypeName + ' ' + CivilWithoutLegEntites.Height_Designed+"HE";

                            var CheckModel = db.MV_CIVIL_WITHOUTLEG_LIBRARY_VIEW
                             .FirstOrDefault(x => x.Model != null &&
                                         x.Model.ToLower() == model.ToLower() &&
                                         !x.Deleted);
            


                            if (CheckModel != null)
                                return new Response<AddCivilWithoutLegsLibraryObject>(false, null, null, $"The name {model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                            string CheckDependencyValidation = CheckDependencyValidationForCivilTypes(AddCivilWithoutLegsLibraryObject, TableName);

                            if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                return new Response<AddCivilWithoutLegsLibraryObject>(false, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);


                            string CheckGeneralValidation = CheckGeneralValidationFunctionLib(AddCivilWithoutLegsLibraryObject.dynamicAttributes, TableNameEntity.TableName);

                            if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                return new Response<AddCivilWithoutLegsLibraryObject>(false, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);


                            CivilWithoutLegEntites.Model = model;
                            var HistoryId= _unitOfWork.CivilWithoutLegLibraryRepository.AddWithH(UserId,null, CivilWithoutLegEntites);

                            _unitOfWork.SaveChanges();

                            dynamic LogisticalItemIds = new ExpandoObject();
                            LogisticalItemIds = AddCivilWithoutLegsLibraryObject.logisticalItems;
                            AddLogisticalItemWithCivilH(UserId,LogisticalItemIds, CivilWithoutLegEntites, TableNameEntity.Id,HistoryId);

                            if (AddCivilWithoutLegsLibraryObject.dynamicAttributes != null ? AddCivilWithoutLegsLibraryObject.dynamicAttributes.Count > 0 : false)
                            {
                                _unitOfWork.DynamicAttLibRepository.AddDynamicLibraryAtt(UserId,AddCivilWithoutLegsLibraryObject.dynamicAttributes, TableNameEntity.Id, CivilWithoutLegEntites.Id, connectionString, HistoryId);
                            }
                         
                            transaction.Complete();
                            tran.Commit();
                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                            return new Response<AddCivilWithoutLegsLibraryObject>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        catch (Exception err)
                        {
                            return new Response<AddCivilWithoutLegsLibraryObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }   }
            }
        }
        public Response<AddCivilNonSteelLibraryObject> AddCivilNonSteelLibrary(string TableName, AddCivilNonSteelLibraryObject AddCivilNonSteelLibraryObject, string connectionString, int UserId)
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

                            TLIcivilNonSteelLibrary CivilNonSteelEntites = _mapper.Map<TLIcivilNonSteelLibrary>(AddCivilNonSteelLibraryObject.attributesActivatedLibrary);
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);

                            if (CivilNonSteelEntites.SpaceLibrary == 0)
                            {
                                return new Response<AddCivilNonSteelLibraryObject>(false, null, null, "spaceLibrary It must be greater than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                            var CheckModel = db.MV_CIVIL_NONSTEEL_LIBRARY_VIEW
                            .FirstOrDefault(x => x.Model != null &&
                            x.Model.ToLower() == CivilNonSteelEntites.Model.ToLower() &&
                            !x.Deleted);

                       
                            if (CheckModel != null)
                                return new Response<AddCivilNonSteelLibraryObject>(true, null, null, $"This model {CivilNonSteelEntites.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                            
                            string CheckDependencyValidation = CheckDependencyValidationForCivilTypes(AddCivilNonSteelLibraryObject, TableName);

                            if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                return new Response<AddCivilNonSteelLibraryObject>(false, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);


                            string CheckGeneralValidation = CheckGeneralValidationFunctionLib(AddCivilNonSteelLibraryObject.dynamicAttributes, TableNameEntity.TableName);

                            if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                return new Response<AddCivilNonSteelLibraryObject>(false, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                           var HistoryId= _unitOfWork.CivilNonSteelLibraryRepository.AddWithH(UserId,null, CivilNonSteelEntites);

                            _unitOfWork.SaveChanges();

                            dynamic LogisticalItemIds = new ExpandoObject();
                            LogisticalItemIds = AddCivilNonSteelLibraryObject.logisticalItems;
                            AddLogisticalItemWithCivilH(UserId,LogisticalItemIds, CivilNonSteelEntites, TableNameEntity.Id,HistoryId);

                            if (AddCivilNonSteelLibraryObject.dynamicAttributes != null ? AddCivilNonSteelLibraryObject.dynamicAttributes.Count > 0 : false)
                            {
                                _unitOfWork.DynamicAttLibRepository.AddDynamicLibraryAtt(UserId, AddCivilNonSteelLibraryObject.dynamicAttributes, TableNameEntity.Id, CivilNonSteelEntites.Id, connectionString,HistoryId);
                            }

                            transaction.Complete();
                            tran.Commit();
                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                            return new Response<AddCivilNonSteelLibraryObject>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        catch (Exception err)
                        {
                            return new Response<AddCivilNonSteelLibraryObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }
            }
        }
        public string CheckDependencyValidationForCivilTypes(object Input, string CivilType, int? catid = null)
        {
            List<DynamicAttViewModel> DynamicAttributes = null;
            if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString().ToLower())
            {
                AddCivilWithLegsLibraryObject AddCivilLibraryViewModel = _mapper.Map<AddCivilWithLegsLibraryObject>(Input);

                DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == CivilType.ToLower() && !x.disable
                        , x => x.tablesNames, x => x.DataType).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDdynamicAttributeInstallationValueViewModel InsertedDynamicAttributeValue = AddCivilLibraryViewModel.dynamicAttributes
                            .FirstOrDefault(x => x.id == DynamicAttribute.Id);

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

                                    InsertedValue = AddCivilLibraryViewModel.attributesActivatedLibrary.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddCivilLibraryViewModel.attributesActivatedLibrary, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDdynamicAttributeInstallationValueViewModel DynamicObject = AddCivilLibraryViewModel.dynamicAttributes
                                        .FirstOrDefault(x => x.id == Rule.dynamicAttId.Value);
                                    var Value = DynamicObject.value.ToString();
                                    if (Value != null)
                                    {
                                        string dataType = DynamicAttribute.DataType_Name.ToLower();

                                        switch (dataType)
                                        {
                                            case "bool":
                                                bool boolValue;
                                                if (bool.TryParse(Value, out boolValue))
                                                {
                                                    InsertedValue = boolValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid boolean value.");
                                                }
                                                break;
                                            case "datetime":
                                                DateTime dateTimeValue;
                                                if (DateTime.TryParse(Value, out dateTimeValue))
                                                {
                                                    InsertedValue = dateTimeValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid datetime value.");
                                                }
                                                break;
                                            case "double":
                                                double doubleValue;
                                                if (double.TryParse(Value, out doubleValue))
                                                {
                                                    InsertedValue = doubleValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid double value.");
                                                }
                                                break;
                                            case "int":
                                                int intValue;
                                                if (int.TryParse(Value, out intValue))
                                                {
                                                    InsertedValue = intValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid int value.");
                                                }
                                                break;
                                            case "string":
                                                InsertedValue = Value;
                                                break;
                                            default:

                                                break;
                                        }
                                    }
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 0) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
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
                                object InsertedDynamicAttributeValueAsObject = new object();
                                var Value = InsertedDynamicAttributeValue.value.ToString();
                                if (Value != null)
                                {
                                    string dataType = DynamicAttribute.DataType_Name.ToLower();

                                    switch (dataType)
                                    {
                                        case "bool":
                                            bool boolValue;
                                            if (bool.TryParse(Value, out boolValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = boolValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid boolean value.");
                                            }
                                            break;
                                        case "datetime":
                                            DateTime dateTimeValue;
                                            if (DateTime.TryParse(Value, out dateTimeValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = dateTimeValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid datetime value.");
                                            }
                                            break;
                                        case "double":
                                            double doubleValue;
                                            if (double.TryParse(Value, out doubleValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = doubleValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid double value.");
                                            }
                                            break;
                                        case "int":
                                            int intValue;
                                            if (int.TryParse(Value, out intValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = intValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid int value.");
                                            }
                                            break;
                                        case "string":
                                            InsertedDynamicAttributeValueAsObject = Value;
                                            break;
                                        default:

                                            break;
                                    }
                                }

                                if (Dependency.ValueDateTime != null && DynamicAttribute.DataType_Name.ToLower()== "datetime" )
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = new DateTime(((DateTime)InsertedDynamicAttributeValue.value).Year,
                                   ((DateTime)InsertedDynamicAttributeValue.value).Month, ((DateTime)InsertedDynamicAttributeValue.value).Day);

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
            else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString().ToLower())
            {

                AddCivilWithoutLegsLibraryObject AddCivilLibraryViewModel = _mapper.Map<AddCivilWithoutLegsLibraryObject>(Input);

                DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == CivilType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDdynamicAttributeInstallationValueViewModel InsertedDynamicAttributeValue = AddCivilLibraryViewModel.dynamicAttributes
                            .FirstOrDefault(x => x.id == DynamicAttribute.Id);

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

                                    InsertedValue = AddCivilLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddCivilLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDdynamicAttributeInstallationValueViewModel DynamicObject = AddCivilLibraryViewModel.dynamicAttributes
                                        .FirstOrDefault(x => x.id == Rule.dynamicAttId.Value);
                                    var Value = DynamicObject.value.ToString();
                                    if (Value != null)
                                    {
                                        string dataType = DynamicAttribute.DataType_Name.ToLower();

                                        switch (dataType)
                                        {
                                            case "bool":
                                                bool boolValue;
                                                if (bool.TryParse(Value, out boolValue))
                                                {
                                                    InsertedValue = boolValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid boolean value.");
                                                }
                                                break;
                                            case "datetime":
                                                DateTime dateTimeValue;
                                                if (DateTime.TryParse(Value, out dateTimeValue))
                                                {
                                                    InsertedValue = dateTimeValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid datetime value.");
                                                }
                                                break;
                                            case "double":
                                                double doubleValue;
                                                if (double.TryParse(Value, out doubleValue))
                                                {
                                                    InsertedValue = doubleValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid double value.");
                                                }
                                                break;
                                            case "int":
                                                int intValue;
                                                if (int.TryParse(Value, out intValue))
                                                {
                                                    InsertedValue = intValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid int value.");
                                                }
                                                break;
                                            case "string":
                                                InsertedValue = Value;
                                                break;
                                            default:

                                                break;
                                        }
                                    }
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 0) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
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
                                object InsertedDynamicAttributeValueAsObject = new object();
                                var Value = InsertedDynamicAttributeValue.value.ToString();
                                if (Value != null)
                                {
                                    string dataType = DynamicAttribute.DataType_Name.ToLower();

                                    switch (dataType)
                                    {
                                        case "bool":
                                            bool boolValue;
                                            if (bool.TryParse(Value, out boolValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = boolValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid boolean value.");
                                            }
                                            break;
                                        case "datetime":
                                            DateTime dateTimeValue;
                                            if (DateTime.TryParse(Value, out dateTimeValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = dateTimeValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid datetime value.");
                                            }
                                            break;
                                        case "double":
                                            double doubleValue;
                                            if (double.TryParse(Value, out doubleValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = doubleValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid double value.");
                                            }
                                            break;
                                        case "int":
                                            int intValue;
                                            if (int.TryParse(Value, out intValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = intValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid int value.");
                                            }
                                            break;
                                        case "string":
                                            InsertedDynamicAttributeValueAsObject = Value;
                                            break;
                                        default:

                                            break;
                                    }
                                }

                                if (Dependency.ValueDateTime != null && DynamicAttribute.DataType_Name.ToLower() == "datetime")
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = new DateTime(((DateTime)InsertedDynamicAttributeValue.value).Year,
                                   ((DateTime)InsertedDynamicAttributeValue.value).Month, ((DateTime)InsertedDynamicAttributeValue.value).Day);

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
            else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString().ToLower())
            {

                AddCivilNonSteelLibraryObject AddCivilLibraryViewModel = _mapper.Map<AddCivilNonSteelLibraryObject>(Input);

                DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == CivilType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDdynamicAttributeInstallationValueViewModel InsertedDynamicAttributeValue = AddCivilLibraryViewModel.dynamicAttributes
                            .FirstOrDefault(x => x.id == DynamicAttribute.Id);

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

                                    InsertedValue = AddCivilLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddCivilLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDdynamicAttributeInstallationValueViewModel DynamicObject = AddCivilLibraryViewModel.dynamicAttributes
                                        .FirstOrDefault(x => x.id == Rule.dynamicAttId.Value);
                                    var Value = DynamicObject.value.ToString();
                                    if (Value != null)
                                    {
                                        string dataType = DynamicAttribute.DataType_Name.ToLower();

                                        switch (dataType)
                                        {
                                            case "bool":
                                                bool boolValue;
                                                if (bool.TryParse(Value, out boolValue))
                                                {
                                                    InsertedValue = boolValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid boolean value.");
                                                }
                                                break;
                                            case "datetime":
                                                DateTime dateTimeValue;
                                                if (DateTime.TryParse(Value, out dateTimeValue))
                                                {
                                                    InsertedValue = dateTimeValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid datetime value.");
                                                }
                                                break;
                                            case "double":
                                                double doubleValue;
                                                if (double.TryParse(Value, out doubleValue))
                                                {
                                                    InsertedValue = doubleValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid double value.");
                                                }
                                                break;
                                            case "int":
                                                int intValue;
                                                if (int.TryParse(Value, out intValue))
                                                {
                                                    InsertedValue = intValue;
                                                }
                                                else
                                                {
                                                    InsertedValue = null;

                                                    throw new ArgumentException("Invalid int value.");
                                                }
                                                break;
                                            case "string":
                                                InsertedValue = Value;
                                                break;
                                            default:

                                                break;
                                        }
                                    }
                                }

                                if (InsertedValue == null)
                                    break;

                                if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                    RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                    RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                    RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                        Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 0) :
                                    RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                    RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
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
                                object InsertedDynamicAttributeValueAsObject = new object();
                                var Value = InsertedDynamicAttributeValue.value.ToString();
                                if (Value != null)
                                {
                                    string dataType = DynamicAttribute.DataType_Name.ToLower();

                                    switch (dataType)
                                    {
                                        case "bool":
                                            bool boolValue;
                                            if (bool.TryParse(Value, out boolValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = boolValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid boolean value.");
                                            }
                                            break;
                                        case "datetime":
                                            DateTime dateTimeValue;
                                            if (DateTime.TryParse(Value, out dateTimeValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = dateTimeValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid datetime value.");
                                            }
                                            break;
                                        case "double":
                                            double doubleValue;
                                            if (double.TryParse(Value, out doubleValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = doubleValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid double value.");
                                            }
                                            break;
                                        case "int":
                                            int intValue;
                                            if (int.TryParse(Value, out intValue))
                                            {
                                                InsertedDynamicAttributeValueAsObject = intValue;
                                            }
                                            else
                                            {
                                                InsertedDynamicAttributeValueAsObject = null;

                                                throw new ArgumentException("Invalid int value.");
                                            }
                                            break;
                                        case "string":
                                            InsertedDynamicAttributeValueAsObject = Value;
                                            break;
                                        default:

                                            break;
                                    }
                                }

                                if (Dependency.ValueDateTime != null && DynamicAttribute.DataType_Name.ToLower() == "datetime")
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = new DateTime(((DateTime)InsertedDynamicAttributeValue.value).Year,
                                   ((DateTime)InsertedDynamicAttributeValue.value).Month, ((DateTime)InsertedDynamicAttributeValue.value).Day);

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
        public string CheckGeneralValidationFunction(List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue, string TableName, int? catid = null)
        {
            List<DynamicAttViewModel> DynamicAttributes = null;

            if (catid != null)
            {
                DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                   .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable && x.CivilWithoutLegCategoryId == catid
                       , x => x.tablesNames).ToList());
            }
            else
            {
                DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                   .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable
                       , x => x.tablesNames).ToList());
            }

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
        public string CheckGeneralValidationFunctionLib(List<AddDdynamicAttributeInstallationValueViewModel> TLIdynamicAttLibValue, string TableName, int? catid = null)
        {
            List<DynamicAttViewModel> DynamicAttributes = null;

            if (catid != null)
            {
                DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                   .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable && x.CivilWithoutLegCategoryId == catid
                       , x => x.tablesNames,x=>x.DataType).ToList());
            }
            else
            {
                DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                   .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable
                       , x => x.tablesNames, x => x.DataType).ToList());
            }
            foreach (DynamicAttViewModel DynamicAttributeEntity in DynamicAttributes)
            {
                TLIvalidation Validation = _unitOfWork.ValidationRepository
                    .GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttributeEntity.Id, x => x.Operation, x => x.DynamicAtt);

                if (Validation != null)
                {
                    AddDdynamicAttributeInstallationValueViewModel DynmaicAttributeValue = TLIdynamicAttLibValue.FirstOrDefault(x => x.id == DynamicAttributeEntity.Id);

                    if (DynmaicAttributeValue == null)
                        return $"({Validation.DynamicAtt.Key}) value can't be null and must be inserted";
                    var Value = DynmaicAttributeValue.value.ToString();
                    string OperationName = Validation.Operation.Name;

                    object InputDynamicValue = null; // Initialize to null

                    if (Value != null)
                    {
                        string dataType = DynamicAttributeEntity.DataType_Name.ToLower();

                        switch (dataType)
                        {
                            case "bool":
                                bool boolValue;
                                if (bool.TryParse(Value, out boolValue))
                                {
                                    InputDynamicValue = boolValue;
                                }
                                else
                                {
                                    InputDynamicValue = null; 
                                                              
                                    throw new ArgumentException("Invalid boolean value.");
                                }
                                break;
                            case "datetime":
                                DateTime dateTimeValue;
                                if (DateTime.TryParse(Value, out dateTimeValue))
                                {
                                    InputDynamicValue = dateTimeValue;
                                }
                                else
                                {
                                    InputDynamicValue = null; 
                                                            
                                    throw new ArgumentException("Invalid datetime value.");
                                }
                                break;
                            case "double":
                                double doubleValue;
                                if (double.TryParse(Value, out doubleValue))
                                {
                                    InputDynamicValue = doubleValue;
                                }
                                else
                                {
                                    InputDynamicValue = null; 
                                                             
                                    throw new ArgumentException("Invalid double value.");
                                }
                                break;
                            case "int":
                                int intValue;
                                if (int.TryParse(Value, out intValue))
                                {
                                    InputDynamicValue = intValue;
                                }
                                else
                                {
                                    InputDynamicValue = null; 
                                                              
                                    throw new ArgumentException("Invalid int value.");
                                }
                                break;
                            case "string":
                                InputDynamicValue = Value; 
                                break;
                            default:
                                
                                break;
                        }
                    }


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
        public void AddLogisticalItemWithCivil(int UserId,dynamic LogisticalItemIds, dynamic CivilEntity, int TableNameEntityId)
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
                                    RecordId = CivilEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId,NewLogisticalItem);
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
                                    RecordId = CivilEntity.Id,
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
                                    RecordId = CivilEntity.Id,
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
                                    RecordId = CivilEntity.Id,
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
                                    RecordId = CivilEntity.Id,
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
                                    RecordId = CivilEntity.Id,
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
        public void AddLogisticalItemWithCivilH(int UserId, dynamic LogisticalItemIds, dynamic CivilEntity, int TableNameEntityId, int HistoryId )
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
                                    RecordId = CivilEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TableNameEntityId, NewLogisticalItem, HistoryId);
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
                                    RecordId = CivilEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TableNameEntityId, NewLogisticalItem, HistoryId);
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
                                    RecordId = CivilEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TableNameEntityId, NewLogisticalItem, HistoryId);
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
                                    RecordId = CivilEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TableNameEntityId, NewLogisticalItem, HistoryId);
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
                                    RecordId = CivilEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TableNameEntityId, NewLogisticalItem, HistoryId);
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
                                    RecordId = CivilEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TableNameEntityId, NewLogisticalItem, HistoryId);
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

        //Function take 2 parameters that disable or enable record depened on record status ex: if record is active then disable else enable
        //First Id to specify the record i deal with
        //Second TableName to specify the table i deal with
        public async Task<Response<AllItemAttributes>> Disable(int Id, string TableName,int UserId,string connectionString)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == TableName)
                    {
                        var UsedCivil = _unitOfWork.CivilSiteDateRepository.GetWhereAndInclude(x => x.allCivilInst.civilWithLegs.CivilWithLegsLibId
                        == Id && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,x=>x.allCivilInst.civilWithLegs.CivilWithLegsLib).ToList();
                        var NewCivilWithLeg = _unitOfWork.CivilWithLegLibraryRepository.GetWhereFirst(x => x.Id == Id);

                        if (UsedCivil != null && UsedCivil.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not change status this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);
                        
                       

                            TLIcivilWithLegLibrary OldCivilWithLeg = _unitOfWork.CivilWithLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);

                            NewCivilWithLeg.Active = !(NewCivilWithLeg.Active);

                            _unitOfWork.CivilWithLegLibraryRepository.UpdateWithH(UserId,null, OldCivilWithLeg, NewCivilWithLeg);
                            await _unitOfWork.SaveChangesAsync();
                          
                        
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == TableName)
                    {
                        var UsedCivil = _unitOfWork.CivilSiteDateRepository.GetWhereAndInclude(x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLibId
                        == Id && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib).ToList();
                        TLIcivilWithoutLegLibrary NewCivilWithoutLeg = _unitOfWork.CivilWithoutLegLibraryRepository.GetWhereFirst(x => x.Id == Id);
                        if (UsedCivil != null && UsedCivil.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not change status this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);
                        
                       
                            TLIcivilWithoutLegLibrary OldCivilWithoutLeg = _unitOfWork.CivilWithoutLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                            NewCivilWithoutLeg.Active = !(NewCivilWithoutLeg.Active);
                            _unitOfWork.CivilWithoutLegLibraryRepository.UpdateWithH(UserId,null, OldCivilWithoutLeg, NewCivilWithoutLeg);
                            await _unitOfWork.SaveChangesAsync();
                          
                        
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == TableName)
                    {
                        var UsedCivil = _unitOfWork.CivilSiteDateRepository.GetWhereAndInclude(x => x.allCivilInst.civilNonSteel.CivilNonSteelLibraryId
                         == Id && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilNonSteel, x => x.allCivilInst.civilNonSteel.CivilNonsteelLibrary).ToList();
                        TLIcivilNonSteelLibrary NewCivilNonSteel = _unitOfWork.CivilNonSteelLibraryRepository.GetWhereFirst(x => x.Id == Id);
                        if (UsedCivil != null && UsedCivil.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not change status this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);
                        

                            TLIcivilNonSteelLibrary OldCivilNonSteel = _unitOfWork.CivilNonSteelLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                            NewCivilNonSteel.Active = !(NewCivilNonSteel.Active);
                            _unitOfWork.CivilNonSteelLibraryRepository.UpdateWithH(UserId,null, OldCivilNonSteel, NewCivilNonSteel);
                            await _unitOfWork.SaveChangesAsync();
                 
                        
                    }
                    transaction.Complete();
                    
                     Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                    
                    return new Response<AllItemAttributes>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {
                    return new Response<AllItemAttributes>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

        }
        //Function take 2 parameters 
        //First CivilLibraryViewModel object contain data to update
        //Second TableName to specify the table i deal with
        public async Task<Response<EditCivilWithLegsLibraryObject>> EditCivilWithLegsLibrary(EditCivilWithLegsLibraryObject editCivilWithLegsLibrary, string TableName,int userId,string connectionString)
        {
            int resultId = 0;
;
            dynamic DtestUpdate = new ExpandoObject();

            using (TransactionScope transaction =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);
                    
                    TLIcivilWithLegLibrary CivilWithLegLibraryEntites = _mapper.Map<TLIcivilWithLegLibrary>(editCivilWithLegsLibrary.attributesActivatedLibrary);

                    TLIcivilWithLegLibrary CivilWithLegLib = _unitOfWork.CivilWithLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == CivilWithLegLibraryEntites.Id);
                  
                    var logisticalObject = _unitOfWork.LogistcalRepository.GetByID(editCivilWithLegsLibrary.logisticalItems.Vendor);
                    var vendor = logisticalObject?.Name;
                    if(CivilWithLegLib.structureTypeId != CivilWithLegLibraryEntites.structureTypeId)
                        return new Response<EditCivilWithLegsLibraryObject>(false, null, null, "can not change structureType of civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                
                    var structureType = db.TLIstructureType.FirstOrDefault(x => x.Id == CivilWithLegLib.structureTypeId);
                    var structureTypeName = structureType?.Name;

                    if (CivilWithLegLibraryEntites.SpaceLibrary == 0)
                    {
                        return new Response<EditCivilWithLegsLibraryObject>(false, null, null, "spaceLibrary It must be greater than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    if (structureTypeName == null)
                    {
                        return new Response<EditCivilWithLegsLibraryObject>(false, null, null, "structureType It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    if (vendor == null)
                    {
                        return new Response<EditCivilWithLegsLibraryObject>(false, null, null, "Vendor It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);

                    }
                    if (CivilWithLegLibraryEntites.Prefix == null)
                    {
                        return new Response<EditCivilWithLegsLibraryObject>(false, null, null, $"{CivilWithLegLib.Prefix} It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);

                    }
          
                    var Civilinst = _unitOfWork.CivilSiteDateRepository.GetAllAsQueryable().AsNoTracking().Where(x => x.allCivilInst.civilWithLegs.CivilWithLegsLibId ==
                      CivilWithLegLibraryEntites.Id).Include(x => x.allCivilInst).Include(x => x.allCivilInst.
                      civilWithLegs).Include(x => x.allCivilInst.civilWithLegs.CivilWithLegsLib).ToList();

                    foreach (var item in Civilinst)
                    { 
                        if(item.allCivilInst.civilWithLegs.IsEnforeced ==false && item.allCivilInst.civilWithLegs.Support_Limited_Load <= 0)
                        {
                            if(item.allCivilInst.civilWithLegs.CurrentLoads > CivilWithLegLibraryEntites.Manufactured_Max_Load)
                            {
                                return new Response<EditCivilWithLegsLibraryObject>(false, null, null, "can not to be Manufactured_Max_Load smaller from CurrentLoads", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }

                    }
                    var model = vendor + ' ' + CivilWithLegLibraryEntites.Prefix + ' ' + structureTypeName + ' ' + CivilWithLegLibraryEntites.Height_Designed + "HE";

                    var CheckModel = db.MV_CIVIL_WITHLEG_LIBRARY_VIEW
                    .FirstOrDefault(x => x.Model != null && x.Id != CivilWithLegLibraryEntites.Id &&
                               x.Model.ToLower() == model.ToLower() &&
                               !x.Deleted);
                    


                    if (CheckModel != null)
                        return new Response<EditCivilWithLegsLibraryObject>(false, null, null, $"The name {model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                    //if (structureTypeName != null && structureTypeName.ToLower() == "triangular")
                    //    CivilWithLegLibraryEntites.NumberOfLegs = 3;

                    //else if (structureTypeName != null && structureTypeName.ToLower() == "square")
                    //    CivilWithLegLibraryEntites.NumberOfLegs = 4;

                    CivilWithLegLibraryEntites.Model = model;

                    CivilWithLegLibraryEntites.Active = CivilWithLegLib.Active;
                    CivilWithLegLibraryEntites.Deleted = CivilWithLegLib.Deleted;
                    
                   var HistoryId=_unitOfWork.CivilWithLegLibraryRepository.UpdateWithH(userId,null, CivilWithLegLib, CivilWithLegLibraryEntites);


                    string CheckDependencyValidation = CheckDependencyValidationForCivilTypesEditApiVersions(editCivilWithLegsLibrary, TableName);
                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                    {
                        return new Response<EditCivilWithLegsLibraryObject>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                    string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersions(editCivilWithLegsLibrary.dynamicAttributes, TableNameEntity.TableName);
                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                    {
                        return new Response<EditCivilWithLegsLibraryObject>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                    AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                    var CheckVendorId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithLegLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckVendorId != null)
                        OldLogisticalItemIds.Vendor = Convert.ToInt32(CheckVendorId.logisticalId);

                    var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithLegLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckSupplierId != null)
                        OldLogisticalItemIds.Supplier = CheckSupplierId.logisticalId;

                    var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithLegLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckDesignerId != null)
                        OldLogisticalItemIds.Designer = CheckDesignerId.logisticalId;


                    var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithLegLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckManufacturerId != null)
                        OldLogisticalItemIds.Manufacturer = CheckManufacturerId.logisticalId;


                    var CheckContractorId = _unitOfWork.LogisticalitemRepository
                 .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Contractor.ToString().ToLower() &&
                     x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithLegLibraryEntites.Id, x => x.logistical,
                         x => x.logistical.logisticalType);

                    if (CheckContractorId != null)
                        OldLogisticalItemIds.Contractor = CheckContractorId.logisticalId;


                    var CheckConsultantId = _unitOfWork.LogisticalitemRepository
                       .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Consultant.ToString().ToLower() &&
                           x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithLegLibraryEntites.Id, x => x.logistical,
                               x => x.logistical.logisticalType);

                    if (CheckConsultantId != null)
                        OldLogisticalItemIds.Consultant = CheckConsultantId.logisticalId;


                    EditLogisticalItemH(userId,editCivilWithLegsLibrary.logisticalItems, CivilWithLegLibraryEntites, TableNameEntity.Id, OldLogisticalItemIds, HistoryId);

                    if (editCivilWithLegsLibrary.dynamicAttributes != null ? editCivilWithLegsLibrary.dynamicAttributes.Count > 0 : false)
                    {
                        _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithH(editCivilWithLegsLibrary.dynamicAttributes, connectionString, TableNameEntity.Id, CivilWithLegLibraryEntites.Id,userId,HistoryId);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    transaction.Complete();
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                    return new Response<EditCivilWithLegsLibraryObject>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }

                catch (Exception err)
                {
                    return new Response<EditCivilWithLegsLibraryObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        #region Helper Methods..
        public async Task<Response<EditCivilWithoutLegsLibraryObject>> EditCivilWithoutlegsLibrary(EditCivilWithoutLegsLibraryObject editCivilWithoutLegsLibraryObject, string TableName, int userId,string connectionString)
        {
            int resultId = 0;
            int civilLibId = 0;
            int tablesNameId = 0;
            dynamic DtestUpdate = new ExpandoObject();

            using (TransactionScope transaction =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);

                    TLIcivilWithoutLegLibrary CivilWithoutLegLibraryEntites = _mapper.Map<TLIcivilWithoutLegLibrary>(editCivilWithoutLegsLibraryObject.attributesActivatedLibrary);
                    TLIcivilWithoutLegLibrary CivilWithoutLegLib = _unitOfWork.CivilWithoutLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == CivilWithoutLegLibraryEntites.Id);
                    var logisticalObject = _unitOfWork.LogistcalRepository.GetByID(editCivilWithoutLegsLibraryObject.logisticalItems.Vendor);
                    var vendor = logisticalObject?.Name;

                    var structureType = db.TLIstructureType.FirstOrDefault(x => x.Id == CivilWithoutLegLibraryEntites.structureTypeId);
                    var structureTypeName = structureType?.Name;
                    if (CivilWithoutLegLibraryEntites.SpaceLibrary == 0)
                    {
                        return new Response<EditCivilWithoutLegsLibraryObject>(false, null, null, "spaceLibrary It must be greater than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    if (structureTypeName == null)
                    {
                        return new Response<EditCivilWithoutLegsLibraryObject>(false, null, null, "structureType It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    if (vendor == null)
                    {
                        return new Response<EditCivilWithoutLegsLibraryObject>(false, null, null, "Vendor It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);

                    }
                    if (CivilWithoutLegLibraryEntites.Prefix == null)
                    {
                        return new Response<EditCivilWithoutLegsLibraryObject>(false, null, null, $"{CivilWithoutLegLibraryEntites.Prefix} It does not have to be empty", (int)Helpers.Constants.ApiReturnCode.fail);

                    }
                    var Civilinst = _unitOfWork.CivilSiteDateRepository.GetAllAsQueryable().AsNoTracking().Where(x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLibId ==
                   CivilWithoutLegLibraryEntites.Id).Include(x => x.allCivilInst).Include( x => x.allCivilInst.
                   civilWithoutLeg).Include( x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib).ToList();

                    foreach (var item in Civilinst)
                    {
                        if (item.allCivilInst.civilWithoutLeg.Support_Limited_Load <= 0)
                        {
                            if (item.allCivilInst.civilWithoutLeg.CurrentLoads > CivilWithoutLegLibraryEntites.Manufactured_Max_Load)
                            {
                                return new Response<EditCivilWithoutLegsLibraryObject>(false, null, null, "can not to be Manufactured_Max_Load smaller from CurrentLoads", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }

                    }
                    var CivilCategoryName = _unitOfWork.CivilWithoutLegCategoryRepository.GetWhereFirst(x => x.Id == CivilWithoutLegLibraryEntites.CivilWithoutLegCategoryId)?.Name;
                    var model = CivilCategoryName + ' ' + vendor + ' ' + CivilWithoutLegLibraryEntites.Prefix + ' ' + structureTypeName + ' ' + CivilWithoutLegLibraryEntites.Height_Designed + "HE";

                    var CheckModel = db.MV_CIVIL_WITHOUTLEG_LIBRARY_VIEW
                    .FirstOrDefault(x => x.Model != null && x.Id != CivilWithoutLegLibraryEntites.Id &&
                               x.Model.ToLower() == model.ToLower() &&
                               !x.Deleted);
       


                    if (CheckModel != null)
                        return new Response<EditCivilWithoutLegsLibraryObject>(false, null, null, $"The name {model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                    CivilWithoutLegLibraryEntites.Model = model;
                    CivilWithoutLegLibraryEntites.CivilSteelSupportCategoryId = CivilWithoutLegLibraryEntites.CivilWithoutLegCategoryId;

                    CivilWithoutLegLibraryEntites.Active = CivilWithoutLegLib.Active;
                    CivilWithoutLegLibraryEntites.Deleted = CivilWithoutLegLib.Deleted;
                    var HistoryId=_unitOfWork.CivilWithoutLegLibraryRepository.UpdateWithH(userId,null, CivilWithoutLegLib, CivilWithoutLegLibraryEntites);
                    _unitOfWork.SaveChanges();

                    string CheckDependencyValidation = CheckDependencyValidationForCivilTypesEditApiVersions(editCivilWithoutLegsLibraryObject, TableName);
                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                    {
                        return new Response<EditCivilWithoutLegsLibraryObject>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                    string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersions(editCivilWithoutLegsLibraryObject.dynamicAttributes, TableNameEntity.TableName);
                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                    {
                        return new Response<EditCivilWithoutLegsLibraryObject>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                    AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                    var CheckVendorId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithoutLegLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckVendorId != null)
                        OldLogisticalItemIds.Vendor = Convert.ToInt32(CheckVendorId.logisticalId);

                    var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithoutLegLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckSupplierId != null)
                        OldLogisticalItemIds.Supplier = CheckSupplierId.logisticalId;

                    var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithoutLegLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckDesignerId != null)
                        OldLogisticalItemIds.Designer = CheckDesignerId.logisticalId;


                    var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithoutLegLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckManufacturerId != null)
                        OldLogisticalItemIds.Manufacturer = CheckManufacturerId.logisticalId;


                    var CheckContractortId = _unitOfWork.LogisticalitemRepository
                     .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Contractor.ToString().ToLower() &&
                         x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithoutLegLibraryEntites.Id, x => x.logistical,
                             x => x.logistical.logisticalType);

                    if (CheckContractortId != null)
                        OldLogisticalItemIds.Contractor = CheckContractortId.logisticalId;


                    var CheckConsultantId = _unitOfWork.LogisticalitemRepository
                       .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Consultant.ToString().ToLower() &&
                           x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithoutLegLibraryEntites.Id, x => x.logistical,
                               x => x.logistical.logisticalType);

                    if (CheckConsultantId != null)
                        OldLogisticalItemIds.Consultant = CheckConsultantId.logisticalId;



                    EditLogisticalItemH(userId,editCivilWithoutLegsLibraryObject.logisticalItems, CivilWithoutLegLibraryEntites, TableNameEntity.Id, OldLogisticalItemIds, HistoryId);

                    if (editCivilWithoutLegsLibraryObject.dynamicAttributes != null ? editCivilWithoutLegsLibraryObject.dynamicAttributes.Count > 0 : false)
                    {
                        _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithH(editCivilWithoutLegsLibraryObject.dynamicAttributes, connectionString, TableNameEntity.Id, CivilWithoutLegLibraryEntites.Id, userId,HistoryId);
                    }
                    civilLibId = CivilWithoutLegLibraryEntites.Id;
                    tablesNameId = TableNameEntity.Id;

                    await _unitOfWork.SaveChangesAsync();

                    transaction.Complete();
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                    return new Response<EditCivilWithoutLegsLibraryObject>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }

                catch (Exception err)
                {
                    return new Response<EditCivilWithoutLegsLibraryObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        public async Task<Response<EditCivilNonSteelLibraryObject>> EditCivilNonSteelLibrary(EditCivilNonSteelLibraryObject editCivilNonSteelLibraryObject, string TableName, int userId,string connectionString)
        {
            int resultId = 0;
            int civilLibId = 0;
            int tablesNameId = 0;
            dynamic DtestUpdate = new ExpandoObject();

            using (TransactionScope transaction =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);

                    TLIcivilNonSteelLibrary CivilNonSteelibraryEntites = _mapper.Map<TLIcivilNonSteelLibrary>(editCivilNonSteelLibraryObject.attributesActivatedLibrary);

                    TLIcivilNonSteelLibrary CivilNonSteelLib = _unitOfWork.CivilNonSteelLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == CivilNonSteelibraryEntites.Id);

                    var CheckModel = db.MV_CIVIL_NONSTEEL_LIBRARY_VIEW
                   .FirstOrDefault(x => x.Model != null && x.Id != CivilNonSteelibraryEntites.Id &&
                             x.Model.ToLower() == CivilNonSteelibraryEntites.Model.ToLower() &&
                             !x.Deleted);

                    if (CheckModel != null)
                    {
                        return new Response<EditCivilNonSteelLibraryObject>(true, null, null, $"This model {CivilNonSteelibraryEntites.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    CivilNonSteelibraryEntites.Active = CivilNonSteelLib.Active;
                    CivilNonSteelibraryEntites.Deleted = CivilNonSteelLib.Deleted;
                    var  HistoryId=_unitOfWork.CivilNonSteelLibraryRepository.UpdateWithH(userId,null, CivilNonSteelLib, CivilNonSteelibraryEntites);


                    string CheckDependencyValidation = CheckDependencyValidationForCivilTypesEditApiVersions(editCivilNonSteelLibraryObject, TableName);
                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                    {
                        return new Response<EditCivilNonSteelLibraryObject>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                    string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersions(editCivilNonSteelLibraryObject.dynamicAttributes, TableNameEntity.TableName);
                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                    {
                        return new Response<EditCivilNonSteelLibraryObject>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                    LogisticalObject OldLogisticalItemIds = new LogisticalObject();

                    var CheckVendorId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilNonSteelibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckVendorId != null)
                        OldLogisticalItemIds.Vendor = CheckVendorId.logisticalId;

                    var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilNonSteelibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckSupplierId != null)
                        OldLogisticalItemIds.Supplier = CheckSupplierId.logisticalId;

                    var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilNonSteelibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckDesignerId != null)
                        OldLogisticalItemIds.Designer = CheckDesignerId.logisticalId;


                    var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilNonSteelibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckManufacturerId != null)
                        OldLogisticalItemIds.Manufacturer = CheckManufacturerId.logisticalId;


                    var CheckContractortId = _unitOfWork.LogisticalitemRepository
                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Contractor.ToString().ToLower() &&
                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilNonSteelibraryEntites.Id, x => x.logistical,
                            x => x.logistical.logisticalType);

                    if (CheckContractortId != null)
                        OldLogisticalItemIds.Contractor = CheckContractortId.logisticalId;


                    var CheckConsultantId = _unitOfWork.LogisticalitemRepository
                       .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Consultant.ToString().ToLower() &&
                           x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilNonSteelibraryEntites.Id, x => x.logistical,
                               x => x.logistical.logisticalType);

                    if (CheckConsultantId != null)
                        OldLogisticalItemIds.Consultant = CheckConsultantId.logisticalId;

                    EditLogisticalItemsH(userId, editCivilNonSteelLibraryObject.logisticalItems, CivilNonSteelibraryEntites, TableNameEntity.Id, OldLogisticalItemIds,HistoryId);

                    if (editCivilNonSteelLibraryObject.dynamicAttributes != null ? editCivilNonSteelLibraryObject.dynamicAttributes.Count > 0 : false)
                    {
                        _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithH(editCivilNonSteelLibraryObject.dynamicAttributes, connectionString, TableNameEntity.Id, CivilNonSteelibraryEntites.Id, userId,HistoryId);
                    }
                    civilLibId = CivilNonSteelibraryEntites.Id;
                    tablesNameId = TableNameEntity.Id;

                    await _unitOfWork.SaveChangesAsync();

                    
                    transaction.Complete();
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                    return new Response<EditCivilNonSteelLibraryObject>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }

                catch (Exception err)
                {
                    return new Response<EditCivilNonSteelLibraryObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        public Response<GetEnableAttribute> GetCivilNonSteelLibrariesEnabledAtt(string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    //string storedProcedureName = "create_dynamic_pivot_nonsteel_library ";
                    //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    //{
                    //    procedureCommand.CommandType = CommandType.StoredProcedure;
                    //    procedureCommand.ExecuteNonQuery();
                    //}
                    var attActivated = db.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "CivilNonSteelLibrary"
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
                        var query = db.MV_CIVIL_NONSTEEL_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();
                       
                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = db.MV_CIVIL_NONSTEEL_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                    .GroupBy(x => new
                    {
                        Id = x.Id,
                        Model = x.Model,
                        Note = x.Note,
                        Prefix = x.Prefix,
                        Hight = x.Hight,
                        VerticalMeasured = x.VerticalMeasured,
                        SpaceLibrary = x.SpaceLibrary,
                        Active = x.Active,
                        Deleted = x.Deleted,
                        CIVILNONSTEELTYPE = x.CIVILNONSTEELTYPE,
                        NumberofBoltHoles = x.NumberofBoltHoles,
                        Manufactured_Max_Load = x.Manufactured_Max_Load,
                        WidthVariation = x.WidthVariation,


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
        public string CheckDependencyValidationForCivilTypesEditApiVersion(object Input, string CivilType, int? catid = null)
        {
            if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString().ToLower())
            {
                EditCivilWithLegLibraryViewModels EditCivilWithLegLibraryViewModels = _mapper.Map<EditCivilWithLegLibraryViewModels>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == CivilType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditCivilWithLegLibraryViewModels.DynamicAtts
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

                                    InsertedValue = EditCivilWithLegLibraryViewModels.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditCivilWithLegLibraryViewModels, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditCivilWithLegLibraryViewModels.DynamicAtts
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
            else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString().ToLower())
            {
                EditCivilWithoutLegLibraryViewModel EditCivilWithoutLegLibraryViewModel = _mapper.Map<EditCivilWithoutLegLibraryViewModel>(Input);
                List<DynamicAttViewModel> DynamicAttributes = new List<DynamicAttViewModel>();
                if (catid != null)
                {
                    DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                                        .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == CivilType.ToLower() && !x.disable && x.CivilWithoutLegCategoryId == catid
                                            , x => x.tablesNames).ToList());
                }
                else
                {
                    DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                                        .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == CivilType.ToLower() && !x.disable
                                            , x => x.tablesNames).ToList());
                }

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditCivilWithoutLegLibraryViewModel.DynamicAtts
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

                                    InsertedValue = EditCivilWithoutLegLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditCivilWithoutLegLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditCivilWithoutLegLibraryViewModel.DynamicAtts
                                        .FirstOrDefault(x => x.Key.ToLower() == Rule.dynamicAtt.Key.ToLower());
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
            else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString().ToLower())
            {
                EditCivilNonSteelLibraryViewModel EditCivilNonSteelLibraryViewModels = _mapper.Map<EditCivilNonSteelLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == CivilType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditCivilNonSteelLibraryViewModels.DynamicAtts
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

                                    InsertedValue = EditCivilNonSteelLibraryViewModels.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditCivilNonSteelLibraryViewModels, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditCivilNonSteelLibraryViewModels.DynamicAtts
                                        .FirstOrDefault(x => x.Key.ToLower() == Rule.dynamicAtt.Key.ToLower());
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
        public string CheckDependencyValidationForCivilTypesEditApiVersions(object Input, string CivilType, int? catid = null)
        {
            if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString().ToLower())
            {
                EditCivilWithLegsLibraryObject EditCivilWithLegLibraryViewModels = _mapper.Map<EditCivilWithLegsLibraryObject>(Input);

                List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes = _mapper.Map<List<AddDdynamicAttributeInstallationValueViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == CivilType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (AddDdynamicAttributeInstallationValueViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDdynamicAttributeInstallationValueViewModel InsertedDynamicAttributeValue = EditCivilWithLegLibraryViewModels.dynamicAttributes
                            .FirstOrDefault(x => x.id == DynamicAttribute.id);
                        var Key = db.TLIdynamicAtt.FirstOrDefault(x => x.Id == InsertedDynamicAttributeValue.id).Key;
                        if (InsertedDynamicAttributeValue == null)
                            return $"({Key}) value can't be null and must be inserted";

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

                                    InsertedValue = EditCivilWithLegLibraryViewModels.attributesActivatedLibrary.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditCivilWithLegLibraryViewModels.attributesActivatedLibrary, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDdynamicAttributeInstallationValueViewModel DynamicObject = EditCivilWithLegLibraryViewModels.dynamicAttributes
                                        .FirstOrDefault(x => x.id == Rule.dynamicAtt.Id);

                                    if (DynamicObject == null)
                                        break;

                                    InsertedValue = DynamicObject.value;
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

                                object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.value;

                                if (Dependency.ValueDateTime != null)
                                {
                                    DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                        Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                    DependencyValdiationValue = DependencyValdiationValueConverter;

                                    DateTime InsertedDynamicAttributeValueAsObjectConverter = DateTime.Parse(InsertedDynamicAttributeValue.value.ToString());

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
            //else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString().ToLower())
            //{
            //    EditCivilWithoutLegLibraryViewModel EditCivilWithoutLegLibraryViewModel = _mapper.Map<EditCivilWithoutLegLibraryViewModel>(Input);
            //    List<DynamicAttViewModel> DynamicAttributes = new List<DynamicAttViewModel>();
            //    if (catid != null)
            //    {
            //        DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
            //                            .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == CivilType.ToLower() && !x.disable && x.CivilWithoutLegCategoryId == catid
            //                                , x => x.tablesNames).ToList());
            //    }
            //    else
            //    {
            //        DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
            //                            .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == CivilType.ToLower() && !x.disable
            //                                , x => x.tablesNames).ToList());
            //    }

            //    foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
            //    {
            //        TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
            //            x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
            //                x => x.Operation, x => x.DynamicAtt);

            //        if (Dependency != null)
            //        {
            //            DynamicAttLibViewModel InsertedDynamicAttributeValue = EditCivilWithoutLegLibraryViewModel.DynamicAtts
            //                .FirstOrDefault(x => x.Key.ToLower() == DynamicAttribute.Key.ToLower());

            //            if (InsertedDynamicAttributeValue == null)
            //                return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

            //            List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

            //            foreach (int RowId in RowsIds)
            //            {
            //                List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
            //                    , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

            //                int Succed = 0;

            //                foreach (TLIrule Rule in Rules)
            //                {
            //                    string RuleOperation = Rule.Operation.Name;
            //                    object RuleValue = new object();

            //                    if (Rule.OperationValueBoolean != null)
            //                        RuleValue = Rule.OperationValueBoolean;

            //                    else if (Rule.OperationValueDateTime != null)
            //                        RuleValue = Rule.OperationValueDateTime;

            //                    else if (Rule.OperationValueDouble != null)
            //                        RuleValue = Rule.OperationValueDouble;

            //                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
            //                        RuleValue = Rule.OperationValueString;

            //                    object InsertedValue = new object();

            //                    if (Rule.attributeActivatedId != null)
            //                    {
            //                        string AttributeName = Rule.attributeActivated.Key;

            //                        InsertedValue = EditCivilWithoutLegLibraryViewModel.GetType().GetProperties()
            //                            .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditCivilWithoutLegLibraryViewModel, null);
            //                    }
            //                    else if (Rule.dynamicAttId != null)
            //                    {
            //                        DynamicAttLibViewModel DynamicObject = EditCivilWithoutLegLibraryViewModel.DynamicAtts
            //                            .FirstOrDefault(x => x.Key.ToLower() == Rule.dynamicAtt.Key.ToLower());
            //                        InsertedValue = DynamicObject.Value;
            //                    }

            //                    if (InsertedValue == null)
            //                        break;

            //                    if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
            //                        RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
            //                        RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
            //                        RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
            //                            InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
            //                        RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
            //                        RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
            //                            InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
            //                    {
            //                        Succed++;
            //                    }
            //                }
            //                if (Rules.Count() == Succed)
            //                {
            //                    string DependencyValidationOperation = Dependency.Operation.Name;

            //                    object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
            //                        Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
            //                        Dependency.ValueDouble != null ? Dependency.ValueDouble :
            //                        !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

            //                    object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.Value;

            //                    if (Dependency.ValueDateTime != null)
            //                    {
            //                        DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
            //                            Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

            //                        DependencyValdiationValue = DependencyValdiationValueConverter;

            //                        DateTime InsertedDynamicAttributeValueAsObjectConverter = DateTime.Parse(InsertedDynamicAttributeValue.Value.ToString());

            //                        InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
            //                    }

            //                    if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
            //                    {
            //                        if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
            //                             DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
            //                             DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
            //                             DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
            //                                 Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
            //                             DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
            //                             DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
            //                                 Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
            //                        {
            //                            string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
            //                                (DependencyValidationOperation == "!=" ? "not equal to" :
            //                                (DependencyValidationOperation == ">" ? "bigger than" :
            //                                (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
            //                                (DependencyValidationOperation == "<" ? "smaller than" :
            //                                (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

            //                            return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString().ToLower())
            //{
            //    EditCivilNonSteelLibraryViewModel EditCivilNonSteelLibraryViewModels = _mapper.Map<EditCivilNonSteelLibraryViewModel>(Input);

            //    List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
            //        .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == CivilType.ToLower() && !x.disable
            //            , x => x.tablesNames).ToList());

            //    foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
            //    {
            //        TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
            //            x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
            //                x => x.Operation, x => x.DynamicAtt);

            //        if (Dependency != null)
            //        {
            //            DynamicAttLibViewModel InsertedDynamicAttributeValue = EditCivilNonSteelLibraryViewModels.DynamicAtts
            //                .FirstOrDefault(x => x.Key.ToLower() == DynamicAttribute.Key.ToLower());

            //            if (InsertedDynamicAttributeValue == null)
            //                return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

            //            List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

            //            foreach (int RowId in RowsIds)
            //            {
            //                List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
            //                    , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

            //                int Succed = 0;

            //                foreach (TLIrule Rule in Rules)
            //                {
            //                    string RuleOperation = Rule.Operation.Name;
            //                    object RuleValue = new object();

            //                    if (Rule.OperationValueBoolean != null)
            //                        RuleValue = Rule.OperationValueBoolean;

            //                    else if (Rule.OperationValueDateTime != null)
            //                        RuleValue = Rule.OperationValueDateTime;

            //                    else if (Rule.OperationValueDouble != null)
            //                        RuleValue = Rule.OperationValueDouble;

            //                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
            //                        RuleValue = Rule.OperationValueString;

            //                    object InsertedValue = new object();

            //                    if (Rule.attributeActivatedId != null)
            //                    {
            //                        string AttributeName = Rule.attributeActivated.Key;

            //                        InsertedValue = EditCivilNonSteelLibraryViewModels.GetType().GetProperties()
            //                            .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditCivilNonSteelLibraryViewModels, null);
            //                    }
            //                    else if (Rule.dynamicAttId != null)
            //                    {
            //                        DynamicAttLibViewModel DynamicObject = EditCivilNonSteelLibraryViewModels.DynamicAtts
            //                            .FirstOrDefault(x => x.Key.ToLower() == Rule.dynamicAtt.Key.ToLower());
            //                        InsertedValue = DynamicObject.Value;
            //                    }

            //                    if (InsertedValue == null)
            //                        break;

            //                    if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
            //                        RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
            //                        RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
            //                        RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
            //                            InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
            //                        RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
            //                        RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
            //                            InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
            //                    {
            //                        Succed++;
            //                    }
            //                }
            //                if (Rules.Count() == Succed)
            //                {
            //                    string DependencyValidationOperation = Dependency.Operation.Name;

            //                    object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
            //                        Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
            //                        Dependency.ValueDouble != null ? Dependency.ValueDouble :
            //                        !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

            //                    object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.Value;

            //                    if (Dependency.ValueDateTime != null)
            //                    {
            //                        DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
            //                            Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

            //                        DependencyValdiationValue = DependencyValdiationValueConverter;

            //                        DateTime InsertedDynamicAttributeValueAsObjectConverter = DateTime.Parse(InsertedDynamicAttributeValue.Value.ToString());

            //                        InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
            //                    }

            //                    if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
            //                    {
            //                        if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
            //                             DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
            //                             DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
            //                             DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
            //                                 Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
            //                             DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
            //                             DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
            //                                 Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
            //                        {
            //                            string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
            //                                (DependencyValidationOperation == "!=" ? "not equal to" :
            //                                (DependencyValidationOperation == ">" ? "bigger than" :
            //                                (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
            //                                (DependencyValidationOperation == "<" ? "smaller than" :
            //                                (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

            //                            return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            return string.Empty;
        }
        public string CheckGeneralValidationFunctionEditApiVersion(List<DynamicAttLibViewModel> TLIdynamicAttLibValue, string TableName, int? catid = null)
            {
                List<DynamicAttViewModel> DynamicAttributes = new List<DynamicAttViewModel>();

                if (catid != null)
                {
                    DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                        .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable && x.CivilWithoutLegCategoryId == catid
                            , x => x.tablesNames).ToList());
                }
                else
                {
                    DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                        .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable
                            , x => x.tablesNames).ToList());
                }

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
        public string CheckGeneralValidationFunctionEditApiVersions(List<AddDdynamicAttributeInstallationValueViewModel> TLIdynamicAttLibValue, string TableName, int? catid = null)
        {
            List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes = new List<AddDdynamicAttributeInstallationValueViewModel>();

            if (catid != null)
            {
                DynamicAttributes = _mapper.Map<List<AddDdynamicAttributeInstallationValueViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable && x.CivilWithoutLegCategoryId == catid
                        , x => x.tablesNames).ToList());
            }
            else
            {
                DynamicAttributes = _mapper.Map<List<AddDdynamicAttributeInstallationValueViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());
            }

            foreach (AddDdynamicAttributeInstallationValueViewModel DynamicAttribute in DynamicAttributes)
            {
                TLIvalidation Validation = _unitOfWork.ValidationRepository
                    .GetIncludeWhereFirst(x => x.DynamicAtt.Id == DynamicAttribute.id, x => x.Operation, x => x.DynamicAtt);

                if (Validation != null)
                {
                    string OperationName = Validation.Operation.Name;

                    AddDdynamicAttributeInstallationValueViewModel TestValue = TLIdynamicAttLibValue.FirstOrDefault(x => x.id == DynamicAttribute.id);

                    if (TestValue == null)
                        return $"({Validation.DynamicAtt.Key}) value can't be null and must be inserted";

                    object InputDynamicValue = TestValue.value;

                    object ValidationValue = new object();

                    if (Validation.ValueBoolean != null)
                    {
                        ValidationValue = Validation.ValueBoolean;
                        InputDynamicValue = bool.Parse(TestValue.value.ToString());
                    }

                    else if (Validation.ValueDateTime != null)
                    {
                        ValidationValue = Validation.ValueDateTime;
                        InputDynamicValue = DateTime.Parse(TestValue.value.ToString());
                    }

                    else if (Validation.ValueDouble != null)
                    {
                        ValidationValue = Validation.ValueDouble;
                        InputDynamicValue = double.Parse(TestValue.value.ToString());
                    }

                    else if (!string.IsNullOrEmpty(Validation.ValueString))
                    {
                        ValidationValue = Validation.ValueString;
                        InputDynamicValue = TestValue.value.ToString();
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
        public void EditLogisticalItem(int UserId,AddLogisticalViewModel LogisticalItemIds, dynamic MainEntity, int TableNameEntityId, AddLogisticalViewModel OldLogisticalItemIds)
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId,OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId,NewLogisticalItem);
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
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId,NewLogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId,OldValueMWRFU, LogisticalItem);
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
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId,NewLogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId,OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId,NewLogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId,NewLogisticalItem);
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
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId,NewLogisticalItem);
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
        public void EditLogisticalItemH(int UserId, AddLogisticalViewModel LogisticalItemIds, dynamic MainEntity, int TableNameEntityId, AddLogisticalViewModel OldLogisticalItemIds,int HistoryId)
        {
           
                try
                {
                    if (LogisticalItemIds != null)
                    {
                    var TabelNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIlogisticalitem").Id;
                    if (LogisticalItemIds.Vendor != null && LogisticalItemIds.Vendor != 0 || LogisticalItemIds.Vendor == null)
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
                     
                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId, HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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
                                _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
                            _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if ((LogisticalItemIds.Supplier != null && LogisticalItemIds.Supplier != 0)|| LogisticalItemIds.Supplier==null)
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId, HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem,HistoryId);
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
                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
                                _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Designer != null && LogisticalItemIds.Designer != 0 || LogisticalItemIds.Designer == null)
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId, HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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
                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
                                _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Manufacturer != null && LogisticalItemIds.Manufacturer != 0 || LogisticalItemIds.Manufacturer == null)
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId, HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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
                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
                                _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Consultant != null && LogisticalItemIds.Consultant != 0 || LogisticalItemIds.Consultant == null)
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId, HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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
                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
                                _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Contractor != null && LogisticalItemIds.Contractor != 0 || LogisticalItemIds.Contractor == null)
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId, HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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
                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
                            }
                            }
                            
                        }

                    }
                }
                catch (Exception)
                {
                    throw;
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
        public void EditLogisticalItemsH(int UserId, LogisticalObject LogisticalItemIds, dynamic MainEntity, int TableNameEntityId, LogisticalObject OldLogisticalItemIds, int HistoryId)
        {
            using (TransactionScope transaction2 =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    if (LogisticalItemIds != null)
                    {
                        var TabelNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIlogisticalitem").Id;
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId,HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);

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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem,HistoryId);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetWhereFirst(x => x.Id == LogisticalItemIds.Vendor);

                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MainEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId, HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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
                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId, HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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
                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId, HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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
                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId, HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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
                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHLogic(UserId, HistoryId, TabelNameId, OldValueMWRFU, LogisticalItem);
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

                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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
                                    _unitOfWork.LogisticalitemRepository.AddWithHDynamic(UserId, TabelNameId, NewLogisticalItem, HistoryId);
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
        #endregion
        //Function take 2 parameters 
        //First Id to get record by Id
        //Second TableName to specify table i deal with
        //Function return list of activated attributes with value, list  dynamic attributes
        public Response<GetForAddCivilLibrarybject> GetCivilWithLegsLibraryById(int Id, string TableName)
        {
            try
            {
                GetForAddCivilLibrarybject attributes = new GetForAddCivilLibrarybject();

                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c =>
                    c.TableName == TableName);

                TLIcivilWithLegLibrary CivilWithLegLibrary = _unitOfWork.CivilWithLegLibraryRepository.GetIncludeWhereFirst(x =>
                    x.Id == Id && !x.Deleted, x => x.sectionsLegType, x => x.supportTypeDesigned, x => x.structureType, x => x.civilSteelSupportCategory);
                if (CivilWithLegLibrary != null)
                {
                    object FK_CivilSteelSupportCategory_Name = CivilWithLegLibrary.civilSteelSupportCategory != null ? CivilWithLegLibrary.civilSteelSupportCategory.Name : null;
                    List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TableName, CivilWithLegLibrary, null).ToList();
                    listofAttributesActivated
                        .Where(FKitem => FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Label))
                        .ToList()
                        .Select(FKitem =>
                        {
                            if (FKitem.Label.ToLower() == "sectionslegtype_name")
                            {
                                FKitem.Options = _mapper.Map<List<SectionsLegTypeViewModel>>(_unitOfWork.SectionsLegTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                                FKitem.Value = _mapper.Map<SectionsLegTypeViewModel>(CivilWithLegLibrary.sectionsLegType);
                            }
                            else if (FKitem.Label.ToLower() == "structuretype_name")
                            {
                                FKitem.Options = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.StructureTypeRepository.GetWhere(x => !x.Deleted && !x.Disable && x.Type == 1).ToList());
                                FKitem.Value = _mapper.Map<StructureTypeViewModel>(CivilWithLegLibrary.structureType);
                            }
                            else if (FKitem.Label.ToLower() == "supporttypedesigned_name")
                            {
                                FKitem.Options = _mapper.Map<List<SupportTypeDesignedViewModel>>(_unitOfWork.SupportTypeDesignedRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                                FKitem.Value = _mapper.Map<SupportTypeDesignedViewModel>(CivilWithLegLibrary.supportTypeDesigned);
                            }
                            return FKitem;
                        })
                        .ToList();
                    var LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticals(Helpers.Constants.TablePartName.CivilSupport.ToString(), TableName, Id);
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
                        NameAttribute.Value = db.MV_CIVIL_WITHLEG_LIBRARY_VIEW.FirstOrDefault(x => x.Id == Id)?.Model;
                    }
                }
                else
                {
                    return new Response<GetForAddCivilLibrarybject>(false, null, null, "this civil is not found", (int)Helpers.Constants.ApiReturnCode.success);
                }
                return new Response<GetForAddCivilLibrarybject>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters 
        //First TableName to specify the table i deal with
        //Second CivilWithoutLegCategoryId to specify the category of civil Without Leg Library 
        //Function return all records depened on TableName
        public Response<GetForAddCivilLibrarybject> GetCivilWithoutLegsLibraryById(int Id, string TableName,int CategoryId)
        {
            int CatId = 0;
            try
            {
                GetForAddCivilLibrarybject attributes = new GetForAddCivilLibrarybject();

                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c =>
                    c.TableName == TableName);

                TLIcivilWithoutLegLibrary CivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository.GetIncludeWhereFirst(x =>
                          x.Id == Id && !x.Deleted, x => x.CivilSteelSupportCategory, x => x.CivilWithoutLegCategory, x => x.InstCivilwithoutLegsType, x => x.structureType);
                CatId = (int)CivilWithoutLegLibrary.CivilWithoutLegCategoryId;
                if (CivilWithoutLegLibrary != null)
                {
                    List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TableName, CivilWithoutLegLibrary, null, "CivilWithoutLegCategoryId").ToList();
                    listofAttributesActivated
                        .Where(FKitem => FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Label))
                        .ToList()
                        .Select(FKitem =>
                        {
                            if (FKitem.Label.ToLower() == "structuretype_name")
                            {
                                FKitem.Options = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.StructureTypeRepository.GetWhere(x => !x.Deleted && !x.Disable && x.Type == 2).ToList());
                                FKitem.Value = _mapper.Map<StructureTypeViewModel>(CivilWithoutLegLibrary.structureType);
                            }
                            else if (FKitem.Label.ToLower() == "instcivilwithoutlegstype_name")
                            {
                                FKitem.Options = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.InstCivilwithoutLegsTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                                FKitem.Value = _mapper.Map<StructureTypeViewModel>(CivilWithoutLegLibrary.InstCivilwithoutLegsType);
                            }
                            else if (FKitem.Label.ToLower() == "civilwithoutlegcategory_name")
                            {
                                FKitem.Options = _mapper.Map<List<CivilWithoutLegCategoryViewModel>>(_unitOfWork.CivilWithoutLegCategoryRepository.GetWhere(x => !x.disable).ToList());
                                FKitem.Value = _mapper.Map<CivilWithoutLegCategoryViewModel>(CivilWithoutLegLibrary.CivilWithoutLegCategory);
                            }

                            return FKitem;
                        })
                        .ToList();
                    var LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticals(Helpers.Constants.TablePartName.CivilSupport.ToString(), TableName, Id);
                    attributes.LogisticalItems = LogisticalItems;
                    attributes.AttributesActivatedLibrary = listofAttributesActivated;

                    attributes.DynamicAttributes = _unitOfWork.DynamicAttLibRepository.GetDynamicLibAtt(TableNameEntity.Id, Id, CategoryId);

                    List<BaseInstAttViews> Test = attributes.AttributesActivatedLibrary.ToList();
                    BaseInstAttViews NameAttribute = Test.FirstOrDefault(x => x.Key.ToLower() == "Model".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = Test.ToList()[0];
                        Test[Test.IndexOf(NameAttribute)] = Swap;
                        Test[0] = NameAttribute;
                        attributes.AttributesActivatedLibrary = Test;
                        NameAttribute.Value = db.MV_CIVIL_WITHOUTLEG_LIBRARY_VIEW.FirstOrDefault(x => x.Id == Id)?.Model;
                    }
                }
                else
                {
                    return new Response<GetForAddCivilLibrarybject>(false, null, null, "this civil is not found", (int)Helpers.Constants.ApiReturnCode.success);
                }

                return new Response<GetForAddCivilLibrarybject>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<GetForAddCivilLibrarybject> GetCivilNonSteelLibraryById(int Id, string TableName)
        {
            int CatId = 0;
            try
            {
                GetForAddCivilLibrarybject attributes = new GetForAddCivilLibrarybject();

                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c =>
                    c.TableName == TableName);

                TLIcivilNonSteelLibrary CivilNonSteelLibrary = _unitOfWork.CivilNonSteelLibraryRepository.GetIncludeWhereFirst(x =>
                          x.Id == Id && !x.Deleted, x => x.civilNonSteelType);
                if (CivilNonSteelLibrary != null)
                {
                    List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TableName, CivilNonSteelLibrary, null).ToList();
                    listofAttributesActivated
                        .Where(FKitem => FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Label))
                        .ToList()
                        .Select(FKitem =>
                        {
                            if (FKitem.Label.ToLower() == "civilnonsteeltype_name")
                            {
                                FKitem.Options = _mapper.Map<List<CivilNonSteelTypeViewModel>>(_unitOfWork.CivilNonSteelTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                                FKitem.Value = _mapper.Map<CivilNonSteelTypeViewModel>(CivilNonSteelLibrary.civilNonSteelType);
                            }

                            return FKitem;
                        })
                        .ToList();
                    var LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticalsNonSteel(Helpers.Constants.TablePartName.CivilSupport.ToString(), TableName, Id);
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
                        NameAttribute.Value = db.MV_CIVIL_NONSTEEL_LIBRARY_VIEW.FirstOrDefault(x => x.Id == Id)?.Model;
                    }
                }
                else
                {
                    return new Response<GetForAddCivilLibrarybject>(false, null, null, "this civil is not found", (int)Helpers.Constants.ApiReturnCode.success);
                }

                return new Response<GetForAddCivilLibrarybject>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<IEnumerable<LibraryNamesViewModel>> GetCivilLibraryByType(string TableName, int? CivilWithoutLegCategoryId = null)
        {
            try
            {
                int count = 0;
                List<LibraryNamesViewModel> LibraryNames = new List<LibraryNamesViewModel>();
                if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == TableName)
                {
                    var CivilWithLegsLib = _unitOfWork.CivilWithLegLibraryRepository.GetAll(out count).ToList();
                    LibraryNames = _mapper.Map<List<LibraryNamesViewModel>>(CivilWithLegsLib);
                }
                else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == TableName)
                {

                    var CivilWithoutLegsLib = _unitOfWork.CivilWithoutLegLibraryRepository.GetWhere(c => c.CivilWithoutLegCategoryId == CivilWithoutLegCategoryId).ToList();
                    count = CivilWithoutLegsLib.Count();
                    LibraryNames = _mapper.Map<List<LibraryNamesViewModel>>(CivilWithoutLegsLib);
                }
                else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == TableName)
                {
                    var CivilNonSteelLib = _unitOfWork.CivilNonSteelLibraryRepository.GetAll(out count).ToList();
                    LibraryNames = _mapper.Map<List<LibraryNamesViewModel>>(CivilNonSteelLib);
                }
                return new Response<IEnumerable<LibraryNamesViewModel>>(true, LibraryNames, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<IEnumerable<LibraryNamesViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        //Function take 2 parameters 
        //First filters to filter data before i get it
        //Second parameters to paginate the records
        //Function return list of filtered CivilNonSteelLibraries 
        public Response<ReturnWithFilters<CivilNonSteelLibraryViewModel>> getCivilNonSteelLibraries(List<FilterObjectList> filters, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                IEnumerable<TLIcivilNonSteelLibrary> civilNonSteelLibraries;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                civilNonSteelLibraries = _unitOfWork.CivilNonSteelLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, null);
                var FilteredcivilNonSteelLibrariesModel = _mapper.Map<IEnumerable<CivilNonSteelLibraryViewModel>>(civilNonSteelLibraries);
                ReturnWithFilters<CivilNonSteelLibraryViewModel> CivilNonSteel = new ReturnWithFilters<CivilNonSteelLibraryViewModel>();
                CivilNonSteel.Model = FilteredcivilNonSteelLibrariesModel.ToList();
                return new Response<ReturnWithFilters<CivilNonSteelLibraryViewModel>>(true, CivilNonSteel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<CivilNonSteelLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters 
        //First filters to filter data before i get it
        //Second WithFilterData to specify if forntend want to get records with filters or not
        //Third parameters to paginate the records
        //Function return list of filtered CivilWithLegLibraries 
        public Response<ReturnWithFilters<CivilWithLegLibraryViewModel>> getCivilWithLegLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                IEnumerable<TLIcivilWithLegLibrary> CivilWithLegLibrariesList;
                List<FilterObject> condition = new List<FilterObject>();
                //condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                CivilWithLegLibrariesList = _unitOfWork.CivilWithLegLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, c => c.civilSteelSupportCategory, c => c.sectionsLegType, c => c.structureType, c => c.supportTypeDesigned);
                var FilteredCivilWithLegLibrariesModel = _mapper.Map<IEnumerable<CivilWithLegLibraryViewModel>>(CivilWithLegLibrariesList);
                ReturnWithFilters<CivilWithLegLibraryViewModel> CivilWithleg = new ReturnWithFilters<CivilWithLegLibraryViewModel>();
                CivilWithleg.Model = FilteredCivilWithLegLibrariesModel.ToList();
                if (WithFilterData.Equals(true))
                {
                    CivilWithleg.filters = _unitOfWork.CivilWithLegLibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<CivilWithLegLibraryViewModel>>(true, CivilWithleg, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<CivilWithLegLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        //Function take 3 parameters 
        //First filters to filter data before i get it
        //Second WithFilterData to specify if forntend want to get records with filters or not
        //Third parameters to paginate the records
        //Function return list of filtered CivilWithoutLegLibraries 
        public Response<ReturnWithFilters<CivilWithoutLegLibraryViewModel>> getCivilWithoutLegLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                IEnumerable<TLIcivilWithoutLegLibrary> civilWithoutLegLibraries;
                List<FilterObject> condition = new List<FilterObject>();
                // condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                civilWithoutLegLibraries = _unitOfWork.CivilWithoutLegLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, c => c.CivilSteelSupportCategory, c => c.CivilWithoutLegCategory, c => c.InstCivilwithoutLegsType).OrderBy(x => x.Id).ToList();
                var FilteredcivilWithoutLegLibrariesModel = _mapper.Map<IEnumerable<CivilWithoutLegLibraryViewModel>>(civilWithoutLegLibraries);
                ReturnWithFilters<CivilWithoutLegLibraryViewModel> civilWithoutLeg = new ReturnWithFilters<CivilWithoutLegLibraryViewModel>();
                civilWithoutLeg.Model = FilteredcivilWithoutLegLibrariesModel.ToList();
                if (WithFilterData.Equals(true))
                {
                    civilWithoutLeg.filters = _unitOfWork.CivilWithoutLegLibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<CivilWithoutLegLibraryViewModel>>(true, civilWithoutLeg, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<CivilWithoutLegLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters
        //First CivilType to specify table i deal with
        //Second CategoryId to specify the category of civilWithoutLegLibrary
        //Function return activated attributes and dynamic attributes
        public Response<GetForAddCivilLibrarybject> GetForAddCivilWithoutMastLibrary (string TableName)
        {
            try
            {
                GetForAddCivilLibrarybject Attributes = new GetForAddCivilLibrarybject();
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);
                List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TableName, null, 1, "Model", "CivilSteelSupportCategoryId").ToList();
                listofAttributesActivated
                  .Where(FKitem => FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                  .ToList()
                  .Select(FKitem =>
                  {
                      if (FKitem.Label.ToLower() == "structuretype_name")
                          FKitem.Options = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.StructureTypeRepository.GetWhere(x => !x.Deleted && !x.Disable && x.Type==2).ToList());
                      else if (FKitem.Label.ToLower() == "installationcivilwithoutlegstype_name")
                       FKitem.Options = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.InstCivilwithoutLegsTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                      else if (FKitem.Label.ToLower() == "civilwithoutlegcategory_name")
                          FKitem.Options = _mapper.Map<List<CivilWithoutLegCategoryViewModel>>(_unitOfWork.CivilWithoutLegCategoryRepository.GetWhere(x => !x.disable).ToList());
                      else if (FKitem.Label.ToLower() == "civilsteelsupportcategory_name")
                          FKitem.Options = _mapper.Map<List<CivilSteelSupportCategoryViewModel>>(db.TLIcivilSteelSupportCategory.ToList());

                      return FKitem;
                  })
                  .ToList();
                var LogisticalAttributes = _unitOfWork.LogistcalRepository.GetLogisticalLibrary(Helpers.Constants.TablePartName.CivilSupport.ToString());
                Attributes.LogisticalItems = LogisticalAttributes;
                Attributes.AttributesActivatedLibrary = listofAttributesActivated;

                IEnumerable<BaseInstAttViewDynamic> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                    .GetDynamicLibAtt(TableNameEntity.Id, 1)
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

                Attributes.DynamicAttributes = DynamicAttributesWithoutValue;

                List<BaseInstAttViews> Test = Attributes.AttributesActivatedLibrary.ToList();
                BaseInstAttViews NameAttribute = Test.FirstOrDefault(x => x.Key.ToLower() == "Model".ToLower());
                if (NameAttribute != null)
                {
                    BaseInstAttViews Swap = Test.ToList()[0];
                    Test[Test.IndexOf(NameAttribute)] = Swap;
                    Test[0] = NameAttribute;
                    Attributes.AttributesActivatedLibrary = Test;
                }

                return new Response<GetForAddCivilLibrarybject>(true, Attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        
        }
        public Response<GetForAddCivilLibrarybject> GetForAddCivilWithoutMonopleLibrary(string TableName)
        {
            try
            {
                GetForAddCivilLibrarybject Attributes = new GetForAddCivilLibrarybject();
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);
                List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TableName, null, 3, "Model", "CivilSteelSupportCategoryId").ToList();
                listofAttributesActivated
                  .Where(FKitem => FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                  .ToList()
                  .Select(FKitem =>
                  {
                      if (FKitem.Label.ToLower() == "structuretype_name")
                          FKitem.Options = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.StructureTypeRepository.GetWhere(x => !x.Deleted && !x.Disable && x.Type==2).ToList());
                      else if (FKitem.Label.ToLower() == "installationcivilwithoutlegstype_name")
                          FKitem.Options = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.InstCivilwithoutLegsTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                      else if (FKitem.Label.ToLower() == "civilwithoutlegcategory_name")
                          FKitem.Options = _mapper.Map<List<CivilWithoutLegCategoryViewModel>>(_unitOfWork.CivilWithoutLegCategoryRepository.GetWhere(x => !x.disable).ToList());
                      else if (FKitem.Label.ToLower() == "civilsteelsupportcategory_name")
                          FKitem.Options = _mapper.Map<List<CivilSteelSupportCategoryViewModel>>(db.TLIcivilSteelSupportCategory.ToList());

                      return FKitem;
                  })
                  .ToList();

                var LogisticalAttributes = _unitOfWork.LogistcalRepository.GetLogisticalLibrary(Helpers.Constants.TablePartName.CivilSupport.ToString());
                Attributes.LogisticalItems = LogisticalAttributes;
                Attributes.AttributesActivatedLibrary = listofAttributesActivated;

                IEnumerable<BaseInstAttViewDynamic> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                    .GetDynamicLibAtt(TableNameEntity.Id, 3)
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

                Attributes.DynamicAttributes = DynamicAttributesWithoutValue;

                List<BaseInstAttViews> Test = Attributes.AttributesActivatedLibrary.ToList();
                BaseInstAttViews NameAttribute = Test.FirstOrDefault(x => x.Key.ToLower() == "Model".ToLower());
                if (NameAttribute != null)
                {
                    BaseInstAttViews Swap = Test.ToList()[0];
                    Test[Test.IndexOf(NameAttribute)] = Swap;
                    Test[0] = NameAttribute;
                    Attributes.AttributesActivatedLibrary = Test;
                }

                return new Response<GetForAddCivilLibrarybject>(true, Attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<GetForAddCivilLibrarybject> GetForAddCivilWithoutCapsuleLibrary(string TableName)
        {
            try
            {
                GetForAddCivilLibrarybject Attributes = new GetForAddCivilLibrarybject();
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);
                List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TableName, null, 2, "Model", "CivilSteelSupportCategoryId").ToList();
                listofAttributesActivated
                  .Where(FKitem => FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Label))
                  .ToList()
                  .Select(FKitem =>
                  {
                      if (FKitem.Label.ToLower() == "structuretype_name")
                          FKitem.Options = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.StructureTypeRepository.GetWhere(x => !x.Deleted && !x.Disable && x.Type == 2).ToList());
                      else if (FKitem.Label.ToLower() == "installationcivilwithoutlegstype_name")
                          FKitem.Options = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.InstCivilwithoutLegsTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                      else if (FKitem.Label.ToLower() == "civilwithoutlegcategory_name")
                          FKitem.Options = _mapper.Map<List<CivilWithoutLegCategoryViewModel>>(_unitOfWork.CivilWithoutLegCategoryRepository.GetWhere(x => !x.disable).ToList());
                      else if (FKitem.Label.ToLower() == "civilsteelsupportcategory_name")
                          FKitem.Options = _mapper.Map<List<CivilSteelSupportCategoryViewModel>>(db.TLIcivilSteelSupportCategory.ToList());

                      return FKitem;
                  })
                  .ToList();

                var LogisticalAttributes = _unitOfWork.LogistcalRepository.GetLogisticalLibrary(Helpers.Constants.TablePartName.CivilSupport.ToString());
                Attributes.LogisticalItems = LogisticalAttributes;
                Attributes.AttributesActivatedLibrary = listofAttributesActivated;

                IEnumerable<BaseInstAttViewDynamic> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                    .GetDynamicLibAtt(TableNameEntity.Id, 2)
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

                Attributes.DynamicAttributes = DynamicAttributesWithoutValue;

                List<BaseInstAttViews> Test = Attributes.AttributesActivatedLibrary.ToList();
                BaseInstAttViews NameAttribute = Test.FirstOrDefault(x => x.Key.ToLower() == "Model".ToLower());
                if (NameAttribute != null)
                {
                    BaseInstAttViews Swap = Test.ToList()[0];
                    Test[Test.IndexOf(NameAttribute)] = Swap;
                    Test[0] = NameAttribute;
                    Attributes.AttributesActivatedLibrary = Test;
                }

                return new Response<GetForAddCivilLibrarybject>(true, Attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<GetForAddCivilLibrarybject> GetForAdd(string TableName)
        {
            try
           {
                GetForAddCivilLibrarybject Attributes = new GetForAddCivilLibrarybject();

                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);

                if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == TableName)
                {
                    List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TableName, null, null, "Model").ToList();
                    listofAttributesActivated
                      .Where(FKitem => FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Label))
                      .ToList()
                      .Select(FKitem =>
                      {
                          if (FKitem.Label.ToLower() == "sectionslegtype_name")
                              FKitem.Options = _mapper.Map<List<SectionsLegTypeViewModel>>(_unitOfWork.SectionsLegTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                          else if (FKitem.Label.ToLower() == "structuretype_name")
                              FKitem.Options = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.StructureTypeRepository.GetWhere(x => !x.Deleted && !x.Disable && x.Type==1).ToList());
                          else if (FKitem.Label.ToLower() == "supporttypedesigned_name")
                              FKitem.Options = _mapper.Map<List<SupportTypeDesignedViewModel>>(_unitOfWork.SupportTypeDesignedRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                          return FKitem;
                      })
                      .ToList();
                    var LogisticalAttributes =_unitOfWork.LogistcalRepository.GetLogisticalLibrary(Helpers.Constants.TablePartName.CivilSupport.ToString());
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
                                    "bool" => bool.Parse(DynamicAttributeEntity.DefaultValue),
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
                }

                else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == TableName)
                {
                    var activatedAttributes = _unitOfWork.AttributeActivatedRepository
                    .GetAttributeActivatedGetForAdd(TableName, null, null)
                    .Select(item =>
                    {
                        if (item.DataType.ToLower() == "list" && item.Label?.ToLower() == "civilnonsteeltype_name")
                            item.Options = _mapper.Map<List<CivilNonSteelTypeViewModel>>(
                                db.TLIcivilNonSteelType.Where(x => !x.Disable && !x.Deleted).ToList());
                        return item;
                    })
                    .ToList();
                    var LogisticalAttributes=_unitOfWork.LogistcalRepository.GetLogisticalLibraryNonSteel("CivilSupport");
                    Attributes.LogisticalItems = LogisticalAttributes;
                    Attributes.AttributesActivatedLibrary = activatedAttributes;

                    Attributes.DynamicAttributes = _unitOfWork.DynamicAttRepository
                    .GetDynamicLibAtt(TableNameEntity.Id, null)
                    .Select(DynamicAttribute =>
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);
                        DynamicAttribute.Value = !string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue) ?
                            DynamicAttribute.DataType.ToLower() switch
                            {
                                "string" => DynamicAttributeEntity.DefaultValue,
                                "int" => int.Parse(DynamicAttributeEntity.DefaultValue),
                                "double" => double.Parse(DynamicAttributeEntity.DefaultValue),
                                "bool" => bool.Parse(DynamicAttributeEntity.DefaultValue),
                                "datetime" => DateTime.Parse(DynamicAttributeEntity.DefaultValue),
                                _ => " ".Split(' ')[0]
                            } :
                            " ".Split(' ')[0];
                        return DynamicAttribute;
                    })
                    .ToList();

                }

                //// Microwave Load Libraries
                //else if (Helpers.Constants.TablesNames.TLImwRFULibrary.ToString() == TableName)
                //{
                //    List<BaseAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository
                //    .GetAttributeActivatedGetForAdd(TableName, null, null, "Model")
                //    .Select(FKitem =>
                //    {
                //        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                //        {
                //            switch (FKitem.Desc.ToLower())
                //            {
                //                case "tlidiversitytype":
                //                    FKitem.Value = _mapper.Map<List<DiversityTypeViewModel>>(
                //                        _unitOfWork.DiversityTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                //                    break;
                //                case "tliboardtype":
                //                    FKitem.Value = _mapper.Map<List<BoardTypeViewModel>>(
                //                        _unitOfWork.BoardTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                //                    break;
                //            }
                //        }
                //        return FKitem;
                //    })
                //    .ToList();

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogisticals(Helpers.Constants.TablePartName.MW.ToString()));
                //    Attributes.AttributesActivatedLibrary = listofAttributesActivated;

                //    IEnumerable<BaseInstAttViewDynamic> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //    .GetDynamicLibAtt(TableNameEntity.Id, null)
                //    .Select(DynamicAttribute =>
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);
                //        DynamicAttribute.Value = !string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue) ?
                //            DynamicAttribute.DataType.ToLower() switch
                //            {
                //                "string" => DynamicAttributeEntity.DefaultValue,
                //                "int" => int.Parse(DynamicAttributeEntity.DefaultValue),
                //                "double" => double.Parse(DynamicAttributeEntity.DefaultValue),
                //                "boolean" => bool.Parse(DynamicAttributeEntity.DefaultValue),
                //                "datetime" => DateTime.Parse(DynamicAttributeEntity.DefaultValue),
                //                _ => DynamicAttribute.Value
                //            } :
                //            " ".Split(' ')[0];
                //        return DynamicAttribute;
                //    });

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue.ToList();

                //}
                //else if (Helpers.Constants.TablesNames.TLImwBULibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();
                //    foreach (BaseAttView FKitem in listofAttributesActivated)
                //    {
                //        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                //        {
                //            if (FKitem.Desc.ToLower() == "tlidiversitytype")
                //                FKitem.Value = _mapper.Map<List<DiversityTypeViewModel>>(_unitOfWork.DiversityTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                //        }
                //    }

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.MW.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}
                //else if (Helpers.Constants.TablesNames.TLImwDishLibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    foreach (BaseAttView FKitem in listofAttributesActivated)
                //    {
                //        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                //        {
                //            if (FKitem.Desc.ToLower() == "tlipolaritytype")
                //                FKitem.Value = _mapper.Map<List<PolarityTypeViewModel>>(_unitOfWork.PolarityTypeRepository.GetAllWithoutCount().ToList());

                //            else if (FKitem.Desc.ToLower() == "tliastype")
                //                FKitem.Value = _mapper.Map<List<AsTypeViewModel>>(_unitOfWork.AsTypeRepository.GetAllWithoutCount().ToList());
                //        }
                //    }

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.MW.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}
                //else if (Helpers.Constants.TablesNames.TLImwODULibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    foreach (BaseAttView FKitem in listofAttributesActivated)
                //    {
                //        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                //        {
                //            if (FKitem.Desc.ToLower() == "tliparity")
                //                FKitem.Value = _mapper.Map<List<ParityViewModel>>(_unitOfWork.ParityRepository.GetAllWithoutCount().ToList());
                //        }
                //    }

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.MW.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}
                //else if (Helpers.Constants.TablesNames.TLImwOtherLibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.MW.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}

                //// Radio Load Libraries
                //else if (Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.Radio.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}
                //else if (Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.Radio.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}
                //else if (Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.Radio.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}

                //// Power Load Library
                //else if (Helpers.Constants.TablesNames.TLIpowerLibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.Power.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}

                //// Load Other Library
                //else if (Helpers.Constants.TablesNames.TLIloadOtherLibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.LoadOther.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}

                //// Side Arm Library
                //else if (Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.SideArm.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}

                //// Other Inventories Libraries
                //else if (Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    foreach (BaseAttView FKitem in listofAttributesActivated)
                //    {
                //        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                //        {
                //            if (FKitem.Desc.ToLower() == "tlicabinetpowertype")
                //                FKitem.Value = _mapper.Map<List<CabinetPowerTypeViewModel>>(_unitOfWork.CabinetPowerTypeRepository.GetAllWithoutCount().ToList());
                //        }
                //    }

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}
                //else if (Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    foreach (BaseAttView FKitem in listofAttributesActivated)
                //    {
                //        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                //        {
                //            if (FKitem.Desc.ToLower() == "tlitelecomtype")
                //                FKitem.Value = _mapper.Map<List<TelecomTypeViewModel>>(_unitOfWork.TelecomTypeRepository.GetAllWithoutCount().ToList());
                //        }
                //    }

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}
                //else if (Helpers.Constants.TablesNames.TLIsolarLibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    foreach (BaseAttView FKitem in listofAttributesActivated)
                //    {
                //        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                //        {
                //            if (FKitem.Desc.ToLower() == "tlicapacity")
                //                FKitem.Value = _mapper.Map<List<CapacityViewModel>>(_unitOfWork.CapacityRepository.GetAllWithoutCount().ToList());
                //        }
                //    }

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}
                //else if (Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString() == TableName)
                //{
                //    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                //    foreach (BaseAttView FKitem in listofAttributesActivated)
                //    {
                //        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                //        {
                //            if (FKitem.Desc.ToLower() == "tlicapacity")
                //                FKitem.Value = _mapper.Map<List<CapacityViewModel>>(_unitOfWork.CapacityRepository.GetAllWithoutCount().ToList());
                //        }
                //    }

                //    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString()));
                //    Attributes.AttributesActivated = listofAttributesActivated;

                //    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                //        .GetDynamicLibAtts(TableNameEntity.Id, null);

                //    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                //    {
                //        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                //        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                //        {
                //            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                //                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                //            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                //                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                //                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                //                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                //            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                //                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                //        }
                //        else
                //        {
                //            DynamicAttribute.Value = " ".Split(' ')[0];
                //        }
                //    }

                //    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                //    Attributes.DynamicAttInst = null;
                //}

                List<BaseInstAttViews> Test = Attributes.AttributesActivatedLibrary.ToList();
                BaseInstAttViews NameAttribute = Test.FirstOrDefault(x => x.Key.ToLower() == "Model".ToLower());
                if (NameAttribute != null)
                {
                    BaseInstAttViews Swap = Test.ToList()[0];
                    Test[Test.IndexOf(NameAttribute)] = Swap;
                    Test[0] = NameAttribute;
                    Attributes.AttributesActivatedLibrary = Test;
                }

                return new Response<GetForAddCivilLibrarybject>(true, Attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters 
        //First Id to specify the record i want to delete
        //Second CivilType to specify the table i deal with
        //Function Update Deleted column to true for record 
        //and Update Delete to all dynamic attributes values related to this record to true   
        public async Task<Response<AllItemAttributes>> Delete(int Id, string CivilType,int UserId,string connectionString)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == CivilType);
                    if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == CivilType)
                    {
                        var UsedCivil = _unitOfWork.CivilSiteDateRepository.GetWhereAndInclude(x => x.allCivilInst.civilWithLegs.CivilWithLegsLibId
                        == Id && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib).ToList();
                        var NewCivilWithLeg = _unitOfWork.CivilWithLegLibraryRepository.GetWhereFirst(x => x.Id == Id);
                        if (UsedCivil != null && UsedCivil.Count>0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not change status this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);
                        
                        else
                        {
                            var NewCivilWithLegLibrary = _unitOfWork.CivilWithLegLibraryRepository.GetWhereFirst(x => x.Id == Id);
                            TLIcivilWithLegLibrary OldCivilWithLegLibrary = _unitOfWork.CivilWithLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                            NewCivilWithLegLibrary.Deleted = true;
                            NewCivilWithLegLibrary.Model = NewCivilWithLegLibrary.Model + "_" + DateTime.Now.ToString();
                            var HistoryId= _unitOfWork.CivilWithLegLibraryRepository.UpdateWithH(UserId,null, OldCivilWithLegLibrary, NewCivilWithLegLibrary);
                            DisableDynamicAttLibValuesH(TableNameEntity.Id, Id,UserId, HistoryId);
                            await _unitOfWork.SaveChangesAsync();
                           
                            //AddHistory(CivilWithLeg.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString());
                        }
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == CivilType)
                    {
                        var UsedCivil = _unitOfWork.CivilSiteDateRepository.GetWhereAndInclude(x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLibId
                         == Id && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib).ToList();
                        var CivilWithoutlib = _unitOfWork.CivilWithoutLegLibraryRepository.GetWhereFirst(x => x.Id == Id);
                        if (UsedCivil != null && UsedCivil.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not change status this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);
                        
                        else
                        {
                            TLIcivilWithoutLegLibrary NewCivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository.GetWhereFirst(x=>x.Id==Id);
                            TLIcivilWithoutLegLibrary OldCivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                            NewCivilWithoutLegLibrary.Deleted = true;
                            NewCivilWithoutLegLibrary.Model = NewCivilWithoutLegLibrary.Model + "_" + DateTime.Now.ToString();
                            var HistoryId = _unitOfWork.CivilWithoutLegLibraryRepository.UpdateWithH(UserId,null, OldCivilWithoutLegLibrary, NewCivilWithoutLegLibrary);
                            DisableDynamicAttLibValuesH(TableNameEntity.Id, Id, UserId, HistoryId);
                            await _unitOfWork.SaveChangesAsync();
                        }
                      
                        //AddHistory(CivilWithoutLeg.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString());
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == CivilType)
                    {
                        var UsedCivil = _unitOfWork.CivilSiteDateRepository.GetWhereAndInclude(x => x.allCivilInst.civilNonSteel.CivilNonSteelLibraryId
                         == Id && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilNonSteel, x => x.allCivilInst.civilNonSteel.CivilNonsteelLibrary).ToList();
                        var CivilNonSteellib = _unitOfWork.CivilNonSteelLibraryRepository.GetWhereFirst(x => x.Id == Id);
                        if (UsedCivil != null && UsedCivil.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not change status this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);
                        
                        else
                        {
                            TLIcivilNonSteelLibrary NewCivilNonSteelLibrary = _unitOfWork.CivilNonSteelLibraryRepository.GetWhereFirst(x => x.Id == Id);
                            TLIcivilNonSteelLibrary OldCivilNonSteelLibrary = _unitOfWork.CivilNonSteelLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                            NewCivilNonSteelLibrary.Deleted = true;
                            NewCivilNonSteelLibrary.Model = NewCivilNonSteelLibrary.Model + "_" + DateTime.Now.ToString();
                            var HistoryId = _unitOfWork.CivilNonSteelLibraryRepository.UpdateWithH(UserId,null, OldCivilNonSteelLibrary, NewCivilNonSteelLibrary);
                            DisableDynamicAttLibValuesH(TableNameEntity.Id, Id, UserId, HistoryId);
                            await _unitOfWork.SaveChangesAsync();
                            
                            //AddHistory(CivilNonSteel.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString());
                        }
                    }
                   

                    transaction.Complete();
                    if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == CivilType)
                    {
                        Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == CivilType)
                    {
                          Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == CivilType)
                    {
                        Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                    }
                    return new Response<AllItemAttributes>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {
                    return new Response<AllItemAttributes>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
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
                    LibDynamicAttListIds.Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                DynamicLibValueListIds = new List<int>();

                List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                    LibDynamicAttListIds.Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

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
        public Response<GetEnableAttribute> GetCivilWithLegLibrariesEnabledAtt( string ConnectionString)
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
                        .Where(x => x.Enable && x.EditableManagmentView.View == "CivilWithLegsLibrary" 
                        &&((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
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
                        var query = db.MV_CIVIL_WITHLEG_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();
                      
                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = db.MV_CIVIL_WITHLEG_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                    .GroupBy(x => new
                    {
                        Id = x.Id,
                        Model = x.Model,
                        Note = x.Note,
                        Prefix = x.Prefix,
                        Height_Designed = x.Height_Designed,
                        Max_load_M2 = x.Max_load_M2,
                        SpaceLibrary = x.SpaceLibrary,
                        Active = x.Active,
                        Deleted = x.Deleted,
                        SUPPORTTYPEDESIGNED = x.SUPPORTTYPEDESIGNED,
                        SECTIONSLEGTYPE = x.SECTIONSLEGTYPE,
                        STRUCTURETYPE = x.STRUCTURETYPE,
                        CIVILSTEELSUPPORTCATEGORY = x.CIVILSTEELSUPPORTCATEGORY,
                        Manufactured_Max_Load = x.Manufactured_Max_Load,
                        WidthVariation = x.WidthVariation,
                        NumberOfLegs = x.NumberOfLegs

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
        private void DisableDynamicAttLibValues(int TableNameId, int Id,int UserId)
        {
            var DynamiAttLibValues = db.TLIdynamicAttLibValue
                .Where(d => d.InventoryId == Id && d.tablesNamesId == TableNameId)
                .ToList();
            foreach (var DynamiAttLibValue in DynamiAttLibValues)
            {
                var OldDynamiAttLibValues = _unitOfWork.DynamicAttLibValueRepository.GetAllAsQueryable().AsNoTracking()
                .FirstOrDefault(d => d.Id == DynamiAttLibValue.Id);
                DynamiAttLibValue.disable = true;
                _unitOfWork.DynamicAttLibValueRepository.UpdateWithHistory(UserId, OldDynamiAttLibValues, DynamiAttLibValue);
            }
        }
        private void DisableDynamicAttLibValuesH(int TableNameId, int Id, int UserId,int HistorId)
        {
            var DynamiAttLibValues = db.TLIdynamicAttLibValue
                .Where(d => d.InventoryId == Id && d.tablesNamesId == TableNameId)
                .ToList();
            var TabelName = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIdynamicAttLibValue").Id;
            foreach (var DynamiAttLibValue in DynamiAttLibValues)
            {
                var OldDynamiAttLibValues = _unitOfWork.DynamicAttLibValueRepository.GetAllAsQueryable().AsNoTracking()
                .FirstOrDefault(d => d.Id == DynamiAttLibValue.Id);
                DynamiAttLibValue.disable = true;
                _unitOfWork.DynamicAttLibValueRepository.UpdateWithHLogic(UserId, HistorId, TabelName, OldDynamiAttLibValues, DynamiAttLibValue);
            }
        }
        public Response<GetEnableAttribute> GetCivilWithoutLegMastLibrariesEnabledAtt(  string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    //string storedProcedureName = "create_dynamic_pivot_withoutleg_library ";
                    //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    //{
                    //    procedureCommand.CommandType = CommandType.StoredProcedure;
                    //    procedureCommand.ExecuteNonQuery();
                    //}
                    var attActivated = db.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "CivilWithoutLegsLibraryMast" && ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
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
                        var query = db.MV_CIVIL_WITHOUTLEG_LIBRARY_VIEW.Where(x => !x.Deleted && x.CIVILWITHOUTLEGCATEGORY.ToLower() == "mast").AsEnumerable()
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();
                      
                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = db.MV_CIVIL_WITHOUTLEG_LIBRARY_VIEW.Where(x => !x.Deleted && x.CIVILWITHOUTLEGCATEGORY.ToLower() == "mast").AsEnumerable()
                    .GroupBy(x => new
                    {
                        Id = x.Id,
                        Model = x.Model,
                        Note = x.Note,
                        Prefix = x.Prefix,
                        Height_Designed = x.Height_Designed,
                        Max_Load = x.Max_Load,
                        SpaceLibrary = x.SpaceLibrary,
                        Active = x.Active,
                        Deleted = x.Deleted,
                        CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                        INSTCIVILWITHOUTLEGSTYPE = x.INSTCIVILWITHOUTLEGSTYPE,
                        CIVILSTEELSUPPORTCATEGORY = x.CIVILSTEELSUPPORTCATEGORY,
                        STRUCTURETYPE = x.STRUCTURETYPE,
                        Manufactured_Max_Load = x.Manufactured_Max_Load,
                        HeightBase = x.HeightBase,
                        WidthVariation = x.WidthVariation

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
        public Response<GetEnableAttribute> GetCivilWithoutLegMonopoleLibrariesEnabledAtt( string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    //string storedProcedureName = "create_dynamic_pivot_withoutleg_library ";
                    //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    //{
                    //    procedureCommand.CommandType = CommandType.StoredProcedure;
                    //    procedureCommand.ExecuteNonQuery();
                    //}
                    var attActivated = db.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "CivilWithoutLegsLibraryMonopole" && ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
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
                        var query = db.MV_CIVIL_WITHOUTLEG_LIBRARY_VIEW.Where(x => !x.Deleted && x.CIVILWITHOUTLEGCATEGORY.ToLower() == "monopole").AsEnumerable()
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();
                       
                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = db.MV_CIVIL_WITHOUTLEG_LIBRARY_VIEW.Where(x => !x.Deleted && x.CIVILWITHOUTLEGCATEGORY.ToLower() == "monopole").AsEnumerable()
                    .GroupBy(x => new
                    {
                        Id = x.Id,
                        Model = x.Model,
                        Note = x.Note,
                        Prefix = x.Prefix,
                        Height_Designed = x.Height_Designed,
                        Max_Load = x.Max_Load,
                        SpaceLibrary = x.SpaceLibrary,
                        Active = x.Active,
                        Deleted = x.Deleted,
                        CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                        INSTCIVILWITHOUTLEGSTYPE = x.INSTCIVILWITHOUTLEGSTYPE,
                        CIVILSTEELSUPPORTCATEGORY = x.CIVILSTEELSUPPORTCATEGORY,
                        STRUCTURETYPE = x.STRUCTURETYPE,
                        Manufactured_Max_Load = x.Manufactured_Max_Load,
                        HeightBase = x.HeightBase,
                        WidthVariation = x.WidthVariation

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
        public Response<GetEnableAttribute> GetCivilWithoutLegCapsuleLibrariesEnabledAtt(string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    //string storedProcedureName = "create_dynamic_pivot_withoutleg_library ";
                    //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    //{
                    //    procedureCommand.CommandType = CommandType.StoredProcedure;
                    //    procedureCommand.ExecuteNonQuery();
                    //}
                    var attActivated = db.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "CivilWithoutLegsLibraryCapsule" && ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
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
                        var query = db.MV_CIVIL_WITHOUTLEG_LIBRARY_VIEW.Where(x => !x.Deleted
                       && x.CIVILWITHOUTLEGCATEGORY.ToLower() == "capsule").AsEnumerable()
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();
                        
                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = db.MV_CIVIL_WITHOUTLEG_LIBRARY_VIEW.Where(x => !x.Deleted && x.CIVILWITHOUTLEGCATEGORY.ToLower() == "capsule").AsEnumerable()
                    .GroupBy(x => new
                    {
                        Id = x.Id,
                        Model = x.Model,
                        Note = x.Note,
                        Prefix = x.Prefix,
                        Height_Designed = x.Height_Designed,
                        Max_Load = x.Max_Load,
                        SpaceLibrary = x.SpaceLibrary,
                        Active = x.Active,
                        Deleted = x.Deleted,
                        CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                        INSTCIVILWITHOUTLEGSTYPE = x.INSTCIVILWITHOUTLEGSTYPE,
                        CIVILSTEELSUPPORTCATEGORY = x.CIVILSTEELSUPPORTCATEGORY,
                        STRUCTURETYPE = x.STRUCTURETYPE,
                        Manufactured_Max_Load = x.Manufactured_Max_Load,
                        HeightBase = x.HeightBase,
                        WidthVariation = x.WidthVariation

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
        //public Response<ReturnWithFilters<object>> GetCivilNonSteelLibrariesEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination)
        //{
        //    try
        //    {
        //        List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
        //        List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
        //        int Count = 0;
        //        List<object> OutPutList = new List<object>();
        //        ReturnWithFilters<object> CivilTableDisplay = new ReturnWithFilters<object>();

        //        List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

        //        List<CivilNonSteelLibraryViewModel> CivilNonSteelLibraries = new List<CivilNonSteelLibraryViewModel>();
        //        List<CivilNonSteelLibraryViewModel> WithoutDateFilterCivilNonSteelLibraries = new List<CivilNonSteelLibraryViewModel>();
        //        List<CivilNonSteelLibraryViewModel> WithDateFilterCivilNonSteelLibraries = new List<CivilNonSteelLibraryViewModel>();

        //        List<TLIattributeActivated> CivilNonSteelLibraryAttribute = new List<TLIattributeActivated>();
        //        if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
        //            (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
        //        {
        //            CivilNonSteelLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
        //                x.Enable && x.AttributeActivatedId != null &&
        //                x.AttributeActivated.DataType.ToLower() != "datetime" &&
        //                x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CivilNonSteelLibrary.ToString() &&
        //                x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString(),
        //                    x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
        //            .Select(x => x.AttributeActivated).ToList();
        //        }

        //        if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
        //        {
        //            List<TLIattributeActivated> NotDateDateCivilNonSteelLibraryAttribute = CivilNonSteelLibraryAttribute.Where(x =>
        //                x.DataType.ToLower() != "datetime").ToList();

        //            foreach (FilterObjectList item in ObjectAttributeFilters)
        //            {
        //                List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

        //                TLIattributeActivated AttributeKey = NotDateDateCivilNonSteelLibraryAttribute.FirstOrDefault(x =>
        //                    x.Label.ToLower() == item.key.ToLower());

        //                string Key = "";

        //                if (AttributeKey != null)
        //                    Key = AttributeKey.Key;

        //                else
        //                    Key = item.key;

        //                AttributeFilters.Add(new StringFilterObjectList
        //                {
        //                    key = Key,
        //                    value = value
        //                });
        //            }
        //        }
        //        if (AttributeFilters != null && AttributeFilters.Count > 0)
        //        {
        //            //
        //            // Library Dynamic Attributes...
        //            //
        //            List<TLIdynamicAtt> LibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
        //                AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
        //                x.LibraryAtt && !x.disable &&
        //                x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

        //            List<int> DynamicLibValueListIds = new List<int>();
        //            bool DynamicLibExist = false;

        //            if (LibDynamicAttListIds.Count > 0)
        //            {
        //                DynamicLibExist = true;
        //                GetInventoriesIdsFromDynamicAttributes(out DynamicLibValueListIds, LibDynamicAttListIds, AttributeFilters);
        //            }

        //            //
        //            // Library Attribute Activated...
        //            //
        //            bool AttrLibExist = typeof(CivilNonSteelLibraryViewModel).GetProperties().ToList().Exists(x =>
        //                AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Where(y => y.ToLower() != "Id".ToLower())
        //                .Contains(x.Name.ToLower()));

        //            List<int> LibraryAttributeActivatedIds = new List<int>();

        //            if (AttrLibExist)
        //            {
        //                List<PropertyInfo> NonStringLibraryProps = typeof(CivilNonSteelLibraryViewModel).GetProperties().Where(x =>
        //                    x.PropertyType.Name.ToLower() != "string" &&
        //                    AttributeFilters.AsEnumerable().Select(y =>
        //                        y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

        //                List<PropertyInfo> StringLibraryProps = typeof(CivilNonSteelLibraryViewModel).GetProperties().Where(x =>
        //                    x.PropertyType.Name.ToLower() == "string" &&
        //                    AttributeFilters.AsEnumerable().Select(y =>
        //                        y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

        //                List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
        //                    NonStringLibraryProps.AsEnumerable().Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
        //                    StringLibraryProps.AsEnumerable().Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

        //                //LibraryAttributeActivatedIds = _unitOfWork.CivilNonSteelLibraryRepository.GetWhere(x =>
        //                //     LibraryPropsAttributeFilters.All(z =>
        //                //        NonStringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null).ToString().ToLower()) : false)) ||
        //                //        StringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
        //                //             y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
        //                // ).Select(i => i.Id).ToList();

        //                IEnumerable<TLIcivilNonSteelLibrary> Libraries = _unitOfWork.CivilNonSteelLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

        //                foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
        //                {
        //                    if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
        //                    {
        //                        Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
        //                             y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
        //                    }
        //                    else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
        //                    {
        //                        Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null ?
        //                            LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
        //                    }
        //                }

        //                LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
        //            }

        //            //
        //            // Library (Attribute Activated + Dynamic) Attributes...
        //            //
        //            List<int> IntersectLibraryIds = new List<int>();
        //            if (AttrLibExist && DynamicLibExist)
        //            {
        //                IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
        //            }
        //            else if (AttrLibExist)
        //            {
        //                IntersectLibraryIds = LibraryAttributeActivatedIds;
        //            }
        //            else if (DynamicLibExist)
        //            {
        //                IntersectLibraryIds = DynamicLibValueListIds;
        //            }

        //            WithoutDateFilterCivilNonSteelLibraries = _mapper.Map<List<CivilNonSteelLibraryViewModel>>(_unitOfWork.CivilNonSteelLibraryRepository.GetIncludeWhere(x =>
        //                x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.civilNonSteelType).ToList());
        //        }

        //        //
        //        // DateTime Objects Filters..
        //        //
        //        List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
        //        if (DateFilter != null ? DateFilter.Count() > 0 : false)
        //        {
        //            List<TLIattributeActivated> DateCivilNonSteelLibraryAttribute = CivilNonSteelLibraryAttribute.Where(x =>
        //                x.DataType.ToLower() == "datetime").ToList();

        //            foreach (DateFilterViewModel item in DateFilter)
        //            {
        //                DateTime DateFrom = Convert.ToDateTime(item.DateFrom);
        //                DateTime DateTo = Convert.ToDateTime(item.DateTo);

        //                if (DateFrom > DateTo)
        //                {
        //                    DateTime Replacer = DateFrom;
        //                    DateFrom = DateTo;
        //                    DateTo = Replacer;
        //                }

        //                TLIattributeActivated AttributeKey = DateCivilNonSteelLibraryAttribute.FirstOrDefault(x =>
        //                    x.Label.ToLower() == item.key.ToLower());
        //                string Key = "";

        //                if (AttributeKey != null)
        //                    Key = AttributeKey.Key;
        //                else
        //                    Key = item.key;

        //                AfterConvertDateFilters.Add(new DateFilterViewModel
        //                {
        //                    key = Key,
        //                    DateFrom = DateFrom,
        //                    DateTo = DateTo
        //                });
        //            }
        //        }
        //        if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
        //        {
        //            //
        //            // Library Dynamic Attributes...
        //            //
        //            List<TLIdynamicAtt> DateTimeLibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
        //                AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
        //                x.LibraryAtt && !x.disable &&
        //                x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString(), x => x.tablesNames).ToList();

        //            List<int> DynamicLibValueListIds = new List<int>();
        //            bool DynamicLibExist = false;

        //            if (DateTimeLibDynamicAttListIds.Count > 0)
        //            {
        //                DynamicLibExist = true;
        //                List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
        //                    DateTimeLibDynamicAttListIds.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

        //                DynamicLibValueListIds = new List<int>();

        //                List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
        //                    DateTimeLibDynamicAttListIds.AsEnumerable().Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

        //                List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

        //                foreach (int InventoryId in InventoriesIds)
        //                {
        //                    List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x =>
        //                        x.InventoryId == InventoryId).ToList();

        //                    if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Select(x => x.ValueDateTime).Any(x =>
        //                         (x != null ?
        //                            (x >= y.DateFrom && x <= y.DateTo) : (false)))))
        //                    {
        //                        DynamicLibValueListIds.Add(InventoryId);
        //                    }
        //                }
        //            }

        //            //
        //            // Library Attribute Activated...
        //            //
        //            List<PropertyInfo> LibraryProps = typeof(CivilNonSteelLibraryViewModel).GetProperties().Where(x =>
        //                AfterConvertDateFilters.Select(y =>
        //                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

        //            List<int> LibraryAttributeActivatedIds = new List<int>();
        //            bool AttrLibExist = false;

        //            if (LibraryProps != null)
        //            {
        //                AttrLibExist = true;

        //                List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
        //                    LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

        //                //LibraryAttributeActivatedIds = _unitOfWork.CivilNonSteelLibraryRepository.GetIncludeWhere(x =>
        //                //    LibraryPropsAttributeFilters.All(z =>
        //                //        (LibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null) ?
        //                //            ((z.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null))) &&
        //                //             (z.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null)))) : (false)))))
        //                //).Select(i => i.Id).ToList();

        //                IEnumerable<TLIcivilNonSteelLibrary> Libraries = _unitOfWork.CivilNonSteelLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

        //                foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
        //                {
        //                    Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null) ?
        //                        ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null))) &&
        //                            (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null)))) : (false))));
        //                }

        //                LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
        //            }

        //            //
        //            // Library (Attribute Activated + Dynamic) Attributes...
        //            //
        //            List<int> IntersectLibraryIds = new List<int>();
        //            if (AttrLibExist && DynamicLibExist)
        //            {
        //                IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
        //            }
        //            else if (AttrLibExist)
        //            {
        //                IntersectLibraryIds = LibraryAttributeActivatedIds;
        //            }
        //            else if (DynamicLibExist)
        //            {
        //                IntersectLibraryIds = DynamicLibValueListIds;
        //            }

        //            WithDateFilterCivilNonSteelLibraries = _mapper.Map<List<CivilNonSteelLibraryViewModel>>(_unitOfWork.CivilNonSteelLibraryRepository.GetIncludeWhere(x =>
        //                x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.civilNonSteelType).ToList());
        //        }

        //        //
        //        // Intersect Between WithoutDateFilterCivilNonSteelLibraries + WithDateFilterCivilNonSteelLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
        //        //
        //        if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
        //            (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
        //        {
        //            CivilNonSteelLibraries = _mapper.Map<List<CivilNonSteelLibraryViewModel>>(_unitOfWork.CivilNonSteelLibraryRepository.GetIncludeWhere(x =>
        //                x.Id > 0 && !x.Deleted, x => x.civilNonSteelType).ToList());
        //        }
        //        else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
        //                (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
        //        {
        //            List<int> CivilIds = WithoutDateFilterCivilNonSteelLibraries.Select(x => x.Id).Intersect(WithDateFilterCivilNonSteelLibraries.Select(x => x.Id)).ToList();
        //            CivilNonSteelLibraries = _mapper.Map<List<CivilNonSteelLibraryViewModel>>(_unitOfWork.CivilNonSteelLibraryRepository.GetWhere(x =>
        //                CivilIds.Contains(x.Id)).ToList());
        //        }
        //        else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
        //        {
        //            CivilNonSteelLibraries = WithoutDateFilterCivilNonSteelLibraries;
        //        }
        //        else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
        //        {
        //            CivilNonSteelLibraries = WithDateFilterCivilNonSteelLibraries;
        //        }

        //        Count = CivilNonSteelLibraries.Count();

        //        CivilNonSteelLibraries = CivilNonSteelLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
        //            Take(parameterPagination.PageSize).ToList();

        //        List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
        //           (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CivilNonSteelLibrary.ToString() &&
        //           (x.AttributeActivatedId != null ?
        //                (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString() && x.AttributeActivated.enable) :
        //                (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString()))) ||
        //            (x.AttributeActivated != null ?
        //                ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString()) : false),
        //               x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
        //               x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

        //        List<TLIattributeViewManagment> NotDateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
        //            x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime") : false).ToList();

        //        List<TLIattributeViewManagment> NotDateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
        //            x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() != "datetime" : false).ToList();

        //        List<TLIattributeViewManagment> DateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
        //            x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

        //        List<TLIattributeViewManagment> DateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
        //            x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() == "datetime" : false).ToList();

        //        foreach (CivilNonSteelLibraryViewModel CivilNonSteelLibraryViewModel in CivilNonSteelLibraries)
        //        {
        //            dynamic DynamicCivilNonSteelLibrary = new ExpandoObject();

        //            //
        //            // Library Object ViewModel... (Not DateTime DataType Attribute)
        //            //
        //            if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
        //            {
        //                List<PropertyInfo> LibraryProps = typeof(CivilNonSteelLibraryViewModel).GetProperties().Where(x =>
        //                    x.PropertyType.GenericTypeArguments != null ?
        //                        (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
        //                        (x.PropertyType.Name.ToLower() != "datetime")) :
        //                    (x.PropertyType.Name.ToLower() != "datetime")).ToList();

        //                foreach (PropertyInfo prop in LibraryProps)
        //                {
        //                    if (prop.Name.ToLower().Contains("_name") &&
        //                        NotDateTimeLibraryAttributesViewModel.Select(x =>
        //                            x.AttributeActivated.Label.ToLower()).Contains(prop.Name.ToLower()))
        //                    {
        //                        object ForeignKeyNamePropObject = prop.GetValue(CivilNonSteelLibraryViewModel, null);
        //                        ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
        //                    }
        //                    else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
        //                         x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
        //                        !prop.Name.ToLower().Contains("_name") &&
        //                        (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
        //                    {
        //                        if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
        //                        {
        //                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
        //                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString() &&
        //                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

        //                            if (LabelName != null)
        //                            {
        //                                object PropObject = prop.GetValue(CivilNonSteelLibraryViewModel, null);
        //                                ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            object PropObject = prop.GetValue(CivilNonSteelLibraryViewModel, null);
        //                            ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
        //                        }
        //                    }
        //                }
        //            }

        //            //
        //            // Library Dynamic Attributes... (Not DateTime DataType Attribute)
        //            // 
        //            List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
        //               !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString() &&
        //                x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
        //                NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

        //            foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
        //            {
        //                TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
        //                    x.DynamicAttId == LibraryDynamicAtt.Id &&
        //                    x.InventoryId == CivilNonSteelLibraryViewModel.Id && !x.disable &&
        //                    x.DynamicAtt.LibraryAtt &&
        //                    x.DynamicAtt.Key == LibraryDynamicAtt.Key,
        //                        x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

        //                if (DynamicAttLibValue != null)
        //                {
        //                    dynamic DynamicAttValue = new ExpandoObject();

        //                    if (DynamicAttLibValue.ValueString != null)
        //                        DynamicAttValue = DynamicAttLibValue.ValueString;

        //                    else if (DynamicAttLibValue.ValueDouble != null)
        //                        DynamicAttValue = DynamicAttLibValue.ValueDouble;

        //                    else if (DynamicAttLibValue.ValueDateTime != null)
        //                        DynamicAttValue = DynamicAttLibValue.ValueDateTime;

        //                    else if (DynamicAttLibValue.ValueBoolean != null)
        //                        DynamicAttValue = DynamicAttLibValue.ValueBoolean;

        //                    ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
        //                }
        //                else
        //                {
        //                    ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
        //                }
        //            }

        //            //
        //            // Library Object ViewModel... (DateTime DataType Attribute)
        //            //
        //            dynamic DateTimeAttributes = new ExpandoObject();
        //            if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
        //            {
        //                List<PropertyInfo> DateTimeLibraryProps = typeof(CivilNonSteelLibraryViewModel).GetProperties().Where(x =>
        //                    x.PropertyType.GenericTypeArguments != null ?
        //                        (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
        //                        (x.PropertyType.Name.ToLower() == "datetime")) :
        //                    (x.PropertyType.Name.ToLower() == "datetime")).ToList();

        //                foreach (PropertyInfo prop in DateTimeLibraryProps)
        //                {
        //                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
        //                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString() &&
        //                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

        //                    if (LabelName != null)
        //                    {
        //                        object PropObject = prop.GetValue(CivilNonSteelLibraryViewModel, null);
        //                        ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
        //                    }
        //                }
        //            }

        //            //
        //            // Library Dynamic Attributes... (DateTime DataType Attribute)
        //            // 
        //            List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
        //               !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString() &&
        //                x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
        //                DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

        //            foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
        //            {
        //                TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
        //                    x.DynamicAttId == LibraryDynamicAtt.Id &&
        //                    x.InventoryId == CivilNonSteelLibraryViewModel.Id && !x.disable &&
        //                    x.DynamicAtt.LibraryAtt &&
        //                    x.DynamicAtt.Key == LibraryDynamicAtt.Key,
        //                        x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

        //                if (DynamicAttLibValue != null)
        //                {
        //                    dynamic DynamicAttValue = new ExpandoObject();
        //                    if (DynamicAttLibValue.ValueDateTime != null)
        //                        DynamicAttValue = DynamicAttLibValue.ValueDateTime;

        //                    ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
        //                }
        //                else
        //                {
        //                    ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
        //                }
        //            }

        //            ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

        //            OutPutList.Add(DynamicCivilNonSteelLibrary);
        //        }

        //        CivilTableDisplay.Model = OutPutList;

        //        if (WithFilterData)
        //        {
        //            CivilTableDisplay.filters = _unitOfWork.CivilNonSteelLibraryRepository.GetRelatedTables();
        //        }
        //        else
        //        {
        //            CivilTableDisplay.filters = null;
        //        }

        //        return new Response<ReturnWithFilters<object>>(true, CivilTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
        //    }
        //    catch (Exception err)
        //    {
        //        return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
        //    }
        //}
        #endregion

        #region ManagmentTabkes
        public Response<ReturnWithFilters<CivilWithLegLibraryViewModel>> getCivilWithLegLibrariesManagmentTables(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                IEnumerable<TLIcivilWithLegLibrary> CivilWithLegLibrariesList;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));

                CivilWithLegLibrariesList = _unitOfWork.CivilWithLegLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, c => c.civilSteelSupportCategory, c => c.sectionsLegType, c => c.structureType, c => c.supportTypeDesigned);
                var FilteredCivilWithLegLibrariesModel = _mapper.Map<IEnumerable<CivilWithLegLibraryViewModel>>(CivilWithLegLibrariesList);
                ReturnWithFilters<CivilWithLegLibraryViewModel> CivilWithleg = new ReturnWithFilters<CivilWithLegLibraryViewModel>();
                CivilWithleg.Model = FilteredCivilWithLegLibrariesModel.ToList();
                if (WithFilterData.Equals(true))
                {
                    CivilWithleg.filters = _unitOfWork.CivilWithLegLibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<CivilWithLegLibraryViewModel>>(true, CivilWithleg, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<CivilWithLegLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        #endregion
        #region History (No Need For This Functions AnyMore)
        //#region Add History
        //public void AddHistory(int Civil_lib_id, string historyType, string TableName)
        //{
        //    AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
        //    history.RecordId = Civil_lib_id;
        //    history.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableName).Id;
        //    history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
        //    history.UserId = 261;
        //    history.Date = DateTime.Now;
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
        //    history.UserId = 261;
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

        //public EditHistoryDetails CheckUpdateObject(object originalObj, object updateObj)
        //{
        //    EditHistoryDetails result = new EditHistoryDetails();
        //    result.original = originalObj;
        //    result.Details = new List<TLIhistoryDetails>();
        //    foreach (var property in updateObj.GetType().GetProperties())
        //    {

        //        var x = property.GetValue(updateObj);
        //        var y = property.GetValue(originalObj);

        //        if (x != null || y != null)
        //        {
        //            if (x != null)
        //            {
        //                if (!x.Equals(y))

        //                {
        //                    property.SetValue(result.original, x);
        //                    TLIhistoryDetails historyDetails = new TLIhistoryDetails();
        //                    // historyDetails.AttType = "static";
        //                    historyDetails.AttName = property.Name;
        //                    if (y != null)
        //                    {
        //                        historyDetails.OldValue = y.ToString();
        //                    }
        //                    if (x != null)
        //                    {
        //                        historyDetails.NewValue = x.ToString();
        //                    }

        //                    result.Details.Add(historyDetails);
        //                    // _unitOfWork.HistoryDetailsRepository.Add(historyDetails);
        //                    // _unitOfWork.SaveChanges();
        //                    //property.SetValue(originalObj, updateObj.GetType().GetProperty(property.Name)
        //                    //.GetValue(originalObj, null));
        //                }


        //            }
        //            else
        //            {
        //                property.SetValue(result.original, x);
        //                TLIhistoryDetails historyDetails = new TLIhistoryDetails();
        //                // historyDetails.AttType = "static";
        //                historyDetails.AttName = property.Name;
        //                if (y != null)
        //                {
        //                    historyDetails.OldValue = y.ToString();
        //                }
        //                if (x != null)
        //                {
        //                    historyDetails.NewValue = x.ToString();
        //                }
        //                result.Details.Add(historyDetails);
        //            }

        //        }
        //    }
        //    return result;
        //}
        #endregion

    }
}
