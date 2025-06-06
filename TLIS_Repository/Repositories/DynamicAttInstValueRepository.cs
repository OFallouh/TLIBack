﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DependencyDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace TLIS_Repository.Repositories
{
    public class DynamicAttInstValueRepository : RepositoryBase<TLIdynamicAttInstValue, DynamicAttInstValueViewModel, int>, IDynamicAttInstValueRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public DynamicAttInstValueRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddDynamicInstAtts(AddDynamicAttInstValueViewModel addDynamicInstAttValue, int TableNameId, int Id)
        {
            var DynamicAtt = _context.TLIdynamicAtt.Include(x => x.DataType).FirstOrDefault(x => x.Id == addDynamicInstAttValue.DynamicAttId);
            TLIdynamicAttInstValue dynamicAttInstValue = new TLIdynamicAttInstValue();
            if (!string.IsNullOrEmpty(addDynamicInstAttValue.ValueString))
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueString = addDynamicInstAttValue.ValueString;
                dynamicAttInstValue.disable = false;
                _context.TLIdynamicAttInstValue.Add(dynamicAttInstValue);
                _context.SaveChanges();
            }
            else if (addDynamicInstAttValue.ValueDouble != null)
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueDouble = addDynamicInstAttValue.ValueDouble;
                dynamicAttInstValue.disable = false;
                _context.TLIdynamicAttInstValue.Add(dynamicAttInstValue);
                _context.SaveChanges();
            }
            else if (addDynamicInstAttValue.ValueDateTime != null)
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueDateTime = addDynamicInstAttValue.ValueDateTime;
                dynamicAttInstValue.disable = false;
                _context.TLIdynamicAttInstValue.Add(dynamicAttInstValue);
                _context.SaveChanges();
            }
            else if (addDynamicInstAttValue.ValueBoolean != null)
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueBoolean = addDynamicInstAttValue.ValueBoolean;
                dynamicAttInstValue.disable = false;
                _context.TLIdynamicAttInstValue.Add(dynamicAttInstValue);
                _context.SaveChanges();
            }
        }
        public void AddDdynamicAttributeInstallation(int UserId,AddDdynamicAttributeInstallationValueViewModel addDynamicInstAttValue, int TableNameId, int Id,string connectionString)
        {

            var DynamicAtt = _context.TLIdynamicAtt.Include(x => x.DataType).Include(x=>x.tablesNames).FirstOrDefault(x => x.Id == addDynamicInstAttValue.id);
            TLIdynamicAttInstValue dynamicAttInstValue = new TLIdynamicAttInstValue();
            if (addDynamicInstAttValue.value is string stringValue)
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueString = stringValue;
                dynamicAttInstValue.disable = false;
               AddWithHistorys(UserId,dynamicAttInstValue);
                _context.SaveChanges();
            }
            else if( addDynamicInstAttValue.value is double DoubleValue)
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueDouble = DoubleValue;
                dynamicAttInstValue.disable = false;
               AddWithHistorys(UserId,dynamicAttInstValue);
                _context.SaveChanges();
            }
            else if (addDynamicInstAttValue.value is DateTime dateTimeValue)
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueDateTime = dateTimeValue;
                dynamicAttInstValue.disable = false;
             AddWithHistorys(UserId,dynamicAttInstValue);
                _context.SaveChanges();
            }
            else if (addDynamicInstAttValue.value is bool booleanValue)
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueBoolean = booleanValue;
                dynamicAttInstValue.disable = false;
                AddWithHistorys(UserId,dynamicAttInstValue);
                _context.SaveChanges();
            }
 
        }
        public List<DynaminAttInstViewModel> GetDynamicInstAtts(int TableNameId, int Id, int? CategoryId = null)
        {
            List<DynaminAttInstViewModel> dynamicAttInstViewModels = new List<DynaminAttInstViewModel>();
            List<TLIdynamicAtt> DynamicAtts = new List<TLIdynamicAtt>();

            if (CategoryId == null)
            {
                DynamicAtts = _context.TLIdynamicAtt.Include(x => x.DataType)
                    .Where(x => !x.LibraryAtt && x.tablesNamesId == TableNameId && !x.disable).ToList();
            }
            else
            {
                DynamicAtts = _context.TLIdynamicAtt.Include(x => x.tablesNames).Include(x => x.DataType)
                    .Where(x => x.CivilWithoutLegCategoryId == CategoryId && !x.LibraryAtt &&
                        x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString().ToLower() &&
                        !x.disable).ToList();
            }
            foreach (TLIdynamicAtt DynamicAtt in DynamicAtts)
            {
                TLIdynamicAttInstValue DynamicInstAtt = _context.TLIdynamicAttInstValue
                    .FirstOrDefault(x => x.DynamicAttId == DynamicAtt.Id && 
                        x.tablesNamesId == TableNameId && x.InventoryId == Id);

                if (DynamicAtt != null)
                {
                    if (DynamicInstAtt == null)
                    {
                        DynamicInstAtt = new TLIdynamicAttInstValue();
                    }
                    dynamicAttInstViewModels.Add(new DynaminAttInstViewModel
                    {
                        Id = DynamicAtt.Id,
                        Key = DynamicAtt.Key,
                        DataTypeId = DynamicAtt.DataTypeId,
                        DataType = DynamicAtt.DataType.Name,
                        ValueString = DynamicInstAtt.ValueString != null ? DynamicInstAtt.ValueString : null,
                        ValueBoolean = DynamicInstAtt.ValueBoolean != null ? DynamicInstAtt.ValueBoolean : null,
                        ValueDateTime = DynamicInstAtt.ValueDateTime != null ? DynamicInstAtt.ValueDateTime : null,
                        ValueDouble = DynamicInstAtt.ValueDouble != null ? DynamicInstAtt.ValueDouble : null,
                        Required= DynamicAtt.Required
                    });
                }
            }
            return dynamicAttInstViewModels;
        }
        public void AddDdynamicAttributeInstallations(int UserId, List<AddDdynamicAttributeInstallationValueViewModel> addDynamicInstAttValue, int TableNameId, int Id,string connectionString)
        {
            var TabelName = _context.TLItablesNames.FirstOrDefault(x => x.Id == TableNameId)?.TableName;
            var dynamicAttLibValueEntities = addDynamicInstAttValue.Select(DynamicLibAttValue =>
            {
                var DynamicAtt = _context.TLIdynamicAtt
                   .Where(x => x.Id == DynamicLibAttValue.id)
                   .Include(x => x.DataType).Include(x=>x.tablesNames)
                   .FirstOrDefault();
                TLIdynamicAttInstValue dynamicAttInstValue = new TLIdynamicAttInstValue();
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamic value = DynamicLibAttValue.value?.ToString();
                if (value != null)
                {
                    string dataType = DynamicAtt.DataType.Name.ToLower();

                    switch (dataType)
                    {
                        case "bool":
                            bool boolValue;
                            if (bool.TryParse(value, out boolValue))
                            {
                                dynamicAttInstValue.ValueBoolean = boolValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDouble = null;

                                throw new ArgumentException("Invalid boolean value.");
                            }
                            break;
                        case "datetime":
                            DateTime dateTimeValue;
                            if (DateTime.TryParse(value, out dateTimeValue))
                            {
                                dynamicAttInstValue.ValueDateTime = dateTimeValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDateTime = null;

                                throw new ArgumentException("Invalid datetime value.");
                            }
                            break;
                        case "double":
                            double doubleValue;
                            if (double.TryParse(value, out doubleValue))
                            {
                                dynamicAttInstValue.ValueDouble = doubleValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDouble = null;

                                throw new ArgumentException("Invalid double value.");
                            }
                            break;
                        case "int":
                            int intValue;
                            if (int.TryParse(value, out intValue))
                            {
                                dynamicAttInstValue.ValueDouble = intValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDouble = null;

                                throw new ArgumentException("Invalid int value.");
                            }
                            break;
                        case "string":
                            dynamicAttInstValue.ValueString = value;
                            break;
                        default:

                            break;
                    }
                }
                dynamicAttInstValue.disable = false;
                return dynamicAttInstValue;
            }).ToList();

            AddRangeWithHistory(UserId, dynamicAttLibValueEntities);
            _context.SaveChanges();
            Task.Run(() =>RefreshView(connectionString));
        }
        public void AddDdynamicAttributeInstallationsH(int UserId, List<AddDdynamicAttributeInstallationValueViewModel> addDynamicInstAttValue, int TableNameId, int Id, string connectionString,int HistoryId)
        {
            var TabelName = _context.TLItablesNames.FirstOrDefault(x => x.Id == TableNameId)?.TableName;
            var TabelNameId = _context.TLItablesNames.FirstOrDefault(x => x.TableName == "TLIdynamicAttInstValue").Id;
            var dynamicAttLibValueEntities = addDynamicInstAttValue.Select(DynamicLibAttValue =>
            {
                var DynamicAtt = _context.TLIdynamicAtt
                   .Where(x => x.Id == DynamicLibAttValue.id)
                   .Include(x => x.DataType).Include(x => x.tablesNames)
                   .FirstOrDefault();
                TLIdynamicAttInstValue dynamicAttInstValue = new TLIdynamicAttInstValue();
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamic value = DynamicLibAttValue.value?.ToString();
                if (value != null)
                {
                    string dataType = DynamicAtt.DataType.Name.ToLower();

                    switch (dataType)
                    {
                        case "bool":
                            bool boolValue;
                            if (bool.TryParse(value, out boolValue))
                            {
                                dynamicAttInstValue.ValueBoolean = boolValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDouble = null;

                                throw new ArgumentException("Invalid boolean value.");
                            }
                            break;
                        case "datetime":
                            DateTime dateTimeValue;
                            if (DateTime.TryParse(value, out dateTimeValue))
                            {
                                dynamicAttInstValue.ValueDateTime = dateTimeValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDateTime = null;

                                throw new ArgumentException("Invalid datetime value.");
                            }
                            break;
                        case "double":
                            double doubleValue;
                            if (double.TryParse(value, out doubleValue))
                            {
                                dynamicAttInstValue.ValueDouble = doubleValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDouble = null;

                                throw new ArgumentException("Invalid double value.");
                            }
                            break;
                        case "int":
                            int intValue;
                            if (int.TryParse(value, out intValue))
                            {
                                dynamicAttInstValue.ValueDouble = intValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDouble = null;

                                throw new ArgumentException("Invalid int value.");
                            }
                            break;
                        case "string":
                            dynamicAttInstValue.ValueString = value;
                            break;
                        default:

                            break;
                    }
                }
                dynamicAttInstValue.disable = false;
                return dynamicAttInstValue;
            }).ToList();

            AddRangeWithHDynamic(UserId, HistoryId, TabelNameId, dynamicAttLibValueEntities);
            _context.SaveChanges();
            Task.Run(() => RefreshView(connectionString));
        }
        public void AddDdynamicAttributeInstallationsHInternal(int? UserId, List<AddDdynamicAttributeInstallationValueViewModel> addDynamicInstAttValue, int TableNameId, int Id, string connectionString, int HistoryId)
        {
            var TabelName = _context.TLItablesNames.FirstOrDefault(x => x.Id == TableNameId)?.TableName;
            var TabelNameId = _context.TLItablesNames.FirstOrDefault(x => x.TableName == "TLIdynamicAttInstValue").Id;
            var dynamicAttLibValueEntities = addDynamicInstAttValue.Select(DynamicLibAttValue =>
            {
                var DynamicAtt = _context.TLIdynamicAtt
                   .Where(x => x.Id == DynamicLibAttValue.id)
                   .Include(x => x.DataType).Include(x => x.tablesNames)
                   .FirstOrDefault();
                TLIdynamicAttInstValue dynamicAttInstValue = new TLIdynamicAttInstValue();
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamic value = DynamicLibAttValue.value?.ToString();
                if (value != null)
                {
                    string dataType = DynamicAtt.DataType.Name.ToLower();

                    switch (dataType)
                    {
                        case "bool":
                            bool boolValue;
                            if (bool.TryParse(value, out boolValue))
                            {
                                dynamicAttInstValue.ValueBoolean = boolValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDouble = null;

                                throw new ArgumentException("Invalid boolean value.");
                            }
                            break;
                        case "datetime":
                            DateTime dateTimeValue;
                            if (DateTime.TryParse(value, out dateTimeValue))
                            {
                                dynamicAttInstValue.ValueDateTime = dateTimeValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDateTime = null;

                                throw new ArgumentException("Invalid datetime value.");
                            }
                            break;
                        case "double":
                            double doubleValue;
                            if (double.TryParse(value, out doubleValue))
                            {
                                dynamicAttInstValue.ValueDouble = doubleValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDouble = null;

                                throw new ArgumentException("Invalid double value.");
                            }
                            break;
                        case "int":
                            int intValue;
                            if (int.TryParse(value, out intValue))
                            {
                                dynamicAttInstValue.ValueDouble = intValue;
                            }
                            else
                            {
                                dynamicAttInstValue.ValueDouble = null;

                                throw new ArgumentException("Invalid int value.");
                            }
                            break;
                        case "string":
                            dynamicAttInstValue.ValueString = value;
                            break;
                        default:

                            break;
                    }
                }
                dynamicAttInstValue.disable = false;
                return dynamicAttInstValue;
            }).ToList();

            AddRangeWithHDynamic(UserId, HistoryId, TabelNameId, dynamicAttLibValueEntities);
            _context.SaveChanges();
            Task.Run(() => RefreshView(connectionString));
        }
        public List<BaseInstAttViewDynamic> GetDynamicInstAtt(int TableNameId, int Id, int? CategoryId = null)
        {
            List<BaseInstAttViewDynamic> dynamicAttInstViewModels = new List<BaseInstAttViewDynamic>();
            List<TLIdynamicAtt> DynamicAtts = new List<TLIdynamicAtt>();

            if (CategoryId == null)
            {
                DynamicAtts = _context.TLIdynamicAtt.Include(x => x.DataType)
                    .Where(x => !x.LibraryAtt && x.tablesNamesId == TableNameId && !x.disable).ToList();
            }
            else
            {
                DynamicAtts = _context.TLIdynamicAtt.Include(x => x.tablesNames).Include(x => x.DataType)
                    .Where(x => x.CivilWithoutLegCategoryId == CategoryId && !x.LibraryAtt &&
                        x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString().ToLower() &&
                        !x.disable).ToList();
            }
            foreach (TLIdynamicAtt DynamicAtt in DynamicAtts)
            {
                TLIdynamicAttInstValue DynamicInstAtt = _context.TLIdynamicAttInstValue
                    .FirstOrDefault(x => x.DynamicAttId == DynamicAtt.Id &&
                        x.tablesNamesId == TableNameId && x.InventoryId == Id);

                if (DynamicAtt != null)
                {
                  

                    dynamic value = null;
                    if (DynamicInstAtt == null)
                    {


                        value = DynamicAtt.DataTypeId switch
                        {
                            1 => DynamicAtt.DefaultValue.ToString(),
                            21 or 22 => Convert.ToDouble(DynamicAtt.DefaultValue),
                            24 => Convert.ToBoolean(DynamicAtt.DefaultValue),
                            25 => Convert.ToDateTime(DynamicAtt.DefaultValue),
                            _ => null
                        };


                    }
                    else
                    {
                        if (DynamicInstAtt.ValueString != null)
                        {
                            value = DynamicInstAtt.ValueString;
                        }
                        else if (DynamicInstAtt.ValueBoolean != null)
                        {
                            value = DynamicInstAtt.ValueBoolean;
                        }
                        else if (DynamicInstAtt.ValueDateTime != null)
                        {
                            value = DynamicInstAtt.ValueDateTime;
                        }
                        else if (DynamicInstAtt.ValueDouble != null)
                        {
                            value = DynamicInstAtt.ValueDouble;
                        }
                    }

                    dynamicAttInstViewModels.Add(new BaseInstAttViewDynamic
                    {
                        Id = DynamicAtt.Id,
                        Key = DynamicAtt.Key,
                        DataTypeId = DynamicAtt.DataTypeId,
                        DataType = DynamicAtt.DataType.Name,
                        Value = value, 
                        Required = DynamicAtt.Required,
                        Label= DynamicAtt.Key,
                        Desc= DynamicAtt.Description
                    });
                }

            }
            return dynamicAttInstViewModels;
        }
        public void UpdateDynamicValue(List<BaseInstAttView> DynamicInstAttsValue, int TableNameId, int InstId )
        {
            foreach (var DynamicIns in DynamicInstAttsValue)
            {
                if (DynamicIns.Value != null)
                {
                    var DynamicAttInstValue = _context.TLIdynamicAttInstValue.Where(x => x.DynamicAttId == DynamicIns.Id && x.InventoryId == InstId && x.DynamicAtt.Key == DynamicIns.Key && x.tablesNamesId == TableNameId).FirstOrDefault();
                    if (DynamicAttInstValue != null)
                    {
                        if (DynamicAttInstValue.ValueString != null && DynamicAttInstValue.ValueString != "")
                        {
                            string tests = DynamicIns.Value.ToString().Trim();
                            if (tests != "")
                            DynamicAttInstValue.ValueString = tests;
                        }
                        else if (DynamicAttInstValue.ValueDouble != null)
                        {
                            DynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.Value.ToString());
                        }
                        else if (DynamicAttInstValue.ValueDateTime != null)
                        {
                            DynamicAttInstValue.ValueDateTime = DateTime.Parse(DynamicIns.Value.ToString());
                        }
                        else if (DynamicAttInstValue.ValueBoolean != null)
                        {
                            DynamicAttInstValue.ValueBoolean = bool.Parse(DynamicIns.Value.ToString());
                        }
                        _context.SaveChanges();
                    }
                    else if (DynamicAttInstValue == null)
                    {
                        TLIdynamicAttInstValue dynamicAttInstValue = new TLIdynamicAttInstValue();
                        var datatype = _context.TLIdataType.Where(x => x.Id == DynamicIns.DataTypeId).Select(x => x.Name).FirstOrDefault();

                        if (datatype.ToLower() == "string")
                        {
                            string test = DynamicIns.Value.ToString().Trim();
                            if (test != "" )
                            {
                                dynamicAttInstValue.ValueString = test;
                            }
                            // test.Trim().ToString();
                            //dynamicAttInstValue.Value = test.ToString();
                        }
                        else if (datatype.ToLower() == "double")
                        {
                            dynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.Value.ToString());
                        }
                        else if (datatype.ToLower() == "int")
                        {
                            dynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.Value.ToString());
                        }
                        else if (datatype.ToLower() == "datetime")
                        {
                            dynamicAttInstValue.ValueDateTime = DateTime.Parse(DynamicIns.Value.ToString());
                        }
                        else if (datatype.ToLower() == "boolean")
                        {
                            dynamicAttInstValue.ValueBoolean = bool.Parse(DynamicIns.Value.ToString());
                        }
                        dynamicAttInstValue.DynamicAttId = DynamicIns.Id;
                        dynamicAttInstValue.tablesNamesId = TableNameId;
                        dynamicAttInstValue.InventoryId = InstId;
                        _context.TLIdynamicAttInstValue.Add(dynamicAttInstValue);
                        _context.SaveChanges();
                    }
                }                               
            }
           
        }
        public void UpdateDynamicValues(int UserId,List<AddDdynamicAttributeInstallationValueViewModel> DynamicInstAttsValue, int TableNameId, int InstId,string connectionString)
        {
            foreach (var DynamicIns in DynamicInstAttsValue)
            {
                if (DynamicIns.value != null)
                {
                    var DynamicAttInstValue = _context.TLIdynamicAttInstValue
                        .Where(x => x.DynamicAttId == DynamicIns.id && x.InventoryId == InstId  && x.tablesNamesId == TableNameId)
                        .Include(x=>x.DynamicAtt).ThenInclude(x=>x.tablesNames).FirstOrDefault();
                    if (DynamicAttInstValue != null)
                    {
                        if (DynamicAttInstValue.ValueString != null && DynamicAttInstValue.ValueString != "")
                        {
                            string tests = DynamicIns.value.ToString().Trim();
                            if (tests != "")
                                DynamicAttInstValue.ValueString = tests;
                        }
                        else if (DynamicAttInstValue.ValueDouble != null)
                        {
                            DynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.value.ToString());
                        }
                        else if (DynamicAttInstValue.ValueDateTime != null)
                        {
                            DynamicAttInstValue.ValueDateTime = DateTime.Parse(DynamicIns.value.ToString());
                        }
                        else if (DynamicAttInstValue.ValueBoolean != null)
                        {
                            DynamicAttInstValue.ValueBoolean = bool.Parse(DynamicIns.value.ToString());
                        }
                        _context.SaveChanges();
                        Task.Run(() => RefreshView(connectionString));
                    }
                    else if (DynamicAttInstValue == null)
                    {
                        TLIdynamicAttInstValue dynamicAttInstValue = new TLIdynamicAttInstValue();
                        var datatypes = _context.TLIdynamicAtt.FirstOrDefault(x => x.Id == DynamicIns.id);
                        var datatype = _context.TLIdataType.Where(x => x.Id == datatypes.DataTypeId).Select(x => x.Name).FirstOrDefault();

                        if (datatype.ToLower() == "string")
                        {
                            string test = DynamicIns.value.ToString().Trim();
                            if (test != "")
                            {
                                dynamicAttInstValue.ValueString = test;
                            }
                            // test.Trim().ToString();
                            //dynamicAttInstValue.Value = test.ToString();
                        }
                        else if (datatype.ToLower() == "double")
                        {
                            dynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.value.ToString());
                        }
                        else if (datatype.ToLower() == "int")
                        {
                            dynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.value.ToString());
                        }
                        else if (datatype.ToLower() == "datetime")
                        {
                            dynamicAttInstValue.ValueDateTime = DateTime.Parse(DynamicIns.value.ToString());
                        }
                        else if (datatype.ToLower() == "boolean")
                        {
                            dynamicAttInstValue.ValueBoolean = bool.Parse(DynamicIns.value.ToString());
                        }
                        dynamicAttInstValue.DynamicAttId = DynamicIns.id;
                        dynamicAttInstValue.tablesNamesId = TableNameId;
                        dynamicAttInstValue.tablesNames = _context.TLItablesNames.FirstOrDefault(x=>x.Id== TableNameId);
                        dynamicAttInstValue.tablesNames.TableName = _context.TLItablesNames.FirstOrDefault(x=>x.Id== TableNameId)?.TableName;
                        dynamicAttInstValue.InventoryId = InstId;
                        AddWithHistorys(UserId,dynamicAttInstValue);
                        _context.SaveChanges();

                        Task.Run(() => RefreshView(connectionString));
                    }


                }
            }

        }
        public void UpdateDynamicValuesH(int UserId, List<AddDdynamicAttributeInstallationValueViewModel> DynamicInstAttsValue, int TableNameId, int InstId, string connectionString,int HistoryId)
        {
            foreach (var DynamicIns in DynamicInstAttsValue)
            {
                if (DynamicIns.value != null)
                {
                    var DynamicAttInstValue = _context.TLIdynamicAttInstValue
                        .Where(x => x.DynamicAttId == DynamicIns.id && x.InventoryId == InstId && x.tablesNamesId == TableNameId)
                        .Include(x => x.DynamicAtt).ThenInclude(x => x.tablesNames).FirstOrDefault();
                    var tablesNames = _context.TLItablesNames.FirstOrDefault(x => x.TableName == "TLIdynamicAttInstValue").Id;
                    if (DynamicAttInstValue != null)
                    {

                        var Old = DynamicAttInstValue.Value;

                        var New = DynamicIns.value.ToString();
                        if (DynamicAttInstValue.ValueString != null && DynamicAttInstValue.ValueString != "")
                        {
                            string tests = DynamicIns.value.ToString().Trim();
                            if (tests != "")
                                DynamicAttInstValue.ValueString = tests;
                        }
                        else if (DynamicAttInstValue.ValueDouble != null)
                        {
                            DynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.value.ToString());
                        }
                        else if (DynamicAttInstValue.ValueDateTime != null)
                        {
                            DynamicAttInstValue.ValueDateTime = DateTime.Parse(DynamicIns.value.ToString());
                        }
                        else if (DynamicAttInstValue.ValueBoolean != null)
                        {
                            DynamicAttInstValue.ValueBoolean = bool.Parse(DynamicIns.value.ToString());
                        }

                        TLIhistoryDet historyDetails = new TLIhistoryDet();
                        var AttName = _context.TLIdynamicAtt.Where(x => x.Id == DynamicAttInstValue.DynamicAttId).Select(x => x.Key).FirstOrDefault();
                        historyDetails.AttributeName = AttName;
                        historyDetails.TablesNameId = tablesNames;
                        historyDetails.OldValue = Old;
                        historyDetails.NewValue = New.ToString();
                        historyDetails.HistoryId = HistoryId;
                        historyDetails.AttributeType = AttributeType.Dynamic;
                        historyDetails.RecordId = DynamicAttInstValue.Id.ToString();
                        _context.TLIhistoryDet.Add(historyDetails);
                        _context.SaveChanges();
                        Task.Run(() => RefreshView(connectionString));
                    }
                    else if (DynamicAttInstValue == null)
                    {
                        TLIdynamicAttInstValue dynamicAttInstValue = new TLIdynamicAttInstValue();
                        var datatypes = _context.TLIdynamicAtt.FirstOrDefault(x => x.Id == DynamicIns.id);
                        var datatype = _context.TLIdataType.Where(x => x.Id == datatypes.DataTypeId).Select(x => x.Name).FirstOrDefault();

                        if (datatype.ToLower() == "string")
                        {
                            string test = DynamicIns.value.ToString().Trim();
                            if (test != "")
                            {
                                dynamicAttInstValue.ValueString = test;
                            }
                            // test.Trim().ToString();
                            //dynamicAttInstValue.Value = test.ToString();
                        }
                        else if (datatype.ToLower() == "double")
                        {
                            dynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.value.ToString());
                        }
                        else if (datatype.ToLower() == "int")
                        {
                            dynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.value.ToString());
                        }
                        else if (datatype.ToLower() == "datetime")
                        {
                            dynamicAttInstValue.ValueDateTime = DateTime.Parse(DynamicIns.value.ToString());
                        }
                        else if (datatype.ToLower() == "boolean")
                        {
                            dynamicAttInstValue.ValueBoolean = bool.Parse(DynamicIns.value.ToString());
                        }
                        dynamicAttInstValue.DynamicAttId = DynamicIns.id;
                        dynamicAttInstValue.tablesNamesId = TableNameId;
                        dynamicAttInstValue.tablesNames = _context.TLItablesNames.FirstOrDefault(x => x.Id == TableNameId);
                        dynamicAttInstValue.tablesNames.TableName = _context.TLItablesNames.FirstOrDefault(x => x.Id == TableNameId)?.TableName;
                        dynamicAttInstValue.InventoryId = InstId;
                        
                        AddWithHDynamic(UserId, tablesNames, dynamicAttInstValue, HistoryId);
                        _context.SaveChanges();

                        Task.Run(() => RefreshView(connectionString));
                    }


                }
            }

        }
        public void UpdateDynamicValuesHInternal(int? UserId, List<AddDdynamicAttributeInstallationValueViewModel> DynamicInstAttsValue, int TableNameId, int InstId, string connectionString, int HistoryId)
        {
            foreach (var DynamicIns in DynamicInstAttsValue)
            {
                if (DynamicIns.value != null)
                {
                    var DynamicAttInstValue = _context.TLIdynamicAttInstValue
                        .Where(x => x.DynamicAttId == DynamicIns.id && x.InventoryId == InstId && x.tablesNamesId == TableNameId)
                        .Include(x => x.DynamicAtt).ThenInclude(x => x.tablesNames).FirstOrDefault();
                    var tablesNames = _context.TLItablesNames.FirstOrDefault(x => x.TableName == "TLIdynamicAttInstValue").Id;
                    if (DynamicAttInstValue != null)
                    {

                        var Old = DynamicAttInstValue.Value;

                        var New = DynamicIns.value.ToString();
                        if (DynamicAttInstValue.ValueString != null && DynamicAttInstValue.ValueString != "")
                        {
                            string tests = DynamicIns.value.ToString().Trim();
                            if (tests != "")
                                DynamicAttInstValue.ValueString = tests;
                        }
                        else if (DynamicAttInstValue.ValueDouble != null)
                        {
                            DynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.value.ToString());
                        }
                        else if (DynamicAttInstValue.ValueDateTime != null)
                        {
                            DynamicAttInstValue.ValueDateTime = DateTime.Parse(DynamicIns.value.ToString());
                        }
                        else if (DynamicAttInstValue.ValueBoolean != null)
                        {
                            DynamicAttInstValue.ValueBoolean = bool.Parse(DynamicIns.value.ToString());
                        }

                        TLIhistoryDet historyDetails = new TLIhistoryDet();
                        var AttName = _context.TLIdynamicAtt.Where(x => x.Id == DynamicAttInstValue.DynamicAttId).Select(x => x.Key).FirstOrDefault();
                        historyDetails.AttributeName = AttName;
                        historyDetails.TablesNameId = tablesNames;
                        historyDetails.OldValue = Old;
                        historyDetails.NewValue = New.ToString();
                        historyDetails.HistoryId = HistoryId;
                        historyDetails.AttributeType = AttributeType.Dynamic;
                        historyDetails.RecordId = DynamicAttInstValue.Id.ToString();
                        _context.TLIhistoryDet.Add(historyDetails);
                        _context.SaveChanges();
                        Task.Run(() => RefreshView(connectionString));
                    }
                    else if (DynamicAttInstValue == null)
                    {
                        TLIdynamicAttInstValue dynamicAttInstValue = new TLIdynamicAttInstValue();
                        var datatypes = _context.TLIdynamicAtt.FirstOrDefault(x => x.Id == DynamicIns.id);
                        var datatype = _context.TLIdataType.Where(x => x.Id == datatypes.DataTypeId).Select(x => x.Name).FirstOrDefault();

                        if (datatype.ToLower() == "string")
                        {
                            string test = DynamicIns.value.ToString().Trim();
                            if (test != "")
                            {
                                dynamicAttInstValue.ValueString = test;
                            }
                            // test.Trim().ToString();
                            //dynamicAttInstValue.Value = test.ToString();
                        }
                        else if (datatype.ToLower() == "double")
                        {
                            dynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.value.ToString());
                        }
                        else if (datatype.ToLower() == "int")
                        {
                            dynamicAttInstValue.ValueDouble = double.Parse(DynamicIns.value.ToString());
                        }
                        else if (datatype.ToLower() == "datetime")
                        {
                            dynamicAttInstValue.ValueDateTime = DateTime.Parse(DynamicIns.value.ToString());
                        }
                        else if (datatype.ToLower() == "boolean")
                        {
                            dynamicAttInstValue.ValueBoolean = bool.Parse(DynamicIns.value.ToString());
                        }
                        dynamicAttInstValue.DynamicAttId = DynamicIns.id;
                        dynamicAttInstValue.tablesNamesId = TableNameId;
                        dynamicAttInstValue.tablesNames = _context.TLItablesNames.FirstOrDefault(x => x.Id == TableNameId);
                        dynamicAttInstValue.tablesNames.TableName = _context.TLItablesNames.FirstOrDefault(x => x.Id == TableNameId)?.TableName;
                        dynamicAttInstValue.InventoryId = InstId;

                        AddWithHDynamic(UserId, tablesNames, dynamicAttInstValue, HistoryId);
                        _context.SaveChanges();

                        Task.Run(() => RefreshView(connectionString));
                    }


                }
            }

        }
        public void RefreshView(string connectionString)
        {
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    string storedProcedureName = "refresh_all_mviews";
                    using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    {
                        procedureCommand.CommandType = CommandType.StoredProcedure;
                        procedureCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during refreshing view: {ex.Message}");
            }
        }
        public virtual void AddWithHistorys(int? UserId, TLIdynamicAttInstValue entity)
        {
  
            _context.TLIdynamicAttInstValue.Add(entity);
            _context.SaveChanges();
            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == entity.GetType().Name.ToLower());

                int HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x =>
                    x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Add.ToString().ToLower()).Id;

                int entityId = (int)entity.GetType().GetProperty("Id").GetValue(entity, null);

                TLItablesHistory AddTablesHistory = new TLItablesHistory
                {
                    Date = DateTime.Now,
                    HistoryTypeId = HistoryTypeId,
                    PreviousHistoryId = null,
                    RecordId = entityId.ToString(),
                    TablesNameId = EntityTableNameModel.Id,
                    UserId = UserId.Value
                };
                _context.TLItablesHistory.Add(AddTablesHistory);
                _context.SaveChanges();
            }
        }
    }
}
