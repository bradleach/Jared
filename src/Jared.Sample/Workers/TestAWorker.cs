using Jared.Sample.Services;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Context;
using System.Threading.Tasks;

namespace Jared.Sample.Workers
{
    class TestAWorker : IWorker
    {
        private readonly IExampleService exampleService;
        private readonly IConfiguration configuration;

        public TestAWorker(IExampleService exampleService, IConfiguration configuration)
        {
            this.exampleService = exampleService;
            this.configuration = configuration;
        }

        public async Task Execute()
        {
            using (LogContext.PushProperty("Worker", "TestAWorker"))
            using (var tracker = Tracker.Start())
            {
                Log.Information(configuration["Example"]);
                Log.Information(configuration["TestAWorker:SomeProperty"]);
                Log.Information("Hello TestAWorker");
            }
        }
    }
}
