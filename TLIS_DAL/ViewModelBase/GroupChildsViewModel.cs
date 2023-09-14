using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.UserDTOs;

namespace TLIS_DAL.ViewModelBase
{
    public class GroupChildsViewModel
    {
        public List<GroupViewModel> Group;
        public List<UserViewModel> User;
    }
}
