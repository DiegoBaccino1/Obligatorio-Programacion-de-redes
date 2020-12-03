using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class DataTransfer
    {
        public const string OK_MESSAGE_RESPONSE = "OK";

        public const string SEPARATOR = "%";

        public static byte[] GenMenssage(string message, Header header)
        {
            byte[] codedMessage = Encoding.UTF8.GetBytes(message);
            byte[] headerBytes = header.GenRequest();

            byte[] fullMessage = new byte[headerBytes.Length + codedMessage.Length];
            Array.Copy(headerBytes,0,fullMessage,0,headerBytes.Length);
            Array.Copy(codedMessage, 0, fullMessage, headerBytes.Length, codedMessage.Length);

            return fullMessage;
        }

        public static string DecodeMessage (byte[] codedMessage)
        {
            string decodedMessage;
            int index = HeaderConstants.GetLength();
            int count = codedMessage.Length - index;

            decodedMessage = Encoding.UTF8.GetString(codedMessage,index,count);

            return decodedMessage;
        }
        
        public static void SendData(byte[] message,Socket socket)
        {
            socket.Send(message, 0, message.Length, SocketFlags.None);
        }
        
        public static DataTransferResult RecieveData(Socket socket)
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

            var word = Encoding.UTF8.GetString(data);
            result.objectResult = word;
            return result;
        }
    }
}
