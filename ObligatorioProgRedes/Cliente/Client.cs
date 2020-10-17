using System;
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

                var mensaje = userName + "%" +userPassword;

                CommunicateWithServer(99, mensaje);

                    if (IsLogged)
                    {
                        Console.WriteLine("1- Alta cliente\n");
                        Console.WriteLine("2- Cargar foto\n");
                        Console.WriteLine("3- Listado de usuarios\n");
                        Console.WriteLine("4- Listado de fotos de un usuario\n");
                        Console.WriteLine("5- Ver comentarios de una foto\n");
                        Console.WriteLine("6- Agregar comentarios a una foto\n");
                    }
                while (true)
                {
                    var dataWord = Console.ReadLine();
                    byte[] data = Encoding.UTF8.GetBytes(dataWord);

                    Header header = new Header(HeaderConstants.Request, 1, mensaje.Length);

                    var codedMessge = DataSend.GenMenssage(mensaje, header);

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

        public static void CreateStringProtocol(int command, string data)
        {

        }
        public static void CommunicateWithServer(int command, string data)
        {
            CreateStringProtocol(command,data);
            //envio comando y data al server
        }
    }
}
