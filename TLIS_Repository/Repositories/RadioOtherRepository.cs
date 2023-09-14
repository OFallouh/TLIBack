using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class RadioOtherRepository: RepositoryBase<TLIradioOther, RadioOtherViewModel, int>, IRadioOtherRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public RadioOtherRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
            var owners = _dbContext.TLIowner.Where(x => !x.Disable && !x.Deleted).ToList();
            List<DropDownListFilters> ownerLists = _mapper.Map<List<DropDownListFilters>>(owners);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ownerId", ownerLists));
            var InstallationPlaces = _dbContext.TLIinstallationPlace.Where(x => !x.Disable && !x.Deleted).ToList();
            List<DropDownListFilters> InstallationPlaceLists = _mapper.Map<List<DropDownListFilters>>(InstallationPlaces);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("installationPlaceId", InstallationPlaceLists));
            var radioOtherLibrary = _dbContext.TLIradioOtherLibrary.Where(x => x.Active == true && x.Deleted == false).ToList();
            List<DropDownListFilters> radioOtherLibraryLists = _mapper.Map<List<DropDownListFilters>>(radioOtherLibrary);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("radioOtherLibraryId", radioOtherLibraryLists));
            return RelatedTables;
        }
    }
}
