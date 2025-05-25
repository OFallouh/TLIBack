using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ActionDTOs;
using TLIS_DAL.ViewModels.ActorDTOs;
using TLIS_DAL.ViewModels.AreaDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IBatteryTypeRepository : IRepositoryBase<TLIBatteryType, ActorViewModel, int>
    {
    }
}
