using AutoMapper;
using System;
using System.Collections.Generic;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_Repository.Base;
using TLIS_Service.Helpers;
using TLIS_Service.IService;
using System.Collections;
using System.Globalization;
using System.Linq;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.OperationDTOs;
using TLIS_DAL.ViewModels.LogicalOperationDTOs;
using TLIS_DAL.ViewModels.DependencyDTOs;
using System.Linq.Expressions;
using System.Transactions;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.RowRuleDTOs;
using TLIS_DAL.ViewModels.RuleDTOs;
using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using System.Reflection;
using TLIS_DAL.ViewModels.DynamicListValuesDTOs;
using TLIS_DAL.ViewModels.DependencyRowDTOs;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using System.Dynamic;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.ValidationDTOs;
using static TLIS_Service.Helpers.Constants;
using TLIS_DAL;
using Nancy;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using System.Runtime.CompilerServices;
using AutoMapper;

namespace TLIS_Service.Services
{
    public class DynamicAttService : IDynamicAttService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private readonly ApplicationDbContext _dbContext;
        private IMapper _mapper;
        public DynamicAttService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext dbContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public Response<DynamicAttributeValidations> GetDynamicAttributeValidation(int DynamicAttId)
        {
            try
            {
                DynamicAttributeValidations OutPut = new DynamicAttributeValidations();

                List<TLIdependency> Dependencies = _unitOfWork.DependencieRepository.GetIncludeWhere(x => x.DynamicAttId == DynamicAttId, x => x.Operation).ToList();
                foreach (TLIdependency Dependency in Dependencies)
                {
                    OutPut.Dependency.Id = Dependency.Id;
                    OutPut.Dependency.OperationId = Dependency.OperationId.Value;
                    OutPut.Dependency.OperationName = Dependency.Operation.Name;
                    OutPut.Dependency.Value = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                        Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                        Dependency.ValueDouble != null ? Dependency.ValueDouble :
                        !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                    List<TLIdependencyRow> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => Dependency.Id == x.DependencyId).ToList();

                    List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => DependencyRows.Exists(y => y.RowId == x.RowId),
                        x => x.Rule, x => x.Rule.Operation, x => x.Rule.tablesNames).Select(x => x.Rule).ToList();

                    foreach (TLIrule Rule in Rules)
                    {
                        DynamicAttributeDependencyRule DependencyRule = new DynamicAttributeDependencyRule();
                        DependencyRule.AttributeName = Rule.attributeActivatedId != null ?
                            _unitOfWork.AttributeActivatedRepository.GetByID(Rule.attributeActivatedId.Value).Label :
                            _unitOfWork.DynamicAttRepository.GetByID(Rule.dynamicAttId.Value).Key;

                        DependencyRule.Id = Rule.Id;
                        DependencyRule.RuleOperation = Rule.Operation.Name;
                        DependencyRule.RuleValue = Rule.OperationValueBoolean != null ? Rule.OperationValueBoolean :
                            Rule.OperationValueDateTime != null ? Rule.OperationValueDateTime :
                            Rule.OperationValueDouble != null ? Rule.OperationValueDouble :
                            !string.IsNullOrEmpty(Rule.OperationValueString) ? Rule.OperationValueString : null;

                        DependencyRule.TableName = Rule.tablesNames.TableName;
                    }
                }

