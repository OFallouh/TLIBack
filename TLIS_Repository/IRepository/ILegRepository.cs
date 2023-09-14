using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.LegDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ILegRepository : IRepositoryBase<TLIleg, LegViewModel, int>
    {
        void AddLegs(List<AddLegViewModel> LegsViewModel, int CivilWithLegId);

    }
}
