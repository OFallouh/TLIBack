using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AllLoadInstDTOs;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RuleDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using AutoMapper;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;

namespace TLIS_Service.Services
{
    public class LoadOtherService : ILoadOtherService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private readonly ApplicationDbContext _dbContext;
        private IMapper _mapper;
        public LoadOtherService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext dbContext,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _dbContext = dbContext;
            _mapper = mapper;
        }
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
            string MainTableName = TablesNames.TLIloadOther.ToString();
            AddLoadOtherViewModel AddLoadOtherInstallationViewModel = _mapper.Map<AddLoadOtherViewModel>(Input);

            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                    , x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
            {
                TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                    (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)) &&
                        x.OperationId != null, x => x.Operation);

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

                                object TestValue = AddLoadOtherInstallationViewModel.GetType().GetProperties()
                                    .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddLoadOtherInstallationViewModel, null);

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
                                AddDynamicAttInstValueViewModel DynamicObject = AddLoadOtherInstallationViewModel.TLIdynamicAttInstValue
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
                                object CivilLoads = AddLoadOtherInstallationViewModel.GetType().GetProperty(Path[0])
                                    .GetValue(AddLoadOtherInstallationViewModel, null);

                                CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                    (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                            }
                            else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                            {
                                CheckId = SiteCode;
                            }
                            else if (Path.Count() == 1)
                            {
                                if (AddLoadOtherInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(AddLoadOtherInstallationViewModel, null) != null)
                                    CheckId = (int)AddLoadOtherInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(AddLoadOtherInstallationViewModel, null);
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

                        AddDynamicAttInstValueViewModel InputDynamicAttribute = AddLoadOtherInstallationViewModel.TLIdynamicAttInstValue
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
        public Response<ObjectInstAtts> AddLoadOther(AddLoadOtherViewModel LoadOtherViewModel, string SiteCode, string ConnectionString, int? TaskId)
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
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == LoadSubType.TLIloadOther.ToString());
                            var LoadOtherEntity = _mapper.Map<TLIloadOther>(LoadOtherViewModel);
                            if (LoadOtherViewModel.TLIcivilLoads.ReservedSpace == true)
                            {
                                var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivil(LoadOtherViewModel.TLIcivilLoads.allCivilInstId).Message;
                                if (Message != "Success")
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            var loadOtherLibrary = _dbContext.TLIloadOtherLibrary.Where(x => x.Id == LoadOtherViewModel.loadOtherLibraryId).FirstOrDefault();
                            if (LoadOtherEntity.CenterHigh == 0)
                            {
                                LoadOtherEntity.CenterHigh = LoadOtherEntity.HBA + loadOtherLibrary.Length / 2;
                            }
                            var test = true;

                            TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                !x.allLoadInst.Draft && (x.allLoadInst.loadOtherId != null ? x.allLoadInst.loadOther.Name.ToLower() == LoadOtherEntity.Name.ToLower() : false) : false) &&
                                x.SiteCode.ToLower() == SiteCode.ToLower(),
                                    x => x.allLoadInst, x => x.allLoadInst.loadOther);

                            if (CheckName != null)
                                return new Response<ObjectInstAtts>(true, null, null, $"This name {LoadOtherEntity.Name} is already exists", (int)ApiReturnCode.fail);

                            string CheckDependencyValidation = this.CheckDependencyValidation(LoadOtherViewModel, SiteCode);

                            if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                            string CheckGeneralValidation = CheckGeneralValidationFunction(LoadOtherViewModel.TLIdynamicAttInstValue, TableNameEntity.TableName);

                            if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                            if (test == true)
                            {
                                _unitOfWork.LoadOtherRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, LoadOtherEntity);
                                _unitOfWork.SaveChanges();
                                var Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLIloadOther.ToString(), LoadOtherEntity.Id);
                                _unitOfWork.CivilLoadsRepository.AddCivilLoad(LoadOtherViewModel.TLIcivilLoads, Id, SiteCode);
                                if (LoadOtherViewModel.TLIdynamicAttInstValue.Count() > 0)
                                {
                                    foreach (var DynamicAttInstValue in LoadOtherViewModel.TLIdynamicAttInstValue)
                                    {
                                        _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, LoadOtherEntity.Id);
                                    }
                                }
                            }
                            else
                            {
                                return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
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
        public async Task<Response<ObjectInstAtts>> EditLoadOther(EditLoadOtherViewModel LoadOtherViewModel, int? TaskId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                        x.allLoadInst.loadOtherId == LoadOtherViewModel.Id : false), x => x.allLoadInst);

                    string SiteCode = "";

                    if (CivilLoads != null)
                        SiteCode = CivilLoads.SiteCode;

                    else
                        SiteCode = null;

                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && x.allLoadInst.loadOtherId != LoadOtherViewModel.Id && (x.allLoadInstId != null ?
                        !x.allLoadInst.Draft && (x.allLoadInst.loadOtherId != null ? x.allLoadInst.loadOther.Name.ToLower() == LoadOtherViewModel.Name.ToLower() : false) : false) &&
                        x.SiteCode.ToLower() == CivilLoads.SiteCode.ToLower(),
                            x => x.allLoadInst, x => x.allLoadInst.loadOther);

                    if (CheckName != null)
                        return new Response<ObjectInstAtts>(true, null, null, $"This name [{LoadOtherViewModel.Name}] is already exists", (int)ApiReturnCode.fail);

                    string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(LoadOtherViewModel.DynamicInstAttsValue, TablesNames.TLIloadOther.ToString());

                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                    string CheckDependencyValidation = CheckDependencyValidationEditVersion(LoadOtherViewModel, SiteCode);

                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                    int TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TablesNames.TLIloadOther.ToString()).Id;

                    TLIloadOther LoadOtherEntity = _mapper.Map<TLIloadOther>(LoadOtherViewModel);

                    TLIloadOther OldloadOther = _unitOfWork.LoadOtherRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == LoadOtherViewModel.Id);
                    if (LoadOtherEntity.HBA != OldloadOther.HBA || LoadOtherEntity.CenterHigh != OldloadOther.CenterHigh || LoadOtherEntity.SpaceInstallation != OldloadOther.SpaceInstallation && LoadOtherViewModel.TLIcivilLoads.ReservedSpace == true)
                    {
                        var loadOtherLibrary = _dbContext.TLIloadOtherLibrary.Where(x => x.Id == LoadOtherEntity.loadOtherLibraryId).FirstOrDefault();
                        if (LoadOtherEntity.CenterHigh == 0)
                        {
                            LoadOtherEntity.CenterHigh = LoadOtherEntity.HBA + loadOtherLibrary.Length / 2;
                        }
                        if (LoadOtherViewModel.TLIcivilLoads.ReservedSpace == true && (LoadOtherViewModel.TLIcivilLoads.sideArmId == null || LoadOtherViewModel.TLIcivilLoads.sideArmId == 0))
                        {
                            LoadOtherEntity.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(LoadOtherViewModel.TLIcivilLoads.allCivilInstId, Helpers.Constants.TablesNames.TLIloadOther.ToString(), LoadOtherEntity.SpaceInstallation, LoadOtherEntity.CenterHigh, LoadOtherEntity.loadOtherLibraryId, LoadOtherEntity.HBA).Data;
                        }
                    }

                    _unitOfWork.LoadOtherRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldloadOther, LoadOtherEntity);
                    await _unitOfWork.SaveChangesAsync();
                    var allloads = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.loadOtherId == LoadOtherViewModel.Id).Id;
                    var civilloads = _unitOfWork.CivilLoadsRepository.GetWhereFirst(x => x.allLoadInstId == allloads);
                    CivilLoads.InstallationDate = LoadOtherViewModel.TLIcivilLoads.InstallationDate;
                    CivilLoads.ItemOnCivilStatus = LoadOtherViewModel.TLIcivilLoads.ItemOnCivilStatus;
                    CivilLoads.ItemStatus = LoadOtherViewModel.TLIcivilLoads.ItemStatus;
                    CivilLoads.ReservedSpace = LoadOtherViewModel.TLIcivilLoads.ReservedSpace;
                    CivilLoads.sideArmId = LoadOtherViewModel.TLIcivilLoads.sideArmId;
                    CivilLoads.allCivilInstId = LoadOtherViewModel.TLIcivilLoads.allCivilInstId;
                    CivilLoads.legId = LoadOtherViewModel.TLIcivilLoads.legId;
                    CivilLoads.Leg2Id = LoadOtherViewModel.TLIcivilLoads.Leg2Id;

                    _unitOfWork.SaveChanges();
                    if (LoadOtherViewModel.DynamicInstAttsValue.Count > 0)
                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(LoadOtherViewModel.DynamicInstAttsValue, TableNameId, LoadOtherEntity.Id);
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
            string MainTableName = TablesNames.TLIloadOther.ToString();
            EditLoadOtherViewModel EditInstallationViewModel = _mapper.Map<EditLoadOtherViewModel>(Input);

            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                    , x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
            {
                TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                    (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)) &&
                        x.OperationId != null, x => x.Operation);

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
        private async Task UpdateDynamicInstAttsValue(List<DynamicAttInstValueViewModel> dynamicAttInstValueViews)
        {
            foreach (var atts in dynamicAttInstValueViews)
            {
                await _unitOfWork.DynamicAttInstValueRepository.UpdateItem(atts);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        public Response<ObjectInstAtts> GetAttForAdd(int LibId, string SiteCode)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l =>
                    l.TableName == LoadSubType.TLIloadOther.ToString());

                ObjectInstAtts objectInst = new ObjectInstAtts();

                LoadOtherLibraryViewModel LoadOtherLibrary = _mapper.Map<LoadOtherLibraryViewModel>(_unitOfWork.LoadOtherLibraryRepository.GetByID(LibId));

                List<BaseAttView> LibraryAttributeActivated = _unitOfWork.AttributeActivatedRepository.
                    GetAttributeActivated(TablesNames.TLIloadOtherLibrary.ToString(), LoadOtherLibrary).ToList();

                List<BaseAttView> AddToLibraryAttributesActivated = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                    .GetLogistical(TablePartName.LoadOther.ToString(), TablesNames.TLIloadOtherLibrary.ToString(), LoadOtherLibrary.Id).ToList());

                LibraryAttributeActivated.AddRange(AddToLibraryAttributesActivated);

                objectInst.LibraryActivatedAttributes = LibraryAttributeActivated;

                List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                    GetInstAttributeActivated(LoadSubType.TLIloadOther.ToString(), null, "loadOtherLibraryId"/*, "EquivalentSpace"*/, "InstallationPlaceId").ToList();

                BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                if (NameAttribute != null)
                {
                    BaseInstAttView Swap = ListAttributesActivated[0];
                    ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                    ListAttributesActivated[0] = NameAttribute;
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
                    .GetInstAttributeActivated(TablesNames.TLIcivilLoads.ToString(), null, "allLoadInstId", "Dismantle", "SiteCode",
                        "legId", "Leg2Id", "sideArmId", "allCivilInstId", "civilSteelSupportCategoryId");

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
                                        var PortCascade = _dbContext.TLImwPort.Where(x => x.Id == PortCascadeId).Select(x => x.MwBUId).ToList();
                                        foreach (var Port in PortCascade)
                                        {
                                            var allload = _dbContext.TLIallLoadInst.Where(x => x.mwBUId == Port).Select(x => x.Id).FirstOrDefault();
                                            var Civilloads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allload && x.allCivilInstId == t.Id && x.Dismantle == false).FirstOrDefault();
                                            if (Civilloads != null)
                                            {
                                                Civilloads.Dismantle = true;
                                                EquivalentSpace += 0;
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
                                        var PortCascade = _dbContext.TLImwPort.Where(x => x.Id == PortCascadeId).Select(x => x.MwBUId).ToList();
                                        foreach (var Port in PortCascade)
                                        {
                                            var allload = _dbContext.TLIallLoadInst.Where(x => x.mwBUId == Port).Select(x => x.Id).FirstOrDefault();
                                            var Civilloads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allload && x.allCivilInstId == t.Id && x.Dismantle == false).FirstOrDefault();
                                            if (Civilloads != null)
                                            {
                                                Civilloads.Dismantle = true;
                                                EquivalentSpace += 0;
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
        public Response<ObjectInstAttsForSideArm> GetById(int Id)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == LoadSubType.TLIloadOther.ToString());
                ObjectInstAttsForSideArm objectInst = new ObjectInstAttsForSideArm();

                TLIloadOther LoadOther = _unitOfWork.LoadOtherRepository
                        .GetIncludeWhereFirst(x => x.Id == Id, x => x.loadOtherLibrary);

                LoadOtherLibraryViewModel loadotherlib = _mapper.Map<LoadOtherLibraryViewModel>(_unitOfWork.LoadOtherLibraryRepository
                       .GetIncludeWhereFirst(x => x.Id == LoadOther.loadOtherLibraryId));
                List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                   .GetAttributeActivated(TablesNames.TLIloadOtherLibrary.ToString(), loadotherlib, null).ToList();

                foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                {
                    if (LibraryAttribute.DataType.ToLower() == "list")
                    {
                        LibraryAttribute.Value = loadotherlib.GetType().GetProperties()
                            .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(loadotherlib);
                    }
                }
                List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                    .GetLogistical(Helpers.Constants.TablePartName.LoadOther.ToString(), TablesNames.TLIloadOtherLibrary.ToString(), loadotherlib.Id).ToList());

                LibraryAttributes.AddRange(LogisticalAttributes);
                objectInst.LibraryActivatedAttributes = LibraryAttributes;

                List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                    .GetInstAttributeActivated(LoadSubType.TLIloadOther.ToString(), LoadOther
                    , "InstallationPlaceId").ToList();

                BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                if (NameAttribute != null)
                {
                    BaseInstAttView Swap = ListAttributesActivated[0];
                    ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                    ListAttributesActivated[0] = NameAttribute;
                }
                foreach (BaseInstAttView FKitem in ListAttributesActivated)
                {
                    if (FKitem.Desc.ToLower() == "tliloadotherlibrary")
                    {
                        if (LoadOther.loadOtherLibrary == null)
                            FKitem.Value = "NA";

                        else
                            FKitem.Value = LoadOther.loadOtherLibrary.Model;
                    }
                }
                objectInst.AttributesActivated = ListAttributesActivated;

                objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                    .GetDynamicInstAtts(TableNameEntity.Id, Id, null);

                TLIallLoadInst AllLoadInst = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.loadOtherId == Id);

                TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.allLoadInstId == AllLoadInst.Id, x => x.sideArm, x => x.site,
                    x => x.leg, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
                    x => x.allCivilInst.civilNonSteel, x => x.civilSteelSupportCategory, x => x.allLoadInst.loadOther);

                List<KeyValuePair<string, List<DropDownListFilters>>> loadotherRelatedTables = _unitOfWork.LoadOtherRepository.GetRelatedTables();

                List<KeyValuePair<string, List<DropDownListFilters>>> CivilLoadsRelatedTables = _unitOfWork.CivilLoadsRepository
                    .GetRelatedTables(CivilLoads.SiteCode);

                loadotherRelatedTables.AddRange(CivilLoadsRelatedTables);

                if (CivilLoads.allCivilInst.civilWithLegsId != null)
                {
                    List<TLIleg> LegsForCivilWithLegLibrary = _unitOfWork.LegRepository
                        .GetWhere(x => x.CivilWithLegInstId == CivilLoads.allCivilInst.civilWithLegsId).ToList();

                    List<DropDownListFilters> LegIds = _mapper.Map<List<DropDownListFilters>>(LegsForCivilWithLegLibrary);

                    loadotherRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg1Id", LegIds));

                    List<TLIleg> Legs2ForCivilWithLegLibrary = LegsForCivilWithLegLibrary.Except(LegsForCivilWithLegLibrary
                        .Where(x => x.Id == CivilLoads.legId)).ToList();

                    List<DropDownListFilters> Leg2Ids = _mapper.Map<List<DropDownListFilters>>(Legs2ForCivilWithLegLibrary);

                    loadotherRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg2Id", Leg2Ids));
                }

                objectInst.RelatedTables = loadotherRelatedTables;

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

                List<BaseInstAttView> LoadOtherInstallationInfo = new List<BaseInstAttView>();
                if (CivilLoads != null)
                {
                    TLIallCivilInst AllCivilInst = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhereFirst(x => (x.allLoadInstId != null ? (x.allLoadInst.loadOtherId != null ?
                            x.allLoadInst.loadOtherId.Value == Id : false) : false) && !x.Dismantle, x => x.allCivilInst, x => x.allLoadInst).allCivilInst;

                    if (AllCivilInst.civilWithLegsId != null)
                    {
                        LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                        LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                        LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                            LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                            LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                        LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                        LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                        LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                            LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                        LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                        LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                        LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                            LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                    LoadOtherInstallationInfo.Add(new BaseInstAttView
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
                    objectInst.SideArmInstallationInfo = LoadOtherInstallationInfo;
                }

                return new Response<ObjectInstAttsForSideArm>(true, objectInst, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAttsForSideArm>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }

        public Response<ReturnWithFilters<LoadOtherViewModel>> GetLoadOtherList(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                var LoadOthers = new ReturnWithFilters<LoadOtherViewModel>();
                var Others = _unitOfWork.LoadOtherRepository.GetAllIncludeMultiple(parameters, filters, out count, null).ToList();
                LoadOthers.Model = _mapper.Map<List<LoadOtherViewModel>>(Others);
                if (WithFilterData)
                {
                    LoadOthers.filters = _unitOfWork.LoadOtherRepository.GetRelatedTables();
                }
                else
                {
                    LoadOthers.filters = null;
                }
                return new Response<ReturnWithFilters<LoadOtherViewModel>>(true, LoadOthers, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<LoadOtherViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }

        public Response<ReturnWithFilters<LoadsOtherDisplayedOnTableViewModel>> GetLoadsOtherBySite(LoadsOnSiteFilter BaseFilter, bool WithFilterData, List<FilterObjectList> ObjectAttributeFilters, ParameterPagination parameterPagination)
        {
            try
            {
                List<LoadsOtherDisplayedOnTableViewModel> OutPutList = new List<LoadsOtherDisplayedOnTableViewModel>();
                ReturnWithFilters<LoadsOtherDisplayedOnTableViewModel> LoadsOtherDisplay = new ReturnWithFilters<LoadsOtherDisplayedOnTableViewModel>();
                int count = 0;

                List<TLIcivilLoads> CivilLoads = new List<TLIcivilLoads>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();
                        string key = item.key;
                        AttributeFilters.Add(new StringFilterObjectList
                        {
                            key = key,
                            value = value
                        });
                    }
                }
                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    var InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        !x.LibraryAtt &&
                        x.tablesNames.TableName == "TLIloadOther", x => x.tablesNames).ToList();

                    List<int> DynamicInstValueListIds = new List<int>();
                    bool DynamicInstExist = false;
                    if (InstDynamicAttListIds.Count() > 0)
                    {
                        List<StringFilterObjectList> DynamicInstAttributeFilters = AttributeFilters.Where(x =>
                            InstDynamicAttListIds.Exists(y => y.Key.ToLower() == x.key.ToLower())).ToList();
                        DynamicInstExist = true;
                        DynamicInstValueListIds = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            InstDynamicAttListIds.Exists(y => y.Id == x.DynamicAttId) ? (
                                DynamicInstAttributeFilters.All(y =>
                                    x.ValueBoolean != null ? (
                                        y.value.Contains(x.ValueBoolean.ToString().ToLower()))
                                    :
                                    x.ValueDateTime != null ? (
                                        y.value.Contains(x.ValueDateTime.ToString())
                                    )
                                    :
                                    x.ValueDouble != null ? (
                                        y.value.Contains(x.ValueDouble.ToString())
                                    )
                                    :
                                    x.ValueString != null ? (
                                        y.value.Any(z => x.ValueString.ToLower().StartsWith(z.ToLower()) || z.ToLower() == x.ValueString.ToLower())
                                    )
                                    : false
                                )) : false
                        ).Select(i => i.InventoryId).ToList();
                    }

                    //
                    // Library Dynamic Attributes...
                    //
                    var LibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.Exists(y => y.key.ToLower() == x.Key.ToLower()) &&
                        x.LibraryAtt &&
                        x.tablesNames.TableName == "TLIloadOtherLibrary", x => x.tablesNames).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;
                    if (LibDynamicAttListIds.Count > 0)
                    {
                        List<StringFilterObjectList> DynamicLibAttributeFilters = AttributeFilters.Where(x =>
                            LibDynamicAttListIds.Exists(y => y.Key.ToLower() == x.key.ToLower())).ToList();
                        DynamicLibExist = true;
                        DynamicLibValueListIds = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            LibDynamicAttListIds.Exists(y => y.Id == x.DynamicAttId) ? (
                                DynamicLibAttributeFilters.All(y =>
                                    x.ValueBoolean != null ? (
                                        y.value.Contains(x.ValueBoolean.ToString().ToLower()))
                                    :
                                    x.ValueDateTime != null ? (
                                        y.value.Contains(x.ValueDateTime.ToString())
                                    )
                                    :
                                    x.ValueDouble != null ? (
                                        y.value.Contains(x.ValueDouble.ToString())
                                    )
                                    :
                                    x.ValueString != null ? (
                                        y.value.Any(z => x.ValueString.ToLower().StartsWith(z.ToLower()) || z.ToLower() == x.ValueString.ToLower())
                                    )
                                    : false
                                )) : false
                        ).Select(i => i.InventoryId).ToList();
                    }

                    //
                    // Installation Attribute Activated...
                    //
                    bool AttrInstExist = typeof(LoadOtherViewModel).GetProperties().ToList().Exists(x =>
                        ObjectAttributeFilters.Exists(y =>
                            y.key == x.Name));

                    List<PropertyInfo> NotStringProps = typeof(LoadOtherViewModel).GetProperties().Where(x =>
                        x.PropertyType.Name != "String" &&
                        ObjectAttributeFilters.Exists(y => y.key.ToLower() == x.Name.ToLower())).ToList();

                    List<PropertyInfo> StringProps = typeof(LoadOtherViewModel).GetProperties().Where(x =>
                        x.PropertyType.Name == "String" &&
                        ObjectAttributeFilters.Exists(y => y.key.ToLower() == x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Exists(y => y.Name.ToLower() == x.key.ToLower()) ||
                            StringProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();
                        InstallationAttributeActivated = _unitOfWork.LoadOtherRepository.GetIncludeWhere(x =>
                            AttrInstAttributeFilters.All(z =>
                                NotStringProps.Exists(y => z.value.Contains(y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null).ToString().ToLower())) ||
                                StringProps.Exists(y => z.value.Any(w =>
                                    w == y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null).ToString().ToLower() ||
                                    y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()))))
                        ).Select(i => i.Id).ToList();
                    }

                    //
                    // Library Attribute Activated...
                    //
                    bool AttrLibExist = typeof(LoadOtherLibraryViewModel).GetProperties().ToList().Exists(x =>
                        ObjectAttributeFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower() && y.key.ToLower() != "id"));

                    List<PropertyInfo> NotStringLibraryProps = typeof(LoadOtherLibraryViewModel).GetProperties().Where(x =>
                        x.PropertyType.Name != "String" &&
                        ObjectAttributeFilters.Exists(y => y.key.ToLower() == x.Name.ToLower())).ToList();

                    List<PropertyInfo> StringLibraryProps = typeof(LoadOtherLibraryViewModel).GetProperties().Where(x =>
                        x.PropertyType.Name == "String" &&
                        ObjectAttributeFilters.Exists(y => y.key.ToLower() == x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivated = new List<int>();

                    if (AttrLibExist)
                    {
                        List<StringFilterObjectList> LibraryAttAttributeFilters = AttributeFilters.Where(x =>
                            NotStringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower()) ||
                            StringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                        LibraryAttributeActivated = _unitOfWork.LoadOtherLibraryRepository.GetIncludeWhere(x =>
                            LibraryAttAttributeFilters.All(z =>
                                NotStringLibraryProps.Exists(y => z.value.Contains(y.GetValue(_mapper.Map<LoadOtherLibraryViewModel>(x), null).ToString().ToLower())) ||
                                StringLibraryProps.Exists(y => z.value.Any(w =>
                                    w == y.GetValue(_mapper.Map<LoadOtherLibraryViewModel>(x), null).ToString().ToLower() ||
                                    y.GetValue(_mapper.Map<LoadOtherLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()))))
                        ).Select(i => i.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectInstallationIds = new List<int>();
                    if (AttrInstExist && DynamicInstExist)
                    {
                        IntersectInstallationIds = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        IntersectInstallationIds = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        IntersectInstallationIds = DynamicInstValueListIds;
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

                    //
                    // Both (Installation + Library) Attributes...
                    //
                    List<int> LoadsOtherIds = new List<int>();

                    if ((AttrInstExist || DynamicInstExist) && (AttrLibExist || DynamicLibExist))
                    {
                        LoadsOtherIds = _unitOfWork.LoadOtherRepository.GetIncludeWhere(x =>
                            IntersectInstallationIds.Contains(x.Id) &&
                            IntersectLibraryIds.Contains(x.loadOtherLibraryId)
                        ).Select(i => i.Id).ToList();
                    }
                    else if (AttrInstExist || DynamicInstExist)
                    {
                        LoadsOtherIds = _unitOfWork.LoadOtherRepository.GetIncludeWhere(x =>
                            IntersectInstallationIds.Contains(x.Id)
                        ).Select(i => i.Id).ToList();
                    }
                    else if (AttrLibExist || DynamicLibExist)
                    {
                        LoadsOtherIds = _unitOfWork.LoadOtherRepository.GetIncludeWhere(x =>
                            IntersectLibraryIds.Contains(x.loadOtherLibraryId)
                        ).Select(i => i.Id).ToList();
                    }

                    if (parameterPagination == null)
                    {
                        CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                            (x.SiteCode == BaseFilter.SiteCode) &&
                            (BaseFilter.ItemStatusId != null ? (
                                x.allLoadInst != null ? (
                                    x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                    : true)
                                : true)
                            : true) &&
                            (BaseFilter.TicketId != null ? (
                                x.allLoadInst != null ? (
                                    x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                    : true)
                                : true)
                            : true) &&
                            (BaseFilter.AllCivilId != null ?
                                (x.allCivilInstId == BaseFilter.AllCivilId)
                            : true) &&
                            LoadsOtherIds.Contains(x.Id)
                            , x => x.allLoadInst, x => x.allCivilInst
                        ).ToList();
                    }
                    else
                    {
                        CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                            (x.SiteCode == BaseFilter.SiteCode) &&
                            (BaseFilter.ItemStatusId != null ? (
                                x.allLoadInst != null ? (
                                    x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                    : true)
                                : true)
                            : true) &&
                            (BaseFilter.TicketId != null ? (
                                x.allLoadInst != null ? (
                                    x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                    : true)
                                : true)
                            : true) &&
                            (BaseFilter.AllCivilId != null ?
                                (x.allCivilInstId == BaseFilter.AllCivilId)
                            : true) &&
                            LoadsOtherIds.Contains(x.Id)
                            , x => x.allLoadInst, x => x.allCivilInst).
                        Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                        Take(parameterPagination.PageSize).AsQueryable().ToList();
                    }
                }
                else
                {
                    if (parameterPagination == null)
                    {
                        CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                            (x.SiteCode == BaseFilter.SiteCode) &&
                            (BaseFilter.ItemStatusId != null ? (
                                x.allLoadInst != null ? (
                                    x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                    : true)
                                : true)
                            : true) &&
                            (BaseFilter.TicketId != null ? (
                                x.allLoadInst != null ? (
                                    x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                    : true)
                                : true)
                            : true) &&
                            (BaseFilter.AllCivilId != null ?
                                (x.allCivilInstId == BaseFilter.AllCivilId)
                            : true)
                            , x => x.allLoadInst, x => x.allCivilInst
                        ).ToList();
                    }
                    else
                    {
                        CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                            (x.SiteCode == BaseFilter.SiteCode) &&
                            (BaseFilter.ItemStatusId != null ? (
                                x.allLoadInst != null ? (
                                    x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                    : true)
                                : true)
                            : true) &&
                            (BaseFilter.TicketId != null ? (
                                x.allLoadInst != null ? (
                                    x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                    : true)
                                : true)
                            : true) &&
                            (BaseFilter.AllCivilId != null ?
                                (x.allCivilInstId == BaseFilter.AllCivilId)
                            : true)
                            , x => x.allLoadInst, x => x.allCivilInst).
                        Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                        Take(parameterPagination.PageSize).AsQueryable().ToList();
                    }
                }

                // Delete Duplicated Objects Based On Installation Date...
                List<TLIcivilLoads> NewList = new List<TLIcivilLoads>();
                foreach (var item in CivilLoads)
                {
                    var CheckIfExist = NewList.FirstOrDefault(x => x.allLoadInst.loadOtherId.Value == item.allLoadInst.loadOtherId.Value);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                    {
                        NewList.Add(item);
                    }
                }
                CivilLoads = NewList;

                foreach (var CivilLoad in CivilLoads)
                {
                    if (CivilLoad.allLoadInstId != null)
                    {
                        TLIallLoadInst allLoadsInst = _unitOfWork.AllLoadInstRepository.GetIncludeWhere(x => x.Id == CivilLoad.allLoadInstId.Value, x => x.loadOtherId).FirstOrDefault();

                        //
                        // Installation + Libraries + Dynamic Att...
                        //
                        List<DynamicAttDto> DynamicAttListCopy = new List<DynamicAttDto>();
                        LoadsOtherDisplayedOnTableViewModel OutPut = new LoadsOtherDisplayedOnTableViewModel();

                        // Installation Object + Dynamic Attributes For This Installation Object..
                        OutPut.LoadsOtherInstallation = _unitOfWork.LoadOtherRepository.Get(allLoadsInst.loadOtherId.Value);

                        List<TLIdynamicAttInstValue> DynamicAttInstValueRecords = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x => (x.DynamicAtt.disable == false &&
                            x.DynamicAtt.tablesNames.TableName == "TLIloadOther" &&
                            x.InventoryId == OutPut.LoadsOtherInstallation.Id &&
                            !x.DynamicAtt.LibraryAtt), x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                        foreach (var DynamicAttInstValueRecord in DynamicAttInstValueRecords)
                        {
                            var DynamicAttDtoObject = GetDynamicAttDto(DynamicAttInstValueRecord, null);
                            DynamicAttListCopy.Add(DynamicAttDtoObject);
                        }

                        // Library Object + Dynamic Attributes For This Library Object..
                        OutPut.LoadsOtherLibrary = _unitOfWork.LoadOtherLibraryRepository.Get(OutPut.LoadsOtherInstallation.loadOtherLibraryId);

                        List<TLIdynamicAttLibValue> DynamicAttLibRecords = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x => (
                            x.DynamicAtt.disable == false && x.DynamicAtt.tablesNames.TableName == "TLIloadOtherLibrary" &&
                            x.InventoryId == OutPut.LoadsOtherInstallation.loadOtherLibraryId && x.DynamicAtt.LibraryAtt),
                            x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                        foreach (var DynamicAttLibRecord in DynamicAttLibRecords)
                        {
                            var DynamicAttDtoObject = GetDynamicAttDto(null, DynamicAttLibRecord);
                            DynamicAttListCopy.Add(DynamicAttDtoObject);
                        }
                        OutPut.DynamicAttList = DynamicAttListCopy;

                        // TicketId..
                        if (allLoadsInst.TicketId != null)
                        {
                            OutPut.TicketId = allLoadsInst.TicketId;
                        }

                        // ItemStatusId + ItemStatus Object..
                        if (allLoadsInst.ItemStatusId != null)
                        {
                            OutPut.ItemStatusId = allLoadsInst.ItemStatusId;
                            OutPut.ItemStatus = _unitOfWork.ItemStatusRepository.GetByID(OutPut.ItemStatusId.Value);
                        }

                        OutPutList.Add(OutPut);
                    }
                }
                LoadsOtherDisplay.Model = OutPutList;
                if (WithFilterData)
                {
                    LoadsOtherDisplay.filters = _unitOfWork.LoadOtherRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<LoadsOtherDisplayedOnTableViewModel>>(true, LoadsOtherDisplay, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<LoadsOtherDisplayedOnTableViewModel>>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        #region Helper Method..
        public void GetInventoriesIdsFromDynamicAttributes(out List<int> DynamicInstValueListIds, List<TLIdynamicAtt> InstDynamicAttListIds, List<StringFilterObjectList> AttributeFilters)
        {
            try
            {
                List<StringFilterObjectList> DynamicInstAttributeFilters = AttributeFilters.Where(x =>
                    InstDynamicAttListIds.Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                DynamicInstValueListIds = new List<int>();

                List<TLIdynamicAttInstValue> DynamicInstValueListObjects = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                    InstDynamicAttListIds.Select(y => y.Id).Contains(x.DynamicAttId) && !x.disable).ToList();

                List<int> InventoriesIds = DynamicInstValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                foreach (int InventoryId in InventoriesIds)
                {
                    List<TLIdynamicAttInstValue> DynamicInstValueListInventories = DynamicInstValueListObjects.Where(x => x.InventoryId == InventoryId).ToList();

                    if (DynamicInstAttributeFilters.All(y => DynamicInstValueListInventories.Exists(x =>
                         (x.ValueBoolean != null) ?
                            (y.value.Any(z => x.ValueBoolean.ToString().ToLower().StartsWith(z.ToLower()))) :

                         (x.ValueDateTime != null ?
                            (y.value.Any(z => z.ToLower() == x.ValueDateTime.ToString().ToLower())) :

                         (x.ValueDouble != null ?
                            (y.value.Any(z => z.ToLower() == x.ValueDouble.ToString().ToLower())) :

                         (!string.IsNullOrEmpty(x.ValueString) ?
                            (y.value.Any(z => x.ValueString.ToLower().StartsWith(z.ToLower()))) : (false)))))))
                    {
                        DynamicInstValueListIds.Add(InventoryId);
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
        public Response<ReturnWithFilters<object>> GetLoadOtherOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType)
        {
            try
            {
                int Count = 0;
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> LoadOtheresTableDisplay = new ReturnWithFilters<object>();

                List<TLIcivilLoads> AllCivilLoadsRecords = new List<TLIcivilLoads>();
                List<TLIcivilLoads> CivilLoadsRecords = new List<TLIcivilLoads>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();
                List<AttributeActivatedViewModel> OtherLoadInstallationAttribute = new List<AttributeActivatedViewModel>();

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    OtherLoadInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.OtherLoadInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == TablesNames.TLIloadOther.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<AttributeActivatedViewModel> NotDateDateOtherLoadInstallationAttribute = OtherLoadInstallationAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        AttributeActivatedViewModel AttributeKey = NotDateDateOtherLoadInstallationAttribute.FirstOrDefault(x =>
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

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<AttributeActivatedViewModel> DateOtherLoadInstallationAttribute = OtherLoadInstallationAttribute.Where(x =>
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

                        AttributeActivatedViewModel AttributeKey = DateOtherLoadInstallationAttribute.FirstOrDefault(x =>
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

                List<int> LoadOtherIds = new List<int>();
                List<int> WithoutDateFilterOtherLoadInstallation = new List<int>();
                List<int> WithDateFilterOtherLoadInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLIloadOther.ToString()
                            , x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicInstValueListIds = new List<int>();
                    bool DynamicInstExist = false;
                    if (InstDynamicAttListIds.Count() > 0)
                    {
                        DynamicInstExist = true;
                        GetInventoriesIdsFromDynamicAttributes(out DynamicInstValueListIds, InstDynamicAttListIds, AttributeFilters);
                    }

                    //
                    // Installation Attribute Activated...
                    //
                    bool AttrInstExist = typeof(LoadOtherViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(LoadOtherViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(LoadOtherViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //InstallationAttributeActivated = _unitOfWork.LoadOtherRepository.GetWhere(x =>
                        //    AttrInstAttributeFilters.All(z =>
                        //    NotStringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null).ToString().ToLower()) : false)) ||
                        //    StringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                        //            y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null) != null ? y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                        //).Select(i => i.Id).ToList();

                        IEnumerable<TLIloadOther> Installations = _unitOfWork.LoadOtherRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null) != null ? y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        InstallationAttributeActivated = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectInstallationIds = new List<int>();
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithoutDateFilterOtherLoadInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterOtherLoadInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterOtherLoadInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLIloadOther.ToString()
                            , x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicInstValueListIds = new List<int>();
                    bool DynamicInstExist = false;

                    if (DateTimeInstDynamicAttListIds.Count > 0)
                    {
                        DynamicInstExist = true;
                        List<DateFilterViewModel> DynamicInstAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeInstDynamicAttListIds.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                        DynamicInstValueListIds = new List<int>();

                        List<TLIdynamicAttInstValue> DynamicInstValueListObjects = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            DateTimeInstDynamicAttListIds.Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicInstValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttInstValue> DynamicInstValueListInventories = DynamicInstValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicInstAttributeFilters.All(y => DynamicInstValueListInventories.Select(x => x.ValueDateTime).Any(x =>
                                (x != null ?
                                    (x >= y.DateFrom && x <= y.DateTo) : (false)))))
                            {
                                DynamicInstValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Installation Attribute Activated...
                    //
                    List<PropertyInfo> InstallationProps = typeof(LoadOtherViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIloadOther> Installations = _unitOfWork.LoadOtherRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<LoadOtherViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterOtherLoadInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterOtherLoadInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterOtherLoadInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                            (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        LoadOtherIds = WithoutDateFilterOtherLoadInstallation.Intersect(WithDateFilterOtherLoadInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        LoadOtherIds = WithoutDateFilterOtherLoadInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        LoadOtherIds = WithDateFilterOtherLoadInstallation;
                    }

                    AllCivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.loadOtherId != null : false) &&
                        (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true) &&

                        LoadOtherIds.Contains(x.allLoadInst.loadOtherId.Value),
                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.loadOther, x => x.allLoadInst.loadOther.loadOtherLibrary).AsQueryable().ToList();
                    //Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    //Take(parameterPagination.PageSize).AsQueryable().ToList();
                }

                else
                {
                    AllCivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                       (x.allLoadInstId != null ? x.allLoadInst.loadOtherId != null : false) &&
                       (!x.Dismantle) &&
                       (x.SiteCode == BaseFilter.SiteCode) &&
                       (BaseFilter.ItemStatusId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.ItemStatusId != null ?
                                       (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.TicketId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.TicketId != null ?
                                       (x.allLoadInst.TicketId == BaseFilter.TicketId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.AllCivilId != null ?
                           (x.allCivilInstId == BaseFilter.AllCivilId)
                       : true),

                   x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.loadOther, x => x.allLoadInst.loadOther.loadOtherLibrary).AsQueryable().ToList();
                    //Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    //Take(parameterPagination.PageSize).AsQueryable().ToList();
                }

                // Delete Duplicated Objects Based On Installation Date...
                List<TLIcivilLoads> NewList = new List<TLIcivilLoads>();
                foreach (var item in AllCivilLoadsRecords)
                {
                    TLIcivilLoads CheckIfExist = NewList.FirstOrDefault(x => x.allLoadInst.loadOtherId.Value == item.allLoadInst.loadOtherId.Value);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                    {
                        NewList.Add(item);
                    }
                }
                CivilLoadsRecords = NewList;

                if (CivilId != null)
                {
                    TLIallCivilInst AllCivilInst = new TLIallCivilInst();

                    if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString().ToLower())
                    {
                        AllCivilInst = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilWithLegsId == CivilId);
                    }
                    else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString().ToLower())
                    {
                        AllCivilInst = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilWithoutLegId == CivilId);
                    }
                    else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString().ToLower())
                    {
                        AllCivilInst = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilNonSteelId == CivilId);
                    }

                    List<int> MW_LoadOthersIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.loadOtherId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.loadOtherId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_LoadOthersIds.Contains(x.allLoadInst.loadOtherId.Value)).ToList();
                }

                if (BaseFilter.SideArmId != null)
                {
                    List<int> MW_LoadOthersIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.sideArmId == BaseFilter.SideArmId && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.loadOtherId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.loadOtherId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_LoadOthersIds.Contains(x.allLoadInst.loadOtherId.Value)).ToList();
                }

                Count = CivilLoadsRecords.Count();

                CivilLoadsRecords = CivilLoadsRecords.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<LoadOtherViewModel> Others = _mapper.Map<List<LoadOtherViewModel>>(CivilLoadsRecords.Select(x => x.allLoadInst.loadOther).ToList());

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.OtherLoadInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == TablesNames.TLIloadOther.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == TablesNames.TLIloadOther.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == TablesNames.TLIloadOther.ToString()) : false),
                       x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
                       x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

                List<TLIattributeViewManagment> NotDateTimeInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime") : false).ToList();

                List<TLIattributeViewManagment> NotDateTimeDynamicInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() != "datetime" : false).ToList();

                List<TLIattributeViewManagment> DateTimeInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

                List<TLIattributeViewManagment> DateTimeDynamicInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() == "datetime" : false).ToList();


                foreach (LoadOtherViewModel OthersInstallationObject in Others)
                {
                    dynamic DynamiOtherLoadInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(LoadOtherViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in InstallationProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeInstallationAttributesViewModel.Select(x =>
                                    x.AttributeActivated.Label.ToLower()).Contains(prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(OthersInstallationObject, null);
                                ((IDictionary<String, Object>)DynamiOtherLoadInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == TablesNames.TLIloadOther.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(OthersInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamiOtherLoadInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(OthersInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiOtherLoadInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeDynamicInstallationAttributesViewModel != null ? NotDateTimeDynamicInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<TLIdynamicAtt> NotDateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                            !x.disable && x.tablesNames.TableName == TablesNames.TLIloadOther.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == OthersInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == TablesNames.TLIloadOther.ToString()
                                , x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                        foreach (TLIdynamicAtt InstallationDynamicAtt in NotDateTimeInstallationDynamicAttributes)
                        {
                            TLIdynamicAttInstValue DynamicAttInstValue = NotDateTimeDynamicAttInstValues.FirstOrDefault(x =>
                                x.DynamicAtt.Key.ToLower() == InstallationDynamicAtt.Key.ToLower());

                            if (DynamicAttInstValue != null)
                            {
                                dynamic DynamicAttValue = new ExpandoObject();
                                if (DynamicAttInstValue.ValueString != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueString;

                                else if (DynamicAttInstValue.ValueDouble != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueDouble;

                                else if (DynamicAttInstValue.ValueDateTime != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueDateTime;

                                else if (DynamicAttInstValue.ValueBoolean != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueBoolean;

                                ((IDictionary<String, Object>)DynamiOtherLoadInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamiOtherLoadInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(LoadOtherViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == TablesNames.TLIloadOther.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(OthersInstallationObject, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    if (DateTimeDynamicInstallationAttributesViewModel != null ? DateTimeDynamicInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<TLIdynamicAtt> DateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                           !x.disable && x.tablesNames.TableName == TablesNames.TLIloadOther.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == OthersInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == TablesNames.TLIloadOther.ToString()
                               , x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                        foreach (TLIdynamicAtt InstallationDynamicAtt in DateTimeInstallationDynamicAttributes)
                        {
                            TLIdynamicAttInstValue DynamicAttInstallationValue = DateTimeDynamicAttInstValues.FirstOrDefault(x =>
                                x.DynamicAtt.Key.ToLower() == InstallationDynamicAtt.Key.ToLower());

                            if (DynamicAttInstallationValue != null)
                            {
                                dynamic DynamicAttValue = new ExpandoObject();
                                if (DynamicAttInstallationValue.ValueDateTime != null)
                                    DynamicAttValue = DynamicAttInstallationValue.ValueDateTime;

                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    ((IDictionary<String, Object>)DynamiOtherLoadInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamiOtherLoadInstallation);
                }
                LoadOtheresTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    LoadOtheresTableDisplay.filters = _unitOfWork.LoadOtherRepository.GetRelatedTables();
                }
                else
                {
                    LoadOtheresTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, LoadOtheresTableDisplay, null, null, (int)ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        #region Helper Methods..
        public DynamicAttDto GetDynamicAttDto(TLIdynamicAttInstValue DynamicAttInstValueRecord, TLIdynamicAttLibValue DynamicAttLibRecord)
        {
            DynamicAttDto DynamicAttDto = new DynamicAttDto
            {
                DataType = new DataTypeViewModel(),
                DynamicAttInstValue = new DynamicAttInstValueViewModel(),
                DynamicAttLibValue = new DynamicAttLibValueViewMdodel()
            };
            if (DynamicAttInstValueRecord != null)
            {
                // Key
                DynamicAttDto.Key = DynamicAttInstValueRecord.DynamicAtt.Key;

                // DataType.Id + DataType.Name
                DynamicAttDto.DataType.Id = DynamicAttInstValueRecord.DynamicAtt.DataTypeId.Value;
                DynamicAttDto.DataType.Name = DynamicAttInstValueRecord.DynamicAtt.DataType.Name;

                // DynamicAttInstValue.Id
                DynamicAttDto.DynamicAttInstValue.Id = DynamicAttInstValueRecord.Id;

                // DynamicAttInstValueRecord.ValueString
                DynamicAttDto.DynamicAttInstValue.Value = GetDynamicAttValue(DynamicAttInstValueRecord, null);

                // DynamicAttInstValue.DynamicAttId
                DynamicAttDto.DynamicAttInstValue.DynamicAttId = DynamicAttInstValueRecord.DynamicAttId;
                DynamicAttDto.DynamicAttLibValue = null;

            }
            else if (DynamicAttLibRecord != null)
            {
                // Key
                DynamicAttDto.Key = DynamicAttLibRecord.DynamicAtt.Key;

                // DataType.Id + DataType.Name
                DynamicAttDto.DataType.Id = DynamicAttLibRecord.DynamicAtt.DataTypeId.Value;
                DynamicAttDto.DataType.Name = DynamicAttLibRecord.DynamicAtt.DataType.Name;

                // DynamicAttLibValue.Id
                DynamicAttDto.DynamicAttLibValue.Id = DynamicAttLibRecord.Id;

                // DynamicAttLibValue.Value
                DynamicAttDto.DynamicAttLibValue.Value = GetDynamicAttValue(null, DynamicAttLibRecord);

                // DynamicAttLibValue.DynamicAttId
                DynamicAttDto.DynamicAttLibValue.DynamicAttId = DynamicAttLibRecord.DynamicAttId;
                DynamicAttDto.DynamicAttInstValue = null;
            }
            return DynamicAttDto;
        }
        public string GetDynamicAttValue(TLIdynamicAttInstValue DynamicAttInstValueRecord, TLIdynamicAttLibValue DynamicAttLibRecord)
        {
            if (DynamicAttInstValueRecord != null)
            {
                if (DynamicAttInstValueRecord.ValueString != null) return DynamicAttInstValueRecord.ValueString;

                else if (DynamicAttInstValueRecord.ValueDouble != null) return DynamicAttInstValueRecord.ValueDouble.ToString();

                else if (DynamicAttInstValueRecord.ValueDateTime != null) return DynamicAttInstValueRecord.ValueDateTime.ToString();

                else if (DynamicAttInstValueRecord.ValueBoolean != null) return DynamicAttInstValueRecord.ValueBoolean.ToString();
            }
            else if (DynamicAttLibRecord != null)
            {
                if (DynamicAttLibRecord.ValueString != null)
                    return DynamicAttLibRecord.ValueString;

                else if (DynamicAttLibRecord.ValueDouble != null)
                    return DynamicAttLibRecord.ValueDouble.ToString();

                else if (DynamicAttLibRecord.ValueDateTime != null)
                    return DynamicAttLibRecord.ValueDateTime.ToString();

                else if (DynamicAttLibRecord.ValueBoolean != null)
                    return DynamicAttLibRecord.ValueBoolean.ToString();
            }
            return "";
        }
        #endregion
    }
}
