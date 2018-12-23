using Jared.Sample.Workers;
using Serilog;
using System;
using System.Collections.Generic;

namespace Jared.Sample.Jobs
{
    /// <summary>
    /// This is an example of a typical job. It can be started from the command line by using the -t or the --typical command line options.
    /// </summary>
    [Option('t', "typical", IsDefaultJob = true)]
    public class TypicalJob : ManualSortJob
    {
        /// <summary>
        /// This job runs the workers below in the order they appear in the array. This is useful to manually
        /// control so that workers run in the correct order.
        /// </summary>
        public override IEnumerable<Type> Workers => new Type[] { typeof(TestAWorker), typeof(TestBWorker) };

        /// <summary>
        /// Usually you would override this method and implement custom logging similar to what is presented below.
        /// You would generally enter a good name for the rolling log and appropriate subject for email alerts.
        /// </summary>
        public override ILogger ConfigureLogger()
        {
            return new LoggerConfiguration()
                       .Enrich.FromLogContext()
                       .WriteTo.Console()
                       .WriteTo.File("Typical-.log", rollingInterval: RollingInterval.Day)
                       //.WriteTo.EmailOnProcessExit(LogEventLevel.Error, "[Project][Error] TypicalJob", "alerts@example.com", "donotreply@example.com")
                       .CreateLogger();
        }
    }
}
