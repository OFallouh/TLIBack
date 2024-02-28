using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

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
        public void AddDdynamicAttributeInstallation(AddDdynamicAttributeInstallationValueViewModel addDynamicInstAttValue, int TableNameId, int Id)
        {
            var DynamicAtt = _context.TLIdynamicAtt.Include(x => x.DataType).FirstOrDefault(x => x.Id == addDynamicInstAttValue.id);
            TLIdynamicAttInstValue dynamicAttInstValue = new TLIdynamicAttInstValue();
            if (addDynamicInstAttValue.value is string stringValue)
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueString = stringValue;
                dynamicAttInstValue.disable = false;
                _context.TLIdynamicAttInstValue.Add(dynamicAttInstValue);
                _context.SaveChanges();
            }
            else if( addDynamicInstAttValue.value is double DoubleValue)
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueDouble = DoubleValue;
                dynamicAttInstValue.disable = false;
                _context.TLIdynamicAttInstValue.Add(dynamicAttInstValue);
                _context.SaveChanges();
            }
            else if (addDynamicInstAttValue.value is DateTime dateTimeValue)
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueDateTime = dateTimeValue;
                dynamicAttInstValue.disable = false;
                _context.TLIdynamicAttInstValue.Add(dynamicAttInstValue);
                _context.SaveChanges();
            }
            else if (addDynamicInstAttValue.value is bool booleanValue)
            {
                dynamicAttInstValue.DynamicAttId = DynamicAtt.Id;
                dynamicAttInstValue.InventoryId = Id;
                dynamicAttInstValue.tablesNamesId = TableNameId;
                dynamicAttInstValue.ValueBoolean = booleanValue;
                dynamicAttInstValue.disable = false;
                _context.TLIdynamicAttInstValue.Add(dynamicAttInstValue);
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
    }
}
