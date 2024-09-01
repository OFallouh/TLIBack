using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
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
        Response<float> CheckAvailableSpaceOnCivil(int AllCivilInst);
        Response<bool> FilterAzimuthAndHeight(
          string? SiteCode,
          int? FirstLegId,
          int? SecondLegId,
          int? CivilwithLegId,
          int? CivilWithoutLegId,
          int? CivilNonSteelId,
          int? FirstSideArmId,
          int? SecondSideArmId,
          float Azimuth,
          float Height,
          int switchValue);
        public Response<bool> EditFilterAzimuthAndHeight(int? MWDishID, int? MWODUID, int? MWRFUID, int? MWBUID, int? MWOTHERID, int? RadioAntennaID
          , int? RadioRRUID, int? RadioOtherID, int? LOADOTHERID, int? PowerID, int? SideArmId, string LoadName,
          string? SiteCode, int? FirstLegId, int? SecondLegId, int? CivilwithLegId, int? CivilWithoutLegId, int? CivilNonSteelId, int? FirstSideArmId, int? SecondSideArmId,
      float Azimuth, float Height, int switchValue);
        Response<List<RecalculatSpace>> RecalculatSpace(int CivilId, string CivilType);
        Response<float> CheckAvailableSpaceOnCivils(TLIallCivilInst AllCivilInst);
        Response<float> Checkspaceload(int allcivilinstId, string TableName, float SpaceInstallation, float CenterHigh, int libraryId, float HBA);
        Response<AddDynamicObject> CheckDynamicValidationAndDependence(int DynamicAttributeId, object value, int RecordId, int HistoryId);
        Response<AddDynamicObject> EditCheckDynamicValidationAndDependence(int DynamicAttributeId, object value, int RecordId, int HistoryId);

    }
}
