using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.OwnerDTOs
{
    public class EditOwnerViewModel
    {
        public int Id { get; set; }
        public string OwnerName { get; set; }
        public string Remark { get; set; }
        public bool Deleted { get; set; }
        public bool Disable { get; set; }
    }
}
