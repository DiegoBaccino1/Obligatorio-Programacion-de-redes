using Consumers;
using Entities;
using Newtonsoft.Json;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumers
{
    public class ConsumerRepository : ConsumerSuper
    {
        public ConsumerRepository()
        {
        }
        protected override object GetMessage(byte[] body)
        {
            string logString = Encoding.UTF8.GetString(body);
            Log log = JsonConvert.DeserializeObject<Log>(logString);
            return log;
        }

        protected override object ProcessMessage(object message)
        {
            try
            {
                SingletonRepository.GetLog().Add(message as Log);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        protected override void SetACK()
        {
            this.ACK=true;
        }
    }
}
