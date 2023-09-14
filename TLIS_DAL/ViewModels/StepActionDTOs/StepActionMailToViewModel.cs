using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class StepActionMailToViewModel
    {
        public int Id { get; set; }
        public MailtType Type { get; set; }
        public int StepActionMailId { get; set; }
        public int StepActionId { get; set; }
    }
}
