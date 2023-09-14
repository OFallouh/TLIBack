using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class RadioRRURepository: RepositoryBase<TLIRadioRRU, RadioRRUViewModel, int>, IRadioRRURepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public RadioRRURepository(ApplicationDbContext context, IMapper mapper):base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var owners = _context.TLIowner.Where(x => !x.Disable && !x.Deleted).ToList();
            List<DropDownListFilters> ownerLists = _mapper.Map<List<DropDownListFilters>>(owners);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ownerId", ownerLists));

            var InstallationPlaces = _context.TLIinstallationPlace.Where(x => !x.Disable && !x.Deleted).ToList();
            List<DropDownListFilters> InstallationPlaceLists = _mapper.Map<List<DropDownListFilters>>(InstallationPlaces);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("installationPlaceId", InstallationPlaceLists));

            var RadioAntennaes =_context.TLIcivilLoads.Include(x => x.allLoadInst).Where(x => !x.Dismantle && x.allLoadInstId != null &&
                            x.allLoadInst.radioAntennaId != null).Select(x => x.allLoadInst.radioAntennaId).ToList();
            List<TLIradioAntenna> tLIradioAntennas = new List<TLIradioAntenna>();
            foreach (var item in RadioAntennaes)
            {

                var Radio = _context.TLIradioAntenna.FirstOrDefault(x => x.Id==item);
                tLIradioAntennas.Add(Radio);

            }
            List<DropDownListFilters> RadioAntennaLists = _mapper.Map<List<DropDownListFilters>>(tLIradioAntennas);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("radioAntennaId", RadioAntennaLists));

            var RadioRRULibraries = _context.TLIradioRRULibrary.Where(x => x.Active == true && x.Deleted == false).ToList();
            List<DropDownListFilters> RadioRRULibraryLists = _mapper.Map<List<DropDownListFilters>>(RadioRRULibraries);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("radioRRULibraryId", RadioRRULibraryLists));
            return RelatedTables;
        }
    }
}
