using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.ValidationDTOs
{
    public class GeneralValidation
    {
        public int Id { get; set; }
        public int OperationId { get; set; }
        public string OperationName { get; set; }
        public object Value { get; set; }
    }
}
