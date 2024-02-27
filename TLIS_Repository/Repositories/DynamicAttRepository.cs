using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class DynamicAttRepository : RepositoryBase<TLIdynamicAtt, DynamicAttViewModel, int>, IDynamicAttRepository
    {
        private ApplicationDbContext _context;
        IMapper _mapper;
        public DynamicAttRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<DynamicAttLibViewModel> GetDynamicLibAtts(int TableNameId, int? CategoryId)
        {
            List<DynamicAttLibViewModel> dynamicAtts = null;
            if (CategoryId == null)
            {
                List<TLIdynamicAtt> DynamicAtts = _context.TLIdynamicAtt
                    .Where(d => d.LibraryAtt == true && d.tablesNamesId == TableNameId && !d.disable)
                    .Include(d => d.DataType)
                    .ToList();
                dynamicAtts = _mapper.Map<List<DynamicAttLibViewModel>>(DynamicAtts);
            }
            else
            {
                List<TLIdynamicAtt> DynamicAtts = _context.TLIdynamicAtt
                    .Where(d => !d.disable && d.LibraryAtt == true && d.tablesNamesId == TableNameId && d.CivilWithoutLegCategoryId == CategoryId)
                    .Include(d => d.DataType)
                    .ToList();
                dynamicAtts = _mapper.Map<List<DynamicAttLibViewModel>>(DynamicAtts);
            }
            return dynamicAtts;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
            var DataTypes = _context.TLIdataType.Where(x => x.Disable == false && x.Deleted == false).ToList();
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("DataTypeId", _mapper.Map<List<DropDownListFilters>>(DataTypes)));
            var TableNames = _context.TLItablesNames.Where(x => x.IsEquip == true).ToList();
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("tablesNamesId", _mapper.Map<List<DropDownListFilters>>(TableNames)));
            var CivilWithoutLegCategories = _context.TLIcivilWithoutLegCategory.Where(x => x.disable == false).ToList();
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("CivilWithoutLegCategoryId", _mapper.Map<List<DropDownListFilters>>(CivilWithoutLegCategories)));
            return RelatedTables;
        }
        public IEnumerable<DynaminAttInstViewModel> GetDynamicInstAtts(int TableNameId, int? CategoryId)
        {
            List<DynaminAttInstViewModel> dynamicAtts = null;
            if (CategoryId == null)
            {
                var DynamicAtts = _context.TLIdynamicAtt
                    .Where(d => !d.LibraryAtt && d.tablesNamesId == TableNameId && !d.disable)
                    .Include(d => d.DataType)
                    .ToList();
                dynamicAtts = _mapper.Map<List<DynaminAttInstViewModel>>(DynamicAtts);
            }
            else
            {
                var DynamicAtts = _context.TLIdynamicAtt
                    .Where(d => !d.LibraryAtt && d.tablesNamesId == TableNameId && d.CivilWithoutLegCategoryId == CategoryId && !d.disable)
                    .Include(d => d.DataType)
                    .ToList();
                dynamicAtts = _mapper.Map<List<DynaminAttInstViewModel>>(DynamicAtts);
            }
            return dynamicAtts;

        }
        public IEnumerable<BaseInstAttViewDynamic> GetDynamicInstAttInst(int TableNameId, int? CategoryId)
        {
            List<BaseInstAttViewDynamic> dynamicAtts = null;
            if (CategoryId == null)
            {
                var DynamicAtts = _context.TLIdynamicAtt
                    .Where(d => !d.LibraryAtt && d.tablesNamesId == TableNameId && !d.disable)
                    .Include(d => d.DataType)
                    .ToList();
                dynamicAtts = _mapper.Map<List<BaseInstAttViewDynamic>>(DynamicAtts);
            }
            else
            {
                var DynamicAtts = _context.TLIdynamicAtt
                    .Where(d => !d.LibraryAtt && d.tablesNamesId == TableNameId && d.CivilWithoutLegCategoryId == CategoryId && !d.disable)
                    .Include(d => d.DataType)
                    .ToList();
                dynamicAtts = _mapper.Map<List<BaseInstAttViewDynamic>>(DynamicAtts);
            }
            return dynamicAtts;

        }
    }
}
