using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class CivilNonSteelRepository: RepositoryBase<TLIcivilNonSteel, CivilNonSteelViewModel, int>, ICivilNonSteelRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public CivilNonSteelRepository(ApplicationDbContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var CivilNonSteelLibraries = _context.TLIcivilNonSteelLibrary.Where(x => !x.Deleted && x.Active).ToList();
            List<DropDownListFilters> CivilNonSteelLibraryLists = _mapper.Map<List<DropDownListFilters>>(CivilNonSteelLibraries);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("CivilNonSteelLibraryId", CivilNonSteelLibraryLists));

            var owner = _context.TLIowner.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> ownerLists = _mapper.Map<List<DropDownListFilters>>(owner);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ownerId", ownerLists));

            var supportTypeImplemented = _context.TLIsupportTypeImplemented.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> supportTypeImplementedLists = _mapper.Map<List<DropDownListFilters>>(supportTypeImplemented);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("supportTypeImplementedId", supportTypeImplementedLists));

            var locationType = _context.TLIlocationType.Where(x => !x.Disable && !x.Deleted).ToList();
            List<DropDownListFilters> locationTypeLists = _mapper.Map<List<DropDownListFilters>>(locationType);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("locationTypeId", locationTypeLists));
            return RelatedTables;
        }
    }
}
