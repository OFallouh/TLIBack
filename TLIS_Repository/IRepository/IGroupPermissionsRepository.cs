using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.GroupPermissions;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IGroupPermissionsRepository : IRepositoryBase<TLIgroupPermissions, NewGroupPermissionsViewModel, int>
    {
    }
}
