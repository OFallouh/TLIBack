using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using System.Collections;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.PowerTypeDTOs;
using TLIS_DAL.ViewModels.RuleDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_DAL.ViewModels.WorkflowHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;
using Microsoft.EntityFrameworkCore;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using AutoMapper;

namespace TLIS_Service.Services
{
    public class PowerService : IPowerService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private readonly ApplicationDbContext _dbContext;
        private IMapper _mapper;
        public PowerService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext dbContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        //Function take 2 parameters 
        //get table name Entity by table name
        //Check business rules
        //add new power
        //add new record to TLIcivilLoads table to specify the power install on civil or side arm
        //add dynamic installation attributes value
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

            PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
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
        public string CheckDependencyValidation(object Input, string SiteCode)
        {
            string MainTableName = TablesNames.TLIpower.ToString();
            AddPowerViewModel AddInstallationViewModel = _mapper.Map<AddPowerViewModel>(Input);

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
        public Response<ObjectInstAtts> AddPower(AddPowerViewModel PowerViewModel, string SiteCode, string ConnectionString, int TaskId)
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
                            var PowerEntity = _mapper.Map<TLIpower>(PowerViewModel);
                            if (PowerViewModel.TLIcivilLoads.ReservedSpace == true)
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivil(PowerViewModel.TLIcivilLoads.allCivilInstId).Message;
                                if (Message != "Success")
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            var powerLibrary = _dbContext.TLIpowerLibrary.Where(x => x.Id == PowerViewModel.powerLibraryId).FirstOrDefault();
                            if (PowerEntity.CenterHigh == 0 )
                            {
                                PowerEntity.CenterHigh = PowerEntity.HBA + powerLibrary.Length / 2;
                            }
                            string CheckGeneralValidation = CheckGeneralValidationFunction(PowerViewModel.TLIdynamicAttInstValue, TablesNames.TLIpower.ToString());

                            if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                            string CheckDependencyValidation = this.CheckDependencyValidation(PowerViewModel, SiteCode);

                            if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);
                           
