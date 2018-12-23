using Serilog;
using System;
using System.Diagnostics;

namespace Jared
{
    public class Tracker : IDisposable
    {
        public static Tracker Start(string name = null)
        {
            return new Tracker(name);
        }

        private Tracker(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                var frame = new StackFrame(2);
                var method = frame.GetMethod();
                var type = method.DeclaringType;
                Name = type.Name;
            }
            else
            {
                Name = name;
            }

            WriteLog("Started");
        }

        public string Name { get; private set; }
        public int Updated { get; set; }
        public int Processed { get; set; }

        public void WriteLog(string format, params object[] args)
        {
            Log.Information(format, args);
        }

        public void Dispose()
        {
            WriteLog("Processed {0} records. Updated {1} records.", Processed, Updated);

            WriteLog("Finished");
        }
    }
}
