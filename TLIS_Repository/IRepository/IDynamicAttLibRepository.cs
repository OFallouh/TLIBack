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
        void AddDynamicLibAtts(List<AddDynamicLibAttValueViewModel> addDynamicLibAttValues, int TableNameId, int Id);
       // void UpdateDynamicLibAtts(List<DynamicAttLibViewModel> DynamicLibAttValues, int TablesNameId, int LibId);
        void UpdateDynamicLibAttsWithHistory(List<DynamicAttLibViewModel> DynamicLibAttValues, int TablesNameId, int LibId, int? UserId, int? TableHistoryId = null, int EntitesId = 0);
        List<DynamicAttLibViewModel> GetDynamicLibAtts(int TableNameId, int Id, int? CategoryId);

        void DisableDynamicAttLibValues(int TableNameId, int Id);
        List<BaseInstAttViewDynamic> GetDynamicLibAtt(int TableNameId, int Id, int? CategoryId);
        void UpdateDynamicLibAttsWithHistorys(List<AddDdynamicAttributeInstallationValueViewModel> DynamicLibAttValues, int TablesNameId, int LibId, int? UserId, int? TableHistoryId = null, int EntitesId = 0);
        void AddDynamicLibAtt(int UserId, List<AddDdynamicAttributeInstallationValueViewModel> addDynamicLibAttValues, int TableNameId, int Id);
    }
}
