using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ICivilWithLegsRepository : IRepositoryBase<TLIcivilWithLegs,CivilWithLegsViewModel,int>
    {
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();
        Response<float> CheckloadsOnCivil(int allcivilinstId,int? loadid,float Azimuth, float CenterHigh);
        Response<float> CheckAvailableSpaceOnCivil(int allcivilinstId);
        Response<float> Checkspaceload(int allcivilinstId, string TableName, float SpaceInstallation, float CenterHigh, int libraryId, float HBA);
     
    }
}
