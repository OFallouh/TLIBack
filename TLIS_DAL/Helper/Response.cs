using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLIS_DAL.Helpers
{
    public class Response<TModel>
    {
        public Response()
        {
            Succeeded = true;
            Message = "Success";
            Errors = null;
            Code = 0;
        }
        public Response(TModel data, int count = 0)
        {
            Succeeded = true;
            Message = string.Empty;
            Errors = null;
            Data = data;
            Code = 0;
            Count = count;
        }
        public Response(bool succeeded, TModel data, string[] errors, string message, int code, int count = 0)
        {
            if(code == 0 && !string.IsNullOrEmpty(message))
            {
                message = "Success";
            }
            Succeeded = succeeded;
            Message = message;
            Errors = errors;
            Data = data;
            Code = code;
            Count = count;
        }
        public TModel Data { get; set; }
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public int Count { get; set; }
    }
}
