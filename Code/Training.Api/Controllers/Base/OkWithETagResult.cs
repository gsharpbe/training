using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Training.Api.Controllers.Base
{
    public class OkWithETagResult : OkObjectResult
    {
        public OkWithETagResult(object content) : base(content) { }
        
        public string ETagValue { get; set; }
        public DateTime? LastModified { get; set; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.Headers["ETag"] = ETagValue;
            if (LastModified.HasValue)
            {
                context.HttpContext.Response.Headers["LastModified"] = LastModified.Value.ToString(CultureInfo.InvariantCulture);
            }

            return base.ExecuteResultAsync(context);
        }
    }
}