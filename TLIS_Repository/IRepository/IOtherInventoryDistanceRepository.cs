using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IOtherInventoryDistanceRepository:IRepositoryBase<TLIotherInventoryDistance, OtherInventoryDistanceViewModel, int>
    {
        List<KeyValuePair<string, List<DropDownListFilters>>> CabientGetRelatedTables(string SiteCode);
        List<KeyValuePair<string, List<DropDownListFilters>>> SolarGetRelatedTables(string SiteCode);
        List<KeyValuePair<string, List<DropDownListFilters>>> GeneratorGetRelatedTables(string SiteCode);
    }
}
