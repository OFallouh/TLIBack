using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class SolarRepository: RepositoryBase<TLIsolar, SolarViewModel, int>, ISolarRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public SolarRepository(ApplicationDbContext context, IMapper mapper):base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            List<TLIcabinet> TLIcabinet = new List<TLIcabinet>();
            var Cabinet = _context.TLIotherInSite.Include(x => x.allOtherInventoryInstId).Where(x => x.SiteCode == SiteCode && !x.Dismantle && x.allOtherInventoryInstId != null &&
              x.allOtherInventoryInst.cabinetId != null).Select(x => x.allOtherInventoryInst.cabinetId).ToList();
           
            foreach (var item in Cabinet)
            {

                var cabinetname = _context.TLIcabinet.Where(x => x.Id == item).FirstOrDefault();
                TLIcabinet.Add(cabinetname);

            }

            List<DropDownListFilters> CabinetLists = _mapper.Map<List<DropDownListFilters>>(TLIcabinet);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("CabinetId", CabinetLists));

            var SolarLibraries = _context.TLIsolarLibrary.Where(x => x.Active == true && x.Deleted == false).ToList();
            List<DropDownListFilters> SolarLibraryLists = _mapper.Map<List<DropDownListFilters>>(SolarLibraries);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("SolarLibraryId", SolarLibraryLists));
            return RelatedTables;
        }
    }
}
