using Consumers;
using Entities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogServerImp
{
    public class LoggerAllInOne : ILogServer
    {
        private const string HOSTNAME = "localhost";
        private static IRepository repository;
        private List<Log> Logs = new List<Log>();
        public LoggerAllInOne(IRepository repo)
        {
            repository = repo;
        }

        public LoggerAllInOne()
        {
        }

        public override object ConsumeLog(ConsumerSuper consumer)
        {
            ConsumeLogAux();
            return null;
        }
        public List<Log> GetLogs()
        {
            return this.Logs;
        }
        public void ConsumeLogAux()
        {
            var factory = new ConnectionFactory() { HostName = HOSTNAME };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: QUEUE_NAME,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    string logString = Encoding.UTF8.GetString(body);
                    var log = JsonConvert.DeserializeObject<Log>(logString);
                    this.Logs.Add(log);
                };
                channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer);
            }
        }
        public override void PublishLog(Log message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: QUEUE_NAME + "/2",
                    durable: false,
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
                    routingKey: QUEUE_NAME + "/2",
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
