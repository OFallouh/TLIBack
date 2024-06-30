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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using TLIS_DAL.ViewModels.DependencyDTOs;
using static TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs.EditRadioAntennaLibraryObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.PowerLibraryDTOs;
using static TLIS_DAL.ViewModels.PowerLibraryDTOs.EditPowerLibraryObject;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.SectionsLegTypeDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;

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
        public Response<ObjectInstAtts> AddPower(AddPowerViewModel PowerViewModel, string SiteCode, string ConnectionString, int? TaskId)
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
                                    return new Response<ObjectInstAtts>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                                }
                            }
                            else
                            {
                                _unitOfWork.SaveChanges();
                                transaction.Complete();
                            }
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
        public Response<bool> DismantleLoads(string sitecode, int LoadId, string LoadName, int? TaskId, int UserId, string connectionString)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    if (LoadName == Helpers.Constants.TablesNames.TLImwDish.ToString())
                    {
                        var DishLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInst.mwDishId
                        == LoadId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.mwDish
                        , x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                        if (DishLoad != null)
                        {
                            var ODULoad = _unitOfWork.CivilLoadsRepository.GetWhereAndInclude(x => x.allLoadInst.mwODU.Mw_DishId ==
                            DishLoad.allLoadInst.mwDishId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower()
                            && x.allCivilInstId == DishLoad.allCivilInstId, x => x.allLoadInst).ToList();

                            if (ODULoad != null && ODULoad.Count > 0)
                                return new Response<bool>(true, false, null, "can not dismantle this dish because found loaed on it", (int)ApiReturnCode.fail);

                            var MWBULoadSdDish = _unitOfWork.CivilLoadsRepository.GetWhereAndInclude(x => x.allLoadInst.mwBU.SdDishId ==
                            DishLoad.allLoadInst.mwDishId
                            && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst).ToList();

                            if (MWBULoadSdDish != null && MWBULoadSdDish.Count > 0)
                                return new Response<bool>(true, false, null, "can not dismantle this dish because found loaed on it", (int)ApiReturnCode.fail);

                            var MWBULoadMainDish = _unitOfWork.CivilLoadsRepository.GetWhereAndInclude(x => x.allLoadInst.mwBU.MainDishId
                            == DishLoad.allLoadInst.mwDishId
                              && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.mwBU);

                            if (MWBULoadMainDish != null && MWBULoadMainDish.Count > 0)
                                return new Response<bool>(true, false, null, "can not dismantle this dish because found loaed on it", (int)ApiReturnCode.fail);



                            DishLoad.Dismantle = true;
                            var OldDishLoad = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable()
                                  .AsNoTracking().FirstOrDefault(x => x.Id == DishLoad.Id);

                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, OldDishLoad, DishLoad);
                            _unitOfWork.SaveChanges();
                            if (DishLoad.ReservedSpace == true)
                            {
                                if (DishLoad.allCivilInst.civilWithLegsId != null)
                                {
                                    DishLoad.allCivilInst.civilWithLegs.CurrentLoads -= DishLoad.allLoadInst.mwDish.EquivalentSpace;
                                    var OldCivilWithlegs = _unitOfWork.CivilWithLegsRepository.GetAllAsQueryable()
                                     .AsNoTracking().FirstOrDefault(x => x.Id == DishLoad.allCivilInst.civilWithLegsId);

                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldCivilWithlegs, DishLoad.allCivilInst.civilWithLegs);
                                    _unitOfWork.SaveChanges();
                                }
                                else if (DishLoad.allCivilInst.civilWithoutLegId != null)
                                {
                                    DishLoad.allCivilInst.civilWithoutLeg.CurrentLoads -= DishLoad.allLoadInst.mwDish.EquivalentSpace;
                                    var OldCivilWithoutLegs = _unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable()
                                    .AsNoTracking().FirstOrDefault(x => x.Id == DishLoad.allCivilInst.civilWithoutLegId);

                                    _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldCivilWithoutLegs, DishLoad.allCivilInst.civilWithoutLeg);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            return new Response<bool>(true, false, null, "this item is not found", (int)ApiReturnCode.fail);
                        }
                    }
                    else if (LoadName == Helpers.Constants.TablesNames.TLImwODU.ToString())
                    {
                        var ODULoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInst.mwODUId
                        == LoadId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.mwODU
                        , x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                        if (ODULoad != null)
                        {
                            ODULoad.Dismantle = true;

                            var OldODULoad = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable()
                               .AsNoTracking().FirstOrDefault(x => x.Id == ODULoad.Id);

                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, OldODULoad, ODULoad);
                            _unitOfWork.SaveChanges();
                            if (ODULoad.ReservedSpace == true)
                            {
                                if (ODULoad.allCivilInst.civilWithLegsId != null)
                                {
                                    ODULoad.allCivilInst.civilWithLegs.CurrentLoads -= ODULoad.allLoadInst.mwODU.EquivalentSpace;
                                    var OldCivilWithlegs = _unitOfWork.CivilWithLegsRepository.GetAllAsQueryable()
                                     .AsNoTracking().FirstOrDefault(x => x.Id == ODULoad.allCivilInst.civilWithLegsId);

                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldCivilWithlegs, ODULoad.allCivilInst.civilWithLegs);
                                    _unitOfWork.SaveChanges();
                                }
                                else if (ODULoad.allCivilInst.civilWithoutLegId != null)
                                {
                                    ODULoad.allCivilInst.civilWithoutLeg.CurrentLoads -= ODULoad.allLoadInst.mwODU.EquivalentSpace;
                                    var OldCivilWithoutLegs = _unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable()
                                    .AsNoTracking().FirstOrDefault(x => x.Id == ODULoad.allCivilInst.civilWithoutLegId);

                                    _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldCivilWithoutLegs, ODULoad.allCivilInst.civilWithoutLeg);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            return new Response<bool>(true, false, null, "this item is not found", (int)ApiReturnCode.fail);
                        }
                    }
                    else if (LoadName == Helpers.Constants.TablesNames.TLIradioRRU.ToString())
                    {
                        var RadioRRULoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInst.radioRRUId
                        == LoadId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.radioRRU
                        , x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                        if (RadioRRULoad != null)
                        {
                            var RadioAntenna = _unitOfWork.CivilLoadsRepository.GetWhereAndInclude(x => x.allLoadInst.radioRRUId
                            == LoadId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower() && x.allLoadInst.radioAntennaId != null
                            , x => x.allLoadInst).ToList();

                            if (RadioAntenna != null && RadioAntenna.Count > 0)
                                return new Response<bool>(true, false, null, "can not dismantle this radio because found loaed on it", (int)ApiReturnCode.fail);
                            foreach (var item in RadioAntenna)
                            {
                                _unitOfWork.AllLoadInstRepository.RemoveItemWithHistory(UserId, item.allLoadInst);
                                _unitOfWork.SaveChanges();
                            }

                            RadioRRULoad.Dismantle = true;
                            var OldORadioRRULoad = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable()
                               .AsNoTracking().FirstOrDefault(x => x.Id == RadioRRULoad.Id);

                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, OldORadioRRULoad, RadioRRULoad);
                            _unitOfWork.SaveChanges();
                            if (RadioRRULoad.ReservedSpace == true)
                            {
                                if (RadioRRULoad.allCivilInst.civilWithLegsId != null)
                                {
                                    RadioRRULoad.allCivilInst.civilWithLegs.CurrentLoads -= RadioRRULoad.allLoadInst.radioRRU.EquivalentSpace;
                                    var OldCivilWithlegs = _unitOfWork.CivilWithLegsRepository.GetAllAsQueryable()
                                     .AsNoTracking().FirstOrDefault(x => x.Id == RadioRRULoad.allCivilInst.civilWithLegsId);

                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldCivilWithlegs, RadioRRULoad.allCivilInst.civilWithLegs);
                                    _unitOfWork.SaveChanges();
                                }
                                else if (RadioRRULoad.allCivilInst.civilWithoutLegId != null)
                                {
                                    RadioRRULoad.allCivilInst.civilWithoutLeg.CurrentLoads -= RadioRRULoad.allLoadInst.radioRRU.EquivalentSpace;
                                    var OldCivilWithoutLegs = _unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable()
                                    .AsNoTracking().FirstOrDefault(x => x.Id == RadioRRULoad.allCivilInst.civilWithoutLegId);

                                    _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldCivilWithoutLegs, RadioRRULoad.allCivilInst.civilWithoutLeg);
                                    _unitOfWork.SaveChanges();
                                }
                            }

                        }
                        else
                        {
                            return new Response<bool>(true, false, null, "this item is not found", (int)ApiReturnCode.fail);
                        }
                    }
                    else if (LoadName == Helpers.Constants.TablesNames.TLIpower.ToString())
                    {
                        var PowerLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInst.powerId
                        == LoadId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.power
                        , x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                        if (PowerLoad != null)
                        {
                            PowerLoad.Dismantle = true;

                            var OldOPowerLoad = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable()
                               .AsNoTracking().FirstOrDefault(x => x.Id == PowerLoad.Id);

                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, OldOPowerLoad, PowerLoad);
                            _unitOfWork.SaveChanges();
                            if (PowerLoad.ReservedSpace == true)
                            {
                                if (PowerLoad.allCivilInst.civilWithLegsId != null)
                                {
                                    PowerLoad.allCivilInst.civilWithLegs.CurrentLoads -= PowerLoad.allLoadInst.power.EquivalentSpace;
                                    var OldCivilWithlegs = _unitOfWork.CivilWithLegsRepository.GetAllAsQueryable()
                                     .AsNoTracking().FirstOrDefault(x => x.Id == PowerLoad.allCivilInst.civilWithLegsId);

                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldCivilWithlegs, PowerLoad.allCivilInst.civilWithLegs);
                                    _unitOfWork.SaveChanges();
                                }
                                else if (PowerLoad.allCivilInst.civilWithoutLegId != null)
                                {
                                    PowerLoad.allCivilInst.civilWithoutLeg.CurrentLoads -= PowerLoad.allLoadInst.power.EquivalentSpace;
                                    var OldCivilWithoutLegs = _unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable()
                                    .AsNoTracking().FirstOrDefault(x => x.Id == PowerLoad.allCivilInst.civilWithoutLegId);

                                    _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldCivilWithoutLegs, PowerLoad.allCivilInst.civilWithoutLeg);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            return new Response<bool>(true, false, null, "this item is not found", (int)ApiReturnCode.fail);
                        }
                    }
                    else if (LoadName == Helpers.Constants.TablesNames.TLIloadOther.ToString())
                    {
                        var OtherLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInst.loadOtherId
                        == LoadId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.loadOther
                        , x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                        if (OtherLoad != null)
                        {
                            OtherLoad.Dismantle = true;

                            var OldOtherLoad = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable()
                               .AsNoTracking().FirstOrDefault(x => x.Id == OtherLoad.Id);

                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, OldOtherLoad, OtherLoad);
                            _unitOfWork.SaveChanges();
                            if (OtherLoad.ReservedSpace == true)
                            {
                                if (OtherLoad.allCivilInst.civilWithLegsId != null)
                                {
                                    OtherLoad.allCivilInst.civilWithLegs.CurrentLoads -= OtherLoad.allLoadInst.loadOther.EquivalentSpace;
                                    var OldCivilWithlegs = _unitOfWork.CivilWithLegsRepository.GetAllAsQueryable()
                                     .AsNoTracking().FirstOrDefault(x => x.Id == OtherLoad.allCivilInst.civilWithLegsId);

                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldCivilWithlegs, OtherLoad.allCivilInst.civilWithLegs);
                                    _unitOfWork.SaveChanges();
                                }
                                else if (OtherLoad.allCivilInst.civilWithoutLegId != null)
                                {
                                    OtherLoad.allCivilInst.civilWithoutLeg.CurrentLoads -= OtherLoad.allLoadInst.loadOther.EquivalentSpace;
                                    var OldCivilWithoutLegs = _unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable()
                                    .AsNoTracking().FirstOrDefault(x => x.Id == OtherLoad.allCivilInst.civilWithoutLegId);

                                    _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldCivilWithoutLegs, OtherLoad.allCivilInst.civilWithoutLeg);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            return new Response<bool>(true, false, null, "this item is not found", (int)ApiReturnCode.fail);
                        }
                    }
                    else if (LoadName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString())
                    {
                        var RadioAntennaLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInst.radioAntennaId
                        == LoadId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.radioAntenna
                        , x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                        if (RadioAntennaLoad != null)
                        {
                            var RadioRRuLoad = _unitOfWork.CivilLoadsRepository.GetWhereAndInclude(x =>
                             x.allLoadInst.radioAntennaId == LoadId &&
                             x.allLoadInst.radioRRUId != null &&
                             !x.Dismantle &&
                             x.SiteCode.ToLower() == sitecode.ToLower(),
                             x => x.allLoadInst
                            ).ToList();

                            if (RadioRRuLoad != null && RadioRRuLoad.Count > 0)
                                return new Response<bool>(true, false, null, "can not dismantle this radio because found loaed on it", (int)ApiReturnCode.fail);
                            foreach (var item in RadioRRuLoad)
                            {
                                _unitOfWork.AllLoadInstRepository.RemoveItemWithHistory(UserId, item.allLoadInst);
                                _unitOfWork.SaveChanges();
                            }
                            RadioAntennaLoad.Dismantle = true;

                            var OldOtherLoad = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable()
                               .AsNoTracking().FirstOrDefault(x => x.Id == RadioAntennaLoad.Id);

                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, OldOtherLoad, RadioAntennaLoad);
                            _unitOfWork.SaveChanges();
                            if (RadioAntennaLoad.ReservedSpace == true)
                            {
                                if (RadioAntennaLoad.allCivilInst.civilWithLegsId != null)
                                {
                                    RadioAntennaLoad.allCivilInst.civilWithLegs.CurrentLoads -= RadioAntennaLoad.allLoadInst.radioAntenna.EquivalentSpace;
                                    var OldCivilWithlegs = _unitOfWork.CivilWithLegsRepository.GetAllAsQueryable()
                                     .AsNoTracking().FirstOrDefault(x => x.Id == RadioAntennaLoad.allCivilInst.civilWithLegsId);

                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldCivilWithlegs, RadioAntennaLoad.allCivilInst.civilWithLegs);
                                    _unitOfWork.SaveChanges();
                                }
                                else if (RadioAntennaLoad.allCivilInst.civilWithoutLegId != null)
                                {
                                    RadioAntennaLoad.allCivilInst.civilWithoutLeg.CurrentLoads -= RadioAntennaLoad.allLoadInst.radioAntenna.EquivalentSpace;
                                    var OldCivilWithoutLegs = _unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable()
                                    .AsNoTracking().FirstOrDefault(x => x.Id == RadioAntennaLoad.allCivilInst.civilWithoutLegId);

                                    _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldCivilWithoutLegs, RadioAntennaLoad.allCivilInst.civilWithoutLeg);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            return new Response<bool>(true, false, null, "this item is not found", (int)ApiReturnCode.fail);
                        }
                    }
                    else if (LoadName == Helpers.Constants.TablesNames.TLImwBU.ToString())
                    {
                        var MWBULoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInst.mwBUId
                        == LoadId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.mwBU
                        , x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                        if (MWBULoad != null)
                        {
                            var RadioRFULoad = _unitOfWork.CivilLoadsRepository.GetWhereAndInclude(x =>
                             x.allLoadInst.mwRFU.MwPort.MwBUId == LoadId && !x.Dismantle &&
                             x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.mwRFU);

                            if (RadioRFULoad != null && RadioRFULoad.Count > 0)
                                return new Response<bool>(true, false, null, "can not dismantle this MWBU because found loaed on it", (int)ApiReturnCode.fail);


                            var CascudedMWBU = _unitOfWork.CivilLoadsRepository.GetWhereAndInclude(x => x.allLoadInst.mwBU.PortCascadeId == LoadId
                            && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.mwBU).ToList();

                            if (CascudedMWBU != null && CascudedMWBU.Count > 0)
                                return new Response<bool>(true, false, null, "can not dismantle this MWBU because found loaed on it", (int)ApiReturnCode.fail);


                            var BaseMWBU = _unitOfWork.CivilLoadsRepository.GetWhereAndInclude(x => x.allLoadInst.mwBU.BaseBUId == LoadId
                            && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.mwBU).ToList();

                            if (BaseMWBU != null && BaseMWBU.Count > 0)
                                return new Response<bool>(true, false, null, "can not dismantle this MWBU because found loaed on it", (int)ApiReturnCode.fail);

                            MWBULoad.Dismantle = true;
                            var OldOtherLoad = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable()
                               .AsNoTracking().FirstOrDefault(x => x.Id == MWBULoad.Id);
                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, OldOtherLoad, MWBULoad);
                            _unitOfWork.SaveChanges();

                            if (MWBULoad.ReservedSpace == true)
                            {
                                if (MWBULoad.allCivilInst.civilWithLegsId != null)
                                {
                                    MWBULoad.allCivilInst.civilWithLegs.CurrentLoads -= MWBULoad.allLoadInst.mwBU.EquivalentSpace;
                                    var OldCivilWithlegs = _unitOfWork.CivilWithLegsRepository.GetAllAsQueryable()
                                     .AsNoTracking().FirstOrDefault(x => x.Id == MWBULoad.allCivilInst.civilWithLegsId);

                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldCivilWithlegs, MWBULoad.allCivilInst.civilWithLegs);
                                    _unitOfWork.SaveChanges();
                                }
                                else if (MWBULoad.allCivilInst.civilWithoutLegId != null)
                                {
                                    MWBULoad.allCivilInst.civilWithoutLeg.CurrentLoads -= MWBULoad.allLoadInst.mwBU.EquivalentSpace;
                                    var OldCivilWithoutLegs = _unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable()
                                    .AsNoTracking().FirstOrDefault(x => x.Id == MWBULoad.allCivilInst.civilWithoutLegId);

                                    _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldCivilWithoutLegs, MWBULoad.allCivilInst.civilWithoutLeg);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            return new Response<bool>(true, false, null, "this item is not found", (int)ApiReturnCode.fail);
                        }
                    }
                    else if (LoadName == Helpers.Constants.TablesNames.TLImwRFU.ToString())
                    {
                        var MWRFULoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInst.mwRFUId
                        == LoadId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.mwRFU
                        , x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                        if (MWRFULoad != null)
                        {
                            MWRFULoad.Dismantle = true;

                            var OldMWRFULoad = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable()
                               .AsNoTracking().FirstOrDefault(x => x.Id == MWRFULoad.Id);

                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, OldMWRFULoad, MWRFULoad);
                            _unitOfWork.SaveChanges();
                            if (MWRFULoad.ReservedSpace == true)
                            {
                                if (MWRFULoad.allCivilInst.civilWithLegsId != null)
                                {
                                    MWRFULoad.allCivilInst.civilWithLegs.CurrentLoads -= MWRFULoad.allLoadInst.mwRFU.EquivalentSpace;
                                    var OldCivilWithlegs = _unitOfWork.CivilWithLegsRepository.GetAllAsQueryable()
                                     .AsNoTracking().FirstOrDefault(x => x.Id == MWRFULoad.allCivilInst.civilWithLegsId);

                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldCivilWithlegs, MWRFULoad.allCivilInst.civilWithLegs);
                                    _unitOfWork.SaveChanges();
                                }
                                else if (MWRFULoad.allCivilInst.civilWithoutLegId != null)
                                {
                                    MWRFULoad.allCivilInst.civilWithoutLeg.CurrentLoads -= MWRFULoad.allLoadInst.mwRFU.EquivalentSpace;
                                    var OldCivilWithoutLegs = _unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable()
                                    .AsNoTracking().FirstOrDefault(x => x.Id == MWRFULoad.allCivilInst.civilWithoutLegId);

                                    _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldCivilWithoutLegs, MWRFULoad.allCivilInst.civilWithoutLeg);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            return new Response<bool>(true, false, null, "this item is not found", (int)ApiReturnCode.fail);
                        }
                    }
                    else if (LoadName == Helpers.Constants.TablesNames.TLImwOther.ToString())
                    {
                        var MWOtherLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInst.mwOtherId
                        == LoadId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.mwOther
                        , x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                        if (MWOtherLoad != null)
                        {
                            MWOtherLoad.Dismantle = true;

                            var OldMWOtherLoad = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable()
                               .AsNoTracking().FirstOrDefault(x => x.Id == MWOtherLoad.Id);

                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, OldMWOtherLoad, MWOtherLoad);
                            _unitOfWork.SaveChanges();
                            if (MWOtherLoad.ReservedSpace == true)
                            {
                                if (MWOtherLoad.allCivilInst.civilWithLegsId != null)
                                {
                                    MWOtherLoad.allCivilInst.civilWithLegs.CurrentLoads -= MWOtherLoad.allLoadInst.mwOther.EquivalentSpace;
                                    var OldCivilWithlegs = _unitOfWork.CivilWithLegsRepository.GetAllAsQueryable()
                                     .AsNoTracking().FirstOrDefault(x => x.Id == MWOtherLoad.allCivilInst.civilWithLegsId);

                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldCivilWithlegs, MWOtherLoad.allCivilInst.civilWithLegs);
                                    _unitOfWork.SaveChanges();
                                }
                                else if (MWOtherLoad.allCivilInst.civilWithoutLegId != null)
                                {
                                    MWOtherLoad.allCivilInst.civilWithoutLeg.CurrentLoads -= MWOtherLoad.allLoadInst.mwOther.EquivalentSpace;
                                    var OldCivilWithoutLegs = _unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable()
                                    .AsNoTracking().FirstOrDefault(x => x.Id == MWOtherLoad.allCivilInst.civilWithoutLegId);

                                    _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldCivilWithoutLegs, MWOtherLoad.allCivilInst.civilWithoutLeg);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            return new Response<bool>(true, false, null, "this item is not found", (int)ApiReturnCode.fail);
                        }
                    }
                    else if (LoadName == Helpers.Constants.TablesNames.TLIradioOther.ToString())
                    {
                        var RdioOtherLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInst.radioOtherId
                        == LoadId && !x.Dismantle && x.SiteCode.ToLower() == sitecode.ToLower(), x => x.allLoadInst, x => x.allLoadInst.radioOther
                        , x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                        if (RdioOtherLoad != null)
                        {
                            RdioOtherLoad.Dismantle = true;

                            var OldRdioOtherLoad = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable()
                               .AsNoTracking().FirstOrDefault(x => x.Id == RdioOtherLoad.Id);

                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, OldRdioOtherLoad, RdioOtherLoad);
                            _unitOfWork.SaveChanges();
                            if (RdioOtherLoad.ReservedSpace == true)
                            {
                                if (RdioOtherLoad.allCivilInst.civilWithLegsId != null)
                                {
                                    RdioOtherLoad.allCivilInst.civilWithLegs.CurrentLoads -= RdioOtherLoad.allLoadInst.radioOther.EquivalentSpace;
                                    var OldCivilWithlegs = _unitOfWork.CivilWithLegsRepository.GetAllAsQueryable()
                                     .AsNoTracking().FirstOrDefault(x => x.Id == RdioOtherLoad.allCivilInst.civilWithLegsId);

                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldCivilWithlegs, RdioOtherLoad.allCivilInst.civilWithLegs);
                                    _unitOfWork.SaveChanges();
                                }
                                else if (RdioOtherLoad.allCivilInst.civilWithoutLegId != null)
                                {
                                    RdioOtherLoad.allCivilInst.civilWithoutLeg.CurrentLoads -= RdioOtherLoad.allLoadInst.radioOther.EquivalentSpace;
                                    var OldCivilWithoutLegs = _unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable()
                                    .AsNoTracking().FirstOrDefault(x => x.Id == RdioOtherLoad.allCivilInst.civilWithoutLegId);

                                    _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldCivilWithoutLegs, RdioOtherLoad.allCivilInst.civilWithoutLeg);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            return new Response<bool>(true, false, null, "this item is not found", (int)ApiReturnCode.fail);
                        }
                    }
                    if (TaskId != null)
                    {
                        var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                        var result = Submit.Result;
                        if (result.result == true && result.errorMessage == null)
                        {
                            _unitOfWork.SaveChanges();
                            transactionScope.Complete();
                        }
                        else
                        {
                            transactionScope.Dispose();
                            return new Response<bool>(false, false, null, result.errorMessage.ToString(), (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        _unitOfWork.SaveChanges();
                        transactionScope.Complete();
                    }

                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));


                    return new Response<bool>(true, true, null, null, (int)ApiReturnCode.success);
                }
                catch (Exception err)
                {

                    return new Response<bool>(true, false, null, err.Message, (int)ApiReturnCode.fail);
                }
            }
        }
        //Function take 1 parameter
        //Update power
        //Update dynamic installation attribute values 
        public async Task<Response<ObjectInstAtts>> EditPower(EditPowerViewModel PowerViewModel, int? TaskId)
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
                            return new Response<ObjectInstAtts>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        _unitOfWork.SaveChanges();
                        transaction.Complete();
                    }
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
        public Response<GetForAddMWDishInstallationObject> GetAttForAddPowerInstallation(int LibraryID, string SiteCode)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x =>
                    x.TableName == "TLIpower");

                GetForAddMWDishInstallationObject objectInst = new GetForAddMWDishInstallationObject();
                List<BaseInstAttViews> ListAttributesActivated = new List<BaseInstAttViews>();

                EditPowerLibraryAttributes PowerLibrary = _mapper.Map<EditPowerLibraryAttributes>(_unitOfWork.PowerLibraryRepository
                    .GetIncludeWhereFirst(x => x.Id == LibraryID));
                if (PowerLibrary != null)
                {
                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetForAdd(TablesNames.TLIpowerLibrary.ToString(), PowerLibrary, null).ToList();


                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                        .GetLogisticals(TablePartName.Power.ToString(), Helpers.Constants.TablesNames.TLIpowerLibrary.ToString(), PowerLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivatedGetForAdd(LoadSubType.TLIpower.ToString(), null, "Name", "installationPlaceId", "powerLibraryId", "EquivalentSpace").ToList();

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
        public Response<GetForAddLoadObject> GetPowerInstallationById(int PowerId)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == "TLIpower");
                GetForAddLoadObject objectInst = new GetForAddLoadObject();
                List<BaseInstAttViews> Civilload = new List<BaseInstAttViews>();
                List<BaseInstAttViews> Config = new List<BaseInstAttViews>();


                var Power = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInstId != null && x.allLoadInst.powerId == PowerId
                && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilNonSteel, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                x => x.allLoadInst, x => x.allLoadInst.power, x => x.allLoadInst.power.powerLibrary,
                x => x.allLoadInst.power.owner, x => x.allLoadInst.power.installationPlace
               , x => x.sideArm, x => x.leg);

                if (Power != null)
                {
                    EditPowerLibraryAttributes PowerLibrary = _mapper.Map<EditPowerLibraryAttributes>(Power.allLoadInst.power.powerLibrary);

                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetLibrary(TablesNames.TLIpowerLibrary.ToString(), PowerLibrary, null).ToList();


                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                        .GetLogisticals(TablePartName.Radio.ToString(), TablesNames.TLIpowerLibrary.ToString(), Power.allLoadInst.power.powerLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    List<BaseInstAttViews> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivatedGetForAdd(TablesNames.TLIpower.ToString(), Power.allLoadInst.power
                            ).ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                        NameAttribute.Value = _dbContext.MV_POWER_VIEW.FirstOrDefault(x => x.Id == PowerId)?.Name;
                    }
                    var foreignKeyAttributes = ListAttributesActivated.Select(FKitem =>
                    {
                        switch (FKitem.Label.ToLower())
                        {

                            case "owner_name":
                                if (Power.allLoadInst.power.owner != null)
                                {
                                    FKitem.Value = _mapper.Map<OwnerViewModel>(Power.allLoadInst.power.owner);
                                    FKitem.Options = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                                }
                                else
                                {
                                    FKitem.Value = null;
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
                    .Where(x => new[] { "installationplace_name", "powerlibrary_name" }
                                .Contains(x.Label.ToLower()))
                    .ToList();
                    var foreignKeyAttribute = selectedAttributes.Select(FKitem =>
                    {
                        switch (FKitem.Label.ToLower())
                        {
                            case "installationplace_name":
                                FKitem.Key = "installationPlaceId";
                                FKitem.Label = "Select Installation Place";
                                FKitem.Value = _mapper.Map<InstallationPlaceViewModel>(Power.allLoadInst.power.installationPlace);
                                FKitem.Options = _mapper.Map<List<InstallationPlaceViewModel>>(_dbContext.TLIinstallationPlace.ToList());
                                break;


                        }
                        return FKitem;
                    }).ToList();

                    Config.AddRange(foreignKeyAttribute);

                    if (Power.allCivilInst != null)
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
                        if (Power.allCivilInst.civilWithoutLegId != null)
                        {
                            ConfigureView1("civilWithoutLeg", sectionsLegTypeViewModels[0], "civilWithoutLegId", Power.allCivilInst.civilWithoutLeg, _dbContext.MV_CIVIL_WITHOUTLEGS_VIEW.Where(x => x.Id == Power.allCivilInst.civilWithoutLegId));

                        }
                        else if (Power.allCivilInst.civilNonSteelId != null)
                        {
                            ConfigureView2("civilNonSteel", sectionsLegTypeViewModels[1], "civilNonSteelId", Power.allCivilInst.civilNonSteel, _dbContext.MV_CIVIL_NONSTEEL_VIEW.Where(x => x.Id == Power.allCivilInst.civilNonSteelId));
                        }
                        else if (Power.allCivilInst.civilWithLegsId != null)
                        {
                            ConfigureView3("civilWithLeg", sectionsLegTypeViewModels[2], "civilWithLegId", Power.allCivilInst.civilWithLegs, _dbContext.MV_CIVIL_WITHLEGS_VIEW.Where(x => x.Id == Power.allCivilInst.civilWithLegsId));
                        }
                        if (Power.legId != null)
                        {

                            var Leg1 = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == Power.legId);
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
                                    Value = Leg1.Id,
                                    Label = "Select Leg",
                                    Options = sectionsLegTypeViewModel,
                                    DataType = "list",
                                    visible = false
                                };
                                Config.Add(baseInstAttViews);
                            }

                        }
                        if (Power.sideArmId != null)
                        {
                            List<SectionsLegTypeViewModel> sectionsLegTypeViewModelsidearm = new List<SectionsLegTypeViewModel>();
                            SectionsLegTypeViewModel sectionsLegTypeViewModel = new SectionsLegTypeViewModel()
                            {
                                Id = Convert.ToInt32(Power.sideArmId),
                                Name = _dbContext.MV_SIDEARM_VIEW.FirstOrDefault(x => x.Id == Power.sideArm.Id)?.Name
                            };
                            BaseInstAttViews baseInstAttViews = new BaseInstAttViews();
                            baseInstAttViews.Key = "sideArmId";
                            baseInstAttViews.Value = sectionsLegTypeViewModel;
                            baseInstAttViews.Label = "Select SideArm";
                            baseInstAttViews.Options = sectionsLegTypeViewModelsidearm;
                            baseInstAttViews.DataType = "list";
                            Config.Add(baseInstAttViews);
                        }
                        if (Power.sideArm == null)
                        {
                            BaseInstAttViews baseInstAttViews = new BaseInstAttViews();
                            baseInstAttViews.Key = "SideArmId";
                            baseInstAttViews.Value = null;
                            baseInstAttViews.Label = "Select SideArm";
                            baseInstAttViews.Options = new object[0];
                            baseInstAttViews.DataType = "list";
                            baseInstAttViews.visible = false;
                            Config.Add(baseInstAttViews);

                        }
                        if (Power.legId == null)
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
                        string[] prefixes = new string[]
                           {
                                    "installationplaceid",
                                    "civilsteeltype",
                                    "civilwithleg",
                                    "civilwithoutleg",
                                    "civilnonsteel",
                                    "legid",
                                    "sidearmid",

                           };

                        objectInst.installationConfig = Config
                            .OrderBy(x => Array.FindIndex(prefixes, prefix => x.Key.ToLower().StartsWith(prefix)))
                            .ThenBy(x => x.Key);
                    }
                    var InstallationDate = new BaseInstAttViews()
                    {
                        Key = "InstallationDate",
                        Value = Power.InstallationDate,
                        DataType = "datetime",
                        Label = "InstallationDate",


                    };
                    Civilload.Add(InstallationDate);
                    var ItemOnCivilStatus = new BaseInstAttViews()
                    {
                        Key = "ItemOnCivilStatus",
                        Value = Power.ItemOnCivilStatus,
                        DataType = "string",
                        Label = "ItemOnCivilStatus",


                    };
                    Civilload.Add(ItemOnCivilStatus);
                    var ItemStatus = new BaseInstAttViews()
                    {
                        Key = "ItemStatus",
                        Value = Power.ItemStatus,
                        DataType = "string",
                        Label = "ItemStatus",


                    };
                    Civilload.Add(ItemStatus);
                    var ReservedSpace = new BaseInstAttViews()
                    {
                        Key = "ReservedSpace",
                        Value = Power.ReservedSpace,
                        DataType = "bool",
                        Label = "ReservedSpace",

                    };
                    Civilload.Add(ReservedSpace);


                    objectInst.InstallationAttributes = ListAttributesActivated;
                    objectInst.CivilLoads = Civilload;
                    objectInst.InstallationAttributes = objectInst.InstallationAttributes.Except(ExeptAttributes).ToList();
                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttInstValueRepository.
                        GetDynamicInstAtt(TableNameEntity.Id, PowerId, null);

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
        public Response<GetEnableAttribute> GetPowerInstallationWithEnableAtt(string SiteCode, string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    var attActivated = _dbContext.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "PowerInstallation" &&
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
                        var query = _dbContext.MV_POWER_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();
                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = _dbContext.MV_POWER_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                       .GroupBy(x => new
                       {
                           SiteCode = x.SiteCode,
                           Id = x.Id,
                           Name = x.Name,
                           Azimuth = x.Azimuth,
                           Notes = x.Notes,
                           VisibleStatus = x.VisibleStatus,
                           POWERTYPE = x.POWERTYPE,
                           SerialNumber = x.SerialNumber,
                           HBA = x.HBA,
                           SpaceInstallation = x.SpaceInstallation,
                           HeightBase = x.HeightBase,
                           HeightLand = x.HeightLand,
                           INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                           POWERLIBRARY = x.POWERLIBRARY,
                           Dismantle = x.Dismantle,
                           CenterHigh = x.CenterHigh,
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
        public Response<GetForAddMWDishInstallationObject> AddPowerInstallation(object PowerInstallationViewModel, string SiteCode, string ConnectionString, int? TaskId, int UserId)
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
                            TLIcivilSiteDate AllcivilinstId = null;
                            string ErrorMessage = string.Empty;
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == "TLIpower");
                            AddPowerInstallationObject AddPower= _mapper.Map<AddPowerInstallationObject>(PowerInstallationViewModel);
                            TLIpower Power = _mapper.Map<TLIpower>(AddPower.installationAttributes);
                            var PowerLibrary = _unitOfWork.RadioAntennaLibraryRepository.GetWhereFirst(x => x.Id == AddPower.installationConfig.powerLibraryId
                            && !x.Deleted && x.Active);
                            if (PowerLibrary == null)
                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "PowerLibrary is not found", (int)ApiReturnCode.fail);

                            if (AddPower.installationConfig.InstallationPlaceId == 1)
                            {

                                if (AddPower.installationConfig.civilWithLegId != null)
                                {
                                    AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                    AddPower.installationConfig.civilWithLegId && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower()
                                    , x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                    x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                    if (AllcivilinstId != null)
                                    {
                                        if (AddPower.installationConfig.legId != null)
                                        {
                                            var Leg = _unitOfWork.LegRepository.GetIncludeWhereFirst(x => x.CivilWithLegInstId ==
                                                AddPower.installationConfig.civilWithLegId && x.Id == AddPower.installationConfig.legId
                                                , x => x.CivilWithLegInst);
                                            if (Leg != null)
                                            {
                                                if (AddPower.installationConfig.sideArmId != null)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not selected sidearm because installation place is leg ", (int)ApiReturnCode.fail);

                                                if (!string.IsNullOrEmpty(Power.SerialNumber))
                                                {
                                                    bool CheckSerialNumber = _dbContext.MV_POWER_VIEW.Any(x => x.SerialNumber == Power.SerialNumber && !x.Dismantle);
                                                    if (CheckSerialNumber)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {Power.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                }

                                                if (AddPower.civilLoads.ReservedSpace == true)
                                                {
                                                    var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                    if (Message != "Success")
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                    }
                                                    if (Power.CenterHigh <= 0)
                                                    {
                                                        if (Power.HBA <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (PowerLibrary.Length <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            Power.CenterHigh = Power.HBA + PowerLibrary.Length / 2;
                                                        }
                                                    }
                                                    if (Power.SpaceInstallation == 0)
                                                    {
                                                        if (PowerLibrary.SpaceLibrary == 0)
                                                        {
                                                            if (PowerLibrary.Length == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (PowerLibrary.Width == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                Power.SpaceInstallation = PowerLibrary.Length * PowerLibrary.Width;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Power.SpaceInstallation = PowerLibrary.SpaceLibrary;
                                                        }
                                                    }

                                                    if (AddPower.installationAttributes.Azimuth <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (AddPower.installationAttributes.HeightBase <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                    }

                                                    var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                            x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                            x.LEGID == AddPower.installationConfig.legId
                                                            && x.Azimuth == Power.Azimuth && x.HeightBase == Power.HeightBase && !x.Dismantle)
                                                            .GroupBy(x => new { x.ALLCIVILINST_ID, x.LEGID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                            .Select(g => g.First())
                                                            .ToList();

                                                    if (CheckAzimuthAndHeightBase.Count > 0)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the Radio on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                    TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == AddPower.installationConfig.legId);
                                                    if (legname != null && Power.Azimuth > 0 && Power.HeightBase > 0)
                                                    {
                                                        Power.Name = legname?.CiviLegName + " " + Power.HeightBase+"HE";

                                                    }


                                                    var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                                !x.Dismantle &&
                                                                x.Name.ToLower() == Power.Name.ToLower() &&
                                                                x.SiteCode.ToLower() == SiteCode.ToLower());
                                                    if (CheckName != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {Power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                    if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                    {
                                                        AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                    }
                                                    var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                    if (OldVcivilinfo != null)
                                                    {
                                                        if (Power.SpaceInstallation != 0 && Power.CenterHigh != 0 && AllcivilinstId.allCivilInst.civilWithLegs.HeightBase != 0)
                                                        {
                                                            var EquivalentSpace = Power.SpaceInstallation * (Power.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);

                                                            AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                            Power.EquivalentSpace = EquivalentSpace;
                                                            _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                            _unitOfWork.SaveChanges();
                                                        }
                                                    }

                                                    Power.powerLibraryId = AddPower.installationConfig.powerLibraryId;
                                                    Power.installationPlaceId = AddPower.installationConfig.InstallationPlaceId;
                                                    _unitOfWork.PowerRepository.AddWithHistory(UserId, Power);
                                                    _unitOfWork.SaveChanges();
                                                    int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIpower.ToString(), Power.Id);
                                                    if (AddPower.civilLoads != null && Id != 0)
                                                    {
                                                        TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                        {
                                                            InstallationDate = AddPower.civilLoads.InstallationDate,
                                                            allLoadInstId = Id,
                                                            legId = AddPower.installationConfig?.legId,
                                                            allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                            sideArmId = AddPower.installationConfig?.sideArmId,
                                                            ItemOnCivilStatus = AddPower.civilLoads.ItemOnCivilStatus,
                                                            ItemStatus = AddPower.civilLoads?.ItemStatus,
                                                            Dismantle = false,
                                                            ReservedSpace = AddPower.civilLoads.ReservedSpace,
                                                            SiteCode = SiteCode,


                                                        };
                                                        _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                        _unitOfWork.SaveChanges();

                                                    }

                                                    if (AddPower.dynamicAttribute.Count > 0)
                                                    {

                                                        _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallations(UserId, AddPower.dynamicAttribute, TableNameEntity.Id, Power.Id, ConnectionString);

                                                    }
                                                }
                                                else
                                                {
                                                    if (Power.CenterHigh <= 0)
                                                    {
                                                        if (Power.HBA <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (PowerLibrary.Length <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            Power.CenterHigh = Power.HBA + PowerLibrary.Length / 2;
                                                        }
                                                    }
                                                    if (Power.SpaceInstallation == 0)
                                                    {
                                                        if (PowerLibrary.SpaceLibrary == 0)
                                                        {
                                                            if (PowerLibrary.Length == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (PowerLibrary.Width == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                Power.SpaceInstallation = PowerLibrary.Length * PowerLibrary.Width;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Power.SpaceInstallation = PowerLibrary.SpaceLibrary;
                                                        }
                                                    }
                                                    if (AddPower.installationAttributes.Azimuth <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (AddPower.installationAttributes.HeightBase <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                            x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                            x.LEGID == AddPower.installationConfig.legId
                                                            && x.Azimuth == Power.Azimuth && x.HeightBase == Power.HeightBase && !x.Dismantle)
                                                            .GroupBy(x => new { x.ALLCIVILINST_ID, x.LEGID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                            .Select(g => g.First())
                                                            .ToList();

                                                    if (CheckAzimuthAndHeightBase.Count > 0)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the Radio on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                    TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == AddPower.installationConfig.legId);
                                                    if (legname != null && Power.Azimuth > 0 && Power.HeightBase > 0)
                                                    {
                                                        Power.Name = legname?.CiviLegName + " " + Power.HeightBase + "HE";

                                                    }

                                                    var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                                !x.Dismantle &&
                                                                x.Name.ToLower() == Power.Name.ToLower() &&
                                                                x.SiteCode.ToLower() == SiteCode.ToLower());

                                                    if (CheckName != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {Power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                    Power.powerLibraryId = AddPower.installationConfig.powerLibraryId;
                                                    Power.installationPlaceId = AddPower.installationConfig.InstallationPlaceId;
                                                    _unitOfWork.PowerRepository.AddWithHistory(UserId, Power);
                                                    _unitOfWork.SaveChanges();
                                                    int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIpower.ToString(), Power.Id);
                                                    if (AddPower.civilLoads != null && Id != 0)
                                                    {
                                                        TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                        {
                                                            InstallationDate = AddPower.civilLoads.InstallationDate,
                                                            allLoadInstId = Id,
                                                            legId = AddPower.installationConfig?.legId,
                                                            allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                            sideArmId = AddPower.installationConfig?.sideArmId,
                                                            ItemOnCivilStatus = AddPower.civilLoads.ItemOnCivilStatus,
                                                            ItemStatus = AddPower.civilLoads?.ItemStatus,
                                                            Dismantle = false,
                                                            ReservedSpace = AddPower.civilLoads.ReservedSpace,
                                                            SiteCode = SiteCode,


                                                        };
                                                        _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                        _unitOfWork.SaveChanges();

                                                    }


                                                    if (AddPower.dynamicAttribute != null ? AddPower.dynamicAttribute.Count > 0 : false)
                                                    {
                                                        foreach (var DynamicAttInstValue in AddPower.dynamicAttribute)
                                                        {
                                                            _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, Power.Id, ConnectionString);
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

                            if (AddPower.installationConfig.InstallationPlaceId == 2)
                            {
                                if (AddPower.installationConfig.civilSteelType == 0)
                                {
                                    if (AddPower.installationConfig.civilWithLegId != null)
                                    {
                                        AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                            AddPower.installationConfig.civilWithLegId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                            x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                        if (AllcivilinstId != null)
                                        {
                                            if (AddPower.installationConfig.legId != null)
                                            {
                                                var LegIsFound = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == AddPower.installationConfig.legId
                                                    && x.CivilWithLegInstId == AddPower.installationConfig.civilWithLegId);
                                                if (LegIsFound == null)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "selected Leg is not found on civil", (int)ApiReturnCode.fail);

                                                if (AddPower.installationConfig.sideArmId != null)
                                                {
                                                    var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                                        AddPower.installationConfig.civilWithLegId && !x.Dismantle && x.sideArmId == AddPower.installationConfig.sideArmId
                                                        && (x.legId == AddPower.installationConfig.legId || x.Leg2Id == AddPower.installationConfig.legId), x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                                        x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                                    if (SideArm != null)
                                                    {

                                                        if (!string.IsNullOrEmpty(Power.SerialNumber))
                                                        {
                                                            bool CheckSerialNumber = _dbContext.MV_POWER_VIEW.Any(x => x.SerialNumber == Power.SerialNumber && !x.Dismantle);
                                                            if (CheckSerialNumber)
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {Power.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                        }

                                                        if (AllcivilinstId != null)
                                                        {
                                                            if (AddPower.civilLoads.ReservedSpace == true)
                                                            {
                                                                var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                                if (Message != "Success")
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                                }
                                                                if (Power.CenterHigh <= 0)
                                                                {
                                                                    if (Power.HBA <= 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    if (PowerLibrary.Length <= 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else
                                                                    {
                                                                        Power.CenterHigh = Power.HBA + PowerLibrary.Length / 2;
                                                                    }
                                                                }
                                                                if (Power.SpaceInstallation == 0)
                                                                {
                                                                    if (PowerLibrary.SpaceLibrary == 0)
                                                                    {
                                                                        if (PowerLibrary.Length == 0)
                                                                        {
                                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                        }
                                                                        if (PowerLibrary.Width == 0)
                                                                        {
                                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                        }
                                                                        else
                                                                        {
                                                                            Power.SpaceInstallation = PowerLibrary.Length * PowerLibrary.Width;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Power.SpaceInstallation = PowerLibrary.SpaceLibrary;
                                                                    }
                                                                }

                                                                if (AddPower.installationAttributes.Azimuth <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                if (AddPower.installationAttributes.HeightBase <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                                        x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                        x.SIDEARM_ID == AddPower.installationConfig.sideArmId
                                                                        && x.Azimuth == Power.Azimuth && x.HeightBase ==
                                                                        Power.HeightBase && !x.Dismantle).
                                                                        GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                                        .Select(g => g.First())
                                                                        .ToList();

                                                                if (CheckAzimuthAndHeightBase.Count > 0)
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the Radio on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                                var SideArmName1 = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == AddPower.installationConfig.sideArmId);
                                                                if (SideArmName1 != null && Power.Azimuth > 0 && Power.HeightBase > 0)
                                                                {
                                                                    Power.Name = SideArmName1?.Name + " " + Power.HeightBase+"HE";
                                                                }


                                                                var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                                            !x.Dismantle &&
                                                                            x.Name.ToLower() == Power.Name.ToLower() &&
                                                                            x.SiteCode.ToLower() == SiteCode.ToLower());
                                                                if (CheckName != null)
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {Power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                                if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                                {
                                                                    AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                                }
                                                                var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                                if (OldVcivilinfo != null)
                                                                {

                                                                    var EquivalentSpace = Power.SpaceInstallation * (Power.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);

                                                                    AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                                    Power.EquivalentSpace = EquivalentSpace;
                                                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                                    _unitOfWork.SaveChanges();
                                                                }


                                                                Power.powerLibraryId = AddPower.installationConfig.powerLibraryId;
                                                                Power.installationPlaceId = AddPower.installationConfig.InstallationPlaceId;
                                                                _unitOfWork.PowerRepository.AddWithHistory(UserId, Power);
                                                                _unitOfWork.SaveChanges();
                                                                int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIpower.ToString(), Power.Id);
                                                                if (AddPower.civilLoads != null && Id != 0)
                                                                {
                                                                    TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                                    {
                                                                        InstallationDate = AddPower.civilLoads.InstallationDate,
                                                                        allLoadInstId = Id,
                                                                        legId = AddPower.installationConfig?.legId,
                                                                        allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                                        sideArmId = AddPower.installationConfig?.sideArmId,
                                                                        ItemOnCivilStatus = AddPower.civilLoads.ItemOnCivilStatus,
                                                                        ItemStatus = AddPower.civilLoads?.ItemStatus,
                                                                        Dismantle = false,
                                                                        ReservedSpace = AddPower.civilLoads.ReservedSpace,
                                                                        SiteCode = SiteCode,


                                                                    };
                                                                    _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                                    _unitOfWork.SaveChanges();

                                                                }

                                                                if (AddPower.dynamicAttribute != null ? AddPower.dynamicAttribute.Count > 0 : false)
                                                                {
                                                                    foreach (var DynamicAttInstValue in AddPower.dynamicAttribute)
                                                                    {
                                                                        _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, Power.Id, ConnectionString);
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (Power.CenterHigh <= 0)
                                                                {
                                                                    if (Power.HBA <= 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    if (PowerLibrary.Length <= 0)
                                                                    {
                                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                                    }
                                                                    else
                                                                    {
                                                                        Power.CenterHigh = Power.HBA + PowerLibrary.Length / 2;
                                                                    }
                                                                }
                                                                if (Power.SpaceInstallation == 0)
                                                                {
                                                                    if (PowerLibrary.SpaceLibrary == 0)
                                                                    {
                                                                        if (PowerLibrary.Length == 0)
                                                                        {
                                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                        }
                                                                        if (PowerLibrary.Width == 0)
                                                                        {
                                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                        }
                                                                        else
                                                                        {
                                                                            Power.SpaceInstallation = PowerLibrary.Length * PowerLibrary.Width;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Power.SpaceInstallation = PowerLibrary.SpaceLibrary;
                                                                    }
                                                                }

                                                                if (AddPower.installationAttributes.Azimuth <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                if (AddPower.installationAttributes.HeightBase <= 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                                        x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                        x.SIDEARM_ID == AddPower.installationConfig.sideArmId
                                                                        && x.Azimuth == Power.Azimuth && x.HeightBase ==
                                                                        Power.HeightBase && !x.Dismantle).
                                                                        GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                                        .Select(g => g.First())
                                                                        .ToList();


                                                                if (CheckAzimuthAndHeightBase.Count > 0)
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the Radio on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);

                                                                var SideArmName1 = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == AddPower.installationConfig.sideArmId);
                                                                if (SideArmName1 != null && Power.Azimuth > 0 && Power.HeightBase > 0)
                                                                {
                                                                    Power.Name = SideArmName1?.Name + " "+ Power.HeightBase+"HE";
                                                                }


                                                                var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                                            !x.Dismantle &&
                                                                            x.Name.ToLower() == Power.Name.ToLower() &&
                                                                            x.SiteCode.ToLower() == SiteCode.ToLower());

                                                                if (CheckName != null)
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {Power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                                Power.powerLibraryId = AddPower.installationConfig.powerLibraryId;
                                                                Power.installationPlaceId = AddPower.installationConfig.InstallationPlaceId;
                                                                _unitOfWork.PowerRepository.AddWithHistory(UserId, Power);
                                                                _unitOfWork.SaveChanges();
                                                                int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIpower.ToString(), Power.Id);
                                                                if (AddPower.civilLoads != null && Id != 0)
                                                                {
                                                                    TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                                    {
                                                                        InstallationDate = AddPower.civilLoads.InstallationDate,
                                                                        allLoadInstId = Id,
                                                                        legId = AddPower.installationConfig?.legId,
                                                                        allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                                        sideArmId = AddPower.installationConfig?.sideArmId,
                                                                        ItemOnCivilStatus = AddPower.civilLoads.ItemOnCivilStatus,
                                                                        ItemStatus = AddPower.civilLoads?.ItemStatus,
                                                                        Dismantle = false,
                                                                        ReservedSpace = AddPower.civilLoads.ReservedSpace,
                                                                        SiteCode = SiteCode,


                                                                    };
                                                                    _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                                    _unitOfWork.SaveChanges();

                                                                }

                                                                if (AddPower.dynamicAttribute != null ? AddPower.dynamicAttribute.Count > 0 : false)
                                                                {
                                                                    foreach (var DynamicAttInstValue in AddPower.dynamicAttribute)
                                                                    {
                                                                        _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, Power.Id, ConnectionString);
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
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected Leg ", (int)ApiReturnCode.fail);
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
                                if (AddPower.installationConfig.civilSteelType == 1)
                                {
                                    if (AddPower.installationConfig.civilWithoutLegId != null)
                                    {
                                        AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLegId ==
                                            AddPower.installationConfig.civilWithoutLegId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                            x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);

                                        if (AllcivilinstId != null)
                                        {
                                            if (AddPower.installationConfig.sideArmId != null)

                                            {
                                                var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLegId ==
                                                AddPower.installationConfig.civilWithoutLegId && !x.Dismantle && x.sideArmId == AddPower.installationConfig.sideArmId, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                                x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);

                                                if (SideArm != null)
                                                {
                                                    if (AddPower.installationConfig.legId != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not selected leg because installation place is sidearm ", (int)ApiReturnCode.fail);

                                                    if (!string.IsNullOrEmpty(Power.SerialNumber))
                                                    {
                                                        bool CheckSerialNumber = _dbContext.MV_POWER_VIEW.Any(x => x.SerialNumber == Power.SerialNumber && !x.Dismantle);
                                                        if (CheckSerialNumber)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {Power.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                    }

                                                    if (AddPower.civilLoads.ReservedSpace == true)
                                                    {
                                                        var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                        if (Message != "Success")
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                        }
                                                        if (Power.CenterHigh <= 0)
                                                        {
                                                            if (Power.HBA <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (PowerLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                Power.CenterHigh = Power.HBA + PowerLibrary.Length / 2;
                                                            }
                                                        }
                                                        if (Power.SpaceInstallation == 0)
                                                        {
                                                            if (PowerLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (PowerLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                if (PowerLibrary.Width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    Power.SpaceInstallation = PowerLibrary.Length * PowerLibrary.Width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Power.SpaceInstallation = PowerLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        if (AddPower.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (AddPower.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                                        x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                        x.SIDEARM_ID == AddPower.installationConfig.sideArmId
                                                                        && x.Azimuth == Power.Azimuth && x.HeightBase ==
                                                                        Power.HeightBase && !x.Dismantle).
                                                                        GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                                        .Select(g => g.First())
                                                                        .ToList();


                                                        if (CheckAzimuthAndHeightBase.Count > 0)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the Radio on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                        var SideArmName1 = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == AddPower.installationConfig.sideArmId);
                                                        if (SideArmName1 != null && Power.Azimuth > 0 && Power.HeightBase > 0)
                                                        {
                                                            Power.Name = SideArmName1?.Name + " " + Power.HeightBase+"HE";
                                                        }



                                                        var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                                    !x.Dismantle &&
                                                                    x.Name.ToLower() == Power.Name.ToLower() &&
                                                                    x.SiteCode.ToLower() == SiteCode.ToLower());
                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {Power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        if (AllcivilinstId.allCivilInst.civilWithoutLeg?.CurrentLoads == null)
                                                        {
                                                            AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads = 0;
                                                        }
                                                        var OldVcivilinfo = _dbContext.TLIcivilWithoutLeg.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithoutLegId);

                                                        if (OldVcivilinfo != null)
                                                        {

                                                            var EquivalentSpace = Power.SpaceInstallation * (Power.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithoutLeg.HeightBase);

                                                            AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads += EquivalentSpace;
                                                            Power.EquivalentSpace = EquivalentSpace;
                                                            _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithoutLeg);

                                                            _unitOfWork.SaveChanges();
                                                        }


                                                        Power.powerLibraryId = AddPower.installationConfig.powerLibraryId;
                                                        Power.installationPlaceId = AddPower.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.PowerRepository.AddWithHistory(UserId, Power);
                                                        _unitOfWork.SaveChanges();
                                                        int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIpower.ToString(), Power.Id);
                                                        if (AddPower.civilLoads != null && Id != 0)
                                                        {
                                                            TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                            {
                                                                InstallationDate = AddPower.civilLoads.InstallationDate,
                                                                allLoadInstId = Id,
                                                                legId = AddPower.installationConfig?.legId,
                                                                allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                                sideArmId = AddPower.installationConfig?.sideArmId,
                                                                ItemOnCivilStatus = AddPower.civilLoads.ItemOnCivilStatus,
                                                                ItemStatus = AddPower.civilLoads?.ItemStatus,
                                                                Dismantle = false,
                                                                ReservedSpace = AddPower.civilLoads.ReservedSpace,
                                                                SiteCode = SiteCode,


                                                            };
                                                            _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                            _unitOfWork.SaveChanges();

                                                        }

                                                        if (AddPower.dynamicAttribute != null ? AddPower.dynamicAttribute.Count > 0 : false)
                                                        {
                                                            foreach (var DynamicAttInstValue in AddPower.dynamicAttribute)
                                                            {
                                                                _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, Power.Id, ConnectionString);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Power.CenterHigh <= 0)
                                                        {
                                                            if (Power.HBA <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (PowerLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                Power.CenterHigh = Power.HBA + PowerLibrary.Length / 2;
                                                            }
                                                        }
                                                        if (Power.SpaceInstallation == 0)
                                                        {
                                                            if (PowerLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (PowerLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                if (PowerLibrary.Width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    Power.SpaceInstallation = PowerLibrary.Length * PowerLibrary.Width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Power.SpaceInstallation = PowerLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        if (AddPower.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (AddPower.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                                    x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                    x.SIDEARM_ID == AddPower.installationConfig.sideArmId
                                                                    && x.Azimuth == Power.Azimuth && x.HeightBase ==
                                                                    Power.HeightBase && !x.Dismantle).
                                                                    GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                                        .Select(g => g.First())
                                                                        .ToList();


                                                        if (CheckAzimuthAndHeightBase.Count > 0)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the Radio on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                        var SideArmName1 = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == AddPower.installationConfig.sideArmId);
                                                        if (SideArmName1 != null && Power.Azimuth > 0 && Power.HeightBase > 0)
                                                        {
                                                            Power.Name = SideArmName1?.Name + " "+ Power.HeightBase+"HE";
                                                        }



                                                        var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                                    !x.Dismantle &&
                                                                    x.Name.ToLower() == Power.Name.ToLower() &&
                                                                    x.SiteCode.ToLower() == SiteCode.ToLower());

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {Power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        Power.powerLibraryId = AddPower.installationConfig.powerLibraryId;
                                                        Power.installationPlaceId = AddPower.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.PowerRepository.AddWithHistory(UserId, Power);
                                                        _unitOfWork.SaveChanges();
                                                        int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIpower.ToString(), Power.Id);
                                                        if (AddPower.civilLoads != null && Id != 0)
                                                        {
                                                            TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                            {
                                                                InstallationDate = AddPower.civilLoads.InstallationDate,
                                                                allLoadInstId = Id,
                                                                legId = AddPower.installationConfig?.legId,
                                                                allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                                sideArmId = AddPower.installationConfig?.sideArmId,
                                                                ItemOnCivilStatus = AddPower.civilLoads.ItemOnCivilStatus,
                                                                ItemStatus = AddPower.civilLoads?.ItemStatus,
                                                                Dismantle = false,
                                                                ReservedSpace = AddPower.civilLoads.ReservedSpace,
                                                                SiteCode = SiteCode,


                                                            };
                                                            _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                            _unitOfWork.SaveChanges();

                                                        }

                                                        if (AddPower.dynamicAttribute != null ? AddPower.dynamicAttribute.Count > 0 : false)
                                                        {
                                                            foreach (var DynamicAttInstValue in AddPower.dynamicAttribute)
                                                            {
                                                                _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, Power.Id, ConnectionString);
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
                                if (AddPower.installationConfig.civilSteelType == 2)
                                {
                                    if (AddPower.installationConfig.civilNonSteelId != null)
                                    {
                                        AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId ==
                                            AddPower.installationConfig.civilNonSteelId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                            x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                        if (AllcivilinstId != null)
                                        {
                                            if (AddPower.installationConfig.sideArmId != null)
                                            {
                                                var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId ==
                                                    AddPower.installationConfig.civilNonSteelId && !x.Dismantle && x.sideArmId == AddPower.installationConfig.sideArmId, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                                    x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                                if (SideArm != null)
                                                {
                                                    if (AddPower.installationConfig.legId != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not selected leg because installation place is sidearm ", (int)ApiReturnCode.fail);

                                                    if (!string.IsNullOrEmpty(Power.SerialNumber))
                                                    {
                                                        bool CheckSerialNumber = _dbContext.MV_POWER_VIEW.Any(x => x.SerialNumber == Power.SerialNumber && !x.Dismantle);
                                                        if (CheckSerialNumber)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {Power.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                    }
                                                    if (AddPower.civilLoads.ReservedSpace == true)
                                                    {
                                                        var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                        if (Message != "Success")
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                        }
                                                    }
                                                    if (Power.CenterHigh <= 0)
                                                    {
                                                        if (Power.HBA <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (PowerLibrary.Length <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            Power.CenterHigh = Power.HBA + PowerLibrary.Length / 2;
                                                        }
                                                    }
                                                    else if (Power.SpaceInstallation == 0)
                                                    {
                                                        if (PowerLibrary.SpaceLibrary == 0)
                                                        {
                                                            if (PowerLibrary.Length == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (PowerLibrary.Width == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                Power.SpaceInstallation = PowerLibrary.Length * PowerLibrary.Width;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Power.SpaceInstallation = PowerLibrary.SpaceLibrary;
                                                        }
                                                    }

                                                    if (AddPower.installationAttributes.Azimuth <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (AddPower.installationAttributes.HeightBase <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                                        x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                        x.SIDEARM_ID == AddPower.installationConfig.sideArmId
                                                                        && x.Azimuth == Power.Azimuth && x.HeightBase ==
                                                                        Power.HeightBase && !x.Dismantle).
                                                                        GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                                        .Select(g => g.First())
                                                                        .ToList();


                                                    if (CheckAzimuthAndHeightBase.Count > 0)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the Radio on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                    var SideArmName1 = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == AddPower.installationConfig.sideArmId);
                                                    if (SideArmName1 != null && Power.Azimuth > 0 && Power.HeightBase > 0)
                                                    {
                                                        Power.Name = SideArmName1?.Name + " " + Power.HeightBase+"HE";
                                                    }


                                                    var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                                !x.Dismantle &&
                                                                x.Name.ToLower() == Power.Name.ToLower() &&
                                                                x.SiteCode.ToLower() == SiteCode.ToLower());

                                                    if (CheckName != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {Power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                    Power.powerLibraryId = AddPower.installationConfig.powerLibraryId;
                                                    Power.installationPlaceId = AddPower.installationConfig.InstallationPlaceId;
                                                    _unitOfWork.PowerRepository.AddWithHistory(UserId, Power);
                                                    _unitOfWork.SaveChanges();
                                                    int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIpower.ToString(), Power.Id);
                                                    if (AddPower.civilLoads != null && Id != 0)
                                                    {
                                                        TLIcivilLoads tLIcivilLoads = new TLIcivilLoads()
                                                        {
                                                            InstallationDate = AddPower.civilLoads.InstallationDate,
                                                            allLoadInstId = Id,
                                                            legId = AddPower.installationConfig?.legId,
                                                            allCivilInstId = AllcivilinstId.allCivilInst.Id,
                                                            sideArmId = AddPower.installationConfig?.sideArmId,
                                                            ItemOnCivilStatus = AddPower.civilLoads.ItemOnCivilStatus,
                                                            ItemStatus = AddPower.civilLoads?.ItemStatus,
                                                            Dismantle = false,
                                                            ReservedSpace = AddPower.civilLoads.ReservedSpace,
                                                            SiteCode = SiteCode,


                                                        };
                                                        _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, tLIcivilLoads);
                                                        _unitOfWork.SaveChanges();

                                                    }

                                                    if (AddPower.dynamicAttribute != null ? AddPower.dynamicAttribute.Count > 0 : false)
                                                    {
                                                        foreach (var DynamicAttInstValue in AddPower.dynamicAttribute)
                                                        {
                                                            _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, Power.Id, ConnectionString);
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
                           
                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(ConnectionString));
                           
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
        public async Task<Response<GetForAddMWDishInstallationObject>> EditPowerInstallation(object PowerInstallationViewModel, int? TaskId, int UserId, string ConnectionString)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    TLIcivilSiteDate AllcivilinstId = null;
                    List<TLIallLoadInst> RadioAntennas = new List<TLIallLoadInst>();
                    
                        var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLIpower.ToString().ToLower()).Id;
                        EditPowerInstallationOject Editpower = _mapper.Map<EditPowerInstallationOject>(PowerInstallationViewModel);
                        TLIpower power = _mapper.Map<TLIpower>(Editpower.installationAttributes);
                        TLIcivilLoads powerInst = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable().AsNoTracking()
                       .Include(x => x.allLoadInst).Include(x => x.allLoadInst.power).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                       .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                       .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                        if (powerInst == null)
                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "power is not found", (int)ApiReturnCode.fail);
                        if (Editpower.installationConfig.InstallationPlaceId == 1)
                        {
                            if (Editpower.installationConfig.civilWithLegId != null)
                            {
                                AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                Editpower.installationConfig.civilWithLegId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                if (AllcivilinstId != null)
                                {
                                    if (Editpower.installationConfig.legId != null)
                                    {
                                        if (Editpower.installationConfig.sideArmId != null)
                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not selected sidearm because installation place is leg ", (int)ApiReturnCode.fail);

                                        var Leg = _unitOfWork.LegRepository.GetIncludeWhereFirst(x => x.CivilWithLegInstId ==
                                                Editpower.installationConfig.civilWithLegId && x.Id == Editpower.installationConfig.legId
                                                , x => x.CivilWithLegInst);
                                        if (Leg != null)
                                        {
                                            if (!string.IsNullOrEmpty(power.SerialNumber))
                                            {
                                                bool CheckSerialNumber = _dbContext.MV_POWER_VIEW.Any(x => x.SerialNumber == power.SerialNumber && !x.Dismantle && x.Id != power.Id);
                                                if (CheckSerialNumber)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {power.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                            }

                                            if (powerInst.ReservedSpace == true && Editpower.civilLoads.ReservedSpace == true)
                                            {
                                                if (power.CenterHigh <= 0)
                                                {
                                                    if (power.HBA <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    else
                                                    {
                                                        power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                    }
                                                }
                                                if (power.SpaceInstallation == 0)
                                                {
                                                    if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                    {
                                                        if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                    }
                                                }

                                                if (Editpower.installationAttributes.Azimuth <= 0)
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                }
                                                if (Editpower.installationAttributes.HeightBase <= 0)
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                }

                                                var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                        x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                        x.LEGID == Editpower.installationConfig.legId && x.Id != power.Id
                                                        && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                      .GroupBy(x => new { x.ALLCIVILINST_ID, x.LEGID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                                            .Select(g => g.First())
                                                                            .ToList();

                                                if (CheckAzimuthAndHeightBase.Count > 0)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == Editpower.installationConfig.legId);
                                                if (legname != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                {
                                                power.Name = legname?.CiviLegName + " " + power.HeightBase+"HE";

                                                }
                                                var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                           !x.Dismantle && x.Id != power.Id &&
                                                           x.Name.ToLower() == power.Name.ToLower() &&
                                                           x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());
                                                if (CheckName != null)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                {
                                                    AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                }
                                                var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                if (OldVcivilinfo != null)
                                                {
                                                    var EquivalentSpace = power.SpaceInstallation * (power.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);
                                                    AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                    power.EquivalentSpace = EquivalentSpace;
                                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                    _unitOfWork.SaveChanges();
                                                }
                                                power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                _unitOfWork.SaveChanges();
                                                if (Editpower.civilLoads != null)
                                                {

                                                    var existingEntity = _unitOfWork.CivilLoadsRepository
                                                        .GetAllAsQueryable()
                                                        .AsNoTracking()
                                                        .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                    TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                      .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                      .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                      .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                    NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                    NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                    NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                    NewpowerInst.sideArm2Id = null;
                                                    NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                    NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                    NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                    NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                    _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                    _unitOfWork.SaveChanges();

                                                }

                                                if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                    _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);

                                            }
                                            if (powerInst.ReservedSpace == true && Editpower.civilLoads.ReservedSpace == false)
                                            {
                                                if (power.CenterHigh <= 0)
                                                {
                                                    if (power.HBA <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    else
                                                    {
                                                        power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                    }
                                                }
                                                if (power.SpaceInstallation == 0)
                                                {
                                                    if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                    {
                                                        if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                    }
                                                }

                                                if (Editpower.installationAttributes.Azimuth <= 0)
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                }
                                                if (Editpower.installationAttributes.HeightBase <= 0)
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                }
                                                var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                x.LEGID == Editpower.installationConfig.legId && x.Id != power.Id
                                                && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                               .GroupBy(x => new { x.ALLCIVILINST_ID, x.LEGID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                                    .Select(g => g.First())
                                                                    .ToList();

                                                if (CheckAzimuthAndHeightBase.Count > 0)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == Editpower.installationConfig.legId);
                                                if (legname != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                {
                                                    power.Name = legname?.CiviLegName + " " + power.HeightBase+"HE";

                                                }

                                                var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                              !x.Dismantle && x.Id != power.Id &&
                                                              x.Name.ToLower() == power.Name.ToLower() &&
                                                              x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                if (CheckName != null)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);
                                                if (OldVcivilinfo != null)
                                                {
                                                    AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads - powerInst.allLoadInst.power.EquivalentSpace;
                                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);
                                                    _unitOfWork.SaveChanges();
                                                    power.EquivalentSpace = 0;
                                                }
                                                power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                _unitOfWork.SaveChanges();
                                                if (Editpower.civilLoads != null)
                                                {

                                                    var existingEntity = _unitOfWork.CivilLoadsRepository
                                                        .GetAllAsQueryable()
                                                        .AsNoTracking()
                                                        .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                    TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                      .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                      .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                      .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                    NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                    NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                    NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                    NewpowerInst.sideArm2Id = null;
                                                    NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                    NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                    NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                    NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                    _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                    _unitOfWork.SaveChanges();

                                                }

                                                if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                    _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
                                            }
                                            if (powerInst.ReservedSpace == false && Editpower.civilLoads.ReservedSpace == true)
                                            {
                                                if (power.CenterHigh <= 0)
                                                {
                                                    if (power.HBA <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    else
                                                    {
                                                        power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                    }
                                                }
                                                if (power.SpaceInstallation == 0)
                                                {
                                                    if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                    {
                                                        if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                    }
                                                }

                                                if (Editpower.installationAttributes.Azimuth <= 0)
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                }
                                                if (Editpower.installationAttributes.HeightBase <= 0)
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                }

                                                var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                        x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                        x.LEGID == Editpower.installationConfig.legId && x.Id != power.Id
                                                        && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                       .GroupBy(x => new { x.ALLCIVILINST_ID, x.LEGID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                                    .Select(g => g.First())
                                                                    .ToList();

                                                if (CheckAzimuthAndHeightBase.Count > 0)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == Editpower.installationConfig.legId);
                                                if (legname != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                {
                                                   power.Name = legname?.CiviLegName + " " + power.HeightBase+"HE";
  
                                                }

                                                var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                       !x.Dismantle && x.Id != power.Id &&
                                                       x.Name.ToLower() == power.Name.ToLower() &&
                                                       x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                if (CheckName != null)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                {
                                                    AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                }
                                                var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                if (OldVcivilinfo != null)
                                                {
                                                    var EquivalentSpace = power.SpaceInstallation * (power.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);
                                                    AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                    power.EquivalentSpace = EquivalentSpace;
                                                    _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                    _unitOfWork.SaveChanges();
                                                }

                                                power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                _unitOfWork.SaveChanges();
                                                if (Editpower.civilLoads != null)
                                                {

                                                    var existingEntity = _unitOfWork.CivilLoadsRepository
                                                        .GetAllAsQueryable()
                                                        .AsNoTracking()
                                                        .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                    TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                      .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                      .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                      .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                    NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                    NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                    NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                    NewpowerInst.sideArm2Id = null;
                                                    NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                    NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                    NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                    NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                    _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                    _unitOfWork.SaveChanges();

                                                }

                                                if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                    _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
                                            }
                                            if (powerInst.ReservedSpace == false && Editpower.civilLoads.ReservedSpace == false)
                                            {
                                                if (power.CenterHigh <= 0)
                                                {
                                                    if (power.HBA <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    else
                                                    {
                                                        power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                    }
                                                }
                                                if (power.SpaceInstallation == 0)
                                                {
                                                    if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                    {
                                                        if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                    }
                                                }

                                                if (Editpower.installationAttributes.Azimuth <= 0)
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                }
                                                if (Editpower.installationAttributes.HeightBase <= 0)
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                }

                                                var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                        x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                        x.LEGID == Editpower.installationConfig.legId && x.Id != power.Id
                                                        && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                 .GroupBy(x => new { x.ALLCIVILINST_ID, x.LEGID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                                    .Select(g => g.First())
                                                                    .ToList();

                                                if (CheckAzimuthAndHeightBase.Count > 0)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                TLIleg legname = _dbContext.TLIleg.FirstOrDefault(x => x.Id == Editpower.installationConfig.legId);
                                                if (legname != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                {
                                                   power.Name = legname?.CiviLegName + " " + power.HeightBase+"HE";

                                                }

                                                var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                           !x.Dismantle && x.Id != power.Id &&
                                                           x.Name.ToLower() == power.Name.ToLower() &&
                                                           x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                if (CheckName != null)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                _unitOfWork.SaveChanges();
                                                if (Editpower.civilLoads != null)
                                                {

                                                    var existingEntity = _unitOfWork.CivilLoadsRepository
                                                        .GetAllAsQueryable()
                                                        .AsNoTracking()
                                                        .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                    TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                      .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                      .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                      .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                    NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                    NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                    NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                    NewpowerInst.sideArm2Id = null;
                                                    NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                    NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                    NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                    NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                    _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                    _unitOfWork.SaveChanges();

                                                }

                                                if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                    _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
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
                        else if (Editpower.installationConfig.InstallationPlaceId == 2)
                        {
                            if (Editpower.installationConfig.civilSteelType == 0)
                            {
                                if (Editpower.installationConfig.civilWithLegId != null)
                                {

                                    AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                   Editpower.installationConfig.civilWithLegId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                   x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                    if (AllcivilinstId != null)
                                    {
                                        if (Editpower.installationConfig.legId != null)
                                        {
                                            var LegIsFound = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == Editpower.installationConfig.legId
                                                     && x.CivilWithLegInstId == Editpower.installationConfig.civilWithLegId);
                                            if (LegIsFound == null)
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "selected Leg is not found on civil", (int)ApiReturnCode.fail);

                                            if (Editpower.installationConfig.sideArmId != null)
                                            {

                                                var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId ==
                                                  Editpower.installationConfig.civilWithLegId && !x.Dismantle && x.sideArmId ==
                                                  Editpower.installationConfig.sideArmId
                                                  && x.legId == Editpower.installationConfig.legId, x => x.allCivilInst,
                                                  x => x.allCivilInst.civilWithLegs,
                                                  x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib,
                                                  x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                                if (SideArm != null)
                                                {
                                                    if (!string.IsNullOrEmpty(power.SerialNumber))
                                                    {
                                                        bool CheckSerialNumber = _dbContext.MV_POWER_VIEW.Any(x => x.SerialNumber == power.SerialNumber && !x.Dismantle && x.Id != x.Id);
                                                        if (CheckSerialNumber)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {power.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                    }


                                                    if (powerInst.ReservedSpace == true && Editpower.civilLoads.ReservedSpace == true)
                                                    {
                                                        if (power.CenterHigh <= 0)
                                                        {
                                                            if (power.HBA <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                            }
                                                        }
                                                        if (power.SpaceInstallation == 0)
                                                        {
                                                            if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        if (Editpower.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (Editpower.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }

                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                                x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                x.SIDEARM_ID == Editpower.installationConfig.sideArmId && x.Id != power.Id
                                                                && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                            .GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                              .Select(g => g.First())
                                                              .ToList();

                                                        if (CheckAzimuthAndHeightBase.Count > 0)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);

                                                        var SideArmName = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == Editpower.installationConfig.sideArmId);
                                                        if (SideArmName != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                        {
                                                            power.Name = SideArmName?.Name + " " + power.HeightBase+"HE";
                                                        }

                                                        var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                                  !x.Dismantle && x.Id != power.Id &&
                                                                  x.Name.ToLower() == power.Name.ToLower() &&
                                                                  x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                        {
                                                            AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                        }
                                                        var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                        if (OldVcivilinfo != null)
                                                        {
                                                            var EquivalentSpace = power.SpaceInstallation * (power.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);
                                                            AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                            power.EquivalentSpace = EquivalentSpace;
                                                            _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                            _unitOfWork.SaveChanges();
                                                        }
                                                        power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                        power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                        _unitOfWork.SaveChanges();
                                                        if (Editpower.civilLoads != null)
                                                        {

                                                            var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                .GetAllAsQueryable()
                                                                .AsNoTracking()
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                            TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                                .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                                .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                            NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                            NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                            NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                            NewpowerInst.sideArm2Id = null;
                                                            NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                            NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                            NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                            NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                            _unitOfWork.SaveChanges();

                                                        }
                                                        if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
                                                    }
                                                    if (powerInst.ReservedSpace == true && Editpower.civilLoads.ReservedSpace == false)
                                                    {
                                                        if (power.CenterHigh <= 0)
                                                        {
                                                            if (power.HBA <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                            }
                                                        }
                                                        if (power.SpaceInstallation == 0)
                                                        {
                                                            if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        if (Editpower.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (Editpower.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }

                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                               x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                               x.SIDEARM_ID == Editpower.installationConfig.sideArmId && x.Id != power.Id
                                                               && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                           .GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                              .Select(g => g.First())
                                                              .ToList();

                                                        if (CheckAzimuthAndHeightBase.Count > 0)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);

                                                        var SideArmName = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == Editpower.installationConfig.sideArmId);
                                                        if (SideArmName != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                        {
                                                            power.Name = SideArmName?.Name + " " + power.HeightBase+"HE";
                                                    }

                                                        var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                           !x.Dismantle && x.Id != power.Id &&
                                                           x.Name.ToLower() == power.Name.ToLower() &&
                                                           x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);
                                                        if (OldVcivilinfo != null)
                                                        {
                                                            AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads - powerInst.allLoadInst.power.EquivalentSpace;
                                                            _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);
                                                            _unitOfWork.SaveChanges();
                                                            power.EquivalentSpace = 0;
                                                        }
                                                        power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                        power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                        _unitOfWork.SaveChanges();
                                                        if (Editpower.civilLoads != null)
                                                        {

                                                            var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                .GetAllAsQueryable()
                                                                .AsNoTracking()
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                            TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                                .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                                .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                            NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                            NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                            NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                            NewpowerInst.sideArm2Id = null;
                                                            NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                            NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                            NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                            NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                            _unitOfWork.SaveChanges();

                                                        }

                                                        if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
                                                    }
                                                    if (powerInst.ReservedSpace == false && Editpower.civilLoads.ReservedSpace == true)
                                                    {
                                                        if (power.CenterHigh <= 0)
                                                        {
                                                            if (power.HBA <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                            }
                                                        }
                                                        if (power.SpaceInstallation == 0)
                                                        {
                                                            if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        if (Editpower.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (Editpower.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }

                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                                   x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                                   x.SIDEARM_ID == Editpower.installationConfig.sideArmId && x.Id != power.Id
                                                                   && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                               .GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                              .Select(g => g.First())
                                                              .ToList();
                                                        if (CheckAzimuthAndHeightBase.Count > 0)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);

                                                        var SideArmName = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == Editpower.installationConfig.sideArmId);
                                                        if (SideArmName != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                        {
                                                            power.Name = SideArmName?.Name + " " + power.HeightBase+"HE";
                                                    }


                                                        var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                            !x.Dismantle && x.Id != power.Id &&
                                                            x.Name.ToLower() == power.Name.ToLower() &&
                                                            x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        if (AllcivilinstId.allCivilInst.civilWithLegs?.CurrentLoads == null)
                                                        {
                                                            AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads = 0;
                                                        }
                                                        var OldVcivilinfo = _dbContext.TLIcivilWithLegs.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithLegsId);

                                                        if (OldVcivilinfo != null)
                                                        {
                                                            var EquivalentSpace = power.SpaceInstallation * (power.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithLegs.HeightBase);
                                                            AllcivilinstId.allCivilInst.civilWithLegs.CurrentLoads += EquivalentSpace;
                                                            power.EquivalentSpace = EquivalentSpace;
                                                            _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithLegs);

                                                            _unitOfWork.SaveChanges();
                                                        }

                                                        power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                        power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                        _unitOfWork.SaveChanges();
                                                        if (Editpower.civilLoads != null)
                                                        {

                                                            var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                .GetAllAsQueryable()
                                                                .AsNoTracking()
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                            TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                                .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                                .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                            NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                            NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                            NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                            NewpowerInst.sideArm2Id = null;
                                                            NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                            NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                            NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                            NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                            _unitOfWork.SaveChanges();

                                                        }

                                                        if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
                                                    }
                                                    if (powerInst.ReservedSpace == false && Editpower.civilLoads.ReservedSpace == false)
                                                    {
                                                        if (power.CenterHigh <= 0)
                                                        {
                                                            if (power.HBA <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                            }
                                                        }
                                                        if (power.SpaceInstallation == 0)
                                                        {
                                                            if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                            {
                                                                if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                                {
                                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                                }
                                                                else
                                                                {
                                                                    power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                            }
                                                        }

                                                        if (Editpower.installationAttributes.Azimuth <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (Editpower.installationAttributes.HeightBase <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                        }

                                                        var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                          x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                          x.SIDEARM_ID == Editpower.installationConfig.sideArmId && x.Id != power.Id
                                                          && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                         .GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                              .Select(g => g.First())
                                                              .ToList();
                                                        if (CheckAzimuthAndHeightBase.Count > 0)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);
                                                        var SideArmName = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == Editpower.installationConfig.sideArmId);
                                                        if (SideArmName != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                        {
                                                            power.Name = SideArmName?.Name + " " + power.HeightBase+"HE";
                                                    }

                                                        var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                              !x.Dismantle && x.Id != power.Id &&
                                                              x.Name.ToLower() == power.Name.ToLower() &&
                                                              x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                        if (CheckName != null)
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                        power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                        power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                        _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                        _unitOfWork.SaveChanges();
                                                        if (Editpower.civilLoads != null)
                                                        {

                                                            var existingEntity = _unitOfWork.CivilLoadsRepository
                                                                .GetAllAsQueryable()
                                                                .AsNoTracking()
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                            TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                                .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                                .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                                .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                            NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                            NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                            NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                            NewpowerInst.sideArm2Id = null;
                                                            NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                            NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                            NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                            NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                            _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                            _unitOfWork.SaveChanges();

                                                        }

                                                        if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
                                                    }



                                                }

                                                else
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "this sidearm is not found", (int)ApiReturnCode.fail);
                                                }
                                            }
                                            else
                                            {
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected sideArm ", (int)ApiReturnCode.fail);
                                            }

                                        }
                                        else
                                        {
                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "must selected Leg ", (int)ApiReturnCode.fail);
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
                            else if (Editpower.installationConfig.civilSteelType == 1)
                            {
                                if (Editpower.installationConfig.civilWithoutLegId != null)
                                {
                                    AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLegId ==
                                      Editpower.installationConfig.civilWithoutLegId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                      x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);

                                    if (AllcivilinstId != null)
                                    {
                                        if (Editpower.installationConfig.sideArmId != null)
                                        {
                                            if (Editpower.installationConfig.legId != null)
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not selected leg because installation place is sidearm ", (int)ApiReturnCode.fail);

                                            var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLegId ==
                                              Editpower.installationConfig.civilWithoutLegId && !x.Dismantle && x.sideArmId == Editpower.installationConfig.sideArmId, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                              x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                            if (SideArm != null)
                                            {
                                                if (!string.IsNullOrEmpty(power.SerialNumber))
                                                {
                                                    bool CheckSerialNumber = _dbContext.MV_POWER_VIEW.Any(x => x.SerialNumber == power.SerialNumber && !x.Dismantle && x.Id != x.Id);
                                                    if (CheckSerialNumber)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {power.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                }


                                                if (powerInst.ReservedSpace == true && Editpower.civilLoads.ReservedSpace == true)
                                                {
                                                    if (power.CenterHigh <= 0)
                                                    {
                                                        if (power.HBA <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                        }
                                                    }
                                                    if (power.SpaceInstallation == 0)
                                                    {
                                                        if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                        {
                                                            if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                        }
                                                    }

                                                    if (Editpower.installationAttributes.Azimuth <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (Editpower.installationAttributes.HeightBase <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                         x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                         x.SIDEARM_ID == Editpower.installationConfig.sideArmId && x.Id != power.Id
                                                         && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                     .GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                              .Select(g => g.First())
                                                              .ToList();

                                                    if (CheckAzimuthAndHeightBase.Count > 0)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);


                                                    var SideArmName = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == Editpower.installationConfig.sideArmId);
                                                    if (SideArmName != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                    {
                                                        power.Name = SideArmName?.Name + " " + power.HeightBase+"HE";
                                                }
                                                    var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                               !x.Dismantle && x.Id != power.Id &&
                                                               x.Name.ToLower() == power.Name.ToLower() &&
                                                               x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                    if (CheckName != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                    if (AllcivilinstId.allCivilInst.civilWithoutLeg?.CurrentLoads == null)
                                                    {
                                                        AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads = 0;
                                                    }
                                                    var OldVcivilinfo = _dbContext.TLIcivilWithoutLeg.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithoutLegId);

                                                    if (OldVcivilinfo != null)
                                                    {
                                                        var EquivalentSpace = power.SpaceInstallation * (power.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithoutLeg.HeightBase);
                                                        AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads += EquivalentSpace;
                                                        power.EquivalentSpace = EquivalentSpace;
                                                        _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithoutLeg);

                                                        _unitOfWork.SaveChanges();
                                                    }
                                                    power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                    power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                    _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                    _unitOfWork.SaveChanges();
                                                    if (Editpower.civilLoads != null)
                                                    {

                                                        var existingEntity = _unitOfWork.CivilLoadsRepository
                                                            .GetAllAsQueryable()
                                                            .AsNoTracking()
                                                            .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                        TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                          .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                          .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                          .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                        NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                        NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                        NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                        NewpowerInst.sideArm2Id = null;
                                                        NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                        NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                        NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                        NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                        _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                        _unitOfWork.SaveChanges();

                                                    }

                                                    if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
                                                }
                                                if (powerInst.ReservedSpace == true && Editpower.civilLoads.ReservedSpace == false)
                                                {
                                                    if (power.CenterHigh <= 0)
                                                    {
                                                        if (power.HBA <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                        }
                                                    }
                                                    if (power.SpaceInstallation == 0)
                                                    {
                                                        if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                        {
                                                            if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                        }
                                                    }

                                                    if (Editpower.installationAttributes.Azimuth <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (Editpower.installationAttributes.HeightBase <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                    }

                                                    var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                       x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                       x.SIDEARM_ID == Editpower.installationConfig.sideArmId && x.Id != power.Id
                                                       && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                    .GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                              .Select(g => g.First())
                                                              .ToList();

                                                    if (CheckAzimuthAndHeightBase.Count > 0)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);

                                                    var SideArmName = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == Editpower.installationConfig.sideArmId);
                                                    if (SideArmName != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                    {
                                                        power.Name = SideArmName?.Name + " " + power.HeightBase+"HE";
                                                }

                                                    var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                        !x.Dismantle && x.Id != power.Id &&
                                                        x.Name.ToLower() == power.Name.ToLower() &&
                                                        x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                    if (CheckName != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                    var OldVcivilinfo = _dbContext.TLIcivilWithoutLeg.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithoutLegId);
                                                    if (OldVcivilinfo != null)
                                                    {
                                                        AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads = AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads - powerInst.allLoadInst.power.EquivalentSpace;
                                                        _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithoutLeg);
                                                        _unitOfWork.SaveChanges();
                                                        power.EquivalentSpace = 0;
                                                    }
                                                    power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                    power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                    _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                    _unitOfWork.SaveChanges();
                                                    if (Editpower.civilLoads != null)
                                                    {

                                                        var existingEntity = _unitOfWork.CivilLoadsRepository
                                                            .GetAllAsQueryable()
                                                            .AsNoTracking()
                                                            .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                        TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                          .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                          .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                          .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                        NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                        NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                        NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                        NewpowerInst.sideArm2Id = null;
                                                        NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                        NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                        NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                        NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                        _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                        _unitOfWork.SaveChanges();

                                                    }
                                                    if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
                                                }
                                                if (powerInst.ReservedSpace == false && Editpower.civilLoads.ReservedSpace == true)
                                                {
                                                    if (power.CenterHigh <= 0)
                                                    {
                                                        if (power.HBA <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                        }
                                                    }
                                                    if (power.SpaceInstallation == 0)
                                                    {
                                                        if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                        {
                                                            if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                        }
                                                    }

                                                    if (Editpower.installationAttributes.Azimuth <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (Editpower.installationAttributes.HeightBase <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                    }

                                                    var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                         x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                         x.SIDEARM_ID == Editpower.installationConfig.sideArmId && x.Id != power.Id
                                                         && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                      .GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                              .Select(g => g.First())
                                                              .ToList();

                                                    if (CheckAzimuthAndHeightBase.Count > 0)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);

                                                    var SideArmName = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == Editpower.installationConfig.sideArmId);
                                                    if (SideArmName != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                    {
                                                        power.Name = SideArmName?.Name + " " + power.HeightBase+"HE";
                                                }

                                                    var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                         !x.Dismantle && x.Id != power.Id &&
                                                         x.Name.ToLower() == power.Name.ToLower() &&
                                                         x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                    if (CheckName != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                    if (AllcivilinstId.allCivilInst.civilWithoutLeg?.CurrentLoads == null)
                                                    {
                                                        AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads = 0;
                                                    }
                                                    var OldVcivilinfo = _dbContext.TLIcivilWithoutLeg.AsNoTracking().FirstOrDefault(x => x.Id == AllcivilinstId.allCivilInst.civilWithoutLegId);

                                                    if (OldVcivilinfo != null)
                                                    {
                                                        var EquivalentSpace = power.SpaceInstallation * (power.CenterHigh / (float)AllcivilinstId.allCivilInst.civilWithoutLeg.HeightBase);
                                                        AllcivilinstId.allCivilInst.civilWithoutLeg.CurrentLoads += EquivalentSpace;
                                                        power.EquivalentSpace = EquivalentSpace;
                                                        _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(UserId, OldVcivilinfo, AllcivilinstId.allCivilInst.civilWithoutLeg);

                                                        _unitOfWork.SaveChanges();
                                                    }

                                                    power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                    power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                    _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                    _unitOfWork.SaveChanges();
                                                    if (Editpower.civilLoads != null)
                                                    {

                                                        var existingEntity = _unitOfWork.CivilLoadsRepository
                                                            .GetAllAsQueryable()
                                                            .AsNoTracking()
                                                            .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                        TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                          .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                          .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                          .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                        NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                        NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                        NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                        NewpowerInst.sideArm2Id = null;
                                                        NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                        NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                        NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                        NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                        _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                        _unitOfWork.SaveChanges();

                                                    }

                                                    if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
                                                }
                                                if (powerInst.ReservedSpace == false && Editpower.civilLoads.ReservedSpace == false)
                                                {
                                                    if (power.CenterHigh <= 0)
                                                    {
                                                        if (power.HBA <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                        }
                                                    }
                                                    if (power.SpaceInstallation == 0)
                                                    {
                                                        if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                        {
                                                            if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                            {
                                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                            }
                                                            else
                                                            {
                                                                power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                        }
                                                    }

                                                    if (Editpower.installationAttributes.Azimuth <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (Editpower.installationAttributes.HeightBase <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                    }

                                                    var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                       x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                       x.SIDEARM_ID == Editpower.installationConfig.sideArmId && x.Id != power.Id
                                                       && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                   .GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                              .Select(g => g.First())
                                                              .ToList();

                                                    if (CheckAzimuthAndHeightBase.Count > 0)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);

                                                    var SideArmName = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == Editpower.installationConfig.sideArmId);
                                                    if (SideArmName != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                    {
                                                        power.Name = SideArmName?.Name + " " + power.HeightBase+"HE";
                                                }

                                                    var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                            !x.Dismantle && x.Id != power.Id &&
                                                            x.Name.ToLower() == power.Name.ToLower() &&
                                                            x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                    if (CheckName != null)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                    power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                    power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                    _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                    _unitOfWork.SaveChanges();
                                                    if (Editpower.civilLoads != null)
                                                    {

                                                        var existingEntity = _unitOfWork.CivilLoadsRepository
                                                            .GetAllAsQueryable()
                                                            .AsNoTracking()
                                                            .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                        TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                          .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                          .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                          .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                        NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                        NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                        NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                        NewpowerInst.sideArm2Id = null;
                                                        NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                        NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                        NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                        NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                        _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                        _unitOfWork.SaveChanges();

                                                    }

                                                    if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
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
                            else if (Editpower.installationConfig.civilSteelType == 2)
                            {
                                if (Editpower.installationConfig.civilNonSteelId != null)
                                {
                                    AllcivilinstId = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId ==
                                       Editpower.installationConfig.civilNonSteelId && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                                       x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                    if (AllcivilinstId != null)
                                    {
                                        if (Editpower.installationConfig.sideArmId != null)
                                        {
                                            if (Editpower.installationConfig.legId != null)
                                                return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not selected leg because installation place is sidearm ", (int)ApiReturnCode.fail);

                                            var SideArm = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId ==
                                              Editpower.installationConfig.civilNonSteelId && !x.Dismantle && x.sideArmId == Editpower.installationConfig.sideArmId, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                              x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);
                                            if (SideArm != null)
                                            {
                                                if (!string.IsNullOrEmpty(power.SerialNumber))
                                                {
                                                    bool CheckSerialNumber = _dbContext.MV_POWER_VIEW.Any(x => x.SerialNumber == power.SerialNumber && !x.Dismantle && x.Id != x.Id);
                                                    if (CheckSerialNumber)
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The Serial Number {power.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                                }
                                                var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivils(AllcivilinstId.allCivilInst).Message;

                                                if (Message != "Success")
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(true, null, null, Message, (int)ApiReturnCode.fail);
                                                }
                                                if (power.CenterHigh <= 0)
                                                {
                                                    if (power.HBA <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HBA_Surface must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    if (powerInst.allLoadInst.power.powerLibrary.Length <= 0)
                                                    {
                                                        return new Response<GetForAddMWDishInstallationObject>(false, null, null, "CenterHigh must bigger from zero", (int)ApiReturnCode.fail);
                                                    }
                                                    else
                                                    {
                                                        power.CenterHigh = power.HBA + powerInst.allLoadInst.power.powerLibrary.Length / 2;
                                                    }
                                                }
                                                if (power.SpaceInstallation == 0)
                                                {
                                                    if (powerInst.allLoadInst.power.powerLibrary.SpaceLibrary == 0)
                                                    {
                                                        if (powerInst.allLoadInst.power.powerLibrary.Length == 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        if (powerInst.allLoadInst.power.powerLibrary.width == 0)
                                                        {
                                                            return new Response<GetForAddMWDishInstallationObject>(false, null, null, "SpaceInstallation must bigger from zero", (int)ApiReturnCode.fail);
                                                        }
                                                        else
                                                        {
                                                            power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.Length * powerInst.allLoadInst.power.powerLibrary.width;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        power.SpaceInstallation = powerInst.allLoadInst.power.powerLibrary.SpaceLibrary;
                                                    }
                                                }

                                                if (Editpower.installationAttributes.Azimuth <= 0)
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "Azimuth must bigger from zero", (int)ApiReturnCode.fail);
                                                }
                                                if (Editpower.installationAttributes.HeightBase <= 0)
                                                {
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                                }
                                                var CheckAzimuthAndHeightBase = _dbContext.MV_POWER_VIEW.Where(
                                                        x => x.ALLCIVILINST_ID == AllcivilinstId.allCivilInst.Id &&
                                                        x.SIDEARM_ID == Editpower.installationConfig.sideArmId && x.Id != power.Id
                                                        && x.Azimuth == power.Azimuth && x.HeightBase == power.HeightBase && !x.Dismantle)
                                                  .GroupBy(x => new { x.ALLCIVILINST_ID, x.SIDEARM_ID, x.SiteCode, x.Azimuth, x.HeightBase })
                                                              .Select(g => g.First())
                                                              .ToList();

                                                if (CheckAzimuthAndHeightBase.Count > 0)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, "can not installed the dish on same azimuth and height because found other dish in same angle", (int)ApiReturnCode.fail);

                                                var SideArmName = _unitOfWork.SideArmRepository.GetWhereFirst(x => x.Id == Editpower.installationConfig.sideArmId);
                                                if (SideArmName != null && power.Azimuth > 0 && power.HeightBase > 0)
                                                {
                                                    power.Name = SideArmName?.Name + " " + power.HeightBase+"HE";
                                            }

                                                var CheckName = _dbContext.MV_POWER_VIEW.FirstOrDefault(x =>
                                                         !x.Dismantle && x.Id != power.Id &&
                                                         x.Name.ToLower() == power.Name.ToLower() &&
                                                         x.SiteCode.ToLower() == AllcivilinstId.SiteCode.ToLower());

                                                if (CheckName != null)
                                                    return new Response<GetForAddMWDishInstallationObject>(false, null, null, $"The name {power.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                power.powerLibraryId = Editpower.civilType.powerLibraryId;
                                                power.installationPlaceId = Editpower.installationConfig.InstallationPlaceId;
                                                _unitOfWork.PowerRepository.UpdateWithHistory(UserId, powerInst.allLoadInst.power, power);
                                                _unitOfWork.SaveChanges();
                                                if (Editpower.civilLoads != null)
                                                {

                                                    var existingEntity = _unitOfWork.CivilLoadsRepository
                                                        .GetAllAsQueryable()
                                                        .AsNoTracking()
                                                        .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);


                                                    TLIcivilLoads NewpowerInst = _dbContext.TLIcivilLoads
                                                      .Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.power.powerLibrary).Include(x => x.allCivilInst)
                                                      .Include(x => x.allCivilInst.civilNonSteel).Include(x => x.allCivilInst.civilWithLegs).Include(x => x.allCivilInst.civilWithoutLeg)
                                                      .FirstOrDefault(x => x.allLoadInstId != null && x.allLoadInst.powerId == power.Id && !x.Dismantle);

                                                    NewpowerInst.allCivilInstId = AllcivilinstId.allCivilInst.Id;
                                                    NewpowerInst.InstallationDate = Editpower.civilLoads.InstallationDate;
                                                    NewpowerInst.sideArmId = Editpower.installationConfig?.sideArmId ?? null;
                                                    NewpowerInst.sideArm2Id = null;
                                                    NewpowerInst.legId = Editpower.installationConfig?.legId ?? null;
                                                    NewpowerInst.ItemOnCivilStatus = Editpower.civilLoads.ItemOnCivilStatus;
                                                    NewpowerInst.ItemStatus = Editpower.civilLoads?.ItemStatus;
                                                    NewpowerInst.ReservedSpace = Editpower.civilLoads.ReservedSpace;
                                                    _unitOfWork.CivilLoadsRepository.UpdateWithHistory(UserId, existingEntity, NewpowerInst);
                                                    _unitOfWork.SaveChanges();

                                                }

                                                if (Editpower.dynamicAttribute != null ? Editpower.dynamicAttribute.Count() > 0 : false)
                                                    _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, Editpower.dynamicAttribute, TableNameId, power.Id, ConnectionString);
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
                   
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(ConnectionString));
                   
                    return new Response<GetForAddMWDishInstallationObject>();
                }
                catch (Exception err)
                {
                    return new Response<GetForAddMWDishInstallationObject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                }
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
