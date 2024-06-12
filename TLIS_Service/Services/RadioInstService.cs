using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Collections;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_DAL.ViewModels.RuleDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_DAL.ViewModels.WorkflowHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using static TLIS_DAL.ViewModels.MW_DishLbraryDTOs.EditMWDishLibraryObject;
using TLIS_DAL.ViewModels.AsTypeDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.ItemConnectToDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.PolarityOnLocationDTOs;
using TLIS_DAL.ViewModels.PolarityTypeDTOs;
using TLIS_DAL.ViewModels.RepeaterTypeDTOs;
using static TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs.EditRadioAntennaLibraryObject;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.SectionsLegTypeDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using static TLIS_DAL.ViewModels.RadioRRULibraryDTOs.EditRadioRRULibraryObject;

namespace TLIS_Service.Services
{
    public class RadioInstService : IRadioInstService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        ApplicationDbContext _dbContext;
        private IMapper _mapper;
        public RadioInstService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext dbContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        //Function take 3 parameters RadioInstallationViewModel, TableName, SiteCode
        //get table name Entity by TableName
        //specify the table i deal with
        //map object to ViewModel
        //map ViewModel to Entity
        //check business rules
        //add Entity
        //add dynamic installation attributes values
        //add record to TLIcivilLoads table to specify where i have to install the load (on sideArm or on civil)
        #region Helper Methods..
        public void LoopForPath(List<string> Path, int StartIndex, ApplicationDbContext _dbContext, object Value, List<object> OutPutIds)
        {
            if (StartIndex == Path.Count())
            {
                OutPutIds.Add(Value);
            }
            else
            {
                List<object> TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                    .GetProperties().FirstOrDefault(x => x.Name.ToLower() == Path[StartIndex].ToLower())
                    .GetValue(_dbContext, null))
                        .Where(x => x.GetType().GetProperty(Path[StartIndex + 1]).GetValue(x, null) != null ?
                            x.GetType().GetProperty(Path[StartIndex + 1]).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower() : false).ToList();

                foreach (object Record in TableRecords)
                {
                    // The New Value..
                    object PrimaryKeyValue = Record.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower() == Path[StartIndex + 2].ToLower()).GetValue(Record, null);

                    if (StartIndex + 3 < Path.Count())
                        LoopForPath(Path, StartIndex + 3, _dbContext, PrimaryKeyValue, OutPutIds);

                    else if (StartIndex + 3 == Path.Count())
                        OutPutIds.Add(PrimaryKeyValue);
                }
            }
        }
        public List<object> GetRecordsIds(string MainTableName, AddInstRuleViewModel Rule)
        {
            string SDTableName = Rule.TableName;
            string Operation = _unitOfWork.OperationRepository.GetByID(Rule.OperationId.Value).Name;
            object Value = new object();

            string DataType = "";

            if (Rule.OperationValueBoolean != null)
            {
                DataType = "Bool";
                Value = Rule.OperationValueBoolean;
            }
            else if (Rule.OperationValueDateTime != null)
            {
                DataType = "DateTime";
                Value = Rule.OperationValueDateTime;
            }
            else if (Rule.OperationValueDouble != null)
            {
                DataType = "Double";
                Value = Rule.OperationValueDouble;
            }
            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
            {
                DataType = "String";
                Value = Rule.OperationValueString;
            }

            List<object> OutPutIds = new List<object>();

            PathToCheckDependencyValidation Item = new PathToCheckDependencyValidation();

            if (MainTableName.ToLower() == Helpers.Constants.TablesNames.TLIradioRRU.ToString().ToLower() ||
                    SDTableName.ToLower() == Helpers.Constants.TablesNames.TLIradioRRU.ToString().ToLower())
                Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                    (MainTableName + SDTableName).ToLower());

            else
                Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                    MainTableName + SDTableName);

            List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

            if (Path[0].ToLower() == MainTableName.ToLower() &&
                Path[1].ToLower() == SDTableName.ToLower())
            {
                List<object> TableRecords = new List<object>();
                if (Rule.attributeActivatedId != null && !Rule.IsDynamic)
                {
                    string AttributeName = _unitOfWork.AttributeActivatedRepository
                        .GetByID(Rule.attributeActivatedId.Value).Key;

                    TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                        .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                            (DataType.ToLower() == "DateTime".ToLower() ?
                                Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 :
                            DataType.ToLower() == "Double".ToLower() ?
                                Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 : false) :
                        Operation == ">=" ?
                            (DataType.ToLower() == "DateTime".ToLower() ?
                                (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 ||
                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) :
                            DataType.ToLower() == "Double".ToLower() ?
                                (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 ||
                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) : false) :
                        Operation == "<" ?
                            (DataType.ToLower() == "DateTime".ToLower() ?
                                Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 :
                            DataType.ToLower() == "Double".ToLower() ?
                                Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 : false) :
                        Operation == "<=" ?
                            (DataType.ToLower() == "DateTime".ToLower() ?
                                (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 ||
                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) :
                            DataType.ToLower() == "Double".ToLower() ?
                                (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 ||
                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) : false) :
                        Operation == "==" ?
                            x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower() :
                        Operation == "!=" ?
                            x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != Value.ToString().ToLower() : false) : false).ToList();

                    foreach (object Record in TableRecords)
                    {
                        object PrimaryKeyValue = Record.GetType().GetProperty(Path[2]).GetValue(Record, null);

                        LoopForPath(Path, 3, _dbContext, PrimaryKeyValue, OutPutIds);
                    }
                }
                else if (Rule.dynamicAttId != null && Rule.IsDynamic)
                {
                    TLIdynamicAtt DynamicAttribute = _unitOfWork.DynamicAttRepository
                        .GetByID(Rule.dynamicAttId.Value);

                    if (!DynamicAttribute.LibraryAtt)
                    {
                        List<TLIdynamicAttInstValue> DynamicAttValues = new List<TLIdynamicAttInstValue>();

                        if (Rule.OperationValueBoolean != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDateTime != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDouble != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false)).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                               .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                   (!string.IsNullOrEmpty(x.ValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)).ToList();
                        }
                        if (DynamicAttValues != null ? DynamicAttValues.Count() > 0 : false)
                        {
                            TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                .GetProperty(Path[1]).GetValue(_dbContext, null))
                                    .Where(x => DynamicAttValues.FirstOrDefault(y =>
                                        y.InventoryId.ToString() == x.GetType().GetProperty("Id").GetValue(x, null).ToString() ? (
                                            (y.ValueBoolean != null ? (
                                                Operation == "==" ? y.ValueBoolean.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueBoolean.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueDateTime != null ? (
                                                Operation == "==" ? y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 :
                                                Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 ||
                                                    y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 :
                                                Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 ||
                                                    y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "!=" ? y.ValueDateTime.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueDouble != null ? (
                                                Operation == "==" ? y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 :
                                                Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 ||
                                                    y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 :
                                                Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 ||
                                                    y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "!=" ? y.ValueDouble.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueString != null ? (
                                                Operation == "==" ? y.ValueString.ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueString.ToLower() != Value.ToString().ToLower() : false
                                            ) : false)
                                        ) : false) != null).ToList();

                            foreach (object Record in TableRecords)
                            {
                                object PrimaryKeyValue = Record.GetType().GetProperty(Path[2]).GetValue(Record, null);

                                LoopForPath(Path, 3, _dbContext, PrimaryKeyValue, OutPutIds);
                            }
                        }
                    }
                    else
                    {
                        List<TLIdynamicAttLibValue> DynamicAttValues = new List<TLIdynamicAttLibValue>();

                        if (Rule.OperationValueBoolean != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDateTime != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDouble != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false)).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                               .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                   (!string.IsNullOrEmpty(x.ValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)).ToList();
                        }
                        if (DynamicAttValues != null ? DynamicAttValues.Count() > 0 : false)
                        {
                            TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                .GetProperty(Path[1]).GetValue(_dbContext, null))
                                    .Where(x => DynamicAttValues.FirstOrDefault(y =>
                                        y.InventoryId.ToString() == x.GetType().GetProperty("Id").GetValue(x, null).ToString() ? (
                                            (y.ValueBoolean != null ? (
                                                Operation == "==" ? y.ValueBoolean.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueBoolean.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueDateTime != null ? (
                                                Operation == "==" ? y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 :
                                                Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 ||
                                                    y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 :
                                                Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 ||
                                                    y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "!=" ? y.ValueDateTime.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueDouble != null ? (
                                                Operation == "==" ? y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 :
                                                Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 ||
                                                    y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 :
                                                Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 ||
                                                    y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "!=" ? y.ValueDouble.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueString != null ? (
                                                Operation == "==" ? y.ValueString.ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueString.ToLower() != Value.ToString().ToLower() : false
                                            ) : false)
                                        ) : false) != null).ToList();

                            foreach (object Record in TableRecords)
                            {
                                object PrimaryKeyValue = Record.GetType().GetProperty(Path[2]).GetValue(Record, null);

                                LoopForPath(Path, 3, _dbContext, PrimaryKeyValue, OutPutIds);
                            }
                        }
                    }
                }
            }
            OutPutIds = OutPutIds.Distinct().ToList();
            return OutPutIds;
        }
        public string CheckDependencyValidationForRadioTypes(object Input, string RadioType, string SiteCode)
        {
            if (RadioType.ToLower() == TablesNames.TLIradioAntenna.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIradioAntenna.ToString();
                AddRadioAntennaViewModel AddInstallationViewModel = _mapper.Map<AddRadioAntennaViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = AddInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicAttInstValueViewModel DynamicObject = AddInstallationViewModel.TLIdynamicAttInstValue
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

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    MainTableName + SDTableName + "Goal");

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = AddInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(AddInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (AddInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(AddInstallationViewModel, null) != null)
                                        CheckId = (int)AddInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(AddInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            AddDynamicAttInstValueViewModel InputDynamicAttribute = AddInstallationViewModel.TLIdynamicAttInstValue
                                .FirstOrDefault(x => x.DynamicAttId == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                object DependencyValidationValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;

                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;

                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;

                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;

                                object InputDynamicValue = new object();

                                if (InputDynamicAttribute.ValueBoolean != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueBoolean;

                                else if (InputDynamicAttribute.ValueDateTime != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDateTime;

                                else if (InputDynamicAttribute.ValueDouble != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDouble;

                                else if (!string.IsNullOrEmpty(InputDynamicAttribute.ValueString))
                                    InputDynamicValue = InputDynamicAttribute.ValueString;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            else if (RadioType.ToLower() == TablesNames.TLIradioRRU.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIradioRRU.ToString();
                AddRadioRRUViewModel AddInstallationViewModel = _mapper.Map<AddRadioRRUViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = AddInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicAttInstValueViewModel DynamicObject = AddInstallationViewModel.TLIdynamicAttInstValue
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

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    (MainTableName + SDTableName + "Goal").ToLower());

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = AddInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(AddInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (AddInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(AddInstallationViewModel, null) != null)
                                        CheckId = (int)AddInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(AddInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            AddDynamicAttInstValueViewModel InputDynamicAttribute = AddInstallationViewModel.TLIdynamicAttInstValue
                                .FirstOrDefault(x => x.DynamicAttId == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                object DependencyValidationValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;

                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;

                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;

                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;

                                object InputDynamicValue = new object();

                                if (InputDynamicAttribute.ValueBoolean != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueBoolean;

                                else if (InputDynamicAttribute.ValueDateTime != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDateTime;

                                else if (InputDynamicAttribute.ValueDouble != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDouble;

                                else if (!string.IsNullOrEmpty(InputDynamicAttribute.ValueString))
                                    InputDynamicValue = InputDynamicAttribute.ValueString;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            else if (RadioType.ToLower() == TablesNames.TLIradioOther.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIradioOther.ToString();
                AddRadioOtherViewModel AddInstallationViewModel = _mapper.Map<AddRadioOtherViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = AddInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicAttInstValueViewModel DynamicObject = AddInstallationViewModel.TLIdynamicAttInstValue
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

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    MainTableName + SDTableName + "Goal");

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = AddInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(AddInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (AddInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(AddInstallationViewModel, null) != null)
                                        CheckId = (int)AddInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(AddInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            AddDynamicAttInstValueViewModel InputDynamicAttribute = AddInstallationViewModel.TLIdynamicAttInstValue
                                .FirstOrDefault(x => x.DynamicAttId == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                object DependencyValidationValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;

                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;

                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;

                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;

                                object InputDynamicValue = new object();

                                if (InputDynamicAttribute.ValueBoolean != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueBoolean;

                                else if (InputDynamicAttribute.ValueDateTime != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDateTime;

                                else if (InputDynamicAttribute.ValueDouble != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDouble;

                                else if (!string.IsNullOrEmpty(InputDynamicAttribute.ValueString))
                                    InputDynamicValue = InputDynamicAttribute.ValueString;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
        public string CheckGeneralValidationFunction(List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue, string TableName)
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
                    AddDynamicAttInstValueViewModel DynmaicAttributeValue = TLIdynamicAttInstValue.FirstOrDefault(x => x.DynamicAttId == DynamicAttributeEntity.Id);

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
        #endregion
        public Response<GetForAddMWDishInstallationObject> AddRadioInstallation(object RadioInstallationViewModel, string TableName, string SiteCode, string ConnectionString, int? TaskId, int UserId)
        {
            using (var con = new OracleConnection(ConnectionString))
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
                            if (LoadSubType.TLIradioAntenna.ToString() == TableName)
                            {
                                AddRadioAntennaInstallationObject AddRadioAntenna = _mapper.Map<AddRadioAntennaInstallationObject>(RadioInstallationViewModel);
                                TLIradioAntenna RadioAntenna = _mapper.Map<TLIradioAntenna>(AddRadioAntenna.installationAttributes);
                                var RadioAntennaLibrary = _unitOfWork.RadioAntennaLibraryRepository.GetWhereFirst(x => x.Id == AddRadioAntenna.installationConfig.radioAntennaLibraryId
                                && !x.Deleted && x.Active);
                                if (RadioAntennaLibrary == null)
                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "MWDishLibrary is not found", (int)ApiReturnCode.fail);

                                if (AddRadioAntenna.installationConfig.InstallationPlaceId == 1)
                                {

                                    if (AddRadioAntenna.installationConfig.civilWithLegId != null)
                                    {
                                        TLIcivilSiteDate AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                         AddRadioAntenna.installationConfig.civilWithLegId && !x.Dismantle && x.SiteCode.ToLower()==SiteCode.ToLower()
                                         , x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                         x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                        if (AllcivilinstId != null)
                                        {
                                            if (AddRadioAntenna.installationConfig.legId != null)
                                            {
                                                var Leg = _unitOfWork.LegRepository.GetIncludeWhereFirst(x => x.CivilWithLegInstId ==
                                                 AddRadioAntenna.installationConfig.civilWithLegId  && x.Id == AddRadioAntenna.installationConfig.legId
                                                 , x => x.CivilWithLegInst);
                                                if (Leg != null)
                                                {
                                                    if (!string.IsNullOrEmpty(RadioAntenna.SerialNumber))
                                                    {
                                                        bool CheckSerialNumber = _dbContext.MV_RADIO_ANTENNA_VIEW.Any(x => x.SerialNumber == RadioAntenna.SerialNumber && !x.Dismantle);
                                                        if (CheckSerialNumber)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {RadioAntenna.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                    }

                                                    if (AddRadioAntenna.civilLoads.ReservedSpace == true)
                                                    {
                                                        var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                        if (Message != "Success")
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                        }
                                                        if (RadioAntenna.CenterHigh <= 0)
                                                        {
                                                            if (RadioAntenna.HBASurface <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaLibrary.Length / 2;
                                                            }
                                                        }
                                                        else if (RadioAntenna.SpaceInstallation == 0)
                                                        {
                                                            if (RadioAntennaLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (RadioAntennaLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaLibrary.Width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaLibrary.Length * RadioAntennaLibrary.Width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        else if (AddRadioAntenna.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (AddRadioAntenna.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }

                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                x.LEGID == AddRadioAntenna.installationConfig.legId
                                                                && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                        if (CheckAzimuthAndHeightBase != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                        TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == AddRadioAntenna.installationConfig.legId);
                                                        if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                        {
                                                            RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                        }

                                                        var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                        (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                           && x.SiteCode.ToLower() == SiteCode.ToLower()));

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                        {
                                                            AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                        }
                                                        var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                        if (OldVcivilinfo != null)
                                                        {
                                                            if (RadioAntenna.SpaceInstallation != 0 && RadioAntenna.CenterHigh != 0 && AllcivilinstId.allCivilInst.civilWithLegs.HeightBase != 0)
                                                            {
                                                                var EquivalentSpace = RadioAntenna.SpaceInstallation * (RadioAntenna.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);

                                                                AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                                RadioAntenna.EquivalentSpace = EquivalentSpace;
                                                                _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                                _unitOfWork.SaveChanges();
                                                            }
                                                        }

                                                        RadioAntenna.radioAntennaLibraryId = AddRadioAntenna.installationConfig.radioAntennaLibraryId;
                                                        RadioAntenna.installationPlaceId = AddRadioAntenna.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.RadioAntennaRepository.AddWithHistory(UserId, RadioAntenna);
                                                        _unitOfWork.SaveChanges();
                                                        int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIradioAntenna.ToString(), RadioAntenna.Id);
                                                        if (AddRadioAntenna.civilLoads != null && Id != 0)
                                                        {
                                                            TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                            {
                                                                InstallationDate = AddRadioAntenna.civilLoads.InstallationDate,
                                                                allLoadInstId = Id,
                                                                legId = AddRadioAntenna.installationConfig?.legId,
                                                                allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                                sideArmId = null,
                                                                ItemOnCivilStatus = AddRadioAntenna.civilLoads.ItemOnCivilStatus,
                                                                ItemStatus = AddRadioAntenna.civilLoads?.ItemStatus,
                                                                Dismantle = false,
                                                                ReservedSpace = AddRadioAntenna.civilLoads.ReservedSpace,
                                                                SiteCode = SiteCode,


                                                            };
                                                            _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                            _unitOfWork.SaveChanges();

                                                        }

                                                        if (AddRadioAntenna.dynamicAttribute.Count > 0)
                                                        {

                                                            _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallations(UserId, AddRadioAntenna.dynamicAttribute, TableNameEntity.Id, RadioAntenna.Id, ConnectionString);

                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (RadioAntenna.CenterHigh <= 0)
                                                        {
                                                            if (RadioAntenna.HBASurface <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaLibrary.Length / 2;
                                                            }
                                                        }
                                                        else if (RadioAntenna.SpaceInstallation == 0)
                                                        {
                                                            if (RadioAntennaLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (RadioAntennaLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaLibrary.Width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaLibrary.Length * RadioAntennaLibrary.Width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaLibrary.SpaceLibrary;
                                                            }
                                                        }
                                                        else if (AddRadioAntenna.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (AddRadioAntenna.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                x.LEGID == AddRadioAntenna.installationConfig.legId
                                                                && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                        if (CheckAzimuthAndHeightBase != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                        TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == AddRadioAntenna.installationConfig.legId);
                                                        if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                        {
                                                            RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                        }
                                                        var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                                                                               (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                                                                                  && x.SiteCode.ToLower() == SiteCode.ToLower()));

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        RadioAntenna.radioAntennaLibraryId = AddRadioAntenna.installationConfig.radioAntennaLibraryId;
                                                        RadioAntenna.installationPlaceId = AddRadioAntenna.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.RadioAntennaRepository.AddWithHistory(UserId, RadioAntenna);
                                                        _unitOfWork.SaveChanges();
                                                        int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIradioAntenna.ToString(), RadioAntenna.Id);
                                                        if (AddRadioAntenna.civilLoads != null && Id != 0)
                                                        {
                                                            TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                            {
                                                                InstallationDate = AddRadioAntenna.civilLoads.InstallationDate,
                                                                allLoadInstId = Id,
                                                                legId = AddRadioAntenna.installationConfig?.legId,
                                                                allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                                sideArmId = null,
                                                                ItemOnCivilStatus = AddRadioAntenna.civilLoads.ItemOnCivilStatus,
                                                                ItemStatus = AddRadioAntenna.civilLoads?.ItemStatus,
                                                                Dismantle = false,
                                                                ReservedSpace = AddRadioAntenna.civilLoads.ReservedSpace,
                                                                SiteCode = SiteCode,


                                                            };
                                                            _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                            _unitOfWork.SaveChanges();

                                                        }

                                                       
                                                        if (AddRadioAntenna.dynamicAttribute != null ? AddRadioAntenna.dynamicAttribute.Count > 0 : false)
                                                        {
                                                            foreach (var DynamicAttInstValue in AddRadioAntenna.dynamicAttribute)
                                                            {
                                                                _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, RadioAntenna.Id, ConnectionString);
                                                            }
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this leg is not found", (int)ApiReturnCode.fail);
                                                }
                                            }
                                            else
                                            {
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected leg ", (int)ApiReturnCode.fail);
                                            }



                                        }
                                        else
                                        {
                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this civil is not found ", (int)ApiReturnCode.fail);
                                        }

                                    }
                                    else
                                    {
                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected civilwithlegs item ", (int)ApiReturnCode.fail);
                                    }

                                }

                                if (AddRadioAntenna.installationConfig.InstallationPlaceId == 2)
                                {
                                    if (AddRadioAntenna.installationConfig.civilSteelType == 0)
                                    {
                                        if (AddRadioAntenna.installationConfig.civilWithLegId != null)
                                        {
                                            var AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                               AddRadioAntenna.installationConfig.civilWithLegId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                               x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                            if (AllcivilinstId != null)
                                            {
                                                if (AddRadioAntenna.installationConfig.sideArmId != null)
                                                {
                                                    var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                                      AddRadioAntenna.installationConfig.civilWithLegId && !x.Dismantle && x.sideArmId == AddRadioAntenna.installationConfig.sideArmId, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                                      x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                                    if (SideArm != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(RadioAntenna.SerialNumber))
                                                        {
                                                            bool CheckSerialNumber = _dbContext.MV_RADIO_ANTENNA_VIEW.Any(x => x.SerialNumber == RadioAntenna.SerialNumber && !x.Dismantle);
                                                            if (CheckSerialNumber)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {RadioAntenna.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                        }

                                                        if (AllcivilinstId != null)
                                                        {
                                                            if (AddRadioAntenna.civilLoads.ReservedSpace == true)
                                                            {
                                                                var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                                if (Message != "Success")
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                                }
                                                                if (RadioAntenna.CenterHigh <= 0)
                                                                {
                                                                    if (RadioAntenna.HBASurface <= 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else if (RadioAntennaLibrary.Length <= 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else
                                                                    {
                                                                        RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaLibrary.Length / 2;
                                                                    }
                                                                }
                                                                else if (RadioAntenna.SpaceInstallation == 0)
                                                                {
                                                                    if (RadioAntennaLibrary.SpaceLibrary == 0)
                                                                    {
                                                                        if (RadioAntennaLibrary.Length == 0)
                                                                        {
                                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                        }
                                                                        else if (RadioAntennaLibrary.Width == 0)
                                                                        {
                                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                        }
                                                                        else
                                                                        {
                                                                            RadioAntenna.SpaceInstallation = RadioAntennaLibrary.Length * RadioAntennaLibrary.Width;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        RadioAntenna.SpaceInstallation = RadioAntennaLibrary.SpaceLibrary;
                                                                    }
                                                                }

                                                                else if (AddRadioAntenna.installationAttributes.Azimuth <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (AddRadioAntenna.installationAttributes.HeightBase <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                      x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                      x.SIDEARM_ID == AddRadioAntenna.installationConfig.sideArmId
                                                                      && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                                if (CheckAzimuthAndHeightBase != null)
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                                var SideArmName1 = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == AddRadioAntenna.installationConfig.sideArmId);
                                                                if (SideArmName1 != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                                {
                                                                    RadioAntenna.Name = SideArmName1?.Name + " " + RadioAntenna.Azimuth + " " + RadioAntenna.HeightBase;
                                                                }



                                                                var CheckName = _dbContext.MV_MWDISH_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                                        (x.Id != null ? x.DishName.ToLower() == RadioAntenna.Name.ToLower() : false
                                                                           && x.SiteCode.ToLower() == SiteCode.ToLower()));

                                                                if (CheckName != null)
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                                if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                                {
                                                                    AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                                }
                                                                var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                                if (OldVcivilinfo != null)
                                                                {

                                                                    var EquivalentSpace = RadioAntenna.SpaceInstallation * (RadioAntenna.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);

                                                                    AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                                    RadioAntenna.EquivalentSpace = EquivalentSpace;
                                                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                                    _unitOfWork.SaveChanges();
                                                                }


                                                                RadioAntenna.radioAntennaLibraryId = AddRadioAntenna.installationConfig.radioAntennaLibraryId;
                                                                RadioAntenna.installationPlaceId = AddRadioAntenna.installationConfig.InstallationPlaceId;
                                                                _unitOfWork.RadioAntennaRepository.AddWithHistory(UserId, RadioAntenna);
                                                                _unitOfWork.SaveChanges();
                                                                int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIradioAntenna.ToString(), RadioAntenna.Id);
                                                                if (AddRadioAntenna.civilLoads != null && Id != 0)
                                                                {
                                                                    TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                                    {
                                                                        InstallationDate = AddRadioAntenna.civilLoads.InstallationDate,
                                                                        allLoadInstId = Id,
                                                                        legId = AddRadioAntenna.installationConfig?.legId,
                                                                        allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                                        sideArmId = null,
                                                                        ItemOnCivilStatus = AddRadioAntenna.civilLoads.ItemOnCivilStatus,
                                                                        ItemStatus = AddRadioAntenna.civilLoads?.ItemStatus,
                                                                        Dismantle = false,
                                                                        ReservedSpace = AddRadioAntenna.civilLoads.ReservedSpace,
                                                                        SiteCode = SiteCode,


                                                                    };
                                                                    _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                                    _unitOfWork.SaveChanges();

                                                                }

                                                                if (AddRadioAntenna.dynamicAttribute != null ? AddRadioAntenna.dynamicAttribute.Count > 0 : false)
                                                                {
                                                                    foreach (var DynamicAttInstValue in AddRadioAntenna.dynamicAttribute)
                                                                    {
                                                                        _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, RadioAntenna.Id, ConnectionString);
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                                if (Message != "Success")
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                                }
                                                                if (RadioAntenna.CenterHigh <= 0)
                                                                {
                                                                    if (RadioAntenna.HBASurface <= 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else if (RadioAntennaLibrary.Length <= 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else
                                                                    {
                                                                        RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaLibrary.Length / 2;
                                                                    }
                                                                }
                                                                else if (RadioAntenna.SpaceInstallation == 0)
                                                                {
                                                                    if (RadioAntennaLibrary.SpaceLibrary == 0)
                                                                    {
                                                                        if (RadioAntennaLibrary.Length == 0)
                                                                        {
                                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                        }
                                                                        else if (RadioAntennaLibrary.Width == 0)
                                                                        {
                                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                        }
                                                                        else
                                                                        {
                                                                            RadioAntenna.SpaceInstallation = RadioAntennaLibrary.Length * RadioAntennaLibrary.Width;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        RadioAntenna.SpaceInstallation = RadioAntennaLibrary.SpaceLibrary;
                                                                    }
                                                                }

                                                                else if (AddRadioAntenna.installationAttributes.Azimuth <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (AddRadioAntenna.installationAttributes.HeightBase <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                      x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                      x.SIDEARM_ID == AddRadioAntenna.installationConfig.sideArmId
                                                                      && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase);

                                                                if (CheckAzimuthAndHeightBase != null)
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                                var SideArmName1 = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == AddRadioAntenna.installationConfig.sideArmId);
                                                                if (SideArmName1 != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                                {
                                                                    RadioAntenna.Name = SideArmName1?.Name + " " + RadioAntenna.Azimuth + " " + RadioAntenna.HeightBase;
                                                                }



                                                                var CheckName = _dbContext.MV_MWDISH_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                                        (x.Id != null ? x.DishName.ToLower() == RadioAntenna.Name.ToLower() : false
                                                                           && x.SiteCode.ToLower() == SiteCode.ToLower()));

                                                                if (CheckName != null)
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                                RadioAntenna.radioAntennaLibraryId = AddRadioAntenna.installationConfig.radioAntennaLibraryId;
                                                                RadioAntenna.installationPlaceId = AddRadioAntenna.installationConfig.InstallationPlaceId;
                                                                _unitOfWork.RadioAntennaRepository.AddWithHistory(UserId, RadioAntenna);
                                                                _unitOfWork.SaveChanges();
                                                                int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIradioAntenna.ToString(), RadioAntenna.Id);
                                                                if (AddRadioAntenna.civilLoads != null && Id != 0)
                                                                {
                                                                    TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                                    {
                                                                        InstallationDate = AddRadioAntenna.civilLoads.InstallationDate,
                                                                        allLoadInstId = Id,
                                                                        legId = AddRadioAntenna.installationConfig?.legId,
                                                                        allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                                        sideArmId = null,
                                                                        ItemOnCivilStatus = AddRadioAntenna.civilLoads.ItemOnCivilStatus,
                                                                        ItemStatus = AddRadioAntenna.civilLoads?.ItemStatus,
                                                                        Dismantle = false,
                                                                        ReservedSpace = AddRadioAntenna.civilLoads.ReservedSpace,
                                                                        SiteCode = SiteCode,


                                                                    };
                                                                    _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                                    _unitOfWork.SaveChanges();

                                                                }

                                                                if (AddRadioAntenna.dynamicAttribute != null ? AddRadioAntenna.dynamicAttribute.Count > 0 : false)
                                                                {
                                                                    foreach (var DynamicAttInstValue in AddRadioAntenna.dynamicAttribute)
                                                                    {
                                                                        _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, RadioAntenna.Id, ConnectionString);
                                                                    }
                                                                }

                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this sidearm is not found ", (int)ApiReturnCode.fail);
                                                    }
                                                }
                                                else
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected sideArm ", (int)ApiReturnCode.fail);
                                                }
                                            }
                                            else
                                            {
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this civil is not found ", (int)ApiReturnCode.fail);
                                            }


                                        }
                                        else
                                        {
                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected civilwithlegs item ", (int)ApiReturnCode.fail);
                                        }

                                    }
                                    if (AddRadioAntenna.installationConfig.civilSteelType == 1)
                                    {
                                        if (AddRadioAntenna.installationConfig.civilWithoutLegId != null)
                                        {
                                            var AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLegId ==
                                               AddRadioAntenna.installationConfig.civilWithoutLegId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                               x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);

                                            if (AllcivilinstId != null)
                                            {
                                                if (AddRadioAntenna.installationConfig.sideArmId != null)

                                                {
                                                    var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLegId ==
                                                   AddRadioAntenna.installationConfig.civilWithoutLegId && !x.Dismantle && x.sideArmId == AddRadioAntenna.installationConfig.sideArmId, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                                   x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);

                                                    if (SideArm != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(RadioAntenna.SerialNumber))
                                                        {
                                                            bool CheckSerialNumber = _dbContext.MV_RADIO_ANTENNA_VIEW.Any(x => x.SerialNumber == RadioAntenna.SerialNumber && !x.Dismantle);
                                                            if (CheckSerialNumber)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {RadioAntenna.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                        }

                                                        if (AddRadioAntenna.civilLoads.ReservedSpace == true)
                                                        {
                                                            var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                            if (Message != "Success")
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                            }
                                                            if (RadioAntenna.CenterHigh <= 0)
                                                            {
                                                                if (RadioAntenna.HBASurface <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaLibrary.Length <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaLibrary.Length / 2;
                                                                }
                                                            }
                                                            else if (RadioAntenna.SpaceInstallation == 0)
                                                            {
                                                                if (RadioAntennaLibrary.SpaceLibrary == 0)
                                                                {
                                                                    if (RadioAntennaLibrary.Length == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else if (RadioAntennaLibrary.Width == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else
                                                                    {
                                                                        RadioAntenna.SpaceInstallation = RadioAntennaLibrary.Length * RadioAntennaLibrary.Width;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaLibrary.SpaceLibrary;
                                                                }
                                                            }

                                                            else if (AddRadioAntenna.installationAttributes.Azimuth <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (AddRadioAntenna.installationAttributes.HeightBase <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                  x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                  x.SIDEARM_ID == AddRadioAntenna.installationConfig.sideArmId
                                                                  && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase);

                                                            if (CheckAzimuthAndHeightBase != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                            var SideArmName1 = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == AddRadioAntenna.installationConfig.sideArmId);
                                                            if (SideArmName1 != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                            {
                                                                RadioAntenna.Name = SideArmName1?.Name + " " + RadioAntenna.Azimuth + " " + RadioAntenna.HeightBase;
                                                            }



                                                            var CheckName = _dbContext.MV_MWDISH_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                                    (x.Id != null ? x.DishName.ToLower() == RadioAntenna.Name.ToLower() : false
                                                                       && x.SiteCode.ToLower() == SiteCode.ToLower()));

                                                            if (CheckName != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                            if (AllcivilinstId.allCivilInst.civilWithoutLeg?.CurrentLoads == null)
                                                            {
                                                                AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads = 0;
                                                            }
                                                            var OldVcivilinfo = _dbContext.TLIcivilWithoutLeg.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithoutLegId);

                                                            if (OldVcivilinfo != null)
                                                            {

                                                                var EquivalentSpace = RadioAntenna.SpaceInstallation * (RadioAntenna.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithoutLeg.HeightBase);

                                                                AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads += EquivalentSpace;
                                                                RadioAntenna.EquivalentSpace = EquivalentSpace;
                                                                _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithoutLeg);

                                                                _unitOfWork.SaveChanges();
                                                            }


                                                            RadioAntenna.radioAntennaLibraryId = AddRadioAntenna.installationConfig.radioAntennaLibraryId;
                                                            RadioAntenna.installationPlaceId = AddRadioAntenna.installationConfig.InstallationPlaceId;
                                                            _unitOfWork.RadioAntennaRepository.AddWithHistory(UserId, RadioAntenna);
                                                            _unitOfWork.SaveChanges();
                                                            int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIradioAntenna.ToString(), RadioAntenna.Id);
                                                            if (AddRadioAntenna.civilLoads != null && Id != 0)
                                                            {
                                                                TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                                {
                                                                    InstallationDate = AddRadioAntenna.civilLoads.InstallationDate,
                                                                    allLoadInstId = Id,
                                                                    legId = AddRadioAntenna.installationConfig?.legId,
                                                                    allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                                    sideArmId = null,
                                                                    ItemOnCivilStatus = AddRadioAntenna.civilLoads.ItemOnCivilStatus,
                                                                    ItemStatus = AddRadioAntenna.civilLoads?.ItemStatus,
                                                                    Dismantle = false,
                                                                    ReservedSpace = AddRadioAntenna.civilLoads.ReservedSpace,
                                                                    SiteCode = SiteCode,


                                                                };
                                                                _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                                _unitOfWork.SaveChanges();

                                                            }

                                                            if (AddRadioAntenna.dynamicAttribute != null ? AddRadioAntenna.dynamicAttribute.Count > 0 : false)
                                                            {
                                                                foreach (var DynamicAttInstValue in AddRadioAntenna.dynamicAttribute)
                                                                {
                                                                    _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, RadioAntenna.Id, ConnectionString);
                                                                }
                                                            }
                                                        }


                                                        else
                                                        {
                                                            var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                            if (Message != "Success")
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                            }
                                                            if (RadioAntenna.CenterHigh <= 0)
                                                            {
                                                                if (RadioAntenna.HBASurface <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaLibrary.Length <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaLibrary.Length / 2;
                                                                }
                                                            }
                                                            else if (RadioAntenna.SpaceInstallation == 0)
                                                            {
                                                                if (RadioAntennaLibrary.SpaceLibrary == 0)
                                                                {
                                                                    if (RadioAntennaLibrary.Length == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else if (RadioAntennaLibrary.Width == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else
                                                                    {
                                                                        RadioAntenna.SpaceInstallation = RadioAntennaLibrary.Length * RadioAntennaLibrary.Width;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaLibrary.SpaceLibrary;
                                                                }
                                                            }

                                                            else if (AddRadioAntenna.installationAttributes.Azimuth <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (AddRadioAntenna.installationAttributes.HeightBase <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                  x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                  x.SIDEARM_ID == AddRadioAntenna.installationConfig.sideArmId
                                                                  && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase);

                                                            if (CheckAzimuthAndHeightBase != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                            var SideArmName1 = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == AddRadioAntenna.installationConfig.sideArmId);
                                                            if (SideArmName1 != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                            {
                                                                RadioAntenna.Name = SideArmName1?.Name + " " + RadioAntenna.Azimuth + " " + RadioAntenna.HeightBase;
                                                            }



                                                            var CheckName = _dbContext.MV_MWDISH_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                                    (x.Id != null ? x.DishName.ToLower() == RadioAntenna.Name.ToLower() : false
                                                                       && x.SiteCode.ToLower() == SiteCode.ToLower()));

                                                            if (CheckName != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                            RadioAntenna.radioAntennaLibraryId = AddRadioAntenna.installationConfig.radioAntennaLibraryId;
                                                            RadioAntenna.installationPlaceId = AddRadioAntenna.installationConfig.InstallationPlaceId;
                                                            _unitOfWork.RadioAntennaRepository.AddWithHistory(UserId, RadioAntenna);
                                                            _unitOfWork.SaveChanges();
                                                            int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIradioAntenna.ToString(), RadioAntenna.Id);
                                                            if (AddRadioAntenna.civilLoads != null && Id != 0)
                                                            {
                                                                TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                                {
                                                                    InstallationDate = AddRadioAntenna.civilLoads.InstallationDate,
                                                                    allLoadInstId = Id,
                                                                    legId = AddRadioAntenna.installationConfig?.legId,
                                                                    allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                                    sideArmId = null,
                                                                    ItemOnCivilStatus = AddRadioAntenna.civilLoads.ItemOnCivilStatus,
                                                                    ItemStatus = AddRadioAntenna.civilLoads?.ItemStatus,
                                                                    Dismantle = false,
                                                                    ReservedSpace = AddRadioAntenna.civilLoads.ReservedSpace,
                                                                    SiteCode = SiteCode,


                                                                };
                                                                _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                                _unitOfWork.SaveChanges();

                                                            }

                                                            if (AddRadioAntenna.dynamicAttribute != null ? AddRadioAntenna.dynamicAttribute.Count > 0 : false)
                                                            {
                                                                foreach (var DynamicAttInstValue in AddRadioAntenna.dynamicAttribute)
                                                                {
                                                                    _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, RadioAntenna.Id, ConnectionString);
                                                                }
                                                            }

                                                        }
                                                    }
                                                    else
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this sidearm is not found ", (int)ApiReturnCode.fail);
                                                    }
                                                }
                                                else
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected sideArm ", (int)ApiReturnCode.fail);
                                                }
                                            }
                                            else
                                            {
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this civil is not found ", (int)ApiReturnCode.fail);
                                            }

                                        }

                                        else
                                        {
                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected civilwithoutlegs item ", (int)ApiReturnCode.fail);
                                        }
                                    }
                                    if (AddRadioAntenna.installationConfig.civilSteelType == 2)
                                    {
                                        if (AddRadioAntenna.installationConfig.civilNonSteelId != null)
                                        {
                                            var AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId ==
                                                AddRadioAntenna.installationConfig.civilNonSteelId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                                x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                            if (AllcivilinstId != null)
                                            {
                                                if (AddRadioAntenna.installationConfig.sideArmId != null)
                                                {
                                                    var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId ==
                                                     AddRadioAntenna.installationConfig.civilNonSteelId && !x.Dismantle && x.sideArmId == AddRadioAntenna.installationConfig.sideArmId, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                                     x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                                    if (SideArm != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(RadioAntenna.SerialNumber))
                                                        {
                                                            bool CheckSerialNumber = _dbContext.MV_RADIO_ANTENNA_VIEW.Any(x => x.SerialNumber == RadioAntenna.SerialNumber && !x.Dismantle);
                                                            if (CheckSerialNumber)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {RadioAntenna.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                        }


                                                        var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                        if (Message != "Success")
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                        }
                                                        if (RadioAntenna.CenterHigh <= 0)
                                                        {
                                                            if (RadioAntenna.HBASurface <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaLibrary.Length / 2;
                                                            }
                                                        }
                                                        else if (RadioAntenna.SpaceInstallation == 0)
                                                        {
                                                            if (RadioAntennaLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (RadioAntennaLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaLibrary.Width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaLibrary.Length * RadioAntennaLibrary.Width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        else if (AddRadioAntenna.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (AddRadioAntenna.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                              x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                              x.SIDEARM_ID == AddRadioAntenna.installationConfig.sideArmId
                                                              && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase);

                                                        if (CheckAzimuthAndHeightBase != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                        var SideArmName1 = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == AddRadioAntenna.installationConfig.sideArmId);
                                                        if (SideArmName1 != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                        {
                                                            RadioAntenna.Name = SideArmName1?.Name + " " + RadioAntenna.Azimuth + " " + RadioAntenna.HeightBase;
                                                        }



                                                        var CheckName = _dbContext.MV_MWDISH_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                                (x.Id != null ? x.DishName.ToLower() == RadioAntenna.Name.ToLower() : false
                                                                   && x.SiteCode.ToLower() == SiteCode.ToLower()));

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        RadioAntenna.radioAntennaLibraryId = AddRadioAntenna.installationConfig.radioAntennaLibraryId;
                                                        RadioAntenna.installationPlaceId = AddRadioAntenna.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.RadioAntennaRepository.AddWithHistory(UserId, RadioAntenna);
                                                        _unitOfWork.SaveChanges();
                                                        int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIradioAntenna.ToString(), RadioAntenna.Id);
                                                        if (AddRadioAntenna.civilLoads != null && Id != 0)
                                                        {
                                                            TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                            {
                                                                InstallationDate = AddRadioAntenna.civilLoads.InstallationDate,
                                                                allLoadInstId = Id,
                                                                legId = AddRadioAntenna.installationConfig?.legId,
                                                                allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                                sideArmId = null,
                                                                ItemOnCivilStatus = AddRadioAntenna.civilLoads.ItemOnCivilStatus,
                                                                ItemStatus = AddRadioAntenna.civilLoads?.ItemStatus,
                                                                Dismantle = false,
                                                                ReservedSpace = AddRadioAntenna.civilLoads.ReservedSpace,
                                                                SiteCode = SiteCode,


                                                            };
                                                            _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                            _unitOfWork.SaveChanges();

                                                        }

                                                        if (AddRadioAntenna.dynamicAttribute != null ? AddRadioAntenna.dynamicAttribute.Count > 0 : false)
                                                        {
                                                            foreach (var DynamicAttInstValue in AddRadioAntenna.dynamicAttribute)
                                                            {
                                                                _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, RadioAntenna.Id, ConnectionString);
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this sidearm is not found ", (int)ApiReturnCode.fail);
                                                    }
                                                }

                                                else
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected sideArm ", (int)ApiReturnCode.fail);
                                                }

                                            }
                                            else
                                            {
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this civil is not found ", (int)ApiReturnCode.fail);
                                            }

                                        }
                                        else
                                        {
                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected civilnonsteel item ", (int)ApiReturnCode.fail);
                                        }


                                    }

                                }
                            }
                            if (TaskId != null)
                            {
                                var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                                var result = Submit.Result;
                                if (result.result == true && result.errorMessage == null)
                                {
                                    _unitOfWork.SaveChanges();
                                    transaction.Complete();
                                }
                                else
                                {
                                    transaction.Dispose();
                                    return new Response<GetForAddMWDishInstallationObject>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                                }
                            }
                            else
                            {
                                _unitOfWork.SaveChanges();
                                transaction.Complete();
                            }
                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(ConnectionString, "MV_RADIO_ANTENNA_VIEW"));
                            return new Response<GetForAddMWDishInstallationObject>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<GetForAddMWDishInstallationObject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                        }
                    }
                }
            }


        }
        //Function 2 parameters
        //map object to ViewModel
        //map ViewModel to Entity
        //update Entity
        //update dynamic attributes
        public async Task<Response<GetForAddMWDishInstallationObject>> EditRadioInstallation(object RadioInstallationViewModel, string TableName, int? TaskId, int UserId,string ConnectionString)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (LoadSubType.TLIradioAntenna.ToString() == TableName)
                    {
                        var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLIradioAntenna.ToString().ToLower()).Id;
                        EditRadioAntennaInstallationObject EditRadioAntenna = _mapper.Map<EditRadioAntennaInstallationObject>(RadioInstallationViewModel);
                        TLIradioAntenna RadioAntenna = _mapper.Map<TLIradioAntenna>(EditRadioAntenna.installationAttributes);
                        TLIcivilLoads RadioAntennaInst = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable().AsNoTracking()
                       .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.radioAntenna.radioAntennaLibrary).Include(x => x.allCivilInst)
                       .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                       .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.radioAntennaId == RadioAntenna.Id && !x.Dismantle);

                        if (RadioAntennaInst == null)
                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "MWDish is not found", (int)ApiReturnCode.fail);
                        if (EditRadioAntenna.installationConfig.InstallationPlaceId == 1)
                        {
                            if (EditRadioAntenna.installationConfig.civilWithLegId != null)
                            {
                                TLIcivilSiteDate AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                 EditRadioAntenna.installationConfig.civilWithLegId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                 x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                if (AllcivilinstId != null)
                                {
                                    if (EditRadioAntenna.installationConfig.legId != null)
                                    {
                                        if (EditRadioAntenna.installationConfig.sideArmId != null)
                                        {

                                            var Leg = _unitOfWork.LegRepository.GetIncludeWhereFirst(x => x.CivilWithLegInstId ==
                                                EditRadioAntenna.installationConfig.civilWithLegId && x.Id == EditRadioAntenna.installationConfig.legId
                                                , x => x.CivilWithLegInst);
                                            if (Leg != null)
                                            {
                                                if (!string.IsNullOrEmpty(RadioAntenna.SerialNumber))
                                                {
                                                    bool CheckSerialNumber = _dbContext.MV_RADIO_ANTENNA_VIEW.Any(x => x.SerialNumber == RadioAntenna.SerialNumber && !x.Dismantle && x.Id != RadioAntenna.Id);
                                                    if (CheckSerialNumber)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {RadioAntenna.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                }

                                                if (RadioAntennaInst.ReservedSpace == true && EditRadioAntenna.civilLoads.ReservedSpace==true)
                                                {
                                                    if (RadioAntenna.CenterHigh <= 0)
                                                    {
                                                        if (RadioAntenna.HBASurface <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                        }
                                                    }
                                                    else if (RadioAntenna.SpaceInstallation == 0)
                                                    {
                                                        if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                        {
                                                            if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                        }
                                                    }

                                                    else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                    }

                                                    var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                            x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                            x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                            && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                    if (CheckAzimuthAndHeightBase != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                    TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                    if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                    {
                                                        RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                    }

                                                    var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                    (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                       && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                    if (CheckName != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                    if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                    {
                                                        AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                    }
                                                    var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                    if (OldVcivilinfo != null)
                                                    {
                                                        var EquivalentSpace = RadioAntenna.SpaceInstallation * (RadioAntenna.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);
                                                        AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                        RadioAntenna.EquivalentSpace = EquivalentSpace;
                                                        _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                        _unitOfWork.SaveChanges();
                                                    }
                                                    RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                    RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                    _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                    _unitOfWork.SaveChanges();
                                                    if (EditRadioAntenna.civilLoads != null)
                                                    {

                                                        var existingEntity = _unitOfWork.CivilLoadsRepository
                                                            .GetAllAsQueryable()
                                                            .AsNoTracking()
                                                            .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                        RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                        RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                        RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                        RadioAntennaInst.sideArm2Id = null;
                                                        RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                        RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                        RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                        RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                        _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                        _unitOfWork.SaveChanges();

                                                    }

                                                    if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);

                                                }
                                                if (RadioAntennaInst.ReservedSpace == true && EditRadioAntenna.civilLoads.ReservedSpace == false)
                                                {
                                                    if (RadioAntenna.CenterHigh <= 0)
                                                    {
                                                        if (RadioAntenna.HBASurface <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                        }
                                                    }
                                                    else if (RadioAntenna.SpaceInstallation == 0)
                                                    {
                                                        if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                        {
                                                            if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                        }
                                                    }

                                                    else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                    }

                                                    var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                            x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                            x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                            && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                    if (CheckAzimuthAndHeightBase != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                    TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                    if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                    {
                                                        RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                    }

                                                    var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                    (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                       && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                    if (CheckName != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                    var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);
                                                    if (OldVcivilinfo != null)
                                                    {
                                                        AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads - RadioAntennaInst.allLoadInst.radioAntenna.EquivalentSpace;
                                                        _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);
                                                        _unitOfWork.SaveChanges();
                                                    }
                                                    RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                    RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                    _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                    _unitOfWork.SaveChanges();
                                                    if (EditRadioAntenna.civilLoads != null)
                                                    {

                                                        var existingEntity = _unitOfWork.CivilLoadsRepository
                                                            .GetAllAsQueryable()
                                                            .AsNoTracking()
                                                            .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);
                                                        RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                        RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                        RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                        RadioAntennaInst.sideArm2Id = null;
                                                        RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                        RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                        RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                        RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                        _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                        _unitOfWork.SaveChanges();


                                                    }

                                                    if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);
                                                }
                                                if (RadioAntennaInst.ReservedSpace == false && EditRadioAntenna.civilLoads.ReservedSpace == true)
                                                {
                                                    if (RadioAntenna.CenterHigh <= 0)
                                                    {
                                                        if (RadioAntenna.HBASurface <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                        }
                                                    }
                                                    else if (RadioAntenna.SpaceInstallation == 0)
                                                    {
                                                        if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                        {
                                                            if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                        }
                                                    }

                                                    else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                    }

                                                    var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                            x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                            x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                            && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                    if (CheckAzimuthAndHeightBase != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                    TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                    if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                    {
                                                        RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                    }

                                                    var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                    (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                       && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                    if (CheckName != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                    if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                    {
                                                        AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                    }
                                                    var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                    if (OldVcivilinfo != null)
                                                    {
                                                        var EquivalentSpace = RadioAntenna.SpaceInstallation * (RadioAntenna.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);
                                                        AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                        RadioAntenna.EquivalentSpace = EquivalentSpace;
                                                        _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                        _unitOfWork.SaveChanges();
                                                    }

                                                    RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                    RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                    _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                    _unitOfWork.SaveChanges();
                                                    if (EditRadioAntenna.civilLoads != null)
                                                    {

                                                        var existingEntity = _unitOfWork.CivilLoadsRepository
                                                            .GetAllAsQueryable()
                                                            .AsNoTracking()
                                                            .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                        RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                        RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                        RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                        RadioAntennaInst.sideArm2Id = null;
                                                        RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                        RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                        RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                        RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                        _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                        _unitOfWork.SaveChanges();


                                                    }

                                                    if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);
                                                }
                                                if (RadioAntennaInst.ReservedSpace == false && EditRadioAntenna.civilLoads.ReservedSpace == false)
                                                {
                                                    if (RadioAntenna.CenterHigh <= 0)
                                                    {
                                                        if (RadioAntenna.HBASurface <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                        }
                                                    }
                                                    else if (RadioAntenna.SpaceInstallation == 0)
                                                    {
                                                        if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                        {
                                                            if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                        }
                                                    }

                                                    else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                    }

                                                    var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                            x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                            x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                            && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                    if (CheckAzimuthAndHeightBase != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                    TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                    if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                    {
                                                        RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                    }

                                                    var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                    (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                       && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                    if (CheckName != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                    RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                    RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                    _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                    _unitOfWork.SaveChanges();
                                                    if (EditRadioAntenna.civilLoads != null)
                                                    {

                                                        var existingEntity = _unitOfWork.CivilLoadsRepository
                                                            .GetAllAsQueryable()
                                                            .AsNoTracking()
                                                            .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                        RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                        RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                        RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                        RadioAntennaInst.sideArm2Id = null;
                                                        RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                        RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                        RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                        RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                        _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                        _unitOfWork.SaveChanges();


                                                    }

                                                    if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);
                                                }
                                            }
                                            else
                                            {
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this leg is not found", (int)ApiReturnCode.fail);
                                            }
                                        }
                                        else
                                        {
                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not selected sidearm because installation place is leg ", (int)ApiReturnCode.fail);
                                        }
                                    }

                                    else
                                    {
                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected leg ", (int)ApiReturnCode.fail);
                                    }


                                }

                                else
                                {
                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this civil is not found ", (int)ApiReturnCode.fail);
                                }

                            }
                            else
                            {
                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected civilwithlegs item ", (int)ApiReturnCode.fail);
                            }


                        }
                        else if (EditRadioAntenna.installationConfig.InstallationPlaceId == 2)
                        {
                            if (EditRadioAntenna.installationConfig.civilSteelType == 0)
                            {
                                if (EditRadioAntenna.installationConfig.civilWithLegId != null)
                                {
                                    var AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                       EditRadioAntenna.installationConfig.civilWithLegId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                       x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                    if (AllcivilinstId != null)
                                    {
                                        if (EditRadioAntenna.installationConfig.sideArmId != null)
                                        {
                                            if (EditRadioAntenna.installationConfig.legId != null)
                                            {
                                                var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                              EditRadioAntenna.installationConfig.civilWithLegId && !x.Dismantle && x.sideArmId == EditRadioAntenna.installationConfig.sideArmId, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                              x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                                if (SideArm != null)
                                                {
                                                    if (!string.IsNullOrEmpty(RadioAntenna.SerialNumber))
                                                    {
                                                        bool CheckSerialNumber = _dbContext.MV_RADIO_ANTENNA_VIEW.Any(x => x.SerialNumber == RadioAntenna.SerialNumber && !x.Dismantle && x.Id != x.Id);
                                                        if (CheckSerialNumber)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {RadioAntenna.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                    }

                                                    if (AllcivilinstId != null)
                                                    {
                                                        if (RadioAntennaInst.ReservedSpace == true && EditRadioAntenna.civilLoads.ReservedSpace == true)
                                                        {
                                                            if (RadioAntenna.CenterHigh <= 0)
                                                            {
                                                                if (RadioAntenna.HBASurface <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                                }
                                                            }
                                                            else if (RadioAntenna.SpaceInstallation == 0)
                                                            {
                                                                if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                                {
                                                                    if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else
                                                                    {
                                                                        RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                                }
                                                            }

                                                            else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                            }

                                                            var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                    x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                    x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                                    && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                            if (CheckAzimuthAndHeightBase != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                            TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                            if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                            {
                                                                RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                            }

                                                            var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                            (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                               && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                            if (CheckName != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                            if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                            {
                                                                AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                            }
                                                            var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                            if (OldVcivilinfo != null)
                                                            {
                                                                var EquivalentSpace = RadioAntenna.SpaceInstallation * (RadioAntenna.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);
                                                                AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                                RadioAntenna.EquivalentSpace = EquivalentSpace;
                                                                _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                                _unitOfWork.SaveChanges();
                                                            }
                                                            RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                            RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                            _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                            _unitOfWork.SaveChanges();
                                                            if (EditRadioAntenna.civilLoads != null)
                                                            {

                                                                var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                    .GetAllAsQueryable()
                                                                    .AsNoTracking()
                                                                    .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                                RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                                RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                                RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                                RadioAntennaInst.sideArm2Id = null;
                                                                RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                                RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                                RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                                RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                                _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                                _unitOfWork.SaveChanges();


                                                            }
                                                            if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                                _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);
                                                        }
                                                        if (RadioAntennaInst.ReservedSpace == true && EditRadioAntenna.civilLoads.ReservedSpace == false)
                                                        {
                                                            if (RadioAntenna.CenterHigh <= 0)
                                                            {
                                                                if (RadioAntenna.HBASurface <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                                }
                                                            }
                                                            else if (RadioAntenna.SpaceInstallation == 0)
                                                            {
                                                                if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                                {
                                                                    if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else
                                                                    {
                                                                        RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                                }
                                                            }

                                                            else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                            }

                                                            var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                    x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                    x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                                    && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                            if (CheckAzimuthAndHeightBase != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                            TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                            if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                            {
                                                                RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                            }

                                                            var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                            (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                               && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                            if (CheckName != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                            var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);
                                                            if (OldVcivilinfo != null)
                                                            {
                                                                AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads - RadioAntennaInst.allLoadInst.radioAntenna.EquivalentSpace;
                                                                _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);
                                                                _unitOfWork.SaveChanges();
                                                            }
                                                            RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                            RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                            _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                            _unitOfWork.SaveChanges();
                                                            if (EditRadioAntenna.civilLoads != null)
                                                            {

                                                                var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                    .GetAllAsQueryable()
                                                                    .AsNoTracking()
                                                                    .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                                RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                                RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                                RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                                RadioAntennaInst.sideArm2Id = null;
                                                                RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                                RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                                RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                                RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                                _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                                _unitOfWork.SaveChanges();


                                                            }

                                                            if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                                _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);
                                                        }
                                                        if (RadioAntennaInst.ReservedSpace == false && EditRadioAntenna.civilLoads.ReservedSpace == true)
                                                        {
                                                            if (RadioAntenna.CenterHigh <= 0)
                                                            {
                                                                if (RadioAntenna.HBASurface <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                                }
                                                            }
                                                            else if (RadioAntenna.SpaceInstallation == 0)
                                                            {
                                                                if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                                {
                                                                    if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else
                                                                    {
                                                                        RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                                }
                                                            }

                                                            else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                            }

                                                            var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                    x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                    x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                                    && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                            if (CheckAzimuthAndHeightBase != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                            TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                            if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                            {
                                                                RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                            }

                                                            var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                            (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                               && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                            if (CheckName != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                            if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                            {
                                                                AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                            }
                                                            var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                            if (OldVcivilinfo != null)
                                                            {
                                                                var EquivalentSpace = RadioAntenna.SpaceInstallation * (RadioAntenna.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);
                                                                AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                                RadioAntenna.EquivalentSpace = EquivalentSpace;
                                                                _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                                _unitOfWork.SaveChanges();
                                                            }

                                                            RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                            RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                            _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                            _unitOfWork.SaveChanges();
                                                            if (EditRadioAntenna.civilLoads != null)
                                                            {

                                                                var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                    .GetAllAsQueryable()
                                                                    .AsNoTracking()
                                                                    .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                                RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                                RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                                RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                                RadioAntennaInst.sideArm2Id = null;
                                                                RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                                RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                                RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                                RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                                _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                                _unitOfWork.SaveChanges();


                                                            }

                                                            if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                                _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);
                                                        }
                                                        if (RadioAntennaInst.ReservedSpace == false && EditRadioAntenna.civilLoads.ReservedSpace == false)
                                                        {
                                                            if (RadioAntenna.CenterHigh <= 0)
                                                            {
                                                                if (RadioAntenna.HBASurface <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                                }
                                                            }
                                                            else if (RadioAntenna.SpaceInstallation == 0)
                                                            {
                                                                if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                                {
                                                                    if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else
                                                                    {
                                                                        RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                                }
                                                            }

                                                            else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                            }

                                                            var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                    x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                    x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                                    && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                            if (CheckAzimuthAndHeightBase != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                            TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                            if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                            {
                                                                RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                            }

                                                            var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                            (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                               && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                            if (CheckName != null)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                            RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                            RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                            _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                            _unitOfWork.SaveChanges();
                                                            if (EditRadioAntenna.civilLoads != null)
                                                            {

                                                                var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                    .GetAllAsQueryable()
                                                                    .AsNoTracking()
                                                                    .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                                RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                                RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                                RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                                RadioAntennaInst.sideArm2Id = null;
                                                                RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                                RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                                RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                                RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                                _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                                _unitOfWork.SaveChanges();


                                                            }

                                                            if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                                _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);
                                                        }

                                                    }
                                                    else
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this sidearm is not found ", (int)ApiReturnCode.fail);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not selected leg because installation place is sidearm ", (int)ApiReturnCode.fail);
                                            }
                                        }
                                        else
                                        {
                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected sideArm ", (int)ApiReturnCode.fail);
                                        }



                                    }
                                    else
                                    {
                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this civil is not found ", (int)ApiReturnCode.fail);
                                    }


                                }

                                else
                                {
                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected civilwithlegs item ", (int)ApiReturnCode.fail);
                                }
                            }
                            else if (EditRadioAntenna.installationConfig.civilSteelType == 1)
                            {
                                if (EditRadioAntenna.installationConfig.civilWithoutLegId != null)
                                {
                                    var AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLegId ==
                                       EditRadioAntenna.installationConfig.civilWithoutLegId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                       x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);

                                    if (AllcivilinstId != null)
                                    {
                                        if (EditRadioAntenna.installationConfig.sideArmId != null)
                                        {
                                            if (EditRadioAntenna.installationConfig.legId != null)
                                            {
                                                var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLegId ==
                                              EditRadioAntenna.installationConfig.civilWithoutLegId && !x.Dismantle && x.sideArmId == EditRadioAntenna.installationConfig.sideArmId, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                              x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                                if (SideArm != null)
                                                {
                                                    if (!string.IsNullOrEmpty(RadioAntenna.SerialNumber))
                                                    {
                                                        bool CheckSerialNumber = _dbContext.MV_RADIO_ANTENNA_VIEW.Any(x => x.SerialNumber == RadioAntenna.SerialNumber && !x.Dismantle && x.Id != x.Id);
                                                        if (CheckSerialNumber)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {RadioAntenna.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                    }


                                                    if (RadioAntennaInst.ReservedSpace == true && EditRadioAntenna.civilLoads.ReservedSpace == true)
                                                    {
                                                        if (RadioAntenna.CenterHigh <= 0)
                                                        {
                                                            if (RadioAntenna.HBASurface <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                            }
                                                        }
                                                        else if (RadioAntenna.SpaceInstallation == 0)
                                                        {
                                                            if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }

                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                                && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                        if (CheckAzimuthAndHeightBase != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                        TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                        if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                        {
                                                            RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                        }

                                                        var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                        (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                           && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        if (AllcivilinstId.allCivilInst.civilWithoutLeg?.CurrentLoads == null)
                                                        {
                                                            AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads = 0;
                                                        }
                                                        var OldVcivilinfo = _dbContext.TLIcivilWithoutLeg.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithoutLegId);

                                                        if (OldVcivilinfo != null)
                                                        {
                                                            var EquivalentSpace = RadioAntenna.SpaceInstallation * (RadioAntenna.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithoutLeg.HeightBase);
                                                            AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads += EquivalentSpace;
                                                            RadioAntenna.EquivalentSpace = EquivalentSpace;
                                                            _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithoutLeg);

                                                            _unitOfWork.SaveChanges();
                                                        }
                                                        RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                        RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                        _unitOfWork.SaveChanges();
                                                        if (EditRadioAntenna.civilLoads != null)
                                                        {

                                                            var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                .GetAllAsQueryable()
                                                                .AsNoTracking()
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                            RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                            RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                            RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                            RadioAntennaInst.sideArm2Id = null;
                                                            RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                            RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                            RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                            RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                            _unitOfWork.SaveChanges();


                                                        }

                                                        if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);
                                                    }
                                                    if (RadioAntennaInst.ReservedSpace == true && EditRadioAntenna.civilLoads.ReservedSpace == false)
                                                    {
                                                        if (RadioAntenna.CenterHigh <= 0)
                                                        {
                                                            if (RadioAntenna.HBASurface <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                            }
                                                        }
                                                        else if (RadioAntenna.SpaceInstallation == 0)
                                                        {
                                                            if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }

                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                                && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                        if (CheckAzimuthAndHeightBase != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                        TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                        if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                        {
                                                            RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                        }

                                                        var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                        (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                           && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        var OldVcivilinfo = _dbContext.TLIcivilWithoutLeg.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithoutLegId);
                                                        if (OldVcivilinfo != null)
                                                        {
                                                            AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads = AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads - RadioAntennaInst.allLoadInst.radioAntenna.EquivalentSpace;
                                                            _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithoutLeg);
                                                            _unitOfWork.SaveChanges();
                                                        }
                                                        RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                        RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                        _unitOfWork.SaveChanges();
                                                        if (EditRadioAntenna.civilLoads != null)
                                                        {

                                                            var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                .GetAllAsQueryable()
                                                                .AsNoTracking()
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                            RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                            RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                            RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                            RadioAntennaInst.sideArm2Id = null;
                                                            RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                            RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                            RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                            RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                            _unitOfWork.SaveChanges();


                                                        }

                                                        if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);
                                                    }
                                                    if (RadioAntennaInst.ReservedSpace == false && EditRadioAntenna.civilLoads.ReservedSpace == true)
                                                    {
                                                        if (RadioAntenna.CenterHigh <= 0)
                                                        {
                                                            if (RadioAntenna.HBASurface <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                            }
                                                        }
                                                        else if (RadioAntenna.SpaceInstallation == 0)
                                                        {
                                                            if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }

                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                                && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                        if (CheckAzimuthAndHeightBase != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                        TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                        if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                        {
                                                            RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                        }

                                                        var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                        (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                           && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        if (AllcivilinstId.allCivilInst.civilWithoutLeg?.CurrentLoads == null)
                                                        {
                                                            AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads = 0;
                                                        }
                                                        var OldVcivilinfo = _dbContext.TLIcivilWithoutLeg.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithoutLegId);

                                                        if (OldVcivilinfo != null)
                                                        {
                                                            var EquivalentSpace = RadioAntenna.SpaceInstallation * (RadioAntenna.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithoutLeg.HeightBase);
                                                            AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads += EquivalentSpace;
                                                            RadioAntenna.EquivalentSpace = EquivalentSpace;
                                                            _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithoutLeg);

                                                            _unitOfWork.SaveChanges();
                                                        }

                                                        RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                        RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                        _unitOfWork.SaveChanges();
                                                        if (EditRadioAntenna.civilLoads != null)
                                                        {

                                                            var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                .GetAllAsQueryable()
                                                                .AsNoTracking()
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                            RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                            RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                            RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                            RadioAntennaInst.sideArm2Id = null;
                                                            RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                            RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                            RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                            RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                            _unitOfWork.SaveChanges();


                                                        }

                                                        if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);
                                                    }
                                                    if (RadioAntennaInst.ReservedSpace == false && EditRadioAntenna.civilLoads.ReservedSpace == false)
                                                    {
                                                        if (RadioAntenna.CenterHigh <= 0)
                                                        {
                                                            if (RadioAntenna.HBASurface <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                            }
                                                        }
                                                        else if (RadioAntenna.SpaceInstallation == 0)
                                                        {
                                                            if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }

                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                                x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                x.LEGID == EditRadioAntenna.installationConfig.legId && x.Id != RadioAntenna.Id
                                                                && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                        if (CheckAzimuthAndHeightBase != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                        TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == EditRadioAntenna.installationConfig.legId);
                                                        if (legname != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                        {
                                                            RadioAntenna.Name = legname?.CiviLegName + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;

                                                        }

                                                        var CheckName = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => !x.Dismantle && x.Id != RadioAntenna.Id &&
                                                        (x.Id != null ? x.Name.ToLower() == RadioAntenna.Name.ToLower() : false
                                                           && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                        RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                        _unitOfWork.SaveChanges();
                                                        if (EditRadioAntenna.civilLoads != null)
                                                        {

                                                            var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                .GetAllAsQueryable()
                                                                .AsNoTracking()
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                            RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                            RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                            RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                            RadioAntennaInst.sideArm2Id = null;
                                                            RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                            RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                            RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                            RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                            _unitOfWork.SaveChanges();


                                                        }

                                                        if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);
                                                    }


                                                }
                                                else
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this sidearm is not found ", (int)ApiReturnCode.fail);
                                                }

                                            }
                                            else
                                            {
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not selected leg because installation place is sidearm ", (int)ApiReturnCode.fail);
                                            }
                                        }
                                        else
                                        {
                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected sideArm ", (int)ApiReturnCode.fail);
                                        }
                                    }
                                    else
                                    {
                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this civil is not found ", (int)ApiReturnCode.fail);
                                    }
                                }

                                else
                                {
                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected civilwithoutlegs item ", (int)ApiReturnCode.fail);
                                }
                            }
                            else if (EditRadioAntenna.installationConfig.civilSteelType == 2)
                            {
                                if (EditRadioAntenna.installationConfig.civilNonSteelId != null)
                                {
                                    var AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId ==
                                        EditRadioAntenna.installationConfig.civilNonSteelId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                        x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                    if (AllcivilinstId != null)
                                    {
                                        if (EditRadioAntenna.installationConfig.sideArmId != null)
                                        {
                                            if (EditRadioAntenna.installationConfig.legId != null)
                                            {
                                                var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId ==
                                              EditRadioAntenna.installationConfig.civilNonSteelId && !x.Dismantle && x.sideArmId == EditRadioAntenna.installationConfig.sideArmId, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                              x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                                if (SideArm != null)
                                                {
                                                    if (!string.IsNullOrEmpty(RadioAntenna.SerialNumber))
                                                    {
                                                        bool CheckSerialNumber = _dbContext.MV_RADIO_ANTENNA_VIEW.Any(x => x.SerialNumber == RadioAntenna.SerialNumber && !x.Dismantle && x.Id != x.Id);
                                                        if (CheckSerialNumber)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {RadioAntenna.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                    }


                                                    if (EditRadioAntenna.civilLoads.ReservedSpace == true)
                                                    {
                                                        var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                        if (Message != "Success")
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                        }
                                                        if (RadioAntenna.CenterHigh <= 0)
                                                        {
                                                            if (RadioAntenna.HBASurface <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBASurface_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.CenterHigh = RadioAntenna.HBASurface + RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length / 2;
                                                            }
                                                        }
                                                        else if (RadioAntenna.SpaceInstallation == 0)
                                                        {
                                                            if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else if (RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Length * RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.Width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                RadioAntenna.SpaceInstallation = RadioAntennaInst.allLoadInst.radioAntenna.radioAntennaLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        else if (EditRadioAntenna.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else if (EditRadioAntenna.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(
                                                              x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id && x.Id != RadioAntenna.Id &&
                                                              x.SIDEARM_ID == EditRadioAntenna.installationConfig.sideArmId
                                                              && x.Azimuth == RadioAntenna.Azimuth && x.HeightBase == RadioAntenna.HeightBase && !x.Dismantle);

                                                        if (CheckAzimuthAndHeightBase != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                        var SideArmName = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == EditRadioAntenna.installationConfig.sideArmId);
                                                        if (SideArmName != null && RadioAntenna.Azimuth > 0 && RadioAntenna.HeightBase > 0)
                                                        {
                                                            RadioAntenna.Name = SideArmName?.Name + " " + RadioAntenna.HeightBase + " " + RadioAntenna.Azimuth;
                                                        }



                                                        var CheckName = _dbContext.MV_MWDISH_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                                (x.Id != null ? x.DishName.ToLower() == RadioAntenna.Name.ToLower() : false
                                                                   && x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower()));

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {RadioAntenna.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        RadioAntenna.radioAntennaLibraryId = EditRadioAntenna.civilType.radioAntennaLibraryId;
                                                        RadioAntenna.installationPlaceId = EditRadioAntenna.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.RadioAntennaRepository.UpdateWithHistory(UserId, RadioAntennaInst.allLoadInst.radioAntenna, RadioAntenna);
                                                        _unitOfWork.SaveChanges();
                                                        if (EditRadioAntenna.civilLoads != null)
                                                        {

                                                            var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                .GetAllAsQueryable()
                                                                .AsNoTracking()
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.mwDishId == RadioAntenna.Id && !x.Dismantle);

                                                            RadioAntennaInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                            RadioAntennaInst.InstallationDate = EditRadioAntenna.civilLoads.InstallationDate;
                                                            RadioAntennaInst.sideArmId = EditRadioAntenna.installationConfig?.sideArmId ?? null;
                                                            RadioAntennaInst.sideArm2Id = null;
                                                            RadioAntennaInst.legId = EditRadioAntenna.installationConfig?.legId ?? null;
                                                            RadioAntennaInst.ItemOnCivilStatus = EditRadioAntenna.civilLoads.ItemOnCivilStatus;
                                                            RadioAntennaInst.ItemStatus = EditRadioAntenna.civilLoads?.ItemStatus;
                                                            RadioAntennaInst.ReservedSpace = EditRadioAntenna.civilLoads.ReservedSpace;
                                                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, RadioAntennaInst);
                                                            _unitOfWork.SaveChanges();


                                                        }

                                                        if (EditRadioAntenna.dynamicAttribute != null ? EditRadioAntenna.dynamicAttribute.Count() > 0 : false)
                                                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, EditRadioAntenna.dynamicAttribute, TableNameId, RadioAntenna.Id, ConnectionString);

                                                    }


                                                }
                                                else
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this sidearm is not found ", (int)ApiReturnCode.fail);
                                                }

                                            }
                                            else
                                            {
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not selected leg because installation place is sidearm ", (int)ApiReturnCode.fail);
                                            }
                                        }
                                        else
                                        {
                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected sideArm ", (int)ApiReturnCode.fail);
                                        }

                                    }
                                    else
                                    {
                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this civil is not found ", (int)ApiReturnCode.fail);
                                    }

                                }
                                else
                                {
                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected civilnonsteel item ", (int)ApiReturnCode.fail);
                                }


                            }

                        }
                    }                    
                    if (TaskId != null)
                    {
                        var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                        var result = Submit.Result;
                        if (result.result == true && result.errorMessage == null)
                        {
                            _unitOfWork.SaveChanges();
                            transaction.Complete();
                        }
                        else
                        {
                            transaction.Dispose();
                            return new Response<GetForAddMWDishInstallationObject>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        _unitOfWork.SaveChanges();
                        transaction.Complete();
                    }
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(ConnectionString, "MV_RADIO_ANTENNA_VIEW"));
                    return new Response<GetForAddMWDishInstallationObject>();
                }
                catch (Exception err)
                {
                    return new Response<GetForAddMWDishInstallationObject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                }
            }
        }
        #region Helper Methods For UpdateSideArm Function..
        public string CheckDependencyValidationEditVersion(object Input, string SiteCode, string RadioType)
        {
            if (RadioType.ToLower() == TablesNames.TLIradioAntenna.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIradioAntenna.ToString();
                EditRadioAntennaViewModel EditInstallationViewModel = _mapper.Map<EditRadioAntennaViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = EditInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    BaseInstAttView DynamicObject = EditInstallationViewModel.DynamicInstAttsValue
                                        .FirstOrDefault(x => x.Id == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    string DynamicAttributeDataType = _unitOfWork.DataTypeRepository.GetByID(DynamicObject.DataTypeId.Value).Name;

                                    if (DynamicAttributeDataType.ToLower() == "boolean".ToLower())
                                        InsertedValue = bool.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "string".ToLower())
                                        InsertedValue = DynamicObject.Value.ToString();

                                    else if (DynamicAttributeDataType.ToLower() == "int".ToLower() || DynamicAttributeDataType.ToLower() == "double".ToLower())
                                        InsertedValue = double.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "datetime".ToLower())
                                        InsertedValue = DateTime.Parse(DynamicObject.Value.ToString());
                                }

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    MainTableName + SDTableName + "Goal");

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = EditInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(EditInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (EditInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(EditInstallationViewModel, null) != null)
                                        CheckId = (int)EditInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(EditInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            BaseInstAttView InputDynamicAttribute = EditInstallationViewModel.DynamicInstAttsValue
                                .FirstOrDefault(x => x.Id == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                object DependencyValidationValue = new object();
                                object InputDynamicValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;
                                    InputDynamicValue = bool.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;
                                    InputDynamicValue = DateTime.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;
                                    InputDynamicValue = double.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;
                                    InputDynamicValue = InputDynamicAttribute.Value.ToString();
                                }

                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            else if (RadioType.ToLower() == TablesNames.TLIradioRRU.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIradioRRU.ToString();
                EditRadioRRUViewModel EditInstallationViewModel = _mapper.Map<EditRadioRRUViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = EditInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    BaseInstAttView DynamicObject = EditInstallationViewModel.DynamicInstAttsValue
                                        .FirstOrDefault(x => x.Id == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    string DynamicAttributeDataType = _unitOfWork.DataTypeRepository.GetByID(DynamicObject.DataTypeId.Value).Name;

                                    if (DynamicAttributeDataType.ToLower() == "boolean".ToLower())
                                        InsertedValue = bool.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "string".ToLower())
                                        InsertedValue = DynamicObject.Value.ToString();

                                    else if (DynamicAttributeDataType.ToLower() == "int".ToLower() || DynamicAttributeDataType.ToLower() == "double".ToLower())
                                        InsertedValue = double.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "datetime".ToLower())
                                        InsertedValue = DateTime.Parse(DynamicObject.Value.ToString());
                                }

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    (MainTableName + SDTableName + "Goal").ToLower());

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = EditInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(EditInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (EditInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(EditInstallationViewModel, null) != null)
                                        CheckId = (int)EditInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(EditInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            BaseInstAttView InputDynamicAttribute = EditInstallationViewModel.DynamicInstAttsValue
                                .FirstOrDefault(x => x.Id == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                object DependencyValidationValue = new object();
                                object InputDynamicValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;
                                    InputDynamicValue = bool.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;
                                    InputDynamicValue = DateTime.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;
                                    InputDynamicValue = double.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;
                                    InputDynamicValue = InputDynamicAttribute.Value.ToString();
                                }

                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            else if (RadioType.ToLower() == TablesNames.TLIradioOther.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIradioOther.ToString();
                EditRadioOtherViewModel EditInstallationViewModel = _mapper.Map<EditRadioOtherViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = EditInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    BaseInstAttView DynamicObject = EditInstallationViewModel.DynamicInstAttsValue
                                        .FirstOrDefault(x => x.Id == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    string DynamicAttributeDataType = _unitOfWork.DataTypeRepository.GetByID(DynamicObject.DataTypeId.Value).Name;

                                    if (DynamicAttributeDataType.ToLower() == "boolean".ToLower())
                                        InsertedValue = bool.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "string".ToLower())
                                        InsertedValue = DynamicObject.Value.ToString();

                                    else if (DynamicAttributeDataType.ToLower() == "int".ToLower() || DynamicAttributeDataType.ToLower() == "double".ToLower())
                                        InsertedValue = double.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "datetime".ToLower())
                                        InsertedValue = DateTime.Parse(DynamicObject.Value.ToString());
                                }

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    MainTableName + SDTableName + "Goal");

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = EditInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(EditInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (EditInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(EditInstallationViewModel, null) != null)
                                        CheckId = (int)EditInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(EditInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            BaseInstAttView InputDynamicAttribute = EditInstallationViewModel.DynamicInstAttsValue
                                .FirstOrDefault(x => x.Id == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                object DependencyValidationValue = new object();
                                object InputDynamicValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;
                                    InputDynamicValue = bool.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;
                                    InputDynamicValue = DateTime.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;
                                    InputDynamicValue = double.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;
                                    InputDynamicValue = InputDynamicAttribute.Value.ToString();
                                }

                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
        public string CheckGeneralValidationFunctionEditVersion(List<BaseInstAttView> TLIdynamicAttInstValue, string TableName)
        {
            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable, x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttributeEntity in DynamicAttributes)
            {
                TLIvalidation Validation = _unitOfWork.ValidationRepository
                    .GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttributeEntity.Id, x => x.Operation, x => x.DynamicAtt);

                if (Validation != null)
                {
                    BaseInstAttView DynmaicAttributeValue = TLIdynamicAttInstValue.FirstOrDefault(x => x.Id == DynamicAttributeEntity.Id);

                    if (DynmaicAttributeValue == null)
                        return $"({Validation.DynamicAtt.Key}) value can't be null and must be inserted";

                    string OperationName = Validation.Operation.Name;

                    object InputDynamicValue = new object();
                    object ValidationValue = new object();

                    if (Validation.ValueBoolean != null)
                    {
                        ValidationValue = Validation.ValueBoolean;
                        InputDynamicValue = bool.Parse(DynmaicAttributeValue.Value.ToString());
                    }
                    else if (Validation.ValueDateTime != null)
                    {
                        ValidationValue = Validation.ValueDateTime;
                        InputDynamicValue = DateTime.Parse(DynmaicAttributeValue.Value.ToString());
                    }
                    else if (Validation.ValueDouble != null)
                    {
                        ValidationValue = Validation.ValueDouble;
                        InputDynamicValue = double.Parse(DynmaicAttributeValue.Value.ToString());
                    }
                    else if (!string.IsNullOrEmpty(Validation.ValueString))
                    {
                        ValidationValue = Validation.ValueString;
                        if (DynmaicAttributeValue.Value == null)
                            ValidationValue = "";
                        else
                            InputDynamicValue = DynmaicAttributeValue.Value.ToString();
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
        //update dynamicAttInstValueViews list
        //loop to update each one of the list
        private async Task UpdateDynamicInstAttsValue(List<DynamicAttInstValueViewModel> dynamicAttInstValueViews)
        {
            foreach (var atts in dynamicAttInstValueViews)
            {
                await _unitOfWork.DynamicAttInstValueRepository.UpdateItem(atts);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        //Function take 1 parameter
        //get table name Entity by table name
        //specifty the table i deal with
        //get activated attributes
        //get dynamic attributes
        //get related tables
        public Response<ObjectInstAtts> GetAttForAdd(string TableName, int LibId, string SiteCode)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l =>
                    l.TableName == TableName);

                ObjectInstAtts objectInst = new ObjectInstAtts();

                List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                    GetInstAttributeActivated(TableName, null, "installationPlaceId", /*"EquivalentSpace",*/ "radioAntennaLibraryId", "radioRRULibraryId",
                        "radioOtherLibraryId").ToList();

                BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                if (NameAttribute != null)
                {
                    BaseInstAttView Swap = ListAttributesActivated[0];
                    ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                    ListAttributesActivated[0] = NameAttribute;
                }
                if (LoadSubType.TLIradioAntenna.ToString().ToLower() == TableName.ToLower())
                {
                    ListAttributesActivated.Remove(NameAttribute);
                }

                if (LoadSubType.TLIradioAntenna.ToString().ToLower() == TableName.ToLower())
                {
                    RadioAntennaLibraryViewModel RadioAntennaLibrary = _mapper.Map<RadioAntennaLibraryViewModel>(_unitOfWork.RadioAntennaLibraryRepository.GetByID(LibId));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIradioAntennaLibrary.ToString(), RadioAntennaLibrary, null).ToList();

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.Radio.ToString(), TablesNames.TLIradioAntennaLibrary.ToString(), RadioAntennaLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Key.ToLower() == "OwnerId".ToLower())
                            FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                    }
                }
                else if (LoadSubType.TLIradioRRU.ToString().ToLower() == TableName.ToLower())
                {
                    TLIradioRRULibrary RadioRRULibrary = _unitOfWork.RadioRRULibraryRepository.GetByID(LibId);

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIradioRRULibrary.ToString(), RadioRRULibrary, null).ToList();

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.Radio.ToString(), TablesNames.TLIradioRRULibrary.ToString(), RadioRRULibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Key.ToLower() == "ownerId".ToLower())
                            FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Key.ToLower() == "radioAntennaId".ToLower())
                            FKitem.Value = _mapper.Map<List<RadioAntennaViewModel>>(_unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower() &&
                                x.allLoadInstId != null ? (x.allLoadInst.radioAntennaId != null && !x.allLoadInst.Draft) : false, x => x.allLoadInst, x => x.allLoadInst.radioAntenna)
                            .Select(x => x.allLoadInst.radioAntenna).ToList());
                    }
                }
                else if (LoadSubType.TLIradioOther.ToString().ToLower() == TableName.ToLower())
                {
                    TLIradioOtherLibrary RadioOtherLibrary = _unitOfWork.RadioOtherLibraryRepository.GetByID(LibId);

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIradioOtherLibrary.ToString(), RadioOtherLibrary, null).ToList();

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.Radio.ToString(), TablesNames.TLIradioOtherLibrary.ToString(), RadioOtherLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tliowner")
                            FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                    }
                }

                objectInst.AttributesActivated = ListAttributesActivated;

                objectInst.RelatedTables = _unitOfWork.CivilLoadsRepository.GetRelatedTables(SiteCode);

                IEnumerable<DynaminAttInstViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, null);

                foreach (DynaminAttInstViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                {
                    TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                    if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                    {
                        if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                            DynamicAttribute.ValueString = DynamicAttributeEntity.DefaultValue;

                        else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                            DynamicAttribute.ValueDouble = int.Parse(DynamicAttributeEntity.DefaultValue);

                        else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                            DynamicAttribute.ValueDouble = double.Parse(DynamicAttributeEntity.DefaultValue);

                        else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                            DynamicAttribute.ValueBoolean = bool.Parse(DynamicAttributeEntity.DefaultValue);

                        else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                            DynamicAttribute.ValueDateTime = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                    }
                    else
                    {
                        DynamicAttribute.ValueString = " ".Split(' ')[0];
                    }
                }

                objectInst.DynamicAtts = DynamicAttributesWithoutValue;

                objectInst.CivilLoads = _unitOfWork.AttributeActivatedRepository
                    .GetInstAttributeActivated(TablesNames.TLIcivilLoads.ToString(), null, "allLoadInstId", "Dismantle", "SiteCode", "legId", "Leg2Id", "sideArmId",
                        "allCivilInstId", "civilSteelSupportCategoryId");

                return new Response<ObjectInstAtts>(true, objectInst, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<bool> DismantleLoads(string sitecode, int LoadId, string LoadName, int? TaskId)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    double? Freespace = 0;
                    double EquivalentSpace = 0;
                    var allLoadInst = _dbContext.TLIallLoadInst.Where(x => x.mwBUId == LoadId || x.mwDishId == LoadId || x.mwODUId == LoadId || x.mwRFUId == LoadId || x.mwOtherId == LoadId || x.radioAntennaId == LoadId || x.radioRRUId == LoadId || x.radioOtherId == LoadId || x.powerId == LoadId || x.loadOtherId == LoadId)
                        .Include(x => x.mwBU).Include(x => x.mwDish).Include(x => x.mwODU)
                        .Include(x => x.mwRFU).Include(x => x.mwOther).Include(x => x.radioAntenna).Include(x => x.radioRRU).Include(x => x.radioOther).
                        Include(x => x.power).Include(x => x.loadOther).ToList();

                    foreach (var item in allLoadInst)
                    {
                        var civilload = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id).Select(x => x.allCivilInstId).ToList();
                        foreach (var civilloadinst in civilload)
                        {
                            var allcivil = _dbContext.TLIallCivilInst.Where(x => x.Id == civilloadinst).Include(x => x.civilWithLegs).Include(x => x.civilWithoutLeg).Include(x => x.civilNonSteel).ToList();


                            foreach (var t in allcivil)
                            {
                                if (t.civilWithLegsId != null)
                                {
                                    if (item.mwBUId != null && LoadName == Helpers.Constants.TablesNames.TLImwBU.ToString())
                                    {

                                        TLImwBU TLImwBU = item.mwBU;
                                        var PortCascadeId = _dbContext.TLImwBU.Where(x => x.Id == item.mwBUId).Select(x => x.PortCascadeId).FirstOrDefault();
                                        var PortCascade = _dbContext.TLImwPort.Where(x => x.Id == PortCascadeId).ToList();
                                        foreach (var Port in PortCascade)
                                        {
                                            var allload = _dbContext.TLIallLoadInst.Where(x => x.mwBUId == Port.MwBUId).Select(x => x.Id).FirstOrDefault();
                                            var Civilloads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allload && x.allCivilInstId == t.Id && x.Dismantle == false).FirstOrDefault();
                                            if (Civilloads != null)
                                            {
                                                Civilloads.Dismantle = true;
                                                EquivalentSpace += 0;
                                                Port.MwBUId = 0;
                                                Port.MwBULibraryId = 0;
                                            }
                                        }
                                        var mwport = _dbContext.TLImwPort.Where(x => x.MwBUId == item.mwBUId).Select(x => x.Id).ToList();
                                        foreach (var port in mwport)
                                        {
                                            var mwrfu = _dbContext.TLImwRFU.Where(x => x.MwPortId == port).Select(x => x.Id).ToList();

                                            foreach (var rfu in mwrfu)
                                            {
                                                var allLoadRFU = _dbContext.TLIallLoadInst.Where(x => x.mwRFUId == rfu).Select(x => x.Id).ToList();
                                                foreach (var allLoad in allLoadRFU)
                                                {
                                                    var MwRfu = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allLoad && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                                    foreach (var MWRfU in MwRfu)
                                                    {
                                                        MWRfU.Dismantle = true;
                                                        EquivalentSpace += 0;
                                                    }

                                                }
                                            }

                                        }
                                        var civilLoads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.SiteCode == sitecode && x.allCivilInstId == t.Id).ToList();
                                        foreach (var civilLoad in civilLoads)
                                        {
                                            civilLoad.Dismantle = true;
                                            if (civilLoad.sideArmId == null)
                                            {

                                                EquivalentSpace += 0;

                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }

                                        }
                                        TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                        tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                    }
                                    else if (item.mwDishId != null && LoadName == Helpers.Constants.TablesNames.TLImwDish.ToString())
                                    {
                                        TLImwDish TLImwDish = item.mwDish;
                                        var mwODU = _dbContext.TLImwODU.Where(x => x.Mw_DishId == item.mwDishId).Select(x => x.Id).ToList();
                                        foreach (var ODU in mwODU)
                                        {
                                            var allLoadinst = _dbContext.TLIallLoadInst.Where(x => x.mwODUId == ODU).Select(x => x.Id).ToList();
                                            foreach (var Load in allLoadinst)
                                            {
                                                var civil = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == Load && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();


                                                foreach (var civillload in civil)
                                                {
                                                    civillload.Dismantle = true;
                                                    if (civillload.sideArmId == null)
                                                    {
                                                        EquivalentSpace += 0;
                                                    }
                                                    else
                                                    {
                                                        EquivalentSpace += 0;
                                                    }
                                                }
                                            }
                                        }
                                        var mwdish = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false).ToList();
                                        foreach (var TlImwdish in mwdish)
                                        {
                                            TlImwdish.Dismantle = true;
                                            var Bu = _dbContext.TLImwBU.Where(x => x.MainDishId == item.mwDishId).ToList();
                                            foreach (var TLIBu in Bu)
                                            {
                                                TLIBu.MainDishId = null;
                                            }
                                            if (TlImwdish.sideArmId == null)
                                            {
                                                EquivalentSpace += TLImwDish.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }

                                        }
                                        var TLImwdish = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false).ToList();
                                        foreach (var TLmwdish in TLImwdish)
                                        {
                                            TLmwdish.Dismantle = true;
                                            var Bu = _dbContext.TLImwBU.Where(x => x.MainDishId == item.mwDishId).ToList();
                                            foreach (var TLIBu in Bu)
                                            {
                                                TLIBu.MainDishId = null;
                                            }
                                        }
                                        TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                        tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;

                                    }
                                    else if (item.mwODUId != null && LoadName == Helpers.Constants.TablesNames.TLImwODU.ToString())
                                    {
                                        TLImwODU TLImwODU = item.mwODU;
                                        var MWODU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var mwodu in MWODU)
                                        {
                                            mwodu.Dismantle = true;
                                            EquivalentSpace += 0;
                                        }
                                        TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                        tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;

                                    }
                                    else if (item.mwRFUId != null && LoadName == Helpers.Constants.TablesNames.TLImwRFU.ToString())
                                    {
                                        TLImwRFU TLImwRFU = item.mwRFU;
                                        var MWRFU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var mwrfu in MWRFU)
                                        {
                                            mwrfu.Dismantle = true;
                                            EquivalentSpace += 0;
                                        }
                                        TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                        tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                    }
                                    else if (item.mwOtherId != null && LoadName == Helpers.Constants.TablesNames.TLImwOther.ToString())
                                    {
                                        TLImwOther TLImwOther = item.mwOther;
                                        var MWOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var mwother in MWOTHER)
                                        {
                                            mwother.Dismantle = true;
                                            if (mwother.sideArmId == null)
                                            {
                                                EquivalentSpace += TLImwOther.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }

                                        }
                                        var TLIMWOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var mwother in TLIMWOTHER)
                                        {
                                            mwother.Dismantle = true;
                                        }
                                        TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                        tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                    }
                                    else if (item.radioAntennaId != null && LoadName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString())
                                    {
                                        TLIradioAntenna TLIradioAntenna = item.radioAntenna;
                                        var RADIOANTENNA = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radioantenna in RADIOANTENNA)
                                        {
                                            radioantenna.Dismantle = true;
                                            var RadioRRu = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == item.radioAntennaId).ToList();
                                            foreach (var radioRru in RadioRRu)
                                            {
                                                radioRru.radioAntennaId = null;
                                            }
                                            if (radioantenna.sideArmId == null)
                                            {
                                                EquivalentSpace += TLIradioAntenna.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }

                                        }
                                        var TLIRADIOANTENNA = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radioantenna in TLIRADIOANTENNA)
                                        {
                                            radioantenna.Dismantle = true;
                                            var RadioRRu = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == item.radioAntennaId).ToList();
                                            foreach (var radioRru in RadioRRu)
                                            {
                                                radioRru.radioAntennaId = null;
                                            }
                                        }
                                        TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                        tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                    }
                                    else if (item.radioRRUId != null && LoadName == Helpers.Constants.TablesNames.TLIradioRRU.ToString())
                                    {

                                        TLIRadioRRU TLIRadioRRU = item.radioRRU;
                                        var RADIORRU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radiorru in RADIORRU)
                                        {
                                            radiorru.Dismantle = true;
                                            if (radiorru.sideArmId == null)
                                            {
                                                EquivalentSpace += TLIRadioRRU.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }
                                        }
                                        var TLIRADIORRU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radio in TLIRADIORRU)
                                        {
                                            radio.Dismantle = true;
                                        }
                                        TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                        tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                    }
                                    else if (item.radioOtherId != null && LoadName == Helpers.Constants.TablesNames.TLIradioOther.ToString())
                                    {
                                        TLIradioOther TLIradioOther = item.radioOther;
                                        var RADIOOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radioother in RADIOOTHER)
                                        {
                                            radioother.Dismantle = true;
                                            if (radioother.sideArmId == null)
                                            {
                                                EquivalentSpace += TLIradioOther.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }
                                        }
                                        var TLIRADIOOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radioother in TLIRADIOOTHER)
                                        {
                                            radioother.Dismantle = true;
                                        }
                                        TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                        tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                    }
                                    else if (item.powerId != null && LoadName == Helpers.Constants.TablesNames.TLIpower.ToString())
                                    {
                                        TLIpower TLIpower = item.power;
                                        var POWER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var power in POWER)
                                        {
                                            power.Dismantle = true;
                                            if (power.sideArmId == null)
                                            {
                                                EquivalentSpace += TLIpower.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }
                                        }
                                        var TLIPOWER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var power in TLIPOWER)
                                        {
                                            power.Dismantle = true;
                                        }
                                        TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                        tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                    }
                                    else if (item.loadOtherId != null && LoadName == Helpers.Constants.TablesNames.TLIloadOther.ToString())
                                    {
                                        TLIloadOther tLIloadOther = item.loadOther;
                                        var LOADOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var loadother in LOADOTHER)
                                        {
                                            loadother.Dismantle = true;
                                            if (loadother.sideArmId == null)
                                            {
                                                EquivalentSpace += tLIloadOther.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }
                                        }
                                        var TLILOADOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var loadother in TLILOADOTHER)
                                        {
                                            loadother.Dismantle = true;
                                        }
                                        TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                        tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                    }
                                    _dbContext.SaveChanges();
                                }
                                else if (t.civilWithoutLegId != null)
                                {

                                    if (item.mwBUId != null && LoadName == Helpers.Constants.TablesNames.TLImwBU.ToString())
                                    {

                                        TLImwBU TLImwBU = item.mwBU;
                                        var PortCascadeId = _dbContext.TLImwBU.Where(x => x.Id == item.mwBUId).Select(x => x.PortCascadeId).FirstOrDefault();
                                        var PortCascade = _dbContext.TLImwPort.Where(x => x.Id == PortCascadeId).ToList();
                                        foreach (var Port in PortCascade)
                                        {
                                            var allload = _dbContext.TLIallLoadInst.Where(x => x.mwBUId == Port.MwBUId).Select(x => x.Id).FirstOrDefault();
                                            var Civilloads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allload && x.allCivilInstId == t.Id && x.Dismantle == false).FirstOrDefault();
                                            if (Civilloads != null)
                                            {
                                                Civilloads.Dismantle = true;
                                                EquivalentSpace += 0;
                                                Port.MwBUId = 0;
                                                Port.MwBULibraryId = 0;
                                            }
                                        }
                                        var mwport = _dbContext.TLImwPort.Where(x => x.MwBUId == item.mwBUId).Select(x => x.Id).ToList();
                                        foreach (var port in mwport)
                                        {
                                            var mwrfu = _dbContext.TLImwRFU.Where(x => x.MwPortId == port).Select(x => x.Id).ToList();

                                            foreach (var rfu in mwrfu)
                                            {
                                                var allLoadRFU = _dbContext.TLIallLoadInst.Where(x => x.mwRFUId == rfu).Select(x => x.Id).ToList();
                                                foreach (var allLoad in allLoadRFU)
                                                {
                                                    var MwRfu = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allLoad && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                                    foreach (var MWRfU in MwRfu)
                                                    {
                                                        MWRfU.Dismantle = true;
                                                        EquivalentSpace += 0;
                                                    }

                                                }
                                            }

                                        }
                                        var civilLoads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.SiteCode == sitecode && x.allCivilInstId == t.Id).ToList();
                                        foreach (var civilLoad in civilLoads)
                                        {
                                            civilLoad.Dismantle = true;
                                            if (civilLoad.sideArmId == null)
                                            {

                                                EquivalentSpace += 0;

                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }

                                        }
                                        TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                        tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float)EquivalentSpace;
                                    }
                                    else if (item.mwDishId != null && LoadName == Helpers.Constants.TablesNames.TLImwDish.ToString())
                                    {
                                        TLImwDish TLImwDish = item.mwDish;
                                        var mwODU = _dbContext.TLImwODU.Where(x => x.Mw_DishId == item.mwDishId).Select(x => x.Id).ToList();
                                        foreach (var ODU in mwODU)
                                        {
                                            var allLoadinst = _dbContext.TLIallLoadInst.Where(x => x.mwODUId == ODU).Select(x => x.Id).ToList();
                                            foreach (var Load in allLoadinst)
                                            {
                                                var civil = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == Load && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();


                                                foreach (var civillload in civil)
                                                {
                                                    civillload.Dismantle = true;
                                                    if (civillload.sideArmId == null)
                                                    {
                                                        EquivalentSpace += 0;
                                                    }
                                                    else
                                                    {
                                                        EquivalentSpace += 0;
                                                    }
                                                }
                                            }
                                        }
                                        var mwdish = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false).ToList();
                                        foreach (var TLImwdish in mwdish)
                                        {
                                            TLImwdish.Dismantle = true;
                                            var Bu = _dbContext.TLImwBU.Where(x => x.MainDishId == item.mwDishId).ToList();
                                            foreach (var TLIBu in Bu)
                                            {
                                                TLIBu.MainDishId = null;
                                            }
                                            if (TLImwdish.sideArmId == null)
                                            {
                                                EquivalentSpace += TLImwDish.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }

                                        }
                                        var tlimwdish = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false).ToList();
                                        foreach (var MwDish in tlimwdish)
                                        {
                                            MwDish.Dismantle = true;
                                            var Bu = _dbContext.TLImwBU.Where(x => x.MainDishId == item.mwDishId).ToList();
                                            foreach (var TLIBu in Bu)
                                            {
                                                TLIBu.MainDishId = null;
                                            }
                                        }
                                        TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                        tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float)EquivalentSpace;

                                    }
                                    else if (item.mwODUId != null && LoadName == Helpers.Constants.TablesNames.TLImwODU.ToString())
                                    {
                                        TLImwODU TLImwODU = item.mwODU;
                                        var MWODU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var mwodu in MWODU)
                                        {
                                            mwodu.Dismantle = true;
                                            EquivalentSpace += 0;
                                        }
                                        TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                        tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float)EquivalentSpace;

                                    }
                                    else if (item.mwRFUId != null && LoadName == Helpers.Constants.TablesNames.TLImwRFU.ToString())
                                    {
                                        TLImwRFU TLImwRFU = item.mwRFU;
                                        var MWRFU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var mwrfu in MWRFU)
                                        {
                                            mwrfu.Dismantle = true;
                                            EquivalentSpace += 0;
                                        }
                                        TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                        tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float)EquivalentSpace;
                                    }
                                    else if (item.mwOtherId != null && LoadName == Helpers.Constants.TablesNames.TLImwOther.ToString())
                                    {
                                        TLImwOther TLImwOther = item.mwOther;
                                        var MWOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var mwother in MWOTHER)
                                        {
                                            mwother.Dismantle = true;
                                            if (mwother.sideArmId == null)
                                            {
                                                EquivalentSpace += TLImwOther.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }

                                        }
                                        var TLIMWOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var mwother in TLIMWOTHER)
                                        {
                                            mwother.Dismantle = true;
                                        }
                                        TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                        tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float)EquivalentSpace;
                                    }
                                    else if (item.radioAntennaId != null && LoadName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString())
                                    {
                                        TLIradioAntenna TLIradioAntenna = item.radioAntenna;
                                        var RADIOANTENNA = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radioantenna in RADIOANTENNA)
                                        {
                                            radioantenna.Dismantle = true;
                                            var RadioRRu = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == item.radioAntennaId).ToList();
                                            foreach (var TLIRadioRRu in RadioRRu)
                                            {
                                                TLIRadioRRu.radioAntennaId = null;
                                            }
                                            if (radioantenna.sideArmId == null)
                                            {
                                                EquivalentSpace += TLIradioAntenna.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }

                                        }
                                        var TLIRADIOANTENNA = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radioantenna in TLIRADIOANTENNA)
                                        {
                                            radioantenna.Dismantle = true;
                                            var RadioRRu = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == item.radioAntennaId).ToList();
                                            foreach (var TLIRadioRRu in RadioRRu)
                                            {
                                                TLIRadioRRu.radioAntennaId = null;
                                            }
                                        }
                                        TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                        tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float)EquivalentSpace;
                                    }
                                    else if (item.radioRRUId != null && LoadName == Helpers.Constants.TablesNames.TLIradioRRU.ToString())
                                    {

                                        TLIRadioRRU TLIRadioRRU = item.radioRRU;
                                        var RADIORRU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radiorru in RADIORRU)
                                        {
                                            radiorru.Dismantle = true;
                                            if (radiorru.sideArmId == null)
                                            {
                                                EquivalentSpace += TLIRadioRRU.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }
                                        }
                                        var TLIRADIORRU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radiorru in TLIRADIORRU)
                                        {
                                            radiorru.Dismantle = true;
                                        }
                                        TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                        tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float)EquivalentSpace;
                                    }
                                    else if (item.radioOtherId != null && LoadName == Helpers.Constants.TablesNames.TLIradioOther.ToString())
                                    {
                                        TLIradioOther TLIradioOther = item.radioOther;
                                        var RADIOOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radioother in RADIOOTHER)
                                        {
                                            radioother.Dismantle = true;
                                            if (radioother.sideArmId == null)
                                            {
                                                EquivalentSpace += TLIradioOther.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }
                                        }
                                        var TLIRADIOOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radioother in TLIRADIOOTHER)
                                        {
                                            radioother.Dismantle = true;
                                        }
                                        TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                        tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float)EquivalentSpace;
                                    }
                                    else if (item.powerId != null && LoadName == Helpers.Constants.TablesNames.TLIpower.ToString())
                                    {
                                        TLIpower TLIpower = item.power;
                                        var POWER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var power in POWER)
                                        {
                                            power.Dismantle = true;
                                            if (power.sideArmId == null)
                                            {
                                                EquivalentSpace += TLIpower.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }
                                        }
                                        var TLIPOWER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var power in TLIPOWER)
                                        {
                                            power.Dismantle = true;
                                        }
                                        TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                        tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float)EquivalentSpace;
                                    }
                                    else if (item.loadOtherId != null && LoadName == Helpers.Constants.TablesNames.TLIloadOther.ToString())
                                    {
                                        TLIloadOther tLIloadOther = item.loadOther;
                                        var LOADOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var loadother in LOADOTHER)
                                        {
                                            loadother.Dismantle = true;
                                            if (loadother.sideArmId == null)
                                            {
                                                EquivalentSpace += tLIloadOther.EquivalentSpace;
                                            }
                                            else
                                            {
                                                EquivalentSpace += 0;
                                            }
                                        }
                                        var TLILOADOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var loadother in TLILOADOTHER)
                                        {
                                            loadother.Dismantle = true;
                                        }
                                        TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                        tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float)EquivalentSpace;
                                    }
                                    _dbContext.SaveChanges();
                                }
                                else if (t.civilNonSteelId != null)
                                {
                                    if (item.mwBUId != null && LoadName == Helpers.Constants.TablesNames.TLImwBU.ToString())
                                    {
                                        TLImwBU TLImwBU = item.mwBU;
                                        var PortCascadeId = _dbContext.TLImwBU.Where(x => x.Id == item.mwBUId).Select(x => x.PortCascadeId).FirstOrDefault();
                                        var PortCascade = _dbContext.TLImwPort.Where(x => x.Id == PortCascadeId).ToList();
                                        foreach (var Port in PortCascade)
                                        {
                                            var allload = _dbContext.TLIallLoadInst.Where(x => x.mwBUId == Port.MwBUId).Select(x => x.Id).FirstOrDefault();
                                            var Civilloads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allload && x.allCivilInstId == t.Id && x.Dismantle == false).FirstOrDefault();
                                            if (Civilloads != null)
                                            {
                                                Civilloads.Dismantle = true;
                                                EquivalentSpace += 0;
                                                Port.MwBUId = 0;
                                                Port.MwBULibraryId = 0;
                                            }
                                        }
                                        var mwport = _dbContext.TLImwPort.Where(x => x.MwBUId == item.mwBUId).Select(x => x.Id).ToList();
                                        foreach (var port in mwport)
                                        {
                                            var mwrfu = _dbContext.TLImwRFU.Where(x => x.MwPortId == port).Select(x => x.Id).ToList();

                                            foreach (var rfu in mwrfu)
                                            {
                                                var allLoadRFU = _dbContext.TLIallLoadInst.Where(x => x.mwRFUId == rfu).Select(x => x.Id).ToList();
                                                foreach (var allLoad in allLoadRFU)
                                                {
                                                    var MwRfu = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allLoad && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                                    foreach (var MWRfU in MwRfu)
                                                    {
                                                        MWRfU.Dismantle = true;
                                                        EquivalentSpace += 0;
                                                    }

                                                }
                                            }

                                        }
                                        var civilLoads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.SiteCode == sitecode && x.allCivilInstId == t.Id).ToList();
                                        foreach (var civilLoad in civilLoads)
                                        {
                                            civilLoad.Dismantle = true;
                                            EquivalentSpace += 0;

                                        }
                                        TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                        tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;
                                    }
                                    else if (item.mwDishId != null && LoadName == Helpers.Constants.TablesNames.TLImwDish.ToString())
                                    {
                                        TLImwDish TLImwDish = item.mwDish;
                                        var mwODU = _dbContext.TLImwODU.Where(x => x.Mw_DishId == item.mwDishId).Select(x => x.Id).ToList();
                                        foreach (var ODU in mwODU)
                                        {
                                            var allLoadinst = _dbContext.TLIallLoadInst.Where(x => x.mwODUId == ODU).Select(x => x.Id).ToList();
                                            foreach (var Load in allLoadinst)
                                            {
                                                var civil = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == Load && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();


                                                foreach (var civillload in civil)
                                                {
                                                    civillload.Dismantle = true;
                                                    EquivalentSpace += 0;
                                                }
                                            }
                                        }
                                        var mwdish = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false).ToList();
                                        foreach (var TLImwdish in mwdish)
                                        {
                                            TLImwdish.Dismantle = true;
                                            var Bu = _dbContext.TLImwBU.Where(x => x.MainDishId == item.mwDishId).ToList();
                                            foreach (var TLIBu in Bu)
                                            {
                                                TLIBu.MainDishId = null;
                                            }
                                            EquivalentSpace += 0;

                                        }
                                        TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                        tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                    }
                                    else if (item.mwODUId != null && LoadName == Helpers.Constants.TablesNames.TLImwODU.ToString())
                                    {
                                        TLImwODU TLImwODU = item.mwODU;
                                        var MWODU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var mwodu in MWODU)
                                        {
                                            mwodu.Dismantle = true;
                                            EquivalentSpace += 0;
                                        }
                                        TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                        tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;


                                    }
                                    else if (item.mwRFUId != null && LoadName == Helpers.Constants.TablesNames.TLImwRFU.ToString())
                                    {
                                        TLImwRFU TLImwRFU = item.mwRFU;
                                        var MWRFU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var mwrfu in MWRFU)
                                        {
                                            mwrfu.Dismantle = true;
                                            EquivalentSpace += 0;
                                        }
                                        TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                        tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                    }
                                    else if (item.mwOtherId != null && LoadName == Helpers.Constants.TablesNames.TLImwOther.ToString())
                                    {
                                        TLImwOther TLImwOther = item.mwOther;
                                        var MWOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var mwother in MWOTHER)
                                        {
                                            mwother.Dismantle = true;
                                            EquivalentSpace += 0;
                                        }
                                        TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                        tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                    }
                                    else if (item.radioAntennaId != null && LoadName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString())
                                    {
                                        TLIradioAntenna TLIradioAntenna = item.radioAntenna;
                                        var RADIOANTENNA = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radioantenna in RADIOANTENNA)
                                        {
                                            radioantenna.Dismantle = true;
                                            var RadioRRu = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == item.radioAntennaId).ToList();
                                            foreach (var radioRru in RadioRRu)
                                            {
                                                radioRru.radioAntennaId = null;
                                            }
                                            EquivalentSpace += 0;

                                        }
                                        TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                        tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                    }
                                    else if (item.radioRRUId != null && LoadName == Helpers.Constants.TablesNames.TLIradioRRU.ToString())
                                    {

                                        TLIRadioRRU TLIRadioRRU = item.radioRRU;
                                        var RADIORRU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radiorru in RADIORRU)
                                        {
                                            radiorru.Dismantle = true;
                                            EquivalentSpace += 0;

                                        }
                                        TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                        tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                    }
                                    else if (item.radioOtherId != null && LoadName == Helpers.Constants.TablesNames.TLIradioOther.ToString())
                                    {
                                        TLIradioOther TLIradioOther = item.radioOther;
                                        var RADIOOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var radioother in RADIOOTHER)
                                        {
                                            radioother.Dismantle = true;
                                            EquivalentSpace += 0;

                                        }
                                        TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                        tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                    }
                                    else if (item.powerId != null && LoadName == Helpers.Constants.TablesNames.TLIpower.ToString())
                                    {
                                        TLIpower TLIpower = item.power;
                                        var POWER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var power in POWER)
                                        {
                                            power.Dismantle = true;
                                            EquivalentSpace += 0;

                                        }
                                        TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                        tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                    }
                                    else if (item.loadOtherId != null && LoadName == Helpers.Constants.TablesNames.TLIloadOther.ToString())
                                    {
                                        TLIloadOther tLIloadOther = item.loadOther;
                                        var LOADOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                        foreach (var loadother in LOADOTHER)
                                        {
                                            loadother.Dismantle = true;
                                            EquivalentSpace += 0;
                                        }
                                        TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                        tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                    }
                                    _dbContext.SaveChanges();

                                }
                            }
                        }
                        _dbContext.SaveChanges();
                        if (TaskId != null)
                        {
                            var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                        }
                        scope.Complete();
                    }
                    return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception er)
                {

                    return new Response<bool>(false, false, null, er.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        //Function take 2 parameters
        //get table name Entity by TableName
        //specify the table i deal with
        //get record by Id
        //get activated attributes with values
        //get dynamic attributes
        //get related tables
        public Response<ObjectInstAttsForSideArm> GetById(int RadioInsId, string TableName)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);

                ObjectInstAttsForSideArm objectInst = new ObjectInstAttsForSideArm();
                TLIallLoadInst AllLoadInst = new TLIallLoadInst();

                TLIcivilLoads CivilLoads = new TLIcivilLoads();
                List<BaseAttView> LoadInstAttributes = new List<BaseAttView>();

                TLIallCivilInst AllCivilInst = new TLIallCivilInst();
                List<BaseInstAttView> RadioInstallationInfo = new List<BaseInstAttView>();

                if (LoadSubType.TLIradioAntenna.ToString() == TableName)
                {
                    TLIradioAntenna Radio_Antenna = _unitOfWork.RadioAntennaRepository
                        .GetIncludeWhereFirst(x => x.Id == RadioInsId, x => x.owner,
                            x => x.installationPlace, x => x.radioAntennaLibrary);

                    RadioAntennaLibraryViewModel RadioAntinaLibrary = _mapper.Map<RadioAntennaLibraryViewModel>(_unitOfWork.RadioAntennaLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == Radio_Antenna.radioAntennaLibraryId));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                       .GetAttributeActivated(TablesNames.TLIradioAntennaLibrary.ToString(), RadioAntinaLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = RadioAntinaLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(RadioAntinaLibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                       .GetLogistical(TablePartName.Radio.ToString(), TablesNames.TLIradioAntennaLibrary.ToString(), RadioAntinaLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(TablesNames.TLIradioAntenna.ToString(), Radio_Antenna, "installationPlaceId").ToList();


                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tliowner")
                        {
                            if (Radio_Antenna.owner == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = Radio_Antenna.owner.OwnerName;
                        }
                        else if (FKitem.Desc.ToLower() == "tliinstallationplace")
                        {
                            if (Radio_Antenna.installationPlace == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = Radio_Antenna.installationPlace.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tliradioantennalibrary")
                        {
                            if (Radio_Antenna.radioAntennaLibrary == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = Radio_Antenna.radioAntennaLibrary.Model;
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;
                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, RadioInsId, null);

                    objectInst.RelatedTables = _unitOfWork.RadioAntennaRepository.GetRelatedTables();

                    AllLoadInst = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.radioAntennaId == RadioInsId);

                    CivilLoads = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhereFirst(x => x.allLoadInstId == AllLoadInst.Id, x => x.sideArm, x => x.site, x => x.leg, x => x.allCivilInst,
                            x => x.allLoadInst.loadOther, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwOther,
                            x => x.allLoadInst.mwRFU, x => x.allLoadInst.power, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioRRU,
                            x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel, x => x.civilSteelSupportCategory);

                    LoadInstAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIcivilLoads.ToString(), CivilLoads, null, "allLoadInstId",
                            "Dismantle", "SiteCode", "civilSteelSupportCategoryId", "legId", "Leg2Id",
                                "sideArmId", "allCivilInstId").ToList();

                    List<KeyValuePair<string, List<DropDownListFilters>>> CivilLoadsRelatedTables = _unitOfWork.CivilLoadsRepository
                        .GetRelatedTables(CivilLoads.SiteCode);

                    if (CivilLoads != null)
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> radioantennaRelatedTables = _unitOfWork.RadioAntennaRepository
                            .GetRelatedTables();

                        radioantennaRelatedTables.AddRange(CivilLoadsRelatedTables);

                        if (CivilLoads.allCivilInst.civilWithLegsId != null)
                        {
                            List<TLIleg> LegsForCivilWithLegLibrary = _unitOfWork.LegRepository
                                .GetWhere(x => x.CivilWithLegInstId == CivilLoads.allCivilInst.civilWithLegsId).ToList();

                            List<DropDownListFilters> LegIds = _mapper.Map<List<DropDownListFilters>>(LegsForCivilWithLegLibrary);

                            radioantennaRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg1Id", LegIds));

                            List<TLIleg> Legs2ForCivilWithLegLibrary = LegsForCivilWithLegLibrary.Except(LegsForCivilWithLegLibrary
                                .Where(x => x.Id == CivilLoads.legId)).ToList();

                            List<DropDownListFilters> Leg2Ids = _mapper.Map<List<DropDownListFilters>>(Legs2ForCivilWithLegLibrary);

                            radioantennaRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg2Id", Leg2Ids));
                        }

                        objectInst.RelatedTables = radioantennaRelatedTables;

                        AllCivilInst = _unitOfWork.CivilLoadsRepository
                           .GetIncludeWhereFirst(x => (x.allLoadInstId != null ? (x.allLoadInst.radioAntennaId != null ?
                               x.allLoadInst.radioAntennaId.Value == RadioInsId : false) : false) && !x.Dismantle, x => x.allCivilInst, x => x.allLoadInst).allCivilInst;

                        if (AllCivilInst.civilWithLegsId != null)
                        {
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil with legs"
                            });
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil with legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil with legs support items",
                                Label = "Select civil with legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithLegsRepository.GetByID(AllCivilInst.civilWithLegsId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else if (CivilLoads.legId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Leg"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the leg",
                                    enable = true,
                                    Id = CivilLoads.legId.Value,
                                    Key = "Select the leg",
                                    Label = "Select the leg",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.leg.CiviLegName
                                });
                            }
                            else
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }
                        else if (AllCivilInst.civilWithoutLegId != null)
                        {
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil without legs"
                            });
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil without legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil without legs support items",
                                Label = "Select civil without legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithoutLegRepository.GetByID(AllCivilInst.civilWithoutLegId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }
                        else if (AllCivilInst.civilNonSteelId != null)
                        {
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil non steel"
                            });
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil non steel support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil non steel support items",
                                Label = "Select civil non steel support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilNonSteelRepository.GetByID(AllCivilInst.civilNonSteelId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }

                        RadioInstallationInfo.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "allLoadInstId",
                            enable = false,
                            Id = AllLoadInst.Id,
                            Key = "allLoadInstId",
                            Label = "allLoadInstId",
                            Manage = false,
                            Required = false,
                            Value = AllLoadInst.Id
                        });
                        objectInst.SideArmInstallationInfo = RadioInstallationInfo;
                    }
                }
                else if (LoadSubType.TLIradioRRU.ToString() == TableName)
                {
                    TLIRadioRRU Radio_RRU = _unitOfWork.RadioRRURepository
                        .GetIncludeWhereFirst(x => x.Id == RadioInsId, x => x.owner,
                            x => x.installationPlace, x => x.radioRRULibrary, x => x.radioAntenna);

                    RadioRRULibraryViewModel RadioRRuLibrary = _mapper.Map<RadioRRULibraryViewModel>(_unitOfWork.RadioRRULibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == Radio_RRU.radioRRULibraryId));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                       .GetAttributeActivated(TablesNames.TLIradioRRULibrary.ToString(), RadioRRuLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = RadioRRuLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(RadioRRuLibrary);
                        }
                    }
                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.Radio.ToString(), TablesNames.TLIradioRRULibrary.ToString(), RadioRRuLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);
                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(TablesNames.TLIradioRRU.ToString(), Radio_RRU, "installationPlaceId").ToList();


                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Key.ToLower() == "ownerId".ToLower())
                        {
                            if (Radio_RRU.owner == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = Radio_RRU.owner.OwnerName;
                        }
                        else if (FKitem.Key.ToLower() == "installationPlaceId".ToLower())
                        {
                            if (Radio_RRU.installationPlace == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = Radio_RRU.installationPlace.Name;
                        }
                        else if (FKitem.Key.ToLower() == "radioAntennaId".ToLower())
                        {
                            if (Radio_RRU.radioAntenna == null)
                                FKitem.Value = "NA";

                            else
                            {
                                FKitem.Value = Radio_RRU.radioAntenna.Name;
                            }
                        }
                        else if (FKitem.Key.ToLower() == "radioRRULibraryId".ToLower())
                        {
                            if (Radio_RRU.radioRRULibrary == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = Radio_RRU.radioRRULibrary.Model;
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;
                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, RadioInsId, null);

                    List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = _unitOfWork.RadioRRURepository.GetRelatedTables(CivilLoads.SiteCode);

                    objectInst.RelatedTables = RelatedTables;

                    AllLoadInst = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.radioRRUId == RadioInsId);

                    CivilLoads = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhereFirst(x => x.allLoadInstId == AllLoadInst.Id, x => x.sideArm, x => x.site, x => x.leg, x => x.allCivilInst,
                            x => x.allLoadInst.loadOther, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwOther,
                            x => x.allLoadInst.mwRFU, x => x.allLoadInst.power, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioRRU,
                            x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel, x => x.civilSteelSupportCategory);

                    LoadInstAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIcivilLoads.ToString(), CivilLoads, null, "allLoadInstId",
                            "Dismantle", "SiteCode", "civilSteelSupportCategoryId", "legId", "Leg2Id",
                                "sideArmId", "allCivilInstId").ToList();

                    List<KeyValuePair<string, List<DropDownListFilters>>> CivilLoadsRelatedTables = _unitOfWork.CivilLoadsRepository
                        .GetRelatedTables(CivilLoads.SiteCode);

                    if (CivilLoads != null)
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> radiorruRelatedTables = _unitOfWork.RadioRRURepository
                            .GetRelatedTables(CivilLoads.SiteCode);

                        radiorruRelatedTables.AddRange(CivilLoadsRelatedTables);

                        if (CivilLoads.allCivilInst.civilWithLegsId != null)
                        {
                            List<TLIleg> LegsForCivilWithLegLibrary = _unitOfWork.LegRepository
                                .GetWhere(x => x.CivilWithLegInstId == CivilLoads.allCivilInst.civilWithLegsId).ToList();

                            List<DropDownListFilters> LegIds = _mapper.Map<List<DropDownListFilters>>(LegsForCivilWithLegLibrary);

                            radiorruRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg1Id", LegIds));

                            List<TLIleg> Legs2ForCivilWithLegLibrary = LegsForCivilWithLegLibrary.Except(LegsForCivilWithLegLibrary
                                .Where(x => x.Id == CivilLoads.legId)).ToList();

                            List<DropDownListFilters> Leg2Ids = _mapper.Map<List<DropDownListFilters>>(Legs2ForCivilWithLegLibrary);

                            radiorruRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg2Id", Leg2Ids));
                        }

                        objectInst.RelatedTables = radiorruRelatedTables;

                        AllCivilInst = _unitOfWork.CivilLoadsRepository
                           .GetIncludeWhereFirst(x => (x.allLoadInstId != null ? (x.allLoadInst.radioRRUId != null ?
                               x.allLoadInst.radioRRUId.Value == RadioInsId : false) : false) && !x.Dismantle, x => x.allCivilInst, x => x.allLoadInst).allCivilInst;

                        if (AllCivilInst.civilWithLegsId != null)
                        {
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil with legs"
                            });
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil with legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil with legs support items",
                                Label = "Select civil with legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithLegsRepository.GetByID(AllCivilInst.civilWithLegsId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else if (CivilLoads.legId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Leg"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the leg",
                                    enable = true,
                                    Id = CivilLoads.legId.Value,
                                    Key = "Select the leg",
                                    Label = "Select the leg",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.leg.CiviLegName
                                });
                            }
                            else
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }
                        else if (AllCivilInst.civilWithoutLegId != null)
                        {
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil without legs"
                            });
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil without legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil without legs support items",
                                Label = "Select civil without legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithoutLegRepository.GetByID(AllCivilInst.civilWithoutLegId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }
                        else if (AllCivilInst.civilNonSteelId != null)
                        {
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil non steel"
                            });
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil non steel support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil non steel support items",
                                Label = "Select civil non steel support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilNonSteelRepository.GetByID(AllCivilInst.civilNonSteelId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }

                        RadioInstallationInfo.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "allLoadInstId",
                            enable = false,
                            Id = AllLoadInst.Id,
                            Key = "allLoadInstId",
                            Label = "allLoadInstId",
                            Manage = false,
                            Required = false,
                            Value = AllLoadInst.Id
                        });
                        objectInst.SideArmInstallationInfo = RadioInstallationInfo;
                    }
                }
                else if (LoadSubType.TLIradioOther.ToString() == TableName)
                {
                    TLIradioOther RadioOther = _unitOfWork.RadioOtherRepository
                       .GetIncludeWhereFirst(x => x.Id == RadioInsId, x => x.owner,
                           x => x.installationPlace, x => x.radioOtherLibrary);

                    RadioOtherLibraryViewModel RadioOtherLibrary = _mapper.Map<RadioOtherLibraryViewModel>(_unitOfWork.RadioOtherLibraryRepository
                       .GetIncludeWhereFirst(x => x.Id == RadioOther.radioOtherLibraryId));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                       .GetAttributeActivated(TablesNames.TLIradioOtherLibrary.ToString(), RadioOtherLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = RadioOtherLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(RadioOtherLibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.Radio.ToString(), TablesNames.TLIradioOtherLibrary.ToString(), RadioOtherLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(TablesNames.TLIradioOther.ToString(), RadioOther, "installationPlaceId").ToList();


                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tliowner")
                        {
                            if (RadioOther.owner == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = RadioOther.owner.OwnerName;
                        }
                        else if (FKitem.Desc.ToLower() == "tliinstallationplace")
                        {
                            if (RadioOther.installationPlace == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = RadioOther.installationPlace.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tliradiootherlibrary")
                        {
                            if (RadioOther.radioOtherLibrary == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = RadioOther.radioOtherLibrary.Model;
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;
                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, RadioInsId, null);

                    objectInst.RelatedTables = _unitOfWork.RadioOtherRepository.GetRelatedTables();

                    AllLoadInst = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.radioOtherId == RadioInsId);

                    CivilLoads = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhereFirst(x => x.allLoadInstId == AllLoadInst.Id, x => x.sideArm, x => x.site, x => x.leg, x => x.allCivilInst,
                            x => x.allLoadInst.loadOther, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwOther,
                            x => x.allLoadInst.mwRFU, x => x.allLoadInst.power, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioRRU,
                            x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel, x => x.civilSteelSupportCategory);

                    LoadInstAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIcivilLoads.ToString(), CivilLoads, null, "allLoadInstId",
                            "Dismantle", "SiteCode", "civilSteelSupportCategoryId", "legId", "Leg2Id",
                                "sideArmId", "allCivilInstId").ToList();

                    List<KeyValuePair<string, List<DropDownListFilters>>> CivilLoadsRelatedTables = _unitOfWork.CivilLoadsRepository
                        .GetRelatedTables(CivilLoads.SiteCode);

                    if (CivilLoads != null)
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> radiootherRelatedTables = _unitOfWork.RadioOtherRepository
                            .GetRelatedTables();

                        radiootherRelatedTables.AddRange(CivilLoadsRelatedTables);

                        if (CivilLoads.allCivilInst.civilWithLegsId != null)
                        {
                            List<TLIleg> LegsForCivilWithLegLibrary = _unitOfWork.LegRepository
                                .GetWhere(x => x.CivilWithLegInstId == CivilLoads.allCivilInst.civilWithLegsId).ToList();

                            List<DropDownListFilters> LegIds = _mapper.Map<List<DropDownListFilters>>(LegsForCivilWithLegLibrary);

                            radiootherRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg1Id", LegIds));

                            List<TLIleg> Legs2ForCivilWithLegLibrary = LegsForCivilWithLegLibrary.Except(LegsForCivilWithLegLibrary
                                .Where(x => x.Id == CivilLoads.legId)).ToList();

                            List<DropDownListFilters> Leg2Ids = _mapper.Map<List<DropDownListFilters>>(Legs2ForCivilWithLegLibrary);

                            radiootherRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg2Id", Leg2Ids));
                        }

                        objectInst.RelatedTables = radiootherRelatedTables;

                        AllCivilInst = _unitOfWork.CivilLoadsRepository
                           .GetIncludeWhereFirst(x => (x.allLoadInstId != null ? (x.allLoadInst.radioOtherId != null ?
                               x.allLoadInst.radioOtherId.Value == RadioInsId : false) : false) && !x.Dismantle, x => x.allCivilInst, x => x.allLoadInst).allCivilInst;

                        if (AllCivilInst.civilWithLegsId != null)
                        {
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil with legs"
                            });
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil with legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil with legs support items",
                                Label = "Select civil with legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithLegsRepository.GetByID(AllCivilInst.civilWithLegsId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else if (CivilLoads.legId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Leg"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the leg",
                                    enable = true,
                                    Id = CivilLoads.legId.Value,
                                    Key = "Select the leg",
                                    Label = "Select the leg",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.leg.CiviLegName
                                });
                            }
                            else
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }
                        else if (AllCivilInst.civilWithoutLegId != null)
                        {
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil without legs"
                            });
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil without legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil without legs support items",
                                Label = "Select civil without legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithoutLegRepository.GetByID(AllCivilInst.civilWithoutLegId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }
                        else if (AllCivilInst.civilNonSteelId != null)
                        {
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil non steel"
                            });
                            RadioInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil non steel support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil non steel support items",
                                Label = "Select civil non steel support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilNonSteelRepository.GetByID(AllCivilInst.civilNonSteelId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                RadioInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }

                        RadioInstallationInfo.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "allLoadInstId",
                            enable = false,
                            Id = AllLoadInst.Id,
                            Key = "allLoadInstId",
                            Label = "allLoadInstId",
                            Manage = false,
                            Required = false,
                            Value = AllLoadInst.Id
                        });
                        objectInst.SideArmInstallationInfo = RadioInstallationInfo;
                    }
                }
                if (CivilLoads != null)
                {
                    foreach (BaseAttView FKitem in LoadInstAttributes)
                    {
                        if (FKitem.Desc.ToLower() == "tlisite")
                        {
                            FKitem.Value = CivilLoads.site.SiteName;
                        }
                    }
                    objectInst.CivilLoads = _mapper.Map<IEnumerable<BaseInstAttView>>(LoadInstAttributes);
                }

                return new Response<ObjectInstAttsForSideArm>(true, objectInst, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAttsForSideArm>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        //Function return all RadioRRU
        public Response<ReturnWithFilters<RadioRRUViewModel>> GetRadioRRUsList(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                var RadioRRUs = new ReturnWithFilters<RadioRRUViewModel>();
                var RRUs = _unitOfWork.RadioRRURepository.GetAllIncludeMultiple(parameters, filters, out count, x => x.installationPlace, x => x.owner, x => x.radioAntenna, x => x.radioRRULibrary).ToList();
                RadioRRUs.Model = _mapper.Map<List<RadioRRUViewModel>>(RRUs);
                if (WithFilterData)
                {
                    // RadioRRUs.filters = _unitOfWork.RadioRRURepository.GetRelatedTables();
                }
                else
                {
                    RadioRRUs.filters = null;
                }
                return new Response<ReturnWithFilters<RadioRRUViewModel>>(true, RadioRRUs, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<RadioRRUViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        //Function return all RadioAntenna
        public Response<ReturnWithFilters<RadioAntennaViewModel>> GetRadioAntennasList(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                var RadioAntennas = new ReturnWithFilters<RadioAntennaViewModel>();
                var Antennas = _unitOfWork.RadioAntennaRepository.GetAllIncludeMultiple(parameters, filters, out count, x => x.installationPlace, x => x.owner, x => x.radioAntennaLibrary).ToList();
                RadioAntennas.Model = _mapper.Map<List<RadioAntennaViewModel>>(Antennas);
                if (WithFilterData)
                {
                    RadioAntennas.filters = _unitOfWork.RadioAntennaRepository.GetRelatedTables();
                }
                else
                {
                    RadioAntennas.filters = null;
                }
                return new Response<ReturnWithFilters<RadioAntennaViewModel>>(true, RadioAntennas, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<RadioAntennaViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<GetForAddMWDishInstallationObject> GetAttForAddRadioAntennaInstallation(int LibraryID, string SiteCode)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x =>
                    x.TableName == "TLIradioAntenna");

                GetForAddMWDishInstallationObject objectInst = new GetForAddMWDishInstallationObject();
                List<BaseInstAttViews> ListAttributesActivated = new List<BaseInstAttViews>();

                EditRadioAntennaLibraryAttributes RadioAntennaLibrary = _mapper.Map<EditRadioAntennaLibraryAttributes>(_unitOfWork.RadioAntennaLibraryRepository
                    .GetIncludeWhereFirst(x => x.Id == LibraryID));
                if (RadioAntennaLibrary != null)
                {
                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetForAdd(TablesNames.TLIradioAntennaLibrary.ToString(), RadioAntennaLibrary, null).ToList();


                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                        .GetLogisticals(TablePartName.Radio.ToString(), Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString(), RadioAntennaLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivatedGetForAdd(LoadSubType.TLIradioAntenna.ToString(), null, "Name", "installationPlaceId", "radioAntennaLibraryId", "EquivalentSpace").ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }

                    Dictionary<string, Func<IEnumerable<object>>> repositoryMethods = new Dictionary<string, Func<IEnumerable<object>>>
                    {
                         { "owner_name", () => _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList()) },

                    };

                    ListAttributesActivated = ListAttributesActivated
                        .Select(FKitem =>
                        {
                            if (repositoryMethods.ContainsKey(FKitem.Label.ToLower()))
                            {
                                FKitem.Options = repositoryMethods[FKitem.Label.ToLower()]().ToList();
                            }
                            else
                            {
                                FKitem.Options = new object[0];
                            }

                            return FKitem;
                        })
                        .ToList();

                    objectInst.InstallationAttributes = ListAttributesActivated;
                    objectInst.CivilLoads = _unitOfWork.AttributeActivatedRepository
                     .GetInstAttributeActivatedGetForAdd(TablesNames.TLIcivilLoads.ToString(), null, null, "allLoadInstId", "Dismantle", "SiteCode", "legId",
                         "Leg2Id", "sideArmId", "allCivilInstId", "civilSteelSupportCategoryId").ToList();

                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttRepository
                    .GetDynamicInstAttInst(TableNameEntity.Id, null);
                    return new Response<GetForAddMWDishInstallationObject>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
                }
                else
                {
                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this mwdishlibrary is not found", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                return new Response<GetForAddMWDishInstallationObject>(false, null, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            catch (Exception err)
            {
                return new Response<GetForAddMWDishInstallationObject>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<GetForAddMWDishInstallationObject> GetAttForAddRadioRRUInstallation(int LibraryID, string SiteCode)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x =>
                    x.TableName == "TLIradioRRU");

                GetForAddMWDishInstallationObject objectInst = new GetForAddMWDishInstallationObject();
                List<BaseInstAttViews> ListAttributesActivated = new List<BaseInstAttViews>();

                EditRadioRRULibraryAttributes RadioRRULibrary = _mapper.Map<EditRadioRRULibraryAttributes>(_unitOfWork.RadioRRULibraryRepository
                    .GetIncludeWhereFirst(x => x.Id == LibraryID));
                if (RadioRRULibrary != null)
                {
                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetForAdd(TablesNames.TLIradioRRULibrary.ToString(), RadioRRULibrary, null).ToList();


                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                        .GetLogisticals(TablePartName.Radio.ToString(), Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString(), RadioRRULibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivatedGetForAdd(LoadSubType.TLIradioAntenna.ToString(), null, "Name", "installationPlaceId", "radioAntennaLibraryId", "EquivalentSpace").ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }

                    Dictionary<string, Func<IEnumerable<object>>> repositoryMethods = new Dictionary<string, Func<IEnumerable<object>>>
                    {
                         { "owner_name", () => _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList()) },

                    };

                    ListAttributesActivated = ListAttributesActivated
                        .Select(FKitem =>
                        {
                            if (repositoryMethods.ContainsKey(FKitem.Label.ToLower()))
                            {
                                FKitem.Options = repositoryMethods[FKitem.Label.ToLower()]().ToList();
                            }
                            else
                            {
                                FKitem.Options = new object[0];
                            }

                            return FKitem;
                        })
                        .ToList();

                    objectInst.InstallationAttributes = ListAttributesActivated;
                    objectInst.CivilLoads = _unitOfWork.AttributeActivatedRepository
                     .GetInstAttributeActivatedGetForAdd(TablesNames.TLIcivilLoads.ToString(), null, null, "allLoadInstId", "Dismantle", "SiteCode", "legId",
                         "Leg2Id", "sideArmId", "allCivilInstId", "civilSteelSupportCategoryId").ToList();

                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttRepository
                    .GetDynamicInstAttInst(TableNameEntity.Id, null);
                    return new Response<GetForAddMWDishInstallationObject>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
                }
                else
                {
                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this mwdishlibrary is not found", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                return new Response<GetForAddMWDishInstallationObject>(false, null, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            catch (Exception err)
            {
                return new Response<GetForAddMWDishInstallationObject>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<GetForAddLoadObject> GetRadioAntennaInstallationById(int RadioId, string TableName)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);
                GetForAddLoadObject objectInst = new GetForAddLoadObject();
                List<BaseInstAttViews> Civilload = new List<BaseInstAttViews>();
                List<BaseInstAttViews> Config = new List<BaseInstAttViews>();


                var RadioAntenna = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInstId != null && x.allLoadInst.radioAntennaId == RadioId
                && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilNonSteel, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                x => x.allLoadInst, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioAntenna.radioAntennaLibrary,
                x => x.allLoadInst.radioAntenna.owner, x => x.allLoadInst.radioAntenna.installationPlace
               , x => x.sideArm, x => x.leg);

                if (RadioAntenna != null)
                {
                    EditRadioAntennaLibraryAttributes RadioAntennaLibrary = _mapper.Map<EditRadioAntennaLibraryAttributes>(RadioAntenna.allLoadInst.radioAntenna.radioAntennaLibrary);

                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetLibrary(TablesNames.TLIradioAntennaLibrary.ToString(), RadioAntennaLibrary, null).ToList();


                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                        .GetLogisticals(TablePartName.Radio.ToString(), TablesNames.TLImwDishLibrary.ToString(), RadioAntenna.allLoadInst.radioAntenna.radioAntennaLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    List<BaseInstAttViews> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivatedGetForAdd(TablesNames.TLIradioAntenna.ToString(), RadioAntenna.allLoadInst.radioAntenna
                            ).ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                        NameAttribute.Value = _dbContext.MV_RADIO_ANTENNA_VIEW.FirstOrDefault(x => x.Id == RadioId)?.Name;
                    }
                    var foreignKeyAttributes = ListAttributesActivated.Select(FKitem =>
                    {
                        switch (FKitem.Label.ToLower())
                        {

                            case "owner_name":
                                if (RadioAntenna.allLoadInst.radioAntenna.owner != null)
                                {
                                    FKitem.Value = _mapper.Map<OwnerViewModel>(RadioAntenna.allLoadInst.radioAntenna.owner);
                                    FKitem.Options = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                                }
                                else
                                {
                                    FKitem.Value = new object[0];
                                    FKitem.Options = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                                }
                                break;
                        }
                        return FKitem;
                    }).ToList();
                    var selectedAttributes = ListAttributesActivated
                    .Where(x => new[] { "installationplace_name" }
                                .Contains(x.Label.ToLower()))
                    .ToList();

                    var ExeptAttributes = ListAttributesActivated
                    .Where(x => new[] { "installationplace_name", "radioantennalibrary_name" }
                                .Contains(x.Label.ToLower()))
                    .ToList();
                    var foreignKeyAttribute = selectedAttributes.Select(FKitem =>
                    {
                        switch (FKitem.Label.ToLower())
                        {
                            case "installationplace_name":
                                FKitem.Key = "installationPlaceId";
                                FKitem.Label = "Select Installation Place";
                                FKitem.Value = _mapper.Map<InstallationPlaceViewModel>(RadioAntenna.allLoadInst.radioAntenna.installationPlace);
                                FKitem.Options = _mapper.Map<List<InstallationPlaceViewModel>>(_dbContext.TLIinstallationPlace.ToList());
                                break;


                        }
                        return FKitem;
                    }).ToList();

                    Config.AddRange(foreignKeyAttribute);

                    if (RadioAntenna.allCivilInst != null)
                    {
                        List<SectionsLegTypeViewModel> sectionsLegTypeViewModels = new List<SectionsLegTypeViewModel>
                            {
                                new SectionsLegTypeViewModel { Id = 1, Name = "civilWithoutLeg" },
                                new SectionsLegTypeViewModel { Id = 2, Name = "civilNonSteel" },
                                new SectionsLegTypeViewModel { Id = 0, Name = "civilWithLeg" }
                            };

                        void AddBaseInstAttView(string key, string label, object value, object options, bool Visable)
                        {
                            Config.Add(new BaseInstAttViews
                            {
                                Key = key,
                                Label = label,
                                Value = value,
                                Options = options,
                                DataType = "List",
                                visible = Visable
                            });
                        }

                        void ConfigureView3(string steelTypeKey, SectionsLegTypeViewModel steelTypeValue, string idKey, object idValue, object idOptions)
                        {
                            AddBaseInstAttView("civilSteelType", "Select Civil Steel Type", steelTypeValue, _mapper.Map<List<SectionsLegTypeViewModel>>(sectionsLegTypeViewModels), true);
                            AddBaseInstAttView(idKey, $"Select {steelTypeKey}", _mapper.Map<SectionsLegTypeViewModel>(idValue), _mapper.Map<List<SectionsLegTypeViewModel>>(idOptions), true);
                            AddBaseInstAttView("civilWithoutLegId", "Select Civil Without Leg", null, new object[0], false);
                            AddBaseInstAttView("civilNonSteelId", "Select Civil Non Steel", null, new object[0], false);
                        }
                        void ConfigureView1(string steelTypeKey, SectionsLegTypeViewModel steelTypeValue, string idKey, object idValue, object idOptions)
                        {
                            AddBaseInstAttView("civilSteelType", "Select Civil Steel Type", steelTypeValue, _mapper.Map<List<SectionsLegTypeViewModel>>(sectionsLegTypeViewModels), true);
                            AddBaseInstAttView(idKey, $"Select {steelTypeKey}", _mapper.Map<SectionsLegTypeViewModel>(idValue), _mapper.Map<List<SectionsLegTypeViewModel>>(idOptions), true);
                            AddBaseInstAttView("civilWithLegId", "Select Civil With Leg", null, new object[0], false);

                            AddBaseInstAttView("civilNonSteelId", "Select Civil Non Steel", null, new object[0], false);
                        }
                        void ConfigureView2(string steelTypeKey, SectionsLegTypeViewModel steelTypeValue, string idKey, object idValue, object idOptions)
                        {
                            AddBaseInstAttView("civilSteelType", "Select Civil Steel Type", steelTypeValue, _mapper.Map<List<SectionsLegTypeViewModel>>(sectionsLegTypeViewModels), true);
                            AddBaseInstAttView(idKey, $"Select {steelTypeKey}", _mapper.Map<SectionsLegTypeViewModel>(idValue), _mapper.Map<List<SectionsLegTypeViewModel>>(idOptions), true);
                            AddBaseInstAttView("civilWithLegId", "Select Civil With Leg", null, new object[0], false);
                            AddBaseInstAttView("civilWithoutLegId", "Select Civil Without Leg", null, new object[0], false);

                        }
                        if (RadioAntenna.allCivilInst.civilWithoutLegId != null)
                        {
                            ConfigureView1("civilWithoutLeg", sectionsLegTypeViewModels[0], "civilWithoutLegId", RadioAntenna.allCivilInst.civilWithoutLeg, _dbContext.MV_CIVIL_WITHOUTLEGS_VIEW.Where(x => x.Id == RadioAntenna.allCivilInst.civilWithoutLegId));

                        }
                        else if (RadioAntenna.allCivilInst.civilNonSteelId != null)
                        {
                            ConfigureView2("civilNonSteel", sectionsLegTypeViewModels[1], "civilNonSteelId", RadioAntenna.allCivilInst.civilNonSteel, _dbContext.MV_CIVIL_NONSTEEL_VIEW.Where(x => x.Id == RadioAntenna.allCivilInst.civilNonSteelId));
                        }
                        else if (RadioAntenna.allCivilInst.civilWithLegsId != null)
                        {
                            ConfigureView3("civilWithLeg", sectionsLegTypeViewModels[2], "civilWithLegId", RadioAntenna.allCivilInst.civilWithLegs, _dbContext.MV_CIVIL_WITHLEGS_VIEW.Where(x => x.Id == RadioAntenna.allCivilInst.civilWithLegsId));
                        }
                        if (RadioAntenna.legId != null && RadioAntenna.sideArmId == null)
                        {

                            var Leg1 = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == RadioAntenna.legId);
                            if (Leg1 != null)
                            {
                                List<SectionsLegTypeViewModel> sectionsLegTypeViewModel = new List<SectionsLegTypeViewModel>();
                                sectionsLegTypeViewModel.Add(new SectionsLegTypeViewModel
                                {
                                    Id = Leg1.Id,
                                    Name = Leg1.CiviLegName
                                });

                                BaseInstAttViews baseInstAttViews = new BaseInstAttViews
                                {
                                    Key = "legId",
                                    Value = sectionsLegTypeViewModel,
                                    Label = "Select Leg",
                                    Options = sectionsLegTypeViewModel,
                                    DataType = "list",
                                    visible = false
                                };
                                Config.Add(baseInstAttViews);
                            }

                        }
                        if (RadioAntenna.legId == null && RadioAntenna.sideArmId != null)
                        {
                            List<SectionsLegTypeViewModel> sectionsLegTypeViewModelsidearm = new List<SectionsLegTypeViewModel>();
                            SectionsLegTypeViewModel sectionsLegTypeViewModel = new SectionsLegTypeViewModel()
                            {
                                Id = Convert.ToInt32(RadioAntenna.sideArmId),
                                Name = _dbContext.MV_SIDEARM_VIEW.FirstOrDefault(x => x.Id == RadioAntenna.sideArm.Id)?.Name
                            };
                            BaseInstAttViews baseInstAttViews = new BaseInstAttViews();
                            baseInstAttViews.Key = "SideArmd";
                            baseInstAttViews.Value = sectionsLegTypeViewModel.Id;
                            baseInstAttViews.Label = "leg_name";
                            baseInstAttViews.Options = sectionsLegTypeViewModelsidearm;
                            baseInstAttViews.DataType = "list";
                            Config.Add(baseInstAttViews);
                        }
                        if (RadioAntenna.sideArm == null)
                        {
                            BaseInstAttViews baseInstAttViews = new BaseInstAttViews();
                            baseInstAttViews.Key = "SideArmd";
                            baseInstAttViews.Value = null;
                            baseInstAttViews.Label = "Select SideArm'";
                            baseInstAttViews.Options = new object[0];
                            baseInstAttViews.DataType = "list";
                            baseInstAttViews.visible = false;
                            Config.Add(baseInstAttViews);

                        }
                        if (RadioAntenna.legId == null)
                        {
                            BaseInstAttViews baseInstAttViews = new BaseInstAttViews
                            {
                                Key = "legId",
                                Value = null,
                                Label = "Select Leg",
                                Options = new object[0],
                                DataType = "list",
                                visible = false
                            };
                            Config.Add(baseInstAttViews);
                        }
                        objectInst.installationConfig = Config;
                    }
                    var InstallationDate = new BaseInstAttViews()
                    {
                        Key = "InstallationDate",
                        Value = RadioAntenna.InstallationDate,
                        DataType = "datetime",
                        Label = "InstallationDate",


                    };
                    Civilload.Add(InstallationDate);
                    var ItemOnCivilStatus = new BaseInstAttViews()
                    {
                        Key = "ItemOnCivilStatus",
                        Value = RadioAntenna.ItemOnCivilStatus,
                        DataType = "string",
                        Label = "ItemOnCivilStatus",


                    };
                    Civilload.Add(ItemOnCivilStatus);
                    var ItemStatus = new BaseInstAttViews()
                    {
                        Key = "ItemStatus",
                        Value = RadioAntenna.ItemStatus,
                        DataType = "string",
                        Label = "ItemStatus",


                    };
                    Civilload.Add(ItemStatus);
                    var ReservedSpace = new BaseInstAttViews()
                    {
                        Key = "ReservedSpace",
                        Value = RadioAntenna.ReservedSpace,
                        DataType = "bool",
                        Label = "ReservedSpace",

                    };
                    Civilload.Add(ReservedSpace);


                    objectInst.InstallationAttributes = ListAttributesActivated;
                    objectInst.CivilLoads = Civilload;
                    objectInst.InstallationAttributes = objectInst.InstallationAttributes.Except(ExeptAttributes).ToList();
                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttInstValueRepository.
                        GetDynamicInstAtt(TableNameEntity.Id, RadioId, null);

                    return new Response<GetForAddLoadObject>(false, objectInst, null, null, (int)ApiReturnCode.fail);
                }
                else
                {
                    return new Response<GetForAddLoadObject>(false, null, null, "this id is not found", (int)ApiReturnCode.fail);
                }

                return new Response<GetForAddLoadObject>(true, objectInst, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddLoadObject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<GetEnableAttribute> GetRadioAntennaInstallationWithEnableAtt(string SiteCode, string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_MWDISH";
                    //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    //{
                    //    procedureCommand.CommandType = CommandType.StoredProcedure;
                    //    procedureCommand.ExecuteNonQuery();
                    //}
                    var attActivated = _dbContext.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "RadioAntennaInstallation" &&
                        ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
                        .Select(x => new { attribute = x.AttributeActivated.Key, dynamic = x.DynamicAtt.Key, dataType = x.DynamicAtt != null ? x.DynamicAtt.DataType.Name.ToString() : x.AttributeActivated.DataType.ToString() })
                      .OrderByDescending(x => x.attribute.ToLower().StartsWith("name"))
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
                    propertyNamesStatic.Add("CIVILNAME");
                    propertyNamesStatic.Add("CIVIL_ID");
                    propertyNamesStatic.Add("SIDEARMNAME");
                    propertyNamesStatic.Add("SIDEARM_ID");
                    propertyNamesStatic.Add("ALLCIVILINST_ID");
                    propertyNamesStatic.Add("LEGID");
                    propertyNamesStatic.Add("LEGNAME");

                    if (propertyNamesDynamic.Count == 0)
                    {
                        var query = _dbContext.MV_RADIO_ANTENNA_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();
                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = _dbContext.MV_RADIO_ANTENNA_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                       .GroupBy(x => new
                       {
                           SiteCode = x.SiteCode,
                           Id = x.Id,
                           Name = x.Name,
                           Azimuth = x.Azimuth,
                           Notes = x.Notes,
                           VisibleStatus = x.VisibleStatus,
                           MechanicalTilt = x.MechanicalTilt,
                           ElectricalTilt = x.ElectricalTilt,
                           SerialNumber = x.SerialNumber,
                           HBASurface = x.HBASurface,
                           SpaceInstallation = x.SpaceInstallation,
                           HeightBase = x.HeightBase,
                           HeightLand = x.HeightLand,
                           INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                           RADIOANTENNALIBRARY = x.RADIOANTENNALIBRARY,
                           Dismantle = x.Dismantle,
                           CenterHigh = x.CenterHigh,
                           HBA = x.HBA,
                           HieghFromLand = x.HieghFromLand,
                           EquivalentSpace = x.EquivalentSpace,
                           LEGNAME = x.LEGNAME,
                           CIVILNAME = x.CIVILNAME,
                           CIVIL_ID = x.CIVIL_ID,
                           SIDEARMNAME = x.SIDEARMNAME,
                           SIDEARM_ID = x.SIDEARM_ID,
                           ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                           LEGID = x.LEGID,
                           OWNER = x.OWNER


                       })
                       .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                       .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                        int count = query.Count();

                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                }
                catch (Exception err)
                {
                    return new Response<GetEnableAttribute>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

        }
        public Response<ReturnWithFilters<RadioOtherViewModel>> GetRadioOtherList(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                var RadioOthers = new ReturnWithFilters<RadioOtherViewModel>();
                var Others = _unitOfWork.RadioOtherRepository.GetAllIncludeMultiple(parameters, filters, out count, x => x.installationPlace, x => x.owner, x => x.radioOtherLibrary).ToList();
                RadioOthers.Model = _mapper.Map<List<RadioOtherViewModel>>(Others);
                if (WithFilterData)
                {
                    RadioOthers.filters = _unitOfWork.RadioOtherRepository.GetRelatedTables();
                }
                else
                {
                    RadioOthers.filters = null;
                }
                return new Response<ReturnWithFilters<RadioOtherViewModel>>(true, RadioOthers, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<RadioOtherViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }


        #region AddHistory
        //public void AddHistory(TicketAttributes ticketAtt, int allLoadInstId, string historyType)
        //{
        //    if (ticketAtt != null)
        //    {

        //        AddWorkflowHistoryViewModel workflowhistory = _mapper.Map<AddWorkflowHistoryViewModel>(ticketAtt);
        //        workflowhistory.RecordId = allLoadInstId;
        //        workflowhistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIallLoadInst").Id;
        //        workflowhistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
        //        workflowhistory.UserId = 83;
        //        _unitOfWork.WorkflowHistoryRepository.AddWorkflowHistory(workflowhistory);
        //    }
        //    else
        //    {
        //        AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
        //        history.RecordId = allLoadInstId;
        //        history.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIallLoadInst").Id;
        //        history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
        //        history.UserId = 83;
        //        _unitOfWork.TablesHistoryRepository.AddTableHistory(history);
        //    }
        //}
        #endregion
    }
}
