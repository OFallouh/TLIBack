using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.ApplicationPool
{
   public class ApplicationPoolBindingModel
    {
        public string AppPoolName { get; set; }

        public string IIS_UserName { get; set; }
        public string IIS_Password { get; set; }
    }
}
