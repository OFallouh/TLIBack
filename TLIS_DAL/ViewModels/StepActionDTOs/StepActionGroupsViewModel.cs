using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.ActorDTOs;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.IntegrationDTOs;
using TLIS_DAL.ViewModels.UserDTOs;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class StepActionGroupsViewModel
    {
        //*
        public List<UserViewModel> users { get; set; }
        public List<GroupViewModel> groups { get; set; }
        public List<ActorViewModel> actors { get; set; }
        public List<IntegrationViewModel> integration { get; set; }
        //*/
        /*
        public UserViewModel users { get; set; }
        public GroupViewModel groups { get; set; }
        public ActorViewModel actors { get; set; }
        public IntegrationViewModel integration { get; set; }

        //*/
    }
    public class StepActionGroupViewModel
    {
        public UserViewModel users { get; set; }
        public GroupViewModel groups { get; set; }
        public ActorViewModel actors { get; set; }
        public IntegrationViewModel integration { get; set; }
    }
}
