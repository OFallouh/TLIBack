using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.ItemStatusDTOs
{
    public class ListItemStatusViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool active { get; set; }
    }
}
