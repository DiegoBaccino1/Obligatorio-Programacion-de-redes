using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consumer;
using Consumers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogServer
{
    class Program
    {
        List<string> listaTest = new List<string>();
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection=factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Success",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
               
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(message);
                    Console.ReadLine();
                };
                channel.BasicConsume(queue: "Success", autoAck: false, consumer);

                /*
                ConsumerTest consumer = new ConsumerTest();
                AcceptConsumer(consumer,channel);*/
            }
        }
        private static void AcceptConsumer(ConsumerSuper consumer,IModel channel)
        {
            consumer.Consume(channel);
        }
    }
}
