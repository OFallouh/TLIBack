using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class EditStepActionApplyCalculationViewModel
    {
        public int Id { get; set; }
        public string label { get; set; }
        public int Period { get; set; }
        public bool CalculateLandSpace { get; set; }
        public bool CalculateLoadSpace { get; set; }
        public List<int> NextStepActions { get; set; }
    }
}
