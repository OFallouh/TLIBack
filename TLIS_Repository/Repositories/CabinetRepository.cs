using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class CabinetRepository: RepositoryBase<TLIcabinet, CabinetViewModel, int>, ICabinetRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public CabinetRepository(ApplicationDbContext context, IMapper mapper):base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var renewableCabinetTypes = _context.TLIrenewableCabinetType.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> renewableCabinetTypeLists = _mapper.Map<List<DropDownListFilters>>(renewableCabinetTypes);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("RenewableCabinetTypeId", renewableCabinetTypeLists));

            var CabinetPowerLibraries = _context.TLIcabinetPowerLibrary.Where(x => !x.Deleted && x.Active).ToList();
            List<DropDownListFilters> CabinetPowerLibraryLists = _mapper.Map<List<DropDownListFilters>>(CabinetPowerLibraries);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("CabinetPowerLibraryId", CabinetPowerLibraryLists));

            var CabinetTelecomLibraries = _context.TLIcabinetTelecomLibrary.Where(x => !x.Deleted && x.Active).ToList();
            List<DropDownListFilters> CabinetTelecomLibraryLists = _mapper.Map<List<DropDownListFilters>>(CabinetTelecomLibraries);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("CabinetTelecomLibraryId", CabinetTelecomLibraryLists));
            return RelatedTables;
        }
    }
}
