using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.SubTypeDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class CivilWithoutLegRepository: RepositoryBase<TLIcivilWithoutLeg, CivilWithoutLegViewModel, int>, ICivilWithoutLegRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public CivilWithoutLegRepository(ApplicationDbContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var Owners = _context.TLIowner.Where(x => !x.Deleted && !x.Disable).ToList();
            List<DropDownListFilters> OwnerLists = _mapper.Map<List<DropDownListFilters>>(Owners);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("OwnerId", OwnerLists));

            var Subtype = _context.TLIsubType.Where(x => !x.Disable && !x.Delete).ToList();
            List<DropDownListFilters> SubTypeList = _mapper.Map<List<DropDownListFilters>>(Subtype);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("subTypeId", SubTypeList));

            var CivilWithoutLegLibraries = _context.TLIcivilWithoutLegLibrary.Where(x => x.Active && !x.Deleted).ToList();
            List<DropDownListFilters> CivilWithoutLegLibraryLists = _mapper.Map<List<DropDownListFilters>>(CivilWithoutLegLibraries);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("CivilWithoutlegsLibId", CivilWithoutLegLibraryLists));

            var EquipmentsLocation = Enum.GetNames(typeof(EquipmentsLocation)).ToList();
            List<SubTypeViewModel> OutPut = new List<SubTypeViewModel>();
            if (EquipmentsLocation.Exists(x => x == "Body"))
            {
                SubTypeViewModel SubType = new SubTypeViewModel();
                SubType.Id = 0;
                SubType.Name = "Body";
                OutPut.Add(SubType);
            }
            if (EquipmentsLocation.Exists(x =>x == "Platform"))
            {
                SubTypeViewModel SubType = new SubTypeViewModel();
                SubType.Id = 1;
                SubType.Name = "Platform";
                OutPut.Add(SubType);
            }
            if (EquipmentsLocation.Exists(x => x == "Together"))
            {
                SubTypeViewModel SubType = new SubTypeViewModel();
                SubType.Id = 2;
                SubType.Name = "Together";
                OutPut.Add(SubType);
            }
            List<DropDownListFilters> equipmentsLocation = _mapper.Map<List<DropDownListFilters>>(OutPut);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("equipmentsLocation", equipmentsLocation));

            //var availabilityOfWorkPlatforms = Enum.GetNames(typeof(AvailabilityOfWorkPlatforms)).ToList();
            //List<SubTypeViewModel> outPut = new List<SubTypeViewModel>();
            //if (availabilityOfWorkPlatforms.Exists(x => x == "Yes"))
            //{
            //    SubTypeViewModel SubType = new SubTypeViewModel();
            //    SubType.Id = 0;
            //    SubType.Name = "Yes";
            //    outPut.Add(SubType);
            //}
            //if (availabilityOfWorkPlatforms.Exists(x => x == "No"))
            //{
            //    SubTypeViewModel SubType = new SubTypeViewModel();
            //    SubType.Id = 1;
            //    SubType.Name = "No";
            //    outPut.Add(SubType);
            //}
           
            //List<DropDownListFilters> AvailabilityOfWorkPlatforms = _mapper.Map<List<DropDownListFilters>>(outPut);
            //RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("availabilityOfWorkPlatforms", AvailabilityOfWorkPlatforms));

            var LadderSteps = Enum.GetNames(typeof(LadderSteps)).ToList();
            List<SubTypeViewModel> output = new List<SubTypeViewModel>();
            if (LadderSteps.Exists(x => x == "Ladder"))
            {
                SubTypeViewModel SubType = new SubTypeViewModel();
                SubType.Id = 0;
                SubType.Name = "Ladder";
                output.Add(SubType);
            }
            if (LadderSteps.Exists(x => x == "Steps"))
            {
                SubTypeViewModel SubType = new SubTypeViewModel();
                SubType.Id = 1;
                SubType.Name = "Steps";
                output.Add(SubType);
            }

            List<DropDownListFilters> ladderSteps = _mapper.Map<List<DropDownListFilters>>(output);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ladderSteps", ladderSteps));

            //var Reinforced = Enum.GetNames(typeof(Reinforced)).ToList();
            //List<SubTypeViewModel> outpput = new List<SubTypeViewModel>();
            //if (Reinforced.Exists(x => x == "Yes"))
            //{
            //    SubTypeViewModel SubType = new SubTypeViewModel();
            //    SubType.Id = 0;
            //    SubType.Name = "Yes";
            //    outpput.Add(SubType);
            //}
            //if (Reinforced.Exists(x => x == "No"))
            //{
            //    SubTypeViewModel SubType = new SubTypeViewModel();
            //    SubType.Id = 1;
            //    SubType.Name = "No";
            //    outpput.Add(SubType);
            //}

            //List<DropDownListFilters> reinforced = _mapper.Map<List<DropDownListFilters>>(outpput);
            //RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("reinforced", reinforced));
            return RelatedTables;
        }
    }
}
