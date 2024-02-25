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
using TLIS_DAL.ViewModels.InstallationCivilwithoutLegsTypeDTOs;
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
        public Response<AllItemAttributes> AddCivilLibrary(string TableName, object CivilLibraryViewModel, string connectionString)
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
                            //Get TableName Id to Get DynamicAtt
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);
                            if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == TableName)
                            {
                                //Map object to ViewModel becaue i can't deal with ViewModel directly
                                AddCivilWithLegLibraryViewModel civilWithLegLibraryViewModel = _mapper.Map<AddCivilWithLegLibraryViewModel>(CivilLibraryViewModel);
                                //Map ViewModel to Entity
                                TLIcivilWithLegLibrary CivilWithLegEntites = _mapper.Map<TLIcivilWithLegLibrary>(CivilLibraryViewModel);
                                bool test = true;
                                string CheckGeneralValidation = CheckGeneralValidationFunction(civilWithLegLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                string CheckDependencyValidation = CheckDependencyValidationForCivilTypes(CivilLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                if (test == true)
                                {
                                    //Check If Model is already exists in database return true or false
                                    var CheckModel = _unitOfWork.CivilWithLegLibraryRepository.GetWhereFirst(x => x.Model == CivilWithLegEntites.Model && !x.Deleted);
                                    //if CheckModel return true return error message that the model is already exists
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {CivilWithLegEntites.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                    //Check Height_Designed(Shouldn't be equal to zero)
                                    /* if (civilWithLegLibraryViewModel.Height_Designed <= 0)
                                     {
                                         return new Response<AllItemAttributes>(true, null, null, "HeightDesigned Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                     }
                                     //Check SpaceLibrary(Shouldn't be equal to zero)
                                     if (civilWithLegLibraryViewModel.SpaceLibrary <= 0)
                                     {
                                         return new Response<AllItemAttributes>(true, null, null, "SpaceLibrary Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                     }*/

                                    _unitOfWork.CivilWithLegLibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, CivilWithLegEntites);
                                    //SaveChangesAsync return int number if number bigger than zero then task is completed
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = CivilLibraryViewModel;

                                    AddLogisticalItemWithCivil(LogisticalItemIds, CivilWithLegEntites, TableNameEntity.Id);

                                    //Check if there are DynamicAtt values
                                    if (civilWithLegLibraryViewModel.TLIdynamicAttLibValue != null ? civilWithLegLibraryViewModel.TLIdynamicAttLibValue.Count > 0 : false)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(civilWithLegLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.Id, CivilWithLegEntites.Id);
                                        // _unitOfWork.LegRepository.AddLegs(civilWithLegLibraryViewModel.LegsViewModel, CivilWithLegEntites.Id);
                                    }
                                    //AddHistory(CivilWithLegEntites.Id, "Add", "TLIcivilWithLegLibrary");
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == TableName)
                            {
                                //Map object to ViewModel becaue i can't deal with ViewModel directly
                                AddCivilWithoutLegLibraryViewModel withoutLegLibraryViewModel = _mapper.Map<AddCivilWithoutLegLibraryViewModel>(CivilLibraryViewModel);
                                //Map from ViewModel to Entity
                                TLIcivilWithoutLegLibrary CivilWithoutLegEntites = _mapper.Map<TLIcivilWithoutLegLibrary>(CivilLibraryViewModel);

                                bool test = true;
                                string CheckDependencyValidation = CheckDependencyValidationForCivilTypes(CivilLibraryViewModel, TableName, withoutLegLibraryViewModel.CivilWithoutLegCategoryId);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(withoutLegLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.TableName, withoutLegLibraryViewModel.CivilWithoutLegCategoryId);
                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                if (test == true)
                                {
                                    //Check If Model is already exists in database return true or false
                                    var CheckModel = _unitOfWork.CivilWithoutLegLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == CivilWithoutLegEntites.Model.ToLower() &&
                                        !x.Deleted && x.Id != CivilWithoutLegEntites.Id && x.CivilWithoutLegCategoryId == CivilWithoutLegEntites.CivilWithoutLegCategoryId);
                                    //if CheckModel return true return error message that the model is already exists
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {CivilWithoutLegEntites.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                    //Check Height_Designed(Shouldn't be equal to zero)
                                    //if (CivilWithoutLegEntites.Height_Designed <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "HeightDesigned Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    ////Check SpaceLibrary(Shouldn't be equal to zero)
                                    //if (CivilWithoutLegEntites.SpaceLibrary <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "SpaceLibrary Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    _unitOfWork.CivilWithoutLegLibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, CivilWithoutLegEntites);
                                    //SaveChangesAsync return int number if number bigger than zero then task is completed
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = CivilLibraryViewModel;

                                    AddLogisticalItemWithCivil(LogisticalItemIds, CivilWithoutLegEntites, TableNameEntity.Id);

                                    //Check if there are DynamicAtt values
                                    if (withoutLegLibraryViewModel.TLIdynamicAttLibValue.Count > 0)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(withoutLegLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.Id, CivilWithoutLegEntites.Id);
                                    }
                                    //AddHistory(CivilWithoutLegEntites.Id, "Add", "TLIcivilWithoutLegLibrary");
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == TableName)
                            {
                                //Map object to ViewModel becaue i can't deal with ViewModel directly
                                AddCivilNonSteelLibraryViewModel nonSteelLibraryViewModel = _mapper.Map<AddCivilNonSteelLibraryViewModel>(CivilLibraryViewModel);
                                //Map from ViewModel to Entity
                                TLIcivilNonSteelLibrary civilNonSteelLibraryEntity = _mapper.Map<TLIcivilNonSteelLibrary>(CivilLibraryViewModel);
                                bool test = true;
                                string CheckDependencyValidation = CheckDependencyValidationForCivilTypes(CivilLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(nonSteelLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.TableName);
                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                                if (test == true)
                                {
                                    //Check If Model is already exists in database return true or false
                                    var CheckModel = _unitOfWork.CivilNonSteelLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == civilNonSteelLibraryEntity.Model.ToLower() &&
                                        !x.Deleted && x.Id != civilNonSteelLibraryEntity.Id);
                                    //if CheckModel return true return error message that the model is already exists
                                    if (CheckModel != null)
                                    {
                                        return new Response<AllItemAttributes>(true, null, null, $"This model {civilNonSteelLibraryEntity.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                    //Check Hight(Shouldn't be equal to zero)
                                    //if (civilNonSteelLibraryEntity.Hight <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "Height Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    ////Check SpaceLibrary(Shouldn't be equal to zero)
                                    //if (civilNonSteelLibraryEntity.SpaceLibrary <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "SpaceLibrary Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    ////Check NumberofBoltHoles(Shouldn't be equal to zero)
                                    //if (civilNonSteelLibraryEntity.NumberofBoltHoles <= 0)
                                    //{
                                    //    return new Response<AllItemAttributes>(true, null, null, "NumberofBoltHoles Should be bigger than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                                    //}
                                    _unitOfWork.CivilNonSteelLibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, civilNonSteelLibraryEntity);
                                    //SaveChangesAsync return int number if number bigger than zero then task is completed
                                    _unitOfWork.SaveChanges();

                                    dynamic LogisticalItemIds = new ExpandoObject();
                                    LogisticalItemIds = CivilLibraryViewModel;

                                    AddLogisticalItemWithCivil(LogisticalItemIds, civilNonSteelLibraryEntity, TableNameEntity.Id);

                                    //Check if there are DynamicAtt values
                                    if (nonSteelLibraryViewModel.TLIdynamicAttLibValue.Count > 0)
                                    {
                                        _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtts(nonSteelLibraryViewModel.TLIdynamicAttLibValue, TableNameEntity.Id, civilNonSteelLibraryEntity.Id);
                                    }
                                    //      AddHistory(civilNonSteelLibraryEntity.Id, "Add", "TLIcivilNonSteelLibrary");
                                }
                                else
                                {
                                    return new Response<AllItemAttributes>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            transaction.Complete();
                            tran.Commit();
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
        public string CheckDependencyValidationForCivilTypes(object Input, string CivilType, int? catid = null)
        {
            List<DynamicAttViewModel> DynamicAttributes = null;
            if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString().ToLower())
            {
                AddCivilWithLegLibraryViewModel AddCivilLibraryViewModel = _mapper.Map<AddCivilWithLegLibraryViewModel>(Input);

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
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddCivilLibraryViewModel.TLIdynamicAttLibValue
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

                                    InsertedValue = AddCivilLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddCivilLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddCivilLibraryViewModel.TLIdynamicAttLibValue
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
            else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString().ToLower())
            {
                AddCivilWithoutLegLibraryViewModel AddCivilLibraryViewModel = _mapper.Map<AddCivilWithoutLegLibraryViewModel>(Input);
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
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddCivilLibraryViewModel.TLIdynamicAttLibValue
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

                                    InsertedValue = AddCivilLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddCivilLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddCivilLibraryViewModel.TLIdynamicAttLibValue
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
            else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString().ToLower())
            {
                AddCivilNonSteelLibraryViewModel AddCivilLibraryViewModel = _mapper.Map<AddCivilNonSteelLibraryViewModel>(Input);

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
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddCivilLibraryViewModel.TLIdynamicAttLibValue
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

                                    InsertedValue = AddCivilLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddCivilLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddCivilLibraryViewModel.TLIdynamicAttLibValue
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
        public void AddLogisticalItemWithCivil(dynamic LogisticalItemIds, dynamic CivilEntity, int TableNameEntityId)
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
                                RecordId = CivilEntity.Id,
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
                                RecordId = CivilEntity.Id,
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
                                RecordId = CivilEntity.Id,
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
                                RecordId = CivilEntity.Id,
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

        //Function take 2 parameters that disable or enable record depened on record status ex: if record is active then disable else enable
        //First Id to specify the record i deal with
        //Second TableName to specify the table i deal with
        public async Task<Response<AllItemAttributes>> Disable(int Id, string TableName)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == TableName)
                    {
                        TLIcivilWithLegLibrary OldCivilWithLeg = _unitOfWork.CivilWithLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);

                        TLIcivilWithLegLibrary NewCivilWithLeg = _unitOfWork.CivilWithLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewCivilWithLeg.Active = !(NewCivilWithLeg.Active);

                        _unitOfWork.CivilWithLegLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldCivilWithLeg, NewCivilWithLeg);
                        //await _unitOfWork.SaveChangesAsync();
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == TableName)
                    {
                        TLIcivilWithoutLegLibrary OldCivilWithoutLeg = _unitOfWork.CivilWithoutLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);

                        TLIcivilWithoutLegLibrary NewCivilWithoutLeg = _unitOfWork.CivilWithoutLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewCivilWithoutLeg.Active = !(NewCivilWithoutLeg.Active);

                        _unitOfWork.CivilWithoutLegLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldCivilWithoutLeg, NewCivilWithoutLeg);
                        //await _unitOfWork.SaveChangesAsync();
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == TableName)
                    {

                        TLIcivilNonSteelLibrary OldCivilNonSteel = _unitOfWork.CivilNonSteelLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);

                        TLIcivilNonSteelLibrary NewCivilNonSteel = _unitOfWork.CivilNonSteelLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewCivilNonSteel.Active = !(NewCivilNonSteel.Active);
                        _unitOfWork.CivilNonSteelLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldCivilNonSteel, NewCivilNonSteel);
                        //await _unitOfWork.SaveChangesAsync();
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
        //Function take 2 parameters 
        //First CivilLibraryViewModel object contain data to update
        //Second TableName to specify the table i deal with
        public async Task<Response<AllItemAttributes>> EditCivilLibrary(object CivilLibraryViewModel, string TableName)
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
                    //Get TableName Id from TLItablesNames by TableName
                    TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);

                    if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString().ToLower() == TableName.ToLower())
                    {
                        //Map object to ViewModel
                        EditCivilWithLegLibraryViewModels editCivilWithLeg = _mapper.Map<EditCivilWithLegLibraryViewModels>(CivilLibraryViewModel);

                        //Map ViewModel to Entity
                        TLIcivilWithLegLibrary CivilWithLegLibraryEntites = _mapper.Map<TLIcivilWithLegLibrary>(CivilLibraryViewModel);

                        TLIcivilWithLegLibrary CivilWithLegLib = _unitOfWork.CivilWithLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editCivilWithLeg.Id);
                        //Check if there is another record have the same model return true and false
                        var CheckModel = _unitOfWork.CivilWithLegLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == CivilWithLegLibraryEntites.Model.ToLower() &&
                            x.Id != CivilWithLegLibraryEntites.Id && !x.Deleted);
                        //if CheckModel is true return error message that the model is already exists
                        if (CheckModel != null)
                        {
                            return new Response<AllItemAttributes>(true, null, null, $"This model {CivilWithLegLibraryEntites.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        CivilWithLegLibraryEntites.Active = CivilWithLegLib.Active;
                        CivilWithLegLibraryEntites.Deleted = CivilWithLegLib.Deleted;

                        _unitOfWork.CivilWithLegLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, CivilWithLegLib, CivilWithLegLibraryEntites);

                        //var testUpdate = CheckUpdateObject(CivilWithLegLib, CivilWithLegLibraryEntites);
                        //if (testUpdate.Details.Count != 0)
                        //{
                        //await _unitOfWork.SaveChangesAsync();

                        //resultId = AddHistoryForEdit(CivilWithLegLibraryEntites.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
                        //}
                        string CheckDependencyValidation = CheckDependencyValidationForCivilTypesEditApiVersion(CivilLibraryViewModel, TableName);
                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersion(editCivilWithLeg.DynamicAtts, TableNameEntity.TableName);
                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        dynamic LogisticalItemIds = new ExpandoObject();
                        LogisticalItemIds = CivilLibraryViewModel;

                        AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                        var CheckVendorId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithLegLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckVendorId != null)
                            OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

                        else
                            OldLogisticalItemIds.VendorId = 0;

                        var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithLegLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckSupplierId != null)
                            OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

                        else
                            OldLogisticalItemIds.SupplierId = 0;

                        var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithLegLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckDesignerId != null)
                            OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

                        else
                            OldLogisticalItemIds.DesignerId = 0;

                        var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithLegLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckManufacturerId != null)
                            OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

                        else
                            OldLogisticalItemIds.ManufacturerId = 0;

                        EditLogisticalItem(LogisticalItemIds, CivilWithLegLibraryEntites, TableNameEntity.Id, OldLogisticalItemIds);

                        if (editCivilWithLeg.DynamicAtts != null ? editCivilWithLeg.DynamicAtts.Count > 0 : false)
                        {
                            _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editCivilWithLeg.DynamicAtts, TableNameEntity.Id, CivilWithLegLibraryEntites.Id, Helpers.LogFilterAttribute.UserId, resultId, CivilWithLegLib.Id);
                        }
                        civilLibId = CivilWithLegLibraryEntites.Id;
                        tablesNameId = TableNameEntity.Id;

                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString().ToLower() == TableName.ToLower())
                    {
                        //Map object to ViewModel
                        EditCivilWithoutLegLibraryViewModel editCivilWithoutLeg = _mapper.Map<EditCivilWithoutLegLibraryViewModel>(CivilLibraryViewModel);
                        //Map ViewModel to Entity
                        TLIcivilWithoutLegLibrary CivilWithoutLegLibraryEntites = _mapper.Map<TLIcivilWithoutLegLibrary>(CivilLibraryViewModel);
                        TLIcivilWithoutLegLibrary civilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editCivilWithoutLeg.Id);
                        //Check if there is another record have the same model return true and false

                        TLIcivilWithoutLegLibrary CheckModel = _unitOfWork.CivilWithoutLegLibraryRepository
                            .GetWhereFirst(x => x.Model == CivilWithoutLegLibraryEntites.Model && x.Id != CivilWithoutLegLibraryEntites.Id &&
                                !x.Deleted && x.CivilWithoutLegCategoryId == editCivilWithoutLeg.CivilWithoutLegCategoryId);

                        //if CheckModel is true return error message that the model is already exists
                        if (CheckModel != null)
                        {
                            return new Response<AllItemAttributes>(true, null, null, $"This model {CivilWithoutLegLibraryEntites.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        CivilWithoutLegLibraryEntites.Active = civilWithoutLegLibrary.Active;
                        CivilWithoutLegLibraryEntites.Deleted = civilWithoutLegLibrary.Deleted;
                        _unitOfWork.CivilWithoutLegLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, civilWithoutLegLibrary, CivilWithoutLegLibraryEntites);

                        //var testUpdate = CheckUpdateObject(civilWithoutLegLibrary, CivilWithoutLegLibraryEntites);
                        //if (testUpdate.Details.Count != 0)
                        //{

                        //    resultId = AddHistoryForEdit(CivilWithoutLegLibraryEntites.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
                        //}
                        string CheckDependencyValidation = CheckDependencyValidationForCivilTypesEditApiVersion(CivilLibraryViewModel, TableName, editCivilWithoutLeg.CivilWithoutLegCategoryId);
                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersion(editCivilWithoutLeg.DynamicAtts, TableNameEntity.TableName, editCivilWithoutLeg.CivilWithoutLegCategoryId);
                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        dynamic LogisticalItemIds = new ExpandoObject();
                        LogisticalItemIds = CivilLibraryViewModel;

                        AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                        var CheckVendorId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithoutLegLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckVendorId != null)
                            OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

                        else
                            OldLogisticalItemIds.VendorId = 0;

                        var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithoutLegLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckSupplierId != null)
                            OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

                        else
                            OldLogisticalItemIds.SupplierId = 0;

                        var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithoutLegLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckDesignerId != null)
                            OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

                        else
                            OldLogisticalItemIds.DesignerId = 0;

                        var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == CivilWithoutLegLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckManufacturerId != null)
                            OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

                        else
                            OldLogisticalItemIds.ManufacturerId = 0;

                        EditLogisticalItem(LogisticalItemIds, CivilWithoutLegLibraryEntites, TableNameEntity.Id, OldLogisticalItemIds);

                        if (editCivilWithoutLeg.DynamicAtts != null ? editCivilWithoutLeg.DynamicAtts.Count > 0 : false)
                        {
                            _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editCivilWithoutLeg.DynamicAtts, TableNameEntity.Id, CivilWithoutLegLibraryEntites.Id, Helpers.LogFilterAttribute.UserId, resultId, civilWithoutLegLibrary.Id);
                        }

                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString().ToLower() == TableName.ToLower())
                    {
                        //Map object to ViewModel
                        EditCivilNonSteelLibraryViewModel editCivilNonSteel = _mapper.Map<EditCivilNonSteelLibraryViewModel>(CivilLibraryViewModel);
                        //Map ViewModel to Entity
                        TLIcivilNonSteelLibrary civilNonSteelLibraryEntites = _mapper.Map<TLIcivilNonSteelLibrary>(CivilLibraryViewModel);
                        TLIcivilNonSteelLibrary civilNonSteelLibrary = _unitOfWork.CivilNonSteelLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editCivilNonSteel.Id);
                        //Check if there is another record have the same model return true and false
                        var CheckModel = _unitOfWork.CivilNonSteelLibraryRepository.GetWhereFirst(x => x.Model == civilNonSteelLibraryEntites.Model && x.Id != civilNonSteelLibraryEntites.Id && !x.Deleted);
                        //if CheckModel is true return error message that the model is already exists
                        if (CheckModel != null)
                        {
                            return new Response<AllItemAttributes>(true, null, null, $"This model {civilNonSteelLibraryEntites.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        civilNonSteelLibraryEntites.Active = civilNonSteelLibrary.Active;
                        civilNonSteelLibraryEntites.Deleted = civilNonSteelLibrary.Deleted;
                        _unitOfWork.CivilNonSteelLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, civilNonSteelLibrary, civilNonSteelLibraryEntites);

                        //var testUpdate = CheckUpdateObject(civilNonSteelLibrary, civilNonSteelLibraryEntites);
                        //if (testUpdate.Details.Count != 0)
                        //{
                        //    resultId = AddHistoryForEdit(civilNonSteelLibraryEntites.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
                        //}

                        string CheckDependencyValidation = CheckDependencyValidationForCivilTypesEditApiVersion(CivilLibraryViewModel, TableName);
                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersion(editCivilNonSteel.DynamicAtts, TableNameEntity.TableName);
                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        {
                            return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                        dynamic LogisticalItemIds = new ExpandoObject();
                        LogisticalItemIds = CivilLibraryViewModel;

                        AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                        var CheckVendorId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == civilNonSteelLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckVendorId != null)
                            OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

                        else
                            OldLogisticalItemIds.VendorId = 0;

                        var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == civilNonSteelLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckSupplierId != null)
                            OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

                        else
                            OldLogisticalItemIds.SupplierId = 0;

                        var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == civilNonSteelLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckDesignerId != null)
                            OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

                        else
                            OldLogisticalItemIds.DesignerId = 0;

                        var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == civilNonSteelLibraryEntites.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                        if (CheckManufacturerId != null)
                            OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

                        else
                            OldLogisticalItemIds.ManufacturerId = 0;

                        EditLogisticalItem(LogisticalItemIds, civilNonSteelLibraryEntites, TableNameEntity.Id, OldLogisticalItemIds);

                        if (editCivilNonSteel.DynamicAtts != null ? editCivilNonSteel.DynamicAtts.Count > 0 : false)
                        {
                            _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editCivilNonSteel.DynamicAtts, TableNameEntity.Id, civilNonSteelLibraryEntites.Id, Helpers.LogFilterAttribute.UserId, resultId, civilNonSteelLibrary.Id);
                        }
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
        #region Helper Methods..
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
        #endregion
        //Function take 2 parameters 
        //First Id to get record by Id
        //Second TableName to specify table i deal with
        //Function return list of activated attributes with value, list  dynamic attributes
        public Response<AllItemAttributes> GetById(int Id, string TableName)
        {
            try
            {
                AllItemAttributes attributes = new AllItemAttributes();

                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c =>
                    c.TableName == TableName);

                List<BaseAttView> ListAttributesActivated = new List<BaseAttView>();
                int CatId = 0;

                if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == TableName)
                {
                    TLIcivilWithLegLibrary CivilWithLegLibrary = _unitOfWork.CivilWithLegLibraryRepository.GetIncludeWhereFirst(x =>
                        x.Id == Id, x => x.sectionsLegType, x => x.supportTypeDesigned, x => x.structureType, x => x.civilSteelSupportCategory);

                    object FK_SupportTypeDesigned_Name = CivilWithLegLibrary.supportTypeDesigned != null ? CivilWithLegLibrary.supportTypeDesigned.Name : null;

                    object FK_SectionsLegType_Name = CivilWithLegLibrary.sectionsLegType != null ? CivilWithLegLibrary.sectionsLegType.Name : null;

                    object FK_StructureType_Name = CivilWithLegLibrary.structureType != null ? CivilWithLegLibrary.structureType.Name : null;

                    object FK_CivilSteelSupportCategory_Name = CivilWithLegLibrary.civilSteelSupportCategory != null ? CivilWithLegLibrary.civilSteelSupportCategory.Name : null;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, CivilWithLegLibrary, null).ToList();

                    foreach (BaseAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Label.ToLower() == "supporttypedesigned_name")
                        {
                            if (FK_SupportTypeDesigned_Name == null)
                                FKitem.Value = _unitOfWork.SupportTypeDesignedRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_SupportTypeDesigned_Name;
                        }
                        else if (FKitem.Label.ToLower() == "sectionslegtype_name")
                        {
                            if (FK_SectionsLegType_Name == null)
                                FKitem.Value = _unitOfWork.SectionsLegTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_SectionsLegType_Name;
                        }
                        else if (FKitem.Label.ToLower() == "structuretype_name")
                        {
                            if (FK_StructureType_Name == null)
                                FKitem.Value = _unitOfWork.StructureTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_StructureType_Name;
                        }

                        else if (FKitem.Label.ToLower() == "civilsteelsupportcategory_name")
                        {
                            if (FK_CivilSteelSupportCategory_Name == null)
                                FKitem.Value = _unitOfWork.CivilSteelSupportCategoryRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_CivilSteelSupportCategory_Name;
                        }
                    }
                }
                else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == TableName)
                {
                    TLIcivilWithoutLegLibrary CivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository.GetIncludeWhereFirst(x =>
                        x.Id == Id, x => x.CivilSteelSupportCategory, x => x.CivilWithoutLegCategory, x => x.InstallationCivilwithoutLegsType, x => x.structureType);
                    CatId = (int)CivilWithoutLegLibrary.CivilWithoutLegCategoryId;

                    object FK_CivilSteelSupportCategory_Name = CivilWithoutLegLibrary.CivilSteelSupportCategory != null ? CivilWithoutLegLibrary.CivilSteelSupportCategory.Name : null;

                    object FK_CivilWithoutLegCategory_Name = CivilWithoutLegLibrary.CivilWithoutLegCategory != null ? CivilWithoutLegLibrary.CivilWithoutLegCategory.Name : null;

                    object FK_InstallationCivilwithoutLegsType_Name = CivilWithoutLegLibrary.InstallationCivilwithoutLegsType != null ? CivilWithoutLegLibrary.InstallationCivilwithoutLegsType.Name : null;

                    object FK_structureType_Name = CivilWithoutLegLibrary.structureType != null ? CivilWithoutLegLibrary.structureType.Name : null;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, CivilWithoutLegLibrary, CivilWithoutLegLibrary.CivilWithoutLegCategoryId).ToList();

                    foreach (BaseAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Label.ToLower() == "civilsteelsupportcategory_name")
                        {
                            if (FK_CivilSteelSupportCategory_Name == null)
                                FKitem.Value = _unitOfWork.CivilSteelSupportCategoryRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_CivilSteelSupportCategory_Name;
                        }
                        else if (FKitem.Label.ToLower() == "civilwithoutlegcategory_name")
                        {
                            if (FK_CivilWithoutLegCategory_Name == null)
                                FKitem.Value = _unitOfWork.CivilWithoutLegCategoryRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_CivilWithoutLegCategory_Name;
                        }
                        else if (FKitem.Label.ToLower() == "installationcivilwithoutlegstype_name")
                        {
                            if (FK_InstallationCivilwithoutLegsType_Name == null)
                                FKitem.Value = _unitOfWork.InstallationCivilwithoutLegsTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_InstallationCivilwithoutLegsType_Name;
                        }
                        else if (FKitem.Label.ToLower() == "structuretype_name")
                        {
                            if (FK_structureType_Name == null)
                                FKitem.Value = _unitOfWork.StructureTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_structureType_Name;
                        }
                    }
                }
                else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == TableName)
                {
                    TLIcivilNonSteelLibrary CivilNonSteelLibrary = _unitOfWork.CivilNonSteelLibraryRepository.GetIncludeWhereFirst(x =>
                        x.Id == Id, x => x.civilNonSteelType);

                    object FK_civilNonSteelType_Name = CivilNonSteelLibrary.civilNonSteelType != null ? CivilNonSteelLibrary.civilNonSteelType.Name : null;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, CivilNonSteelLibrary, null).ToList();

                    foreach (BaseAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Label.ToLower() == "civilnonsteeltype_name")
                        {
                            if (FK_civilNonSteelType_Name == null)
                                FKitem.Value = _unitOfWork.CivilNonSteelTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = FK_civilNonSteelType_Name;
                        }
                    }
                }

                ListAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.CivilSupport.ToString(), TableName, Id));
                attributes.AttributesActivated = ListAttributesActivated;
                if (TableName == "TLIcivilWithoutLegLibrary")
                {
                    attributes.DynamicAtts = _unitOfWork.DynamicAttLibRepository.GetDynamicLibAtts(TableNameEntity.Id, Id, CatId);
                }
                else
                {
                    attributes.DynamicAtts = _unitOfWork.DynamicAttLibRepository.GetDynamicLibAtts(TableNameEntity.Id, Id, null);
                }

                List<BaseAttView> Test = attributes.AttributesActivated.ToList();
                BaseAttView NameAttribute = Test.FirstOrDefault(x => x.Key.ToLower() == "Model".ToLower());
                if (NameAttribute != null)
                {
                    BaseAttView Swap = Test.ToList()[0];
                    Test[Test.IndexOf(NameAttribute)] = Swap;
                    Test[0] = NameAttribute;
                    attributes.AttributesActivated = Test;
                }

                attributes.DynamicAttInst = null;

                return new Response<AllItemAttributes>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters 
        //First TableName to specify the table i deal with
        //Second CivilWithoutLegCategoryId to specify the category of civil Without Leg Library 
        //Function return all records depened on TableName
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
                civilWithoutLegLibraries = _unitOfWork.CivilWithoutLegLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, c => c.CivilSteelSupportCategory, c => c.CivilWithoutLegCategory, c => c.InstallationCivilwithoutLegsType).OrderBy(x => x.Id).ToList();
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
        public Response<AllItemAttributes> GetForAdd(string TableName, int? CategoryId = null)
        {
            try
            {
                AllItemAttributes Attributes = new AllItemAttributes();

                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c =>
                    c.TableName == TableName);

                // Civil Support Libraries
                if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    foreach (BaseAttView FKitem in listofAttributesActivated)
                    {
                        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                        {
                            if (FKitem.Desc.ToLower() == "tlicivilsteelsupportcategory")
                                FKitem.Value = _mapper.Map<List<CivilSteelSupportCategoryViewModel>>(_unitOfWork.CivilSteelSupportCategoryRepository.GetAllWithoutCount().ToList());

                            else if (FKitem.Desc.ToLower() == "tlisectionslegtype")
                                FKitem.Value = _mapper.Map<List<SectionsLegTypeViewModel>>(_unitOfWork.SectionsLegTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                            else if (FKitem.Desc.ToLower() == "tlistructuretype")
                                FKitem.Value = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.StructureTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                            else if (FKitem.Desc.ToLower() == "tlisupporttypedesigned")
                                FKitem.Value = _mapper.Map<List<SupportTypeDesignedViewModel>>(_unitOfWork.SupportTypeDesignedRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                            else if (FKitem.Desc.ToLower() == "tliinstallationcivilwithoutlegstype")
                                FKitem.Value = _mapper.Map<List<InstallationCivilwithoutLegsTypeViewModel>>(_unitOfWork.InstallationCivilwithoutLegsTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                            else if (FKitem.Desc.ToLower() == "tlicivilwithoutlegcategory")
                                FKitem.Value = _mapper.Map<List<CivilWithoutLegCategoryViewModel>>(_unitOfWork.CivilWithoutLegCategoryRepository.GetWhere(x => !x.disable).ToList());

                            else if (FKitem.Desc.ToLower() == "tlicivilnonsteeltype")
                                FKitem.Value = _mapper.Map<List<CivilNonSteelTypeViewModel>>(db.TLIcivilNonSteelType.Where(x => !x.Disable).ToList());
                        }
                    }

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.CivilSupport.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }
                else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == TableName)
                {
                    List<BaseAttView> ListOfAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, CategoryId, "CivilWithoutLegCategoryId").ToList();
                    foreach (BaseAttView item in ListOfAttributesActivated)
                    {
                        if (item.DataType.ToLower() == "list" && !string.IsNullOrEmpty(item.Desc))
                        {
                            if (item.Desc.ToLower() == "tlicivilsteelsupportcategory")
                                item.Value = _mapper.Map<List<CivilSteelSupportCategoryViewModel>>(_unitOfWork.CivilSteelSupportCategoryRepository.GetAllWithoutCount().ToList());

                            else if (item.Desc.ToLower() == "tlistructuretype")
                                item.Value = _mapper.Map<List<StructureTypeViewModel>>(_unitOfWork.StructureTypeRepository.GetWhere(x => x.Deleted == false && x.Disable == false).ToList());
                            
                            else if (item.Desc.ToLower() == "tliinstallationcivilwithoutlegstype")
                                item.Value = _mapper.Map<List<InstallationCivilwithoutLegsTypeViewModel>>(_unitOfWork.InstallationCivilwithoutLegsTypeRepository.GetWhere(x => x.Deleted == false && x.Disable == false).ToList());

                            else if (item.Desc.ToLower() == "tlicivilwithoutlegcategory")
                                item.Value = _mapper.Map<List<CivilWithoutLegCategoryViewModel>>(_unitOfWork.CivilWithoutLegCategoryRepository.GetWhere(x => x.disable == false).ToList());
                        }
                    }

                    // var listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString(), null, CategoryId).ToList();
                    // attributes.AttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString(), null, CategoryId);
                    //var listofAttributesActivated = attributes.AttributesActivated.ToList();

                    ListOfAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical("CivilSupport"));
                    Attributes.AttributesActivated = ListOfAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, CategoryId);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }
                else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == TableName)
                {
                    List<BaseAttView> ListOfAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();
                    foreach (BaseAttView item in ListOfAttributesActivated)
                    {
                        if (item.DataType.ToLower() == "list" && !string.IsNullOrEmpty(item.Desc))
                        {
                            if (item.Desc.ToLower() == "tlicivilnonsteeltype")
                                item.Value = _mapper.Map<List<CivilNonSteelTypeViewModel>>(db.TLIcivilNonSteelType.Where(x => !x.Disable).ToList());
                        }
                    }

                    //var listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString(), null, null).ToList();
                    //  var listofAttributesActivated = attributes.AttributesActivated.ToList();

                    ListOfAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical("CivilSupport"));
                    Attributes.AttributesActivated = ListOfAttributesActivated;
                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }

                // Microwave Load Libraries
                else if (Helpers.Constants.TablesNames.TLImwRFULibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();
                    foreach (BaseAttView FKitem in listofAttributesActivated)
                    {
                        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                        {
                            if (FKitem.Desc.ToLower() == "tlidiversitytype")
                                FKitem.Value = _mapper.Map<List<DiversityTypeViewModel>>(_unitOfWork.DiversityTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                            else if (FKitem.Desc.ToLower() == "tliboardtype")
                                FKitem.Value = _mapper.Map<List<BoardTypeViewModel>>(_unitOfWork.BoardTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                        }
                    }

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.MW.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }
                else if (Helpers.Constants.TablesNames.TLImwBULibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();
                    foreach (BaseAttView FKitem in listofAttributesActivated)
                    {
                        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                        {
                            if (FKitem.Desc.ToLower() == "tlidiversitytype")
                                FKitem.Value = _mapper.Map<List<DiversityTypeViewModel>>(_unitOfWork.DiversityTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                        }
                    }

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.MW.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }
                else if (Helpers.Constants.TablesNames.TLImwDishLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    foreach (BaseAttView FKitem in listofAttributesActivated)
                    {
                        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                        {
                            if (FKitem.Desc.ToLower() == "tlipolaritytype")
                                FKitem.Value = _mapper.Map<List<PolarityTypeViewModel>>(_unitOfWork.PolarityTypeRepository.GetAllWithoutCount().ToList());

                            else if (FKitem.Desc.ToLower() == "tliastype")
                                FKitem.Value = _mapper.Map<List<AsTypeViewModel>>(_unitOfWork.AsTypeRepository.GetAllWithoutCount().ToList());
                        }
                    }

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.MW.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }
                else if (Helpers.Constants.TablesNames.TLImwODULibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    foreach (BaseAttView FKitem in listofAttributesActivated)
                    {
                        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                        {
                            if (FKitem.Desc.ToLower() == "tliparity")
                                FKitem.Value = _mapper.Map<List<ParityViewModel>>(_unitOfWork.ParityRepository.GetAllWithoutCount().ToList());
                        }
                    }

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.MW.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }
                else if (Helpers.Constants.TablesNames.TLImwOtherLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.MW.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }

                // Radio Load Libraries
                else if (Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.Radio.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }
                else if (Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.Radio.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }
                else if (Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.Radio.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }

                // Power Load Library
                else if (Helpers.Constants.TablesNames.TLIpowerLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.Power.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }

                // Load Other Library
                else if (Helpers.Constants.TablesNames.TLIloadOtherLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.LoadOther.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }

                // Side Arm Library
                else if (Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.SideArm.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }

                // Other Inventories Libraries
                else if (Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    foreach (BaseAttView FKitem in listofAttributesActivated)
                    {
                        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                        {
                            if (FKitem.Desc.ToLower() == "tlicabinetpowertype")
                                FKitem.Value = _mapper.Map<List<CabinetPowerTypeViewModel>>(_unitOfWork.CabinetPowerTypeRepository.GetAllWithoutCount().ToList());
                        }
                    }

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }
                else if (Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    foreach (BaseAttView FKitem in listofAttributesActivated)
                    {
                        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                        {
                            if (FKitem.Desc.ToLower() == "tlitelecomtype")
                                FKitem.Value = _mapper.Map<List<TelecomTypeViewModel>>(_unitOfWork.TelecomTypeRepository.GetAllWithoutCount().ToList());
                        }
                    }

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }
                else if (Helpers.Constants.TablesNames.TLIsolarLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    foreach (BaseAttView FKitem in listofAttributesActivated)
                    {
                        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                        {
                            if (FKitem.Desc.ToLower() == "tlicapacity")
                                FKitem.Value = _mapper.Map<List<CapacityViewModel>>(_unitOfWork.CapacityRepository.GetAllWithoutCount().ToList());
                        }
                    }

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }
                else if (Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString() == TableName)
                {
                    List<BaseAttView> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, null, null).ToList();

                    foreach (BaseAttView FKitem in listofAttributesActivated)
                    {
                        if (FKitem.DataType.ToLower() == "list" && !string.IsNullOrEmpty(FKitem.Desc))
                        {
                            if (FKitem.Desc.ToLower() == "tlicapacity")
                                FKitem.Value = _mapper.Map<List<CapacityViewModel>>(_unitOfWork.CapacityRepository.GetAllWithoutCount().ToList());
                        }
                    }

                    listofAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString()));
                    Attributes.AttributesActivated = listofAttributesActivated;

                    IEnumerable<DynamicAttLibViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, null);

                    foreach (DynamicAttLibViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.Value = " ".Split(' ')[0];
                        }
                    }

                    Attributes.DynamicAtts = DynamicAttributesWithoutValue;
                    Attributes.DynamicAttInst = null;
                }

                foreach (BaseAttView Attribute in Attributes.AttributesActivated)
                {
                    if (Attribute.Key.ToLower() == "length" || Attribute.Key.ToLower() == "width" ||
                        Attribute.Key.ToLower() == "diameter" || Attribute.Key.ToLower() == "height")
                    {
                        Attribute.Manage = true;
                        Attribute.Required = true;
                        Attribute.enable = true;
                    }
                    if (Attribute.Key.ToLower() == "model")
                    {
                        Attribute.Required = true;
                        Attribute.enable = true;
                    }
                }

                List<BaseAttView> Test = Attributes.AttributesActivated.ToList();
                BaseAttView NameAttribute = Test.FirstOrDefault(x => x.Key.ToLower() == "Model".ToLower());
                if (NameAttribute != null)
                {
                    BaseAttView Swap = Test.ToList()[0];
                    Test[Test.IndexOf(NameAttribute)] = Swap;
                    Test[0] = NameAttribute;
                    Attributes.AttributesActivated = Test;
                }

                return new Response<AllItemAttributes>(true, Attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters 
        //First Id to specify the record i want to delete
        //Second CivilType to specify the table i deal with
        //Function Update Deleted column to true for record 
        //and Update Delete to all dynamic attributes values related to this record to true   
        public async Task<Response<AllItemAttributes>> Delete(int Id, string CivilType)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == CivilType);
                    if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == CivilType)
                    {
                        TLIcivilWithLegLibrary OldCivilWithLegLibrary = _unitOfWork.CivilWithLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);

                        TLIcivilWithLegLibrary NewCivilWithLegLibrary = _unitOfWork.CivilWithLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewCivilWithLegLibrary.Deleted = true;
                        NewCivilWithLegLibrary.Model = NewCivilWithLegLibrary.Model + "_" + DateTime.Now.ToString();

                        _unitOfWork.CivilWithLegLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldCivilWithLegLibrary, NewCivilWithLegLibrary);
                        _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                        //AddHistory(CivilWithLeg.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString());
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == CivilType)
                    {
                        TLIcivilWithoutLegLibrary OldCivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        TLIcivilWithoutLegLibrary NewCivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewCivilWithoutLegLibrary.Deleted = true;
                        NewCivilWithoutLegLibrary.Model = NewCivilWithoutLegLibrary.Model + "_" + DateTime.Now.ToString();
                        //TLIcivilWithoutLegLibrary NewCivilWithoutLegLibrary = _mapper.Map<TLIcivilWithoutLegLibrary>(OldCivilWithoutLegLibrary);
                        //NewCivilWithoutLegLibrary.Deleted = true;

                        _unitOfWork.CivilWithoutLegLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldCivilWithoutLegLibrary, NewCivilWithoutLegLibrary);
                        _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                        //AddHistory(CivilWithoutLeg.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString());
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == CivilType)
                    {
                        TLIcivilNonSteelLibrary OldCivilNonSteelLibrary = _unitOfWork.CivilNonSteelLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        TLIcivilNonSteelLibrary NewCivilNonSteelLibrary = _unitOfWork.CivilNonSteelLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        NewCivilNonSteelLibrary.Deleted = true;
                        NewCivilNonSteelLibrary.Model = NewCivilNonSteelLibrary.Model + "_" + DateTime.Now.ToString();
                        //TLIcivilNonSteelLibrary NewCivilNonSteelLibrary = _mapper.Map<TLIcivilNonSteelLibrary>(OldCivilNonSteelLibrary);
                        //NewCivilNonSteelLibrary.Deleted = true;

                        _unitOfWork.CivilNonSteelLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldCivilNonSteelLibrary, NewCivilNonSteelLibrary);
                        _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                        await _unitOfWork.SaveChangesAsync();
                        //AddHistory(CivilNonSteel.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString());
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
        public Response<ReturnWithFilters<object>> GetCivilWithLegLibrariesEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> CivilTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<CivilWithLegLibraryViewModel> CivilWithLegsLibraries = new List<CivilWithLegLibraryViewModel>();
                List<CivilWithLegLibraryViewModel> WithoutDateFilterCivilWithLegsLibraries = new List<CivilWithLegLibraryViewModel>();
                List<CivilWithLegLibraryViewModel> WithDateFilterCivilWithLegsLibraries = new List<CivilWithLegLibraryViewModel>();

                List<TLIattributeActivated> CivilWithLegLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    CivilWithLegLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CivilWithLegsLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateCivilWithLegLibraryAttribute = CivilWithLegLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateCivilWithLegLibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString()
                            , x => x.tablesNames, x => x.DataType).ToList();

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
                    bool AttrLibExist = typeof(CivilWithLegLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(CivilWithLegLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(CivilWithLegLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //LibraryAttributeActivatedIds = _unitOfWork.CivilWithLegLibraryRepository.GetWhere(x =>
                        //     LibraryPropsAttributeFilters.All(z =>
                        //        NonStringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null).ToString().ToLower()) : false)) ||
                        //        StringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                        //             y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                        // ).Select(i => i.Id).ToList();

                        IEnumerable<TLIcivilWithLegLibrary> Libraries = _unitOfWork.CivilWithLegLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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

                    WithoutDateFilterCivilWithLegsLibraries = _mapper.Map<List<CivilWithLegLibraryViewModel>>(_unitOfWork.CivilWithLegLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.supportTypeDesigned, x => x.sectionsLegType,
                        x => x.structureType, x => x.civilSteelSupportCategory).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateCivilWithLegLibraryAttribute = CivilWithLegLibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = DateCivilWithLegLibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

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
                    List<PropertyInfo> LibraryProps = typeof(CivilWithLegLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //LibraryAttributeActivatedIds = _unitOfWork.CivilWithLegLibraryRepository.GetIncludeWhere(x =>
                        //    LibraryPropsAttributeFilters.All(z =>
                        //        (LibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null) != null) ?
                        //            ((z.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null))) &&
                        //             (z.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null)))) : (false)))))
                        //).Select(i => i.Id).ToList();

                        IEnumerable<TLIcivilWithLegLibrary> Libraries = _unitOfWork.CivilWithLegLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithLegLibraryViewModel>(x), null)))) : (false))));
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

                    WithDateFilterCivilWithLegsLibraries = _mapper.Map<List<CivilWithLegLibraryViewModel>>(_unitOfWork.CivilWithLegLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.supportTypeDesigned, x => x.sectionsLegType,
                        x => x.structureType, x => x.civilSteelSupportCategory).ToList());
                }

                //
                // Intersect Between WithoutDateFilterCivilWithLegsLibraries + WithDateFilterCivilWithLegsLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    CivilWithLegsLibraries = _mapper.Map<List<CivilWithLegLibraryViewModel>>(_unitOfWork.CivilWithLegLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted, x => x.supportTypeDesigned, x => x.sectionsLegType, x => x.structureType, x => x.civilSteelSupportCategory).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> CivilIds = WithoutDateFilterCivilWithLegsLibraries.Select(x => x.Id).Intersect(WithDateFilterCivilWithLegsLibraries.Select(x => x.Id)).ToList();
                    CivilWithLegsLibraries = _mapper.Map<List<CivilWithLegLibraryViewModel>>(_unitOfWork.CivilWithLegLibraryRepository.GetWhere(x =>
                        CivilIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    CivilWithLegsLibraries = WithoutDateFilterCivilWithLegsLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    CivilWithLegsLibraries = WithDateFilterCivilWithLegsLibraries;
                }

                Count = CivilWithLegsLibraries.Count();

                CivilWithLegsLibraries = CivilWithLegsLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CivilWithLegsLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString()) : false),
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

                foreach (CivilWithLegLibraryViewModel CivilWithLegsLibraryViewModel in CivilWithLegsLibraries)
                {
                    dynamic DynamicCivilWithLegLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(CivilWithLegLibraryViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(CivilWithLegsLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicCivilWithLegLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(CivilWithLegsLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicCivilWithLegLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(CivilWithLegsLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicCivilWithLegLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == CivilWithLegsLibraryViewModel.Id && !x.disable &&
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

                            ((IDictionary<String, Object>)DynamicCivilWithLegLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicCivilWithLegLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(CivilWithLegLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(CivilWithLegsLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == CivilWithLegsLibraryViewModel.Id && !x.disable &&
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

                    ((IDictionary<String, Object>)DynamicCivilWithLegLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicCivilWithLegLibrary);
                }


                CivilTableDisplay.Model = OutPutList;

                if (WithFilterData)
                {
                    CivilTableDisplay.filters = _unitOfWork.CivilWithLegLibraryRepository.GetRelatedTables();
                }
                else
                {
                    CivilTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, CivilTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetCivilWithoutLegLibrariesEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, int CategoryId, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> CivilTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<CivilWithoutLegLibraryViewModel> CivilWithoutLegLibraries = new List<CivilWithoutLegLibraryViewModel>();

                List<TLIattributeActivated> CivilWithoutLegLibraryAttribute = new List<TLIattributeActivated>();

                List<CivilWithoutLegLibraryViewModel> WithoutDateFilterCivilWithoutLegLibraries = new List<CivilWithoutLegLibraryViewModel>();
                List<CivilWithoutLegLibraryViewModel> WithDateFilterCivilWithoutLegLibraries = new List<CivilWithoutLegLibraryViewModel>();

                string Category = _unitOfWork.CivilWithoutLegCategoryRepository.GetByID(CategoryId).Name.ToLower();
                string CategoryViewName = "";

                List<TLIattActivatedCategory> AttActivatedCategory = _unitOfWork.AttActivatedCategoryRepository.GetIncludeWhere(x =>
                    x.civilWithoutLegCategoryId == CategoryId, x => x.attributeActivated).ToList();

                if (Category.ToLower() == "mast")
                    CategoryViewName = Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryMast.ToString();

                else if (Category.ToLower() == "capsule")
                    CategoryViewName = Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryCapsule.ToString();

                else if (Category.ToLower() == "monopole")
                    CategoryViewName = Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryMonopole.ToString();

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    CivilWithoutLegLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == CategoryViewName &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();

                    foreach (TLIattributeActivated AttributeActivated in CivilWithoutLegLibraryAttribute)
                    {
                        AttributeActivated.Label = AttActivatedCategory.FirstOrDefault(x =>
                            x.attributeActivatedId == AttributeActivated.Id) != null ?
                        AttributeActivated.Label = AttActivatedCategory.FirstOrDefault(x =>
                            x.attributeActivatedId == AttributeActivated.Id).Label : "NA";
                    }
                }
                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> DateCivilWithoutLegLibraryAttribute = new List<TLIattributeActivated>();
                    DateCivilWithoutLegLibraryAttribute = CivilWithoutLegLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = DateCivilWithoutLegLibraryAttribute.FirstOrDefault(x =>
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
                        x.LibraryAtt && !x.disable && x.CivilWithoutLegCategoryId == CategoryId &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString(), x => x.tablesNames).ToList();

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
                    bool AttrLibExist = typeof(CivilWithoutLegLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivated = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(CivilWithoutLegLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(CivilWithoutLegLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.AsEnumerable().Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.AsEnumerable().Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //LibraryAttributeActivated = _unitOfWork.CivilWithoutLegLibraryRepository.GetWhere(x =>
                        //     LibraryPropsAttributeFilters.All(z =>
                        //        NonStringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null).ToString().ToLower()) : false)) ||
                        //        StringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                        //             y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                        // ).Select(i => i.Id).ToList();

                        IEnumerable<TLIcivilWithoutLegLibrary> Libraries = _unitOfWork.CivilWithoutLegLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        LibraryAttributeActivated = Libraries.Select(x => x.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivated.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivated;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithoutDateFilterCivilWithoutLegLibraries = _mapper.Map<List<CivilWithoutLegLibraryViewModel>>(_unitOfWork.CivilWithoutLegLibraryRepository.GetIncludeWhere(x =>
                        IntersectLibraryIds.Contains(x.Id) && !x.Deleted && x.Id > 0 &&
                        x.CivilWithoutLegCategoryId == CategoryId,
                        x => x.structureType, x => x.CivilSteelSupportCategory, x => x.InstallationCivilwithoutLegsType, x => x.CivilWithoutLegCategory).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> NotDateCivilWithoutLegLibraryAttribute = new List<TLIattributeActivated>();
                    NotDateCivilWithoutLegLibraryAttribute = CivilWithoutLegLibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = NotDateCivilWithoutLegLibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString(), x => x.tablesNames).ToList();

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
                    List<PropertyInfo> LibraryProps = typeof(CivilWithoutLegLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //LibraryAttributeActivatedIds = _unitOfWork.CivilWithoutLegLibraryRepository.GetIncludeWhere(x =>
                        //    LibraryPropsAttributeFilters.All(z =>
                        //        (LibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null) != null) ?
                        //            ((z.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null))) &&
                        //             (z.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null)))) : (false)))))
                        //).Select(i => i.Id).ToList();

                        IEnumerable<TLIcivilWithoutLegLibrary> Libraries = _unitOfWork.CivilWithoutLegLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithoutLegLibraryViewModel>(x), null)))) : (false))));
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

                    WithDateFilterCivilWithoutLegLibraries = _mapper.Map<List<CivilWithoutLegLibraryViewModel>>(_unitOfWork.CivilWithoutLegLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted &&
                        x.CivilWithoutLegCategoryId == CategoryId, x => x.CivilSteelSupportCategory, x => x.CivilWithoutLegCategory,
                        x => x.InstallationCivilwithoutLegsType, x => x.structureType).ToList());
                }

                //
                // Intersect Between WithoutDateFilterCivilWithLegsLibraries + WithDateFilterCivilWithLegsLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    CivilWithoutLegLibraries = _mapper.Map<List<CivilWithoutLegLibraryViewModel>>(_unitOfWork.CivilWithoutLegLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted && x.CivilWithoutLegCategoryId == CategoryId, x => x.CivilSteelSupportCategory, x => x.CivilWithoutLegCategory,
                        x => x.structureType, x => x.InstallationCivilwithoutLegsType).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> CivilIds = WithoutDateFilterCivilWithoutLegLibraries.Select(x => x.Id).Intersect(WithDateFilterCivilWithoutLegLibraries.Select(x => x.Id)).ToList();
                    CivilWithoutLegLibraries = _mapper.Map<List<CivilWithoutLegLibraryViewModel>>(_unitOfWork.CivilWithoutLegLibraryRepository.GetWhere(x =>
                        CivilIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    CivilWithoutLegLibraries = WithoutDateFilterCivilWithoutLegLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    CivilWithoutLegLibraries = WithDateFilterCivilWithoutLegLibraries;
                }

                Count = CivilWithoutLegLibraries.Count();

                CivilWithoutLegLibraries = CivilWithoutLegLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = new List<TLIattributeViewManagment>();

                List<AttActivatedCategoryViewModel> AttributeActivatedCategories = _mapper.Map<List<AttActivatedCategoryViewModel>>(_unitOfWork.AttActivatedCategoryRepository.GetIncludeWhere(x =>
                    (x.civilWithoutLegCategoryId.Value == CategoryId) &&
                    (x.enable) &&
                    (x.attributeActivated != null ?
                        ((x.attributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString()) ||
                        ((x.attributeActivated.Key.ToLower() == "id" || x.attributeActivated.Key.ToLower() == "active") && x.attributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString())) : false)
                    , x => x.attributeActivated).ToList());

                List<DynamicAttViewModel> DynamicAttViewModels = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.CivilWithoutLegCategoryId != null ? (
                        x.CivilWithoutLegCategoryId.Value == CategoryId && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString() && !x.disable && x.LibraryAtt
                    ) : false && !x.disable, x => x.tablesNames).ToList());

                AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == CategoryViewName &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString() &&
                            AttributeActivatedCategories.AsEnumerable().Select(y => y.attributeActivatedId).Contains(x.AttributeActivatedId)) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString() &&
                            (from y in DynamicAttViewModels select y.Id).ToList().Contains(x.DynamicAttId.Value)))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString() && x.EditableManagmentView.View == CategoryViewName) : false),
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

                foreach (CivilWithoutLegLibraryViewModel CivilWithoutLegLibraryViewModel in CivilWithoutLegLibraries)
                {
                    dynamic DynamicCivilWithoutLegLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel...
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(CivilWithoutLegLibraryViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(CivilWithoutLegLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicCivilWithoutLegLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattActivatedCategory LabelName = AttActivatedCategory.FirstOrDefault(x =>
                                        x.attributeActivated.Key.ToLower() == prop.Name.ToLower() &&
                                        AllAttributes.FirstOrDefault(y =>
                                            y.AttributeActivatedId != null ?
                                                y.AttributeActivated.Key.ToLower() == prop.Name.ToLower() : false) != null);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(CivilWithoutLegLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicCivilWithoutLegLibrary).Add(new KeyValuePair<string, object>(LabelName.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(CivilWithoutLegLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicCivilWithoutLegLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes...
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        x.CivilWithoutLegCategoryId == CategoryId &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == CivilWithoutLegLibraryViewModel.Id &&
                           !x.disable && x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.CivilWithoutLegCategoryId == CategoryId &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType, x => x.DynamicAtt.CivilWithoutLegCategory);

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

                            ((IDictionary<String, Object>)DynamicCivilWithoutLegLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicCivilWithoutLegLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(CivilWithoutLegLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(CivilWithoutLegLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString() &&
                        x.CivilWithoutLegCategoryId == CategoryId &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == CivilWithoutLegLibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.CivilWithoutLegCategoryId == CategoryId &&
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

                    ((IDictionary<String, Object>)DynamicCivilWithoutLegLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicCivilWithoutLegLibrary);
                }
                CivilTableDisplay.Model = OutPutList;

                if (WithFilterData)
                {
                    CivilTableDisplay.filters = _unitOfWork.CivilWithoutLegLibraryRepository.GetRelatedTables();
                }
                else
                {
                    CivilTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, CivilTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetCivilNonSteelLibrariesEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> CivilTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<CivilNonSteelLibraryViewModel> CivilNonSteelLibraries = new List<CivilNonSteelLibraryViewModel>();
                List<CivilNonSteelLibraryViewModel> WithoutDateFilterCivilNonSteelLibraries = new List<CivilNonSteelLibraryViewModel>();
                List<CivilNonSteelLibraryViewModel> WithDateFilterCivilNonSteelLibraries = new List<CivilNonSteelLibraryViewModel>();

                List<TLIattributeActivated> CivilNonSteelLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    CivilNonSteelLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CivilNonSteelLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateCivilNonSteelLibraryAttribute = CivilNonSteelLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateCivilNonSteelLibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

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
                    bool AttrLibExist = typeof(CivilNonSteelLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Where(y => y.ToLower() != "Id".ToLower())
                        .Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(CivilNonSteelLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.AsEnumerable().Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(CivilNonSteelLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.AsEnumerable().Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.AsEnumerable().Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.AsEnumerable().Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //LibraryAttributeActivatedIds = _unitOfWork.CivilNonSteelLibraryRepository.GetWhere(x =>
                        //     LibraryPropsAttributeFilters.All(z =>
                        //        NonStringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null).ToString().ToLower()) : false)) ||
                        //        StringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                        //             y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                        // ).Select(i => i.Id).ToList();

                        IEnumerable<TLIcivilNonSteelLibrary> Libraries = _unitOfWork.CivilNonSteelLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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

                    WithoutDateFilterCivilNonSteelLibraries = _mapper.Map<List<CivilNonSteelLibraryViewModel>>(_unitOfWork.CivilNonSteelLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.civilNonSteelType).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateCivilNonSteelLibraryAttribute = CivilNonSteelLibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = DateCivilNonSteelLibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString(), x => x.tablesNames).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (DateTimeLibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeLibDynamicAttListIds.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                        DynamicLibValueListIds = new List<int>();

                        List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            DateTimeLibDynamicAttListIds.AsEnumerable().Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

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
                    List<PropertyInfo> LibraryProps = typeof(CivilNonSteelLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //LibraryAttributeActivatedIds = _unitOfWork.CivilNonSteelLibraryRepository.GetIncludeWhere(x =>
                        //    LibraryPropsAttributeFilters.All(z =>
                        //        (LibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null) ?
                        //            ((z.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null))) &&
                        //             (z.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null)))) : (false)))))
                        //).Select(i => i.Id).ToList();

                        IEnumerable<TLIcivilNonSteelLibrary> Libraries = _unitOfWork.CivilNonSteelLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilNonSteelLibraryViewModel>(x), null)))) : (false))));
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

                    WithDateFilterCivilNonSteelLibraries = _mapper.Map<List<CivilNonSteelLibraryViewModel>>(_unitOfWork.CivilNonSteelLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted, x => x.civilNonSteelType).ToList());
                }

                //
                // Intersect Between WithoutDateFilterCivilNonSteelLibraries + WithDateFilterCivilNonSteelLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    CivilNonSteelLibraries = _mapper.Map<List<CivilNonSteelLibraryViewModel>>(_unitOfWork.CivilNonSteelLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted, x => x.civilNonSteelType).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> CivilIds = WithoutDateFilterCivilNonSteelLibraries.Select(x => x.Id).Intersect(WithDateFilterCivilNonSteelLibraries.Select(x => x.Id)).ToList();
                    CivilNonSteelLibraries = _mapper.Map<List<CivilNonSteelLibraryViewModel>>(_unitOfWork.CivilNonSteelLibraryRepository.GetWhere(x =>
                        CivilIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    CivilNonSteelLibraries = WithoutDateFilterCivilNonSteelLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    CivilNonSteelLibraries = WithDateFilterCivilNonSteelLibraries;
                }

                Count = CivilNonSteelLibraries.Count();

                CivilNonSteelLibraries = CivilNonSteelLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CivilNonSteelLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString()) : false),
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

                foreach (CivilNonSteelLibraryViewModel CivilNonSteelLibraryViewModel in CivilNonSteelLibraries)
                {
                    dynamic DynamicCivilNonSteelLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(CivilNonSteelLibraryViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(CivilNonSteelLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(CivilNonSteelLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(CivilNonSteelLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == CivilNonSteelLibraryViewModel.Id && !x.disable &&
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

                            ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(CivilNonSteelLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(CivilNonSteelLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == CivilNonSteelLibraryViewModel.Id && !x.disable &&
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

                    ((IDictionary<String, Object>)DynamicCivilNonSteelLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicCivilNonSteelLibrary);
                }

                CivilTableDisplay.Model = OutPutList;

                if (WithFilterData)
                {
                    CivilTableDisplay.filters = _unitOfWork.CivilNonSteelLibraryRepository.GetRelatedTables();
                }
                else
                {
                    CivilTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, CivilTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
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
