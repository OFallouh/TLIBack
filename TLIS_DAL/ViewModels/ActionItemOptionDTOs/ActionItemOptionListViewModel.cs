using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.ActionItemOptionDTOs
{
    public class ActionItemOptionListViewModel
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public string Name { get; set; }
    }
}
