using Consumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    public class ConsumerTest : ConsumerSuper
    {
        protected override object GetMessage(byte[] body)
        {
            return Encoding.UTF8.GetString(body);
        }

        protected override object ProcessMessage(object message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
            return null; 
        }
    }
}
