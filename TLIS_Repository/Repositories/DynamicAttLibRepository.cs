using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using static Dapper.SqlMapper;

namespace TLIS_Repository.Repositories
{
    public class DynamicAttLibRepository : RepositoryBase<TLIdynamicAttLibValue, DynamicAttLibViewModel, int>, IDynamicAttLibRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;
        public DynamicAttLibRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddDynamicLibAtts(List<AddDynamicLibAttValueViewModel> addDynamicLibAttValues, int TableNameId, int Id)
        {
            string tests;
            bool test = false;
            foreach (var DynamicLibAttValue in addDynamicLibAttValues)
            {
                test = false;
                var ValidationEntity = _context.TLIvalidation
                    .Where(x => x.DynamicAttId == DynamicLibAttValue.DynamicAttId)
                    .Include(x => x.Operation)
                    .FirstOrDefault();
                var DynamicAttEntity = _context.TLIdynamicAtt
                    .Where(x => x.Id == DynamicLibAttValue.DynamicAttId)
                    .Include(x => x.DataType)
                    .FirstOrDefault();
                var dynamicAttLibValueEntites = _mapper.Map<TLIdynamicAttLibValue>(DynamicLibAttValue);
                dynamicAttLibValueEntites.InventoryId = Id;
                dynamicAttLibValueEntites.tablesNamesId = TableNameId;
                if (DynamicLibAttValue.ValueBoolean != null)
                {
                    dynamicAttLibValueEntites.Value = DynamicLibAttValue.ValueBoolean.ToString();
                }
                else if (dynamicAttLibValueEntites.ValueString != null && !string.IsNullOrEmpty(dynamicAttLibValueEntites.ValueString) && !dynamicAttLibValueEntites.ValueString.All(x => x == ' '))
                {
                    tests = dynamicAttLibValueEntites.ValueString.Trim();

                    dynamicAttLibValueEntites.Value = tests.ToString();
                    dynamicAttLibValueEntites.ValueString = tests.ToString();
                }
                else if (dynamicAttLibValueEntites.ValueDateTime != null)
                {
                    dynamicAttLibValueEntites.Value = DynamicLibAttValue.ValueDateTime.ToString();
                }
                else if (dynamicAttLibValueEntites.ValueDouble != null)
                {
                    dynamicAttLibValueEntites.Value = DynamicLibAttValue.ValueDouble.ToString();
                }
                else
                {
                    continue;
                }

                _context.TLIdynamicAttLibValue.Add(dynamicAttLibValueEntites);
                _context.SaveChanges();
            }
        }
        public void AddDynamicLibAtt(int UserId, List<AddDdynamicAttributeInstallationValueViewModel> addDynamicLibAttValues, int TableNameId, int Id, string connectionString)
        {
            var TablesName = _context.TLItablesNames.FirstOrDefault(x => x.Id == TableNameId)?.TableName;
            var dynamicAttLibValueEntities = addDynamicLibAttValues.Select(DynamicLibAttValue =>
            {
                var dynamicAttEntity = _context.TLIdynamicAtt
                    .Where(x => x.Id == DynamicLibAttValue.id)
                    .Include(x => x.DataType)
                    .FirstOrDefault();
                TLIdynamicAttLibValue dynamicAttLibValueEntity = new TLIdynamicAttLibValue();
                dynamicAttLibValueEntity.InventoryId = Id;
                dynamicAttLibValueEntity.tablesNamesId = TableNameId;
                dynamicAttLibValueEntity.DynamicAttId = dynamicAttEntity.Id;
                dynamic value = DynamicLibAttValue.value.ToString();
                if (value != null)
                {
                    string dataType = dynamicAttEntity.DataType.Name.ToLower();

                    switch (dataType)
                    {
                        case "bool":
                            bool boolValue;
                            if (bool.TryParse(value, out boolValue))
                            {
                                dynamicAttLibValueEntity.ValueBoolean = boolValue;
                            }
                            else
                            {
                                dynamicAttLibValueEntity.ValueDouble = null;

                                throw new ArgumentException("Invalid boolean value.");
                            }
                            break;
                        case "datetime":
                            DateTime dateTimeValue;
                            if (DateTime.TryParse(value, out dateTimeValue))
                            {
                                dynamicAttLibValueEntity.ValueDateTime = dateTimeValue;
                            }
                            else
                            {
                                dynamicAttLibValueEntity.ValueDateTime = null;

                                throw new ArgumentException("Invalid datetime value.");
                            }
                            break;
                        case "double":
                            double doubleValue;
                            if (double.TryParse(value, out doubleValue))
                            {
                                dynamicAttLibValueEntity.ValueDouble = doubleValue;
                            }
                            else
                            {
                                dynamicAttLibValueEntity.ValueDouble = null;

                                throw new ArgumentException("Invalid double value.");
                            }
                            break;
                        case "int":
                            int intValue;
                            if (int.TryParse(value, out intValue))
                            {
                                dynamicAttLibValueEntity.ValueDouble = intValue;
                            }
                            else
                            {
                                dynamicAttLibValueEntity.ValueDouble = null;

                                throw new ArgumentException("Invalid int value.");
                            }
                            break;
                        case "string":
                            dynamicAttLibValueEntity.ValueString = value;
                            break;
                        default:

                            break;
                    }
                }

                dynamicAttLibValueEntity.disable = false;
                return dynamicAttLibValueEntity;
            }).ToList();

            AddRangeWithHistory(UserId, dynamicAttLibValueEntities);
            _context.SaveChanges();
            if (TablesName == "TLIcivilWithLegs")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIcivilWithoutLeg")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIcivilNonSteel")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIcivilNonSteelLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIcivilWithLegLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIcivilWithoutLegLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIsideArm")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIsideArmLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIradioAntenna")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            //else if (TablesName == "TLIradioOther")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIradioRRU")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIradioOtherLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            else if (TablesName == "TLIradioAntennaLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            //else if (TablesName == "TLIradioRRULibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLImwBU")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLImwRFU")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            else if (TablesName == "TLImwDish")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLImwODU")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            //else if (TablesName == "TLImwOther")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLImwBULibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLImwRFULibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            else if (TablesName == "TLImwDishLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLImwODULibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            //else if (TablesName == "TLImwOtherLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIpower")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIpowerLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIloadOther")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIloadOtherLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIcabinet")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIsolar")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIgenerator")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIsolarLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIgeneratorLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIcabinetPowerLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIcabinetTelecomLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
        }
        public void AddDynamicLibraryAtt(int UserId, List<AddDdynamicAttributeInstallationValueViewModel> addDynamicLibAttValues, int? RecordId, int TableNameId, int Id, string connectionString,int HistoryId)
        {
            var TablesName = _context.TLItablesNames.FirstOrDefault(x => x.Id == TableNameId)?.TableName;
            var dynamicAttLibValueEntities = addDynamicLibAttValues.Select(DynamicLibAttValue =>
            {
                var dynamicAttEntity = _context.TLIdynamicAtt
                    .Where(x => x.Id == DynamicLibAttValue.id)
                    .Include(x => x.DataType)
                    .FirstOrDefault();
                TLIdynamicAttLibValue dynamicAttLibValueEntity = new TLIdynamicAttLibValue();
                dynamicAttLibValueEntity.InventoryId = Id;
                dynamicAttLibValueEntity.tablesNamesId = TableNameId;
                dynamicAttLibValueEntity.DynamicAttId = dynamicAttEntity.Id;
                dynamic value = DynamicLibAttValue.value.ToString();
                if (value != null)
                {
                    string dataType = dynamicAttEntity.DataType.Name.ToLower();

                    switch (dataType)
                    {
                        case "bool":
                            bool boolValue;
                            if (bool.TryParse(value, out boolValue))
                            {
                                dynamicAttLibValueEntity.ValueBoolean = boolValue;
                            }
                            else
                            {
                                dynamicAttLibValueEntity.ValueDouble = null;

                                throw new ArgumentException("Invalid boolean value.");
                            }
                            break;
                        case "datetime":
                            DateTime dateTimeValue;
                            if (DateTime.TryParse(value, out dateTimeValue))
                            {
                                dynamicAttLibValueEntity.ValueDateTime = dateTimeValue;
                            }
                            else
                            {
                                dynamicAttLibValueEntity.ValueDateTime = null;

                                throw new ArgumentException("Invalid datetime value.");
                            }
                            break;
                        case "double":
                            double doubleValue;
                            if (double.TryParse(value, out doubleValue))
                            {
                                dynamicAttLibValueEntity.ValueDouble = doubleValue;
                            }
                            else
                            {
                                dynamicAttLibValueEntity.ValueDouble = null;

                                throw new ArgumentException("Invalid double value.");
                            }
                            break;
                        case "int":
                            int intValue;
                            if (int.TryParse(value, out intValue))
                            {
                                dynamicAttLibValueEntity.ValueDouble = intValue;
                            }
                            else
                            {
                                dynamicAttLibValueEntity.ValueDouble = null;

                                throw new ArgumentException("Invalid int value.");
                            }
                            break;
                        case "string":
                            dynamicAttLibValueEntity.ValueString = value;
                            break;
                        default:

                            break;
                    }
                }

                dynamicAttLibValueEntity.disable = false;
                return dynamicAttLibValueEntity;
            }).ToList();

            AddRangeWithHDynamic(UserId, HistoryId, TableNameId, RecordId, dynamicAttLibValueEntities);
            _context.SaveChanges();
            if (TablesName == "TLIcivilWithLegs")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIcivilWithoutLeg")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIcivilNonSteel")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIcivilNonSteelLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIcivilWithLegLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIcivilWithoutLegLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIsideArm")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIsideArmLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLIradioAntenna")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            //else if (TablesName == "TLIradioOther")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIradioRRU")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIradioOtherLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            else if (TablesName == "TLIradioAntennaLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            //else if (TablesName == "TLIradioRRULibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLImwBU")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLImwRFU")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            else if (TablesName == "TLImwDish")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLImwODU")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            //else if (TablesName == "TLImwOther")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLImwBULibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLImwRFULibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            else if (TablesName == "TLImwDishLibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            else if (TablesName == "TLImwODULibrary")
            {
                Task.Run(() => RefreshView(connectionString));
            }
            //else if (TablesName == "TLImwOtherLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIpower")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIpowerLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIloadOther")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIloadOtherLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIcabinet")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIsolar")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIgenerator")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIsolarLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIgeneratorLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIcabinetPowerLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
            //else if (TablesName == "TLIcabinetTelecomLibrary")
            //{
            //    Task.Run(() => RefreshView(connectionString, "MV_CIVIL_WITHLEGS_VIEW"));
            //}
        }
        public void AddRangeWithHistory(int? UserId, IEnumerable<TLIdynamicAttLibValue> Entities)
        {
            foreach (TLIdynamicAttLibValue Entity in Entities)
            {
                AddWithHistory(UserId, Entity);
            }
            _context.SaveChanges();
        }
        public void DisableDynamicAttLibValues(int TableNameId, int Id)
        {
            var DynamiAttLibValues = _context.TLIdynamicAttLibValue
                .Where(d => d.InventoryId == Id && d.tablesNamesId == TableNameId)
                .ToList();
            foreach (var DynamiAttLibValue in DynamiAttLibValues)
            {
                DynamiAttLibValue.disable = true;
                _context.TLIdynamicAttLibValue.Update(DynamiAttLibValue);
            }
        }

        public List<DynamicAttLibViewModel> GetDynamicLibAtts(int TableNameId, int Id, int? CategoryId)
        {

            List<TLIdynamicAtt> DynamicAtts = new List<TLIdynamicAtt>();
            List<DynamicAttLibViewModel> dynamicAttLibViewModels = new List<DynamicAttLibViewModel>();
            if (CategoryId == null)
            {
                DynamicAtts = _context.TLIdynamicAtt
                    .Where(d => d.LibraryAtt == true && d.tablesNamesId == TableNameId && !d.disable)
                    .Include(x => x.DataType)
                    .ToList();
            }
            else
            {
                DynamicAtts = _context.TLIdynamicAtt
                    .Where(d => !d.disable && d.LibraryAtt == true && d.tablesNamesId == TableNameId && d.CivilWithoutLegCategoryId == CategoryId)
                    .Include(x => x.DataType)
                    .ToList();
            }
            List<int> ids = DynamicAtts.Select(x => x.Id).ToList();
            foreach (var DynamicAtt in DynamicAtts)
            {
                TLIdynamicAttLibValue DynamicAttValue = _context.TLIdynamicAttLibValue.FirstOrDefault(d =>
                    d.DynamicAttId == DynamicAtt.Id && d.InventoryId == Id && d.tablesNamesId == TableNameId && !d.disable);

                DynamicAttLibViewModel NewDynamicLibraryValue = new DynamicAttLibViewModel
                {
                    Id = DynamicAtt.Id,
                    Key = DynamicAtt.Key,
                    DataTypeId = DynamicAtt.DataTypeId,
                    DataType = DynamicAtt.DataType.Name,
                    Required = DynamicAtt.Required,
                    disable = DynamicAtt.disable
                };

                if (DynamicAttValue != null)
                {
                    if (!string.IsNullOrEmpty(DynamicAttValue.ValueString))
                        NewDynamicLibraryValue.Value = DynamicAttValue.ValueString;

                    else if (DynamicAttValue.ValueDateTime != null)
                        NewDynamicLibraryValue.Value = DynamicAttValue.ValueDateTime.ToString();

                    else if (DynamicAttValue.ValueDouble != null)
                        NewDynamicLibraryValue.Value = DynamicAttValue.ValueDouble.ToString();

                    else if (DynamicAttValue.ValueBoolean != null)
                        NewDynamicLibraryValue.Value = DynamicAttValue.ValueBoolean.ToString().ToLower();
                }
                else
                {
                    NewDynamicLibraryValue.Value = null;
                }

                dynamicAttLibViewModels.Add(NewDynamicLibraryValue);
            }
            return dynamicAttLibViewModels;
        }
        public List<BaseInstAttViewDynamic> GetDynamicLibAtt(int TableNameId, int Id, int? CategoryId)
        {

            List<TLIdynamicAtt> DynamicAtts = new List<TLIdynamicAtt>();
            List<BaseInstAttViewDynamic> dynamicAttLibViewModels = new List<BaseInstAttViewDynamic>();
            if (CategoryId == null)
            {
                DynamicAtts = _context.TLIdynamicAtt
                    .Where(d => d.LibraryAtt == true && d.tablesNamesId == TableNameId && !d.disable)
                    .Include(x => x.DataType)
                    .ToList();
            }
            else
            {
                DynamicAtts = _context.TLIdynamicAtt
                    .Where(d => !d.disable && d.LibraryAtt == true && d.tablesNamesId == TableNameId && d.CivilWithoutLegCategoryId == CategoryId)
                    .Include(x => x.DataType)
                    .ToList();
            }
            List<int> ids = DynamicAtts.Select(x => x.Id).ToList();
            foreach (var DynamicAtt in DynamicAtts)
            {
                TLIdynamicAttLibValue DynamicAttValue = _context.TLIdynamicAttLibValue.FirstOrDefault(d =>
                    d.DynamicAttId == DynamicAtt.Id && d.InventoryId == Id && d.tablesNamesId == TableNameId && !d.disable);

                BaseInstAttViewDynamic NewDynamicLibraryValue = new BaseInstAttViewDynamic
                {
                    Id = DynamicAtt.Id,
                    Key = DynamicAtt.Key,
                    DataTypeId = DynamicAtt.DataTypeId,
                    DataType = DynamicAtt.DataType.Name,
                    Label = DynamicAtt.Key,
                    Required = DynamicAtt.Required,
                    enable = !DynamicAtt.disable,
                    Options = new object[] { }

                };

                if (DynamicAttValue != null)
                {
                    if (!string.IsNullOrEmpty(DynamicAttValue.ValueString))
                        NewDynamicLibraryValue.Value = DynamicAttValue.ValueString;

                    else if (DynamicAttValue.ValueDateTime != null)
                        NewDynamicLibraryValue.Value = DynamicAttValue.ValueDateTime.ToString();

                    else if (DynamicAttValue.ValueDouble != null)
                        NewDynamicLibraryValue.Value = DynamicAttValue.ValueDouble.ToString();

                    else if (DynamicAttValue.ValueBoolean != null)
                        NewDynamicLibraryValue.Value = DynamicAttValue.ValueBoolean;
                }
                else
                {
                    NewDynamicLibraryValue.Value = null;
                }

                dynamicAttLibViewModels.Add(NewDynamicLibraryValue);
            }
            return dynamicAttLibViewModels;
        }
        //public void UpdateDynamicLibAtts(List<DynamicAttLibViewModel> DynamicLibAttValues, int TablesNameId, int LibId )
        //{

        //    foreach (var DynamicLibAttValue in DynamicLibAttValues)
        //    {
        //        TLIdynamicAttLibValue dynamicAttLibValue = _context.TLIdynamicAttLibValue
        //            .Where(d => d.DynamicAttId == DynamicLibAttValue.Id && d.tablesNamesId == TablesNameId && d.InventoryId == LibId)
        //            .FirstOrDefault();

        //        if (dynamicAttLibValue != null)
        //        {
        //            TLItablesHistory tablesHistory = new TLItablesHistory();
        //            int? TableHistoryId = null;
        //            tablesHistory.TablesNameId = TablesNameId;
        //            string HistoryTybeName = _context.TLIhistoryType.Where(x => x.Name == "Update").Select(x => x.Name).FirstOrDefault();
        //            tablesHistory.HistoryTypeId = _context.TLIhistoryType.Where(x => x.Name == "Update").Select(x => x.Id).FirstOrDefault();
        //            tablesHistory.RecordId = dynamicAttLibValue.Id;
        //            tablesHistory.UserId = 261;
        //            tablesHistory.Date = DateTime.Now;
        //            var CheckTableHistory = _context.TLItablesHistory.Any(x => x.HistoryType.Name == HistoryTybeName && x.RecordId == dynamicAttLibValue.Id && x.TablesNameId == TablesNameId);
        //            if (CheckTableHistory)
        //            {
        //                var TableHistory = _context.TLItablesHistory.Where(x => x.HistoryType.Name == HistoryTybeName && x.RecordId == dynamicAttLibValue.Id && x.TablesNameId == TablesNameId).Select(x => x.Id).Max();
        //                if (TableHistory != null)
        //                {
        //                    TableHistoryId = TableHistory;
        //                    if (TableHistoryId != null)
        //                    {
        //                        tablesHistory.PreviousHistoryId = TableHistoryId;
        //                    }
        //                }
        //            }
        //            _context.TLItablesHistory.Add(tablesHistory);
        //           // _context.SaveChanges();
        //            var x = dynamicAttLibValue.Value;  

        //            var y = DynamicLibAttValue.Value;


        //            if (x != y )
        //            {
        //                dynamicAttLibValue.Value = DynamicLibAttValue.Value;
        //                dynamicAttLibValue.disable = DynamicLibAttValue.disable;

        //                if (dynamicAttLibValue.ValueBoolean != null )
        //                {
        //                    dynamicAttLibValue.ValueBoolean = Boolean.Parse( DynamicLibAttValue.Value);
        //                }
        //                else if (dynamicAttLibValue.ValueDouble != null )
        //                {
        //                    dynamicAttLibValue.ValueDouble = double.Parse(DynamicLibAttValue.Value);
        //                }
        //                else if (dynamicAttLibValue.ValueString != null && dynamicAttLibValue.ValueString != "")
        //                {
        //                    dynamicAttLibValue.ValueString = (DynamicLibAttValue.Value);
        //                }
        //                else if (dynamicAttLibValue.ValueDateTime != null )
        //                {
        //                    dynamicAttLibValue.ValueDateTime =DateTime.Parse (DynamicLibAttValue.Value);
        //                }
        //                    _context.TLIdynamicAttLibValue.Update(dynamicAttLibValue);
        //                var CheckTableHistoryForEdit = _context.TLItablesHistory.Where(x => x.TablesNameId == TablesNameId).Select(x =>x.Id).ToList();
        //                int? tablehistoryid = null;
        //                if (CheckTableHistoryForEdit.Count!= 0)
        //                {
        //                    if (CheckTableHistoryForEdit.Count != 0)
        //                    {
        //                         tablehistoryid = CheckTableHistoryForEdit.Max();
        //                    }

        //                    TLIhistoryDetails historyDetails = new TLIhistoryDetails();
        //                    historyDetails.AttName = "Value";
        //                    historyDetails.TablesHistoryId = tablehistoryid;
        //                    historyDetails.OldValue = x;
        //                    historyDetails.NewValue = y;

        //                    _context.TLIhistoryDetails.Add(historyDetails);
        //                    _context.SaveChanges();
        //                }

        //            }


        //        }
        //        else
        //        {
        //            TLIdynamicAttLibValue dynamicAttLibValuenew = new TLIdynamicAttLibValue();
        //            dynamicAttLibValuenew.InventoryId = LibId;
        //            dynamicAttLibValuenew.DynamicAttId = DynamicLibAttValue.Id;
        //            dynamicAttLibValuenew.tablesNamesId = TablesNameId;
        //            dynamicAttLibValuenew.Value = DynamicLibAttValue.Value;
        //            if (DynamicLibAttValue.DataType == "string")
        //            {
        //                dynamicAttLibValuenew.ValueString = DynamicLibAttValue.Value;
        //            }
        //            else if (DynamicLibAttValue.DataType == "Int" || DynamicLibAttValue.DataType == "int")
        //            {
        //                dynamicAttLibValuenew.ValueDouble = double.Parse (DynamicLibAttValue.Value);
        //            }
        //            else if (DynamicLibAttValue.DataType == "Double" || DynamicLibAttValue.DataType == "double")
        //            {
        //                dynamicAttLibValuenew.ValueDouble = double.Parse(DynamicLibAttValue.Value);
        //            }
        //            else if (DynamicLibAttValue.DataType == "Boolean" || DynamicLibAttValue.DataType == "boolean")
        //            {
        //                dynamicAttLibValuenew.ValueBoolean = Boolean.Parse (DynamicLibAttValue.Value);
        //            }
        //            else if (DynamicLibAttValue.DataType == "DateTime" || DynamicLibAttValue.DataType == "datetime")
        //            {
        //                dynamicAttLibValue.ValueDateTime = DateTime.Parse(DynamicLibAttValue.Value);
        //            }


        //            _context.TLIdynamicAttLibValue.Add(dynamicAttLibValuenew);
        //        }

        //        //_context.SaveChanges();
        //    }
        //}

        public void UpdateDynamicLibAttsWithHistory(List<DynamicAttLibViewModel> DynamicLibAttValues, int TablesNameId, int LibId, int? UserId, int? TableHistoryId = null, int EntitesId = 0)
        {
            string AttName = "";
            string test;
            foreach (var DynamicLibAttValue in DynamicLibAttValues)
            {

                TLIdynamicAtt dynamicatt = _context.TLIdynamicAtt.FirstOrDefault(x => x.Id == DynamicLibAttValue.Id);
                /* dynamicatt.Key = DynamicLibAttValue.Key;
                 dynamicatt.DataTypeId = DynamicLibAttValue.DataTypeId;
                 dynamicatt.Required = DynamicLibAttValue.Required;
                 dynamicatt.disable = DynamicLibAttValue.disable;
                 _context.TLIdynamicAtt.Update(dynamicatt);*/


                TLIdynamicAttLibValue dynamicAttLibValue = _context.TLIdynamicAttLibValue
                    .Where(d => d.DynamicAttId == DynamicLibAttValue.Id && d.tablesNamesId == TablesNameId && d.InventoryId == LibId && d.DynamicAtt.Key == DynamicLibAttValue.Key)
                    .FirstOrDefault();

                if (dynamicAttLibValue != null)
                {
                    if (DynamicLibAttValue.Value == null)
                    {
                        continue;
                    }
                    var OldValue = dynamicAttLibValue.Value;

                    var NewValue = DynamicLibAttValue.Value;
                    if (OldValue != NewValue)
                    {
                        if (TableHistoryId == null || TableHistoryId == 0)
                        {
                            TLItablesHistory tablesHistory = new TLItablesHistory();
                            int? TablehistoryId = null;
                            tablesHistory.TablesNameId = TablesNameId;
                            string HistoryTybeName = _context.TLIhistoryType.Where(x => x.Name == "Update").Select(x => x.Name).FirstOrDefault();
                            tablesHistory.HistoryTypeId = _context.TLIhistoryType.Where(x => x.Name == "Update").Select(x => x.Id).FirstOrDefault();

                            tablesHistory.RecordId = EntitesId.ToString();
                            tablesHistory.UserId = UserId.Value;
                            tablesHistory.Date = DateTime.Now;
                            var CheckTableHistory = _context.TLItablesHistory.Any(x => x.HistoryType.Name == HistoryTybeName && x.RecordId == EntitesId.ToString() && x.TablesNameId == TablesNameId);
                            if (CheckTableHistory)
                            {
                                var TableHistory = _context.TLItablesHistory.Where(x => x.HistoryType.Name == HistoryTybeName && x.RecordId == EntitesId.ToString() && x.TablesNameId == TablesNameId).Select(x => x.Id).Max();

                                TablehistoryId = TableHistory;
                                if (TablehistoryId != null)
                                {
                                    tablesHistory.PreviousHistoryId = TablehistoryId;
                                }

                            }
                            _context.TLItablesHistory.Add(tablesHistory);
                            _context.SaveChanges();

                            dynamicAttLibValue.Value = NewValue.ToString();
                            if (dynamicAttLibValue.ValueString != null && dynamicAttLibValue.ValueString != "")
                            {
                                string tests = NewValue.ToString().Trim().ToString();
                                dynamicAttLibValue.ValueString = tests;
                                dynamicAttLibValue.Value = tests;
                            }
                            else if (dynamicAttLibValue.ValueDouble != null)
                            {
                                dynamicAttLibValue.ValueDouble = double.Parse(NewValue.ToString());
                            }
                            else if (dynamicAttLibValue.ValueDateTime != null)
                            {
                                dynamicAttLibValue.ValueDateTime = DateTime.Parse(NewValue.ToString());
                            }
                            else if (dynamicAttLibValue.ValueBoolean != null)
                            {
                                dynamicAttLibValue.ValueBoolean = bool.Parse(NewValue.ToString());
                            }

                            _context.TLIdynamicAttLibValue.Update(dynamicAttLibValue);
                            var CheckTableHistoryForEdit = _context.TLItablesHistory.Where(x => x.TablesNameId == TablesNameId).Select(x => x.Id).ToList();
                            int? tablehistoryid = null;
                            if (CheckTableHistoryForEdit.Count != 0)
                            {
                                if (CheckTableHistoryForEdit.Count != 0)
                                {
                                    tablehistoryid = CheckTableHistoryForEdit.Max();
                                }

                                TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                                AttName = _context.TLIdynamicAtt.Where(x => x.Id == dynamicAttLibValue.DynamicAttId).Select(x => x.Key).FirstOrDefault();
                                historyDetails.AttName = AttName;
                                historyDetails.TablesHistoryId = tablehistoryid;
                                if (OldValue == null)
                                {
                                    historyDetails.OldValue = null;
                                }
                                else
                                {
                                    historyDetails.OldValue = OldValue;
                                }
                                historyDetails.NewValue = NewValue.ToString();
                                historyDetails.AttributeType = AttributeType.Dynamic;
                                _context.TLIhistoryDetails.Add(historyDetails);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            var Old = dynamicAttLibValue.Value;

                            var New = DynamicLibAttValue.Value;


                            if (Old != New)
                            {
                                dynamicAttLibValue.Value = DynamicLibAttValue.Value.ToString();
                                if (dynamicAttLibValue.ValueString != null && dynamicAttLibValue.ValueString != "")
                                {
                                    string test1 = New.ToString().Trim().ToString();
                                    dynamicAttLibValue.ValueString = test1;
                                    dynamicAttLibValue.Value = test1;
                                }
                                else if (dynamicAttLibValue.ValueDouble != null)
                                {
                                    dynamicAttLibValue.ValueDouble = double.Parse(New.ToString());
                                }
                                else if (dynamicAttLibValue.ValueDateTime != null)
                                {
                                    dynamicAttLibValue.ValueDateTime = DateTime.Parse(New.ToString());
                                }
                                else if (dynamicAttLibValue.ValueBoolean != null)
                                {
                                    dynamicAttLibValue.ValueBoolean = bool.Parse(New.ToString());
                                }
                                _context.TLIdynamicAttLibValue.Update(dynamicAttLibValue);
                                var CheckTableHistoryForEdit = _context.TLItablesHistory.Where(x => x.TablesNameId == TablesNameId).Select(x => x.Id).ToList();
                                int? tablehistoryid = null;
                                if (CheckTableHistoryForEdit.Count != 0)
                                {
                                    if (CheckTableHistoryForEdit.Count != 0)
                                    {
                                        tablehistoryid = CheckTableHistoryForEdit.Max();
                                    }

                                    TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                                    AttName = _context.TLIdynamicAtt.Where(x => x.Id == dynamicAttLibValue.DynamicAttId).Select(x => x.Key).FirstOrDefault();
                                    historyDetails.AttName = AttName;
                                    historyDetails.TablesHistoryId = TableHistoryId;
                                    historyDetails.OldValue = Old;
                                    historyDetails.NewValue = New.ToString();
                                    historyDetails.AttributeType = AttributeType.Dynamic;
                                    _context.TLIhistoryDetails.Add(historyDetails);
                                    _context.SaveChanges();
                                }

                            }
                        }
                    }
                }
                // _context.SaveChanges();
                else
                {
                    if (DynamicLibAttValue.Value == null)
                    {
                        continue;
                    }
                    var Check = _context.TLIdynamicAtt.Where(x => x.Id == DynamicLibAttValue.Id).FirstOrDefault();
                    if (Check != null)
                    {
                        if (Check.tablesNamesId == TablesNameId && Check.LibraryAtt == true && Check.Key == DynamicLibAttValue.Key)
                        {
                            TLIdynamicAttLibValue dynamicAttLibValuenew = new TLIdynamicAttLibValue();
                            dynamicAttLibValuenew.InventoryId = LibId;
                            dynamicAttLibValuenew.DynamicAttId = DynamicLibAttValue.Id;
                            dynamicAttLibValuenew.tablesNamesId = TablesNameId;
                            dynamicAttLibValuenew.Value = DynamicLibAttValue.Value.ToString();
                            if (DynamicLibAttValue.DataType == "string")
                            {
                                test = DynamicLibAttValue.Value.ToString().Trim().ToString();
                                dynamicAttLibValuenew.ValueString = test;
                                dynamicAttLibValuenew.Value = test;
                            }
                            else if (DynamicLibAttValue.DataType == "Int" || DynamicLibAttValue.DataType == "int")
                            {
                                dynamicAttLibValuenew.ValueDouble = double.Parse(DynamicLibAttValue.Value.ToString());
                            }
                            else if (DynamicLibAttValue.DataType == "Double" || DynamicLibAttValue.DataType == "double")
                            {
                                dynamicAttLibValuenew.ValueDouble = double.Parse(DynamicLibAttValue.Value.ToString());
                            }
                            else if (DynamicLibAttValue.DataType == "Boolean" || DynamicLibAttValue.DataType == "boolean")
                            {
                                dynamicAttLibValuenew.ValueBoolean = Boolean.Parse(DynamicLibAttValue.Value.ToString());
                            }
                            else if (DynamicLibAttValue.DataType == "DateTime" || DynamicLibAttValue.DataType == "datetime")
                            {
                                dynamicAttLibValuenew.ValueDateTime = DateTime.Parse(DynamicLibAttValue.Value.ToString());
                            }
                            // DynamicLibAttValue.DynamicAttId = DynamicLibAttValue.Id;
                            _context.TLIdynamicAttLibValue.Add(dynamicAttLibValuenew);
                            _context.SaveChanges();
                            if (TableHistoryId == null || TableHistoryId == 0)
                            {
                                TLItablesHistory tablesHistory = new TLItablesHistory();
                                int? TablehistoryId = null;
                                tablesHistory.TablesNameId = TablesNameId;
                                string HistoryTybeName = _context.TLIhistoryType.Where(x => x.Name == "Update").Select(x => x.Name).FirstOrDefault();
                                tablesHistory.HistoryTypeId = _context.TLIhistoryType.Where(x => x.Name == "Update").Select(x => x.Id).FirstOrDefault();

                                tablesHistory.RecordId = EntitesId.ToString();
                                tablesHistory.UserId = UserId.Value;
                                tablesHistory.Date = DateTime.Now;
                                var CheckTableHistory = _context.TLItablesHistory.Any(x => x.HistoryType.Name == HistoryTybeName && x.RecordId == EntitesId.ToString() && x.TablesNameId == TablesNameId);
                                if (CheckTableHistory)
                                {
                                    var TableHistory = _context.TLItablesHistory.Where(x => x.HistoryType.Name == HistoryTybeName && x.RecordId == EntitesId.ToString() && x.TablesNameId == TablesNameId).Select(x => x.Id).Max();
                                    if (TableHistory != null)
                                    {
                                        TablehistoryId = TableHistory;
                                        if (TablehistoryId != null)
                                        {
                                            tablesHistory.PreviousHistoryId = TablehistoryId;
                                        }
                                    }
                                }
                                _context.TLItablesHistory.Add(tablesHistory);
                                _context.SaveChanges();
                                var CheckTableHistoryForEdit = _context.TLItablesHistory.Where(x => x.TablesNameId == TablesNameId && x.RecordId == EntitesId.ToString()).Select(x => x.Id).ToList();
                                int? tablehistoryid = null;


                                if (CheckTableHistoryForEdit.Count != 0)
                                {
                                    tablehistoryid = CheckTableHistoryForEdit.Max();
                                }

                                TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                                AttName = _context.TLIdynamicAtt.Where(x => x.Id == dynamicAttLibValuenew.DynamicAttId).Select(x => x.Key).FirstOrDefault();
                                historyDetails.AttName = AttName;
                                historyDetails.TablesHistoryId = tablehistoryid;
                                historyDetails.OldValue = null;
                                historyDetails.NewValue = DynamicLibAttValue.Value.ToString();
                                historyDetails.AttributeType = AttributeType.Dynamic;
                                _context.TLIhistoryDetails.Add(historyDetails);
                                _context.SaveChanges();
                            }
                            else
                            {
                                var New = DynamicLibAttValue.Value;

                                TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                                AttName = _context.TLIdynamicAtt.Where(x => x.Id == DynamicLibAttValue.Id).Select(x => x.Key).FirstOrDefault();
                                historyDetails.AttName = AttName;
                                historyDetails.TablesHistoryId = TableHistoryId;
                                historyDetails.OldValue = null;
                                historyDetails.NewValue = New.ToString();
                                historyDetails.AttributeType = AttributeType.Dynamic;
                                _context.TLIhistoryDetails.Add(historyDetails);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
            }
        }
        public void UpdateDynamicLibAttsWithHistorys(List<AddDdynamicAttributeInstallationValueViewModel> DynamicLibAttValues, string connectionString, int TablesNameId, int LibId, int? UserId, int? TableHistoryId = null, int EntitesId = 0)
        {
            var TabelName = _context.TLItablesNames.FirstOrDefault(x => x.Id == TablesNameId)?.TableName;
            string AttName = "";
            string test;
            foreach (var DynamicLibAttValue in DynamicLibAttValues)
            {

                TLIdynamicAtt dynamicatt = _context.TLIdynamicAtt.FirstOrDefault(x => x.Id == DynamicLibAttValue.id);
                /* dynamicatt.Key = DynamicLibAttValue.Key;
                 dynamicatt.DataTypeId = DynamicLibAttValue.DataTypeId;
                 dynamicatt.Required = DynamicLibAttValue.Required;
                 dynamicatt.disable = DynamicLibAttValue.disable;
                 _context.TLIdynamicAtt.Update(dynamicatt);*/


                TLIdynamicAttLibValue dynamicAttLibValue = _context.TLIdynamicAttLibValue
                    .Where(d => d.DynamicAttId == DynamicLibAttValue.id && d.tablesNamesId == TablesNameId && d.InventoryId == LibId)
                    .FirstOrDefault();

                if (dynamicAttLibValue != null)
                {
                    if (DynamicLibAttValue.value == null)
                    {
                        continue;
                    }
                    var OldValue = dynamicAttLibValue.Value;

                    var NewValue = DynamicLibAttValue.value;
                    if (OldValue != NewValue)
                    {
                        if (TableHistoryId == null || TableHistoryId == 0)
                        {
                            TLItablesHistory tablesHistory = new TLItablesHistory();
                            int? TablehistoryId = null;
                            tablesHistory.TablesNameId = TablesNameId;
                            string HistoryTybeName = _context.TLIhistoryType.Where(x => x.Name == "Edit").Select(x => x.Name).FirstOrDefault();
                            tablesHistory.HistoryTypeId = _context.TLIhistoryType.Where(x => x.Name == "Edit").Select(x => x.Id).FirstOrDefault();

                            tablesHistory.RecordId = EntitesId.ToString();
                            tablesHistory.UserId = UserId.Value;
                            tablesHistory.Date = DateTime.Now;
                            var CheckTableHistory = _context.TLItablesHistory.Any(x => x.HistoryType.Name == HistoryTybeName && x.RecordId == EntitesId.ToString() && x.TablesNameId == TablesNameId);
                            if (CheckTableHistory)
                            {
                                var TableHistory = _context.TLItablesHistory.Where(x => x.HistoryType.Name == HistoryTybeName && x.RecordId == EntitesId.ToString() && x.TablesNameId == TablesNameId).Select(x => x.Id).Max();

                                TablehistoryId = TableHistory;
                                if (TablehistoryId != null)
                                {
                                    tablesHistory.PreviousHistoryId = TablehistoryId;
                                }

                            }
                            _context.TLItablesHistory.Add(tablesHistory);
                            _context.SaveChanges();

                            dynamicAttLibValue.Value = NewValue.ToString();
                            if (dynamicAttLibValue.ValueString != null && dynamicAttLibValue.ValueString != "")
                            {
                                string tests = NewValue.ToString().Trim().ToString();
                                dynamicAttLibValue.ValueString = tests;
                                dynamicAttLibValue.Value = tests;
                            }
                            else if (dynamicAttLibValue.ValueDouble != null)
                            {
                                dynamicAttLibValue.ValueDouble = double.Parse(NewValue.ToString());
                            }
                            else if (dynamicAttLibValue.ValueDateTime != null)
                            {
                                dynamicAttLibValue.ValueDateTime = DateTime.Parse(NewValue.ToString());
                            }
                            else if (dynamicAttLibValue.ValueBoolean != null)
                            {
                                dynamicAttLibValue.ValueBoolean = bool.Parse(NewValue.ToString());
                            }

                            _context.TLIdynamicAttLibValue.Update(dynamicAttLibValue);
                            var CheckTableHistoryForEdit = _context.TLItablesHistory.Where(x => x.TablesNameId == TablesNameId).Select(x => x.Id).ToList();
                            int? tablehistoryid = null;
                            if (CheckTableHistoryForEdit.Count != 0)
                            {
                                if (CheckTableHistoryForEdit.Count != 0)
                                {
                                    tablehistoryid = CheckTableHistoryForEdit.Max();
                                }

                                TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                                AttName = _context.TLIdynamicAtt.Where(x => x.Id == dynamicAttLibValue.DynamicAttId).Select(x => x.Key).FirstOrDefault();
                                historyDetails.AttName = AttName;
                                historyDetails.TablesHistoryId = tablehistoryid;
                                if (OldValue == null)
                                {
                                    historyDetails.OldValue = null;
                                }
                                else
                                {
                                    historyDetails.OldValue = OldValue;
                                }
                                historyDetails.NewValue = NewValue.ToString();
                                historyDetails.AttributeType = AttributeType.Dynamic;
                                _context.TLIhistoryDetails.Add(historyDetails);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            var Old = dynamicAttLibValue.Value;

                            var New = DynamicLibAttValue.value;


                            if (Old != New)
                            {
                                dynamicAttLibValue.Value = DynamicLibAttValue.value.ToString();
                                if (dynamicAttLibValue.ValueString != null && dynamicAttLibValue.ValueString != "")
                                {
                                    string test1 = New.ToString().Trim().ToString();
                                    dynamicAttLibValue.ValueString = test1;
                                    dynamicAttLibValue.Value = test1;
                                }
                                else if (dynamicAttLibValue.ValueDouble != null)
                                {
                                    dynamicAttLibValue.ValueDouble = double.Parse(New.ToString());
                                }
                                else if (dynamicAttLibValue.ValueDateTime != null)
                                {
                                    dynamicAttLibValue.ValueDateTime = DateTime.Parse(New.ToString());
                                }
                                else if (dynamicAttLibValue.ValueBoolean != null)
                                {
                                    dynamicAttLibValue.ValueBoolean = bool.Parse(New.ToString());
                                }
                                _context.TLIdynamicAttLibValue.Update(dynamicAttLibValue);
                                var CheckTableHistoryForEdit = _context.TLItablesHistory.Where(x => x.TablesNameId == TablesNameId).Select(x => x.Id).ToList();
                                int? tablehistoryid = null;
                                if (CheckTableHistoryForEdit.Count != 0)
                                {
                                    if (CheckTableHistoryForEdit.Count != 0)
                                    {
                                        tablehistoryid = CheckTableHistoryForEdit.Max();
                                    }

                                    TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                                    AttName = _context.TLIdynamicAtt.Where(x => x.Id == dynamicAttLibValue.DynamicAttId).Select(x => x.Key).FirstOrDefault();
                                    historyDetails.AttName = AttName;
                                    historyDetails.TablesHistoryId = TableHistoryId;
                                    historyDetails.OldValue = Old;
                                    historyDetails.NewValue = New.ToString();
                                    historyDetails.AttributeType = AttributeType.Dynamic;
                                    _context.TLIhistoryDetails.Add(historyDetails);
                                    _context.SaveChanges();
                                }

                            }
                        }
                    }
                }
                // _context.SaveChanges();
                else
                {
                    if (DynamicLibAttValue.value == null)
                    {
                        continue;
                    }
                    var Check = _context.TLIdynamicAtt.Where(x => x.Id == DynamicLibAttValue.id).FirstOrDefault();
                    if (Check != null)
                    {
                        if (Check.tablesNamesId == TablesNameId && Check.LibraryAtt == true && Check.Id == DynamicLibAttValue.id)
                        {
                            TLIdynamicAttLibValue dynamicAttLibValuenew = new TLIdynamicAttLibValue();
                            dynamicAttLibValuenew.InventoryId = LibId;
                            dynamicAttLibValuenew.DynamicAttId = DynamicLibAttValue.id;
                            dynamicAttLibValuenew.tablesNamesId = TablesNameId;
                            dynamicAttLibValuenew.Value = DynamicLibAttValue.value.ToString();
                            dynamic value = DynamicLibAttValue.value;
                            switch (value)
                            {
                                case int NumberValue:
                                    dynamicAttLibValuenew.ValueDouble = NumberValue;
                                    break;
                                case string stringValue:
                                    dynamicAttLibValuenew.ValueString = stringValue;
                                    break;
                                case double doubleValue:
                                    dynamicAttLibValuenew.ValueDouble = doubleValue;
                                    break;
                                case DateTime dateTimeValue:
                                    dynamicAttLibValuenew.ValueDateTime = dateTimeValue;
                                    break;
                                case bool booleanValue:
                                    dynamicAttLibValuenew.ValueBoolean = booleanValue;
                                    break;
                            }

                            _context.TLIdynamicAttLibValue.Add(dynamicAttLibValuenew);
                            _context.SaveChanges();
                            if (TableHistoryId == null || TableHistoryId == 0)
                            {
                                TLItablesHistory tablesHistory = new TLItablesHistory();
                                int? TablehistoryId = null;
                                tablesHistory.TablesNameId = TablesNameId;
                                string HistoryTybeName = _context.TLIhistoryType.Where(x => x.Name == "Edit").Select(x => x.Name).FirstOrDefault();
                                tablesHistory.HistoryTypeId = _context.TLIhistoryType.Where(x => x.Name == "Edit").Select(x => x.Id).FirstOrDefault();

                                tablesHistory.RecordId = EntitesId.ToString();
                                tablesHistory.UserId = UserId.Value;
                                tablesHistory.Date = DateTime.Now;
                                var CheckTableHistory = _context.TLItablesHistory.Any(x => x.HistoryType.Name == HistoryTybeName && x.RecordId == EntitesId.ToString() && x.TablesNameId == TablesNameId);
                                if (CheckTableHistory)
                                {
                                    var TableHistory = _context.TLItablesHistory.Where(x => x.HistoryType.Name == HistoryTybeName && x.RecordId == EntitesId.ToString() && x.TablesNameId == TablesNameId).Select(x => x.Id).Max();
                                    if (TableHistory != null)
                                    {
                                        TablehistoryId = TableHistory;
                                        if (TablehistoryId != null)
                                        {
                                            tablesHistory.PreviousHistoryId = TablehistoryId;
                                        }
                                    }
                                }
                                _context.TLItablesHistory.Add(tablesHistory);
                                _context.SaveChanges();
                                var CheckTableHistoryForEdit = _context.TLItablesHistory.Where(x => x.TablesNameId == TablesNameId && x.RecordId == EntitesId.ToString()).Select(x => x.Id).ToList();
                                int? tablehistoryid = null;


                                if (CheckTableHistoryForEdit.Count != 0)
                                {
                                    tablehistoryid = CheckTableHistoryForEdit.Max();
                                }

                                TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                                AttName = _context.TLIdynamicAtt.Where(x => x.Id == dynamicAttLibValuenew.DynamicAttId).Select(x => x.Key).FirstOrDefault();
                                historyDetails.AttName = AttName;
                                historyDetails.TablesHistoryId = tablehistoryid;
                                historyDetails.OldValue = null;
                                historyDetails.NewValue = DynamicLibAttValue.value.ToString();
                                historyDetails.AttributeType = AttributeType.Dynamic;
                                _context.TLIhistoryDetails.Add(historyDetails);
                                _context.SaveChanges();
                            }
                            else
                            {
                                var New = DynamicLibAttValue.value;

                                TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                                AttName = _context.TLIdynamicAtt.Where(x => x.Id == DynamicLibAttValue.id).Select(x => x.Key).FirstOrDefault();
                                historyDetails.AttName = AttName;
                                historyDetails.TablesHistoryId = TableHistoryId;
                                historyDetails.OldValue = null;
                                historyDetails.NewValue = New.ToString();
                                historyDetails.AttributeType = AttributeType.Dynamic;
                                _context.TLIhistoryDetails.Add(historyDetails);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
            }
            Task.Run(() => RefreshView(connectionString));
        }
        public void UpdateDynamicLibAttsWithH(List<AddDdynamicAttributeInstallationValueViewModel> DynamicLibAttValues, string connectionString, int TablesNameId, int LibId, int? UserId,int HistoryId)
        {
           
                var TabelName = _context.TLItablesNames.FirstOrDefault(x => x.Id == TablesNameId)?.TableName;
                string AttName = "";
                string test;
                foreach (var DynamicLibAttValue in DynamicLibAttValues)
                {

                    TLIdynamicAtt dynamicatt = _context.TLIdynamicAtt.FirstOrDefault(x => x.Id == DynamicLibAttValue.id);
                  
                    TLIdynamicAttLibValue dynamicAttLibValue = _context.TLIdynamicAttLibValue
                        .Where(d => d.DynamicAttId == DynamicLibAttValue.id && d.tablesNamesId == TablesNameId && d.InventoryId == LibId)
                        .FirstOrDefault();
                    var TabelNamesec = _context.TLItablesNames.FirstOrDefault(x => x.TableName == "TLIdynamicAttLibValue").Id;
                    if (dynamicAttLibValue != null)
                    {
                        if (DynamicLibAttValue.value == null)
                        {
                            continue;
                        }
                      
                        var Old = dynamicAttLibValue.Value;

                        var New = DynamicLibAttValue.value.ToString();

                        if (Old != New)
                        {
                            dynamicAttLibValue.Value = DynamicLibAttValue.value.ToString();
                            if (dynamicAttLibValue.ValueString != null && dynamicAttLibValue.ValueString != "")
                            {
                                string test1 = New.ToString().Trim().ToString();
                                dynamicAttLibValue.ValueString = test1;
                                dynamicAttLibValue.Value = test1;
                            }
                            else if (dynamicAttLibValue.ValueDouble != null)
                            {
                                dynamicAttLibValue.ValueDouble = double.Parse(New.ToString());
                            }
                            else if (dynamicAttLibValue.ValueDateTime != null)
                            {
                                dynamicAttLibValue.ValueDateTime = DateTime.Parse(New.ToString());
                            }
                            else if (dynamicAttLibValue.ValueBoolean != null)
                            {
                                dynamicAttLibValue.ValueBoolean = bool.Parse(New.ToString());
                            }
                            _context.TLIdynamicAttLibValue.Update(dynamicAttLibValue);
                            var CheckTableHistoryForEdit = _context.TLItablesHistory.Where(x => x.TablesNameId == TablesNameId).Select(x => x.Id).ToList();
                            TLIhistoryDet historyDetails = new TLIhistoryDet();
                            AttName = _context.TLIdynamicAtt.Where(x => x.Id == dynamicAttLibValue.DynamicAttId).Select(x => x.Key).FirstOrDefault();
                            historyDetails.AttributeName = AttName;
                            historyDetails.TablesNameId = TabelNamesec;
                            historyDetails.OldValue = Old;
                            historyDetails.NewValue = New.ToString();
                            historyDetails.HistoryId = HistoryId;
                            historyDetails.AttributeType = AttributeType.Dynamic;
                            historyDetails.RecordId = dynamicAttLibValue.Id.ToString();
                            _context.TLIhistoryDet.Add(historyDetails);
                            _context.SaveChanges();


                        }

                        
                    }
                    else
                    {
                        if (DynamicLibAttValue.value == null)
                        {
                            continue;
                        }
                        var Check = _context.TLIdynamicAtt.Where(x => x.Id == DynamicLibAttValue.id).FirstOrDefault();
                        if (Check != null)
                        {
                            if (Check.tablesNamesId == TablesNameId && Check.LibraryAtt == true && Check.Id == DynamicLibAttValue.id)
                            {
                                TLIdynamicAttLibValue dynamicAttLibValuenew = new TLIdynamicAttLibValue();
                                dynamicAttLibValuenew.InventoryId = LibId;
                                dynamicAttLibValuenew.DynamicAttId = DynamicLibAttValue.id;
                                dynamicAttLibValuenew.tablesNamesId = TablesNameId;
                                dynamicAttLibValuenew.Value = DynamicLibAttValue.value.ToString();
                                dynamic value = DynamicLibAttValue.value;
                                switch (value)
                                {
                                    case int NumberValue:
                                        dynamicAttLibValuenew.ValueDouble = NumberValue;
                                        break;
                                    case string stringValue:
                                        dynamicAttLibValuenew.ValueString = stringValue;
                                        break;
                                    case double doubleValue:
                                        dynamicAttLibValuenew.ValueDouble = doubleValue;
                                        break;
                                    case DateTime dateTimeValue:
                                        dynamicAttLibValuenew.ValueDateTime = dateTimeValue;
                                        break;
                                    case bool booleanValue:
                                        dynamicAttLibValuenew.ValueBoolean = booleanValue;
                                        break;
                                }

                                _context.TLIdynamicAttLibValue.Add(dynamicAttLibValuenew);
                                _context.SaveChanges();
                                var New = DynamicLibAttValue.value;

                                TLIhistoryDet historyDetails = new TLIhistoryDet();
                                AttName = _context.TLIdynamicAtt.Where(x => x.Id == DynamicLibAttValue.id).Select(x => x.Key).FirstOrDefault();
                                historyDetails.AttributeName = AttName;
                                historyDetails.TablesNameId = TabelNamesec;
                                historyDetails.RecordId = dynamicAttLibValuenew.Id.ToString();
                                historyDetails.HistoryId = HistoryId;

                                historyDetails.OldValue = null;
                                historyDetails.NewValue = New.ToString();
                                historyDetails.AttributeType = AttributeType.Dynamic;
                                _context.TLIhistoryDet.Add(historyDetails);
                                _context.SaveChanges();

                            }
                        }
                    }
           
                Task.Run(() => RefreshView(connectionString));
            }
        }
    }
}
