using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.UserDTOs
{
    public class LoginViewModel
    {
        [Required]
        public string Wedcto { get; set; }
        [DataType(DataType.Password)]
        public string beresd{ get; set; }
        public string Yuqrgh { get; set; }
    }
}
