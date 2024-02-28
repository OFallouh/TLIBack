using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class ValidationRepository : RepositoryBase<TLIvalidation, ValidationViewModel, int>, IValidationRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;
        public ValidationRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool CheckDependencyValidation(object AddViewModel, TLIdependency validation, string TableName, object DynamicAttValue, string DynamicAttName, string SiteCode, OracleConnection con, out string ErrorMessage)
        {
            ErrorMessage = string.Empty;
            try
            {
                object result = null;
                bool ValidationResult = true;
                string DictioneryValue = string.Empty;
                var ValidationOperation = _context.TLIoperation.Find(validation.OperationId);
                var TableNameId = _context.TLItablesNames.Where(x => x.TableName == TableName).FirstOrDefault().Id;
                if (TableName == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIowner", "OwnerId");
                    ForeignKeys.Add("TLIcivilWithLegLibrary", "CivilWithLegsLibId");
                    ForeignKeys.Add("TLIbaseCivilWithLegsType", "BaseCivilWithLegTypeId");
                    ForeignKeys.Add("TLIguyLineType", "GuylineTypeid");
                    ForeignKeys.Add("TLIsupportTypeImplemented", "SupportTypeImplementedId");
                    ForeignKeys.Add("TLIenforcmentCategory", "enforcmentCategoryId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddCivilWithLegsViewModel addCivilWithLegs = _mapper.Map<AddCivilWithLegsViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIcivilWithLegs>(addCivilWithLegs);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode")
                                {
                                    var Entity = _mapper.Map<TLIcivilWithLegs>(addCivilWithLegs);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if(Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if(Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDdynamicAttributeInstallationValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addCivilWithLegs.dynamicAttribute.Where(x => x.id == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {

                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.value is bool booleanValue)
                                        {
                                            if (CheckValue(booleanValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.value is DateTime dateTimeValue)
                                        {
                                            if (CheckValue(dateTimeValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.value is double doubleValue)
                                        {
                                            if (CheckValue(doubleValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.value is string stringValue)
                                        {
                                            if (CheckValue(stringValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if(Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addCivilWithLegs.civilType.civilWithLegsLibId).FirstOrDefault();
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                            ValidationResult = false;
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                            ValidationResult = false;
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                            ValidationResult = false;
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                            ValidationResult = false;
                        }
                    }
                }
                else if (TableName == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIowner", "OwnerId");
                    ForeignKeys.Add("TLIcivilWithoutLegLibrary", "CivilWithoutlegsLibId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddCivilWithoutLegViewModel addCivilWithoutLeg = _mapper.Map<AddCivilWithoutLegViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIcivilWithoutLeg>(addCivilWithoutLeg);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode")
                                {
                                    var Entity = _mapper.Map<TLIcivilWithoutLeg>(addCivilWithoutLeg);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addCivilWithoutLeg.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addCivilWithoutLeg.CivilWithoutlegsLibId).FirstOrDefault();
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIowner", "OwnerId");
                    ForeignKeys.Add("TLIcivilNonSteelLibrary", "CivilNonSteelLibraryId");
                    ForeignKeys.Add("TLIsupportTypeImplemented", "supportTypeImplementedId");
                    ForeignKeys.Add("TLIlocationType", "locationTypeId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddCivilNonSteelViewModel addCivilNonSteel = _mapper.Map<AddCivilNonSteelViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIcivilNonSteel>(addCivilNonSteel);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode")
                                {
                                    var Entity = _mapper.Map<TLIcivilNonSteel>(addCivilNonSteel);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addCivilNonSteel.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addCivilNonSteel.CivilNonSteelLibraryId).FirstOrDefault();
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }

                }
                else if (TableName == Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIsupportTypeDesigned", "supportTypeDesignedId");
                    ForeignKeys.Add("TLIsectionsLegType", "sectionsLegTypeId");
                    ForeignKeys.Add("TLIstructureType", "structureTypeId");
                    ForeignKeys.Add("TLIcivilSteelSupportCategory", "civilSteelSupportCategoryId");
                    var Operations = _context.TLIoperation.ToList();
                    AddCivilWithLegLibraryViewModel addCivilWithLegLibrary = _mapper.Map<AddCivilWithLegLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIcivilWithLegLibrary>(addCivilWithLegLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false)
                                {
                                    var Entity = _mapper.Map<TLIcivilWithLegLibrary>(addCivilWithLegLibrary);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addCivilWithLegLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIcivilSteelSupportCategory", "CivilSteelSupportCategoryId");
                    ForeignKeys.Add("TLIinstallationCivilwithoutLegsType", "InstallationCivilwithoutLegsTypeId");
                    ForeignKeys.Add("TLIcivilWithoutLegCategory", "CivilWithoutLegCategoryId");
                    var Operations = _context.TLIoperation.ToList();
                    AddCivilWithoutLegLibraryViewModel addCivilWithoutLegLibrary = _mapper.Map<AddCivilWithoutLegLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIcivilWithoutLegLibrary>(addCivilWithoutLegLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false)
                                {
                                    var Entity = _mapper.Map<TLIcivilWithoutLegLibrary>(addCivilWithoutLegLibrary);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addCivilWithoutLegLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIcivilNonSteelType", "civilNonSteelTypeId");
                    var Operations = _context.TLIoperation.ToList();
                    AddCivilNonSteelLibraryViewModel addCivilNonSteelLibrary = _mapper.Map<AddCivilNonSteelLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIcivilNonSteelLibrary>(addCivilNonSteelLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false)
                                {
                                    var Entity = _mapper.Map<TLIcivilNonSteelLibrary>(addCivilNonSteelLibrary);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addCivilNonSteelLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.OtherInventoryType.TLIcabinet.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIcabinetPowerLibrary", "CabinetPowerLibraryId");
                    ForeignKeys.Add("TLIcabinetTelecomLibrary", "CabinetTelecomLibraryId");
                    ForeignKeys.Add("TLIrenewableCabinetType", "RenewableCabinetTypeId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddCabinetViewModel addCabinet = _mapper.Map<AddCabinetViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIcabinet>(addCabinet);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode")
                                {
                                    var Entity = _mapper.Map<TLIcabinet>(addCabinet);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addCabinet.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if(TableNameEntity.TableName == Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString())
                                    {
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addCabinet.CabinetPowerLibraryId).FirstOrDefault();
                                    }
                                    else if(TableNameEntity.TableName == Helpers.Constants.OtherInventoryType.TLIcabinetTelecomLibrary.ToString())
                                    {
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addCabinet.CabinetTelecomLibraryId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.OtherInventoryType.TLIgenerator.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIbaseGeneratorType", "BaseGeneratorTypeId");
                    ForeignKeys.Add("TLIgeneratorLibrary", "GeneratorLibraryId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddGeneratorViewModel addGenerator = _mapper.Map<AddGeneratorViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIgenerator>(addGenerator);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode")
                                {
                                    var Entity = _mapper.Map<TLIgenerator>(addGenerator);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addGenerator.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addGenerator.GeneratorLibraryId).FirstOrDefault();
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.OtherInventoryType.TLIsolar.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIsolarLibrary", "SolarLibraryId");
                    ForeignKeys.Add("TLIcabinet", "CabinetId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddSolarViewModel addSolar = _mapper.Map<AddSolarViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIsolar>(addSolar);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode")
                                {
                                    var Entity = _mapper.Map<TLIsolar>(addSolar);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addSolar.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addSolar.SolarLibraryId).FirstOrDefault();
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIcabinetPowerType", "CabinetPowerTypeId");
                    var Operations = _context.TLIoperation.ToList();
                    AddCabinetPowerLibraryViewModel addCabinetPowerLibrary = _mapper.Map<AddCabinetPowerLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIcabinetPowerLibrary>(addCabinetPowerLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false)
                                {
                                    var Entity = _mapper.Map<TLIcabinetPowerLibrary>(addCabinetPowerLibrary);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addCabinetPowerLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.OtherInventoryType.TLIcabinetTelecomLibrary.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLItelecomType", "TelecomTypeId");
                    var Operations = _context.TLIoperation.ToList();
                    AddCabinetTelecomLibraryViewModel addCabinetTelecomLibrary = _mapper.Map<AddCabinetTelecomLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIcabinetTelecomLibrary>(addCabinetTelecomLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false)
                                {
                                    var Entity = _mapper.Map<TLIcabinetTelecomLibrary>(addCabinetTelecomLibrary);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addCabinetTelecomLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIcapacity", "CapacityId");
                    var Operations = _context.TLIoperation.ToList();
                    AddGeneratorLibraryViewModel addGeneratorLibrary = _mapper.Map<AddGeneratorLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIgeneratorLibrary>(addGeneratorLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false)
                                {
                                    var Entity = _mapper.Map<TLIgeneratorLibrary>(addGeneratorLibrary);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addGeneratorLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIcapacity", "CapacityId");
                    var Operations = _context.TLIoperation.ToList();
                    AddSolarLibraryViewModel addSolarLibrary = _mapper.Map<AddSolarLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIsolarLibrary>(addSolarLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false)
                                {
                                    var Entity = _mapper.Map<TLIsolarLibrary>(addSolarLibrary);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addSolarLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLIloadOther.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIloadOtherLibrary", "loadOtherLibraryId");
                    ForeignKeys.Add("TLIsideArm", "sideArmId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddLoadOtherViewModel addLoadOther = _mapper.Map<AddLoadOtherViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIloadOther>(addLoadOther);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode" && DictioneryValue != "sideArmId")
                                {
                                    var Entity = _mapper.Map<TLIloadOther>(addLoadOther);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    if(string.IsNullOrEmpty(SiteCode) == false)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "sideArmId")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Id = addLoadOther.TLIcivilLoads.sideArmId;
                                    if(Id != null)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {Id}";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if(Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    var Id = addLoadOther.TLIcivilLoads.allCivilInstId;
                                    if(Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithLegsId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithLegsId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    var Id = addLoadOther.TLIcivilLoads.allCivilInstId;
                                    if(Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithoutLegId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithoutLegId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    var Id = addLoadOther.TLIcivilLoads.allCivilInstId;
                                    if(Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilNonSteelId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilNonSteelId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addLoadOther.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.LoadSubType.TLIloadOtherLibrary.ToString())
                                    {
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addLoadOther.loadOtherLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString())
                                    {
                                        var SideArmEntity = _context.TLIsideArm.Where(x => x.Id == addLoadOther.TLIcivilLoads.sideArmId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == SideArmEntity.sideArmLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addLoadOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithLegEntity = _context.TLIcivilWithLegs.Where(x => x.Id == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithLegEntity.CivilWithLegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addLoadOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithoutLegEntity = _context.TLIcivilWithoutLeg.Where(x => x.Id == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithoutLegEntity.CivilWithoutlegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addLoadOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilNonSteelEntity = _context.TLIcivilNonSteel.Where(x => x.Id == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilNonSteelEntity.CivilNonSteelLibraryId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArm.ToString())
                                    {
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == addLoadOther.TLIcivilLoads.sideArmId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addLoadOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addLoadOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addLoadOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLIloadOtherLibrary.ToString())
                {
                    var Operations = _context.TLIoperation.ToList();
                    AddLoadOtherLibraryViewModel addLoadOtherLibrary = _mapper.Map<AddLoadOtherLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIloadOtherLibrary>(addLoadOtherLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addLoadOtherLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLIpower.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIowner", "ownerId");
                    ForeignKeys.Add("TLIinstallationPlace", "installationPlaceId");
                    ForeignKeys.Add("TLIpowerLibrary", "powerLibraryId");
                    ForeignKeys.Add("TLIpowerType", "powerTypeId");
                    ForeignKeys.Add("TLIsideArm", "sideArmId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddPowerViewModel addPower = _mapper.Map<AddPowerViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIpower>(addPower);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode" && DictioneryValue != "sideArmId")
                                {
                                    var Entity = _mapper.Map<TLIpower>(addPower);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    if (string.IsNullOrEmpty(SiteCode) == false)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "sideArmId")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Id = addPower.TLIcivilLoads.sideArmId;
                                    if (Id != null)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {Id}";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    var Id = addPower.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithLegsId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithLegsId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    var Id = addPower.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithoutLegId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithoutLegId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    var Id = addPower.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilNonSteelId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilNonSteelId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addPower.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.LoadSubType.TLIloadOtherLibrary.ToString())
                                    {
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addPower.powerLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString())
                                    {
                                        var SideArmEntity = _context.TLIsideArm.Where(x => x.Id == addPower.TLIcivilLoads.sideArmId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == SideArmEntity.sideArmLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addPower.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithLegEntity = _context.TLIcivilWithLegs.Where(x => x.Id == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithLegEntity.CivilWithLegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addPower.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithoutLegEntity = _context.TLIcivilWithoutLeg.Where(x => x.Id == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithoutLegEntity.CivilWithoutlegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addPower.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilNonSteelEntity = _context.TLIcivilNonSteel.Where(x => x.Id == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilNonSteelEntity.CivilNonSteelLibraryId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArm.ToString())
                                    {
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == addPower.TLIcivilLoads.sideArmId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addPower.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addPower.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addPower.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString())
                {
                    var Operations = _context.TLIoperation.ToList();
                    AddPowerLibraryViewModel addPowerLibrary = _mapper.Map<AddPowerLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIpowerLibrary>(addPowerLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addPowerLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLIradioAntenna.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIowner", "ownerId");
                    ForeignKeys.Add("TLIinstallationPlace", "installationPlaceId");
                    ForeignKeys.Add("TLIradioAntennaLibrary", "radioAntennaLibraryId");
                    ForeignKeys.Add("TLIsideArm", "sideArmId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddRadioAntennaViewModel addRadioAntenna = _mapper.Map<AddRadioAntennaViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIradioAntenna>(addRadioAntenna);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode" && DictioneryValue != "sideArmId")
                                {
                                    var Entity = _mapper.Map<TLIradioAntenna>(addRadioAntenna);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    if (string.IsNullOrEmpty(SiteCode) == false)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "sideArmId")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Id = addRadioAntenna.TLIcivilLoads.sideArmId;
                                    if (Id != null)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {Id}";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    var Id = addRadioAntenna.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithLegsId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithLegsId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    var Id = addRadioAntenna.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithoutLegId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithoutLegId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    var Id = addRadioAntenna.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilNonSteelId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilNonSteelId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addRadioAntenna.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString())
                                    {
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addRadioAntenna.radioAntennaLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString())
                                    {
                                        var SideArmEntity = _context.TLIsideArm.Where(x => x.Id == addRadioAntenna.TLIcivilLoads.sideArmId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == SideArmEntity.sideArmLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioAntenna.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithLegEntity = _context.TLIcivilWithLegs.Where(x => x.Id == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithLegEntity.CivilWithLegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioAntenna.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithoutLegEntity = _context.TLIcivilWithoutLeg.Where(x => x.Id == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithoutLegEntity.CivilWithoutlegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioAntenna.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilNonSteelEntity = _context.TLIcivilNonSteel.Where(x => x.Id == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilNonSteelEntity.CivilNonSteelLibraryId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArm.ToString())
                                    {
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == addRadioAntenna.TLIcivilLoads.sideArmId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioAntenna.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioAntenna.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioAntenna.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLIradioRRU.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIowner", "ownerId");
                    ForeignKeys.Add("TLIradioRRULibrary", "radioRRULibraryId");
                    ForeignKeys.Add("TLIradioAntenna", "radioAntennaId");
                    ForeignKeys.Add("TLIinstallationPlace", "installationPlaceId");
                    ForeignKeys.Add("TLIsideArm", "sideArmId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddRadioRRUViewModel addRadioRRU = _mapper.Map<AddRadioRRUViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIRadioRRU>(addRadioRRU);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode" && DictioneryValue != "sideArmId")
                                {
                                    var Entity = _mapper.Map<TLIRadioRRU>(addRadioRRU);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    if (string.IsNullOrEmpty(SiteCode) == false)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "sideArmId")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Id = addRadioRRU.TLIcivilLoads.sideArmId;
                                    if (Id != null)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {Id}";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    var Id = addRadioRRU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithLegsId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithLegsId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    var Id = addRadioRRU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithoutLegId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithoutLegId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    var Id = addRadioRRU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilNonSteelId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilNonSteelId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addRadioRRU.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString())
                                    {
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addRadioRRU.radioRRULibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString())
                                    {
                                        var SideArmEntity = _context.TLIsideArm.Where(x => x.Id == addRadioRRU.TLIcivilLoads.sideArmId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == SideArmEntity.sideArmLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioRRU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithLegEntity = _context.TLIcivilWithLegs.Where(x => x.Id == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithLegEntity.CivilWithLegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioRRU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithoutLegEntity = _context.TLIcivilWithoutLeg.Where(x => x.Id == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithoutLegEntity.CivilWithoutlegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioRRU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilNonSteelEntity = _context.TLIcivilNonSteel.Where(x => x.Id == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilNonSteelEntity.CivilNonSteelLibraryId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArm.ToString())
                                    {
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == addRadioRRU.TLIcivilLoads.sideArmId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioRRU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioRRU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioRRU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLIradioOther.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIowner", "ownerId");
                    ForeignKeys.Add("TLIinstallationPlace", "installationPlaceId");
                    ForeignKeys.Add("TLIradioOtherLibrary", "radioOtherLibraryId");                
                    ForeignKeys.Add("TLIsideArm", "sideArmId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddRadioOtherViewModel addRadioOther = _mapper.Map<AddRadioOtherViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIradioOther>(addRadioOther);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode" && DictioneryValue != "sideArmId")
                                {
                                    var Entity = _mapper.Map<TLIradioOther>(addRadioOther);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    if (string.IsNullOrEmpty(SiteCode) == false)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "sideArmId")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Id = addRadioOther.TLIcivilLoads.sideArmId;
                                    if (Id != null)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {Id}";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    var Id = addRadioOther.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithLegsId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithLegsId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    var Id = addRadioOther.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithoutLegId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithoutLegId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    var Id = addRadioOther.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilNonSteelId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilNonSteelId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addRadioOther.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString())
                                    {
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addRadioOther.radioOtherLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString())
                                    {
                                        var SideArmEntity = _context.TLIsideArm.Where(x => x.Id == addRadioOther.TLIcivilLoads.sideArmId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == SideArmEntity.sideArmLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithLegEntity = _context.TLIcivilWithLegs.Where(x => x.Id == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithLegEntity.CivilWithLegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithoutLegEntity = _context.TLIcivilWithoutLeg.Where(x => x.Id == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithoutLegEntity.CivilWithoutlegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilNonSteelEntity = _context.TLIcivilNonSteel.Where(x => x.Id == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilNonSteelEntity.CivilNonSteelLibraryId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArm.ToString())
                                    {
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == addRadioOther.TLIcivilLoads.sideArmId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addRadioOther.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString())
                {
                    var Operations = _context.TLIoperation.ToList();
                    AddRadioAntennaLibraryViewModel addRadioAntennaLibrary = _mapper.Map<AddRadioAntennaLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIradioAntennaLibrary>(addRadioAntennaLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addRadioAntennaLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString())
                {
                    var Operations = _context.TLIoperation.ToList();
                    AddRadioRRULibraryViewModel addRadioRRULibrary = _mapper.Map<AddRadioRRULibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIradioRRULibrary>(addRadioRRULibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addRadioRRULibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString())
                {
                    var Operations = _context.TLIoperation.ToList();
                    AddRadioOtherLibraryViewModel addRadioOtherLibrary = _mapper.Map<AddRadioOtherLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIradioOtherLibrary>(addRadioOtherLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addRadioOtherLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                //Need Check
                else if (TableName == Helpers.Constants.LoadSubType.TLImwBU.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIowner", "ownerId");
                    ForeignKeys.Add("TLImwBULibrary", "MwBULibraryId");
                    ForeignKeys.Add("TLImwDish", "MainDishId");
                    ForeignKeys.Add("TLImwDish", "SdDishId");
                    ForeignKeys.Add("TLIsideArm", "sideArmId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddMW_BUViewModel addMW_BU = _mapper.Map<AddMW_BUViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLImwBU>(addMW_BU);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode" && DictioneryValue != "sideArmId")
                                {
                                    var Entity = _mapper.Map<TLImwBU>(addMW_BU);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    if (string.IsNullOrEmpty(SiteCode) == false)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "sideArmId")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Id = addMW_BU.TLIcivilLoads.sideArmId;
                                    if (Id != null)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {Id}";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    var Id = addMW_BU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithLegsId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithLegsId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    var Id = addMW_BU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithoutLegId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithoutLegId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    var Id = addMW_BU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilNonSteelId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilNonSteelId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addMW_BU.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.LoadSubType.TLIloadOtherLibrary.ToString())
                                    {
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addMW_BU.MwBULibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString())
                                    {
                                        var SideArmEntity = _context.TLIsideArm.Where(x => x.Id == addMW_BU.TLIcivilLoads.sideArmId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == SideArmEntity.sideArmLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_BU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithLegEntity = _context.TLIcivilWithLegs.Where(x => x.Id == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithLegEntity.CivilWithLegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_BU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithoutLegEntity = _context.TLIcivilWithoutLeg.Where(x => x.Id == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithoutLegEntity.CivilWithoutlegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_BU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilNonSteelEntity = _context.TLIcivilNonSteel.Where(x => x.Id == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilNonSteelEntity.CivilNonSteelLibraryId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArm.ToString())
                                    {
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == addMW_BU.TLIcivilLoads.sideArmId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_BU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_BU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_BU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                //Need Check
                else if (TableName == Helpers.Constants.LoadSubType.TLImwDish.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIrepeaterType", "RepeaterTypeId");
                    ForeignKeys.Add("TLIpolarityOnLocation", "PolarityOnLocationId");
                    ForeignKeys.Add("TLIitemConnectTo", "ItemConnectToId");
                    ForeignKeys.Add("TLImwDishLibrary", "MwDishLibraryId");
                    ForeignKeys.Add("TLIinstallationPlace", "InstallationPlaceId");
                    ForeignKeys.Add("TLIsideArm", "sideArmId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddMW_DishViewModel AddMW_Dish = _mapper.Map<AddMW_DishViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLImwDish>(AddMW_Dish);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode" && DictioneryValue != "sideArmId")
                                {
                                    var Entity = _mapper.Map<TLImwDish>(AddMW_Dish);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    if (string.IsNullOrEmpty(SiteCode) == false)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "sideArmId")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Id = AddMW_Dish.TLIcivilLoads.sideArmId;
                                    if (Id != null)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {Id}";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    var Id = AddMW_Dish.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithLegsId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithLegsId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    var Id = AddMW_Dish.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithoutLegId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithoutLegId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    var Id = AddMW_Dish.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilNonSteelId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilNonSteelId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = AddMW_Dish.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLImwODU.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIowner", "OwnerId");
                    ForeignKeys.Add("TLImwDish", "Mw_DishId");
                    ForeignKeys.Add("TLIoduInstallationType", "OduInstallationTypeId");
                    ForeignKeys.Add("TLImwODULibrary", "MwODULibraryId");
                    ForeignKeys.Add("TLIsideArm", "sideArmId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddMW_ODUViewModel addMW_ODU = _mapper.Map<AddMW_ODUViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLImwODU>(addMW_ODU);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode" && DictioneryValue != "sideArmId")
                                {
                                    var Entity = _mapper.Map<TLImwODU>(addMW_ODU);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    if (string.IsNullOrEmpty(SiteCode) == false)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "sideArmId")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Id = addMW_ODU.TLIcivilLoads.sideArmId;
                                    if (Id != null)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {Id}";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    var Id = addMW_ODU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithLegsId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithLegsId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    var Id = addMW_ODU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithoutLegId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithoutLegId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    var Id = addMW_ODU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilNonSteelId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilNonSteelId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            AddDynamicAttInstValueViewModel dynamicAttributeInstValue = null;
                            TLIdynamicAttLibValue dynamicAttLibValue = null;
                            TLIdynamicAttInstValue dynamicAttInstValue = null;
                            if (Rule.DynamicAttribute.tablesNamesId == TableNameId)
                            {
                                dynamicAttributeInstValue = addMW_ODU.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                                if (Operation.Name.ToLower() == "required")
                                {
                                    if (dynamicAttributeInstValue == null)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                        break;
                                    }
                                }
                                else
                                {
                                    if (dynamicAttributeInstValue != null)
                                    {
                                        if (dynamicAttributeInstValue.ValueBoolean != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDateTime != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueDouble != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (dynamicAttributeInstValue.ValueString != null)
                                        {
                                            if (CheckValue(dynamicAttributeInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Rule.DynamicAttribute.LibraryAtt == true)
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if(TableNameEntity.TableName == Helpers.Constants.LoadSubType.TLImwODULibrary.ToString())
                                    {
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == Rule.DynamicAttribute.tablesNamesId && x.InventoryId == addMW_ODU.MwODULibraryId).FirstOrDefault();
                                    }
                                    else if(TableNameEntity.TableName == Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString())
                                    {
                                        var DishEntity = _context.TLImwDish.Where(x => x.Id == addMW_ODU.Mw_DishId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == DishEntity.MwDishLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString())
                                    {
                                        var SideArmEntity = _context.TLIsideArm.Where(x => x.Id == addMW_ODU.TLIcivilLoads.sideArmId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == SideArmEntity.sideArmLibraryId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_ODU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithLegEntity = _context.TLIcivilWithLegs.Where(x => x.Id == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithLegEntity.CivilWithLegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_ODU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilWithoutLegEntity = _context.TLIcivilWithoutLeg.Where(x => x.Id == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilWithoutLegEntity.CivilWithoutlegsLibId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_ODU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        var CivilNonSteelEntity = _context.TLIcivilNonSteel.Where(x => x.Id == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                        dynamicAttLibValue = _context.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == CivilNonSteelEntity.CivilNonSteelLibraryId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttLibValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttLibValue != null)
                                        {
                                            if (dynamicAttLibValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttLibValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttLibValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    var TableNameEntity = _context.TLItablesNames.Where(x => x.Id == Rule.DynamicAttribute.tablesNamesId).FirstOrDefault();
                                    if (TableNameEntity.TableName == Helpers.Constants.LoadSubType.TLImwDish.ToString())
                                    {
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == addMW_ODU.Mw_DishId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.TablesNames.TLIsideArm.ToString())
                                    {
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == addMW_ODU.TLIcivilLoads.sideArmId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_ODU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithLegsId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_ODU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilWithoutLegId).FirstOrDefault();
                                    }
                                    else if (TableNameEntity.TableName == Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString())
                                    {
                                        var AllCivilInst = _context.TLIallCivilInst.Where(x => x.Id == addMW_ODU.TLIcivilLoads.allCivilInstId).FirstOrDefault();
                                        dynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id && x.tablesNamesId == TableNameEntity.Id && x.InventoryId == AllCivilInst.civilNonSteelId).FirstOrDefault();
                                    }
                                    if (Operation.Name.ToLower() == "required")
                                    {
                                        if (dynamicAttInstValue == null)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name}";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dynamicAttInstValue != null)
                                        {
                                            if (dynamicAttInstValue.ValueBoolean != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDateTime != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueDouble != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (dynamicAttInstValue.ValueString != null)
                                            {
                                                if (CheckValue(dynamicAttInstValue.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationResult = false;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                //Need Check
                else if (TableName == Helpers.Constants.LoadSubType.TLImwRFU.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLImwRFULibrary", "MwRFULibraryId");
                    ForeignKeys.Add("TLImwPort", "MwPortId");
                    ForeignKeys.Add("TLIsideArm", "sideArmId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddMW_RFUViewModel addMW_RFU = _mapper.Map<AddMW_RFUViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLImwRFU>(addMW_RFU);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode" && DictioneryValue != "sideArmId")
                                {
                                    var Entity = _mapper.Map<TLImwRFU>(addMW_RFU);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    if (string.IsNullOrEmpty(SiteCode) == false)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "sideArmId")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Id = addMW_RFU.TLIcivilLoads.sideArmId;
                                    if (Id != null)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {Id}";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    var Id = addMW_RFU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithLegsId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithLegsId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    var Id = addMW_RFU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithoutLegId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithoutLegId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    var Id = addMW_RFU.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilNonSteelId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilNonSteelId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if(Rule.AttributeActivated.Tabel == Helpers.Constants.LoadSubType.TLImwBU.ToString())
                                {
                                    var Id = addMW_RFU.MwPortId;
                                    if (Id != null)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        var mwBUId = _context.TLImwPort.Find(Id);
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"TLImwBu\" Where \"TLImwBu\".\"Id\" = {mwBUId.MwBUId}";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addMW_RFU.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                //Need Check
                else if (TableName == Helpers.Constants.LoadSubType.TLImwOther.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLImwOtherLibrary", "mwOtherLibraryId");
                    ForeignKeys.Add("TLIsideArm", "sideArmId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddMw_OtherViewModel addMw_Other = _mapper.Map<AddMw_OtherViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLImwOther>(addMw_Other);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode" && DictioneryValue != "sideArmId")
                                {
                                    var Entity = _mapper.Map<TLImwOther>(addMw_Other);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    if (string.IsNullOrEmpty(SiteCode) == false)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "sideArmId")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    var Id = addMw_Other.TLIcivilLoads.sideArmId;
                                    if (Id != null)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {Id}";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    var Id = addMw_Other.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithLegsId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithLegsId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    var Id = addMw_Other.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithoutLegId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithoutLegId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    var Id = addMw_Other.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilNonSteelId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilNonSteelId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addMw_Other.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLImwBULibrary.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIdiversityType", "diversityTypeId");
                    var Operations = _context.TLIoperation.ToList();
                    AddMW_BULibraryViewModel addMW_BULibrary = _mapper.Map<AddMW_BULibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLImwBULibrary>(addMW_BULibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false)
                                {
                                    var Entity = _mapper.Map<TLImwBULibrary>(addMW_BULibrary);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addMW_BULibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIpolarityType", "polarityTypeId");
                    ForeignKeys.Add("TLIasType", "asTypeId");
                    var Operations = _context.TLIoperation.ToList();
                    AddMW_DishLibraryViewModel addMW_DishLibrary = _mapper.Map<AddMW_DishLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLImwDishLibrary>(addMW_DishLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false)
                                {
                                    var Entity = _mapper.Map<TLImwDishLibrary>(addMW_DishLibrary);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addMW_DishLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLImwODULibrary.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIparity", "parityId");
                    var Operations = _context.TLIoperation.ToList();
                    AddMW_ODULibraryViewModel addMW_ODULibrary = _mapper.Map<AddMW_ODULibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLImwODULibrary>(addMW_ODULibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false)
                                {
                                    var Entity = _mapper.Map<TLImwODULibrary>(addMW_ODULibrary);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addMW_ODULibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLImwRFULibrary.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIdiversityType", "diversityTypeId");
                    ForeignKeys.Add("TLIboardType", "boardTypeId");
                    var Operations = _context.TLIoperation.ToList();
                    AddMW_RFULibraryViewModel addMW_RFULibrary = _mapper.Map<AddMW_RFULibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLImwRFULibrary>(addMW_RFULibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false)
                                {
                                    var Entity = _mapper.Map<TLImwRFULibrary>(addMW_RFULibrary);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addMW_RFULibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString())
                {
                    var Operations = _context.TLIoperation.ToList();
                    AddMW_OtherLibraryViewModel addMW_OtherLibrary = _mapper.Map<AddMW_OtherLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLImwOtherLibrary>(addMW_OtherLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addMW_OtherLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                //Need Check
                else if (TableName == Helpers.Constants.TablesNames.TLIsideArm.ToString())
                {
                    var ForeignKeys = new Dictionary<string, string>();
                    ForeignKeys.Add("TLIsideArmLibrary", "sideArmLibraryId");
                    ForeignKeys.Add("TLIsideArmInstallationPlace", "sideArmInstallationPlaceId");
                    ForeignKeys.Add("TLIowner", "ownerId");
                    ForeignKeys.Add("TLIsideArmType", "sideArmTypeId");
                    ForeignKeys.Add("TLIsite", "SiteCode");
                    var Operations = _context.TLIoperation.ToList();
                    AddSideArmViewModel addSideArm = _mapper.Map<AddSideArmViewModel>(AddViewModel);

                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIsideArm>(addSideArm);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ForeignKeys.TryGetValue(Rule.AttributeActivated.Tabel, out DictioneryValue);
                                if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue != "SiteCode")
                                {
                                    var Entity = _mapper.Map<TLIsideArm>(addSideArm);
                                    var RecordValue = GetPropertyValue(Entity, DictioneryValue);
                                    var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                    using (OracleCommand cmd = con.CreateCommand())
                                    {
                                        cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {RecordValue}";
                                        var value = cmd.ExecuteReader();
                                        while (value.Read())
                                        {
                                            result = value.GetValue(0);
                                        }
                                        if (Rule.Rule.OperationValueBoolean != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDateTime != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueDouble != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                break;
                                            }
                                        }
                                        else if (Rule.Rule.OperationValueString != null)
                                        {
                                            if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                            {
                                                ValidationResult = false;
                                                ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (string.IsNullOrEmpty(DictioneryValue) == false && DictioneryValue == "SiteCode")
                                {
                                    //var Entity = addCivilWithLegs.TLIcivilSiteDate;
                                    //var RecordValue = GetPropertyValue(addCivilWithLegs.TLIcivilSiteDate, DictioneryValue);
                                    if (string.IsNullOrEmpty(SiteCode) == false)
                                    {
                                        var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                        using (OracleCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"SiteCode\" = '{SiteCode}'";
                                            var value = cmd.ExecuteReader();
                                            while (value.Read())
                                            {
                                                result = value.GetValue(0);
                                            }
                                            if (Rule.Rule.OperationValueBoolean != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDateTime != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueDouble != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                    break;
                                                }
                                            }
                                            else if (Rule.Rule.OperationValueString != null)
                                            {
                                                if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                {
                                                    ValidationResult = false;
                                                    ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithLegs.ToString())
                                {
                                    var Id = addSideArm.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithLegsId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithLegsId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString())
                                {
                                    var Id = addSideArm.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilWithoutLegId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilWithoutLegId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                                else if (Rule.AttributeActivated.Tabel == Helpers.Constants.CivilType.TLIcivilNonSteel.ToString())
                                {
                                    var Id = addSideArm.TLIcivilLoads.allCivilInstId;
                                    if (Id != 0)
                                    {
                                        var AllInst = _context.TLIallCivilInst.Find(Id);
                                        if (AllInst.civilNonSteelId != null)
                                        {
                                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                            using (OracleCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = $"SELECT \"{Rule.AttributeActivated.Key}\" from \"{Rule.AttributeActivated.Tabel}\" Where \"{Rule.AttributeActivated.Tabel}\".\"Id\" = {AllInst.civilNonSteelId}";
                                                var value = cmd.ExecuteReader();
                                                while (value.Read())
                                                {
                                                    result = value.GetValue(0);
                                                }
                                                if (Rule.Rule.OperationValueBoolean != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDateTime != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueDouble != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                                        break;
                                                    }
                                                }
                                                else if (Rule.Rule.OperationValueString != null)
                                                {
                                                    if (CheckValue(result, Operation.Name, Rule.Rule.OperationValueString) == false)
                                                    {
                                                        ValidationResult = false;
                                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationResult = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addSideArm.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                else if (TableName == Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString())
                {
                    var Operations = _context.TLIoperation.ToList();
                    AddSideArmLibraryViewModel addSideArmLibrary = _mapper.Map<AddSideArmLibraryViewModel>(AddViewModel);
                    ValidationResult = true;
                    var Rows = _context.TLIdependencyRow.Where(x => x.DependencyId == validation.Id).Select(x => x.RowId).ToList();
                    var Rules = _context.TLIrowRule
                                    .Where(x => Rows.Contains(x.RowId))
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.attributeActivated)
                                    .Include(x => x.Rule)
                                    .ThenInclude(x => x.dynamicAtt)
                                    .Select(x => new
                                    {
                                        Rule = x.Rule,
                                        AttributeActivated = x.Rule.attributeActivated,
                                        DynamicAttribute = x.Rule.dynamicAtt
                                    })
                                    .ToList();

                    foreach (var Rule in Rules)
                    {
                        result = null;
                        DictioneryValue = string.Empty;
                        if (Rule.AttributeActivated != null)
                        {
                            if (Rule.AttributeActivated.Tabel == TableName)
                            {
                                var Entity = _mapper.Map<TLIsideArmLibrary>(addSideArmLibrary);
                                var RecordValue = GetPropertyValue(Entity, Rule.AttributeActivated.Key);
                                var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                                if (Rule.Rule.OperationValueBoolean != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDateTime != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueDouble != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                        break;
                                    }
                                }
                                else if (Rule.Rule.OperationValueString != null)
                                {
                                    if (CheckValue(RecordValue, Operation.Name, Rule.Rule.OperationValueString) == false)
                                    {
                                        ValidationResult = false;
                                        ErrorMessage = $"{Rule.AttributeActivated.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Rule.DynamicAttribute != null)
                        {
                            var Operation = Operations.Where(x => x.Id == Rule.Rule.OperationId).FirstOrDefault();
                            var dynamicAttribute = addSideArmLibrary.TLIdynamicAttLibValue.Where(x => x.DynamicAttId == Rule.DynamicAttribute.Id).FirstOrDefault();
                            if (Operation.Name.ToLower() == "required")
                            {
                                if (dynamicAttribute == null)
                                {
                                    ValidationResult = false;
                                    ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name}";
                                    break;
                                }
                            }
                            else
                            {
                                if (dynamicAttribute != null)
                                {
                                    if (dynamicAttribute.ValueBoolean != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueBoolean, Operation.Name, Rule.Rule.OperationValueBoolean) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueBoolean}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDateTime != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDateTime, Operation.Name, Rule.Rule.OperationValueDateTime) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDateTime}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueDouble != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueDouble, Operation.Name, Rule.Rule.OperationValueDouble) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueDouble}";
                                            break;
                                        }
                                    }
                                    else if (dynamicAttribute.ValueString != null)
                                    {
                                        if (CheckValue(dynamicAttribute.ValueString, Operation.Name, Rule.Rule.OperationValueString) == false)
                                        {
                                            ValidationResult = false;
                                            ErrorMessage = $"{Rule.DynamicAttribute.Key} from {Rule.AttributeActivated.Tabel} Should {Operation.Name} {Rule.Rule.OperationValueString}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationResult = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (validation.ValueBoolean != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueBoolean) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueBoolean}";
                        }
                    }
                    else if (validation.ValueDateTime != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDateTime) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDateTime}";
                        }
                    }
                    else if (validation.ValueDouble != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueDouble) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueDouble}";
                        }
                    }
                    else if (validation.ValueString != null)
                    {
                        if (CheckValue(DynamicAttValue, ValidationOperation.Name, validation.ValueString) == false)
                        {
                            ValidationResult = false;
                            ErrorMessage = $"{DynamicAttName} from TLIdynamicAtt Should {ValidationOperation.Name} {validation.ValueString}";
                        }
                    }
                }
                return ValidationResult;
            }
            catch (Exception err)
            {
                return false;
            }
        }
        private bool CheckValue(object RecordValue, string OperationName, object ValidationValue)
        {
            if (RecordValue.GetType() == typeof(string))
            {
                if (OperationName == "==")
                {
                    if (RecordValue.ToString() == ValidationValue.ToString())
                    {
                        return true;
                    }
                }
                else if (OperationName == "!=")
                {
                    if (RecordValue.ToString() == ValidationValue.ToString())
                    {
                        return true;
                    }
                }
            }
            else if (RecordValue.GetType() == typeof(double))
            {
                if (OperationName == "==")
                {
                    if (Convert.ToDouble(RecordValue) == Convert.ToDouble(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "!=")
                {
                    if (Convert.ToDouble(RecordValue) != Convert.ToDouble(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == ">")
                {
                    if (Convert.ToDouble(RecordValue) > Convert.ToDouble(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == ">=")
                {
                    if (Convert.ToDouble(RecordValue) >= Convert.ToDouble(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "<")
                {
                    if (Convert.ToDouble(RecordValue) < Convert.ToDouble(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "<=")
                {
                    if (Convert.ToDouble(RecordValue) <= Convert.ToDouble(ValidationValue))
                    {
                        return true;
                    }
                }
            }
            else if (RecordValue.GetType() == typeof(float))
            {
                if (OperationName == "==")
                {
                    if (Convert.ToSingle(RecordValue) == Convert.ToSingle(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "!=")
                {
                    if (Convert.ToSingle(RecordValue) != Convert.ToSingle(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == ">")
                {
                    if (Convert.ToSingle(RecordValue) > Convert.ToSingle(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == ">=")
                {
                    if (Convert.ToSingle(RecordValue) >= Convert.ToSingle(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "<")
                {
                    if (Convert.ToSingle(RecordValue) < Convert.ToSingle(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "<=")
                {
                    if (Convert.ToSingle(RecordValue) <= Convert.ToSingle(ValidationValue))
                    {
                        return true;
                    }
                }
            }
            else if (RecordValue.GetType() == typeof(bool))
            {
                if (OperationName == "==")
                {
                    if (Convert.ToBoolean(RecordValue) == Convert.ToBoolean(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "!=")
                {
                    if (Convert.ToBoolean(RecordValue) != Convert.ToBoolean(ValidationValue))
                    {
                        return true;
                    }
                }
            }
            else if (RecordValue.GetType() == typeof(DateTime))
            {
                if (OperationName == "==")
                {
                    if (Convert.ToDateTime(RecordValue) == Convert.ToDateTime(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "!=")
                {
                    if (Convert.ToDateTime(RecordValue) != Convert.ToDateTime(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == ">")
                {
                    if (Convert.ToDateTime(RecordValue) > Convert.ToDateTime(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == ">=")
                {
                    if (Convert.ToDateTime(RecordValue) >= Convert.ToDateTime(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "<")
                {
                    if (Convert.ToDateTime(RecordValue) < Convert.ToDateTime(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "<=")
                {
                    if (Convert.ToDateTime(RecordValue) <= Convert.ToDateTime(ValidationValue))
                    {
                        return true;
                    }
                }
            }
            else if (RecordValue.GetType() == typeof(Int16) || RecordValue.GetType() == typeof(Int32) || RecordValue.GetType() == typeof(Int64))
            {
                if (OperationName == "==")
                {
                    if (Convert.ToInt64(RecordValue) == Convert.ToInt64(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "!=")
                {
                    if (Convert.ToInt64(RecordValue) != Convert.ToInt64(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == ">")
                {
                    if (Convert.ToInt64(RecordValue) > Convert.ToInt64(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == ">=")
                {
                    if (Convert.ToInt64(RecordValue) >= Convert.ToInt64(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "<")
                {
                    if (Convert.ToInt64(RecordValue) < Convert.ToInt64(ValidationValue))
                    {
                        return true;
                    }
                }
                else if (OperationName == "<=")
                {
                    if (Convert.ToInt64(RecordValue) <= Convert.ToInt64(ValidationValue))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private object GetPropertyValue(object Source, string PropertyName)
        {
            return Source.GetType().GetProperty(PropertyName).GetValue(Source);
        }
        public bool CheckValidation(List<TLIvalidation> Validations, string DynamicAttName, object Value, out string ErrorMessage)
        {
            ErrorMessage = string.Empty;
            foreach (var Validation in Validations)
            {
                var Operation = _context.TLIoperation.Find(Validation.OperationId);
                if (Value.GetType() == typeof(string))
                {
                    if (Operation.Name == "==")
                    {
                        if (!(Validation.ValueString == Value.ToString()))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueString}";
                            return false;
                        }
                    }
                    else if (Operation.Name == "!=")
                    {
                        if (!(Validation.ValueString != Value.ToString()))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueString}";
                            return false;
                        }
                    }
                }
                else if (Value.GetType() == typeof(double))
                {
                    if (Operation.Name == "==")
                    {
                        if (!(Validation.ValueDouble == Convert.ToDouble(Value)))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDouble}";
                            return false;
                        }
                    }
                    else if (Operation.Name == "!=")
                    {
                        if (!(Convert.ToDouble(Value) != Validation.ValueDouble))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDouble}";
                            return false;
                        }
                    }
                    else if (Operation.Name == ">")
                    {
                        if (!(Convert.ToDouble(Value) > Validation.ValueDouble))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDouble}";
                            return false;
                        }
                    }
                    else if (Operation.Name == ">=")
                    {
                        if (!(Convert.ToDouble(Value) >= Validation.ValueDouble))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDouble}";
                            return false;
                        }
                    }
                    else if (Operation.Name == "<")
                    {
                        if (!(Convert.ToDouble(Value) < Validation.ValueDouble))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDouble}";
                            return false;
                        }
                    }
                    else if (Operation.Name == "<=")
                    {
                        if (!(Convert.ToDouble(Value) <= Validation.ValueDouble))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDouble}";
                            return false;
                        }
                    }
                }
                else if (Value.GetType() == typeof(bool))
                {
                    if (Operation.Name == "==")
                    {
                        if (!(Validation.ValueBoolean == Convert.ToBoolean(Value)))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueBoolean}";
                            return false;
                        }
                    }
                    else if (Operation.Name == "!=")
                    {
                        if (!(Validation.ValueBoolean != Convert.ToBoolean(Value)))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueBoolean}";
                            return false;
                        }
                    }
                }
                else if (Value.GetType() == typeof(DateTime))
                {
                    if (Operation.Name == "==")
                    {
                        if (!(Validation.ValueDateTime == Convert.ToDateTime(Value)))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDateTime}";
                            return false;
                        }
                    }
                    else if (Operation.Name == "!=")
                    {
                        if (!(Convert.ToDateTime(Value) != Validation.ValueDateTime))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDateTime}";
                            return false;
                        }
                    }
                    else if (Operation.Name == ">")
                    {
                        if (!(Convert.ToDateTime(Value) > Validation.ValueDateTime))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDateTime}";
                            return false;
                        }
                    }
                    else if (Operation.Name == ">=")
                    {
                        if (!(Convert.ToDateTime(Value) >= Validation.ValueDateTime))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDateTime}";
                            return false;
                        }
                    }
                    else if (Operation.Name == "<")
                    {
                        if (!(Convert.ToDateTime(Value) < Validation.ValueDateTime))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDateTime}";
                            return false;
                        }
                    }
                    else if (Operation.Name == "<=")
                    {
                        if (!(Convert.ToDateTime(Value) <= Validation.ValueDateTime))
                        {
                            ErrorMessage = $"{DynamicAttName} Should {Operation.Name} {Validation.ValueDateTime}";
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
