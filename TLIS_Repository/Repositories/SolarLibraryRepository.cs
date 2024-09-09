using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.Helpers;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class SolarLibraryRepository: RepositoryBase<TLIsolarLibrary, SolarLibraryViewModel, int>, ISolarLibraryRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public SolarLibraryRepository(ApplicationDbContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
            //var Capacities = _context.TLIcapacity.AsNoTracking().Where(x => !x.Delete && !x.Disable).ToList();
            //List<DropDownListFilters> CapacityFilters = _mapper.Map<List<DropDownListFilters>>(Capacities);
            //RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("CapacityId", CapacityFilters));

            var Suppliers = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
                .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Supplier.ToString().ToLower() && !x.logisticalType.Disable &&
                    !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.OtherInventory.ToString().ToLower()).ToList();
            List<DropDownListFilters> SuppliersFilters = _mapper.Map<List<DropDownListFilters>>(Suppliers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Suppliers", SuppliersFilters));

            var Designers = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
                .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Designer.ToString().ToLower() && !x.logisticalType.Disable &&
                    !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.OtherInventory.ToString().ToLower()).ToList();
            List<DropDownListFilters> DesignersFilters = _mapper.Map<List<DropDownListFilters>>(Designers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Designers", DesignersFilters));

            var Manufacturers = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Manufacturer.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.OtherInventory.ToString().ToLower()).ToList();
            List<DropDownListFilters> ManufacturersFilters = _mapper.Map<List<DropDownListFilters>>(Manufacturers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Manufacturers", ManufacturersFilters));

            var Vendors = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Vendor.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.OtherInventory.ToString().ToLower()).ToList();
            List<DropDownListFilters> VendorsFilters = _mapper.Map<List<DropDownListFilters>>(Vendors);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Vendors", VendorsFilters));

            var Contractors = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
              .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Contractor.ToString().ToLower() && !x.logisticalType.Disable &&
                  !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.CivilSupport.ToString().ToLower()).ToList();
            List<DropDownListFilters> ContractorsFilters = _mapper.Map<List<DropDownListFilters>>(Contractors);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Contractors", ContractorsFilters));

            var Conultants = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Consultant.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.CivilSupport.ToString().ToLower()).ToList();
            List<DropDownListFilters> ConultantsFilters = _mapper.Map<List<DropDownListFilters>>(Conultants);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Conultants", ConultantsFilters));
            return RelatedTables;
        }
    }
}
