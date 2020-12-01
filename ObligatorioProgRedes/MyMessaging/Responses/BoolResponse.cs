using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessaging.Responses
{
    public class BoolResponse : Response
    {
        BoolDataTransform transfer = new BoolDataTransform();
        protected override byte[] CodeMessage(object obj, Header header)
        {
            return transfer.GenMenssage(obj,header);
        }
    }
}
