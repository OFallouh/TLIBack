using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class CivilLoadsRepository: RepositoryBase<TLIcivilLoads, CivilLoadsViewModel, int>, ICivilLoadsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public CivilLoadsRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddCivilLoad(AddCivilLoadsViewModel civilLoadsViewModel, int allLoadInstId, string SiteCode)
        {
            var Entity = _mapper.Map<TLIcivilLoads>(civilLoadsViewModel);
            Entity.SiteCode = SiteCode;
            Entity.allLoadInstId = allLoadInstId;
            Entity.InstallationDate = Entity.InstallationDate;
            _dbContext.TLIcivilLoads.Add(Entity);
            _dbContext.SaveChanges();
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var SideArms = _dbContext.TLIcivilLoads.Where(x => x.SiteCode == SiteCode && !x.Dismantle && x.sideArmId != null).Select(x => x.sideArmId).ToList();
            List<TLIsideArm> tLIsideArms = new List<TLIsideArm>();
            foreach (var item in SideArms)
            {

                var SideArm = _dbContext.TLIsideArm.Where(x => x.Id == item).FirstOrDefault();
                tLIsideArms.Add(SideArm);

            }
            List<DropDownListFilters> SideArmFilters = _mapper.Map<List<DropDownListFilters>>(tLIsideArms);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("sideArmId", SideArmFilters));

            var CivilInstallations = _dbContext.TLIcivilSiteDate.Where(x => x.SiteCode == SiteCode && !x.Dismantle).Select(x => x.allCivilInst).ToList();
            List<DropDownListFilters> CivilInstallationFilters = new List<DropDownListFilters>();
            foreach(var CivilInstallation in CivilInstallations)
            {
                var Entity = _dbContext.TLIallCivilInst
                                .Where(x => x.Id == CivilInstallation.Id && !x.Draft)
                                .Include(x => x.civilNonSteel)
                                .Include(x => x.civilWithLegs)
                                .Include(x => x.civilWithoutLeg)
                                .FirstOrDefault();
                if(Entity.civilNonSteel != null)
                {
                    CivilInstallationFilters.Add(new DropDownListFilters(Entity.Id, Entity.civilNonSteel.Name));
                }
                else if(Entity.civilWithLegs != null)
                {
                    CivilInstallationFilters.Add(new DropDownListFilters(Entity.Id, Entity.civilWithLegs.Name));
                }
                else if(Entity.civilWithoutLeg != null)
                {
                    CivilInstallationFilters.Add(new DropDownListFilters(Entity.Id, Entity.civilWithoutLeg.Name));
                }
            }
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("allCivilInstId", CivilInstallationFilters));

            var civilSteelSupportCategories = _dbContext.TLIcivilSteelSupportCategory.ToList();
            List<DropDownListFilters> civilSteelSupportCategoryFilters = _mapper.Map<List<DropDownListFilters>>(civilSteelSupportCategories);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("civilSteelSupportCategoryId", civilSteelSupportCategoryFilters));

            return RelatedTables;
        }
    }
}
