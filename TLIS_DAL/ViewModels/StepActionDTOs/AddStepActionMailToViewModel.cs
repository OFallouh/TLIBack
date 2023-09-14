using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionMailToViewModel
    {
        public MailtType Type { get; set; }
        public int? GroupId { get; set; }
        public int? ActorId { get; set; }
        //public int? IntegrationId { get; set; }
        public int? UserId { get; set; }
    }

}

