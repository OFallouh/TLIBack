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
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.Helpers;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class PowerLibraryRepository : RepositoryBase<TLIpowerLibrary, PowerLibraryViewModel, int>, IPowerLibraryRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;
        public PowerLibraryRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //public void Add(PowerLibraryViewModel AddPowerLibraryViewModel)
        //{
        //    throw new NotImplementedException();
        //}

        //public virtual async Task<IEnumerable<TLIpowerLibrary>> GetAll(ParameterPagination parameterPagination)
        //{
        //    return this.GetAllAsQueryable().ToList();
        //}
        //public void Add(PowerLibraryViewModel AddPowerLibraryViewModel)
        //{
        //    TLIpowerLibrary powerLibrary = new TLIpowerLibrary();
        //    powerLibrary.Id = AddPowerLibraryViewModel.Id;
        //    powerLibrary.Name = AddPowerLibraryViewModel.Name;
        //    powerLibrary.Weight = AddPowerLibraryViewModel.Weight;
        //    powerLibrary.width = AddPowerLibraryViewModel.width;
        //    powerLibrary.Length = AddPowerLibraryViewModel.Length;
        //    powerLibrary.Depth = AddPowerLibraryViewModel.Depth;
        //    powerLibrary.LoadType_Id = AddPowerLibraryViewModel.LoadType_Id;

        //    _context.TLIpowerLibrary.Add(powerLibrary);
        //    _context.SaveChanges();
        //}
        //public void Update(PowerLibraryViewModel EditPowerLibraryViewModel)
        //{
        //    var PowerLibrary = _context.TLIpowerLibrary.Find(EditPowerLibraryViewModel.Id);
        //    PowerLibrary.Id = EditPowerLibraryViewModel.Id;
        //    PowerLibrary.Name = EditPowerLibraryViewModel.Name;
        //    PowerLibrary.Weight = EditPowerLibraryViewModel.Weight;
        //    PowerLibrary.width = EditPowerLibraryViewModel.width;
        //    PowerLibrary.Length = EditPowerLibraryViewModel.Length;
        //    PowerLibrary.LoadType_Id = EditPowerLibraryViewModel.LoadType_Id;

        //    _context.SaveChanges();
        //}

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var Suppliers = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
                .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Supplier.ToString().ToLower() && !x.logisticalType.Disable &&
                    !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.Power.ToString().ToLower()).ToList();
            List<DropDownListFilters> SuppliersFilters = _mapper.Map<List<DropDownListFilters>>(Suppliers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Suppliers", SuppliersFilters));

            var Designers = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
              .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Designer.ToString().ToLower() && !x.logisticalType.Disable &&
                  !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.Power.ToString().ToLower()).ToList();
            List<DropDownListFilters> DesignersFilters = _mapper.Map<List<DropDownListFilters>>(Designers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Designers", DesignersFilters));

            var Manufacturers = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Manufacturer.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.Power.ToString().ToLower()).ToList();
            List<DropDownListFilters> ManufacturersFilters = _mapper.Map<List<DropDownListFilters>>(Manufacturers);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Manufacturers", ManufacturersFilters));

            var Vendors = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Vendor.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.Power.ToString().ToLower()).ToList();
            List<DropDownListFilters> VendorsFilters = _mapper.Map<List<DropDownListFilters>>(Vendors);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Vendors", VendorsFilters));

            var Contractors = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
              .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Contractor.ToString().ToLower() && !x.logisticalType.Disable &&
                  !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.Power.ToString().ToLower()).ToList();
            List<DropDownListFilters> ContractorsFilters = _mapper.Map<List<DropDownListFilters>>(Contractors);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Contractors", ContractorsFilters));

            var Conultants = _context.TLIlogistical.AsNoTracking().Include(x => x.logisticalType).Include(x => x.tablePartName)
               .Where(x => x.Active && !x.Deleted && x.logisticalType.Name.ToLower() == Constants.TLIlogisticalType.Consultant.ToString().ToLower() && !x.logisticalType.Disable &&
                   !x.logisticalType.Deleted && x.tablePartName.PartName.ToLower() == Constants.TablePartName.Power.ToString().ToLower()).ToList();
            List<DropDownListFilters> ConultantsFilters = _mapper.Map<List<DropDownListFilters>>(Conultants);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Conultants", ConultantsFilters));
            return RelatedTables;
        }

    }
}
