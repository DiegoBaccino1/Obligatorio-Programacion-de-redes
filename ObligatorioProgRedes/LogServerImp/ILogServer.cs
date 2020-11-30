using Consumers;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogServerImp
{
    public abstract class ILogServer
    {
        public const string QUEUE_NAME = "LogQueue";
        public abstract void PublishLog(Log log);
        public abstract object ConsumeLog(ConsumerSuper consumer);
    }
}
