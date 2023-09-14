using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_API
{
    [LayoutRenderer("Custom-Layout")]
    public class CustomlayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            // 1. Date Time.. 
            // Done in NLog.config File..

            // 2. Activity Id..
            Guid ActivityId = Trace.CorrelationManager.ActivityId;
            if (ActivityId != null && ActivityId != Guid.Empty)
                builder.Append($"ActivityId: {ActivityId} | ");
            
            // 3. User Name.. 
            string UserName = Environment.UserName;
            if(!string.IsNullOrEmpty(UserName))
                builder.Append($"UserName: {UserName}");
            else
                builder.Append(" | UserName: Null");

            // 4. Message
            // Done in NLog.config File

            // 5. Log Level
            // Done in NLog.config File

        }
    }
}