                TLIvalidation GeneralValidation = _unitOfWork.ValidationRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttId, x => x.Operation);

                if (GeneralValidation != null)
                {
                    OutPut.GeneralValidation = new GeneralValidation
                    {
                        Id = GeneralValidation.Id,
                        OperationId = GeneralValidation.OperationId,
                        OperationName = GeneralValidation.Operation.Name,
                        Value = GeneralValidation.ValueBoolean != null ? GeneralValidation.ValueBoolean :
                        GeneralValidation.ValueDateTime != null ? GeneralValidation.ValueDateTime :
                        GeneralValidation.ValueDouble != null ? GeneralValidation.ValueDouble :
                        !string.IsNullOrEmpty(GeneralValidation.ValueString) ? GeneralValidation.ValueString : null
                    };
                }
                else
                    OutPut.GeneralValidation = null;
                return new Response<DynamicAttributeValidations>(true, OutPut, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<DynamicAttributeValidations>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        //Function to add dynamic library attribute value
        public Response<AddDynamicLibAttValueViewModel> AddDynamicAttLibValue(AddDynamicLibAttValueViewModel addDynamicLibAttValueViewModel)
        {
            Response<AddDynamicLibAttValueViewModel> result;
            try
            {
                TLIdynamicAtt dynamicAttEntites = _mapper.Map<TLIdynamicAtt>(addDynamicLibAttValueViewModel);
                //TLIdynamicAttLibValue dynamicAttLibValueEntites = _mapper.Map<TLIdynamicAttLibValue>(addDynamicLibAttValueViewModel);


                _unitOfWork.DynamicAttRepository.Add(dynamicAttEntites);
                //dynamicAttLibValueEntites.DynamicAttId = dynamicAttEntites.Id;
                //_unitOfWork.DynamicAttLibRepository.Add(dynamicAttLibValueEntites);
                _unitOfWork.SaveChanges();
                result = new Response<AddDynamicLibAttValueViewModel>();
            }
            catch (Exception err)
            {

                result = new Response<AddDynamicLibAttValueViewModel>(true, null, null, err.Message, Int32.Parse(Constants.ApiReturnCode.fail.ToString()));
            }

            return result;

        }
        //Function to add dynamic intallation attributed value
        public Response<AddDependencyInstViewModel> AddDynamicAttInst(AddDependencyInstViewModel addDependencyInstViewModel, string ConnectionString)
        {
            using (var con = new OracleConnection(ConnectionString))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required,
                        new System.TimeSpan(0, 15, 0)))
                    {
                        try
                        {
                            List<int> ResultDataType = _unitOfWork.OperationRepository.GetWhereAndSelect(x =>
                                x.Name.ToLower() == "result" || x.Name == "==", x => new { x.Id }).Select(x => x.Id).ToList();

                            TLIdynamicAtt DynamicAttEntity = _mapper.Map<TLIdynamicAtt>(addDependencyInstViewModel);

                            string DataTypeName = _unitOfWork.DataTypeRepository.GetByID(addDependencyInstViewModel.DataTypeId.Value).Name;

                            if (DataTypeName.ToLower() == "string".ToLower())
                            {
                                DynamicAttEntity.DefaultValue = !string.IsNullOrEmpty(addDependencyInstViewModel.StringDefaultValue) ?
                                    addDependencyInstViewModel.StringDefaultValue : "";
                            }

                            else if (DataTypeName.ToLower() == "int".ToLower())
                            {
                                DynamicAttEntity.DefaultValue = addDependencyInstViewModel.DoubleDefaultValue != null ?
                                    addDependencyInstViewModel.DoubleDefaultValue.ToString() : "0";
                            }

                            else if (DataTypeName.ToLower() == "double".ToLower())
                            {
                                DynamicAttEntity.DefaultValue = addDependencyInstViewModel.DoubleDefaultValue != null ?
                                    addDependencyInstViewModel.DoubleDefaultValue.ToString() : "0";
                            }

                            else if (DataTypeName.ToLower() == "boolean".ToLower())
                            {
                                DynamicAttEntity.DefaultValue = addDependencyInstViewModel.BooleanDefaultValue != null ?
                                    addDependencyInstViewModel.BooleanDefaultValue.ToString() : "false";
                            }

                            else if (DataTypeName.ToLower() == "datetime".ToLower())
                            {
                                DynamicAttEntity.DefaultValue = addDependencyInstViewModel.DateTimeDefaultValue != null ?
                                    addDependencyInstViewModel.DateTimeDefaultValue.ToString() : DateTime.Now.ToString();
                            }

                            DynamicAttEntity.tablesNames = _unitOfWork.TablesNamesRepository.GetByID(DynamicAttEntity.tablesNamesId);

                            // Validation For Dynamic Attribute Key (Dynamic Attribute Key Can't Be Reapeated For The Same TableName)..
                            TLIdynamicAtt CheckNameInTLIDynamic = _unitOfWork.DynamicAttRepository.GetIncludeWhereFirst(x =>
                                x.Key.ToLower() == DynamicAttEntity.Key.ToLower() && x.CivilWithoutLegCategoryId == DynamicAttEntity.CivilWithoutLegCategoryId &&
                                x.tablesNames.TableName.ToLower() == DynamicAttEntity.tablesNames.TableName.ToLower(),
                                    x => x.tablesNames);

                            if (CheckNameInTLIDynamic != null)
                                return new Response<AddDependencyInstViewModel>(true, null, null, $"This Key {DynamicAttEntity.Key} is Already Exist in Table {DynamicAttEntity.tablesNames.TableName} as a Dynamic Attribute", (int)Constants.ApiReturnCode.fail);

                            // Validation For Dynamic Attribute Key (Can't Add New Dynamic Attribute Key If It is Already Exist in Atttribute Activated Table (TLIattributeActivated))..
                            TLIattributeActivated CheckNameInTLIAttribute = _unitOfWork.AttributeActivatedRepository.GetWhereFirst(x =>
                                x.Key.ToLower() == DynamicAttEntity.Key.ToLower() &&
                                x.Tabel.ToLower() == DynamicAttEntity.tablesNames.TableName.ToLower());

                            if (CheckNameInTLIAttribute != null)
                                return new Response<AddDependencyInstViewModel>(true, null, null, $"This Key {DynamicAttEntity.Key} is Already Exist in Table {DynamicAttEntity.tablesNames.TableName} as a Static Attribute", (int)Constants.ApiReturnCode.fail);

                            DynamicAttEntity.LibraryAtt = false;
                            DynamicAttEntity.CivilWithoutLegCategoryId = addDependencyInstViewModel.CivilWithoutLegCategoryId;

                            TLIdataType DataType = _unitOfWork.DataTypeRepository.GetWhereFirst(x =>
                                x.Id == addDependencyInstViewModel.DataTypeId);

                            int DynamicAttId;
                            Dictionary<string, int> ListValuesIds = new Dictionary<string, int>();

                            _unitOfWork.DynamicAttRepository.Add(DynamicAttEntity);
                            UnitOfWork.AllDynamicAttribute.Add(DynamicAttEntity);
                            _unitOfWork.SaveChanges();

                            DynamicAttId = DynamicAttEntity.Id;

                            if (addDependencyInstViewModel.validations != null ? addDependencyInstViewModel.validations.Count > 0 : false)
                            {
                                foreach (var GeneralValidation in addDependencyInstViewModel.validations)
                                {
                                    if (GeneralValidation.OperationId > 0 && !string.IsNullOrEmpty(GeneralValidation.OperationValue))
                                    {
                                        TLIvalidation validation = new TLIvalidation();
                                        validation.DynamicAttId = DynamicAttId;
                                        validation.OperationId = GeneralValidation.OperationId;
                                        if (DataType.Name.ToLower() == "string")
                                        {
                                            validation.ValueString = GeneralValidation.OperationValue;
                                        }
                                        else if (DataType.Name.ToLower() == "int" || DataType.Name.ToLower() == "double" || DataType.Name.ToLower() == "float")
                                        {
                                            validation.ValueDouble = Convert.ToDouble(GeneralValidation.OperationValue);
                                        }
                                        else if (DataType.Name.ToLower() == "boolean")
                                        {
                                            validation.ValueBoolean = Convert.ToBoolean(GeneralValidation.OperationValue);
                                        }
                                        else if (DataType.Name.ToLower() == "datetime")
                                        {
                                            validation.ValueDateTime = Convert.ToDateTime(GeneralValidation.OperationValue);
                                        }
                                        //validation.OperationValue = addDependencyInstViewModel.validation.OperationValue;
                                        _unitOfWork.ValidationRepository.Add(validation);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }

                            string TableName = string.Empty;
                            if (addDependencyInstViewModel.Dependencies != null ? addDependencyInstViewModel.Dependencies.Count > 0 : false)
                            {
                                TableName = addDependencyInstViewModel.TableName;
                                foreach (var Dependencie in addDependencyInstViewModel.Dependencies)
                                {
                                    TLIdependency dependency = new TLIdependency();
                                    dependency.DynamicAttId = DynamicAttId;
                                    dependency.OperationId = Dependencie.OperationId;
                                    if (DataType.Name.ToLower() == "string")
                                    {
                                        dependency.ValueString = Dependencie.ValueString;
                                    }
                                    else if (DataType.Name.ToLower() == "int" || DataType.Name.ToLower() == "double" || DataType.Name.ToLower() == "float")
                                    {
                                        dependency.ValueDouble = Convert.ToDouble(Dependencie.ValueDouble);
                                    }
                                    else if (DataType.Name.ToLower() == "boolean")
                                    {
                                        dependency.ValueBoolean = Convert.ToBoolean(Dependencie.ValueBoolean);
                                    }
                                    else if (DataType.Name.ToLower() == "datetime")
                                    {
                                        dependency.ValueDateTime = Convert.ToDateTime(Dependencie.ValueDateTime);
                                    }
                                    _unitOfWork.DependencieRepository.Add(dependency);
                                    _unitOfWork.SaveChanges();

                                    foreach (var DependencyRow in Dependencie.DependencyRows)
                                    {
                                        TLIrow row = new TLIrow();
                                        _unitOfWork.RowRepository.Add(row);
                                        _unitOfWork.SaveChanges();

                                        foreach (var RowRule in DependencyRow.RowRules)
                                        {
                                            var TableNameEntity = _unitOfWork.TablesNamesRepository
                                                .GetWhereFirst(x => x.TableName.ToLower() == RowRule.Rule.TableName.ToLower());
                                            TLIrule Rule = _mapper.Map<TLIrule>(RowRule.Rule);
                                            Rule.tablesNamesId = TableNameEntity.Id;
                                            //TableName = _unitOfWork.AttributeActivatedRepository.GetAllAsQueryable().Where(x => x.Id == Rule.attributeActivatedId).FirstOrDefault().Tabel;
                                            _unitOfWork.RuleRepository.Add(Rule);
                                            _unitOfWork.SaveChanges();
                                            TLIrowRule RowRuleEntity = new TLIrowRule();
                                            RowRuleEntity.RuleId = Rule.Id;
                                            RowRuleEntity.RowId = row.Id;
                                            RowRuleEntity.LogicalOperationId = RowRule.LogicalOperationId;
                                            _unitOfWork.RowRuleRepository.Add(RowRuleEntity);
                                            _unitOfWork.SaveChanges();
                                        }
                                        TLIdependencyRow DependencyRowEntity = new TLIdependencyRow();
                                        DependencyRowEntity.DependencyId = dependency.Id;
                                        DependencyRowEntity.RowId = row.Id;
                                        DependencyRowEntity.LogicalOperationId = DependencyRow.LogicalOperationId;
                                        _unitOfWork.DependencyRowRepository.Add(DependencyRowEntity);
                                        _unitOfWork.SaveChanges();

                                        // int Count = ResultDataType.Count();
                                        //for (int i = 0; i < Count; i++)
                                        //{
                                        //    if (Dependencie.OperationId == ResultDataType[i])
                                        //        FilterInstData(addDependencyInstViewModel, addDependencyInstViewModel.TableName, DynamicAttId, null, con);
                                        //}
                                    }
                                }
                            }

                            bool CheckIfDynamicInCivilWithoutLeg = _unitOfWork.TablesNamesRepository
                                .GetWhereFirst(x => x.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString().ToLower()).Id == addDependencyInstViewModel.tablesNamesId ?
                                    true : false;

                            if (CheckIfDynamicInCivilWithoutLeg)
                            {
                                TLIattributeViewManagment AttributeForAdd = new TLIattributeViewManagment
                                {
                                    DynamicAttId = DynamicAttId,
                                    Enable = true,
                                    EditableManagmentViewId = _unitOfWork.EditableManagmentViewRepository.GetWhereFirst(x =>
                                        x.TLItablesNames1Id == addDependencyInstViewModel.tablesNamesId &&
                                        (x.CivilWithoutLegCategoryId != null ?
                                            x.CivilWithoutLegCategoryId == addDependencyInstViewModel.CivilWithoutLegCategoryId : false)).Id
                                };

                                _unitOfWork.AttributeViewManagmentRepository.Add(AttributeForAdd);
                                UnitOfWork.AllAttributeViewManagment.Add(AttributeForAdd);
                                _unitOfWork.SaveChanges();
                            }
                            else
                            {
                                TLIattributeViewManagment AttributeForAdd = new TLIattributeViewManagment
                                {
                                    DynamicAttId = DynamicAttId,
                                    Enable = true,
                                    EditableManagmentViewId = _unitOfWork.EditableManagmentViewRepository.GetWhereFirst(x =>
                                        x.TLItablesNames1Id == addDependencyInstViewModel.tablesNamesId).Id
                                };

                                _unitOfWork.AttributeViewManagmentRepository.Add(AttributeForAdd);
                                UnitOfWork.AllAttributeViewManagment.Add(AttributeForAdd);
                                _unitOfWork.SaveChanges();
                            }

                            AddInstallationListValues(addDependencyInstViewModel, DynamicAttId);

                            //for ADO.NET
                            tran.Commit();
                            _unitOfWork.SaveChanges();
                            //FOR ENTITIES
                            transaction.Complete();
                            return new Response<AddDependencyInstViewModel>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<AddDependencyInstViewModel>(true, null, null, err.Message, Int32.Parse(Constants.ApiReturnCode.fail.ToString()));
                        }
                    }
                }
            }
        }
        //Function to get dependency for library table by table name
        public Response<DependencyColumnForAdd> GetDependencyLib(string TableName, int? CategoryId)
        {
            try
            {
                if (CategoryId == null)
                {
                    List<DependencyColumn> Data = _unitOfWork.AttributeActivatedRepository
                        .GetWhere(x => x.Tabel.ToLower() == TableName.ToLower() && x.Key.ToLower() != "id" && x.Key.ToLower() != "active" && x.Key.ToLower() != "deleted")
                        .Select(x => new DependencyColumn(x.Label, x.DataType, false, null, x.Id)).ToList();

                    int TableNameId = _unitOfWork.TablesNamesRepository
                        .GetWhereFirst(x => x.TableName.ToLower() == TableName.ToLower()).Id;

                    List<AttributeActivatedViewModel> AttributeActivated = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeActivatedRepository
                        .GetWhere(x => x.Tabel.ToLower() == TableName.ToLower()).ToList());

                    AttributeActivatedViewModel Record = new AttributeActivatedViewModel();

                    if (TableName.ToLower().Contains("civil"))
                    {
                        if (CivilType.TLIcivilWithLegLibrary.ToString().ToLower() == TableName.ToLower())
                        {
                            List<KeyValuePair<string, List<DropDownListFilters>>> CivilWithLegLibraryRelatedTables = _unitOfWork.CivilWithLegLibraryRepository.GetRelatedTables();

                            foreach (KeyValuePair<string, List<DropDownListFilters>> CivilWithLegLibraryRelatedTable in CivilWithLegLibraryRelatedTables)
                            {
                                Record = AttributeActivated
                                    .FirstOrDefault(x => x.Key == CivilWithLegLibraryRelatedTable.Key);

                                if (Record != null)
                                    Data.Add(new DependencyColumn(Record.Label, Record.DataType, false, CivilWithLegLibraryRelatedTable.Value, Record.Id));
                            }
                        }
                        else if (CivilType.TLIcivilNonSteelLibrary.ToString().ToLower() == TableName.ToLower())
                        {
                            List<KeyValuePair<string, List<DropDownListFilters>>> civilNonSteelLibraryRelatedTables = _unitOfWork.CivilNonSteelLibraryRepository.GetRelatedTables();

                            foreach (KeyValuePair<string, List<DropDownListFilters>> civilNonSteelLibraryRelatedTable in civilNonSteelLibraryRelatedTables)
                            {
                                Record = AttributeActivated
                                    .FirstOrDefault(x => x.Key == civilNonSteelLibraryRelatedTable.Key);

                                if (Record != null)
                                    Data.Add(new DependencyColumn(Record.Label, Record.DataType, false, civilNonSteelLibraryRelatedTable.Value, Record.Id));
                            }
                        }
                    }
                    else if (TableName.ToLower().Contains("tlimw"))
                    {
                        if (LoadSubType.TLImwBULibrary.ToString().ToLower() == TableName.ToLower())
                        {
                            List<KeyValuePair<string, List<DropDownListFilters>>> MWBURelatedTables = _unitOfWork.MW_BULibraryRepository.GetRelatedTables();

                            foreach (KeyValuePair<string, List<DropDownListFilters>> MWBURelatedTable in MWBURelatedTables)
                            {
                                Record = AttributeActivated
                                    .FirstOrDefault(x => x.Key == MWBURelatedTable.Key);

                                if (Record != null)
                                    Data.Add(new DependencyColumn(Record.Label, Record.DataType, false, MWBURelatedTable.Value, Record.Id));
                            }
                        }
                        else if (LoadSubType.TLImwDishLibrary.ToString().ToLower() == TableName.ToLower())
                        {
                            List<KeyValuePair<string, List<DropDownListFilters>>> mwDishLibraryRelatedTables = _unitOfWork.MW_DishLibraryRepository.GetRelatedTables();

                            foreach (KeyValuePair<string, List<DropDownListFilters>> mwDishLibraryRelatedTable in mwDishLibraryRelatedTables)
                            {
                                Record = AttributeActivated
                                    .FirstOrDefault(x => x.Key == mwDishLibraryRelatedTable.Key);

                                if (Record != null)
                                    Data.Add(new DependencyColumn(Record.Label, Record.DataType, false, mwDishLibraryRelatedTable.Value, Record.Id));
                            }
                        }
                        else if (LoadSubType.TLImwODULibrary.ToString().ToLower() == TableName.ToLower())
                        {
                            List<KeyValuePair<string, List<DropDownListFilters>>> mwODULibraryRelatedTables = _unitOfWork.MW_ODULibraryRepository.GetRelatedTables();

                            foreach (KeyValuePair<string, List<DropDownListFilters>> mwODULibraryRelatedTable in mwODULibraryRelatedTables)
                            {
                                Record = AttributeActivated
                                    .FirstOrDefault(x => x.Key == mwODULibraryRelatedTable.Key);

                                if (Record != null)
                                    Data.Add(new DependencyColumn(Record.Label, Record.DataType, false, mwODULibraryRelatedTable.Value, Record.Id));
                            }
                        }
                        else if (LoadSubType.TLImwRFULibrary.ToString().ToLower() == TableName.ToLower())
                        {
                            List<KeyValuePair<string, List<DropDownListFilters>>> mwRFULibraryRelatedTables = _unitOfWork.MW_RFULibraryRepository.GetRelatedTables();

                            foreach (KeyValuePair<string, List<DropDownListFilters>> mwRFULibraryRelatedTable in mwRFULibraryRelatedTables)
                            {
                                Record = AttributeActivated
                                    .FirstOrDefault(x => x.Key == mwRFULibraryRelatedTable.Key);

                                if (Record != null)
                                    Data.Add(new DependencyColumn(Record.Label, Record.DataType, false, mwRFULibraryRelatedTable.Value, Record.Id));
                            }
                        }
                    }

                    else if (LoadSubType.TLIsideArmLibrary.ToString().ToLower() == TableName.ToLower())
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> sideArmLibraryRelatedTables = _unitOfWork.CivilWithLegLibraryRepository.GetRelatedTables();

                        foreach (KeyValuePair<string, List<DropDownListFilters>> sideArmLibraryRelatedTable in sideArmLibraryRelatedTables)
                        {
                            Record = AttributeActivated
                                .FirstOrDefault(x => x.Key == sideArmLibraryRelatedTable.Key);

                            if (Record != null)
                                Data.Add(new DependencyColumn(Record.Label, Record.DataType, false, sideArmLibraryRelatedTable.Value, Record.Id));
                        }
                    }

                    else if (OtherInventoryType.TLIcabinetPowerLibrary.ToString().ToLower() == TableName.ToLower())
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> cabinetPowerLibraryRelatedTables = _unitOfWork.CabinetPowerLibraryRepository.GetRelatedTables();

                        foreach (KeyValuePair<string, List<DropDownListFilters>> cabinetPowerLibraryRelatedTable in cabinetPowerLibraryRelatedTables)
                        {
                            Record = AttributeActivated
                                .FirstOrDefault(x => x.Key == cabinetPowerLibraryRelatedTable.Key);

                            if (Record != null)
                                Data.Add(new DependencyColumn(Record.Label, Record.DataType, false, cabinetPowerLibraryRelatedTable.Value, Record.Id));
                        }
                    }
                    else if (OtherInventoryType.TLIcabinetTelecomLibrary.ToString().ToLower() == TableName.ToLower())
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> cabinetTelecomLibraryRelatedTables = _unitOfWork.CabinetTelecomLibraryRepository.GetRelatedTables();

                        foreach (KeyValuePair<string, List<DropDownListFilters>> cabinetTelecomLibraryRelatedTable in cabinetTelecomLibraryRelatedTables)
                        {
                            Record = AttributeActivated
                                .FirstOrDefault(x => x.Key == cabinetTelecomLibraryRelatedTable.Key);

                            if (Record != null)
                                Data.Add(new DependencyColumn(Record.Label, Record.DataType, false, cabinetTelecomLibraryRelatedTable.Value, Record.Id));
                        }
                    }
                    else if (OtherInventoryType.TLIgeneratorLibrary.ToString().ToLower() == TableName.ToLower())
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> generatorLibraryRelatedTables = _unitOfWork.GeneratorLibraryRepository.GetRelatedTables();

                        foreach (KeyValuePair<string, List<DropDownListFilters>> generatorLibraryRelatedTable in generatorLibraryRelatedTables)
                        {
                            Record = AttributeActivated
                                .FirstOrDefault(x => x.Key == generatorLibraryRelatedTable.Key);

                            if (Record != null)
                                Data.Add(new DependencyColumn(Record.Label, Record.DataType, false, generatorLibraryRelatedTable.Value, Record.Id));
                        }
                    }
                    else if (OtherInventoryType.TLIsolarLibrary.ToString().ToLower() == TableName.ToLower())
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> solarLibraryRelatedTables = _unitOfWork.SolarLibraryRepository.GetRelatedTables();

                        foreach (KeyValuePair<string, List<DropDownListFilters>> solarLibraryRelatedTable in solarLibraryRelatedTables)
                        {
                            Record = AttributeActivated
                                .FirstOrDefault(x => x.Key == solarLibraryRelatedTable.Key);

                            if (Record != null)
                                Data.Add(new DependencyColumn(Record.Label, Record.DataType, false, solarLibraryRelatedTable.Value, Record.Id));
                        }
                    }

                    List<DependencyColumn> DynamicAtts = _unitOfWork.DynamicAttRepository.GetDynamicLibAtts(TableNameId, null)
                        .Select(x => new DependencyColumn(x.Key, x.DataType, true, null, x.Id)).ToList();

                    Data.AddRange(DynamicAtts);

                    DependencyColumnForAdd result = new DependencyColumnForAdd
                    {
                        dependencyColumns = Data,
                        Operations = _mapper.Map<List<OperationViewModel>>(_unitOfWork.OperationRepository.GetAllWithoutCount().ToList()),
                        LogicalOperations = _mapper.Map<List<LogicalOperationViewModel>>(_unitOfWork.LogicalOperationRepository.GetAllWithoutCount().ToList()),
                        DataTypes = _mapper.Map<List<DataTypeViewModel>>(_unitOfWork.DataTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList()),
                        TableNameId = TableNameId
                    };

                    return new Response<DependencyColumnForAdd>(result);
                }
                else
                {
                    List<DependencyColumn> Data = new List<DependencyColumn>();

                    int TableNameId = _unitOfWork.TablesNamesRepository
                        .GetWhereFirst(x => x.TableName.ToLower() == TableName.ToLower()).Id;

                    TLIattActivatedCategory Record = new TLIattActivatedCategory();

                    List<TLIattActivatedCategory> AttributeActivatedCategory = _unitOfWork.AttActivatedCategoryRepository
                        .GetIncludeWhere(x => x.civilWithoutLegCategoryId.Value == CategoryId.Value && x.IsLibrary && x.enable && x.attributeActivated.Key.ToLower() != "active" &&
                            x.attributeActivated.Key.ToLower() != "id" && x.attributeActivated.Key.ToLower() != "deleted" && x.attributeActivated.DataType.ToLower() != "list"
                                , x => x.attributeActivated).ToList();

                    Data = AttributeActivatedCategory.Select(x =>
                        new DependencyColumn(x.Label, x.attributeActivated.DataType, false, null, x.attributeActivatedId)).ToList();

                    List<KeyValuePair<string, List<DropDownListFilters>>> CivilWithoutLegLibraryRelatedTables = _unitOfWork.CivilWithoutLegLibraryRepository.GetRelatedTables();

                    foreach (KeyValuePair<string, List<DropDownListFilters>> CivilWithoutLegLibraryRelatedTable in CivilWithoutLegLibraryRelatedTables)
                    {
                        Record = AttributeActivatedCategory
                            .FirstOrDefault(x => x.attributeActivated.Key.ToLower() == CivilWithoutLegLibraryRelatedTable.Key.ToLower());

                        if (Record != null)
                            Data.Add(new DependencyColumn(Record.Label, Record.attributeActivated.DataType, false, CivilWithoutLegLibraryRelatedTable.Value, Record.attributeActivatedId));
                    }

                    List<DependencyColumn> DynamicAtts = _unitOfWork.DynamicAttRepository.GetDynamicLibAtts(TableNameId, CategoryId)
                        .Select(x => new DependencyColumn(x.Key, x.DataType, true, null, x.Id)).ToList();

                    Data.AddRange(DynamicAtts);

                    DependencyColumnForAdd result = new DependencyColumnForAdd
                    {
                        dependencyColumns = Data,
                        Operations = _mapper.Map<List<OperationViewModel>>(_unitOfWork.OperationRepository.GetAllWithoutCount().ToList()),
                        LogicalOperations = _mapper.Map<List<LogicalOperationViewModel>>(_unitOfWork.LogicalOperationRepository.GetAllWithoutCount().ToList()),
                        DataTypes = _mapper.Map<List<DataTypeViewModel>>(_unitOfWork.DataTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList()),
                        TableNameId = TableNameId
                    };

                    return new Response<DependencyColumnForAdd>(result);
                }
            }
            catch (Exception err)
            {
                return new Response<DependencyColumnForAdd>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        //Function to get activated attributes
        public Response<IEnumerable<AttributeActivatedViewModel>> GetAttributes(string TableName)
        {
            try
            {
                var AttributeActivated = _unitOfWork.AttributeActivatedRepository.GetWhere(x => x.Tabel == TableName).ToList();
                var AttributeActivatedModel = _mapper.Map<List<AttributeActivatedViewModel>>(AttributeActivated);
                return new Response<IEnumerable<AttributeActivatedViewModel>>(true, AttributeActivatedModel, null, null, (int)Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<AttributeActivatedViewModel>>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        //Function to add dynamic attribute

        /************************************************************************************/
        /*     Delete From AddDependencyViewModel The Attribute (dynamicListValues)         */
        /************************************************************************************/
        #region Helper Methods
        public void AddLibraryListValues(AddDependencyViewModel addDependencyViewModel, int DynamicAttId)
        {
            try
            {
                if (addDependencyViewModel.Dependencies != null ? addDependencyViewModel.Dependencies.Count() == 0 : true)
                {
                    AddDefaultValues(addDependencyViewModel, null, DynamicAttId);
                }
                else
                {
                    if (addDependencyViewModel.BooleanResult != null || addDependencyViewModel.DoubleResult != null ||
                        addDependencyViewModel.DateTimeResult != null || !string.IsNullOrEmpty(addDependencyViewModel.StringResult))
                    {
                        if (addDependencyViewModel.LibraryAtt)
                        {
                            // Civils..
                            if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIcivilWithLegLibrary.ToString().ToLower())
                            {
                                List<CivilWithLegLibraryViewModel> CivilWithLegLibraries = _mapper.Map<List<CivilWithLegLibraryViewModel>>(CivilLibraryService._CivilWithLegLibraryEntities).ToList();

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (CivilWithLegLibraryViewModel CivilWithLegLibrary in CivilWithLegLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(CivilWithLegLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(CivilWithLegLibrary, null);

                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == CivilWithLegLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == CivilWithLegLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = CivilWithLegLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == CivilWithLegLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = CivilWithLegLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }
                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIcivilWithoutLegLibrary.ToString().ToLower())
                            {
                                List<CivilWithoutLegLibraryViewModel> CivilWithoutLegLibraries = _mapper.Map<List<CivilWithoutLegLibraryViewModel>>(_unitOfWork.CivilWithoutLegLibraryRepository.GetIncludeWhere(x =>
                                    x.Id > 0 && !x.Deleted, x => x.CivilSteelSupportCategory, x => x.CivilWithoutLegCategory, x => x.InstallationCivilwithoutLegsType,
                                    x => x.structureType).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (CivilWithoutLegLibraryViewModel CivilWithoutLegLibrary in CivilWithoutLegLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(CivilWithoutLegLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(CivilWithoutLegLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == CivilWithoutLegLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == CivilWithoutLegLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = CivilWithoutLegLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == CivilWithoutLegLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = CivilWithoutLegLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIcivilNonSteelLibrary.ToString().ToLower())
                            {
                                List<CivilNonSteelLibraryViewModel> CivilNonSteelLibraries = _mapper.Map<List<CivilNonSteelLibraryViewModel>>(_unitOfWork.CivilNonSteelLibraryRepository.GetIncludeWhere(x =>
                                    x.Id > 0 && !x.Deleted, x => x.civilNonSteelType).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (CivilNonSteelLibraryViewModel CivilNonSteelLibrary in CivilNonSteelLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(CivilNonSteelLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(CivilNonSteelLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == CivilNonSteelLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == CivilNonSteelLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = CivilNonSteelLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == CivilNonSteelLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = CivilNonSteelLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }


                            // SideArm..
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIsideArmLibrary.ToString().ToLower())
                            {
                                List<SideArmLibraryViewModel> SideArmLibraries = _mapper.Map<List<SideArmLibraryViewModel>>(_unitOfWork.SideArmLibraryRepository.GetWhere(x =>
                                    x.Id > 0 && !x.Deleted).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (SideArmLibraryViewModel SideArmLibrary in SideArmLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(SideArmLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(SideArmLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == SideArmLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == SideArmLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = SideArmLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == SideArmLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = SideArmLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }

                            // Other Inventories..
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIcabinetPowerLibrary.ToString().ToLower())
                            {
                                List<CabinetPowerLibraryViewModel> CabinetPowerLibraries = _mapper.Map<List<CabinetPowerLibraryViewModel>>(_unitOfWork.CabinetPowerLibraryRepository.GetIncludeWhere(x =>
                                    x.Id > 0 && !x.Deleted, x => x.CabinetPowerType).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (CabinetPowerLibraryViewModel CabinetPowerLibrary in CabinetPowerLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(CabinetPowerLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(CabinetPowerLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == CabinetPowerLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == CabinetPowerLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = CabinetPowerLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == CabinetPowerLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = CabinetPowerLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIcabinetTelecomLibrary.ToString().ToLower())
                            {
                                List<CabinetTelecomLibraryViewModel> CabinetTelecomLibraries = _mapper.Map<List<CabinetTelecomLibraryViewModel>>(_unitOfWork.CabinetTelecomLibraryRepository.GetIncludeWhere(x =>
                                    x.Id > 0 && !x.Deleted, x => x.TelecomType).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (CabinetTelecomLibraryViewModel CabinetTelecomLibrary in CabinetTelecomLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(CabinetTelecomLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(CabinetTelecomLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == CabinetTelecomLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == CabinetTelecomLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = CabinetTelecomLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == CabinetTelecomLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = CabinetTelecomLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIsolarLibrary.ToString().ToLower())
                            {
                                List<SolarLibraryViewModel> SolarLibraries = _mapper.Map<List<SolarLibraryViewModel>>(_unitOfWork.SolarLibraryRepository.GetIncludeWhere(x =>
                                    x.Id > 0 && !x.Deleted, x => x.Capacity).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (SolarLibraryViewModel SolarLibrary in SolarLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(SolarLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(SolarLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == SolarLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == SolarLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = SolarLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == SolarLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = SolarLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIgeneratorLibrary.ToString().ToLower())
                            {
                                List<GeneratorLibraryViewModel> GeneratorLibraries = _mapper.Map<List<GeneratorLibraryViewModel>>(_unitOfWork.GeneratorLibraryRepository.GetIncludeWhere(x =>
                                    x.Id > 0 && !x.Deleted, x => x.Capacity).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (GeneratorLibraryViewModel GeneratorLibrary in GeneratorLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(GeneratorLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(GeneratorLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == GeneratorLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == GeneratorLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = GeneratorLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == GeneratorLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = GeneratorLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }

                            // Loads..
                            // Microwaves..
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLImwDishLibrary.ToString().ToLower())
                            {
                                List<MW_DishLibraryViewModel> MWDishLibraries = _mapper.Map<List<MW_DishLibraryViewModel>>(_unitOfWork.MW_DishLibraryRepository.GetIncludeWhere(x =>
                                    x.Id > 0 && !x.Deleted, x => x.asType, x => x.polarityType).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (MW_DishLibraryViewModel MWDishLibrary in MWDishLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(MW_DishLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(MWDishLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == MWDishLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == MWDishLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = MWDishLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == MWDishLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = MWDishLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLImwODULibrary.ToString().ToLower())
                            {
                                List<MW_ODULibraryViewModel> MWODULibraries = _mapper.Map<List<MW_ODULibraryViewModel>>(_unitOfWork.MW_ODULibraryRepository.GetIncludeWhere(x =>
                                    x.Id > 0 && !x.Deleted, x => x.parity).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (MW_ODULibraryViewModel MWODULibrary in MWODULibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(MW_ODULibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(MWODULibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == MWODULibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == MWODULibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = MWODULibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == MWODULibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = MWODULibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLImwBULibrary.ToString().ToLower())
                            {
                                List<MW_BULibraryViewModel> MWBULibraries = _mapper.Map<List<MW_BULibraryViewModel>>(_unitOfWork.MW_BULibraryRepository.GetIncludeWhere(x =>
                                    x.Id > 0 && !x.Deleted, x => x.diversityType).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (MW_BULibraryViewModel MWBULibrary in MWBULibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(MW_BULibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(MWBULibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == MWBULibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == MWBULibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = MWBULibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == MWBULibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = MWBULibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLImwRFULibrary.ToString().ToLower())
                            {
                                List<MW_RFULibraryViewModel> MWRFULibraries = _mapper.Map<List<MW_RFULibraryViewModel>>(_unitOfWork.MW_RFULibraryRepository.GetIncludeWhere(x =>
                                    x.Id > 0 && !x.Deleted, x => x.boardType, x => x.diversityType).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (MW_RFULibraryViewModel MWRFULibrary in MWRFULibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(MW_RFULibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(MWRFULibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == MWRFULibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == MWRFULibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = MWRFULibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == MWRFULibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = MWRFULibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLImwOtherLibrary.ToString().ToLower())
                            {
                                List<MW_OtherLibraryViewModel> MWOtherLibraries = _mapper.Map<List<MW_OtherLibraryViewModel>>(_unitOfWork.MW_OtherLibraryRepository.GetWhere(x =>
                                    x.Id > 0 && !x.Deleted).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (MW_OtherLibraryViewModel MWOtherLibrary in MWOtherLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(MW_OtherLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(MWOtherLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == MWOtherLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == MWOtherLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = MWOtherLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == MWOtherLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = MWOtherLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }

                            // Radios..
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIradioAntennaLibrary.ToString().ToLower())
                            {
                                List<RadioAntennaLibraryViewModel> RadioAntennaLibraries = _mapper.Map<List<RadioAntennaLibraryViewModel>>(_unitOfWork.RadioAntennaLibraryRepository.GetWhere(x =>
                                    x.Id > 0 && !x.Deleted).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (RadioAntennaLibraryViewModel RadioAntennaLibrary in RadioAntennaLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(RadioAntennaLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(RadioAntennaLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == RadioAntennaLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == RadioAntennaLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = RadioAntennaLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == RadioAntennaLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = RadioAntennaLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIradioRRULibrary.ToString().ToLower())
                            {
                                List<RadioRRULibraryViewModel> RadioRRULibraries = _mapper.Map<List<RadioRRULibraryViewModel>>(_unitOfWork.RadioRRULibraryRepository.GetWhere(x =>
                                    x.Id > 0 && !x.Deleted).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (RadioRRULibraryViewModel RadioRRULibrary in RadioRRULibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(RadioRRULibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(RadioRRULibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == RadioRRULibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == RadioRRULibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = RadioRRULibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == RadioRRULibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = RadioRRULibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIradioOtherLibrary.ToString().ToLower())
                            {
                                List<RadioOtherLibraryViewModel> RadioOtherLibraries = _mapper.Map<List<RadioOtherLibraryViewModel>>(_unitOfWork.RadioOtherLibraryRepository.GetWhere(x =>
                                    x.Id > 0 && !x.Deleted).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (RadioOtherLibraryViewModel RadioOtherLibrary in RadioOtherLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(RadioOtherLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(RadioOtherLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == RadioOtherLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == RadioOtherLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = RadioOtherLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == RadioOtherLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = RadioOtherLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }

                            // Power..
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIpowerLibrary.ToString().ToLower())
                            {
                                List<PowerLibraryViewModel> PowerLibraries = _mapper.Map<List<PowerLibraryViewModel>>(_unitOfWork.PowerLibraryRepository.GetWhere(x =>
                                    x.Id > 0 && !x.Deleted).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (PowerLibraryViewModel PowerLibrary in PowerLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(PowerLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(PowerLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == PowerLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == PowerLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = PowerLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == PowerLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = PowerLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }

                            // Load Other..
                            else if (addDependencyViewModel.TableName.ToLower() == TablesNames.TLIloadOtherLibrary.ToString().ToLower())
                            {
                                List<LoadOtherLibraryViewModel> LoadOtherLibraries = _mapper.Map<List<LoadOtherLibraryViewModel>>(_unitOfWork.LoadOtherLibraryRepository.GetWhere(x =>
                                    x.Id > 0 && !x.Deleted).ToList());

                                foreach (DependencyViewModel Dependency in addDependencyViewModel.Dependencies)
                                {
                                    foreach (LoadOtherLibraryViewModel LoadOtherLibrary in LoadOtherLibraries)
                                    {
                                        List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                                        foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                                        {
                                            int Succeed = 0;

                                            foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                            {
                                                if (RowRule.Rule.attributeActivatedId != null)
                                                {
                                                    TLIattributeActivated RuleStaticAttribute = _unitOfWork.AttributeActivatedRepository.GetByID(RowRule.Rule.attributeActivatedId.Value);

                                                    PropertyInfo LibraryProp = typeof(LoadOtherLibraryViewModel).GetProperties().FirstOrDefault(x =>
                                                        x.Name.ToLower() == RuleStaticAttribute.Key.ToLower());

                                                    object PropObject = LibraryProp.GetValue(LoadOtherLibrary, null);
                                                    if (PropObject != null)
                                                    {
                                                        string OperationStatic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;
                                                        if (OperationStatic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString == PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != Convert.ToBoolean(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString != PropObject.ToString())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationStatic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= Convert.ToDateTime(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= Convert.ToDouble(PropObject))
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (RowRule.Rule.dynamicAttId != null)
                                                {
                                                    TLIdynamicAttLibValue RuleDynamicAttribute = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x =>
                                                        x.DynamicAttId == RowRule.Rule.dynamicAttId.Value && x.InventoryId == LoadOtherLibrary.Id);

                                                    if (RuleDynamicAttribute != null)
                                                    {
                                                        string OperationDynamic = _unitOfWork.OperationRepository.GetWhereFirst(x => !x.Deleted && x.Id == RowRule.Rule.OperationId.Value).Name;

                                                        if (OperationDynamic == "==")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean == RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime == RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble == RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() == RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "!=")
                                                        {
                                                            if (RowRule.Rule.OperationValueBoolean != null && RuleDynamicAttribute.ValueBoolean != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueBoolean != RuleDynamicAttribute.ValueBoolean)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime != RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble != RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (!string.IsNullOrEmpty(RowRule.Rule.OperationValueString) && !string.IsNullOrEmpty(RuleDynamicAttribute.ValueString))
                                                            {
                                                                if (RowRule.Rule.OperationValueString.ToLower() != RuleDynamicAttribute.ValueString.ToLower())
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime > RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble > RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == ">=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime >= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble >= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime < RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble < RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                        else if (OperationDynamic == "<=")
                                                        {
                                                            if (RowRule.Rule.OperationValueDateTime != null && RuleDynamicAttribute.ValueDateTime != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDateTime <= RuleDynamicAttribute.ValueDateTime)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                            else if (RowRule.Rule.OperationValueDouble != null && RuleDynamicAttribute.ValueDouble != null)
                                                            {
                                                                if (RowRule.Rule.OperationValueDouble <= RuleDynamicAttribute.ValueDouble)
                                                                {
                                                                    Succeed++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Succeed == DependencyRow.RowRules.Count())
                                            {
                                                TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                    !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                    x.InventoryId == LoadOtherLibrary.Id);

                                                if (Check == null)
                                                {
                                                    ListToAdd.Add(new TLIdynamicAttLibValue
                                                    {
                                                        disable = false,
                                                        DynamicAttId = DynamicAttId,
                                                        InventoryId = LoadOtherLibrary.Id,
                                                        tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                        ValueBoolean = addDependencyViewModel.BooleanResult,
                                                        ValueString = addDependencyViewModel.StringResult,
                                                        ValueDateTime = addDependencyViewModel.DateTimeResult,
                                                        ValueDouble = addDependencyViewModel.DoubleResult
                                                    });
                                                }
                                            }
                                        }
                                        if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                                            addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                                        {
                                            TLIdynamicAttLibValue Check = ListToAdd.FirstOrDefault(x =>
                                                !x.disable && x.DynamicAttId == DynamicAttId && x.tablesNamesId == addDependencyViewModel.tablesNamesId &&
                                                x.InventoryId == LoadOtherLibrary.Id);

                                            if (Check == null)
                                            {
                                                ListToAdd.Add(new TLIdynamicAttLibValue
                                                {
                                                    disable = false,
                                                    DynamicAttId = DynamicAttId,
                                                    InventoryId = LoadOtherLibrary.Id,
                                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                                    ValueString = addDependencyViewModel.StringDefaultValue,
                                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue
                                                });
                                            }
                                        }

                                        _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        AddDefaultValues(addDependencyViewModel, null, DynamicAttId);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void LoopForPath(List<string> Path, int StartIndex, ApplicationDbContext _dbContext, object Value, List<int> OutPutIds)
        {
            if (StartIndex == Path.Count())
            {
                OutPutIds.Add((int)Value);
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
                    object PrimaryKeyValue = Record.GetType().GetProperty(Path[StartIndex + 2]).GetValue(Record, null);

                    if (PrimaryKeyValue != null)
                    {
                        if (StartIndex + 3 < Path.Count())
                            LoopForPath(Path, StartIndex + 3, _dbContext, PrimaryKeyValue, OutPutIds);

                        else if (StartIndex + 3 == Path.Count())
                            OutPutIds.Add((int)PrimaryKeyValue);
                    }
                }
            }
        }
        public List<int> GetRecordsIds(string MainTableName, AddInstRuleViewModel Rule)
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

            List<int> OutPutIds = new List<int>();

            PathToAddDynamicAttValue Item = new PathToAddDynamicAttValue();

            if (MainTableName.ToLower() == TablesNames.TLIradioRRU.ToString().ToLower() || SDTableName.ToLower() == TablesNames.TLIradioRRU.ToString().ToLower())
                Item = (PathToAddDynamicAttValue)Enum.Parse(typeof(PathToAddDynamicAttValue),
                    (MainTableName + SDTableName).ToLower());

            else
                Item = (PathToAddDynamicAttValue)Enum.Parse(typeof(PathToAddDynamicAttValue),
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

                    TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                        .GetProperties().FirstOrDefault(x => x.Name.ToLower() == Path[1].ToLower()).GetValue(_dbContext, null))
                            .Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ?
                               (Operation == ">" ?
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
                                .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
                                    (x.ValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDateTime != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
                                    (x.ValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDouble != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
                                    (x.ValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false)).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
                                    (!string.IsNullOrEmpty(x.ValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToString().ToLower() : false)).ToList();
                        }
                        if (DynamicAttValues != null)
                        {
                            TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                .GetProperties().FirstOrDefault(x => x.Name.ToLower() == Path[1].ToLower()).GetValue(_dbContext, null))
                                    .Where(x => DynamicAttValues.Exists(y =>
                                        y.InventoryId.ToString() == x.GetType().GetProperty("Id").GetValue(x, null).ToString() ? (
                                            (y.ValueBoolean != null ? (
                                                Operation == "==" ? y.ValueBoolean.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueBoolean.ToString().ToLower() != Value.ToString().ToLower() : false) : false) ||
                                            (y.ValueDateTime != null ? (
                                                Operation == "==" ? y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 :
                                                Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 ||
                                                    y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 :
                                                Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 ||
                                                    y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "!=" ? y.ValueDateTime.ToString().ToLower() != Value.ToString().ToLower() : false) : false) ||
                                            (y.ValueDouble != null ? (
                                                Operation == "==" ? y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 :
                                                Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 ||
                                                    y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 :
                                                Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 ||
                                                    y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "!=" ? y.ValueDouble.ToString().ToLower() != Value.ToString().ToLower() : false) : false) ||
                                            (!string.IsNullOrEmpty(y.ValueString) ? (
                                                Operation == "==" ? y.ValueString.ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueString.ToLower() != Value.ToString().ToLower() : false) : false)
                                        ) : false)).ToList();

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
                                .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
                                    (x.ValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDateTime != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
                                    (x.ValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDouble != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
                                    (x.ValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false)).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => x.DynamicAttId == Rule.dynamicAttId && !x.disable &&
                                    (!string.IsNullOrEmpty(x.ValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)).ToList();
                        }
                        if (DynamicAttValues != null)
                        {
                            TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                .GetProperties().FirstOrDefault(x => x.Name.ToLower() == Path[1].ToLower()).GetValue(_dbContext, null))
                                    .Where(x => DynamicAttValues.FirstOrDefault(y =>
                                        y.InventoryId.ToString() == x.GetType().GetProperty("Id").GetValue(x, null).ToString() ? (
                                            (y.ValueBoolean != null ? (
                                                Operation == "==" ? y.ValueBoolean.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueBoolean.ToString().ToLower() != Value.ToString().ToLower() : false) : false) ||
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
                                            (!string.IsNullOrEmpty(y.ValueString) ?
                                                (Operation == "==" ? y.ValueString.ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueString.ToLower() != Value.ToString().ToLower() : false) : false)) : false) != null).ToList();

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
        public void AddInstallationListValues(AddDependencyInstViewModel addDependencyInstViewModel, int DynamicAttId)
        {
            try
            {
                if (addDependencyInstViewModel.Dependencies != null ? addDependencyInstViewModel.Dependencies.Count() == 0 : true)
                {
                    AddDefaultValues(null, addDependencyInstViewModel, DynamicAttId);
                }
                else
                {
                    if (addDependencyInstViewModel.BooleanResult != null || addDependencyInstViewModel.DoubleResult != null ||
                        addDependencyInstViewModel.DateTimeResult != null || !string.IsNullOrEmpty(addDependencyInstViewModel.StringResult))
                    {
                        List<int> DependencySucessRecords = new List<int>();
                        foreach (DependencyViewModel Dependency in addDependencyInstViewModel.Dependencies)
                        {
                            foreach (AddDependencyRowViewModel DependencyRow in Dependency.DependencyRows)
                            {
                                foreach (AddRowRuleViewModel RowRule in DependencyRow.RowRules)
                                {
                                    DependencySucessRecords.AddRange(GetRecordsIds(addDependencyInstViewModel.TableName, RowRule.Rule));
                                }
                            }
                        }

                        DependencySucessRecords = DependencySucessRecords.Distinct().ToList();

                        List<TLIdynamicAttInstValue> ListToAdd = new List<TLIdynamicAttInstValue>();

                        foreach (int Id in DependencySucessRecords)
                        {
                            ListToAdd.Add(new TLIdynamicAttInstValue
                            {
                                disable = false,
                                DynamicAttId = DynamicAttId,
                                InventoryId = Id,
                                tablesNamesId = addDependencyInstViewModel.tablesNamesId,
                                ValueBoolean = addDependencyInstViewModel.BooleanResult,
                                ValueString = addDependencyInstViewModel.StringResult,
                                ValueDateTime = addDependencyInstViewModel.DateTimeResult,
                                ValueDouble = addDependencyInstViewModel.DoubleResult
                            });
                        }

                        List<object> TableRecordsIds = _mapper.Map<List<object>>(_dbContext.GetType()
                            .GetProperties().FirstOrDefault(x => x.Name.ToLower() == addDependencyInstViewModel.TableName.ToLower()).GetValue(_dbContext, null))
                            .Where(x => !DependencySucessRecords.Contains((int)x.GetType().GetProperty("Id").GetValue(x, null))).ToList();

                        foreach (object Record in TableRecordsIds)
                        {
                            ListToAdd.Add(new TLIdynamicAttInstValue
                            {
                                disable = false,
                                DynamicAttId = DynamicAttId,
                                InventoryId = (int)Record.GetType().GetProperty("Id").GetValue(Record, null),
                                tablesNamesId = addDependencyInstViewModel.tablesNamesId,
                                ValueBoolean = addDependencyInstViewModel.BooleanDefaultValue,
                                ValueString = addDependencyInstViewModel.StringDefaultValue,
                                ValueDateTime = addDependencyInstViewModel.DateTimeDefaultValue,
                                ValueDouble = addDependencyInstViewModel.DoubleDefaultValue
                            });
                        }
                        _unitOfWork.DynamicAttInstValueRepository.AddRange(ListToAdd);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        AddDefaultValues(null, addDependencyInstViewModel, DynamicAttId);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void AddDefaultValues(AddDependencyViewModel addDependencyViewModel, AddDependencyInstViewModel addDependencyInstViewModel, int DynamicAttId)
        {
            try
            {
                if (addDependencyViewModel != null)
                {
                    if (addDependencyViewModel.BooleanDefaultValue != null || addDependencyViewModel.DoubleDefaultValue != null ||
                        addDependencyViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue))
                    {
                        List<int> RecordsIds = new List<int>();

                        if (addDependencyViewModel.LibraryAtt)
                        {
                            RecordsIds = GetLibraryRecordsIds(addDependencyViewModel.TableName);
                            List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                            foreach (int RecordId in RecordsIds)
                            {
                                ListToAdd.Add(new TLIdynamicAttLibValue
                                {
                                    disable = false,
                                    DynamicAttId = DynamicAttId,
                                    InventoryId = RecordId,
                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue,
                                    ValueString = addDependencyViewModel.StringDefaultValue
                                });
                            }

                            _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                            _unitOfWork.SaveChanges();
                        }
                        else
                        {
                            RecordsIds = GetInstallationRecordsIds(addDependencyViewModel.TableName);
                            List<TLIdynamicAttInstValue> ListToAdd = new List<TLIdynamicAttInstValue>();

                            foreach (int RecordId in RecordsIds)
                            {
                                ListToAdd.Add(new TLIdynamicAttInstValue
                                {
                                    disable = false,
                                    DynamicAttId = DynamicAttId,
                                    InventoryId = RecordId,
                                    tablesNamesId = addDependencyViewModel.tablesNamesId,
                                    ValueBoolean = addDependencyViewModel.BooleanDefaultValue,
                                    ValueDateTime = addDependencyViewModel.DateTimeDefaultValue,
                                    ValueDouble = addDependencyViewModel.DoubleDefaultValue,
                                    ValueString = addDependencyViewModel.StringDefaultValue
                                });
                            }

                            _unitOfWork.DynamicAttInstValueRepository.AddRange(ListToAdd);
                            _unitOfWork.SaveChanges();
                        }
                    }
                }
                else
                {
                    if (addDependencyInstViewModel.BooleanDefaultValue != null || addDependencyInstViewModel.DoubleDefaultValue != null ||
                        addDependencyInstViewModel.DateTimeDefaultValue != null || !string.IsNullOrEmpty(addDependencyInstViewModel.StringDefaultValue))
                    {
                        List<int> RecordsIds = new List<int>();

                        if (addDependencyInstViewModel.LibraryAtt)
                        {
                            RecordsIds = GetLibraryRecordsIds(addDependencyInstViewModel.TableName);
                            List<TLIdynamicAttLibValue> ListToAdd = new List<TLIdynamicAttLibValue>();

                            foreach (int RecordId in RecordsIds)
                            {
                                ListToAdd.Add(new TLIdynamicAttLibValue
                                {
                                    disable = false,
                                    DynamicAttId = DynamicAttId,
                                    InventoryId = RecordId,
                                    tablesNamesId = addDependencyInstViewModel.tablesNamesId,
                                    ValueBoolean = addDependencyInstViewModel.BooleanDefaultValue,
                                    ValueDateTime = addDependencyInstViewModel.DateTimeDefaultValue,
                                    ValueDouble = addDependencyInstViewModel.DoubleDefaultValue,
                                    ValueString = addDependencyInstViewModel.StringDefaultValue
                                });
                            }
                            _unitOfWork.DynamicAttLibRepository.AddRange(ListToAdd);
                            _unitOfWork.SaveChanges();
                        }
                        else
                        {
                            RecordsIds = GetInstallationRecordsIds(addDependencyInstViewModel.TableName);
                            List<TLIdynamicAttInstValue> ListToAdd = new List<TLIdynamicAttInstValue>();

                            foreach (int RecordId in RecordsIds)
                            {
                                ListToAdd.Add(new TLIdynamicAttInstValue
                                {
                                    disable = false,
                                    DynamicAttId = DynamicAttId,
                                    InventoryId = RecordId,
                                    tablesNamesId = addDependencyInstViewModel.tablesNamesId,
                                    ValueBoolean = addDependencyInstViewModel.BooleanDefaultValue,
                                    ValueDateTime = addDependencyInstViewModel.DateTimeDefaultValue,
                                    ValueDouble = addDependencyInstViewModel.DoubleDefaultValue,
                                    ValueString = addDependencyInstViewModel.StringDefaultValue
                                });
                            }
                            _unitOfWork.DynamicAttInstValueRepository.AddRange(ListToAdd);
                            _unitOfWork.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw;
            }
        }

        // General Helper Methods ...
        public List<int> GetInstallationRecordsIds(string TableName)
        {
            try
            {
                List<int> Records = new List<int>();

                // Civils ...
                if (TableName.ToLower() == TablesNames.TLIcivilWithLegs.ToString().ToLower())
                {
                    Records = _unitOfWork.CivilWithLegsRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIcivilWithoutLeg.ToString().ToLower())
                {
                    Records = _unitOfWork.CivilWithoutLegRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIcivilNonSteel.ToString().ToLower())
                {
                    Records = _unitOfWork.CivilNonSteelRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }

                // Microwaves ...
                else if (TableName.ToLower() == TablesNames.TLImwBU.ToString().ToLower())
                {
                    Records = _unitOfWork.MW_BURepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLImwDish.ToString().ToLower())
                {
                    Records = _unitOfWork.MW_DishRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLImwODU.ToString().ToLower())
                {
                    Records = _unitOfWork.MW_ODURepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLImwOther.ToString().ToLower())
                {
                    Records = _unitOfWork.Mw_OtherRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLImwRFU.ToString().ToLower())
                {
                    Records = _unitOfWork.MW_RFURepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }

                // Load Other ...
                else if (TableName.ToLower() == TablesNames.TLIloadOther.ToString().ToLower())
                {
                    Records = _unitOfWork.LoadOtherRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }

                // Power ...
                else if (TableName.ToLower() == TablesNames.TLIpower.ToString().ToLower())
                {
                    Records = _unitOfWork.PowerRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }

                // Radio ...
                else if (TableName.ToLower() == TablesNames.TLIradioAntenna.ToString().ToLower())
                {
                    Records = _unitOfWork.RadioAntennaRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIradioOther.ToString().ToLower())
                {
                    Records = _unitOfWork.RadioOtherRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIradioRRU.ToString().ToLower())
                {
                    Records = _unitOfWork.RadioRRURepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }

                // Side Arm ...
                else if (TableName.ToLower() == TablesNames.TLIsideArm.ToString().ToLower())
                {
                    Records = _unitOfWork.SideArmRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }

                // Other Inventories ...
                else if (TableName.ToLower() == TablesNames.TLIcabinet.ToString().ToLower())
                {
                    Records = _unitOfWork.CabinetRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIsolar.ToString().ToLower())
                {
                    Records = _unitOfWork.SolarRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIgenerator.ToString().ToLower())
                {
                    Records = _unitOfWork.GeneratorRepository.GetWhere(x =>
                        x.Id > 0).Select(x => x.Id).ToList();
                }

                return Records;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<int> GetLibraryRecordsIds(string TableName)
        {
            try
            {
                List<int> Records = new List<int>();

                // Civils ...
                if (CivilLibraryService._CivilWithLegLibraryEntities == null)
                {
                    CivilLibraryService._CivilWithLegLibraryEntities = _unitOfWork.CivilWithLegLibraryRepository
                        .GetWhereAndInclude(x => !x.Deleted, x => x.civilSteelSupportCategory, x => x.sectionsLegType,
                            x => x.structureType, x => x.supportTypeDesigned).ToList();
                }
                if (TableName.ToLower() == TablesNames.TLIcivilWithLegLibrary.ToString().ToLower())
                {
                    Records = CivilLibraryService._CivilWithLegLibraryEntities.Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIcivilWithoutLegLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.CivilWithoutLegLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIcivilNonSteelLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.CivilNonSteelLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }

                // Microwaves ...
                else if (TableName.ToLower() == TablesNames.TLImwBULibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.MW_BULibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLImwDishLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.MW_DishLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLImwODULibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.MW_ODULibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLImwOtherLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.MW_OtherLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLImwRFULibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.MW_RFULibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }

                // Load Other ...
                else if (TableName.ToLower() == TablesNames.TLIloadOtherLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.LoadOtherLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }

                // Power ...
                else if (TableName.ToLower() == TablesNames.TLIpowerLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.PowerLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }

                // Radio ...
                else if (TableName.ToLower() == TablesNames.TLIradioAntennaLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.RadioAntennaLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIradioOtherLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.RadioOtherLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIradioRRULibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.RadioRRULibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }

                // Side Arm ...
                else if (TableName.ToLower() == TablesNames.TLIsideArmLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.SideArmLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }

                // Other Inventories ...
                else if (TableName.ToLower() == TablesNames.TLIcabinetPowerLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.CabinetPowerLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIcabinetTelecomLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.CabinetTelecomLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIsolarLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.SolarLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }
                else if (TableName.ToLower() == TablesNames.TLIgeneratorLibrary.ToString().ToLower())
                {
                    Records = _unitOfWork.GeneratorLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).Select(x => x.Id).ToList();
                }


                return Records;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public Response<AddDependencyViewModel> AddDynamicAtts(AddDependencyViewModel addDependencyViewModel, string ConnectionString)
        {
            using (var con = new OracleConnection(ConnectionString))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required,
                        new System.TimeSpan(0, 15, 0)))
                    {
                        try
                        {
                           
                            if (UnitOfWork.AllAttributeViewManagment == null)
                            {
                                UnitOfWork.AllAttributeViewManagment = _unitOfWork.AttributeViewManagmentRepository
                                    .GetIncludeWhere(x => true, x => x.AttributeActivated, x => x.DynamicAtt,
                                        x => x.DynamicAtt.CivilWithoutLegCategory, x => x.DynamicAtt.DataType, x => x.DynamicAtt.tablesNames,
                                        x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1).ToList();
                            }

                            if (UnitOfWork.AllDynamicAttribute == null)
                            {
                                UnitOfWork.AllDynamicAttribute = _unitOfWork.DynamicAttRepository
                                    .GetIncludeWhere(x => true, x => x.CivilWithoutLegCategory, x => x.DataType,
                                        x => x.tablesNames).ToList();
                            }
                                // Map ViewModel to Entity
                            TLIdynamicAtt DynamicAttEntity = _mapper.Map<TLIdynamicAtt>(addDependencyViewModel);

                            string DataTypeName = _unitOfWork.DataTypeRepository.GetByID(addDependencyViewModel.DataTypeId.Value).Name;

                            if (DataTypeName.ToLower() == "string".ToLower())
                            {
                                DynamicAttEntity.DefaultValue = !string.IsNullOrEmpty(addDependencyViewModel.StringDefaultValue) ?
                                    addDependencyViewModel.StringDefaultValue : "";
                            }

                            else if (DataTypeName.ToLower() == "int".ToLower())
                            {
                                DynamicAttEntity.DefaultValue = addDependencyViewModel.DoubleDefaultValue != null ?
                                    addDependencyViewModel.DoubleDefaultValue.ToString() : "0";
                            }

                            else if (DataTypeName.ToLower() == "double".ToLower())
                            {
                                DynamicAttEntity.DefaultValue = addDependencyViewModel.DoubleDefaultValue != null ?
                                    addDependencyViewModel.DoubleDefaultValue.ToString() : "0";
                            }

                            else if (DataTypeName.ToLower() == "boolean".ToLower())
                            {
                                DynamicAttEntity.DefaultValue = addDependencyViewModel.BooleanDefaultValue != null ?
                                    addDependencyViewModel.BooleanDefaultValue.ToString() : "false";
                            }

                            else if (DataTypeName.ToLower() == "datetime".ToLower())
                            {
                                DynamicAttEntity.DefaultValue = addDependencyViewModel.DateTimeDefaultValue != null ?
                                    addDependencyViewModel.DateTimeDefaultValue.ToString() : DateTime.Now.ToString();
                            }

                            DynamicAttEntity.tablesNames = _unitOfWork.TablesNamesRepository.GetByID(DynamicAttEntity.tablesNamesId);

                            // Validation For Dynamic Attribute Key (Dynamic Attribute Key Can't Be Reapeated For The Same TableName)..
                            TLIdynamicAtt CheckNameInTLIDynamic = _unitOfWork.DynamicAttRepository.GetIncludeWhereFirst(x =>
                                x.Key.ToLower() == DynamicAttEntity.Key.ToLower() &&
                                x.tablesNames.TableName.ToLower() == DynamicAttEntity.tablesNames.TableName.ToLower() && x.CivilWithoutLegCategoryId == DynamicAttEntity.CivilWithoutLegCategoryId,
                                    x => x.tablesNames);

                            if (CheckNameInTLIDynamic != null)
                                return new Response<AddDependencyViewModel>(true, null, null, $"This Key {DynamicAttEntity.Key} is Already Exist in Table {DynamicAttEntity.tablesNames.TableName} as a Dynamic Attribute", (int)Constants.ApiReturnCode.fail);

                            // Validation For Dynamic Attribute Key (Can't Add New Dynamic Attribute Key If It is Already Exist in Atttribute Activated Table (TLIattributeActivated))..
                            object CheckNameInTLIAttribute = null;
                            if (DynamicAttEntity.CivilWithoutLegCategoryId == null || DynamicAttEntity.CivilWithoutLegCategoryId == 0)
                            {
                                CheckNameInTLIAttribute = _unitOfWork.AttributeActivatedRepository.GetWhereFirst(x =>
                                    x.Key.ToLower() == DynamicAttEntity.Key.ToLower() &&
                                    x.Tabel.ToLower() == DynamicAttEntity.tablesNames.TableName.ToLower());
                            }
                            else
                            {
                                CheckNameInTLIAttribute = _unitOfWork.AttActivatedCategoryRepository.GetIncludeWhereFirst(x =>
                                    x.Label.ToLower() == DynamicAttEntity.Key.ToLower() &&
                                    x.attributeActivated.Tabel.ToLower() == DynamicAttEntity.tablesNames.TableName.ToLower() &&
                                    x.civilWithoutLegCategoryId == DynamicAttEntity.CivilWithoutLegCategoryId, x => x.attributeActivated);
                            }

                            if (CheckNameInTLIAttribute != null)
                                return new Response<AddDependencyViewModel>(true, null, null, $"This Key {DynamicAttEntity.Key} is Already Exist in Table {DynamicAttEntity.tablesNames.TableName} as a Static Attribute", (int)Constants.ApiReturnCode.fail);

                            DynamicAttEntity.LibraryAtt = true;
                            DynamicAttEntity.CivilWithoutLegCategoryId = addDependencyViewModel.CivilWithoutLegCategoryId;

                            TLIdataType DataType = _unitOfWork.DataTypeRepository
                                .GetWhereFirst(x => x.Id == addDependencyViewModel.DataTypeId);

                            int DynamicAttId;
                            Dictionary<string, int> ListValuesIds = new Dictionary<string, int>();

                            _unitOfWork.DynamicAttRepository.Add(DynamicAttEntity);
                            UnitOfWork.AllDynamicAttribute.Add(DynamicAttEntity);
                            _unitOfWork.SaveChanges();

                            DynamicAttId = DynamicAttEntity.Id;

                            if (addDependencyViewModel.validations != null ? addDependencyViewModel.validations.Count > 0 : false)
                            {
                                foreach (var GeneralValidation in addDependencyViewModel.validations)
                                {
                                    if (GeneralValidation.OperationId > 0 && !string.IsNullOrEmpty(GeneralValidation.OperationValue))
                                    {
                                        //if there is validation on that dynamic attribute then add validation
                                        //validation used when i add value for that dynamic attribute then should valid form that validation
                                        TLIvalidation validation = new TLIvalidation();
                                        validation.DynamicAttId = DynamicAttId;
                                        validation.OperationId = GeneralValidation.OperationId;
                                        if (DataType.Name.ToLower() == "string")
                                        {
                                            validation.ValueString = GeneralValidation.OperationValue;
                                        }
                                        else if (DataType.Name.ToLower() == "int" || DataType.Name.ToLower() == "double" || DataType.Name.ToLower() == "float")
                                        {
                                            validation.ValueDouble = Convert.ToDouble(GeneralValidation.OperationValue);
                                        }
                                        else if (DataType.Name.ToLower() == "boolean")
                                        {
                                            validation.ValueBoolean = Convert.ToBoolean(GeneralValidation.OperationValue);
                                        }
                                        else if (DataType.Name.ToLower() == "datetime")
                                        {
                                            validation.ValueDateTime = Convert.ToDateTime(GeneralValidation.OperationValue);
                                        }
                                        //validation.OperationValue = addDependencyViewModel.validation.OperationValue;
                                        _unitOfWork.ValidationRepository.Add(validation);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }

                            if (addDependencyViewModel.Dependencies != null ? addDependencyViewModel.Dependencies.Count > 0 : false)
                            {
                                // Check if there are dependencies
                                foreach (var Dependencie in addDependencyViewModel.Dependencies)
                                {
                                    //First add depenedency 
                                    TLIdependency dependency = new TLIdependency();
                                    dependency.DynamicAttId = DynamicAttId;
                                    dependency.OperationId = Dependencie.OperationId;
                                    if (DataType.Name.ToLower() == "string")
                                    {
                                        dependency.ValueString = Dependencie.ValueString;
                                    }
                                    else if (DataType.Name.ToLower() == "int" || DataType.Name.ToLower() == "double" || DataType.Name.ToLower() == "float")
                                    {
                                        dependency.ValueDouble = Convert.ToDouble(Dependencie.ValueDouble);
                                    }
                                    else if (DataType.Name.ToLower() == "boolean")
                                    {
                                        dependency.ValueBoolean = Convert.ToBoolean(Dependencie.ValueBoolean);
                                    }
                                    else if (DataType.Name.ToLower() == "datetime")
                                    {
                                        dependency.ValueDateTime = Convert.ToDateTime(Dependencie.ValueDateTime);
                                    }
                                    //dependency.Value = Dependencie.Value;
                                    _unitOfWork.DependencieRepository.Add(dependency);
                                    _unitOfWork.SaveChanges();

                                    //Each dependency have more than one row
                                    foreach (var DependencyRow in Dependencie.DependencyRows)
                                    {
                                        //Add new row
                                        TLIrow row = new TLIrow();
                                        _unitOfWork.RowRepository.Add(row);
                                        _unitOfWork.SaveChanges();

                                        //_unitOfWork.SaveChanges();
                                        //Each depenedency row have more than 1 RowRule
                                        foreach (var RowRule in DependencyRow.RowRules)
                                        {
                                            var TableNameEntity = DynamicAttEntity.tablesNames;
                                            //First add the Rule 
                                            TLIrule Rule = _mapper.Map<TLIrule>(RowRule.Rule);
                                            Rule.tablesNamesId = TableNameEntity.Id;
                                            //TableName = _unitOfWork.AttributeActivatedRepository.GetAllAsQueryable().Where(x => x.Id == Rule.attributeActivatedId).FirstOrDefault().Tabel;
                                            _unitOfWork.RuleRepository.Add(Rule);
                                            _unitOfWork.SaveChanges();

                                            //Then add RowRule
                                            TLIrowRule RowRuleEntity = new TLIrowRule();
                                            RowRuleEntity.RuleId = Rule.Id;
                                            RowRuleEntity.RowId = row.Id;
                                            RowRuleEntity.LogicalOperationId = RowRule.LogicalOperationId;
                                            _unitOfWork.RowRuleRepository.Add(RowRuleEntity);
                                            _unitOfWork.SaveChanges();

                                        }
                                        //Then Depenedency Row
                                        TLIdependencyRow DependencyRowEntity = new TLIdependencyRow();
                                        DependencyRowEntity.DependencyId = dependency.Id;
                                        DependencyRowEntity.RowId = row.Id;
                                        DependencyRowEntity.LogicalOperationId = DependencyRow.LogicalOperationId;
                                        _unitOfWork.DependencyRowRepository.Add(DependencyRowEntity);
                                        _unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            if (addDependencyViewModel.CivilWithoutLegCategoryId != null)
                            {
                                TLIattributeViewManagment AttributeForAdd = new TLIattributeViewManagment
                                {
                                    DynamicAttId = DynamicAttId,
                                    Enable = true,
                                    EditableManagmentViewId = _unitOfWork.EditableManagmentViewRepository.GetWhereFirst(x =>
                                        x.TLItablesNames1Id == addDependencyViewModel.tablesNamesId &&
                                        (x.CivilWithoutLegCategoryId != null ?
                                            x.CivilWithoutLegCategoryId == addDependencyViewModel.CivilWithoutLegCategoryId : false)).Id
                                };

                                _unitOfWork.AttributeViewManagmentRepository.Add(AttributeForAdd);
                                UnitOfWork.AllAttributeViewManagment.Add(AttributeForAdd);
                                _unitOfWork.SaveChanges();

                            }
                            else
                            {
                                TLIattributeViewManagment AttributeForAdd = new TLIattributeViewManagment
                                {
                                    DynamicAttId = DynamicAttId,
                                    Enable = true,
                                    EditableManagmentViewId = _unitOfWork.EditableManagmentViewRepository.GetWhereFirst(x =>
                                        x.TLItablesNames1Id == addDependencyViewModel.tablesNamesId).Id
                                };

                                _unitOfWork.AttributeViewManagmentRepository.Add(AttributeForAdd);
                                UnitOfWork.AllAttributeViewManagment.Add(AttributeForAdd);
                                _unitOfWork.SaveChanges();
                            }

                            AddLibraryListValues(addDependencyViewModel, DynamicAttId);

                            tran.Commit();
                            _unitOfWork.SaveChanges();
                            transaction.Complete();

                            return new Response<AddDependencyViewModel>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<AddDependencyViewModel>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
                        }
                    }
                }
            }
        }
        public Response<DynamicAttLibForAddViewModel> GetForAdd()
        {
            try
            {
                DynamicAttLibForAddViewModel dynamicAttLib = new DynamicAttLibForAddViewModel();
                dynamicAttLib.AttributesActivated = _mapper.Map<List<BaseAttView>>(_unitOfWork.AttributeActivatedRepository.GetWhere(x => x.Tabel == TablesNames.TLIdynamicAtt.ToString()).ToList());
                dynamicAttLib.RelatedTables = GetDynamicAttRelatedTables();
                return new Response<DynamicAttLibForAddViewModel>(true, dynamicAttLib, null, null, (int)Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<DynamicAttLibForAddViewModel>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }

        }
        //Return RelatedTables
        private List<KeyValuePair<string, List<DropDownListFilters>>> GetDynamicAttRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> keyValuePairs = new List<KeyValuePair<string, List<DropDownListFilters>>>();
            var DataTypes = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.DataTypeRepository.GetAllWithoutCount());
            keyValuePairs.Add(new KeyValuePair<string, List<DropDownListFilters>>(TablesNames.TLIdataType.ToString(), DataTypes));
            var CivilWithoutLegCategories = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CivilWithoutLegCategoryRepository.GetAllWithoutCount());
            keyValuePairs.Add(new KeyValuePair<string, List<DropDownListFilters>>(TablesNames.TLIcivilWithoutLegCategory.ToString(), CivilWithoutLegCategories));
            return keyValuePairs;
        }
        //Function return all dynamic attributes and related tables
        public Response<ReturnWithFilters<DynamicAttViewModel>> GetDynamicAtts(List<FilterObjectList> filters, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                ReturnWithFilters<DynamicAttViewModel> returnWithFilters = new ReturnWithFilters<DynamicAttViewModel>();
                var DynamicAtts = _unitOfWork.DynamicAttRepository.GetAllIncludeMultiple(parameters, filters, out count, x => x.DataType, x => x.CivilWithoutLegCategory, x => x.tablesNames).ToList();
                returnWithFilters.Model = _mapper.Map<List<DynamicAttViewModel>>(DynamicAtts);
                returnWithFilters.filters = _unitOfWork.DynamicAttRepository.GetRelatedTables();
                return new Response<ReturnWithFilters<DynamicAttViewModel>>(true, returnWithFilters, null, null, (int)Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<DynamicAttViewModel>>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }

        public Response<ReturnWithFilters<DynamicAttViewModel>> GetDynamicAttsByTableName(List<FilterObjectList> filters, ParameterPagination parameters, string TableName, int? CategoryId)
        {
            try
            {
                if (UnitOfWork.AllDynamicAttribute == null)
                {
                    UnitOfWork.AllDynamicAttribute = _unitOfWork.DynamicAttRepository
                        .GetIncludeWhere(x => true, x => x.CivilWithoutLegCategory, x => x.DataType,
                            x => x.tablesNames).ToList();
                }
                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                int TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TableName.ToLower()).Id;

                List<DynamicAttViewModel> DynamicAtt = new List<DynamicAttViewModel>();

                if (filters != null && filters.Count > 0)
                {
                    foreach (FilterObjectList item in filters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();
                        string key = item.key;

                        if (key.Contains("_"))
                            key = key.Split("_")[0] + "Id";

                        AttributeFilters.Add(new StringFilterObjectList
                        {
                            key = key,
                            value = value
                        });
                    }

                    List<PropertyInfo> NonStringLibraryProps = typeof(DynamicAttViewModel).GetProperties().Where(x =>
                        x.PropertyType.Name.ToLower() != "string" &&
                        AttributeFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower())).ToList();

                    List<PropertyInfo> StringLibraryProps = typeof(DynamicAttViewModel).GetProperties().Where(x =>
                        x.PropertyType.Name.ToLower() == "string" &&
                        AttributeFilters.Exists(y =>
                            y.key.ToLower() == x.Name.ToLower())).ToList();

                    List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                        NonStringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower()) ||
                        StringLibraryProps.Exists(y => y.Name.ToLower() == x.key.ToLower())).ToList();

                    List<int> DynamicAttBaseFilter = UnitOfWork.AllDynamicAttribute.Where(x =>
                        x.tablesNamesId == TableNameId && (CategoryId != null ? (x.CivilWithoutLegCategoryId == CategoryId) : true)).Select(x => x.Id).ToList();

                    IEnumerable<TLIdynamicAtt> DynamicAtts = _unitOfWork.DynamicAttRepository
                        .GetWhere(x => DynamicAttBaseFilter.Contains(x.Id));

                    foreach (StringFilterObjectList z in LibraryPropsAttributeFilters)
                    {
                        if (NonStringLibraryProps.Exists(y => y.Name.ToLower() == z.key.ToLower()))
                        {
                            DynamicAtts = DynamicAtts.Where(x => z.value.Any(w => NonStringLibraryProps.FirstOrDefault(y => y.Name.ToLower() == z.key.ToLower())
                                .GetValue(_mapper.Map<DynamicAttViewModel>(x)) != null ? NonStringLibraryProps.FirstOrDefault(y => y.Name.ToLower() == z.key.ToLower())
                                .GetValue(_mapper.Map<DynamicAttViewModel>(x)).ToString().ToLower() == w.ToLower() : false));
                        }
                        else
                        {
                            DynamicAtts = DynamicAtts.Where(x => z.value.Any(w => StringLibraryProps.FirstOrDefault(y => y.Name.ToLower() == z.key.ToLower())
                                .GetValue(_mapper.Map<DynamicAttViewModel>(x)) != null ? StringLibraryProps.FirstOrDefault(y => y.Name.ToLower() == z.key.ToLower())
                                .GetValue(_mapper.Map<DynamicAttViewModel>(x)).ToString().ToLower().StartsWith(w.ToLower()) : false));
                        }
                    }

                    List<int> DynamicAttIds = DynamicAtts.Select(x => x.Id).ToList();

                    DynamicAtt = _mapper.Map<List<DynamicAttViewModel>>(UnitOfWork.AllDynamicAttribute.Where(x =>
                        x.Id > 0 && x.tablesNamesId == TableNameId && DynamicAttIds.Contains(x.Id)).ToList());
                }
                else
                {
                    DynamicAtt = _mapper.Map<List<DynamicAttViewModel>>(UnitOfWork.AllDynamicAttribute.Where(x =>
                        x.Id > 0 && x.tablesNamesId == TableNameId && (CategoryId != null ? (x.CivilWithoutLegCategoryId == CategoryId) : true)).ToList());
                }

                int Count = DynamicAtt.Count();

                DynamicAtt = DynamicAtt.Skip((parameters.PageNumber - 1) * parameters.PageSize).
                    Take(parameters.PageSize).ToList();

                return new Response<ReturnWithFilters<DynamicAttViewModel>>(true, new ReturnWithFilters<DynamicAttViewModel>
                {
                    Model = DynamicAtt,
                    filters = _unitOfWork.DynamicAttRepository.GetRelatedTables()
                }, null, null, (int)Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<DynamicAttViewModel>>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        //Function get dynamic attribute by Id
        public Response<DynamicAttViewModel> GetById(int Id)
        {
            try
            {
                TLIdynamicAtt DynamicAtt = UnitOfWork.AllDynamicAttribute.FirstOrDefault(x => x.Id == Id);
                return new Response<DynamicAttViewModel>(true, _mapper.Map<DynamicAttViewModel>(DynamicAtt), null, null, (int)Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<DynamicAttViewModel>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        //Function Update dynamic attribute
        public async Task<Response<DynamicAttViewModel>> Edit(EditDynamicAttViewModel DynamicAttViewModel)
        {
            try
            {
                TLIdynamicAtt OldDynamicAttData = UnitOfWork.AllDynamicAttribute.AsQueryable().AsNoTracking()
                    .FirstOrDefault(x => x.Id == DynamicAttViewModel.Id);

                if (OldDynamicAttData.DataTypeId != DynamicAttViewModel.DataTypeId)
                {
                    TLIdataType NewDataType = _unitOfWork.DataTypeRepository
                        .GetWhereFirst(x => x.Id == DynamicAttViewModel.DataTypeId.Value);

                    if (NewDataType.Name.ToLower() == "string".ToLower())
                    {
                        if (OldDynamicAttData.LibraryAtt)
                        {
                            List<TLIdynamicAttLibValue> DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => x.DynamicAttId == DynamicAttViewModel.Id).ToList();

                            if (OldDynamicAttData.DataType.Name.ToLower() == "double".ToLower())
                            {
                                foreach (TLIdynamicAttLibValue DynamicAttValue in DynamicAttValues)
                                {
                                    if (DynamicAttValue.ValueDouble != null)
                                    {
                                        DynamicAttValue.ValueString = DynamicAttValue.ValueDouble.ToString();
                                        DynamicAttValue.ValueDouble = null;
                                    }
                                }
                            }
                            else if (OldDynamicAttData.DataType.Name.ToLower() == "boolean".ToLower())
                            {
                                foreach (TLIdynamicAttLibValue DynamicAttValue in DynamicAttValues)
                                {
                                    if (DynamicAttValue.ValueBoolean != null)
                                    {
                                        DynamicAttValue.ValueString = DynamicAttValue.ValueBoolean.ToString();
                                        DynamicAttValue.ValueBoolean = null;
                                    }
                                }
                            }
                            else if (OldDynamicAttData.DataType.Name.ToLower() == "datetime".ToLower())
                            {
                                foreach (TLIdynamicAttLibValue DynamicAttValue in DynamicAttValues)
                                {
                                    if (DynamicAttValue.ValueDateTime != null)
                                    {
                                        DynamicAttValue.ValueString = DynamicAttValue.ValueDateTime.ToString();
                                        DynamicAttValue.ValueDateTime = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            List<TLIdynamicAttInstValue> DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => x.DynamicAttId == DynamicAttViewModel.Id).ToList();

                            if (OldDynamicAttData.DataType.Name.ToLower() == "double".ToLower())
                            {
                                foreach (TLIdynamicAttInstValue DynamicAttValue in DynamicAttValues)
                                {
                                    DynamicAttValue.ValueString = DynamicAttValue.ValueDouble.ToString();
                                    DynamicAttValue.ValueDouble = null;
                                }
                            }
                            else if (OldDynamicAttData.DataType.Name.ToLower() == "boolean".ToLower())
                            {
                                foreach (TLIdynamicAttInstValue DynamicAttValue in DynamicAttValues)
                                {
                                    DynamicAttValue.ValueString = DynamicAttValue.ValueBoolean.ToString();
                                    DynamicAttValue.ValueBoolean = null;
                                }
                            }
                            else if (OldDynamicAttData.DataType.Name.ToLower() == "datetime".ToLower())
                            {
                                foreach (TLIdynamicAttInstValue DynamicAttValue in DynamicAttValues)
                                {
                                    DynamicAttValue.ValueString = DynamicAttValue.ValueDateTime.ToString();
                                    DynamicAttValue.ValueDateTime = null;
                                }
                            }
                        }
                    }
                    else if (NewDataType.Name.ToLower() == "double".ToLower())
                    {
                        if (OldDynamicAttData.LibraryAtt)
                        {
                            List<TLIdynamicAttLibValue> DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => x.DynamicAttId == DynamicAttViewModel.Id && !string.IsNullOrEmpty(x.ValueString) &&
                                    !string.IsNullOrWhiteSpace(x.ValueString)).ToList();

                            foreach (TLIdynamicAttLibValue DynamicAttValue in DynamicAttValues)
                            {
                                DynamicAttValue.ValueDouble = double.Parse(DynamicAttValue.ValueString);
                                DynamicAttValue.ValueString = null;
                            }
                        }
                        else
                        {
                            List<TLIdynamicAttInstValue> DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => x.DynamicAttId == DynamicAttViewModel.Id && !string.IsNullOrEmpty(x.ValueString) &&
                                    !string.IsNullOrWhiteSpace(x.ValueString)).ToList();

                            foreach (TLIdynamicAttInstValue DynamicAttValue in DynamicAttValues)
                            {
                                DynamicAttValue.ValueDouble = double.Parse(DynamicAttValue.ValueString);
                                DynamicAttValue.ValueString = null;
                            }
                        }
                    }
                    else if (NewDataType.Name.ToLower() == "boolean".ToLower())
                    {
                        if (OldDynamicAttData.LibraryAtt)
                        {
                            List<TLIdynamicAttLibValue> DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => x.DynamicAttId == DynamicAttViewModel.Id && !string.IsNullOrEmpty(x.ValueString) &&
                                    !string.IsNullOrWhiteSpace(x.ValueString)).ToList();

                            foreach (TLIdynamicAttLibValue DynamicAttValue in DynamicAttValues)
                            {
                                DynamicAttValue.ValueBoolean = bool.Parse(DynamicAttValue.ValueString);
                                DynamicAttValue.ValueString = null;
                            }
                        }
                        else
                        {
                            List<TLIdynamicAttInstValue> DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => x.DynamicAttId == DynamicAttViewModel.Id && !string.IsNullOrEmpty(x.ValueString) &&
                                    !string.IsNullOrWhiteSpace(x.ValueString)).ToList();

                            foreach (TLIdynamicAttInstValue DynamicAttValue in DynamicAttValues)
                            {
                                DynamicAttValue.ValueBoolean = bool.Parse(DynamicAttValue.ValueString);
                                DynamicAttValue.ValueString = null;
                            }
                        }
                    }
                    else if (NewDataType.Name.ToLower() == "datetime".ToLower())
                    {
                        if (OldDynamicAttData.LibraryAtt)
                        {
                            List<TLIdynamicAttLibValue> DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => x.DynamicAttId == DynamicAttViewModel.Id && !string.IsNullOrEmpty(x.ValueString) &&
                                    !string.IsNullOrWhiteSpace(x.ValueString)).ToList();

                            foreach (TLIdynamicAttLibValue DynamicAttValue in DynamicAttValues)
                            {
                                DynamicAttValue.ValueDateTime = DateTime.Parse(DynamicAttValue.ValueString);
                                DynamicAttValue.ValueString = null;
                            }
                        }
                        else
                        {
                            List<TLIdynamicAttInstValue> DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => x.DynamicAttId == DynamicAttViewModel.Id && !string.IsNullOrEmpty(x.ValueString) &&
                                    !string.IsNullOrWhiteSpace(x.ValueString)).ToList();

                            foreach (TLIdynamicAttInstValue DynamicAttValue in DynamicAttValues)
                            {
                                DynamicAttValue.ValueDateTime = DateTime.Parse(DynamicAttValue.ValueString);
                                DynamicAttValue.ValueString = null;
                            }
                        }
                    }
                }

                TLItablesNames TableName = _unitOfWork.TablesNamesRepository
                    .GetWhereFirst(x => x.Id == DynamicAttViewModel.tablesNamesId);

                TLIdynamicAtt CheckNameInTLIDynamic = _unitOfWork.DynamicAttRepository.GetIncludeWhereFirst(x =>
                    x.Key.ToLower() == DynamicAttViewModel.Key.ToLower() && x.Id != DynamicAttViewModel.Id &&
                    x.tablesNamesId == DynamicAttViewModel.tablesNamesId && x.CivilWithoutLegCategoryId == DynamicAttViewModel.CivilWithoutLegCategoryId,
                        x => x.tablesNames);

                if (CheckNameInTLIDynamic != null)
                    return new Response<DynamicAttViewModel>(true, null, null, $"This Key {DynamicAttViewModel.Key} is Already Exist", (int)Constants.ApiReturnCode.fail);

                // Validation For Dynamic Attribute Key (Can't Add New Dynamic Attribute Key If It is Already Exist in Atttribute Activated Table (TLIattributeActivated))..
                TLIattributeActivated CheckNameInTLIAttribute = _unitOfWork.AttributeActivatedRepository.GetWhereFirst(x =>
                    x.Key.ToLower() == DynamicAttViewModel.Key.ToLower() &&
                    x.Tabel.ToLower() == TableName.TableName.ToLower());

                if (CheckNameInTLIAttribute != null)
                    return new Response<DynamicAttViewModel>(true, null, null, $"This Key {DynamicAttViewModel.Key} is Already Exist in Table {TableName.TableName} as a Static Attribute", (int)Constants.ApiReturnCode.fail);

                //---------------------------------------------------

                TLIdynamicAtt DynamicAtt = _mapper.Map<TLIdynamicAtt>(DynamicAttViewModel);
                DynamicAtt.DefaultValue = OldDynamicAttData.DefaultValue;

                await _unitOfWork.DynamicAttRepository.UpdateItem(DynamicAtt);

                UnitOfWork.AllAttributeViewManagment.FirstOrDefault(x => x.DynamicAttId != null ?
                    x.DynamicAttId == DynamicAtt.Id : false).DynamicAtt = DynamicAtt;

                UnitOfWork.AllDynamicAttribute.Remove(UnitOfWork.AllDynamicAttribute
                    .FirstOrDefault(x => x.Id == DynamicAtt.Id));
                UnitOfWork.AllDynamicAttribute.Add(DynamicAtt);

                await _unitOfWork.SaveChangesAsync();

                return new Response<DynamicAttViewModel>(true, null, null, null, (int)Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<DynamicAttViewModel>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        //Get dependency Installation attributes depened on layer name and table name 
        #region Helper Method..
        public string GetTableNameByLayer(string Layer, bool IsLibrary)
        {
            string TableName = "";
            if (Layer.ToLower().ToLower() == Constants.Layers.CivilWithLegs.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLIcivilWithLegLibrary.ToString();
                else
                    TableName = TablesNames.TLIcivilWithLegs.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.CivilWithoutLeg.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLIcivilWithoutLegLibrary.ToString();
                else
                    TableName = TablesNames.TLIcivilWithoutLeg.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.CivilNonSteel.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLIcivilNonSteelLibrary.ToString();
                else
                    TableName = TablesNames.TLIcivilNonSteel.ToString();
            }
            else if ((Layer.ToLower() == Constants.Layers.Cabinet.ToString().ToLower() ||
                Layer.ToLower() == Constants.Layers.CabinetPower.ToString().ToLower() ||
                Layer.ToLower() == Constants.Layers.CabinetTelecom.ToString().ToLower()) && !IsLibrary)
            {
                TableName = TablesNames.TLIcabinet.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.CabinetPower.ToString().ToLower() && IsLibrary)
            {
                TableName = TablesNames.TLIcabinetPowerLibrary.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.CabinetTelecom.ToString().ToLower() && IsLibrary)
            {
                TableName = TablesNames.TLIcabinetTelecomLibrary.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.Solar.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLIsolarLibrary.ToString();
                else
                    TableName = TablesNames.TLIsolar.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.Generator.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLIgeneratorLibrary.ToString();
                else
                    TableName = TablesNames.TLIgenerator.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.SideArm.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLIsideArmLibrary.ToString();
                else
                    TableName = TablesNames.TLIsideArm.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.Site.ToString().ToLower())
            {
                TableName = TablesNames.TLIsite.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.MW_BU.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLImwBULibrary.ToString();
                else
                    TableName = TablesNames.TLImwBU.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.MW_RFU.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLImwRFULibrary.ToString();
                else
                    TableName = TablesNames.TLImwRFU.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.MW_ODU.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLImwODULibrary.ToString();
                else
                    TableName = TablesNames.TLImwODU.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.MW_Dish.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLImwDishLibrary.ToString();
                else
                    TableName = TablesNames.TLImwDish.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.MW_Other.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLImwOtherLibrary.ToString();
                else
                    TableName = TablesNames.TLImwOther.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.RadioAntenna.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLIradioAntennaLibrary.ToString();
                else
                    TableName = TablesNames.TLIradioAntenna.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.RadioRRU.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLIradioRRULibrary.ToString();
                else
                    TableName = TablesNames.TLIradioRRU.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.RadioOther.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLIradioOtherLibrary.ToString();
                else
                    TableName = TablesNames.TLIradioOther.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.Power.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLIpowerLibrary.ToString();
                else
                    TableName = TablesNames.TLIpower.ToString();
            }
            else if (Layer.ToLower() == Constants.Layers.LoadOther.ToString().ToLower())
            {
                if (IsLibrary)
                    TableName = TablesNames.TLIloadOtherLibrary.ToString();
                else
                    TableName = TablesNames.TLIloadOther.ToString();
            }
            return TableName;
        }
        #endregion
        public Response<DependencyColumnForAdd> GetDependencyInst(string Layer, int? CategoryId, bool IsLibrary = false)
        {
            try
            {
                string TableName = GetTableNameByLayer(Layer, IsLibrary);

                DependencyColumnForAdd DependencyColumn = new DependencyColumnForAdd();

                TLItablesNames TableNamesEntity = _unitOfWork.TablesNamesRepository
                    .GetWhereFirst(x => x.TableName.ToLower() == TableName.ToLower());

                List<DependencyColumn> MainAttributes = new List<DependencyColumn>();
                List<DependencyColumn> ListAttributes = new List<DependencyColumn>();

                if (CategoryId == null)
                {
                    MainAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetWhere(x => x.enable && x.Key.ToLower() != "id" && x.Tabel.ToLower() == TableNamesEntity.TableName.ToLower() && x.Key.ToLower() != "disable" &&
                            x.Key.ToLower() != "active" && x.Key.ToLower() != "deleted" && x.Key.ToLower() != "required")
                        .Select(x => new DependencyColumn(x.Label, x.DataType, false, null, x.Id))
                        .ToList();

                    ListAttributes = MainAttributes
                        .Where(x => x.columnType.ToLower() == "list").ToList();
                }
                else
                {
                    List<TLIattActivatedCategory> AttributeActivatedCategories = _unitOfWork.AttActivatedCategoryRepository
                        .GetWhere(x => x.enable && x.IsLibrary == IsLibrary && x.civilWithoutLegCategoryId == CategoryId.Value).ToList();

                    List<TLIattributeActivated> AttributeActivated = _unitOfWork.AttributeActivatedRepository
                        .GetWhere(x => x.Tabel.ToLower() == TableNamesEntity.TableName.ToLower() && x.Key.ToLower() != "disable" &&
                            x.Key.ToLower() != "active" && x.Key.ToLower() != "id" && x.Key.ToLower() != "deleted" && x.Key.ToLower() != "required")
                        .ToList();

                    foreach (TLIattributeActivated Attribute in AttributeActivated)
                    {
                        TLIattActivatedCategory AttributeActivatedCategory = AttributeActivatedCategories
                            .FirstOrDefault(x => x.attributeActivatedId == Attribute.Id);

                        if (AttributeActivatedCategory != null)
                        {
                            Attribute.Label = AttributeActivatedCategory.Label;
                            Attribute.Required = AttributeActivatedCategory.Required;
                            Attribute.enable = AttributeActivatedCategory.enable;
                            Attribute.Description = AttributeActivatedCategory.Description;
                        }
                    }

                    MainAttributes = AttributeActivated
                        .Select(x => new DependencyColumn(x.Label, x.DataType, false, null, x.Id)).ToList();

                    ListAttributes = MainAttributes
                        .Where(x => x.columnType.ToLower() == "list").ToList();
                }

                if (TableName.ToLower() == TablesNames.TLIsite.ToString().ToLower())
                {
                    foreach (DependencyColumn Attribute in ListAttributes)
                    {
                        if (Attribute.columnName.ToLower() == "sitestatus_name")
                            MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "sitestatus_name")
                                .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SiteStatusRepository
                                    .GetWhere(x => x.Active && !x.Deleted).ToList());

                        else if (Attribute.columnName.ToLower() == "region_name")
                            MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "region_name")
                                .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RegionRepository
                                    .GetAllWithoutCount().ToList());

                        else if (Attribute.columnName.ToLower() == "area_name")
                            MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "area_name")
                                .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.AreaRepository
                                    .GetAllWithoutCount().ToList());
                    }
                }
                else
                {
                    if (!IsLibrary)
                    {
                        //
                        // Civils ...
                        //
                        if (TableName.ToLower() == TablesNames.TLIcivilWithLegs.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "locationtype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "locationtype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.LocationTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "basetype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "basetype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.BaseTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "civilwithlegslib_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "civilwithlegslib_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CivilWithLegLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "basecivilwithlegtype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "basecivilwithlegtype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.BaseCivilWithLegsTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "guylinetype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "guylinetype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.GuyLineTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "supporttypeimplemented_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "supporttypeimplemented_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SupportTypeImplementedRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "enforcmentcategory_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "enforcmentcategory_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.EnforcmentCategoryRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLIcivilWithoutLeg.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "civilwithoutlegslib_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "civilwithoutlegslib_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CivilWithoutLegLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "subtype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "subtype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SubTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Delete).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLIcivilNonSteel.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "civilnonsteellibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "civilnonsteellibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CivilNonSteelLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "subtype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "subtype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SubTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Delete).ToList());
                            }
                        }

                        //
                        // SideArm ...
                        //
                        else if (TableName.ToLower() == TablesNames.TLIsideArm.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "sidearmlibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "sidearmlibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SideArmLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "sidearminstallationplace_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "sidearminstallationplace_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SideArmInstallationPlaceRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "sidearmtype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "sidearmtype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SideArmTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                            }
                        }

                        // 
                        // OtherInventories ...
                        //
                        else if (TableName.ToLower() == TablesNames.TLIcabinet.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "cabinetpowerlibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "cabinetpowerlibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CabinetPowerLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "cabinettelecomlibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "cabinettelecomlibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CabinetTelecomLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "renewablecabinettype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "renewablecabinettype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RenewableCabinetTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLIsolar.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "solarlibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "solarlibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SolarLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "cabinet_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "cabinet_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CabinetRepository
                                            .GetAllWithoutCount().ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLIgenerator.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "basegeneratortype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "basegeneratortype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.BaseGeneratorTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "generatorlibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "generatorlibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.GeneratorLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());
                            }
                        }

                        //
                        // Loads ...
                        //

                        // MWs ...
                        else if (TableName.ToLower() == TablesNames.TLImwBU.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "basebu_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "basebu_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.BaseBURepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "installationplace_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationplace_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "mwbulibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwbulibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_BULibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "maindish_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "maindish_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishRepository
                                            .GetAllWithoutCount().ToList());

                                else if (Attribute.columnName.ToLower() == "sddish_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "sddish_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishRepository
                                            .GetAllWithoutCount().ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLImwDish.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "repeatertype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "repeatertype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RepeaterTypeRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                                else if (Attribute.columnName.ToLower() == "polarityonlocation_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "polarityonlocation_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.PolarityOnLocationRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                                else if (Attribute.columnName.ToLower() == "itemconnectto_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "itemconnectto_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.ItemConnectToRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                                else if (Attribute.columnName.ToLower() == "mwdishlibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwdishlibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishLibraryRepository
                                            .GetWhere(x => !x.Deleted && x.Active).ToList());

                                else if (Attribute.columnName.ToLower() == "installationplace_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationplace_name").value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                                        .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLImwODU.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "mw_dish_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mw_dish_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishRepository
                                            .GetAllWithoutCount().ToList());

                                else if (Attribute.columnName.ToLower() == "oduinstallationtype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "oduinstallationtype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OduInstallationTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "mwodulibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwodulibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_ODULibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLImwRFU.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "mwrfulibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwrfulibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_RFULibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "mwport_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwport_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_PortRepository
                                            .GetAllWithoutCount().ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLImwOther.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "mwotherlibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwotherlibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_OtherLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());
                            }
                        }

                        // Radios ...
                        else if (TableName.ToLower() == TablesNames.TLIradioAntenna.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "installationplace_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationplace_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "radioantennalibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "radioantennalibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioAntennaLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLIradioRRU.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "radiorrulibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "radiorrulibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioRRULibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "radioantenna_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "radioantenna_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioAntennaRepository
                                            .GetAllWithoutCount().ToList());

                                else if (Attribute.columnName.ToLower() == "installationplace_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationplace_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLIradioOther.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "radiootherlibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "radiootherlibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioOtherLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "installationplace_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationplace_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                            }
                        }

                        // Power ...
                        else if (TableName.ToLower() == TablesNames.TLIpower.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "owner_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "installationplace_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationplace_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "powerlibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "powerlibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.PowerLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                                else if (Attribute.columnName.ToLower() == "powertype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "powertype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.PowerTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Delete).ToList());
                            }
                        }

                        // LoadOther ...
                        else if (TableName.ToLower() == TablesNames.TLIloadOther.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "loadotherlibrary_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "loadotherlibrary_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.LoadOtherLibraryRepository
                                            .GetWhere(x => x.Active && !x.Deleted).ToList());
                            }
                        }
                    }
                    else
                    {
                        //
                        // Civils ...
                        //
                        if (TableName.ToLower() == TablesNames.TLIcivilWithLegLibrary.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "supporttypedesigned_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "supporttypedesigned_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SupportTypeDesignedRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                                else if (Attribute.columnName.ToLower() == "sectionslegtype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "sectionslegtype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SectionsLegTypeRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                                else if (Attribute.columnName.ToLower() == "structuretype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "structuretype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.StructureTypeRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                                else if (Attribute.columnName.ToLower() == "civilsteelsupportcategory_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "civilsteelsupportcategory_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CivilSteelSupportCategoryRepository
                                            .GetAllWithoutCount().ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLIcivilWithoutLegLibrary.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "structuretype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "structuretype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.StructureTypeRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                                else if (Attribute.columnName.ToLower() == "civilsteelsupportcategory_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "civilsteelsupportcategory_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CivilSteelSupportCategoryRepository
                                            .GetAllWithoutCount().ToList());

                                else if (Attribute.columnName.ToLower() == "installationcivilwithoutlegstype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationcivilwithoutlegstype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationCivilwithoutLegsTypeRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                                else if (Attribute.columnName.ToLower() == "civilwithoutlegcategory_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "civilwithoutlegcategory_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CivilWithoutLegCategoryRepository
                                            .GetWhere(x => !x.disable).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLIcivilNonSteelLibrary.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "civilnonsteeltype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "civilnonsteeltype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CivilNonSteelTypeRepository
                                            .GetWhere(x => !x.Disable).ToList());
                            }
                        }

                        // 
                        // OtherInventories ...
                        //
                        else if (TableName.ToLower() == TablesNames.TLIcabinetPowerLibrary.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "cabinetpowertype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "cabinetpowertype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CabinetPowerTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Delete).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLIcabinetTelecomLibrary.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "telecomtype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "telecomtype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.TelecomTypeRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLIsolarLibrary.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "capacity_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "capacity_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CapacityRepository
                                            .GetWhere(x => !x.Disable && !x.Delete).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLIgeneratorLibrary.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "capacity_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "capacity_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CapacityRepository
                                            .GetWhere(x => !x.Disable && !x.Delete).ToList());
                            }
                        }

                        //
                        // Loads ...
                        //

                        // MWs ...
                        else if (TableName.ToLower() == TablesNames.TLImwBULibrary.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "diversitytype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "diversitytype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.DiversityTypeRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLImwRFULibrary.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "diversitytype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "diversitytype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.DiversityTypeRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                                else if (Attribute.columnName.ToLower() == "boardtype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "boardtype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.BoardTypeRepository
                                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLImwODULibrary.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "parity_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "parity_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.ParityRepository
                                            .GetWhere(x => !x.Disable && !x.Delete).ToList());
                            }
                        }
                        else if (TableName.ToLower() == TablesNames.TLImwDishLibrary.ToString().ToLower())
                        {
                            foreach (DependencyColumn Attribute in ListAttributes)
                            {
                                if (Attribute.columnName.ToLower() == "polaritytype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "polaritytype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.PolarityTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Delete).ToList());

                                else if (Attribute.columnName.ToLower() == "astype_name")
                                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "astype_name")
                                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.AsTypeRepository
                                            .GetWhere(x => !x.Disable && !x.Delete).ToList());
                            }
                        }
                    }
                }

                #region No Need For This Code Anymore..
                //// for library and library dynamic att
                //if (LayerName == "RelatedTables")
                //{
                //    if (TableName == LoadSubType.TLIradioRRU.ToString())
                //    {
                //        foreach (DependencyColumn Attribute in ListAttributes)
                //        {
                //            if (Attribute.columnName.ToLower() == "radiorrulibrary_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioRRULibraryRepository
                //                    .GetWhere(x => x.Active && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "owner_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "radioantenna_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioAntennaRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "installationplace_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //        }
                //    }
                //    else if (TableName == LoadSubType.TLIradioAntenna.ToString())
                //    {
                //        foreach (DependencyColumn Attribute in ListAttributes)
                //        {
                //            if (Attribute.columnName.ToLower() == "owner_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "installationplace_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "radioantennalibrary_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioAntennaLibraryRepository
                //                    .GetWhere(x => x.Active && !x.Deleted).ToList());
                //        }
                //    }
                //    else if (TableName == LoadSubType.TLImwBU.ToString())
                //    {
                //        foreach (DependencyColumn Attribute in ListAttributes)
                //        {
                //            if (Attribute.columnName.ToLower() == "basebu_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.BaseBURepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "installationplace_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "owner_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "mwbulibrary_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_BULibraryRepository
                //                    .GetWhere(x => x.Active && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "maindish_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "sddish_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //        }
                //    }
                //    else if (TableName == LoadSubType.TLImwDish.ToString())
                //    {
                //        foreach (DependencyColumn Attribute in ListAttributes)
                //        {
                //            if (Attribute.columnName.ToLower() == "owner_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "repeatertype_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RepeaterTypeRepository
                //                    .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                //            else if (Attribute.columnName.ToLower() == "polarityonlocation_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.PolarityOnLocationRepository
                //                    .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                //            else if (Attribute.columnName.ToLower() == "itemconnectto_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.ItemConnectToRepository
                //                    .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                //            else if (Attribute.columnName.ToLower() == "mwdishlibrary_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishLibraryRepository
                //                    .GetWhere(x => !x.Deleted && x.Active).ToList());

                //            else if (Attribute.columnName.ToLower() == "installationplace_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //        }
                //    }
                //    else if (TableName == LoadSubType.TLImwODU.ToString())
                //    {
                //        foreach (DependencyColumn Attribute in ListAttributes)
                //        {
                //            if (Attribute.columnName.ToLower() == "owner_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "mw_dish_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "oduinstallationtype_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OduInstallationTypeRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "mwodulibrary_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_ODULibraryRepository
                //                    .GetWhere(x => x.Active && !x.Deleted).ToList());
                //        }
                //    }
                //    else if (TableName == LoadSubType.TLImwRFU.ToString())
                //    {
                //        foreach (DependencyColumn Attribute in ListAttributes)
                //        {
                //            if (Attribute.columnName.ToLower() == "mwrfulibrary_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_RFULibraryRepository
                //                    .GetWhere(x => x.Active && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "owner_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "mwport_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_PortRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //        }
                //    }
                //    else if (TableName == LoadSubType.TLImwOther.ToString())
                //    {
                //        ListAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwotherlibrary_name")
                //            .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_OtherLibraryRepository
                //                        .GetWhere(x => x.Active && !x.Deleted).ToList());
                //    }
                //    else if (TableName == LoadSubType.TLIradioOther.ToString())
                //    {
                //        foreach (DependencyColumn Attribute in ListAttributes)
                //        {
                //            if (Attribute.columnName.ToLower() == "radiootherlibrary_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioOtherLibraryRepository
                //                    .GetWhere(x => x.Active && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "owner_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "installationplace_name")
                //                Attribute.value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                //                    .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //        }
                //    }
                //    else if (TableName == LoadSubType.TLIloadOther.ToString())
                //    {
                //        ListAttributes.FirstOrDefault(x => x.columnName.ToLower() == "loadotherlibrary_name")
                //            .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.LoadOtherLibraryRepository
                //                        .GetWhere(x => x.Active && !x.Deleted).ToList());
                //    }
                //    else if (TableName == OtherInventoryType.TLIcabinet.ToString())
                //    {

                //    }
                //    else if (TableName == OtherInventoryType.TLIgenerator.ToString())
                //    {
                //        List<DependencyColumn> generatormAttributes = _unitOfWork.AttributeActivatedRepository
                //            .GetWhere(x => x.Tabel.ToLower() == TableName.ToLower() && !x.Key.ToLower().Contains("id"))
                //            .Select(x => new DependencyColumn(x.Label, x.DataType, false, null, x.Id))
                //            .ToList();

                //        DependencyColumn.dependencyColumns.AddRange(generatormAttributes);

                //        List<KeyValuePair<string, List<DropDownListFilters>>> generatorRelatedTablesLibrary = _unitOfWork.GeneratorRepository.GetRelatedTables();

                //        foreach (KeyValuePair<string, List<DropDownListFilters>> generatorRelatedTableLibrary in generatorRelatedTablesLibrary)
                //        {
                //            TLIattributeActivated Record = _unitOfWork.AttributeActivatedRepository
                //                .GetWhereFirst(x => x.Key.ToLower() == generatorRelatedTableLibrary.Key.ToLower() && x.Tabel.ToLower() == TableName.ToLower());

                //            DependencyColumn.dependencyColumns.Add(new DependencyColumn(Record.Label, Record.DataType, false, generatorRelatedTableLibrary.Value, Record.Id));
                //        }
                //    }
                //    else if (TableName == OtherInventoryType.TLIsolar.ToString())
                //    {
                //        List<DependencyColumn> solarAttributes = _unitOfWork.AttributeActivatedRepository
                //            .GetWhere(x => x.Tabel.ToLower() == TableName.ToLower() && !x.Key.ToLower().Contains("id"))
                //            .Select(x => new DependencyColumn(x.Label, x.DataType, false, null, x.Id)).ToList();

                //        DependencyColumn.dependencyColumns.AddRange(solarAttributes);

                //        List<KeyValuePair<string, List<DropDownListFilters>>> SolarRelatedTablesLibrary = _unitOfWork.SolarRepository.GetRelatedTables();

                //        foreach (KeyValuePair<string, List<DropDownListFilters>> SolarRelatedTableLibrary in SolarRelatedTablesLibrary)
                //        {
                //            TLIattributeActivated Record = _unitOfWork.AttributeActivatedRepository
                //                .GetWhereFirst(x => x.Key.ToLower() == SolarRelatedTableLibrary.Key.ToLower() && x.Tabel.ToLower() == TableName.ToLower());

                //            DependencyColumn.dependencyColumns.Add(new DependencyColumn(Record.Label, Record.DataType, false, SolarRelatedTableLibrary.Value, Record.Id));
                //        }
                //    }
                //    else if (TableName == TablesNames.TLIsideArm.ToString())
                //    {
                //        List<DependencyColumn> SideArmAttributes = _unitOfWork.AttributeActivatedRepository
                //            .GetWhere(x => x.Tabel.ToLower() == TableName.ToLower() && !x.Key.ToLower().Contains("id"))
                //            .Select(x => new DependencyColumn(x.Label, x.DataType, false, null, x.Id))
                //            .ToList();

                //        DependencyColumn.dependencyColumns.AddRange(SideArmAttributes);

                //        List<KeyValuePair<string, List<DropDownListFilters>>> SideArmRelatedTables = _unitOfWork.SideArmRepository.GetRelatedTables();

                //        foreach (KeyValuePair<string, List<DropDownListFilters>> SideArmRelatedTable in SideArmRelatedTables)
                //        {
                //            TLIattributeActivated Record = _unitOfWork.AttributeActivatedRepository
                //                .GetWhereFirst(x => x.Key.ToLower() == SideArmRelatedTable.Key.ToLower() && x.Tabel.ToLower() == TableName.ToLower());

                //            DependencyColumn.dependencyColumns.Add(new DependencyColumn(Record.Label, Record.DataType, false, SideArmRelatedTable.Value, Record.Id));
                //        }
                //    }
                //    else if (TableName == Constants.CivilType.TLIcivilWithLegs.ToString())
                //    {
                //        List<DependencyColumn> civilWithLegAttributes = _unitOfWork.AttributeActivatedRepository
                //            .GetWhere(x => x.Tabel.ToLower() == TableName.ToLower() && !x.Key.ToLower().Contains("id"))
                //            .Select(x => new DependencyColumn(x.Label, x.DataType, false, null, x.Id))
                //            .ToList();

                //        DependencyColumn.dependencyColumns.AddRange(civilWithLegAttributes);

                //        List<KeyValuePair<string, List<DropDownListFilters>>> civilWithLegLibraryRelatedTables = _unitOfWork.CivilWithLegsRepository.GetRelatedTables();
                //        foreach (KeyValuePair<string, List<DropDownListFilters>> civilWithLegLibraryRelatedTable in civilWithLegLibraryRelatedTables)
                //        {
                //            TLIattributeActivated Record = _unitOfWork.AttributeActivatedRepository
                //                .GetWhereFirst(x => x.Key.ToLower() == civilWithLegLibraryRelatedTable.Key.ToLower() && x.Tabel.ToLower() == TableName.ToLower());

                //            DependencyColumn.dependencyColumns.Add(new DependencyColumn(Record.Label, Record.DataType, false, civilWithLegLibraryRelatedTable.Value, Record.Id));
                //        }
                //    }
                //    else if (TableName == Constants.CivilType.TLIcivilWithoutLeg.ToString())
                //    {
                //        List<DependencyColumn> civilWithoutLegAttributes = _unitOfWork.AttributeActivatedRepository
                //            .GetWhere(x => x.Tabel.ToLower() == TableName.ToLower() && !x.Key.ToLower().Contains("id"))
                //            .Select(x => new DependencyColumn(x.Label, x.DataType, false, null, x.Id))
                //            .ToList();

                //        DependencyColumn.dependencyColumns.AddRange(civilWithoutLegAttributes);

                //        List<KeyValuePair<string, List<DropDownListFilters>>> civilWithoutLegLibraryRelatedTables = _unitOfWork.CivilWithoutLegRepository.GetRelatedTables();

                //        foreach (KeyValuePair<string, List<DropDownListFilters>> civilWithoutLegLibraryRelatedTable in civilWithoutLegLibraryRelatedTables)
                //        {
                //            TLIattributeActivated Record = _unitOfWork.AttributeActivatedRepository
                //                .GetWhereFirst(x => x.Key.ToLower() == civilWithoutLegLibraryRelatedTable.Key.ToLower() && x.Tabel.ToLower() == TableName.ToLower());

                //            DependencyColumn.dependencyColumns.Add(new DependencyColumn(Record.Label, Record.DataType, false, civilWithoutLegLibraryRelatedTable.Value, Record.Id));
                //        }
                //    }
                //    else if (TableName == Constants.CivilType.TLIcivilNonSteel.ToString())
                //    {
                //        List<DependencyColumn> civilNonSteelAttributes = _unitOfWork.AttributeActivatedRepository
                //            .GetWhere(x => x.Tabel.ToLower() == TableName.ToLower() && !x.Key.ToLower().Contains("id"))
                //            .Select(x => new DependencyColumn(x.Label, x.DataType, false, null, x.Id))
                //            .ToList();

                //        DependencyColumn.dependencyColumns.AddRange(civilNonSteelAttributes);

                //        List<KeyValuePair<string, List<DropDownListFilters>>> civilNonSteelLibraryRelatedTables = _unitOfWork.CivilNonSteelRepository.GetRelatedTables();

                //        foreach (KeyValuePair<string, List<DropDownListFilters>> civilNonSteelLibraryRelatedTable in civilNonSteelLibraryRelatedTables)
                //        {
                //            TLIattributeActivated Record = _unitOfWork.AttributeActivatedRepository
                //                .GetWhereFirst(x => x.Key.ToLower() == civilNonSteelLibraryRelatedTable.Key.ToLower() && x.Tabel.ToLower() == TableName.ToLower());

                //            DependencyColumn.dependencyColumns.Add(new DependencyColumn(Record.Label, Record.DataType, false, civilNonSteelLibraryRelatedTable.Value, Record.Id));
                //        }
                //    }
                //}
                //else
                //{
                //    if (LayerName == "LoadLayer")
                //    {
                //        if (TableName == LoadSubType.TLIradioRRU.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "radiorrulibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "radiorrulibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioRRULibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "owner_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "radioantenna_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "radioantenna_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioAntennaRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "installationplace_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationplace_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == LoadSubType.TLIradioAntenna.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "owner_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "installationplace_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationplace_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "radioantennalibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "radioantennalibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioAntennaLibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == LoadSubType.TLImwBU.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "basebu_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "basebu_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.BaseBURepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "installationplace_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationplace_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "owner_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "mwbulibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwbulibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_BULibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "maindish_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "maindish_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "sddish_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "sddish_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == LoadSubType.TLImwDish.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "owner_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "repeatertype_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "repeatertype_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RepeaterTypeRepository
                //                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                //                else if (Attribute.columnName.ToLower() == "polarityonlocation_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "polarityonlocation_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.PolarityOnLocationRepository
                //                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                //                else if (Attribute.columnName.ToLower() == "itemconnectto_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "itemconnectto_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.ItemConnectToRepository
                //                            .GetWhere(x => !x.Deleted && !x.Disable).ToList());

                //                else if (Attribute.columnName.ToLower() == "mwdishlibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwdishlibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishLibraryRepository
                //                            .GetWhere(x => !x.Deleted && x.Active).ToList());

                //                else if (Attribute.columnName.ToLower() == "installationplace_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationplace_name").value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == LoadSubType.TLImwODU.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "owner_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "mw_dish_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mw_dish_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_DishRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "oduinstallationtype_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "oduinstallationtype_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OduInstallationTypeRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "mwodulibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwodulibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_ODULibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == LoadSubType.TLImwRFU.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "mwrfulibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwrfulibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_RFULibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "owner_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "mwport_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwport_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_PortRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == LoadSubType.TLImwOther.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "mwotherlibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "mwotherlibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.MW_OtherLibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == LoadSubType.TLIradioOther.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "radiootherlibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "radiootherlibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RadioOtherLibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "owner_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "installationplace_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "installationplace_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.InstallationPlaceRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == LoadSubType.TLIloadOther.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "loadotherlibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "loadotherlibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.LoadOtherLibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());
                //            }
                //        }
                //    }
                //    else if (LayerName == "OtherInventoryLayer")
                //    {
                //        if (TableName == OtherInventoryType.TLIcabinet.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "cabinetpowerlibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "cabinetpowerlibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CabinetPowerLibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "cabinettelecomlibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "cabinettelecomlibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CabinetTelecomLibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "renewablecabinettype_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "renewablecabinettype_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RenewableCabinetTypeRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == OtherInventoryType.TLIgenerator.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "basegeneratortype_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "basegeneratortype_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.BaseGeneratorTypeRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "generatorlibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "generatorlibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.GeneratorLibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == OtherInventoryType.TLIsolar.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "solarlibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "solarlibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SolarLibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "cabinet_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "cabinet_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CabinetRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //            }
                //        }
                //    }
                //    else if (LayerName == "SideArmLayer")
                //    {
                //        foreach (DependencyColumn Attribute in ListAttributes)
                //        {
                //            if (Attribute.columnName.ToLower() == "sidearmlibrary_name")
                //                MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "sidearmlibrary_name")
                //                    .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SideArmLibraryRepository
                //                        .GetWhere(x => x.Active && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "sidearminstallationplace_name")
                //                MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "sidearminstallationplace_name")
                //                    .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SideArmInstallationPlaceRepository
                //                        .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "owner_name")
                //                MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                //                    .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                        .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "sidearmtype_name")
                //                MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "sidearmtype_name")
                //                    .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SideArmTypeRepository
                //                        .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //        }
                //    }
                //    else if (LayerName == "CivilLayer")
                //    {
                //        if (TableName == Constants.CivilType.TLIcivilWithLegs.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "locationtype_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "locationtype_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.LocationTypeRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "basetype_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "basetype_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.BaseTypeRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "owner_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "civilwithlegslib_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "civilwithlegslib_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CivilWithLegLibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "basecivilwithlegtype_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "basecivilwithlegtype_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.BaseCivilWithLegsTypeRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "guylinetype_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "guylinetype_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.GuyLineTypeRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "supporttypeimplemented_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "supporttypeimplemented_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SupportTypeImplementedRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "enforcmentcategory_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "enforcmentcategory_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.EnforcmentCategoryRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == Constants.CivilType.TLIcivilWithoutLeg.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "civilwithoutlegslib_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "civilwithoutlegslib_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CivilWithoutLegLibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "owner_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "subtype_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "subtype_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SubTypeRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //            }
                //        }
                //        else if (TableName == Constants.CivilType.TLIcivilNonSteel.ToString())
                //        {
                //            foreach (DependencyColumn Attribute in ListAttributes)
                //            {
                //                if (Attribute.columnName.ToLower() == "civilnonsteellibrary_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "civilnonsteellibrary_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.CivilNonSteelLibraryRepository
                //                            .GetWhere(x => x.Active && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "owner_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "owner_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.OwnerRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //                else if (Attribute.columnName.ToLower() == "subtype_name")
                //                    MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "subtype_name")
                //                        .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SubTypeRepository
                //                            .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //            }
                //        }
                //    }
                //    else if (LayerName == "SiteLayer")
                //    {
                //        foreach (DependencyColumn Attribute in ListAttributes)
                //        {
                //            if (Attribute.columnName.ToLower() == "sitestatus_name")
                //                MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "sitestatus_name")
                //                    .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.SiteStatusRepository
                //                        .GetWhere(x => x.Active && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "region_name")
                //                MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "region_name")
                //                    .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.RegionRepository
                //                        .GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //            else if (Attribute.columnName.ToLower() == "area_name")
                //                MainAttributes.FirstOrDefault(x => x.columnName.ToLower() == "area_name")
                //                    .value = _mapper.Map<List<DropDownListFilters>>(_unitOfWork.AreaRepository
                //                        .GetWhere(x => !x.Disable && !x.Deleted).ToList());
                //        }
                //    }
                //}
                #endregion

                DependencyColumn.dependencyColumns.AddRange(MainAttributes);

                List<DependencyColumn> DynamicAtts = _unitOfWork.DynamicAttRepository.GetDynamicInstAtts(TableNamesEntity.Id, CategoryId)
                    .Select(x => new DependencyColumn(x.Key, x.DataType, true, null, x.Id)).ToList();

                DependencyColumn.dependencyColumns.AddRange(DynamicAtts);

                //List of operations 
                DependencyColumn.Operations = _mapper.Map<List<OperationViewModel>>(_unitOfWork.OperationRepository.GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //List of logial operations
                DependencyColumn.LogicalOperations = _mapper.Map<List<LogicalOperationViewModel>>(_unitOfWork.LogicalOperationRepository.GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //List of data types
                DependencyColumn.DataTypes = _mapper.Map<List<DataTypeViewModel>>(_unitOfWork.DataTypeRepository.GetWhere(x => !x.Disable && !x.Deleted).ToList());

                //get table name Id
                DependencyColumn.TableNameId = TableNamesEntity.Id;

                return new Response<DependencyColumnForAdd>(true, DependencyColumn, null, null, (int)Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<DependencyColumnForAdd>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        //Filter data depened on Dependency 
        private void FilterInstData(AddDependencyInstViewModel addDependencyViewModel, string TableName, int DynamicAttId, int? ValueId, OracleConnection Con)
        {
            AddInstRuleViewModel Rule = null;
            TLIattributeActivated AttributeEntity = null;
            string OperationName = null;
            string LogicalOperationName = null;
            AddRowRuleViewModel Last = null;
            string Type = null;
            CultureInfo culture = new CultureInfo("en-US");
            string LibraryTableName = string.Empty;
            //LibraryTableName = LibraryTableName + "Library";
            TLIdynamicAtt dynamicAttEntity = null;
            string dynamicAttDataType = _unitOfWork.DataTypeRepository.GetByID((int)addDependencyViewModel.DataTypeId).Name.ToLower();
            Dictionary<string, string> Months = new Dictionary<string, string>();
            Months.Add("1", "JAN");
            Months.Add("2", "FEB");
            Months.Add("3", "MAR");
            Months.Add("4", "APR");
            Months.Add("5", "MAY");
            Months.Add("6", "JUN");
            Months.Add("7", "JUL");
            Months.Add("8", "AUG");
            Months.Add("9", "SEP");
            Months.Add("10", "OCT");
            Months.Add("11", "NOV");
            Months.Add("12", "DEC");
            Dictionary<string, string> Libraries = new Dictionary<string, string>();
            Libraries.Add("TLIcivilWithLegs", "TLIcivilWithLegLibrary");
            Libraries.Add("TLIcivilWithoutLeg", "TLIcivilWithoutLegLibrary");
            Libraries.Add("TLIcivilNonSteel", "TLIcivilNonSteelLibrary");
            Libraries.Add("TLIsideArm", "TLIsideArmLibrary");
            Libraries.Add("TLIcabinet", "TLIcabinetPowerLibrary");
            Libraries.Add("TLIsolar", "TLIsolarLibrary");
            Libraries.Add("TLIgenerator", "TLIgeneratorLibrary");
            Libraries.Add("TLIloadOther", "TLIloadOtherLibrary");
            Libraries.Add("TLIpower", "TLIpowerLibrary");
            Libraries.Add("TLIradioAntenna", "TLIradioAntennaLibrary");
            Libraries.Add("TLIRadioRRU", "TLIradioRRULibrary");
            Libraries.Add("TLIradioOther", "TLIradioOtherLibrary");
            Libraries.Add("TLImwBU", "TLImwBULibrary");
            Libraries.Add("TLImwDish", "TLImwDishLibrary");
            Libraries.Add("TLImwODU", "TLImwODULibrary");
            Libraries.Add("TLImwRFU", "TLImwRFULibrary");
            Libraries.Add("TLImwOther", "TLImwOtherLibrary");
            Libraries.TryGetValue(TableName, out LibraryTableName);
            var LibraryTableNameId = _unitOfWork.TablesNamesRepository.GetWhereSelectFirst(x => x.TableName == LibraryTableName, x => new { x.Id }).Id;
            //Build a query have all the layers
            string query = $"select distinct \"{TableName}\".\"Id\" from \"TLIsite\"" +
                            "Left join \"TLIcivilSiteDate\" on \"TLIsite\".\"SiteCode\" = \"TLIcivilSiteDate\".\"SiteCode\"" +
                            "Left join \"TLIallCivilInst\" on \"TLIcivilSiteDate\".\"allCivilInstId\" = \"TLIallCivilInst\".\"Id\"" +
                            "Left join \"TLIcivilWithLegs\" on \"TLIcivilWithLegs\".\"Id\" = \"TLIallCivilInst\".\"civilWithLegsId\"" +
                            "Left join \"TLIcivilWithLegLibrary\" on \"TLIcivilWithLegs\".\"CivilWithLegsLibId\" = \"TLIcivilWithLegLibrary\".\"Id\"" +
                            "Left join \"TLIcivilWithoutLeg\" on \"TLIcivilWithoutLeg\".\"Id\" = \"TLIallCivilInst\".\"civilWithoutLegId\"" +
                            "Left join \"TLIcivilWithoutLegLibrary\" on \"TLIcivilWithoutLeg\".\"CivilWithoutlegsLibId\" = \"TLIcivilWithoutLegLibrary\".\"Id\"" +
                            "Left join \"TLIcivilNonSteel\" on \"TLIcivilNonSteel\".\"Id\" = \"TLIallCivilInst\".\"civilNonSteelId\"" +
                            "Left join \"TLIcivilNonSteelLibrary\" on \"TLIcivilNonSteelLibrary\".\"Id\" = \"TLIcivilNonSteel\".\"CivilNonSteelLibraryId\"" +
                            "Left join \"TLIotherInSite\" on \"TLIsite\".\"SiteCode\" = \"TLIotherInSite\".\"SiteCode\"" +
                            "Left join \"TLIallOtherInventoryInst\" on \"TLIallOtherInventoryInst\".\"Id\" = \"TLIotherInSite\".\"allOtherInventoryInstId\"" +
                            "Left join \"TLIcabinet\" on \"TLIallOtherInventoryInst\".\"cabinetId\" = \"TLIcabinet\".\"Id\"" +
                            "Left join \"TLIcabinetPowerLibrary\" on \"TLIcabinet\".\"CabinetPowerLibraryId\" = \"TLIcabinetPowerLibrary\".\"Id\"" +
                            "Left join \"TLIcabinetTelecomLibrary\" on \"TLIcabinet\".\"CabinetTelecomLibraryId\" = \"TLIcabinetTelecomLibrary\".\"Id\"" +
                            "Left join \"TLIgenerator\" on \"TLIgenerator\".\"Id\" = \"TLIallOtherInventoryInst\".\"generatorId\"" +
                            "Left join \"TLIgeneratorLibrary\" on \"TLIgenerator\".\"GeneratorLibraryId\" = \"TLIgeneratorLibrary\".\"Id\"" +
                            "Left join \"TLIsolar\" on \"TLIsolar\".\"Id\" = \"TLIallOtherInventoryInst\".\"solarId\" or \"TLIcabinet\".\"Id\" = \"TLIsolar\".\"CabinetId\"" +
                            "Left join \"TLIsolarLibrary\" on \"TLIsolar\".\"SolarLibraryId\" = \"TLIsolarLibrary\".\"Id\"" +
                            "Left join \"TLIcivilLoads\" on  \"TLIsite\".\"SiteCode\" = \"TLIcivilLoads\".\"SiteCode\"" +
                            "left join \"TLIcivilLoadLegs\" on \"TLIcivilLoadLegs\".\"civilLoadsId\" = \"TLIcivilLoads\".\"Id\"" +
                            "left join \"TLIleg\" on \"TLIleg\".\"Id\" = \"TLIcivilLoadLegs\".\"legId\"" +
                            "left join \"TLIsideArm\" on \"TLIsideArm\".\"Id\" = \"TLIcivilLoads\".\"sideArmId\"" +
                            "left join \"TLIsideArmLibrary\" on \"TLIsideArmLibrary\".\"Id\" = \"TLIsideArm\".\"sideArmLibraryId\"" +
                            "Left join \"TLIallLoadInst\" on \"TLIallLoadInst\".\"Id\" = \"TLIcivilLoads\".\"allLoadInstId\"" +
                            "Left join \"TLImwDish\" on \"TLImwDish\".\"Id\" = \"TLIallLoadInst\".\"mwDishId\"" +
                            "Left join \"TLImwDishLibrary\" on \"TLImwDishLibrary\".\"Id\" = \"TLImwDish\".\"MwDishLibraryId\"" +
                            "Left join \"TLImwBu\" on \"TLImwBu\".\"Id\" = \"TLIallLoadInst\".\"mwBUId\" or \"TLImwDish\".\"Id\" = \"TLImwBu\".\"MainDishId\"" +
                            "Left join \"TLImwBULibrary\" on \"TLImwBULibrary\".\"Id\" = \"TLImwBu\".\"MwBULibraryId\"" +
                            "left join \"TLImwPort\" on \"TLImwPort\".\"MwBUId\" = \"TLImwBu\".\"Id\"" +
                            "Left join \"TLImwODU\" on \"TLImwODU\".\"Id\" = \"TLIallLoadInst\".\"mwODUId\" or \"TLImwDish\".\"Id\" = \"TLImwODU\".\"Mw_DishId\"" +
                            "Left join \"TLImwODULibrary\" on \"TLImwODULibrary\".\"Id\" = \"TLImwODU\".\"MwODULibraryId\"" +
                            "Left join \"TLImwRFU\" on \"TLImwRFU\".\"Id\" = \"TLIallLoadInst\".\"mwRFUId\" or \"TLImwPort\".\"Id\" = \"TLImwRFU\".\"MwPortId\"" +
                            "Left join \"TLImwRFULibrary\" on \"TLImwRFULibrary\".\"Id\" = \"TLImwRFU\".\"MwRFULibraryId\"" +
                            "Left join \"TLImwOther\" on \"TLImwOther\".\"Id\" = \"TLIallLoadInst\".\"mwOtherId\"" +
                            "Left join \"TLImwOtherLibrary\" on \"TLImwOtherLibrary\".\"Id\" = \"TLImwOther\".\"mwOtherLibraryId\"" +
                            "Left join \"TLIradioAntenna\" on \"TLIradioAntenna\".\"Id\" = \"TLIallLoadInst\".\"radioAntennaId\"" +
                            "Left join \"TLIradioAntennaLibrary\" on \"TLIradioAntennaLibrary\".\"Id\" = \"TLIradioAntenna\".\"radioAntennaLibraryId\"" +
                            "Left join \"TLIRadioRRU\" on \"TLIRadioRRU\".\"Id\" = \"TLIallLoadInst\".\"radioRRUId\" or \"TLIradioAntenna\".\"Id\" = \"TLIRadioRRU\".\"radioAntennaId\"" +
                            "Left join \"TLIradioRRULibrary\" on \"TLIradioRRULibrary\".\"Id\" = \"TLIRadioRRU\".\"radioRRULibraryId\"" +
                            "Left join \"TLIradioOther\" on \"TLIradioOther\".\"Id\" = \"TLIallLoadInst\".\"radioOtherId\"" +
                            "Left join \"TLIradioOtherLibrary\" on \"TLIradioOtherLibrary\".\"Id\" = \"TLIradioOther\".\"radioOtherLibraryId\"" +
                            "Left join \"TLIpower\" on \"TLIpower\".\"Id\" = \"TLIallLoadInst\".\"powerId\"" +
                            "Left join \"TLIpowerLibrary\" on \"TLIpowerLibrary\".\"Id\" = \"TLIpower\".\"powerLibraryId\"" +
                            "Left join \"TLIloadOther\" on \"TLIloadOther\".\"Id\" = \"TLIallLoadInst\".\"loadOtherId\"" +
                            "Left join \"TLIloadOtherLibrary\" on \"TLIloadOtherLibrary\".\"Id\" = \"TLIloadOther\".\"loadOtherLibraryId\"" +
                            $"Left join \"TLIdynamicAttLibValue\" on \"TLIdynamicAttLibValue\".\"InventoryId\" = \"{LibraryTableName}\".\"Id\"" +
                            $"Left join \"TLIdynamicAttInstValue\" on \"TLIdynamicAttInstValue\".\"InventoryId\" = \"{TableName}\".\"Id\"" +
                            "Left join \"TLIdynamicAtt\" on \"TLIdynamicAttLibValue\".\"DynamicAttId\" = \"TLIdynamicAtt\".\"Id\" or \"TLIdynamicAttInstValue\".\"DynamicAttId\" = \"TLIdynamicAtt\".\"Id\"" +
                            $"where ( \"{TableName}\".\"Id\" is not null ) and (";
            //bool test = false;
            string RowRuleLogicalOperation = null;
            //loop on each dependency 
            if (addDependencyViewModel.Dependencies.Count > 0)
            {
                foreach (var Dependencie in addDependencyViewModel.Dependencies)
                {
                    //test = false;
                    //loop on each dependency row
                    foreach (var DependencyRow in Dependencie.DependencyRows)
                    {
                        //to speperate every condition with () like (TLIsite.SiteCode == 1)
                        query += "(";
                        //select logical operation if not null
                        if (DependencyRow.LogicalOperationId != null)
                        {
                            LogicalOperationName = _unitOfWork.LogicalOperationRepository.GetWhereSelectFirst(x => x.Id == DependencyRow.LogicalOperationId, x => new { x.Name }).Name;
                        }
                        Last = DependencyRow.RowRules.Last();
                        //loop on each rowrule 
                        foreach (var RowRule in DependencyRow.RowRules)
                        {

                            Rule = RowRule.Rule;
                            if (Rule.attributeActivatedId != null)
                            {
                                AttributeEntity = _unitOfWork.AttributeActivatedRepository.GetWhereFirst(x => x.Id == Rule.attributeActivatedId);
                            }
                            else if (Rule.dynamicAttId != null)
                            {
                                dynamicAttEntity = _unitOfWork.DynamicAttRepository.GetByID((int)Rule.dynamicAttId);
                                //dynamicAttDataType = _unitOfWork.DataTypeRepository.GetByID((int)dynamicAttEntity.DataTypeId).Name.ToLower();
                            }
                            if (Rule.OperationId != null)
                            {
                                OperationName = _unitOfWork.OperationRepository.GetWhereSelectFirst(x => x.Id == Rule.OperationId, x => new { x.Name }).Name;
                                if (OperationName == "==")
                                {
                                    OperationName = "=";
                                }
                            }
                            if (RowRule.LogicalOperationId != null)
                            {
                                RowRuleLogicalOperation = _unitOfWork.LogicalOperationRepository.GetAllAsQueryable().Where(x => x.Id == RowRule.LogicalOperationId).Select(x => x.Name).FirstOrDefault();
                            }
                            //check if value is string or not if string check if after the condition there is logical operation or not and build query
                            if (Rule.attributeActivatedId != null && Rule.OperationId != null)
                            {
                                //test = true;
                                //vhevk condition if we need check for key (gh)
                                if (AttributeEntity.DataType == "string" || AttributeEntity.Key.Contains("Code"))
                                {
                                    if (RowRule.LogicalOperationId != null)
                                    {
                                        query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} '{Rule.OperationValueString}' {RowRuleLogicalOperation} ";
                                    }
                                    else
                                    {
                                        query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} '{Rule.OperationValueString}'";
                                    }
                                }
                                else
                                {
                                    //else check if after the condition there is logical operation or not and build query
                                    if (RowRule.LogicalOperationId != null)
                                    {
                                        if (Rule.OperationValueBoolean != null)
                                        {
                                            string res = string.Empty;
                                            if (Rule.OperationValueBoolean == true)
                                            {
                                                res = "1";
                                            }
                                            else
                                            {
                                                res = "0";
                                            }
                                            query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} {res} {RowRuleLogicalOperation} ";
                                        }
                                        else if (Rule.OperationValueDateTime != null)
                                        {
                                            var test = Rule.OperationValueDateTime.ToString().Split('/');
                                            Months.TryGetValue(test[1], out test[1]);
                                            var res = string.Join('/', test);
                                            query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} '{res}' {RowRuleLogicalOperation} ";
                                        }
                                        else if (Rule.OperationValueDouble != null)
                                        {
                                            query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} {Rule.OperationValueDouble} {RowRuleLogicalOperation} ";
                                        }
                                    }
                                    else
                                    {
                                        if (Rule.OperationValueBoolean != null)
                                        {
                                            string res = string.Empty;
                                            if (Rule.OperationValueBoolean == true)
                                            {
                                                res = "1";
                                            }
                                            else
                                            {
                                                res = "0";
                                            }
                                            query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} {res}";
                                        }
                                        else if (Rule.OperationValueDateTime != null)
                                        {
                                            var test = Rule.OperationValueDateTime.ToString().Split('/');
                                            string month = string.Empty;
                                            string monthValue = test[0];
                                            Months.TryGetValue(monthValue, out month);
                                            test[0] = test[1];
                                            test[1] = month;
                                            var res = string.Join('/', test);
                                            query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} '{res}'";
                                        }
                                        else if (Rule.OperationValueDouble != null)
                                        {
                                            query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} {Rule.OperationValueDouble}";
                                        }
                                    }

                                }
                            }
                            else if (Rule.dynamicAttId != null && Rule.OperationId != null)
                            {
                                if (RowRule.LogicalOperationId != null)
                                {
                                    if (dynamicAttDataType == "string")
                                    {
                                        query += $"\"{Rule.TableName}\".\"{"ValueString"}\" {OperationName} '{Rule.OperationValueString}' {RowRuleLogicalOperation} ";
                                    }
                                    else if (dynamicAttDataType == "double")
                                    {
                                        query += $"\"{Rule.TableName}\".\"{"ValueDouble"}\" {OperationName} {Rule.OperationValueDouble} {RowRuleLogicalOperation} ";
                                    }
                                    else if (dynamicAttDataType == "datetime")
                                    {
                                        var test = Rule.OperationValueDateTime.ToString().Split('/');
                                        string month = string.Empty;
                                        string monthValue = test[0];
                                        Months.TryGetValue(monthValue, out month);
                                        test[0] = test[1];
                                        test[1] = month;
                                        var res = string.Join('/', test);
                                        query += $"\"{Rule.TableName}\".\"{"ValueDateTime"}\" {OperationName} '{res}' {RowRuleLogicalOperation} ";
                                    }
                                    else if (dynamicAttDataType == "boolean")
                                    {
                                        string res = string.Empty;
                                        if (Rule.OperationValueBoolean == true)
                                        {
                                            res = "1";
                                        }
                                        else
                                        {
                                            res = "0";
                                        }
                                        query += $"\"{Rule.TableName}\".\"{"ValueBoolean"}\" {OperationName} {res} {RowRuleLogicalOperation} ";
                                    }
                                }
                                else
                                {
                                    if (dynamicAttDataType == "string")
                                    {
                                        query += $"\"{Rule.TableName}\".\"{"ValueString"}\" {OperationName} '{Rule.OperationValueString}'";
                                    }
                                    else if (dynamicAttDataType == "double")
                                    {
                                        query += $"\"{Rule.TableName}\".\"{"ValueDouble"}\" {OperationName} {Rule.OperationValueDouble}";
                                    }
                                    else if (dynamicAttDataType == "datetime")
                                    {
                                        var test = Rule.OperationValueDateTime.ToString().Split('/');
                                        string month = string.Empty;
                                        string monthValue = test[0];
                                        Months.TryGetValue(monthValue, out month);
                                        test[0] = test[1];
                                        test[1] = month;
                                        var res = string.Join('/', test);
                                        query += $"\"{Rule.TableName}\".\"{"ValueDateTime"}\" {OperationName} '{res}'";
                                    }
                                    else if (dynamicAttDataType == "boolean")
                                    {
                                        string res = string.Empty;
                                        if (Rule.OperationValueBoolean == true)
                                        {
                                            res = "1";
                                        }
                                        else
                                        {
                                            res = "0";
                                        }
                                        query += $"\"{Rule.TableName}\".\"{"ValueBoolean"}\" {OperationName} {res}";
                                    }
                                }
                            }
                            // filter in tables relation like civil site,civil load ....
                            else if (Rule.attributeActivatedId == null && Rule.OperationId == null && Rule.TableName != null)
                            {
                                //if i want to filter just TLIallCivilInst then execute that condition
                                if (Rule.TableName == Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    query += $"\"TLIallCivilInst\".\"civilWithLegsId\" is not null and \"TLIallCivilInst\".\"civilWithoutLegId\" is null and \"TLIallCivilInst\".\"civilNonSteelId\" is null";
                                }
                                else if (Rule.TableName == Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    query += $"\"TLIallCivilInst\".\"civilWithLegsId\" is null and \"TLIallCivilInst\".\"civilWithoutLegId\" is not null and \"TLIallCivilInst\".\"civilNonSteelId\" is null";
                                }
                                else if (Rule.TableName == Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    query += $"\"TLIallCivilInst\".\"civilWithLegsId\" is null and \"TLIallCivilInst\".\"civilWithoutLegId\" is null and \"TLIallCivilInst\".\"civilNonSteelId\" is not null";
                                }
                            }
                        }
                        query += ")";
                        //Add logical operation
                        if (DependencyRow.LogicalOperationId != null)
                        {
                            query += $" {LogicalOperationName} ";
                        }
                    }
                    query += ")";
                    //fetch data from database depened on last query
                    var InventoryIds = fetchdata(query, Con);
                    if (InventoryIds.Count > 0)
                    {
                        using (var cmd = Con.CreateCommand())
                        {
                            //connection.Open();
                            List<string> StringValues = new List<string>();
                            List<double> DoubleValues = new List<double>();
                            List<DateTime> DateTimeValues = new List<DateTime>();
                            List<int> BooleanValues = new List<int>();
                            List<int> DynamicIds = new List<int>();
                            List<int> tablesNamesIds = new List<int>();
                            List<int?> dynamicListValuesIds = new List<int?>();
                            for (int i = 0; i < InventoryIds.Count; i++)
                            {
                                if (ValueId == null)
                                {
                                    if (dynamicAttDataType == "string")
                                    {
                                        StringValues.Add(Dependencie.ValueString);
                                    }
                                    else if (dynamicAttDataType == "double")
                                    {
                                        DoubleValues.Add((double)Dependencie.ValueDouble);
                                    }
                                    else if (dynamicAttDataType == "datetime")
                                    {

                                        DateTimeValues.Add(Convert.ToDateTime(Dependencie.ValueDateTime));
                                    }
                                    else if (dynamicAttDataType == "boolean")
                                    {
                                        int res = 0;
                                        if (Dependencie.ValueBoolean == true)
                                        {
                                            res = 1;
                                        }
                                        BooleanValues.Add(res);
                                    }
                                }
                                DynamicIds.Add(DynamicAttId);
                                tablesNamesIds.Add(addDependencyViewModel.tablesNamesId);
                                if (ValueId != null)
                                {
                                    dynamicListValuesIds.Add((int)ValueId);
                                }
                                else
                                {
                                    dynamicListValuesIds.Add(null);
                                }

                            }
                            OracleParameter Value = new OracleParameter();
                            if (dynamicAttDataType == "string")
                            {
                                Value.OracleDbType = OracleDbType.NVarchar2;
                                Value.Value = StringValues.ToArray();
                            }
                            else if (dynamicAttDataType == "double")
                            {
                                Value.OracleDbType = OracleDbType.BinaryDouble;
                                Value.Value = DoubleValues.ToArray();
                            }
                            else if (dynamicAttDataType == "datetime")
                            {
                                Value.OracleDbType = OracleDbType.TimeStamp;
                                Value.Value = DateTimeValues.ToArray();
                            }
                            else if (dynamicAttDataType == "boolean")
                            {
                                Value.OracleDbType = OracleDbType.Int16;
                                Value.Value = BooleanValues.ToArray();
                            }

                            OracleParameter DynamicId = new OracleParameter();
                            DynamicId.OracleDbType = OracleDbType.Int32;
                            DynamicId.Value = DynamicIds.ToArray();
                            OracleParameter tablesNamesId = new OracleParameter();
                            tablesNamesId.OracleDbType = OracleDbType.Int32;
                            tablesNamesId.Value = tablesNamesIds.ToArray();
                            OracleParameter Ids = new OracleParameter();
                            Ids.OracleDbType = OracleDbType.Int32;
                            Ids.Value = InventoryIds.ToArray();
                            OracleParameter dynamicListValuesId = new OracleParameter();
                            dynamicListValuesId.OracleDbType = OracleDbType.Int32;
                            dynamicListValuesId.Value = dynamicListValuesIds.ToArray();
                            //OracleCommand cmd = connection.CreateCommand();
                            if (dynamicAttDataType != "list")
                            {
                                if (dynamicAttDataType == "string")
                                {
                                    cmd.CommandText = $"INSERT INTO \"TLIdynamicAttInstValue\" (\"DynamicAttId\", \"tablesNamesId\", \"InventoryId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                                }
                                else if (dynamicAttDataType == "double")
                                {
                                    cmd.CommandText = $"INSERT INTO \"TLIdynamicAttInstValue\" (\"DynamicAttId\", \"tablesNamesId\", \"InventoryId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                                }
                                else if (dynamicAttDataType == "datetime")
                                {
                                    cmd.CommandText = $"INSERT INTO \"TLIdynamicAttInstValue\" (\"DynamicAttId\", \"tablesNamesId\", \"InventoryId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                                }
                                else if (dynamicAttDataType == "boolean")
                                {
                                    cmd.CommandText = $"INSERT INTO \"TLIdynamicAttInstValue\" (\"DynamicAttId\", \"tablesNamesId\", \"InventoryId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                                }
                                cmd.ArrayBindCount = InventoryIds.Count;
                                cmd.Parameters.Add(DynamicId);
                                cmd.Parameters.Add(tablesNamesId);
                                cmd.Parameters.Add(Ids);
                                cmd.Parameters.Add(Value);
                            }
                            else
                            {
                                cmd.CommandText = $"INSERT INTO \"TLIdynamicAttInstValue\" (\"DynamicAttId\", \"tablesNamesId\", \"InventoryId\", \"dynamicListValuesId\") VALUES ( :1, :2, :3, :4)";
                                cmd.ArrayBindCount = InventoryIds.Count;
                                cmd.Parameters.Add(DynamicId);
                                cmd.Parameters.Add(tablesNamesId);
                                cmd.Parameters.Add(Ids);
                                cmd.Parameters.Add(dynamicListValuesId);
                            }
                            cmd.ExecuteNonQuery();
                            //connection.Close();
                        }
                    }
                }
            }
            GC.Collect(2);
        }
        private List<int> fetchdata(string query, OracleConnection Con)
        {
            List<int> InventoryIds = new List<int>();
            try
            {
                using (var cmd = Con.CreateCommand())
                {
                    //connection.Open();
                    //OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = query;
                    var Ids = cmd.ExecuteReader();
                    //read data and save it on list of integers to add dynamic installation attribute values for each filtered data
                    while (Ids.Read())
                    {
                        InventoryIds.Add(Ids.GetInt32(0));
                    }
                    //Ids.Close();
                    //connection.Close();
                }
                return InventoryIds;
            }
            catch (Exception err)
            {

                InventoryIds.Clear();
                return InventoryIds;
            }
        }
        /// <summary>
        /// //////////////////////////
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="addDependencyViewModel"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        private void FilterLibraryDataAndInsertIt(AddDependencyViewModel addDependencyViewModel, string TableName, OracleConnection Con, int DynamicAttId, Dictionary<string, int> ListValuesIds)
        {
            AddInstRuleViewModel Rule = null;
            TLIattributeActivated AttributeEntity = null;
            string OperationName = null;
            string LogicalOperationName = null;
            AddRowRuleViewModel Last = null;
            string Type = null;
            CultureInfo culture = new CultureInfo("en-US");
            string LibraryTableName = string.Empty;
            //LibraryTableName = LibraryTableName + "Library";
            TLIdynamicAtt dynamicAttEntity = null;
            string dynamicAttDataType = _unitOfWork.DataTypeRepository.GetByID((int)addDependencyViewModel.DataTypeId).Name.ToLower();
            //string TableName = string.Empty;
            Dictionary<string, string> Months = new Dictionary<string, string>();
            Months.Add("1", "JAN");
            Months.Add("2", "FEB");
            Months.Add("3", "MAR");
            Months.Add("4", "APR");
            Months.Add("5", "MAY");
            Months.Add("6", "JUN");
            Months.Add("7", "JUL");
            Months.Add("8", "AUG");
            Months.Add("9", "SEP");
            Months.Add("10", "OCT");
            Months.Add("11", "NOV");
            Months.Add("12", "DEC");
            string query = $"select distinct \"{TableName}\".\"Id\" from \"{TableName}\"" +
                            $"Left join \"TLIdynamicAttLibValue\" on \"TLIdynamicAttLibValue\".\"InventoryId\" = \"{TableName}\".\"Id\"" +
                            $"where ( \"{TableName}\".\"Id\" is not null ) and (";
            string RowRuleLogicalOperation = null;
            foreach (var Dependencie in addDependencyViewModel.Dependencies)
            {
                //test = false;
                //loop on each dependency row
                foreach (var DependencyRow in Dependencie.DependencyRows)
                {
                    //to speperate every condition with () like (TLIsite.SiteCode == 1)
                    query += "(";
                    //select logical operation if not null
                    if (DependencyRow.LogicalOperationId != null)
                    {
                        LogicalOperationName = _unitOfWork.LogicalOperationRepository.GetAllAsQueryable().Where(x => x.Id == DependencyRow.LogicalOperationId).Select(x => x.Name).FirstOrDefault();
                    }
                    Last = DependencyRow.RowRules.Last();
                    //loop on each rowrule 
                    foreach (var RowRule in DependencyRow.RowRules)
                    {
                        Rule = RowRule.Rule;
                        if (Rule.attributeActivatedId != null)
                        {
                            AttributeEntity = _unitOfWork.AttributeActivatedRepository.GetWhereFirst(x => x.Id == Rule.attributeActivatedId);
                        }
                        else if (Rule.dynamicAttId != null)
                        {
                            dynamicAttEntity = _unitOfWork.DynamicAttRepository.GetByID((int)Rule.dynamicAttId);
                            //dynamicAttDataType = _unitOfWork.DataTypeRepository.GetByID((int)dynamicAttEntity.DataTypeId).Name.ToLower();
                        }
                        if (Rule.OperationId != null)
                        {
                            OperationName = _unitOfWork.OperationRepository.GetAllAsQueryable().Where(x => x.Id == Rule.OperationId).Select(x => x.Name).FirstOrDefault();
                            if (OperationName == "==")
                            {
                                OperationName = "=";
                            }
                        }
                        if (RowRule.LogicalOperationId != null)
                        {
                            RowRuleLogicalOperation = _unitOfWork.LogicalOperationRepository.GetAllAsQueryable().Where(x => x.Id == RowRule.LogicalOperationId).Select(x => x.Name).FirstOrDefault();
                        }
                        //check if value is string or not if string check if after the condition there is logical operation or not and build query
                        if (Rule.attributeActivatedId != null && Rule.OperationId != null)
                        {
                            //test = true;
                            if (AttributeEntity.DataType == "string" || AttributeEntity.Key.Contains("Code"))
                            {
                                if (RowRule.LogicalOperationId != null)
                                {
                                    query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} '{Rule.OperationValueString}' {RowRuleLogicalOperation} ";
                                }
                                else
                                {
                                    query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} '{Rule.OperationValueString}'";
                                }
                            }
                            else
                            {
                                //else check if after the condition there is logical operation or not and build query
                                if (RowRule.LogicalOperationId != null)
                                {
                                    if (Rule.OperationValueBoolean != null)
                                    {
                                        string res = string.Empty;
                                        if (Rule.OperationValueBoolean == true)
                                        {
                                            res = "1";
                                        }
                                        else
                                        {
                                            res = "0";
                                        }
                                        query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} {res} {RowRuleLogicalOperation} ";
                                    }
                                    else if (Rule.OperationValueDateTime != null)
                                    {
                                        var test = Rule.OperationValueDateTime.ToString().Split('/');
                                        Months.TryGetValue(test[1], out test[1]);
                                        var res = string.Join('/', test);
                                        query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} '{res}' {RowRuleLogicalOperation} ";
                                    }
                                    else if (Rule.OperationValueDouble != null)
                                    {
                                        query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} {Rule.OperationValueDouble} {RowRuleLogicalOperation} ";
                                    }
                                }
                                else
                                {
                                    if (Rule.OperationValueBoolean != null)
                                    {
                                        string res = string.Empty;
                                        if (Rule.OperationValueBoolean == true)
                                        {
                                            res = "1";
                                        }
                                        else
                                        {
                                            res = "0";
                                        }
                                        query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} {res}";
                                    }
                                    else if (Rule.OperationValueDateTime != null)
                                    {
                                        var test = Rule.OperationValueDateTime.ToString().Split('/');
                                        string month = string.Empty;
                                        string monthValue = test[0];
                                        Months.TryGetValue(monthValue, out month);
                                        test[0] = test[1];
                                        test[1] = month;
                                        var res = string.Join('/', test);
                                        query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} '{res}'";
                                    }
                                    else if (Rule.OperationValueDouble != null)
                                    {
                                        query += $"\"{Rule.TableName}\".\"{AttributeEntity.Key}\" {OperationName} {Rule.OperationValueDouble}";
                                    }
                                }

                            }
                        }
                        else if (Rule.dynamicAttId != null && Rule.OperationId != null)
                        {
                            if (RowRule.LogicalOperationId != null)
                            {
                                if (dynamicAttDataType == "string")
                                {
                                    query += $"\"{Rule.TableName}\".\"{"ValueString"}\" {OperationName} '{Rule.OperationValueString}' {RowRuleLogicalOperation} ";
                                }
                                else if (dynamicAttDataType == "double")
                                {
                                    query += $"\"{Rule.TableName}\".\"{"ValueDouble"}\" {OperationName} {Rule.OperationValueDouble} {RowRuleLogicalOperation} ";
                                }
                                else if (dynamicAttDataType == "datetime")
                                {
                                    var test = Rule.OperationValueDateTime.ToString().Split('/');
                                    string month = string.Empty;
                                    string monthValue = test[0];
                                    Months.TryGetValue(monthValue, out month);
                                    test[0] = test[1];
                                    test[1] = month;
                                    var res = string.Join('/', test);
                                    query += $"\"{Rule.TableName}\".\"{"ValueDateTime"}\" {OperationName} '{res}' {RowRuleLogicalOperation} ";
                                }
                                else if (dynamicAttDataType == "boolean")
                                {
                                    string res = string.Empty;
                                    if (Rule.OperationValueBoolean == true)
                                    {
                                        res = "1";
                                    }
                                    else
                                    {
                                        res = "0";
                                    }
                                    query += $"\"{Rule.TableName}\".\"{"ValueBoolean"}\" {OperationName} {res} {RowRuleLogicalOperation} ";
                                }
                            }
                            else
                            {
                                if (dynamicAttDataType == "string")
                                {
                                    query += $"\"{Rule.TableName}\".\"{"ValueString"}\" {OperationName} '{Rule.OperationValueString}'";
                                }
                                else if (dynamicAttDataType == "double")
                                {
                                    query += $"\"{Rule.TableName}\".\"{"ValueDouble"}\" {OperationName} {Rule.OperationValueDouble}";
                                }
                                else if (dynamicAttDataType == "datetime")
                                {
                                    var test = Rule.OperationValueDateTime.ToString().Split('/');
                                    string month = string.Empty;
                                    string monthValue = test[0];
                                    Months.TryGetValue(monthValue, out month);
                                    test[0] = test[1];
                                    test[1] = month;
                                    var res = string.Join('/', test);
                                    query += $"\"{Rule.TableName}\".\"{"ValueDateTime"}\" {OperationName} '{res}'";
                                }
                                else if (dynamicAttDataType == "boolean")
                                {
                                    string res = string.Empty;
                                    if (Rule.OperationValueBoolean == true)
                                    {
                                        res = "1";
                                    }
                                    else
                                    {
                                        res = "0";
                                    }
                                    query += $"\"{Rule.TableName}\".\"{"ValueBoolean"}\" {OperationName} {res}";
                                }
                            }
                        }
                    }
                    query += ")";
                    //Add logical operation
                    if (DependencyRow.LogicalOperationId != null)
                    {
                        query += $" {LogicalOperationName} ";
                    }
                }
                query += ")";
                //fetch data from database depened on last query
                var InventoryIds = fetchdata(query, Con);
                if (InventoryIds.Count > 0)
                {
                    using (var cmd = Con.CreateCommand())
                    {
                        //connection.Open();
                        List<string> StringValues = new List<string>();
                        List<double> DoubleValues = new List<double>();
                        List<DateTime> DateTimeValues = new List<DateTime>();
                        List<int> BooleanValues = new List<int>();
                        List<int> DynamicIds = new List<int>();
                        List<int> tablesNamesIds = new List<int>();
                        List<int?> dynamicListValuesIds = new List<int?>();
                        for (int i = 0; i < InventoryIds.Count; i++)
                        {
                            if (dynamicAttDataType != "list")
                            {
                                if (dynamicAttDataType == "string")
                                {
                                    StringValues.Add(Dependencie.ValueString);
                                }
                                else if (dynamicAttDataType == "double")
                                {
                                    DoubleValues.Add((double)Dependencie.ValueDouble);
                                }
                                else if (dynamicAttDataType == "datetime")
                                {

                                    DateTimeValues.Add(Convert.ToDateTime(Dependencie.ValueDateTime));
                                }
                                else if (dynamicAttDataType == "boolean")
                                {
                                    int res = 0;
                                    if (Dependencie.ValueBoolean == true)
                                    {
                                        res = 1;
                                    }
                                    BooleanValues.Add(res);
                                }
                            }
                            //else
                            //{
                            //    int ValueId = 0;
                            //    ListValuesIds.TryGetValue(Dependencie.ValueList, out ValueId);
                            //    dynamicListValuesIds.Add(ValueId);
                            //}
                            DynamicIds.Add(DynamicAttId);
                            tablesNamesIds.Add(addDependencyViewModel.tablesNamesId);
                        }
                        OracleParameter Value = new OracleParameter();
                        if (dynamicAttDataType == "string")
                        {
                            Value.OracleDbType = OracleDbType.NVarchar2;
                            Value.Value = StringValues.ToArray();
                        }
                        else if (dynamicAttDataType == "double")
                        {
                            Value.OracleDbType = OracleDbType.BinaryDouble;
                            Value.Value = DoubleValues.ToArray();
                        }
                        else if (dynamicAttDataType == "datetime")
                        {
                            Value.OracleDbType = OracleDbType.TimeStamp;
                            Value.Value = DateTimeValues.ToArray();
                        }
                        else if (dynamicAttDataType == "boolean")
                        {
                            Value.OracleDbType = OracleDbType.Int16;
                            Value.Value = BooleanValues.ToArray();
                        }

                        OracleParameter DynamicId = new OracleParameter();
                        DynamicId.OracleDbType = OracleDbType.Int32;
                        DynamicId.Value = DynamicIds.ToArray();
                        OracleParameter tablesNamesId = new OracleParameter();
                        tablesNamesId.OracleDbType = OracleDbType.Int32;
                        tablesNamesId.Value = tablesNamesIds.ToArray();
                        OracleParameter Ids = new OracleParameter();
                        Ids.OracleDbType = OracleDbType.Int32;
                        Ids.Value = InventoryIds.ToArray();
                        OracleParameter dynamicListValuesId = new OracleParameter();
                        dynamicListValuesId.OracleDbType = OracleDbType.Int32;
                        dynamicListValuesId.Value = dynamicListValuesIds.ToArray();
                        //OracleCommand cmd = connection.CreateCommand();
                        if (dynamicAttDataType != "list")
                        {
                            if (dynamicAttDataType == "string")
                            {
                                cmd.CommandText = $"INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"tablesNamesId\", \"InventoryId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                            }
                            else if (dynamicAttDataType == "double")
                            {
                                cmd.CommandText = $"INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"tablesNamesId\", \"InventoryId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                            }
                            else if (dynamicAttDataType == "datetime")
                            {
                                cmd.CommandText = $"INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"tablesNamesId\", \"InventoryId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                            }
                            else if (dynamicAttDataType == "boolean")
                            {
                                cmd.CommandText = $"INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"tablesNamesId\", \"InventoryId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                            }
                            cmd.ArrayBindCount = InventoryIds.Count;
                            cmd.Parameters.Add(DynamicId);
                            cmd.Parameters.Add(tablesNamesId);
                            cmd.Parameters.Add(Ids);
                            //cmd.Parameters.Add(dynamicListValuesId);
                            cmd.Parameters.Add(Value);
                            //cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd.CommandText = $"INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"tablesNamesId\", \"InventoryId\", \"dynamicListValuesId\") VALUES ( :1, :2, :3, :4)";
                            cmd.ArrayBindCount = InventoryIds.Count;
                            cmd.Parameters.Add(DynamicId);
                            cmd.Parameters.Add(tablesNamesId);
                            cmd.Parameters.Add(Ids);
                            cmd.Parameters.Add(dynamicListValuesId);
                            //cmd.ExecuteNonQuery();
                        }
                        cmd.ExecuteNonQuery();

                        //connection.Close();
                    }
                }
            }
        }
        #region Add History
        public void AddHistoryForDynamic(int DynamicId, string historyType, int TableNameId)
        {

            AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
            history.RecordId = DynamicId;
            history.TablesNameId = TableNameId;
            history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
            history.UserId = 83;
            history.Date = DateTime.Now;
            _unitOfWork.TablesHistoryRepository.AddTableHistory(history);

        }
        #endregion
        #region AddHistoryForEdit
        public int AddHistoryForEdit(int RecordId, int TableNameid, string HistoryType, List<TLIhistoryDetails> details)
        {
            AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
            history.RecordId = RecordId;
            history.TablesNameId = TableNameid;//_unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.Id == TableNameid).Id;
            history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == HistoryType, x => new { x.Id }).Id;
            history.UserId = 83;
            int? TableHistoryId = null;
            var CheckTableHistory = _unitOfWork.TablesHistoryRepository.GetWhereFirst(x => x.HistoryType.Name == HistoryType && x.RecordId == RecordId && x.TablesNameId == TableNameid);
            if (CheckTableHistory != null)
            {
                var TableHistory = _unitOfWork.TablesHistoryRepository.GetWhereAndSelect(x => x.HistoryType.Name == HistoryType && x.RecordId == RecordId && x.TablesNameId == TableNameid, x => new { x.Id }).ToList().Max(x => x.Id);
                if (TableHistory != null)
                    TableHistoryId = TableHistory;
                if (TableHistoryId != null)
                {
                    history.PreviousHistoryId = TableHistoryId;
                }
            }

            int HistoryId = _unitOfWork.TablesHistoryRepository.AddTableHistory(history, details);
            _unitOfWork.SaveChangesAsync();
            return HistoryId;
        }

        #endregion
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
                        // _unitOfWork.HistoryDetailsRepository.Add(historyDetails);
                        // _unitOfWork.SaveChanges();
                        //property.SetValue(originalObj, updateObj.GetType().GetProperty(property.Name)
                        //.GetValue(originalObj, null));
                    }
                }



            }
            return result;
        }
        public Response<DynamicAttViewModel> Disable(int RecordId)
        {
            try
            {
                var DynamicAtt = _unitOfWork.DynamicAttRepository.GetByID(RecordId);
                DynamicAtt.disable = !(DynamicAtt.disable);

                if (DynamicAtt.disable)
                    DynamicAtt.Required = false;

                UnitOfWork.AllAttributeViewManagment.FirstOrDefault(x => x.DynamicAttId != null ?
                    x.DynamicAttId == RecordId : false)
                    .DynamicAtt = DynamicAtt;
                UnitOfWork.AllDynamicAttribute.FirstOrDefault(x => x.Id == RecordId).disable = DynamicAtt.disable;
                UnitOfWork.AllDynamicAttribute.FirstOrDefault(x => x.Id == RecordId).Required = DynamicAtt.Required;

                _unitOfWork.SaveChanges();
                return new Response<DynamicAttViewModel>();
            }
            catch (Exception err)
            {
                return new Response<DynamicAttViewModel>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        public Response<DynamicAttViewModel> RequiredNOTRequired(int DynamicAttId)
        {
            try
            {
                TLIdynamicAtt DynamicAtt = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttId);

                DynamicAtt.Required = !(DynamicAtt.Required);

                UnitOfWork.AllAttributeViewManagment.FirstOrDefault(x => x.DynamicAttId != null ?
                    x.DynamicAttId == DynamicAttId : false).DynamicAtt = DynamicAtt;
                UnitOfWork.AllDynamicAttribute.FirstOrDefault(x => x.Id == DynamicAttId)
                    .Required = DynamicAtt.Required;

                _unitOfWork.SaveChanges();

                return new Response<DynamicAttViewModel>();
            }
            catch (Exception err)
            {
                return new Response<DynamicAttViewModel>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<OutPutString>> GetLayers(string TableName)
        {
            try
            {
                List<OutPutString> OutPutString = new List<OutPutString>();

                if (TableName.ToLower() == TablesNames.TLIcivilWithLegs.ToString().ToLower() ||
                    TableName.ToLower() == TablesNames.TLIcivilWithoutLeg.ToString().ToLower() ||
                    TableName.ToLower() == TablesNames.TLIcivilNonSteel.ToString().ToLower())
                {
                    OutPutString.AddRange(new List<OutPutString>
                    {
                        new OutPutString
                        {
                            Name = Constants.Layers.Site.ToString()
                        },
                        new OutPutString
                        {
                            Name = "C" + TableName.Split("TLIc")[1]
                        }
                    });
                    return new Response<List<OutPutString>>(true, OutPutString, null, null, (int)Constants.ApiReturnCode.success);
                }
                else if (TableName.ToLower() == TablesNames.TLIsideArm.ToString().ToLower())
                {
                    OutPutString.AddRange(new List<OutPutString>
                    {
                        new OutPutString
                        {
                            Name = Constants.Layers.Site.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.Civil.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.SideArm.ToString()
                        }
                    });
                    return new Response<List<OutPutString>>(true, OutPutString, null, null, (int)Constants.ApiReturnCode.success);
                }
                else if (TableName.ToLower() == TablesNames.TLIcabinet.ToString().ToLower())
                {
                    OutPutString.AddRange(new List<OutPutString>
                    {
                        new OutPutString
                        {
                            Name = Constants.Layers.Site.ToString()
                        },
                        new OutPutString
                        {
                            Name = "Cabinet"
                        }
                    });
                    return new Response<List<OutPutString>>(true, OutPutString, null, null, (int)Constants.ApiReturnCode.success);
                }
                else if (TableName.ToLower() == TablesNames.TLIsolar.ToString().ToLower())
                {
                    OutPutString.AddRange(new List<OutPutString>
                    {
                        new OutPutString
                        {
                            Name = Constants.Layers.Site.ToString()
                        },
                        new OutPutString
                        {
                            Name = "Solar"
                        }
                    });
                    return new Response<List<OutPutString>>(true, OutPutString, null, null, (int)Constants.ApiReturnCode.success);
                }
                else if (TableName.ToLower() == TablesNames.TLIgenerator.ToString().ToLower())
                {
                    OutPutString.AddRange(new List<OutPutString>
                    {
                        new OutPutString
                        {
                            Name = Constants.Layers.Site.ToString()
                        },
                        new OutPutString
                        {
                            Name = "Generator"
                        }
                    });
                    return new Response<List<OutPutString>>(true, OutPutString, null, null, (int)Constants.ApiReturnCode.success);
                }
                else if (TableName.ToLower() == TablesNames.TLImwBU.ToString().ToLower() ||
                         TableName.ToLower() == TablesNames.TLImwODU.ToString().ToLower() ||
                         TableName.ToLower() == TablesNames.TLImwDish.ToString().ToLower() ||
                         TableName.ToLower() == TablesNames.TLImwOther.ToString().ToLower())
                {
                    OutPutString.AddRange(new List<OutPutString>
                    {
                        new OutPutString
                        {
                            Name = Constants.Layers.Site.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.Civil.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.SideArm.ToString()
                        },
                        new OutPutString
                        {
                            Name = "MW_" + char.ToUpper(TableName.Split("TLImw")[1][0]) + TableName.Split("TLImw")[1].Substring(1)
                        }
                    });
                    return new Response<List<OutPutString>>(true, OutPutString, null, null, (int)Constants.ApiReturnCode.success);
                }
                else if (TableName.ToLower() == TablesNames.TLImwRFU.ToString().ToLower())
                {
                    OutPutString.AddRange(new List<OutPutString>
                    {
                        new OutPutString
                        {
                            Name = Constants.Layers.Site.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.Civil.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.MW_BU.ToString()
                        },
                        new OutPutString
                        {
                            Name = "MW_" + char.ToUpper(TableName.Split("TLImw")[1][0]) + TableName.Split("TLImw")[1].Substring(1)
                        }
                    });
                    return new Response<List<OutPutString>>(true, OutPutString, null, null, (int)Constants.ApiReturnCode.success);
                }
                else if (TableName.ToLower() == TablesNames.TLIradioAntenna.ToString().ToLower() ||
                         TableName.ToLower() == TablesNames.TLIradioRRU.ToString().ToLower() ||
                         TableName.ToLower() == TablesNames.TLIradioOther.ToString().ToLower())
                {
                    OutPutString.AddRange(new List<OutPutString>
                    {
                        new OutPutString
                        {
                            Name = Constants.Layers.Site.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.Civil.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.SideArm.ToString()
                        },
                        new OutPutString
                        {
                            Name = "R" + TableName.Split("TLIr")[1]
                        }
                    });
                    return new Response<List<OutPutString>>(true, OutPutString, null, null, (int)Constants.ApiReturnCode.success);
                }
                else if (TableName.ToLower() == TablesNames.TLIpower.ToString().ToLower())
                {
                    OutPutString.AddRange(new List<OutPutString>
                    {
                        new OutPutString
                        {
                            Name = Constants.Layers.Site.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.Civil.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.SideArm.ToString()
                        },
                        new OutPutString
                        {
                            Name = "Power"
                        }
                    });
                    return new Response<List<OutPutString>>(true, OutPutString, null, null, (int)Constants.ApiReturnCode.success);
                }
                else if (TableName.ToLower() == TablesNames.TLIloadOther.ToString().ToLower())
                {
                    OutPutString.AddRange(new List<OutPutString>
                    {
                        new OutPutString
                        {
                            Name = Constants.Layers.Site.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.Civil.ToString()
                        },
                        new OutPutString
                        {
                            Name = Constants.Layers.SideArm.ToString()
                        },
                        new OutPutString
                        {
                            Name = "LoadOther"
                        }
                    });
                    return new Response<List<OutPutString>>(true, OutPutString, null, null, (int)Constants.ApiReturnCode.success);
                }
                return new Response<List<OutPutString>>(true, null, null, null, (int)Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<OutPutString>>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }

        public Response<FirstStepAddDependencyViewModel> GetForAddingDynamicAttribute(string TableName)
        {
            try
            {
                FirstStepAddDependencyViewModel OutPut = new FirstStepAddDependencyViewModel
                {
                    TableNameId = _unitOfWork.TablesNamesRepository
                        .GetWhereFirst(x => x.TableName.ToLower() == TableName.ToLower()).Id,

                    DataTypes = _mapper.Map<List<DataTypeViewModel>>(_unitOfWork.DataTypeRepository
                        .GetWhere(x => !x.Deleted && !x.Disable).ToList()),

                    LogicalOperations = _mapper.Map<List<LogicalOperationViewModel>>(_unitOfWork.LogicalOperationRepository
                        .GetWhere(x => !x.Deleted && !x.Disable).ToList()),

                    Operations = _mapper.Map<List<OperationViewModel>>(_unitOfWork.OperationRepository
                        .GetWhere(x => !x.Deleted && !x.Disable).ToList())
                };

                return new Response<FirstStepAddDependencyViewModel>(true, OutPut, null, null, (int)Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<FirstStepAddDependencyViewModel>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        public Response<DynamicAttViewModel> CheckEditingDynamicAttDataType(int DynamicAttributeId, int NewDataTypeId)
        {
            TLIdynamicAtt DynamicAttribute = _unitOfWork.DynamicAttRepository
                .GetIncludeWhereFirst(x => x.Id == DynamicAttributeId, x => x.DataType);

            if (DynamicAttribute.DataTypeId == NewDataTypeId)
                return new Response<DynamicAttViewModel>(true, null, null, null, (int)Constants.ApiReturnCode.success);

            TLIdataType NewDataType = _unitOfWork.DataTypeRepository.GetByID(NewDataTypeId);

            try
            {
                if (!string.IsNullOrEmpty(DynamicAttribute.DefaultValue) &&
                    !string.IsNullOrWhiteSpace(DynamicAttribute.DefaultValue))
                {
                    if (NewDataType.Name.ToLower() == "double")
                    {
                        var obj = Convert.ChangeType(DynamicAttribute.DefaultValue, typeof(double));
                    }
                    else if (NewDataType.Name.ToLower() == "boolean")
                    {
                        var obj = Convert.ChangeType(DynamicAttribute.DefaultValue, typeof(bool));
                    }
                    else if (NewDataType.Name.ToLower() == "datetime")
                    {
                        var obj = Convert.ChangeType(DynamicAttribute.DefaultValue, typeof(DateTime));
                    }
                }
            }
            catch (FormatException)
            {
                return new Response<DynamicAttViewModel>(true, null, null,
                    $"Can't Change The Data Type Of This Dynamic Attribute ({DynamicAttribute.Key}) From ({DynamicAttribute.DataType.Name}) To ({NewDataType.Name}) " +
                    $"Because It Has Values That Can't Be Converted",
                    (int)Constants.ApiReturnCode.fail);
            }

            TLIdependency DependecnyValidation = _unitOfWork.DependencieRepository
                .GetWhereFirst(x => x.DynamicAttId == DynamicAttributeId);

            if (DependecnyValidation != null)
                return new Response<DynamicAttViewModel>(true, null, null,
                    $"Can't Change The Data Type Of This Dynamic Attribute ({DynamicAttribute.Key}) From ({DynamicAttribute.DataType.Name}) To ({NewDataType.Name}) " +
                    $"Because it has Dependency Validation",
                    (int)Constants.ApiReturnCode.fail);

            TLIvalidation GeneralValidation = _unitOfWork.ValidationRepository
                .GetWhereFirst(x => x.DynamicAttId == DynamicAttributeId);

            if (GeneralValidation != null)
                return new Response<DynamicAttViewModel>(true, null, null,
                    $"Can't Change The Data Type Of This Dynamic Attribute ({DynamicAttribute.Key}) From ({DynamicAttribute.DataType.Name}) To ({NewDataType.Name}) " +
                    $"Because it has Validation",
                    (int)Constants.ApiReturnCode.fail);

            TLIrule Rule = _unitOfWork.RuleRepository
                .GetWhereFirst(x => x.dynamicAttId == DynamicAttributeId);

            if (Rule != null)
                return new Response<DynamicAttViewModel>(true, null, null,
                    $"Can't Change The Data Type Of This Dynamic Attribute ({DynamicAttribute.Key}) From ({DynamicAttribute.DataType.Name}) To ({NewDataType.Name}) " +
                    $"Because is's Used in Another Dependency",
                    (int)Constants.ApiReturnCode.fail);

            if (NewDataType.Name.ToLower() == "string".ToLower())
                return new Response<DynamicAttViewModel>(true, null, null, null, (int)Constants.ApiReturnCode.success);

            if (DynamicAttribute.DataType.Name.ToLower() == "string".ToLower())
            {
                if (DynamicAttribute.LibraryAtt)
                {
                    List<TLIdynamicAttLibValue> DynamicAttributeLibValues = _unitOfWork.DynamicAttLibRepository
                        .GetWhere(x => x.DynamicAttId == DynamicAttributeId).ToList();

                    try
                    {
                        if (NewDataType.Name.ToLower() == "double".ToLower())
                        {
                            foreach (TLIdynamicAttLibValue DynamicAttributeLibValue in DynamicAttributeLibValues)
                            {
                                if (!string.IsNullOrEmpty(DynamicAttributeLibValue.ValueString) &&
                                    !string.IsNullOrWhiteSpace(DynamicAttributeLibValue.ValueString))
                                {
                                    var obj = Convert.ChangeType(DynamicAttributeLibValue.ValueString, typeof(double));
                                }
                            }
                        }
                        else if (NewDataType.Name.ToLower() == "boolean".ToLower())
                        {
                            foreach (TLIdynamicAttLibValue DynamicAttributeLibValue in DynamicAttributeLibValues)
                            {
                                if (!string.IsNullOrEmpty(DynamicAttributeLibValue.ValueString) &&
                                    !string.IsNullOrWhiteSpace(DynamicAttributeLibValue.ValueString))
                                {
                                    var obj = Convert.ChangeType(DynamicAttributeLibValue.ValueString, typeof(bool));
                                }
                            }
                        }
                        else if (NewDataType.Name.ToLower() == "datetime".ToLower())
                        {
                            foreach (TLIdynamicAttLibValue DynamicAttributeLibValue in DynamicAttributeLibValues)
                            {
                                if (!string.IsNullOrEmpty(DynamicAttributeLibValue.ValueString) &&
                                    !string.IsNullOrWhiteSpace(DynamicAttributeLibValue.ValueString))
                                {
                                    var obj = Convert.ChangeType(DynamicAttributeLibValue.ValueString, typeof(DateTime));
                                }
                            }
                        }
                    }
                    catch (InvalidCastException)
                    {
                        return new Response<DynamicAttViewModel>(true, null, null,
                            $"Can't Change The Data Type Of This Dynamic Attribute ({DynamicAttribute.Key}) From ({DynamicAttribute.DataType.Name}) To ({NewDataType.Name}) " +
                            $"Because It Has Values That Has Nullable Values",
                            (int)Constants.ApiReturnCode.fail);
                    }
                    catch (FormatException)
                    {
                        return new Response<DynamicAttViewModel>(true, null, null,
                            $"Can't Change The Data Type Of This Dynamic Attribute ({DynamicAttribute.Key}) From ({DynamicAttribute.DataType.Name}) To ({NewDataType.Name}) " +
                            $"Because It Has Values That Can't Be Converted",
                            (int)Constants.ApiReturnCode.fail);
                    }
                }
                else
                {
                    List<TLIdynamicAttInstValue> DynamicAttributeLibValues = _unitOfWork.DynamicAttInstValueRepository
                        .GetWhere(x => x.DynamicAttId == DynamicAttributeId).ToList();

                    try
                    {
                        if (NewDataType.Name.ToLower() == "double".ToLower())
                        {
                            foreach (TLIdynamicAttInstValue DynamicAttributeLibValue in DynamicAttributeLibValues)
                            {
                                if (!string.IsNullOrEmpty(DynamicAttributeLibValue.ValueString) &&
                                    !string.IsNullOrWhiteSpace(DynamicAttributeLibValue.ValueString))
                                {
                                    var obj = Convert.ChangeType(DynamicAttributeLibValue.ValueString, typeof(double));
                                }
                            }
                        }
                        else if (NewDataType.Name.ToLower() == "boolean".ToLower())
                        {
                            foreach (TLIdynamicAttInstValue DynamicAttributeLibValue in DynamicAttributeLibValues)
                            {
                                if (!string.IsNullOrEmpty(DynamicAttributeLibValue.ValueString) &&
                                    !string.IsNullOrWhiteSpace(DynamicAttributeLibValue.ValueString))
                                {
                                    var obj = Convert.ChangeType(DynamicAttributeLibValue.ValueString, typeof(bool));
                                }
                            }
                        }
                        else if (NewDataType.Name.ToLower() == "datetime".ToLower())
                        {
                            foreach (TLIdynamicAttInstValue DynamicAttributeLibValue in DynamicAttributeLibValues)
                            {
                                if (!string.IsNullOrEmpty(DynamicAttributeLibValue.ValueString) &&
                                    !string.IsNullOrWhiteSpace(DynamicAttributeLibValue.ValueString))
                                {
                                    var obj = Convert.ChangeType(DynamicAttributeLibValue.ValueString, typeof(DateTime));
                                }
                            }
                        }
                    }
                    catch (InvalidCastException)
                    {
                        return new Response<DynamicAttViewModel>(true, null, null,
                            $"Can't Change The Data Type Of This Dynamic Attribute ({DynamicAttribute.Key}) From ({DynamicAttribute.DataType.Name}) To ({NewDataType.Name}) " +
                            $"Because It Has Values That Has Nullable Values",
                            (int)Constants.ApiReturnCode.fail);
                    }
                    catch (FormatException)
                    {
                        return new Response<DynamicAttViewModel>(true, null, null,
                            $"Can't Change The Data Type Of This Dynamic Attribute ({DynamicAttribute.Key}) From ({DynamicAttribute.DataType.Name}) To ({NewDataType.Name}) " +
                            $"Because It Has Values That Can't Be Converted",
                            (int)Constants.ApiReturnCode.fail);
                    }
                }
            }

            else
                return new Response<DynamicAttViewModel>(true, null, null,
                    $"Can't Change The Data Type Of This Dynamic Attribute ({DynamicAttribute.Key}) From ({DynamicAttribute.DataType.Name}) To ({NewDataType.Name})",
                    (int)Constants.ApiReturnCode.fail);

            return new Response<DynamicAttViewModel>(true, null, null, null, (int)Constants.ApiReturnCode.success);
        }
    }
}
