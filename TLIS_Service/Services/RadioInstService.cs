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
        public Response<ObjectInstAtts> AddRadioInstallation(object RadioInstallationViewModel, string TableName, string SiteCode, string ConnectionString,int TaskId)
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
                                AddRadioAntennaViewModel RadioAntennaModel = _mapper.Map<AddRadioAntennaViewModel>(RadioInstallationViewModel);
                                TLIradioAntenna RadioAntennaEntity = _mapper.Map<TLIradioAntenna>(RadioAntennaModel);
                                if (RadioAntennaModel.TLIcivilLoads.ReservedSpace == true)
                                {
                                    var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivil(RadioAntennaModel.TLIcivilLoads.allCivilInstId).Message;
                                    if (Message != "Success")
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                                var radioAntennaLibrary = _dbContext.TLIradioAntennaLibrary.Where(x => x.Id == RadioAntennaModel.radioAntennaLibraryId).FirstOrDefault();
                                if (RadioAntennaEntity.CenterHigh == 0 )
                                {
                                    RadioAntennaEntity.CenterHigh = RadioAntennaEntity.HBA + radioAntennaLibrary.Length / 2;
                                }
                                var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(RadioAntennaModel.TLIcivilLoads.allCivilInstId, 0, ((float)RadioAntennaEntity.Azimuth), RadioAntennaEntity.CenterHigh).Message;
                                if (message != "Success")
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, message, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                bool test = true;

                                string CheckDependencyValidation = CheckDependencyValidationForRadioTypes(RadioInstallationViewModel, TableName, SiteCode);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(RadioAntennaModel.TLIdynamicAttInstValue, TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                test = true;
                                if (test == true)
                                {
                                    //TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                    //    !x.allLoadInst.Draft && (x.allLoadInst.radioAntennaId != null ? x.allLoadInst.radioAntenna.Name.ToLower() == RadioAntennaEntity.Name.ToLower() : false) : false),
                                    //        x => x.allLoadInst, x => x.allLoadInst.radioAntenna);
                                    //if (CheckName != null)
                                    //    return new Response<ObjectInstAtts>(true, null, null, $"This name {RadioAntennaEntity.Name} is already exists", (int)ApiReturnCode.fail);

                                    var CheckSerialNumber = _unitOfWork.RadioAntennaRepository.GetWhereFirst(x => x.SerialNumber == RadioAntennaEntity.SerialNumber);
                                    if (CheckSerialNumber != null)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, $"The serial number {RadioAntennaEntity.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                    }

                                    TLIsideArm SideArmEntity = new TLIsideArm();

                                    if (RadioAntennaModel.TLIcivilLoads.sideArmId != null)
                                    {
                                        SideArmEntity = _unitOfWork.SideArmRepository.GetByID((int)RadioAntennaModel.TLIcivilLoads.sideArmId);
                                        RadioAntennaEntity.Name = SideArmEntity.Name + " " + RadioAntennaModel.HeightBase + " " + RadioAntennaModel.Azimuth;
                                    }
                                    else if (RadioAntennaModel.TLIcivilLoads.legId != null || RadioAntennaModel.TLIcivilLoads.Leg2Id != null)
                                    {
                                        var LegName = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == RadioAntennaModel.TLIcivilLoads.legId || x.Id == RadioAntennaModel.TLIcivilLoads.Leg2Id).CiviLegName;
                                        RadioAntennaEntity.Name = LegName + " " + RadioAntennaModel.HeightBase + " " + RadioAntennaModel.Azimuth;
                                    }
                                    else
                                    {
                                        RadioAntennaEntity.Name = RadioAntennaModel.HeightBase + " " + RadioAntennaModel.Azimuth;
                                    }

                                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                      !x.allLoadInst.Draft && (x.allLoadInst.radioAntennaId != null ? x.allLoadInst.radioAntenna.Name.ToLower() == RadioAntennaEntity.Name.ToLower() : false) : false) &&
                                      x.SiteCode.ToLower() == SiteCode.ToLower(),
                                          x => x.allLoadInst, x => x.allLoadInst.radioAntenna);
                                    if (CheckName != null)
                                        return new Response<ObjectInstAtts>(true, null, null, $"This name {RadioAntennaEntity.Name} is already exists", (int)ApiReturnCode.fail);

                                    TLIinstallationPlace InstallationPlaceEntity = null;
                                    if (RadioAntennaModel.installationPlaceId != null)
                                    {
                                        InstallationPlaceEntity = _unitOfWork.InstallationPlaceRepository.GetByID((int)RadioAntennaModel.installationPlaceId);
                                        if (InstallationPlaceEntity.Name.ToLower() == "direct")
                                        {
                                            if (RadioAntennaModel.TLIcivilLoads.allCivilInstId == 0 || RadioAntennaModel.TLIcivilLoads.sideArmId != null)
                                            {
                                                return new Response<ObjectInstAtts>(true, null, null, "The antenna should be on civil only", (int)ApiReturnCode.fail);
                                            }
                                        }
                                        else if (InstallationPlaceEntity.Name.ToLower() == "sidearm")
                                        {
                                            if (RadioAntennaModel.TLIcivilLoads.allCivilInstId == 0 || RadioAntennaModel.TLIcivilLoads.sideArmId == null)
                                            {
                                                return new Response<ObjectInstAtts>(true, null, null, "The antenna should be on civil by sidearm", (int)ApiReturnCode.fail);
                                            }
                                        }
                                    }
                                    _unitOfWork.RadioAntennaRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, RadioAntennaEntity);
                                    _unitOfWork.SaveChanges();
                                    int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIradioAntenna.ToString(), RadioAntennaEntity.Id);
                                    _unitOfWork.CivilLoadsRepository.AddCivilLoad(RadioAntennaModel.TLIcivilLoads, Id, SiteCode);
                                    if (RadioAntennaModel.TLIdynamicAttInstValue.Count > 0)
                                    {
                                        foreach (var DynamicAttInstValue in RadioAntennaModel.TLIdynamicAttInstValue)
                                        {
                                            _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, RadioAntennaEntity.Id);
                                        }

                                    }
                                    //AddHistory(RadioAntennaModel.ticketAtt, Id, "Insert");
                                }
                                else
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
                                }
                            }
                            else if (LoadSubType.TLIradioRRU.ToString() == TableName)
                            {
                                var RadioRRuModel = _mapper.Map<AddRadioRRUViewModel>(RadioInstallationViewModel);
                                var RadioRRuEntity = _mapper.Map<TLIRadioRRU>(RadioRRuModel);
                                if (RadioRRuModel.TLIcivilLoads.ReservedSpace == true)
                                {
                                    var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivil(RadioRRuModel.TLIcivilLoads.allCivilInstId).Message;
                                    if (Message != "Success")
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                                var radioRRULibrary = _dbContext.TLIradioRRULibrary.Where(x => x.Id == RadioRRuModel.radioRRULibraryId).FirstOrDefault();
                                if (RadioRRuEntity.CenterHigh == 0)
                                {
                                    RadioRRuEntity.CenterHigh = RadioRRuEntity.HBA + radioRRULibrary.Length / 2;
                                }
                                var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(RadioRRuModel.TLIcivilLoads.allCivilInstId, 0, RadioRRuEntity.Azimuth, RadioRRuEntity.CenterHigh).Message;
                                if (message != "Success")
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, message, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                bool test = true;
                                string CheckDependencyValidation = CheckDependencyValidationForRadioTypes(RadioInstallationViewModel, TableName, SiteCode);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(RadioRRuModel.TLIdynamicAttInstValue, TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);


                                if (test == true)
                                {
                                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                        !x.allLoadInst.Draft && (x.allLoadInst.radioRRUId != null ? x.allLoadInst.radioRRU.Name.ToLower() == RadioRRuEntity.Name.ToLower() : false) : false) &&
                                        x.SiteCode.ToLower() == SiteCode.ToLower(),
                                            x => x.allLoadInst, x => x.allLoadInst.radioRRU);
                                    if (CheckName != null)
                                        return new Response<ObjectInstAtts>(true, null, null, $"This name {RadioRRuEntity.Name} is already exists", (int)ApiReturnCode.fail);

                                    var CheckSerialNumber = _unitOfWork.RadioRRURepository.GetWhereFirst(x => x.SerialNumber == RadioRRuEntity.SerialNumber);
                                    if (CheckSerialNumber != null)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, $"The serial number {RadioRRuEntity.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                    }

                                    //TLIsideArm SideArmEntity = new TLIsideArm();

                                    //if (RadioRRuModel.TLIcivilLoads.sideArmId != null)
                                    //{
                                    //    SideArmEntity = _unitOfWork.SideArmRepository.GetByID((int)RadioRRuModel.TLIcivilLoads.sideArmId);
                                    //    RadioRRuEntity.Name = SideArmEntity.Name + " " + RadioRRuModel.HeightBase + " " + RadioRRuModel.Azimuth;
                                    //}
                                    //else
                                    //{
                                    //    RadioRRuEntity.Name = RadioRRuModel.HeightBase + " " + RadioRRuModel.Azimuth;

                                    //}
                                    TLIinstallationPlace InstallationPlaceEntity = null;
                                    if (RadioRRuModel.installationPlaceId != null)
                                    {
                                        InstallationPlaceEntity = _unitOfWork.InstallationPlaceRepository.GetByID((int)RadioRRuModel.installationPlaceId);
                                        if (InstallationPlaceEntity.Name.ToLower() == "direct")
                                        {
                                            if (RadioRRuModel.TLIcivilLoads.allCivilInstId == 0 || RadioRRuModel.TLIcivilLoads.sideArmId != null)
                                            {
                                                return new Response<ObjectInstAtts>(true, null, null, "The RRU should be on civil only", (int)ApiReturnCode.fail);
                                            }
                                        }
                                        else if (InstallationPlaceEntity.Name.ToLower() == "sidearm")
                                        {
                                            if (RadioRRuModel.TLIcivilLoads.allCivilInstId == 0 || RadioRRuModel.TLIcivilLoads.sideArmId == null)
                                            {
                                                return new Response<ObjectInstAtts>(true, null, null, "The RRU should be on civil by sidearm", (int)ApiReturnCode.fail);
                                            }
                                        }
                                    }
                                    _unitOfWork.RadioRRURepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, RadioRRuEntity);
                                    _unitOfWork.SaveChanges();
                                    int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIradioRRU.ToString(), RadioRRuEntity.Id);
                                    _unitOfWork.CivilLoadsRepository.AddCivilLoad(RadioRRuModel.TLIcivilLoads, Id, SiteCode);
                                    if (RadioRRuModel.TLIdynamicAttInstValue.Count > 0)
                                    {
                                        foreach (var DynamicAttInstValue in RadioRRuModel.TLIdynamicAttInstValue)
                                        {
                                            _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, RadioRRuEntity.Id);
                                        }
                                    }
                                    //AddHistory(RadioRRuModel.ticketAtt, Id, "Insert");
                                }
                                else
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
                                }
                            }
                            else if (LoadSubType.TLIradioOther.ToString() == TableName)
                            {
                                var RadioOtherModel = _mapper.Map<AddRadioOtherViewModel>(RadioInstallationViewModel);
                                var RadioOtherEntity = _mapper.Map<TLIradioOther>(RadioOtherModel);
                                if (RadioOtherModel.TLIcivilLoads.ReservedSpace == true)
                                {
                                    var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivil(RadioOtherModel.TLIcivilLoads.allCivilInstId).Message;
                                    if (Message != "Success")
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                                var radioOtherLibrary = _dbContext.TLIradioOtherLibrary.Where(x => x.Id == RadioOtherModel.radioOtherLibraryId).FirstOrDefault();
                                if (RadioOtherEntity.CenterHigh == 0)
                                {
                                    RadioOtherEntity.CenterHigh = RadioOtherEntity.HBA + radioOtherLibrary.Length / 2;
                                }
                                bool test = true;
                                string CheckDependencyValidation = CheckDependencyValidationForRadioTypes(RadioInstallationViewModel, TableName, SiteCode);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(RadioOtherModel.TLIdynamicAttInstValue, TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                if (test == true)
                                {
                                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                        !x.allLoadInst.Draft && (x.allLoadInst.radioOtherId != null ? x.allLoadInst.radioOther.Name.ToLower() == RadioOtherEntity.Name.ToLower() : false) : false) &&
                                        x.SiteCode.ToLower() == SiteCode.ToLower(),
                                            x => x.allLoadInst, x => x.allLoadInst.radioOther);
                                    if (CheckName != null)
                                        return new Response<ObjectInstAtts>(true, null, null, $"This name {RadioOtherEntity.Name} is already exists", (int)ApiReturnCode.fail);

                                    var CheckSerialNumber = _unitOfWork.RadioOtherRepository.GetWhereFirst(x => x.SerialNumber == RadioOtherEntity.SerialNumber);
                                    if (CheckSerialNumber != null)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, $"The serial number {RadioOtherEntity.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                    }

                                    TLIinstallationPlace InstallationPlaceEntity = null;
                                    if (RadioOtherModel.installationPlaceId != null)
                                    {
                                        InstallationPlaceEntity = _unitOfWork.InstallationPlaceRepository.GetByID((int)RadioOtherModel.installationPlaceId);
                                        if (InstallationPlaceEntity.Name.ToLower() == "direct")
                                        {
                                            if (RadioOtherModel.TLIcivilLoads.allCivilInstId == 0 || RadioOtherModel.TLIcivilLoads.sideArmId != null)
                                            {
                                                return new Response<ObjectInstAtts>(true, null, null, "The RRU should be on civil only", (int)ApiReturnCode.fail);
                                            }
                                        }
                                        else if (InstallationPlaceEntity.Name.ToLower() == "sidearm")
                                        {
                                            if (RadioOtherModel.TLIcivilLoads.allCivilInstId == 0 || RadioOtherModel.TLIcivilLoads.sideArmId == null)
                                            {
                                                return new Response<ObjectInstAtts>(true, null, null, "The RRU should be on civil by sidearm", (int)ApiReturnCode.fail);
                                            }
                                        }
                                    }
                                    _unitOfWork.RadioOtherRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, RadioOtherEntity);
                                    _unitOfWork.SaveChanges();
                                    int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIradioOther.ToString(), RadioOtherEntity.Id);
                                    _unitOfWork.CivilLoadsRepository.AddCivilLoad(RadioOtherModel.TLIcivilLoads, Id, SiteCode);
                                    if (RadioOtherModel.TLIdynamicAttInstValue.Count > 0)
                                    {
                                        foreach (var DynamicAttInstValue in RadioOtherModel.TLIdynamicAttInstValue)
                                        {
                                            _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, RadioOtherEntity.Id);
                                        }
                                    }
                                    //AddHistory(RadioOtherModel.ticketAtt, Id, "Insert");
                                }
                                else
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
                                }
                            }
                            transaction.Complete();
                            tran.Commit();
                            return new Response<ObjectInstAtts>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)ApiReturnCode.fail);
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
        public async Task<Response<ObjectInstAtts>> EditRadioInstallation(object RadioInstallationViewModel, string TableName, int TaskId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (LoadSubType.TLIradioAntenna.ToString().ToLower() == TableName.ToLower())
                    {
                        int TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLIradioAntenna.ToString().ToLower()).Id;
                        EditRadioAntennaViewModel RadioAntennaModel = _mapper.Map<EditRadioAntennaViewModel>(RadioInstallationViewModel);

                        TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                            x.allLoadInst.radioAntennaId == RadioAntennaModel.Id : false), x => x.allLoadInst);

                        string SiteCode = "";

                        if (CivilLoads != null)
                            SiteCode = CivilLoads.SiteCode;

                        else
                            SiteCode = null;

                        TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && x.allLoadInst.radioAntennaId != RadioAntennaModel.Id && (x.allLoadInstId != null ?
                            !x.allLoadInst.Draft && (x.allLoadInst.radioAntennaId != null ?
                                (x.allLoadInst.radioAntenna.Name.ToLower() == RadioAntennaModel.Name.ToLower() &&
                                    x.allLoadInst.radioAntennaId != RadioAntennaModel.Id) : false) : false) &&
                                    x.SiteCode.ToLower() == SiteCode.ToLower(),
                                x => x.allLoadInst, x => x.allLoadInst.radioAntenna);
                        if (CheckName != null)
                            return new Response<ObjectInstAtts>(true, null, null, $"This name [{RadioAntennaModel.Name}] is already exists", (int)ApiReturnCode.fail);

                        string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(RadioAntennaModel.DynamicInstAttsValue, TableName);

                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                        string CheckDependencyValidation = CheckDependencyValidationEditVersion(RadioInstallationViewModel, SiteCode, TableName);

                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                        TLIradioAntenna RadioAntennaEntity = _mapper.Map<TLIradioAntenna>(RadioAntennaModel);
                        var OldRadioAntenna = _unitOfWork.RadioAntennaRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == RadioAntennaModel.Id);
                        if (RadioAntennaEntity.HBA == OldRadioAntenna.HBA && RadioAntennaEntity.CenterHigh == OldRadioAntenna.CenterHigh && RadioAntennaEntity.SpaceInstallation == OldRadioAntenna.SpaceInstallation && RadioAntennaEntity.Azimuth != OldRadioAntenna.Azimuth && RadioAntennaModel.TLIcivilLoads.ReservedSpace == true)
                        {
                            var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(RadioAntennaModel.TLIcivilLoads.allCivilInstId, OldRadioAntenna.Id, RadioAntennaEntity.Azimuth, RadioAntennaEntity.CenterHigh).Message;
                            if (message != "Success")
                            {
                                return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                            }
                        }
                        if (RadioAntennaEntity.HBA != OldRadioAntenna.HBA || RadioAntennaEntity.CenterHigh != OldRadioAntenna.CenterHigh || RadioAntennaEntity.SpaceInstallation != OldRadioAntenna.SpaceInstallation && RadioAntennaModel.TLIcivilLoads.ReservedSpace == true)
                        {
                            var radioAntennaLibrary = _dbContext.TLIradioAntennaLibrary.Where(x => x.Id == RadioAntennaEntity.radioAntennaLibraryId).FirstOrDefault();
                            if (RadioAntennaEntity.CenterHigh == 0 || RadioAntennaEntity.CenterHigh == null)
                            {
                                RadioAntennaEntity.CenterHigh = RadioAntennaEntity.HBA + radioAntennaLibrary.Length / 2;
                            }
                            var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(RadioAntennaModel.TLIcivilLoads.allCivilInstId, OldRadioAntenna.Id, RadioAntennaEntity.Azimuth, RadioAntennaEntity.CenterHigh).Message;
                            if (message != "Success")
                            {
                                return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                            }
                            if (RadioAntennaModel.TLIcivilLoads.ReservedSpace == true && (RadioAntennaModel.TLIcivilLoads.sideArmId == null || RadioAntennaModel.TLIcivilLoads.sideArmId == 0))
                            {
                                RadioAntennaEntity.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(RadioAntennaModel.TLIcivilLoads.allCivilInstId, TableName, RadioAntennaEntity.SpaceInstallation, RadioAntennaEntity.CenterHigh, RadioAntennaEntity.radioAntennaLibraryId, RadioAntennaEntity.HBA).Data;
                            }
                        }
                        _unitOfWork.RadioAntennaRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldRadioAntenna, RadioAntennaEntity);
                        await _unitOfWork.SaveChangesAsync();
                        var allloads = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.radioAntennaId == RadioAntennaModel.Id).Id;
                        var civilloads = _unitOfWork.CivilLoadsRepository.GetWhereFirst(x => x.allLoadInstId == allloads);
                        CivilLoads.InstallationDate = RadioAntennaModel.TLIcivilLoads.InstallationDate;
                        CivilLoads.ItemOnCivilStatus = RadioAntennaModel.TLIcivilLoads.ItemOnCivilStatus;
                        CivilLoads.ItemStatus = RadioAntennaModel.TLIcivilLoads.ItemStatus;
                        CivilLoads.ReservedSpace = RadioAntennaModel.TLIcivilLoads.ReservedSpace;
                        CivilLoads.sideArmId = RadioAntennaModel.TLIcivilLoads.sideArmId;
                        CivilLoads.allCivilInstId = RadioAntennaModel.TLIcivilLoads.allCivilInstId;
                        CivilLoads.legId = RadioAntennaModel.TLIcivilLoads.legId;
                        CivilLoads.Leg2Id = RadioAntennaModel.TLIcivilLoads.Leg2Id;

                        _unitOfWork.SaveChanges();
                        if (RadioAntennaModel.DynamicInstAttsValue != null ? RadioAntennaModel.DynamicInstAttsValue.Count > 0 : false)
                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(RadioAntennaModel.DynamicInstAttsValue, TableNameId, RadioAntennaEntity.Id);

                    }
                    else if (LoadSubType.TLIradioRRU.ToString().ToLower() == TableName.ToLower())
                    {
                        int TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLIradioRRU.ToString().ToLower()).Id;
                        EditRadioRRUViewModel RadioRRUModel = _mapper.Map<EditRadioRRUViewModel>(RadioInstallationViewModel);

                        TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                            x.allLoadInst.radioRRUId == RadioRRUModel.Id : false), x => x.allLoadInst);

                        string SiteCode = "";

                        if (CivilLoads != null)
                            SiteCode = CivilLoads.SiteCode;

                        else
                            SiteCode = null;

                        TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                            !x.allLoadInst.Draft && (x.allLoadInst.radioRRUId != null ?
                                (x.allLoadInst.radioRRU.Name.ToLower() == RadioRRUModel.Name.ToLower() && x.allLoadInst.radioRRUId != RadioRRUModel.Id) : false) : false) &&
                                x.SiteCode.ToLower() == SiteCode.ToLower(),
                                x => x.allLoadInst, x => x.allLoadInst.radioRRU);

                        if (CheckName != null)
                            return new Response<ObjectInstAtts>(true, null, null, $"This name [{RadioRRUModel.Name}] is already exists", (int)ApiReturnCode.fail);

                        string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(RadioRRUModel.DynamicInstAttsValue, TableName);

                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                        string CheckDependencyValidation = CheckDependencyValidationEditVersion(RadioInstallationViewModel, SiteCode, TableName);

                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                        TLIRadioRRU RadioRRUEntity = _mapper.Map<TLIRadioRRU>(RadioRRUModel);

                        TLIRadioRRU OldRadioRRU = _unitOfWork.RadioRRURepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == RadioRRUModel.Id);
                        if (RadioRRUEntity.HBA == OldRadioRRU.HBA && RadioRRUEntity.CenterHigh == OldRadioRRU.CenterHigh && RadioRRUEntity.SpaceInstallation == OldRadioRRU.SpaceInstallation && RadioRRUEntity.Azimuth != OldRadioRRU.Azimuth && RadioRRUModel.TLIcivilLoads.ReservedSpace == true)
                        {
                            var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(RadioRRUModel.TLIcivilLoads.allCivilInstId, OldRadioRRU.Id, RadioRRUEntity.Azimuth, RadioRRUEntity.CenterHigh).Message;
                            if (message != "Success")
                            {
                                return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                            }
                        }
                        if (RadioRRUEntity.HBA != OldRadioRRU.HBA || RadioRRUEntity.CenterHigh != OldRadioRRU.CenterHigh || RadioRRUEntity.SpaceInstallation != OldRadioRRU.SpaceInstallation && RadioRRUModel.TLIcivilLoads.ReservedSpace == true)
                        {
                            var radioRRULibrar = _dbContext.TLIradioRRULibrary.Where(x => x.Id == RadioRRUEntity.radioRRULibraryId).FirstOrDefault();
                            if (RadioRRUEntity.CenterHigh == 0 || RadioRRUEntity.CenterHigh == null)
                            {
                                RadioRRUEntity.CenterHigh = RadioRRUEntity.HBA + radioRRULibrar.Length / 2;
                            }
                            var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(RadioRRUModel.TLIcivilLoads.allCivilInstId, OldRadioRRU.Id, RadioRRUEntity.Azimuth, RadioRRUEntity.CenterHigh).Message;
                            if (message != "Success")
                            {
                                return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                            }
                            if (RadioRRUModel.TLIcivilLoads.ReservedSpace == true && (RadioRRUModel.TLIcivilLoads.sideArmId == null || RadioRRUModel.TLIcivilLoads.sideArmId == 0))
                            {
                                RadioRRUEntity.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(RadioRRUModel.TLIcivilLoads.allCivilInstId, TableName, RadioRRUEntity.SpaceInstallation, RadioRRUEntity.CenterHigh, RadioRRUEntity.radioRRULibraryId, RadioRRUEntity.HBA).Data;
                            }
                        }
                        _unitOfWork.RadioRRURepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldRadioRRU, RadioRRUEntity);
                        await _unitOfWork.SaveChangesAsync();
                        var allloads = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.radioRRUId == RadioRRUModel.Id).Id;
                        var civilloads = _unitOfWork.CivilLoadsRepository.GetWhereFirst(x => x.allLoadInstId == allloads);
                        CivilLoads.InstallationDate = RadioRRUModel.TLIcivilLoads.InstallationDate;
                        CivilLoads.ItemOnCivilStatus = RadioRRUModel.TLIcivilLoads.ItemOnCivilStatus;
                        CivilLoads.ItemStatus = RadioRRUModel.TLIcivilLoads.ItemStatus;
                        CivilLoads.ReservedSpace = RadioRRUModel.TLIcivilLoads.ReservedSpace;
                        CivilLoads.sideArmId = RadioRRUModel.TLIcivilLoads.sideArmId;
                        CivilLoads.allCivilInstId = RadioRRUModel.TLIcivilLoads.allCivilInstId;
                        CivilLoads.legId = RadioRRUModel.TLIcivilLoads.legId;
                        CivilLoads.Leg2Id = RadioRRUModel.TLIcivilLoads.Leg2Id;

                        _unitOfWork.SaveChanges();
                        if (RadioRRUModel.DynamicInstAttsValue != null ? RadioRRUModel.DynamicInstAttsValue.Count > 0 : false)
                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(RadioRRUModel.DynamicInstAttsValue, TableNameId, RadioRRUEntity.Id);
                    }
                    else if (LoadSubType.TLIradioOther.ToString().ToLower() == TableName.ToLower())
                    {
                        int TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLIradioOther.ToString().ToLower()).Id;
                        EditRadioOtherViewModel RadioOtherModel = _mapper.Map<EditRadioOtherViewModel>(RadioInstallationViewModel);

                        TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                            x.allLoadInst.radioOtherId == RadioOtherModel.Id : false), x => x.allLoadInst);

                        string SiteCode = "";

                        if (CivilLoads != null)
                            SiteCode = CivilLoads.SiteCode;

                        else
                            SiteCode = null;

                        TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                            !x.allLoadInst.Draft && (x.allLoadInst.radioOtherId != null ?
                                (x.allLoadInst.radioOther.Name.ToLower() == RadioOtherModel.Name.ToLower() && x.allLoadInst.radioOtherId != RadioOtherModel.Id) : false) : false) &&
                                x.SiteCode.ToLower() == SiteCode.ToLower(),
                                x => x.allLoadInst, x => x.allLoadInst.radioOther);

                        if (CheckName != null)
                            return new Response<ObjectInstAtts>(true, null, null, $"This name [{RadioOtherModel.Name}] is already exists", (int)ApiReturnCode.fail);

                        string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(RadioOtherModel.DynamicInstAttsValue, TableName);

                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                        string CheckDependencyValidation = CheckDependencyValidationEditVersion(RadioInstallationViewModel, SiteCode, TableName);

                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                        TLIradioOther RadioOtherEntity = _mapper.Map<TLIradioOther>(RadioOtherModel);

                        var OldRadioOther = _unitOfWork.RadioOtherRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == RadioOtherModel.Id);
                        if (RadioOtherEntity.HBA != OldRadioOther.HBA || RadioOtherEntity.CenterHigh != OldRadioOther.CenterHigh || RadioOtherEntity.Spaceinstallation != OldRadioOther.Spaceinstallation && RadioOtherModel.TLIcivilLoads.ReservedSpace == true)
                        {
                            var radioOtherLibrary = _dbContext.TLIradioOtherLibrary.Where(x => x.Id == RadioOtherEntity.radioOtherLibraryId).FirstOrDefault();
                            if (RadioOtherEntity.CenterHigh == 0 || RadioOtherEntity.CenterHigh == null)
                            {
                                RadioOtherEntity.CenterHigh = RadioOtherEntity.HBA + radioOtherLibrary.Length / 2;
                            }
                            if (RadioOtherModel.TLIcivilLoads.ReservedSpace == true && (RadioOtherModel.TLIcivilLoads.sideArmId == null || RadioOtherModel.TLIcivilLoads.sideArmId == 0))
                            {
                                RadioOtherEntity.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(RadioOtherModel.TLIcivilLoads.allCivilInstId, TableName, RadioOtherEntity.Spaceinstallation, RadioOtherEntity.CenterHigh, RadioOtherEntity.radioOtherLibraryId, RadioOtherEntity.HBA).Data;
                            }
                        }
                        _unitOfWork.RadioOtherRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldRadioOther, RadioOtherEntity);
                        await _unitOfWork.SaveChangesAsync();
                        var allloads = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.radioOtherId == RadioOtherModel.Id).Id;
                        var civilloads = _unitOfWork.CivilLoadsRepository.GetWhereFirst(x => x.allLoadInstId == allloads);
                        CivilLoads.InstallationDate = RadioOtherModel.TLIcivilLoads.InstallationDate;
                        CivilLoads.ItemOnCivilStatus = RadioOtherModel.TLIcivilLoads.ItemOnCivilStatus;
                        CivilLoads.ItemStatus = RadioOtherModel.TLIcivilLoads.ItemStatus;
                        CivilLoads.ReservedSpace = RadioOtherModel.TLIcivilLoads.ReservedSpace;
                        CivilLoads.sideArmId = RadioOtherModel.TLIcivilLoads.sideArmId;
                        CivilLoads.allCivilInstId = RadioOtherModel.TLIcivilLoads.allCivilInstId;
                        CivilLoads.legId = RadioOtherModel.TLIcivilLoads.legId;
                        CivilLoads.Leg2Id = RadioOtherModel.TLIcivilLoads.Leg2Id;

                        _unitOfWork.SaveChanges();
                        if (RadioOtherModel.DynamicInstAttsValue != null ? RadioOtherModel.DynamicInstAttsValue.Count > 0 : false)
                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(RadioOtherModel.DynamicInstAttsValue, TableNameId, RadioOtherEntity.Id);
                    }
                    transaction.Complete();
                    return new Response<ObjectInstAtts>();
                }
                catch (Exception err)
                {
                    return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)ApiReturnCode.fail);
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
        public Response<bool> DismantleLoads(string sitecode, int LoadId, string LoadName, int TaskId)
        {
            try
            {
                double? Freespace = 0;
                double? EquivalentSpace = 0;
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
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
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
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;

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
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;

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
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
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
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
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
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
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
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
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
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
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
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
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
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
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
                }
                return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception er)
            {

                return new Response<bool>(false, false, null, er.Message, (int)Helpers.Constants.ApiReturnCode.fail);
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
