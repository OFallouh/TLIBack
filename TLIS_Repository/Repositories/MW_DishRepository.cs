using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    class MW_DishRepository:RepositoryBase<TLImwDish, MW_DishViewModel, int>, IMW_DishRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public MW_DishRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var RepeaterType = _context.TLIrepeaterType.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> RepeaterTypeLists = _mapper.Map<List<DropDownListFilters>>(RepeaterType);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("RepeaterTypeId", RepeaterTypeLists));

            var PolarityOnLocation = _context.TLIpolarityOnLocation.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> OduInsPolarityOnLocationlists = _mapper.Map<List<DropDownListFilters>>(PolarityOnLocation);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("PolarityOnLocationId", OduInsPolarityOnLocationlists));

            var ItemConnectTo = _context.TLIitemConnectTo.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> ItemConnectTolists = _mapper.Map<List<DropDownListFilters>>(ItemConnectTo);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ItemConnectToId", ItemConnectTolists));

            var MwDishLibrary = _context.TLImwDishLibrary.Where(x => !x.Deleted && x.Active).ToList();
            List<DropDownListFilters> MwDishLibraryLists = _mapper.Map<List<DropDownListFilters>>(MwDishLibrary);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("MwDishLibraryId", MwDishLibraryLists));

            var InstallationPlace = _context.TLIinstallationPlace.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> InstallationPlacelists = _mapper.Map<List<DropDownListFilters>>(InstallationPlace);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("InstallationPlaceId", InstallationPlacelists));

            var Owner = _context.TLIowner.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> OwnerIds = _mapper.Map<List<DropDownListFilters>>(Owner);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ownerId", OwnerIds));
            return RelatedTables;
        }
    }
}
