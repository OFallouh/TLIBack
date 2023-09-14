using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class ActorsName
    {
        public int? ActorId { get; set; }
        public int? IntegrationId { get; set; }
        public int? UserId { get; set; }
        public int? GroupId { get; set; }
        public string UserName { get; set; }
        public string GroupName { get; set; }
        public string ActorName { get; set; }
        public string IntegrationName { get; set; }
    }
}
