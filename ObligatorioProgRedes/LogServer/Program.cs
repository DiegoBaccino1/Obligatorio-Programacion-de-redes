using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consumer;
using Consumers;
using Entities;
using LogServerImp;
using Repository;

namespace LogServerStart
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsumerSuper consumer = new ConsumerRepository();
            ILogServer logServer = new LogServerRabbitMQ();
            bool exit = false;
            logServer.ConsumeLog(consumer);
            while (!exit) 
            {
                
                List<Log> logs = SingletonRepository.GetLog();
                if ( logs.Count> 0)
                {
                    Console.WriteLine("El Username es: "+ logs[0].Username+"\n");
                    Console.WriteLine("Count de warnings : " + logs.Where(x => x.Level.Equals(Log.SUCCESS_LEVEL)).ToList().Count);
                    Console.WriteLine("Count de success : " + logs.Where(x=>x.Level.Equals(Log.WARNING_LEVEL)).ToList().Count);
                    exit = true;
                }
            }
            Console.ReadLine();
        }
    }
}
