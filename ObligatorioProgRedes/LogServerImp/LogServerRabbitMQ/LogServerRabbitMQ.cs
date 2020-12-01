using Consumers;
using Entities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogServerImp.LogServerRabbitMQ
{
    public class LogServerRabbitMQ : ILogServer
    {
        public override object ConsumeLog(ConsumerSuper consumer)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                DeclareQueue(channel);
                consumer.Consume(channel,QUEUE_NAME_SUCCESS);
                Console.ReadLine();
            }
            return null;
        }

        private static void DeclareQueue(IModel channel)
        {
            channel.QueueDeclare(queue: QUEUE_NAME_SUCCESS,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public override List<string> GetLogs()
        {
            return null;
        }

        public override void PublishLog(Log message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: QUEUE_NAME_SUCCESS,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                SendLog(channel, message);
            }
        }
        private void SendLog(IModel channel, Log log)
        {
            try
            {
                string logString = JsonConvert.SerializeObject(log);
                var logBody = Encoding.UTF8.GetBytes(logString);
                channel.BasicPublish(exchange: "",
                    routingKey: "Success",
                    basicProperties: null,
                    body: logBody);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
