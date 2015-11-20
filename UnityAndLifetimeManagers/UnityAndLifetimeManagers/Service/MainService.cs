using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnityAndLifetimeManagers.Service
{
    public interface IAnotherMainService { }

    public interface IMainService : IDisposable { }

    public class MainService : IMainService, IAnotherMainService
    {
        public static int ConstructorCounter { get; private set; }
        public static int DisposeCounter { get; private set; }

        public static void ResetCounters()
        {
            ConstructorCounter = 0;
            DisposeCounter = 0;
        }

        public MainService()
        {
            ConstructorCounter++;
        }

        public void Dispose()
        {
            DisposeCounter++;
        }
    }    
}


