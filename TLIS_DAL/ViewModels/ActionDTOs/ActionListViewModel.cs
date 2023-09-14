using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.ActionDTOs
{
    public class ActionListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ActionType Type { get; set; }
        public StepProposal Proposal { get; set; }
        public List<ListActionOptionViewModel> ActionOptions { get; set; }
        public List<ListActionItemOptionViewModel> ActionItemOptions { get; set; }
    }
}
