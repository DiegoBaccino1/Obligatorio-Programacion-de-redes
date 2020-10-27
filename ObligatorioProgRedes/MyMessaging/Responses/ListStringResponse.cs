using Common;
using MyMessaging.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessaging.Responses
{
    public class ListStringResponse : Response
    {
        ListStringDataTransfer transfer = new ListStringDataTransfer();
        protected override byte[] CodeMessage(object obj, Header header)
        {
            return transfer.GenMenssage(obj,header);
        }
    }
}
