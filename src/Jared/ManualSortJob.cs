using Autofac;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jared
{
    public abstract class ManualSortJob : Core.SortJob
    {
        public ILifetimeScope LifetimeScope { get; set; }

        public override async Task<JobResult> Execute()
        {
            try
            {
                Log.Information(JobName + " started");

                var workers = new Queue<Type>();

                string workerSetting = GetWorkersSetting();
                if (!String.IsNullOrWhiteSpace(workerSetting))
                {
                    workers = QueueWorkerManager.QueueWorkers(workerSetting.Split(','), true);
                }
                else
                {
                    // Sort out the order they need to be run in
                    workers = QueueWorkerManager.QueueWorkers(Workers, true);
                }

                bool skipEnabled = false;
                string skipTo = GetSkipToSetting();
                if (!String.IsNullOrWhiteSpace(skipTo))
                    skipEnabled = true;

                var excludeSet = GetExcludeSet();

                Log.Information("Executing the Workers");

                IWorker worker = null;

                while (workers.Count > 0)
                {
                    using (var scope = LifetimeScope.BeginLifetimeScope())
                    {
                        var type = workers.Dequeue();

                        if (skipEnabled)
                        {
                            if (type.Name != skipTo)
                                continue;

                            skipEnabled = false;
                        }

                        // If we exclude the worker via config, then continue to the next
                        if (excludeSet.Contains(type.Name))
                            continue;

                        worker = scope.Resolve(type) as IWorker;

                        Log.Information("-------------------------------------------------------------------");
                        Log.Information("Executing the {0} worker", type.Name);

                        await worker.Execute();

                        worker = null; // new NoOpWorker();
                    }
                }

                Log.Information("Job completed");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unknown error occurred. Please check exception for details.");
                if (ex.InnerException != null)
                    Log.Error(ex.InnerException, "Inner Exception: " + ex.InnerException.ToString());
                return JobResult.Failure;
            }

            return JobResult.Success;

        }

        /// <summary>
        /// The IWorker types that will be instantiated and executed in the order they appear by the job.
        /// </summary>
        public abstract IEnumerable<Type> Workers { get; }
    }
}
