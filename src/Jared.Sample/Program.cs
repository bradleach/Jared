using Autofac;
using Jared.Sample.Services;
using System;
using System.Threading.Tasks;

namespace Jared.Sample
{
    class Program : JaredApp
    {
        static async Task<int> Main(string[] args)
        {
            var program = new Program();
            return await program.Execute(args);
        }

        protected override void ConfigureAutofac(ContainerBuilder builder)
        {
            builder.RegisterType<ExampleService>().As<IExampleService>();
        }
    }
}
