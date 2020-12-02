using System;
using Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MyMessaging;
using MyMessaging.DataTransfer;
using System.Runtime.InteropServices;
using System.IO;
using Common.Interfaces;
using MyMessaging.DataTransference;

namespace Cliente
{
    public class Client
    {
        private const string SEPARATOR = "%";//psar a common


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
                Console.WriteLine("Bienvenido.....\n");
                int option = -1;
                bool exit = false;
                string credentials ="";
                string username = "";
                string userPassword = "";
                Header header;
                DataTransformSuper dataTransferReciver = new StringDataTransform();
                DataTransformSuper dataTransferSender = new StringDataTransform();
                while (!exit)
                {
                    DisplayStarMenu();

                    option = GetOption(option);
                    Console.WriteLine("Ingrese usuario:\n");
                    username = Console.ReadLine();
                    Console.WriteLine("Ingrese Contrasenia\n");
                    userPassword = Console.ReadLine();
                    credentials = username + SEPARATOR + userPassword;

                    switch (option)
                    {
                        case 1:
                            option = CommandConstants.SignUp;
                            break;
                        case 2:
                            option = CommandConstants.Login;
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Opcion Invalida");
                            break;
                    }
                    header = new Header(HeaderConstants.Request, option, credentials.Length);
                    var codedMessage = dataTransferSender.GenMenssage(credentials, header);
                    DataTransference.SendData(codedMessage, socket);
                }
                //dataTransferReciver = new StringDataTransfer();
                DataTransferResult result = DataTransference.RecieveData(socket);
                result.objectResult = dataTransferReciver.DecodeMessage((byte[])result.objectResult);
                
                dynamic resultData = result.objectResult;
                bool isLogged =  Boolean.Parse(resultData);
                

                while (isLogged)
                {
                    if (isLogged)
                    {
                        DisplayMenu();
                    }
                    bool needToSend = false;
                    option = -1;
                    int command = -1;
                    string data = "";
                    byte[] codedRequest = new byte[1];
                    option = GetOption(option);
                    switch (option)
                    {
                        case CommandConstants.UploadFile:
                            command = CommandConstants.UploadFile;
                            Console.WriteLine("Ingrese el path del archivo");
                            var path = Console.ReadLine();
                            SendFile(path);
                            break;
                        case CommandConstants.ListUsers:
                            command = CommandConstants.ListUsers;
                            dataTransferReciver = new ListStringDataTransform();
                            //result = DataTransference.RecieveData(socket);
                            needToSend = true;
                            //resultData = result.objectResult as List<string>;
                            //Display(resultData);
                            break;
                        case CommandConstants.ListFiles:
                            command = CommandConstants.ListFiles;
                            Console.WriteLine("Ingrese el usuario");
                            data = Console.ReadLine();
                            dataTransferReciver = new ListStringDataTransform();
                            //RequestUserPhotos(userName);
                            //Display(ListFiles(userName, socket));
                            needToSend = true;

                            break;
                        case 6:
                            command = CommandConstants.ViewComents;
                            Console.WriteLine("Seleccione foto");
                            string photo = Console.ReadLine();
                            data = username + SEPARATOR + photo;
                            break;
                        case 7:
                            command = CommandConstants.AddComent;
                            Console.WriteLine("Seleccione  usuario");
                            string userComment = Console.ReadLine();
                            Console.WriteLine("Seleccione foto");
                            photo = Console.ReadLine();
                            Console.WriteLine("Ingrese comentario");
                            string comment = Console.ReadLine();
                            data = userComment + SEPARATOR + photo + SEPARATOR + comment;
                            break;
                        case CommandConstants.Disconnect:
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                            Console.WriteLine("Shuting down connection");
                            isLogged = false;
                            break;

                        default:
                            Console.WriteLine("Invalid command");
                            break;
                    }
                    if (needToSend)
                    {
                        header = new Header(HeaderConstants.Request, command, data.Length);
                        codedRequest = dataTransferSender.GenMenssage(data, header);
                        DataTransference.SendData(codedRequest, socket);

                        result = DataTransference.RecieveData(socket);
                        result.objectResult = dataTransferReciver.DecodeMessage((byte[])result.objectResult);


                        resultData = result.objectResult as List<string>;
                        Display(resultData);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Ocurrio algun error con el servidor");
            }
        }
        public void RequestUserPhotos(string message)
        {
            DataTransformSuper dataTransferSender = new StringDataTransform();

            Header header = new Header(HeaderConstants.Request, CommandConstants.ListFiles, message.Length);

            var codedMesagge = dataTransferSender.GenMenssage(message, header);

            DataTransference.SendData(codedMesagge, socket);
        }
        private List<string> ListFiles(string message, Socket socket)
        {
            DataTransformSuper dataTransferReciver = new StringDataTransform();
            DataTransferResult result = new DataTransferResult();
            result = DataTransference.RecieveData(socket);
            
            result.objectResult = dataTransferReciver.DecodeMessage((byte[])result.objectResult);

            List<string> resultData = result.objectResult as List<string>;

            return resultData;

        }

