﻿using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.UserPermissionDTOs;

namespace TLIS_DAL.ViewModels.UserDTOs
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string AdGUID { get; set; }
        public int UserType { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool IsFirstLogin { get; set; }
        public string ConfirmationCode { get; set; }
        public bool ValidateAccount { get; set; }
        public string WorkFlowMode { get; set; }
        public string Permissions { get; set; }
        public List<String> PermissionsUser { get; set; }
        public List<PermissionsGroup> PermissionsRole { get; set; }
        public List<GroupNamesViewModel> Groups { get; set; }
    }

    public class PermissionsGroup
    {
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public List<string> PermissionsOfGroup { get; set; }

    }
}
