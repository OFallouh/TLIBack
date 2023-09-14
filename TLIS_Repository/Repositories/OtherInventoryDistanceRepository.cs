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
        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
            var OtherInventoriesInSite = _context.TLIotherInSite.Where(x=>x.Dismantle==false).Select(x => x.allOtherInventoryInstId).ToList();
            List<DropDownListFilters> OtherInventoriesInSiteFilter = new List<DropDownListFilters>();
            foreach(var OtherInventoryInSite in OtherInventoriesInSite)
            {
                var Entity = _context.TLIallOtherInventoryInst
                                .Where(x => x.Id == OtherInventoryInSite)
                                .Include(x => x.cabinet)
                                .Include(x => x.generator)
                                .Include(x => x.solar)
                                .FirstOrDefault();
                if(Entity.cabinet != null)
                {
                    OtherInventoriesInSiteFilter.Add(new DropDownListFilters(Entity.cabinet.Id, Entity.cabinet.Name));
                }
                else if(Entity.generator != null)
                {
                    OtherInventoriesInSiteFilter.Add(new DropDownListFilters(Entity.generator.Id, Entity.generator.Name));
                }
                else if (Entity.solar != null)
                {
                    OtherInventoriesInSiteFilter.Add(new DropDownListFilters(Entity.solar.Id, Entity.solar.Name));
                }
            }
            OtherInventoriesInSiteFilter.Add(new DropDownListFilters(0, "NA"));

            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceOtherInventoryId", OtherInventoriesInSiteFilter));
            return RelatedTables;
        }


        // new add for refrence from self type
        public List<KeyValuePair<string, List<DropDownListFilters>>> CabientGetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
            var OtherInventoriesInSite = _context.TLIotherInSite.Where(x => x.Dismantle == false).Select(x => x.allOtherInventoryInstId).ToList();
            List<DropDownListFilters> OtherInventoriesInSiteFilter = new List<DropDownListFilters>();
            foreach (var OtherInventoryInSite in OtherInventoriesInSite)
            {
                var Entity = _context.TLIallOtherInventoryInst
                                .Where(x => x.Id == OtherInventoryInSite)
                                .Include(x => x.cabinet)
                                .Include(x => x.generator)
                                .Include(x => x.solar)
                                .FirstOrDefault();
                if (Entity.cabinet != null)
                {
                    OtherInventoriesInSiteFilter.Add(new DropDownListFilters(Entity.cabinet.Id, Entity.cabinet.Name));
                }
               
            }
            OtherInventoriesInSiteFilter.Add(new DropDownListFilters(0, "NA"));

            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceOtherInventoryId", OtherInventoriesInSiteFilter));
            return RelatedTables;
        }

        //************************************solar
        public List<KeyValuePair<string, List<DropDownListFilters>>> SolarGetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
            var OtherInventoriesInSite = _context.TLIotherInSite.Where(x => x.Dismantle == false).Select(x => x.allOtherInventoryInstId).ToList();
            List<DropDownListFilters> OtherInventoriesInSiteFilter = new List<DropDownListFilters>();
            foreach (var OtherInventoryInSite in OtherInventoriesInSite)
            {
                var Entity = _context.TLIallOtherInventoryInst
                                .Where(x => x.Id == OtherInventoryInSite)
                                .Include(x => x.cabinet)
                                .Include(x => x.generator)
                                .Include(x => x.solar)
                                .FirstOrDefault();
                if (Entity.solar != null)
                {
                    OtherInventoriesInSiteFilter.Add(new DropDownListFilters(Entity.solar.Id, Entity.solar.Name));
                }
               
            }
            OtherInventoriesInSiteFilter.Add(new DropDownListFilters(0, "NA"));

            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceOtherInventoryId", OtherInventoriesInSiteFilter));
            return RelatedTables;
        }
        //----------------------------------------------------------generator
        public List<KeyValuePair<string, List<DropDownListFilters>>> GeneratorGetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
            var OtherInventoriesInSite = _context.TLIotherInSite.Where(x => x.Dismantle == false).Select(x => x.allOtherInventoryInstId).ToList();
            List<DropDownListFilters> OtherInventoriesInSiteFilter = new List<DropDownListFilters>();
            foreach (var OtherInventoryInSite in OtherInventoriesInSite)
            {
                var Entity = _context.TLIallOtherInventoryInst
                                .Where(x => x.Id == OtherInventoryInSite)
                                .Include(x => x.cabinet)
                                .Include(x => x.generator)
                                .Include(x => x.solar)
                                .FirstOrDefault();
                if (Entity.generator != null)
                {
                    OtherInventoriesInSiteFilter.Add(new DropDownListFilters(Entity.generator.Id, Entity.generator.Name));

                }

            }
            OtherInventoriesInSiteFilter.Add(new DropDownListFilters(0, "NA"));

            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceOtherInventoryId", OtherInventoriesInSiteFilter));
            return RelatedTables;
        }



    }
}