        private void SendFile(string path)
        {
            
            DataTransformSuper dataTransferSender = new StringDataTransform();
          
            IFileSenderHandler senderHandler = new FileSenderHandler();
            IFileHandler fileHandler = new FileHandler();
            long fileSize = fileHandler.GetFileSize(path);
            string fileName = fileHandler.GetFileName(path);
            string message = fileName+ SEPARATOR + fileSize;
            Header header = new Header(HeaderConstants.Request, CommandConstants.UploadFile, message.Length);

            var codedMessage = dataTransferSender.GenMenssage(message, header);
            DataTransference.SendData(codedMessage, socket);

            long segments = (fileSize / FileSenderHandler.FileSegmentSize);
            segments = segments * FileSenderHandler.FileSegmentSize == fileSize ? segments : segments + 1;

            long offset = 0;
            long currentSegments= 1;

            while(fileSize > offset)
            {
                byte[] fileData;
                int size=0;
                if(currentSegments == segments)
                {
                    size = (int)(fileSize - offset);
                    fileData = senderHandler.Read(path, offset, size);
                    offset += size;
                    currentSegments++;
                }
                else
                {
                    size = (int)FileSenderHandler.FileSegmentSize;
                    fileData = senderHandler.Read(path, offset, size);
                    offset += size;
                    currentSegments++;
                }

                dataTransferSender = new ByteDataTransform();
                Header header1 = new Header(HeaderConstants.Request, CommandConstants.UploadFileSignal, size);
                
                var data = dataTransferSender.GenMenssage(fileData, header1);
                Console.WriteLine("Mande paquete " + currentSegments);
                DataTransference.SendData(data, socket);      
            }
        }

        private static void DisplayStarMenu()
        {
            Console.WriteLine(CommandConstants.SignUp + "-Alta Usuario");
            Console.WriteLine(CommandConstants.Login + "-Log in");
        }

        private static void Display(List<string> list)
        {
            int i = 0;
            Console.WriteLine("Usuarios conectados:\n");
            foreach (string username in list)
            {
                Console.WriteLine(++i + " - " + username);
            }
        }

        private static int GetOption(int option)
        {
            bool exit = false;
            while (!exit)
            {
                try
                {
                    option = Int32.Parse(Console.ReadLine());
                    exit = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid Option");
                }
            }
            return option;
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("\n"+CommandConstants.UploadFile+"- Cargar foto\n");
            Console.WriteLine(CommandConstants.ListUsers+"- Listado de usuarios\n");
            Console.WriteLine(CommandConstants.ListFiles+"- Listado de fotos de un usuario\n");
            Console.WriteLine(CommandConstants.ViewComents+"- Ver comentarios de una foto\n");
            Console.WriteLine(CommandConstants.AddComent+"- Agregar comentarios a una foto\n");
            Console.WriteLine(CommandConstants.Disconnect + "- Desconectar\n");
        }
    }
}
