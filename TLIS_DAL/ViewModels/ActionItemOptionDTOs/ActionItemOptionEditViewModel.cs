using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.ActionItemOptionDTOs
{
    public class ActionItemOptionEditViewModel
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public TLIaction Action { get; set; }
        public string Name { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool Deleted { get; set; }
        public List<TLIstepActionItemOption> StepActinItemOptions { get; set; }
    }
}
