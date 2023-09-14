using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IMW_PortRepository:IRepositoryBase<TLImwPort,MW_PortViewModel,int>
    {
    }
}
