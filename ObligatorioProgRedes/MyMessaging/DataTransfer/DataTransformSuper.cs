using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyMessaging
{
    public abstract class DataTransformSuper
    {
        protected abstract byte[] CastMessage(object obj);
        public abstract object DecodeMessage(byte[] data);
        public byte[] GenMenssage(object message, Header header)
        {
            byte[] codedMessage = CastMessage(message);
            byte[] headerBytes = header.GenRequest();

            byte[] fullMessage = new byte[headerBytes.Length + codedMessage.Length];
            Array.Copy(headerBytes, 0, fullMessage, 0, headerBytes.Length);
            Array.Copy(codedMessage, 0, fullMessage, headerBytes.Length, codedMessage.Length);

            return fullMessage;
        }

        

    }
}
