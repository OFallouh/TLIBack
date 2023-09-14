using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.MailTemplateDTOs
{
    public class MailTemplateViewModel
    {
        public int Id { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string Name { get; set; }
        //public bool Deleted { get; set; }
        //public DateTime? DeleteDate { get; set; }
    }
}
