using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnityAndLifetimeManagers.Service
{
    public interface IAnotherMainService
    {
        // nothing here...
    }

    public interface IMainService : IDisposable
    {
        // nothing here...
    }

    public class MainService : IMainService, IAnotherMainService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static int ConstructorCounter { get; private set; }
        public static int DisposeCounter { get; private set; }

        public Guid UniqueId { get; private set; }

        public static void ResetCounters()
        {
            ConstructorCounter = 0;
            DisposeCounter = 0;
        }

        public MainService()
        {
            Logger.Debug("Constructor was called");
            UniqueId = Guid.NewGuid();
            ConstructorCounter++;
        }

        public void Dispose()
        {
            Logger.Debug("Dispose was called");
            DisposeCounter++;
        }
    }
}