using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class Mw_OtherRepository: RepositoryBase<TLImwOther, Mw_OtherViewModel, int>, IMw_OtherRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public Mw_OtherRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var mwOtherLibrary = _dbContext.TLImwOtherLibrary.Where(x => x.Active && !x.Deleted).ToList();
            List<DropDownListFilters> mwOtherLibraryFilters = _mapper.Map<List<DropDownListFilters>>(mwOtherLibrary);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("mwOtherLibraryId", mwOtherLibraryFilters));


            var InstallationPlace = _dbContext.TLIinstallationPlace.Where(x => !x.Disable && !x.Deleted).ToList();
            List<DropDownListFilters> InstallationPlacelists = _mapper.Map<List<DropDownListFilters>>(InstallationPlace);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("InstallationPlaceId", InstallationPlacelists));

            return RelatedTables;
        }
    }
}
