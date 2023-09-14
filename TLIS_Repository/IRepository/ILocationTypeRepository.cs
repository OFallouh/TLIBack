using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ILocationTypeRepository:IRepositoryBase<TLIlocationType, LocationTypeViewModel, int>
    {
    }
}
