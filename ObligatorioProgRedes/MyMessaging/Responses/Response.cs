using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MyMessaging.DataTransfer;


namespace MyMessaging.Responses
{
    public abstract class Response
    {
        protected abstract byte[] CodeMessage(object obj, Header header);

        public virtual void SendResponse(int command, object data, Socket socket, int dataLength)
        {
            Header header = new Header(HeaderConstants.Response, command, dataLength);
            var byteMessage = CodeMessage(data, header);
            DataTransference.DataTransference.SendData(byteMessage, socket);
        }

    }
}
