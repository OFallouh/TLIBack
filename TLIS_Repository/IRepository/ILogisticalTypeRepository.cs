using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.LogisticalTypeDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ILogisticalTypeRepository:IRepositoryBase<TLIlogisticalType,LogisticalTypeViewModel,int>
    {
    }
}
