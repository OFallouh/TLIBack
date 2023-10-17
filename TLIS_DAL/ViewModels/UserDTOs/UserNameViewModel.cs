using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.UserDTOs
{
    public class UserNameViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int? UserType { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }


    }
}
