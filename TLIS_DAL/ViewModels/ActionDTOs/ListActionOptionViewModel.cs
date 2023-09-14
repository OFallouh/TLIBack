using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.ActionDTOs
{
    public class ListActionOptionViewModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public int? ItemStatusId { get; set; }
        public List<ListActionOptionViewModel> SubOptions { get; set; }
    }
}
