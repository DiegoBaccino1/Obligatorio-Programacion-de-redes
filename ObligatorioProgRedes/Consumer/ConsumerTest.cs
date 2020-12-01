using Consumers;
using Entities;
using Newtonsoft.Json;
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
            Log log = new Log();
            string logString = Encoding.UTF8.GetString(body);
            log = JsonConvert.DeserializeObject<Log>(logString);
            return log;
        }

        protected override object ProcessMessage(object message)
        {
            try
            {
                Log log = (Log)message;
                Console.WriteLine("El Username es: "+log.Username);
                return null;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        protected override void SetACK()
        {
            this.ACK=false;
        }
    }
}
