using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class StepActionMailFromViewModel
    {
        public int Id { get; set; }
        public int? GroupId { get; set; }
        public int? ActorId { get; set; }
        public int? IntegrationId { get; set; }
        public int? UserId { get; set; }
    }
}
