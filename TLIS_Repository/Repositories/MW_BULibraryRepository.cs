using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.Helpers;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class MW_BULibraryRepository : RepositoryBase<TLImwBULibrary, MW_BULibraryViewModel, int>, IMW_BULibraryRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;

        public MW_BULibraryRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var diversityType = _context.TLIdiversityType.Where(x => !x.Disable && !x.Deleted).ToList();
            List<DropDownListFilters> diversityTypeFilters = _mapper.Map<List<DropDownListFilters>>(diversityType);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("diversityTypeId", diversityTypeFilters));

            var Suppliers = _context.TLIlogistical.Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Supplier.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.MW.ToString().ToLower()).ToList();
            List<DropDownListFilters> SuppliersFilters = _mapper.Map<List<DropDownListFilters>>(Suppliers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Suppliers", SuppliersFilters));

            var Designers = _context.TLIlogistical.Include(x => x.logisticalType).Include(x => x.tablePartName)
                .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Designer.ToString().ToLower() && !x.logisticalType.Disable &&
                    !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.MW.ToString().ToLower()).ToList();
            List<DropDownListFilters> DesignersFilters = _mapper.Map<List<DropDownListFilters>>(Designers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Designers", DesignersFilters));

            var Manufacturers = _context.TLIlogistical.Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Manufacturer.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.MW.ToString().ToLower()).ToList();
            List<DropDownListFilters> ManufacturersFilters = _mapper.Map<List<DropDownListFilters>>(Manufacturers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Manufacturers", ManufacturersFilters));

            var Vendors = _context.TLIlogistical.Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Vendor.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.MW.ToString().ToLower()).ToList();
            List<DropDownListFilters> VendorsFilters = _mapper.Map<List<DropDownListFilters>>(Vendors);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Vendors", VendorsFilters));

            return RelatedTables;
        }
    }
}
