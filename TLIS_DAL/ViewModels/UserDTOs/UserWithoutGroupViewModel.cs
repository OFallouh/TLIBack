using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.UserDTOs
{
    public class UserWithoutGroupViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string AdGUID { get; set; }
        public int UserType { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string ConfirmationCode { get; set; }
        public bool ValidateAccount { get; set; }
        public List<String> Permissions { get; set; }
    }
}
