using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SingletonRepository
    {
        private static List<Log> Logs;
        public static List<Log> GetLog()
        {
            if (Logs == null)
                Logs = new List<Log>();
            return Logs;
        } 
    }
}
