using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.Helpers;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class CivilWithoutLegLibraryRepository : RepositoryBase<TLIcivilWithoutLegLibrary, CivilWithoutLegLibraryViewModel, int>, ICivilWithoutLegLibraryRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;

        public CivilWithoutLegLibraryRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var CivilSteelSupportCategory = _context.TLIcivilSteelSupportCategory.AsNoTracking().ToList();
            List<DropDownListFilters> CivilSteelSupportCategoryFilters = _mapper.Map<List<DropDownListFilters>>(CivilSteelSupportCategory);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("CivilSteelSupportCategoryId", CivilSteelSupportCategoryFilters));

            var InstCivilwithoutLegsType = _context.TLIinstallationCivilwithoutLegsType.AsNoTracking().Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> InstCivilwithoutLegsTypeFilters = _mapper.Map<List<DropDownListFilters>>(InstCivilwithoutLegsType);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("InstCivilwithoutLegsTypeId", InstCivilwithoutLegsTypeFilters));

            var CivilWithoutlegCategory = _context.TLIcivilWithoutLegCategory.AsNoTracking().Where(x => !x.disable).ToList();
            List<DropDownListFilters> CivilWithoutlegCategoryFilters = _mapper.Map<List<DropDownListFilters>>(CivilWithoutlegCategory);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("CivilWithoutLegCategoryId", CivilWithoutlegCategoryFilters));


            var StructureType = _context.TLIstructureType.AsNoTracking().Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> StructureTypeFilters = _mapper.Map<List<DropDownListFilters>>(StructureType);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("structureTypeId", StructureTypeFilters));

            var Suppliers = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Supplier.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.CivilSupport.ToString().ToLower()).ToList();
            List<DropDownListFilters> SuppliersFilters = _mapper.Map<List<DropDownListFilters>>(Suppliers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Suppliers", SuppliersFilters));

            var Designers = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
                .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Designer.ToString().ToLower() && !x.logisticalType.Disable &&
                    !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.CivilSupport.ToString().ToLower()).ToList();
            List<DropDownListFilters> DesignersFilters = _mapper.Map<List<DropDownListFilters>>(Designers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Designers", DesignersFilters));

            var Manufacturers = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Manufacturer.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.CivilSupport.ToString().ToLower()).ToList();
            List<DropDownListFilters> ManufacturersFilters = _mapper.Map<List<DropDownListFilters>>(Manufacturers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Manufacturers", ManufacturersFilters));

            var Vendors = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Vendor.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.CivilSupport.ToString().ToLower()).ToList();
            List<DropDownListFilters> VendorsFilters = _mapper.Map<List<DropDownListFilters>>(Vendors);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Vendors", VendorsFilters));

            return RelatedTables;
        }
    }
}
