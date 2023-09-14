using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IPowerLibraryRepository : IRepositoryBase<TLIpowerLibrary, PowerLibraryViewModel, int>
    {
        //Task<IEnumerable<TLIpowerLibrary>> GetAll(ParameterPagination parameterPagination);

        //void Add(PowerLibraryViewModel AddPowerLibraryViewModel);
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();

    }
}
