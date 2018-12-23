using Serilog;
using System.Threading.Tasks;

namespace Jared
{
    /// <summary>
    /// A Job is generally responsible for organising, instantiating, and executing Worker objects.
    /// </summary>
    interface IJob
    {
        /// <summary>
        /// Provides a place to configure the Serilog Logging infrastructure.
        /// </summary>
        /// <returns>The created Serilog ILogged instance</returns>
        ILogger ConfigureLogger();

        /// <summary>
        /// Executes the job.
        /// </summary>
        /// <returns>The JobResult (success or failure) is passed back to application and is useful for monitoring success or failure of jobs.</returns>
        Task<JobResult> Execute();
    }
}
