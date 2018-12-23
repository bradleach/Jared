using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jared
{
    public abstract class Job : IJob
    {
        public abstract Task<JobResult> Execute();

        public virtual ILogger ConfigureLogger()
        {
            return new LoggerConfiguration()
                       //.WriteTo.ColoredConsole()
                       .CreateLogger();
        }
    }
}
