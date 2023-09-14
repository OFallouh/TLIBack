using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.WorkFlowGroupDTOs
{
    public class WorkFlowGroupVM
    {
        public int Id { get; set; }
        public int WorkFlowId { get; set; }
        //public TLIworkFlow WorkFlow { get; set; }
        public int? ActorId { get; set; }
        //public TLIactor Actor { get; set; }
        public int? IntegrationId { get; set; }
        //public TLIintegration Integration { get; set; }
        public int? UserId { get; set; }
        //public TLIuser User { get; set; }
        public int? GroupId { get; set; }
        //public TLIgroup Group { get; set; }
    }
}
