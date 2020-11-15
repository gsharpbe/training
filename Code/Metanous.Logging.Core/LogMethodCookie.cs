using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Metanous.Logging.Core
{
    public sealed class LogMethodCookie : IDisposable
    {
        private const string MessageTemplate = "EXIT {MethodName} responded in {Elapsed:0.0000} ms";

        private readonly ILogger _log;
        private readonly string _methodName;
        private readonly Stopwatch _stopWatch;

        internal LogMethodCookie(ILogger log, string methodName)
        {
            _log = log;
            _methodName = methodName;
            _stopWatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopWatch.Stop();
            _log.LogInformation(MessageTemplate, _methodName, _stopWatch.Elapsed.TotalMilliseconds);
        }
    }
}
