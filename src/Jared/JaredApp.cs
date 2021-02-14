using Autofac;
using Jared.Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Jared
{
    public class JaredApp
    {
        protected IContainer container;
        protected IConfiguration Configuration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<int> Execute(string[] args)
        {
            Log.Logger = ConfigureFallbackLogger();

            InitializeConfiguration();

            Initialize();

            // Setup our Dependency Injection container (Autofac)
            // The user can customize this by overriding ConfigureAutofac()
            container = InitializeAutofac();

            // Build the IJob
            var job = CreateJob(args);
            if (job == null)
                return (int)JobResult.Failure;

            // Do any final configuration and execute the job
            var result = await ConfigureAndExecuteJob(job);

            return (int)result;
        }

        /// <summary>
        /// Initialize is called after the logger has been initialized, but before autofac is configured
        /// and is thus a great place to provide any startup logic for the application.
        /// </summary>
        protected virtual void Initialize()
        { }

        /// <summary>
        /// This method allows your app to register any dependencies that need to be resolved via Autofac
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void ConfigureAutofac(ContainerBuilder builder)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual ILogger ConfigureFallbackLogger()
        {
            return new LoggerConfiguration()
                        .WriteTo.Console()
                        .WriteTo.File(FallbackLogFile, rollingInterval: RollingInterval.Day)
                       .CreateLogger();
        }

        protected virtual void InitializeConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(Environment.GetCommandLineArgs())
                .Build();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual string FallbackLogFile => "Jared.{Date}.log";

        private IContainer InitializeAutofac()
        {
            try
            {
                var builder = new ContainerBuilder();

                builder.RegisterInstance(Configuration);

                // Register all the jobs (anything that implements IJob)
                builder.RegisterJobs();

                // Register all the workers (anything that implements IWorker)
                builder.RegisterWorkers();

                ConfigureAutofac(builder);

                return builder.Build();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error Initializing Autofac");
                return null;
            }
        }

        private IJob CreateJob(string[] args)
        {
            Type jobType = null;
            IJob job;

            try
            {
                var jobManager = new JobManager();

                jobType = jobManager.FindJobTypeFromArguments(args);
                if (jobType == null)
                    return null;

                job = container.Resolve(jobType) as IJob;

                return job;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error creating job {0}", (jobType == null) ? string.Empty : jobType.Name);
                return null;
            }
        }

        private async Task<JobResult> ConfigureAndExecuteJob(IJob job)
        {
            try
            {
                Log.Logger = job.ConfigureLogger();

                return await job.Execute();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error running job");
                return JobResult.Failure;
            }
        }
    }
}
