using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper.Enums;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.LogDTOs
{
    public class LogViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ActionType { get; set; }
        public string AffectedTable { get; set; }
        public int? RecordId { get; set; }
        public DateTime Date { get; set; }
    }
}
