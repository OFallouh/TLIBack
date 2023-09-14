using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class RadioAntennaRepository: RepositoryBase<TLIradioAntenna, RadioAntennaViewModel, int>, IRadioAntennaRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public RadioAntennaRepository(ApplicationDbContext context, IMapper mapper):base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            //var owners = _context.TLIowner.Where(x => !x.Disable && !x.Deleted).ToList();
            var owners = _context.TLIowner.ToList();

            List<DropDownListFilters> ownerLists = _mapper.Map<List<DropDownListFilters>>(owners);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ownerId", ownerLists));

            //var InstallationPlaces = _context.TLIinstallationPlace.Where(x => !x.Disable && !x.Deleted).ToList();
            var InstallationPlaces = _context.TLIinstallationPlace.ToList();

            List<DropDownListFilters> InstallationPlaceLists = _mapper.Map<List<DropDownListFilters>>(InstallationPlaces);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("installationPlaceId", InstallationPlaceLists));

            //var RadioAntennaLibraries = _context.TLIradioAntennaLibrary.Where(x => x.Active == true && x.Deleted == false).ToList();
            var RadioAntennaLibraries = _context.TLIradioAntennaLibrary.ToList();

            List<DropDownListFilters> RadioAntennaLibraryLists = _mapper.Map<List<DropDownListFilters>>(RadioAntennaLibraries);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("radioAntennaLibraryId", RadioAntennaLibraryLists));
            return RelatedTables;
        }
    }
}
