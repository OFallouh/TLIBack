using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class MW_ODURepository:RepositoryBase<TLImwODU,MW_ODUViewModel,int>, IMW_ODURepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public MW_ODURepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables(string SiteCode)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var Owners = _context.TLIowner.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> OwnerLists = _mapper.Map<List<DropDownListFilters>>(Owners);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("OwnerId", OwnerLists));

            var MW_Dishes = _context.TLIcivilLoads.Include(x => x.allLoadInst).Where(x => !x.Dismantle && x.allLoadInstId != null &&
                            x.allLoadInst.mwDishId != null).Select(x => x.allLoadInst.mwDishId).ToList();
            List<TLImwDish> b = new List<TLImwDish>();
            foreach (var item in MW_Dishes)
            {
                var dish = _context.TLImwDish.FirstOrDefault(x => x.Id == item);
                b.Add(dish);

            }
       
            List<DropDownListFilters> Mw_DishLists = _mapper.Map<List<DropDownListFilters>>(b);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Mw_DishId", Mw_DishLists));
              

            var OduInstallationType = _context.TLIoduInstallationType.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> OduInstallationTypelists = _mapper.Map<List<DropDownListFilters>>(OduInstallationType);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("OduInstallationTypeId", OduInstallationTypelists));

            var MwODULibrary = _context.TLImwODULibrary.Where(x => x.Active && !x.Deleted).ToList();
            List<DropDownListFilters> MwODULibrarylists = _mapper.Map<List<DropDownListFilters>>(MwODULibrary);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("MwODULibraryId", MwODULibrarylists));
            return RelatedTables;
        }
    }
}
