using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.wf
{
    public class ApiResponse
    {
        public ApiResponse() { }
        public ApiResponse(object? result, string? errorMessage)
        {
            Result = result;
            ErrorMessage = errorMessage;
        }

        public ApiResponse(object? result, string? errorMessage, int count)
        {
            Result = result;
            ErrorMessage = errorMessage;
            Count = count;
        }

        public object? Result { get; set; }

        public int? Count { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
