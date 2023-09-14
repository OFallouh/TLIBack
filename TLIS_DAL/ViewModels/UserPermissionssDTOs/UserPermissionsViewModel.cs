using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.UserPermissionssDTOs
{
    public class UserPermissionsViewModel
    {
        public int Id { get; set; }
        public int Permission_Id { get; set; }
        public string Permission_Name { get; set; }
        public int User_Id { get; set; }
        public string User_Name { get; set; }
        public bool IsActive { get; set; }
    }
}
