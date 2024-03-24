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
using static TLIS_Repository.Repositories.SiteRepository;

namespace TLIS_Repository.IRepository
{
    public interface ISiteRepository : IRepositoryBase<TLIsite, SiteViewModel, string>
    {
        void UpdateReservedSpace(string SiteCode, float SpaceInstallation);
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();
        Response<string> CheckSpace(string SiteCode, string TableName, int LibraryId, float SpaceInstallation, string Cabinet);
        Task<SumbitsTaskByTLI> SubmitTaskByTLI(int? TaskId);
        Task<SumbitsTaskByTLI> EditTicketInfoByTLI(EditTicketInfoBinding editTicketInfoBinding);
    }
}
