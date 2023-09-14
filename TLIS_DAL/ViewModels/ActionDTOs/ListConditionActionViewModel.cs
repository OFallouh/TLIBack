using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ActionItemOptionDTOs;

namespace TLIS_DAL.ViewModels.ActionDTOs
{
    public class ListConditionActionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public StepProposal Proposal { get; set; }
        public List<ListActionOptionViewModel> ActionOptions { get; set; }
        //public List<TLIactionItemOption> ActionItemOptions { get; set; }
    }
    public class ListCivilDecisionActionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public StepProposal Proposal { get; set; }
        public List<ListActionOptionViewModel> ActionOptions { get; set; }
        public List<ActionItemOptionListViewModel> ActionItemOptions { get; set; }
    }
}