                            var CheckSerialNumber = _unitOfWork.PowerRepository.GetWhereFirst(x => x.SerialNumber == PowerEntity.SerialNumber);
                            if (CheckSerialNumber != null)
                            {
                                return new Response<ObjectInstAtts>(true, null, null, $"The serial number {PowerEntity.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                            }
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == LoadSubType.TLIpower.ToString());
                            TLIsideArm SideArmEntity = null;
                            if (PowerViewModel.TLIcivilLoads.sideArmId != null)
                            {
                                SideArmEntity = _unitOfWork.SideArmRepository.GetByID((int)PowerViewModel.TLIcivilLoads.sideArmId);
                                PowerEntity.Name = SideArmEntity.Name + " " + PowerEntity.Height;
                            }
                            else if (PowerViewModel.TLIcivilLoads.legId != null)
                            {
                                var LegEntity = _unitOfWork.LegRepository
                                    .GetWhereFirst(x => x.Id == PowerViewModel.TLIcivilLoads.legId.Value);

                                PowerEntity.Name = LegEntity.CiviLegName + " " + PowerEntity.Height;
                            }
                            else
                            {
                                return new Response<ObjectInstAtts>(true, null, null, $"Power Can Only Be Installed Either on SideArm or on Civil Leg", (int)ApiReturnCode.fail);
                            }

                            TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                               !x.allLoadInst.Draft && (x.allLoadInst.powerId != null ? x.allLoadInst.power.Name.ToLower() == PowerEntity.Name.ToLower() : false) : false) &&
                               x.SiteCode.ToLower() == SiteCode.ToLower(),
                                   x => x.allLoadInst, x => x.allLoadInst.power);

                            if (CheckName != null)
                                return new Response<ObjectInstAtts>(true, null, null, $"This name {PowerEntity.Name} is already exists", (int)ApiReturnCode.fail);

                            if (PowerEntity.installationPlaceId != null)
                            {
                                TLIinstallationPlace InstallationPlaceEntity = null;
                                InstallationPlaceEntity = _unitOfWork.InstallationPlaceRepository.GetByID((int)PowerEntity.installationPlaceId);
                                if (InstallationPlaceEntity.Name.ToLower() == "direct")
                                {
                                    if (PowerViewModel.TLIcivilLoads.allCivilInstId == 0 || PowerViewModel.TLIcivilLoads.sideArmId != null)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, "The power should installed on civil if installation place is direct", (int)ApiReturnCode.fail);
                                    }
                                }
                                else if (InstallationPlaceEntity.Name.ToLower() == "sidearm")
                                {
                                    if (PowerViewModel.TLIcivilLoads.allCivilInstId == 0 || PowerViewModel.TLIcivilLoads.sideArmId == null)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, "The power should installed on civil by SideArm if installation place is SideArm", (int)ApiReturnCode.fail);
                                    }


                                }
                            }
                            _unitOfWork.PowerRepository.Add(PowerEntity);
                            _unitOfWork.SaveChanges();
                            var Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIpower.ToString(), PowerEntity.Id);
                            _unitOfWork.CivilLoadsRepository.AddCivilLoad(PowerViewModel.TLIcivilLoads, Id, SiteCode);
                            if (PowerViewModel.TLIdynamicAttInstValue.Count > 0)
                            {
                                foreach (var DynamicAttInstValue in PowerViewModel.TLIdynamicAttInstValue)
                                {
                                    _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, PowerEntity.Id);
                                }
                            }
                            //AddHistory(PowerViewModel.ticketAtt, Id, "Insert");

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
        //get table name Entity by table name
        //get activated attributes
        //get dynamic attributes
        //get related tables
        public Response<ObjectInstAtts> GetAttForAdd(int Id, string SiteCode)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l =>
                    l.TableName == LoadSubType.TLIpower.ToString());

                string TableName = TableNameEntity.TableName;

                ObjectInstAtts objectInst = new ObjectInstAtts();

                PowerLibraryViewModel PowerLibrary = _mapper.Map<PowerLibraryViewModel>(_unitOfWork.PowerLibraryRepository.GetByID(Id));

                List<BaseAttView> LibraryAttributeActivated = _unitOfWork.AttributeActivatedRepository.
                    GetAttributeActivated(TablesNames.TLIpowerLibrary.ToString(), PowerLibrary).ToList();

                List<BaseAttView> AddToLibraryAttributesActivated = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                    .GetLogistical(TablePartName.Power.ToString(), TablesNames.TLIpowerLibrary.ToString(), PowerLibrary.Id).ToList());

                LibraryAttributeActivated.AddRange(AddToLibraryAttributesActivated);

                objectInst.LibraryActivatedAttributes = LibraryAttributeActivated;

                List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                    .GetInstAttributeActivated(TableName, null, "Name","powerLibraryId", "installationPlaceId"/*, "EquivalentSpace"*/).ToList();

                BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                if (NameAttribute != null)
                {
                    BaseInstAttView Swap = ListAttributesActivated[0];
                    ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                    ListAttributesActivated[0] = NameAttribute;
                }
                foreach (BaseInstAttView FKitem in ListAttributesActivated)
                {
                    if (FKitem.Desc.ToLower() == "tlipowertype")
                        FKitem.Value = _mapper.Map<List<PowerTypeViewModel>>(_unitOfWork.PowerTypeRepository.GetWhere(x => !x.Disable && !x.Delete).ToList());

                    else if (FKitem.Desc.ToLower() == "tliowner")
                        FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Disable && !x.Deleted).ToList());
                }

                objectInst.AttributesActivated = ListAttributesActivated;

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

                objectInst.RelatedTables = _unitOfWork.CivilLoadsRepository.GetRelatedTables(SiteCode);

                objectInst.CivilLoads = _unitOfWork.AttributeActivatedRepository
                    .GetInstAttributeActivated(TablesNames.TLIcivilLoads.ToString(), null, "allLoadInstId", "Dismantle", "SiteCode", "legId", "Leg2Id",
                        "civilSteelSupportCategoryId", "sideArmId", "allCivilInstId");

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
                                if (item.mwBUId != null && LoadName == TablesNames.TLImwBU.ToString())
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
                                else if (item.mwDishId != null && LoadName == TablesNames.TLImwDish.ToString())
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
                                else if (item.mwODUId != null && LoadName == TablesNames.TLImwODU.ToString())
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
                                else if (item.mwRFUId != null && LoadName == TablesNames.TLImwRFU.ToString())
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
                                else if (item.mwOtherId != null && LoadName == TablesNames.TLImwOther.ToString())
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
                                else if (item.radioAntennaId != null && LoadName == TablesNames.TLIradioAntenna.ToString())
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
                                else if (item.radioRRUId != null && LoadName == TablesNames.TLIradioRRU.ToString())
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
                                else if (item.radioOtherId != null && LoadName == TablesNames.TLIradioOther.ToString())
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
                                else if (item.powerId != null && LoadName == TablesNames.TLIpower.ToString())
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
                                else if (item.loadOtherId != null && LoadName == TablesNames.TLIloadOther.ToString())
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

                                if (item.mwBUId != null && LoadName == TablesNames.TLImwBU.ToString())
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
                                else if (item.mwDishId != null && LoadName == TablesNames.TLImwDish.ToString())
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
                                else if (item.mwODUId != null && LoadName == TablesNames.TLImwODU.ToString())
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
                                else if (item.mwRFUId != null && LoadName == TablesNames.TLImwRFU.ToString())
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
                                else if (item.mwOtherId != null && LoadName == TablesNames.TLImwOther.ToString())
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
                                else if (item.radioAntennaId != null && LoadName == TablesNames.TLIradioAntenna.ToString())
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
                                else if (item.radioRRUId != null && LoadName == TablesNames.TLIradioRRU.ToString())
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
                                else if (item.radioOtherId != null && LoadName == TablesNames.TLIradioOther.ToString())
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
                                else if (item.powerId != null && LoadName == TablesNames.TLIpower.ToString())
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
                                else if (item.loadOtherId != null && LoadName == TablesNames.TLIloadOther.ToString())
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
                                if (item.mwBUId != null && LoadName == TablesNames.TLImwBU.ToString())
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
                                else if (item.mwDishId != null && LoadName == TablesNames.TLImwDish.ToString())
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
                                else if (item.mwODUId != null && LoadName == TablesNames.TLImwODU.ToString())
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
                                else if (item.mwRFUId != null && LoadName == TablesNames.TLImwRFU.ToString())
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
                                else if (item.mwOtherId != null && LoadName == TablesNames.TLImwOther.ToString())
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
                                else if (item.radioAntennaId != null && LoadName == TablesNames.TLIradioAntenna.ToString())
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
                                else if (item.radioRRUId != null && LoadName == TablesNames.TLIradioRRU.ToString())
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
                                else if (item.radioOtherId != null && LoadName == TablesNames.TLIradioOther.ToString())
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
                                else if (item.powerId != null && LoadName == TablesNames.TLIpower.ToString())
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
                                else if (item.loadOtherId != null && LoadName == TablesNames.TLIloadOther.ToString())
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
        //Function take 1 parameter
        //Update power
        //Update dynamic installation attribute values 
        public async Task<Response<ObjectInstAtts>> EditPower(EditPowerViewModel PowerViewModel, int TaskId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                        x.allLoadInst.powerId == PowerViewModel.Id : false), x => x.allLoadInst);

                    string SiteCode = "";

                    if (CivilLoads != null)
                        SiteCode = CivilLoads.SiteCode;

                    else
                        SiteCode = null;

                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && x.allLoadInst.powerId != PowerViewModel.Id && (x.allLoadInstId != null ?
                        !x.allLoadInst.Draft && (x.allLoadInst.powerId != null ? x.allLoadInst.power.Name.ToLower() == PowerViewModel.Name.ToLower() : false) : false) &&
                        x.SiteCode.ToLower() == SiteCode.ToLower(),
                            x => x.allLoadInst, x => x.allLoadInst.power);

                   
                    string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(PowerViewModel.DynamicInstAttsValue, TablesNames.TLIpower.ToString());

                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                    string CheckDependencyValidation = CheckDependencyValidationEditVersion(PowerViewModel, SiteCode);

                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                    int TableName = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLIpower.ToString().ToLower()).Id;

                    TLIpower PowerEntity = _mapper.Map<TLIpower>(PowerViewModel);
                    TLIpower PowerInst = _unitOfWork.PowerRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == PowerViewModel.Id);
                    if (PowerEntity.HBA == PowerInst.HBA && PowerEntity.CenterHigh == PowerInst.CenterHigh && PowerEntity.SpaceInstallation == PowerInst.SpaceInstallation && PowerEntity.Azimuth != PowerInst.Azimuth && PowerViewModel.TLIcivilLoads.ReservedSpace == true)
                    {
                        var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(PowerViewModel.TLIcivilLoads.allCivilInstId, 0, PowerEntity.Azimuth, PowerEntity.CenterHigh).Message;
                        if (message != "Success")
                        {
                            return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                        }
                    }
                    if (PowerEntity.HBA != PowerInst.HBA || PowerEntity.CenterHigh != PowerInst.CenterHigh || PowerEntity.SpaceInstallation != PowerInst.SpaceInstallation)
                    {
                        var allload = _dbContext.TLIallLoadInst.Where(x => x.mwBUId == PowerEntity.Id).Select(x => x.Id).FirstOrDefault();
                        var powerLibrary = _dbContext.TLIpowerLibrary.Where(x => x.Id == PowerEntity.powerLibraryId).FirstOrDefault();
                        if (PowerEntity.CenterHigh == 0 || PowerEntity.CenterHigh == null)
                        {
                            PowerEntity.CenterHigh = PowerEntity.HBA + powerLibrary.Length / 2;
                        }
                        var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(PowerViewModel.TLIcivilLoads.allCivilInstId, 0, PowerEntity.Azimuth, PowerEntity.CenterHigh).Message;
                        if (message != "Success")
                        {
                            return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                        }
                        if (PowerViewModel.TLIcivilLoads.ReservedSpace == true && (PowerViewModel.TLIcivilLoads.sideArmId == null || PowerViewModel.TLIcivilLoads.sideArmId == 0))
                        {
                            PowerEntity.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(PowerViewModel.TLIcivilLoads.allCivilInstId, TablesNames.TLIpower.ToString(), PowerEntity.SpaceInstallation, PowerEntity.CenterHigh, PowerEntity.powerLibraryId, PowerEntity.HBA).Data;
                        }
                    }
                    _unitOfWork.PowerRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, PowerInst, PowerEntity);

                    var allloads = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.powerId == PowerViewModel.Id).Id;
                    var civilloads = _unitOfWork.CivilLoadsRepository.GetWhereFirst(x => x.allLoadInstId == allloads);
                    CivilLoads.InstallationDate = PowerViewModel.TLIcivilLoads.InstallationDate;
                    CivilLoads.ItemOnCivilStatus = PowerViewModel.TLIcivilLoads.ItemOnCivilStatus;
                    CivilLoads.ItemStatus = PowerViewModel.TLIcivilLoads.ItemStatus;
                    CivilLoads.ReservedSpace = PowerViewModel.TLIcivilLoads.ReservedSpace;
                    CivilLoads.sideArmId = PowerViewModel.TLIcivilLoads.sideArmId;
                    CivilLoads.allCivilInstId = PowerViewModel.TLIcivilLoads.allCivilInstId;
                    CivilLoads.legId = PowerViewModel.TLIcivilLoads.legId;
                    CivilLoads.Leg2Id = PowerViewModel.TLIcivilLoads.Leg2Id;

                    _unitOfWork.SaveChanges();

                    if (PowerViewModel.DynamicInstAttsValue != null ? PowerViewModel.DynamicInstAttsValue.Count > 0 : false)
                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(PowerViewModel.DynamicInstAttsValue, TableName, PowerEntity.Id);

                    await _unitOfWork.SaveChangesAsync();
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
        public string CheckDependencyValidationEditVersion(object Input, string SiteCode)
        {
            string MainTableName = TablesNames.TLIpower.ToString();
            EditPowerViewModel EditInstallationViewModel = _mapper.Map<EditPowerViewModel>(Input);

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
        //Function take 2 parameters
        //loop on each dynamic attribute and update value
        private void UpdateDynamicAttInstValue(List<BaseInstAttView> DynamicInstAttsValue, int LoadInstId)
        {
            Parallel.ForEach(DynamicInstAttsValue, DynamicInstAttValue =>
            {
                var DAIV = _unitOfWork.DynamicAttInstValueRepository.GetWhereFirst(d => d.InventoryId == LoadInstId && d.DynamicAttId == DynamicInstAttValue.Id);
                if (DAIV.ValueBoolean != null)
                {
                    DAIV.ValueBoolean = (bool)DynamicInstAttValue.Value;
                    DAIV.Value = DynamicInstAttValue.Value.ToString();
                }
                else if (DAIV.ValueDateTime != null)
                {
                    DAIV.ValueDateTime = (DateTime)DynamicInstAttValue.Value;
                    DAIV.Value = DynamicInstAttValue.Value.ToString();
                }
                else if (DAIV.ValueDouble != null)
                {
                    DAIV.ValueDouble = (double)DynamicInstAttValue.Value;
                    DAIV.Value = DynamicInstAttValue.Value.ToString();
                }
                else if (!string.IsNullOrEmpty(DAIV.ValueString))
                {
                    DAIV.ValueString = DynamicInstAttValue.Value.ToString();
                    DAIV.Value = DynamicInstAttValue.Value.ToString();
                }
                _unitOfWork.DynamicAttInstValueRepository.Update(DAIV);
            });
        }

        public Response<ObjectInstAttsForSideArm> GetById(int Id)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == LoadSubType.TLIpower.ToString());
                ObjectInstAttsForSideArm objectInst = new ObjectInstAttsForSideArm();

                TLIpower Power = _unitOfWork.PowerRepository
                    .GetIncludeWhereFirst(x => x.Id == Id, x => x.owner, x => x.installationPlace, x => x.powerType, x => x.powerLibrary);

                PowerLibraryViewModel powerlib = _mapper.Map<PowerLibraryViewModel>(_unitOfWork.PowerLibraryRepository
                    .GetIncludeWhereFirst(x => x.Id == Power.powerLibraryId));

                List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                   .GetAttributeActivated(TablesNames.TLIpowerLibrary.ToString(), powerlib, null).ToList();

                foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                {
                    if (LibraryAttribute.DataType.ToLower() == "list")
                    {
                        LibraryAttribute.Value = powerlib.GetType().GetProperties()
                            .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(powerlib);
                    }
                }
                List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                    .GetLogistical(TablePartName.Power.ToString(), TablesNames.TLIpowerLibrary.ToString(), powerlib.Id).ToList());

                LibraryAttributes.AddRange(LogisticalAttributes);
                objectInst.LibraryActivatedAttributes = LibraryAttributes;

                List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                    .GetInstAttributeActivated(LoadSubType.TLIpower.ToString(), Power, "installationPlaceId").ToList();

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
                        if (Power.owner == null)
                            FKitem.Value = "NA";

                        else
                            FKitem.Value = Power.owner.OwnerName;
                    }
                    else if (FKitem.Desc.ToLower() == "tlipowertype")
                    {
                        if (Power.powerType == null)
                            FKitem.Value = "NA";

                        else
                            FKitem.Value = Power.powerType.Name;
                    }
                    else if (FKitem.Desc.ToLower() == "tlipowerlibrary")
                    {
                        if (Power.powerLibrary == null)
                            FKitem.Value = "NA";

                        else
                            FKitem.Value = Power.powerLibrary.Model;
                    }
                }

                objectInst.AttributesActivated = ListAttributesActivated;

                objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                    .GetDynamicInstAtts(TableNameEntity.Id, Id, null);

                TLIallLoadInst AllLoadInst = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.powerId == Id);

                TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInstId == AllLoadInst.Id, x => x.sideArm,
                    x => x.site, x => x.leg, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                    x => x.allCivilInst.civilNonSteel, x => x.civilSteelSupportCategory, x => x.allLoadInst.power);

                List<KeyValuePair<string, List<DropDownListFilters>>> PowerRelatedTables = _unitOfWork.PowerRepository
                    .GetRelatedTables();

                List<KeyValuePair<string, List<DropDownListFilters>>> CivilLoadsRelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
                if (CivilLoads != null)
                    CivilLoadsRelatedTables = _unitOfWork.CivilLoadsRepository
                        .GetRelatedTables(CivilLoads.SiteCode);

                PowerRelatedTables.AddRange(CivilLoadsRelatedTables);

                if (CivilLoads.allCivilInst.civilWithLegsId != null)
                {
                    List<TLIleg> LegsForCivilWithLegLibrary = _unitOfWork.LegRepository
                        .GetWhere(x => x.CivilWithLegInstId == CivilLoads.allCivilInst.civilWithLegsId).ToList();

                    List<DropDownListFilters> LegIds = _mapper.Map<List<DropDownListFilters>>(LegsForCivilWithLegLibrary);

                    PowerRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg1Id", LegIds));

                    List<TLIleg> Legs2ForCivilWithLegLibrary = LegsForCivilWithLegLibrary.Except(LegsForCivilWithLegLibrary
                        .Where(x => x.Id == CivilLoads.legId)).ToList();

                    List<DropDownListFilters> Leg2Ids = _mapper.Map<List<DropDownListFilters>>(Legs2ForCivilWithLegLibrary);

                    PowerRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg2Id", Leg2Ids));
                }

                objectInst.RelatedTables = PowerRelatedTables;

                List<BaseAttView> LoadInstAttributes = _unitOfWork.AttributeActivatedRepository
                    .GetAttributeActivated(TablesNames.TLIcivilLoads.ToString(), CivilLoads, null, "allLoadInstId",
                        "Dismantle", "SiteCode", "civilSteelSupportCategoryId", "legId", "Leg2Id",
                            "sideArmId", "allCivilInstId").ToList();

                foreach (BaseAttView FKitem in LoadInstAttributes)
                {
                    if (FKitem.Desc.ToLower() == "tlisite")
                    {
                        FKitem.Value = CivilLoads.site.SiteName;
                    }
                }
                objectInst.CivilLoads = _mapper.Map<IEnumerable<BaseInstAttView>>(LoadInstAttributes);

                List<BaseInstAttView> PowerInstallationInfo = new List<BaseInstAttView>();
                if (CivilLoads != null)
                {
                    TLIallCivilInst AllCivilInst = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhereFirst(x => (x.allLoadInstId != null ? (x.allLoadInst.powerId != null ?
                            x.allLoadInst.powerId.Value == Id : false) : false) && !x.Dismantle, x => x.allCivilInst, x => x.allLoadInst).allCivilInst;

                    if (AllCivilInst.civilWithLegsId != null)
                    {
                        PowerInstallationInfo.Add(new BaseInstAttView
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
                        PowerInstallationInfo.Add(new BaseInstAttView
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
                        PowerInstallationInfo.Add(new BaseInstAttView
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
                            Value = CivilLoads.sideArmId != null ?
                                "SideArm" : ((CivilLoads.legId != null && CivilLoads.legId != 0) ? "Leg" : "Direct")
                        });
                        if (CivilLoads.sideArmId != null)
                        {
                            PowerInstallationInfo.Add(new BaseInstAttView
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
                        else if (CivilLoads.legId != null && CivilLoads.legId != 0)
                        {
                            PowerInstallationInfo.Add(new BaseInstAttView
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
                    }
                    else if (AllCivilInst.civilWithoutLegId != null)
                    {
                        PowerInstallationInfo.Add(new BaseInstAttView
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
                            Value = "Civil without Legs"
                        });
                        PowerInstallationInfo.Add(new BaseInstAttView
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
                        PowerInstallationInfo.Add(new BaseInstAttView
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
                            Value = CivilLoads.sideArmId != null ?
                                "SideArm" : "Direct"
                        });
                        if (CivilLoads.sideArmId != null)
                        {
                            PowerInstallationInfo.Add(new BaseInstAttView
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
                    }
                    else if (AllCivilInst.civilNonSteelId != null)
                    {
                        PowerInstallationInfo.Add(new BaseInstAttView
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
                        PowerInstallationInfo.Add(new BaseInstAttView
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
                            Value = _unitOfWork.CivilNonSteelRepository.GetByID(AllCivilInst.civilNonSteelId.Value).Name
                        });
                        PowerInstallationInfo.Add(new BaseInstAttView
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
                            Value = CivilLoads.sideArmId != null ?
                                "SideArm" : "Direct"
                        });
                        if (CivilLoads.sideArmId != null)
                        {
                            PowerInstallationInfo.Add(new BaseInstAttView
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
                    }
                    PowerInstallationInfo.Add(new BaseInstAttView
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
                    objectInst.SideArmInstallationInfo = PowerInstallationInfo;
                }

                return new Response<ObjectInstAttsForSideArm>(true, objectInst, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAttsForSideArm>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<PowerViewModel>> GetList(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                ReturnWithFilters<PowerViewModel> Powers = new ReturnWithFilters<PowerViewModel>();
                Powers.Model = _mapper.Map<List<PowerViewModel>>(_unitOfWork.PowerRepository.GetAllIncludeMultiple(parameters, filters, out count, x => x.installationPlace, x => x.owner, x => x.powerLibrary, x => x.powerType).ToList());
                if (WithFilterData == true)
                {
                    Powers.filters = _unitOfWork.PowerRepository.GetRelatedTables();
                }
                else
                {
                    Powers.filters = null;
                }
                return new Response<ReturnWithFilters<PowerViewModel>>(true, Powers, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<PowerViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<List<PowerTypeViewModel>> GetPowerTypes()
        {
            try
            {
                List<PowerTypeViewModel> PowerTypes = _mapper.Map<List<PowerTypeViewModel>>(_unitOfWork.PowerTypeRepository
                    .GetWhere(x => !x.Delete && !x.Disable).ToList());

                return new Response<List<PowerTypeViewModel>>(true, PowerTypes, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<PowerTypeViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
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
