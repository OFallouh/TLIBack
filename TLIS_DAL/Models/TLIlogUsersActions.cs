using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIlogUsersActions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public string ControllerName { get; set; }
        public string FunctionName { get; set; }
        public string BodyParameters { get; set; }
        public string HeaderParameters { get; set; }
        public string ResponseStatus { get; set; }
        public string Result { get; set; }
    }
}
