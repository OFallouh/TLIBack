using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IRoleRepository:IRepositoryBase<TLIrole, RoleViewModel,int>
    {
        IEnumerable<TLIrole> GetRoles(string filter = null);
        TLIrole AddRole(string RoleName);
        bool CheckRoleNameInDatabaseAdd(string RoleName);
        bool CheckRoleNameInDatabaseUpdate(string RoleName, int RoleId);
        void DeleteRole(int RoleId);
    }
}
