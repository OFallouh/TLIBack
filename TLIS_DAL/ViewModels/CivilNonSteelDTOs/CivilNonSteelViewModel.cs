using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.CivilNonSteelDTOs
{
    public class CivilNonSteelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double CurrentLoads { get; set; }
        public float SpaceInstallation { get; set; }
        public int CivilNonSteelLibraryId { get; set; }
        public string CivilNonsteelLibrary_Name { get; set; }
        public int? ownerId { get; set; }
        public string owner_Name { get; set; }
        public int? supportTypeImplementedId { get; set; }
        public string supportTypeImplemented_Name { get; set; }
        public int? locationTypeId { get; set; }
        public string locationType_Name { get; set; }
        public float locationHeight { get; set; }
        public float BuildingMaxLoad { get; set; }
        public float CivilSupportCurrentLoad { get; set; }
        public float H2Height { get; set; }
        public float Support_Limited_Load { get; set; }
        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }
    }
}
