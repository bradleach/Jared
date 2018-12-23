using Jared.Sample.Services;
using Serilog;
using Serilog.Context;
using System.Threading.Tasks;

namespace Jared.Sample.Workers
{
    class TestAWorker : IWorker
    {
        private readonly IExampleService exampleService;

        public TestAWorker(IExampleService exampleService)
        {
            this.exampleService = exampleService;
        }

        public async Task Execute()
        {
            using (LogContext.PushProperty("Worker", "TestAWorker"))
            using (var tracker = Tracker.Start())
            {
                Log.Information("Hello TestAWorker");
            }
        }
    }
}
