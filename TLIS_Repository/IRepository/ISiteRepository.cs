using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.wf;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ISiteRepository : IRepositoryBase<TLIsite, SiteViewModel, string>
    {
        void UpdateReservedSpace(string SiteCode, float SpaceInstallation);
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();
        Response<string> CheckSpace(string SiteCode, string TableName, int LibraryId, float SpaceInstallation, string Cabinet);
        Task<SumbitTaskByTLI> SubmitTaskByTLI(int? TaskId);
    }
}
