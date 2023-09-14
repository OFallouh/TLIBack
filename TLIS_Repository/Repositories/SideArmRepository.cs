using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class SideArmRepository:RepositoryBase<TLIsideArm,SideArmViewModel,int>, ISideArmRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public SideArmRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var sideArmLibrary = _context.TLIsideArmLibrary.Where(x => x.Active == true && x.Deleted == false).ToList();
            List<DropDownListFilters> sideArmLibraryFilters = _mapper.Map<List<DropDownListFilters>>(sideArmLibrary);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("sideArmLibraryId", sideArmLibraryFilters));

            var sideArmInstallationPlace = _context.TLIsideArmInstallationPlace.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> sideArmInstallationPlaceFilters = _mapper.Map<List<DropDownListFilters>>(sideArmInstallationPlace);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("sideArmInstallationPlaceId", sideArmInstallationPlaceFilters));

            var owner = _context.TLIowner.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> ownerFilters = _mapper.Map<List<DropDownListFilters>>(owner);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ownerId", ownerFilters));

            var sideArmTypes = _context.TLIsideArmType.Where(x => x.Disable == false && x.Deleted == false).ToList();
            List<DropDownListFilters> sideArmTypeFilters = _mapper.Map<List<DropDownListFilters>>(sideArmTypes);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("sideArmTypeId", sideArmTypeFilters));
            return RelatedTables;
        }
    }
}
