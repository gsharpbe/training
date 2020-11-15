using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Metanous.Logging.Core
{
    public static class LogExtensions
    {
        public static LogMethodCookie LogMethodEnter(this ILogger log, MethodParameter[] parameters, [CallerMemberName]string methodName = null)
        {
            var formattedParams = string.Join(", ", parameters.Select(it => it.ToString()));

            var messageTemplate = "ENTER {MethodName} " + formattedParams;

            var args = new List<object>{methodName};
            args.AddRange(parameters.Select(x => x.Value));

            log.LogInformation(messageTemplate, args.ToArray());

            return new LogMethodCookie(log, methodName);
        }

        public static LogMethodCookie LogMethodEnter(this ILogger log, [CallerMemberName]string methodName = null)
        {
            var messageTemplate = "ENTER {MethodName}";

            log.LogInformation(messageTemplate, methodName);

            return new LogMethodCookie(log, methodName);
        }

        public static SerilogLogMethodCookie LogMethodEnter(this Serilog.ILogger log, [CallerMemberName]string methodName = null)
        {
            var messageTemplate = "ENTER {MethodName}";

            log.Information(messageTemplate, methodName);

            return new SerilogLogMethodCookie(log, methodName);
        }
    }
}
