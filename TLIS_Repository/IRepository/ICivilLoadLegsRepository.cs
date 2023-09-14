using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilLoadLegsDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ICivilLoadLegsRepository:IRepositoryBase<TLIcivilLoadLegs, CivilLoadLegsViewModel, int>
    {
    }
}
