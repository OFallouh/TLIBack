using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.AllLoadInstDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class AllLoadInstRepository: RepositoryBase<TLIallLoadInst, AllLoadInstViewModel, int>, IAllLoadInstRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public AllLoadInstRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int AddAllLoadInst(string TableName, int Id)
        {
            TLIallLoadInst allLoad = new TLIallLoadInst();

            if (TableName == Helpers.Constants.LoadSubType.TLImwBU.ToString())
                allLoad.mwBUId = Id;

            else if (TableName == Helpers.Constants.LoadSubType.TLImwDish.ToString())
                allLoad.mwDishId = Id;

            else if (TableName == Helpers.Constants.LoadSubType.TLImwODU.ToString())
                allLoad.mwODUId = Id;

            else if (TableName == Helpers.Constants.LoadSubType.TLImwRFU.ToString())
                allLoad.mwRFUId = Id;

            else if (TableName == Helpers.Constants.LoadSubType.TLImwOther.ToString())
                allLoad.mwOtherId = Id;

            else if (TableName == Helpers.Constants.LoadSubType.TLIradioAntenna.ToString())
                allLoad.radioAntennaId = Id;

            else if (TableName == Helpers.Constants.LoadSubType.TLIradioRRU.ToString())
                allLoad.radioRRUId = Id;

            else if (TableName == Helpers.Constants.LoadSubType.TLIradioOther.ToString())
                allLoad.radioOtherId = Id;

            else if (TableName == Helpers.Constants.LoadSubType.TLIpower.ToString())
                allLoad.powerId = Id;

            else if (TableName == Helpers.Constants.LoadSubType.TLIloadOther.ToString())
                allLoad.loadOtherId = Id;

            _dbContext.TLIallLoadInst.Add(allLoad);
            _dbContext.SaveChanges();
            return allLoad.Id;
        }
    }
}
