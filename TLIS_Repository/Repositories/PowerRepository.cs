using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class PowerRepository: RepositoryBase<TLIpower, PowerViewModel, int>, IPowerRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public PowerRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var owners = _dbContext.TLIowner.Where(x =>!x.Deleted).ToList();

            List<DropDownListFilters> ownerLists = _mapper.Map<List<DropDownListFilters>>(owners);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ownerId", ownerLists));

           var InstallationPlaces = _dbContext.TLIinstallationPlace.Where(x => !x.Deleted).ToList();

            List<DropDownListFilters> InstallationPlaceLists = _mapper.Map<List<DropDownListFilters>>(InstallationPlaces);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("installationPlaceId", InstallationPlaceLists));

             var powerLibrary = _dbContext.TLIpowerLibrary.Where(x => x.Deleted == false).ToList();


            List<DropDownListFilters> powerLibraryLists = _mapper.Map<List<DropDownListFilters>>(powerLibrary);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("powerLibraryId", powerLibraryLists));

            var powerType = _dbContext.TLIpowerType.Where(x => x.Delete == false).ToList();

            List<DropDownListFilters> powerTypeLists = _mapper.Map<List<DropDownListFilters>>(powerType);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("powerTypeId", powerTypeLists));
            return RelatedTables;
        }
    }
}
