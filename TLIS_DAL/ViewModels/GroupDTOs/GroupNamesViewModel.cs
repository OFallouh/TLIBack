using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.GroupDTOs
{
    public class GroupNamesViewModel
    {
        public GroupNamesViewModel(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
