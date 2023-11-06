using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class OtherInventoryDistanceRepository: RepositoryBase<TLIotherInventoryDistance, OtherInventoryDistanceViewModel, int>, IOtherInventoryDistanceRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public OtherInventoryDistanceRepository(ApplicationDbContext context, IMapper mapper):base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public List<KeyValuePair<string, List<DropDownListFilters>>> CabientGetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            List<DropDownListFilters> OtherInventoriesInSite = _context.TLIotherInSite
                .Include(x => x.allOtherInventoryInst)
                .Include(x => x.allOtherInventoryInst.generator)
                .Include(x => x.allOtherInventoryInst.solar)
                .Include(x => x.allOtherInventoryInst.cabinet)
                .Where(x => !x.Dismantle && !x.allOtherInventoryInst.Draft &&
                    x.SiteCode.ToLower() == SiteCode.ToLower()).Select(x => new DropDownListFilters()
                    {
                        Deleted = false,
                        Disable = false,
                        Id = x.allOtherInventoryInstId,
                        Value = x.allOtherInventoryInst.cabinetId != null ? x.allOtherInventoryInst.cabinet.Name :
                            (x.allOtherInventoryInst.solarId != null ? x.allOtherInventoryInst.solar.Name :
                            x.allOtherInventoryInst.generator.Name)

                    }).ToList();

            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceOtherInventoryId", OtherInventoriesInSite));
            return RelatedTables;
        }
        public List<KeyValuePair<string, List<DropDownListFilters>>> SolarGetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            List<DropDownListFilters> OtherInventoriesInSite = _context.TLIotherInSite
                .Include(x => x.allOtherInventoryInst)
                .Include(x => x.allOtherInventoryInst.generator)
                .Include(x => x.allOtherInventoryInst.solar)
                .Include(x => x.allOtherInventoryInst.cabinet)
                .Where(x => !x.Dismantle && !x.allOtherInventoryInst.Draft &&
                    x.SiteCode.ToLower() == SiteCode.ToLower()).Select(x => new DropDownListFilters()
                    {
                        Deleted = false,
                        Disable = false,
                        Id = x.allOtherInventoryInstId,
                        Value = x.allOtherInventoryInst.cabinetId != null ? x.allOtherInventoryInst.cabinet.Name :
                            (x.allOtherInventoryInst.solarId != null ? x.allOtherInventoryInst.solar.Name :
                            x.allOtherInventoryInst.generator.Name)

                    }).ToList();

            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceOtherInventoryId", OtherInventoriesInSite));
            return RelatedTables;
        }
        public List<KeyValuePair<string, List<DropDownListFilters>>> GeneratorGetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            List<DropDownListFilters> OtherInventoriesInSite = _context.TLIotherInSite
                .Include(x => x.allOtherInventoryInst)
                .Include(x => x.allOtherInventoryInst.generator)
                .Include(x => x.allOtherInventoryInst.solar)
                .Include(x => x.allOtherInventoryInst.cabinet)
                .Where(x => !x.Dismantle && !x.allOtherInventoryInst.Draft &&
                    x.SiteCode.ToLower() == SiteCode.ToLower()).Select(x => new DropDownListFilters()
                    {
                        Deleted = false,
                        Disable = false,
                        Id = x.allOtherInventoryInstId,
                        Value = x.allOtherInventoryInst.cabinetId != null ? x.allOtherInventoryInst.cabinet.Name :
                            (x.allOtherInventoryInst.solarId != null ? x.allOtherInventoryInst.solar.Name : 
                            x.allOtherInventoryInst.generator.Name)
                        
                    }).ToList();

            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceOtherInventoryId", OtherInventoriesInSite));
            return RelatedTables;
        }
    }
}
