using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class MW_BURepository:RepositoryBase<TLImwBU, MW_BUViewModel, int>, IMW_BURepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public MW_BURepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTablesForEdit(string SiteCode, int? PortCascadeId)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var Owners = _context.TLIowner.Where(x => !x.Deleted && !x.Disable).ToList();

            List<DropDownListFilters> OwnerLists = _mapper.Map<List<DropDownListFilters>>(Owners);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("OwnerId", OwnerLists));

            var MwBULibrary = _context.TLImwBULibrary.Where(x => x.Active && !x.Deleted).ToList();


            List<DropDownListFilters> MwBULibraryLists = _mapper.Map<List<DropDownListFilters>>(MwBULibrary);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("MwBULibraryId", MwBULibraryLists));

            var MW_DishesOnSite = _context.TLIcivilLoads.Include(x => x.allLoadInst).Where(x => !x.Dismantle && x.allLoadInstId != null &&
                x.allLoadInst.mwDishId != null).Select(x => x.allLoadInst.mwDish).ToList();

            List<DropDownListFilters> mwDishLists = _mapper.Map<List<DropDownListFilters>>(MW_DishesOnSite);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("MainDishId", mwDishLists));
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("SdDishId", mwDishLists));

            //var BaseBU = _context.TLIbaseBU.Where(x => !x.Deleted && !x.Disable).ToList();
            var BaseBU = _context.TLIbaseBU.ToList();

            List<DropDownListFilters> BaseBULists = _mapper.Map<List<DropDownListFilters>>(BaseBU);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("BaseBUId", BaseBULists));

            // var InstallationPlace = _context.TLIinstallationPlace.Where(x => !x.Deleted && !x.Disable).ToList();
            var InstallationPlace = _context.TLIinstallationPlace.ToList();

            List<DropDownListFilters> InstallationPlaceLists = _mapper.Map<List<DropDownListFilters>>(InstallationPlace);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("InstallationPlaceId", InstallationPlaceLists));

            if (PortCascadeId != null && PortCascadeId != 0)
            {
                List<int?> UsedPorts = _context.TLImwBU
                    .Where(x => x.PortCascadeId > 0).Select(x => x.PortCascadeId).ToList();

                List<DropDownListFilters> PortsLists = _mapper.Map<List<DropDownListFilters>>(_context.TLImwPort
                    .Include(x => x.MwBU).Include(x => x.MwBULibrary).Where(x => x.MwBUId == PortCascadeId && !UsedPorts.Contains(x.Id)).ToList());

                RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("PortCascadedBUId", PortsLists));
            }
            return RelatedTables;
        }
        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var Owners = _context.TLIowner.Where(x => !x.Deleted && !x.Disable).ToList();

            List<DropDownListFilters> OwnerLists = _mapper.Map<List<DropDownListFilters>>(Owners);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("OwnerId", OwnerLists));

            var MwBULibrary = _context.TLImwBULibrary.Where(x => x.Active && !x.Deleted).ToList();


            List<DropDownListFilters> MwBULibraryLists = _mapper.Map<List<DropDownListFilters>>(MwBULibrary);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("MwBULibraryId", MwBULibraryLists));

            var MW_DishesOnSite = _context.TLIcivilLoads.Include(x => x.allLoadInst).Where(x => !x.Dismantle && x.allLoadInstId != null &&
                x.allLoadInst.mwDishId != null).Select(x => x.allLoadInst.mwDish).ToList();

            List<DropDownListFilters> mwDishLists = _mapper.Map<List<DropDownListFilters>>(MW_DishesOnSite);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("MainDishId", mwDishLists));
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("SdDishId", mwDishLists));

            //var BaseBU = _context.TLIbaseBU.Where(x => !x.Deleted && !x.Disable).ToList();
            var BaseBU = _context.TLIbaseBU.ToList();

            List<DropDownListFilters> BaseBULists = _mapper.Map<List<DropDownListFilters>>(BaseBU);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("BaseBUId", BaseBULists));

            // var InstallationPlace = _context.TLIinstallationPlace.Where(x => !x.Deleted && !x.Disable).ToList();
            var InstallationPlace = _context.TLIinstallationPlace.ToList();

            List<DropDownListFilters> InstallationPlaceLists = _mapper.Map<List<DropDownListFilters>>(InstallationPlace);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("InstallationPlaceId", InstallationPlaceLists));

            return RelatedTables;
        }
    }
}
