using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyMessaging
{
    public abstract class DataTransferSuper
    {
        protected abstract byte[] CastMessage(object obj);
        protected abstract object DecodeMessage(byte[] data);
        public byte[] GenMenssage(object message, Header header)
        {
            byte[] codedMessage = CastMessage(message);
            byte[] headerBytes = header.GenRequest();

            byte[] fullMessage = new byte[headerBytes.Length + codedMessage.Length];
            Array.Copy(headerBytes, 0, fullMessage, 0, headerBytes.Length);
            Array.Copy(codedMessage, 0, fullMessage, headerBytes.Length, codedMessage.Length);

            return fullMessage;
        }

        public static void SendData(byte[] message, Socket socket)
        {
            socket.Send(message, 0, message.Length, SocketFlags.None);
        }

        public DataTransferResult RecieveData(Socket socket)
        {
            DataTransferResult result = new DataTransferResult();
            int dataLength;

            int headerLength = HeaderConstants.GetLength();
            var headerBytes = new byte[headerLength];
            int received = 0;

            while (received < headerLength)
            {
                received += socket.Receive(headerBytes, received, headerLength - received, SocketFlags.None);

            }

            Header header = new Header(headerBytes);
            result.Header = header;
            dataLength = header.GetDataLength();

            var data = new byte[dataLength];
            received = 0;
            while (received < dataLength)
            {
                received += socket.Receive(data, received, dataLength - received, SocketFlags.None);
            }

            var word = DecodeMessage(data);
            result.objectResult = word;
            return result;
        }

    }
}
