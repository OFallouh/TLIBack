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
    public interface IDynamicAttRepository : IRepositoryBase<TLIdynamicAtt, DynamicAttViewModel, int>
    {
        IEnumerable<DynamicAttLibViewModel> GetDynamicLibAtts(int TableNameId, int? CategoryId);
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();
        IEnumerable<DynaminAttInstViewModel> GetDynamicInstAtts(int TableNameId, int? CategoryId);
        IEnumerable<BaseInstAttViewDynamic> GetDynamicInstAttInst(int TableNameId, int? CategoryId);
    }
}
