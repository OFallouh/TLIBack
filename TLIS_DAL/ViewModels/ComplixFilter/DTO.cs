using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.SideArmDTOs;

namespace TLIS_DAL.ViewModels.ComplixFilter
{
    public class DTO
    {
        public AddSideArmInstallationDTO PageIndex { get; set; } 
        public string SiteCode { get; set; } 
        public string TaskId { get; set; } 
    }
}
