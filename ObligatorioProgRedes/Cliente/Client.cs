using System;
using Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    public class Client
    {
        private static bool IsConectedToServer;


        private static readonly IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);

        private static readonly IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);

        private static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

       
        public void ConectToServer()
        {
            try
            {
                socket.Bind(clientEndPoint);

                socket.Connect(serverEndPoint);

                Console.WriteLine("Conectado al servidor\n");
                Console.WriteLine("Bienvenido, debe ingresar al sistema\n");
                Console.WriteLine("Ingrese usuario:\n");
                var userName = Console.ReadLine();
                Console.WriteLine("Ingrese Contrasenia\n");
                var userPassword = Console.ReadLine();

                var IsLogged = false;

                var credentials = userName + "%" +userPassword;
                Header header = new Header(HeaderConstants.Request, CommandConstants.Login, credentials.Length);
                var codedMessage = DataSend.GenMenssage(credentials, header);
                SendData(codedMessage);
                Console.ReadLine();

                if (IsLogged)
                    {
                        Console.WriteLine("1- Alta cliente\n");
                        Console.WriteLine("2- Cargar foto\n");
                        Console.WriteLine("3- Listado de usuarios\n");
                        Console.WriteLine("4- Listado de fotos de un usuario\n");
                        Console.WriteLine("5- Ver comentarios de una foto\n");
                        Console.WriteLine("6- Agregar comentarios a una foto\n");
                    }
                while (IsLogged)
                {
                    /*var data = Console.ReadLine();
                    IsLogged = true;
                    Header header = new Header(HeaderConstants.Request, CommandConstants.Login, data.Length);

                    var codedMessge = DataSend.GenMenssage(credentials, header);*/

                    //byte[] dataLength = BitConverter.GetBytes(data.Length);
                    //int dataRead = 0;
                    //while (dataRead < ProtocolCommon.WordLength)
                    //{
                    //    dataRead += clientSocket.Send(
                    //        dataLength, // where data is read 
                    //        dataRead, // offset on the data array where we start to read
                    //        ProtocolCommon.WordLength - dataRead, // amount of data to be sent
                    //        SocketFlags.None);
                    //}
                    //dataRead = 0;
                    //while (dataRead < data.Length)
                    //{
                    //    dataRead += clientSocket.Send(
                    //        data, // where data is read 
                    //        dataRead, // offset on the data array where we start to read
                    //        data.Length - dataRead, // amount of data to be sent
                    //        SocketFlags.None);
                    //}
                }
            }
            catch (Exception)
            {
            }
        }

        private void SendData(byte[] message)
        {
            socket.Send(message, 0,message.Length, SocketFlags.None);
        }
    }
}
