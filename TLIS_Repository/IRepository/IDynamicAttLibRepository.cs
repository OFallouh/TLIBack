using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IDynamicAttLibRepository: IRepositoryBase<TLIdynamicAttLibValue, DynamicAttLibViewModel, int>
    {
        void AddDynamicLibAtt(int UserId, List<AddDdynamicAttributeInstallationValueViewModel> addDynamicLibAttValues, int TableNameId, int Id, string connectionString);
        void AddDynamicLibAtts(List<AddDynamicLibAttValueViewModel> addDynamicLibAttValues, int TableNameId, int Id);
       
       // void UpdateDynamicLibAtts(List<DynamicAttLibViewModel> DynamicLibAttValues, int TablesNameId, int LibId);
        void UpdateDynamicLibAttsWithHistory(List<DynamicAttLibViewModel> DynamicLibAttValues, int TablesNameId, int LibId, int? UserId, int? TableHistoryId = null, int EntitesId = 0);
        List<DynamicAttLibViewModel> GetDynamicLibAtts(int TableNameId, int Id, int? CategoryId);
      
        void DisableDynamicAttLibValues(int TableNameId, int Id);
        List<BaseInstAttViewDynamic> GetDynamicLibAtt(int TableNameId, int Id, int? CategoryId);
        void UpdateDynamicLibAttsWithHistorys(List<AddDdynamicAttributeInstallationValueViewModel> DynamicLibAttValues, string connectionString, int TablesNameId, int LibId, int? UserId, int? TableHistoryId = null, int EntitesId = 0);
        
        void UpdateDynamicLibAttsWithH(List<AddDdynamicAttributeInstallationValueViewModel> DynamicLibAttValues, string connectionString, int TablesNameId, int LibId, int? UserId, int HistoryId);
        void AddDynamicLibraryAtt(int UserId, List<AddDdynamicAttributeInstallationValueViewModel> addDynamicLibAttValues, int TableNameId, int Id, string connectionString, int HistoryId);
    }
}
