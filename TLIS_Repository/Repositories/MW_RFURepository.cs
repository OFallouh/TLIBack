using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    class MW_RFURepository:RepositoryBase<TLImwRFU,MW_RFUViewModel,int>, IMW_RFURepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public MW_RFURepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var MwRFULibrary = _context.TLImwRFULibrary.Where(x => x.Active && !x.Deleted).ToList();
            List<DropDownListFilters> MwRFULibraryLists = _mapper.Map<List<DropDownListFilters>>(MwRFULibrary);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("MwRFULibraryId", MwRFULibraryLists));

            var MwPort = _context.TLImwPort.ToList();
            List<DropDownListFilters> MwPortLists = _mapper.Map<List<DropDownListFilters>>(MwPort);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("MwPortId", MwPortLists));

            var Owner = _context.TLIowner.Where(x => !x.Disable && !x.Deleted).ToList();
            List<DropDownListFilters> OwnerLists = _mapper.Map<List<DropDownListFilters>>(Owner);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("OwnerId", OwnerLists));
            return RelatedTables;
        }
    }
}
