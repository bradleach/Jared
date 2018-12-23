using Serilog;
using System.Threading.Tasks;

namespace Jared.Sample.Workers
{
    class TestBWorker : IWorker
    {
        public async Task Execute()
        {
            Log.Information("Hello TestBWorker");
        }
    }
}
