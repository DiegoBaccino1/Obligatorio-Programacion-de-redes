using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumers
{
    public abstract class ConsumerSuper
    {
        public void Consume(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = GetMessage(body);
                Console.WriteLine(message);
                ProcessMessage(message);
            };
            channel.BasicConsume(queue: "Success", autoAck: false, consumer);
        }
        protected abstract object GetMessage(byte[] body);
        protected abstract object ProcessMessage(object message);
    }
}
