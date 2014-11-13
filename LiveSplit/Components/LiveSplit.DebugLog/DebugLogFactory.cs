using LiveSplit.UI.Components;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly:ComponentFactory(typeof(LiveSplit.DebugLog.DebugLogFactory))]
namespace LiveSplit.DebugLog
{
    public class DebugLogFactory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "Debug Log"; }
        }

        public string Description
        {
            get { return "Writes a log of debug information to a file."; }
        }

        public ComponentCategory Category
        {
            get { return ComponentCategory.Other; }
        }

        public IComponent Create(Model.LiveSplitState state)
        {
            var result = new Dictionary<int, string[]>();

            var pid = Process.GetCurrentProcess().Id;

            using (var dataTarget = DataTarget.AttachToProcess(pid, 5000, AttachFlag.Passive))
            {
                string dacLocation = dataTarget.ClrVersions[0].TryGetDacLocation();
                var runtime = dataTarget.CreateRuntime(dacLocation);

                foreach (var t in runtime.Threads)
                {
                    result.Add(
                        t.ManagedThreadId,
                        t.StackTrace.Select(f =>
                        {
                            if (f.Method != null)
                            {
                                return f.Method.Type.Name + "." + f.Method.Name;
                            }

                            return null;
                        }).ToArray()
                    );
                }
            }

            foreach (var thread in result)
            {
                Debug.WriteLine("Thread #" + thread.Key + ":");
                foreach (var method in thread.Value)
                {
                    Debug.WriteLine(method);
                }
            }

            throw new Exception();
        }

        public string UpdateName
        {
            get { return ""; }
        }

        public string UpdateURL
        {
            get { return "http://livesplit.org/update/"; }
        }

        public Version Version
        {
            get { return new Version(); }
        }

        public string XMLURL
        {
            get { return "http://livesplit.org/update/Components/noupdates.xml"; }
        }
    }
}
