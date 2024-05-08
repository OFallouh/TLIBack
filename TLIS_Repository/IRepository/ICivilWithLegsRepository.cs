using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper.Filters;
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
        IDictionary<string, object> BuildDynamicSelect(object obj, Dictionary<string, string>? dynamic, List<string> propertyNamesStatic, Dictionary<string, string> propertyNamesDynamic);
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();
        bool BuildDynamicQuery(List<FilterObjectList> filters, IDictionary<string, object> item);
        Response<float> CheckloadsOnCivil(int allcivilinstId,int? loadid,float Azimuth, float CenterHigh);
        Response<float> CheckAvailableSpaceOnCivil(TLIallCivilInst AllCivilInst);
        Response<float> Checkspaceload(int allcivilinstId, string TableName, float SpaceInstallation, float CenterHigh, int libraryId, float HBA);
     
    }
}
