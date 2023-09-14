using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.PermissionDTOs
{
    public class PermissionViewModel
    {
        public PermissionViewModel(int Id, string name, string module)
        {
            this.Id = Id;
            this.Name = name;
            this.Module = module;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Module { get; set; }
        public string PermissionType { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
