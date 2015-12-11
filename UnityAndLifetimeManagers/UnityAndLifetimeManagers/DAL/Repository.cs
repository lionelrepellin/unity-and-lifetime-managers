using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnityAndLifetimeManagers.DAL
{
    public interface IRepository : IDisposable
    {
        string SayHelloWorld();
    }

    public class Repository : IRepository
    {
        public Repository()
        {

        }
        
        public void Dispose()
        {
            
        }

        public string SayHelloWorld()
        {
            return "Hello World !";
        }
    }
}