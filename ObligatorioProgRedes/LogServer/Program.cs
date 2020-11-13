using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consumer;
using Consumers;
using LogServerImp;
using LogServerImp.LogServerRabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogMain
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsumerSuper consumer = new ConsumerTest();
            ILogServer logServer = new LogServerRabbitMQ();
            logServer.ConsumeLog(consumer);
        }
    }
}
