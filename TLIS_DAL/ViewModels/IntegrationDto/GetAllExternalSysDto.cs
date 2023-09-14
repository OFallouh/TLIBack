using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.IntegrationDto
{
    public class GetAllExternalSysDto
    {
        public int Id { get; set; }
        public string SysName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IP { get; set; }
        public bool IsActive { get; set; }
        public string Token { get; set; }
        public DateTime StartLife { get; set; }
        public DateTime EndLife { get; set; }
        public int LifeTime { get; set; }

        public List<InternalApiPermission> InternalApiPermissions { get; set; }

        public class InternalApiPermission
        {
            public int Id { get; set; }

            public string ApiLabel { get; set; }
        }

    }
}
