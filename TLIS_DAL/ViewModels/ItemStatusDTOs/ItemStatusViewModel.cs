using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.ItemStatusDTOs
{
    public class ItemStatusViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool Active { get; set; }
    }
}
