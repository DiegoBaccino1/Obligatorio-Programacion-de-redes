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
        public void Consume(IModel channel,string queueName)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = GetMessage(body);
                ProcessMessage(message);
            };
            channel.BasicConsume(queue: queueName, autoAck: false, consumer);
        }
        protected abstract object GetMessage(byte[] body);
        protected abstract object ProcessMessage(object message);
    }
}
