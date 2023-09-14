using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class LoadOtherRepository: RepositoryBase<TLIloadOther, LoadOtherViewModel, int>, ILoadOtherRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public LoadOtherRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var loadOtherLibrary = _dbContext.TLIloadOtherLibrary.Where(x => x.Active && !x.Deleted).ToList();
            List<DropDownListFilters> loadOtherLibraryLists = _mapper.Map<List<DropDownListFilters>>(loadOtherLibrary);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("loadOtherLibraryId", loadOtherLibraryLists));

            var InstallationPlace = _dbContext.TLIinstallationPlace.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> InstallationPlacelists = _mapper.Map<List<DropDownListFilters>>(InstallationPlace);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("InstallationPlaceId", InstallationPlacelists));

            return RelatedTables;
        }
    }
}
