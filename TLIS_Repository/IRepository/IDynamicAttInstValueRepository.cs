using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IDynamicAttInstValueRepository:IRepositoryBase<TLIdynamicAttInstValue, DynamicAttInstValueViewModel,int>
    {
        void AddDdynamicAttributeInstallations(int UserId, List<AddDdynamicAttributeInstallationValueViewModel> addDynamicInstAttValue, int TableNameId, int Id, string connectionString);
        List<DynaminAttInstViewModel> GetDynamicInstAtts(int TableNameId, int Id, int? CategoryId);
        void AddDynamicInstAtts(AddDynamicAttInstValueViewModel addDynamicInstAttValue, int TableNameId, int Id);
        void UpdateDynamicValue(List<BaseInstAttView> DynamicInstAttsValue, int TableNameId, int InstId);
        List<BaseInstAttViewDynamic> GetDynamicInstAtt(int TableNameId, int Id, int? CategoryId = null);
        void AddDdynamicAttributeInstallation(int UserId, AddDdynamicAttributeInstallationValueViewModel addDynamicInstAttValue, int TableNameId, int Id, string connectionString);
        void UpdateDynamicValues(int UserId, List<AddDdynamicAttributeInstallationValueViewModel> DynamicInstAttsValue, int TableNameId, int InstId, string connectionString);
    }
}
