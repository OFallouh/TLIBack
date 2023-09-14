using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class GeneratorRepository: RepositoryBase<TLIgenerator, GeneratorViewModel, int>, IGeneratorRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public GeneratorRepository(ApplicationDbContext context, IMapper mapper):base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            var baseGeneratorTypes = _context.TLIbaseGeneratorType.Where(x => !x.Disable && !x.Deleted).ToList();
            List<DropDownListFilters> baseGeneratorTypeLists = _mapper.Map<List<DropDownListFilters>>(baseGeneratorTypes);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("BaseGeneratorTypeId", baseGeneratorTypeLists));

            var GeneratorLibraries = _context.TLIgeneratorLibrary.Where(x => x.Active && !x.Deleted).ToList();
            List<DropDownListFilters> GeneratorLibraryLists = _mapper.Map<List<DropDownListFilters>>(GeneratorLibraries);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("GeneratorLibraryId", GeneratorLibraryLists));
            return RelatedTables;
        }
    }
}
