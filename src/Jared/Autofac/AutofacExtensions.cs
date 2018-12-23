using Autofac;
using System.Reflection;

namespace Jared.Autofac
{
    public static class AutofacExtensions
    {
        public static ContainerBuilder RegisterWorkers(this ContainerBuilder builder)
        {
            // Register all the workers
            builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly())
                   .Where(t => typeof(IWorker).IsAssignableFrom(t));

            return builder;
        }

        public static ContainerBuilder RegisterJobs(this ContainerBuilder builder)
        {
            // Register all the jobs
            builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly())
                   .Where(t => typeof(IJob).IsAssignableFrom(t))
                   .PropertiesAutowired();

            return builder;
        }
    }
}
