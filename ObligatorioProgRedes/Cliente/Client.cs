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
                DataTransferSuper dataTransferReciver=new StringDataTransfer();
                DataTransferSuper dataTransferSender = new StringDataTransfer();
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
                    DataTransferSuper.SendData(codedMessage, socket);
                }
                dataTransferReciver = new StringDataTransfer();
                DataTransferResult result = dataTransferReciver.RecieveData(socket);

                dynamic resultData = (string)result.objectResult;
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
                            dataTransferReciver = new ListStringDataTransfer();
                            //result = dataTransfer.RecieveData(socket);
                            //resultData = result.objectResult as List<string>;
                            //Display(resultData);
                            break;
                        case 5:
                            command = CommandConstants.ListFiles;
                            Console.WriteLine("Ingrese el usuario");
                            data = Console.ReadLine();
                            
                            //dataTransferReciver = new ListStringDataTransfer();
                            //result = dataTransferReciver.RecieveData(socket);

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
                        default:
                            Console.WriteLine("Invalid command");
                            break;
                    }
                    if (needToSend)
                    {
                        header = new Header(HeaderConstants.Request, command, data.Length);
                        codedRequest = dataTransferSender.GenMenssage(data, header);
                        DataTransferSuper.SendData(codedRequest, socket);

                        result = dataTransferReciver.RecieveData(socket);
                        resultData = result.objectResult as List<string>;
                        Display(resultData);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void SendFile(string path)
        {
            
            DataTransferSuper dataTransferSender = new StringDataTransfer();
          
            IFileSenderHandler senderHandler = new FileSenderHandler();
            IFileHandler fileHandler = new FileHandler();
            long fileSize = fileHandler.GetFileSize(path);
            string fileName = fileHandler.GetFileName(path);
            string message = fileName+ SEPARATOR + fileSize;
            Header header = new Header(HeaderConstants.Request, CommandConstants.UploadFile, message.Length);

            var codedMessage = dataTransferSender.GenMenssage(message, header);
            DataTransferSuper.SendData(codedMessage, socket);

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

                dataTransferSender = new ByteDataTransfer();
                Header header1 = new Header(HeaderConstants.Request, CommandConstants.UploadFileSignal, size);
                
                var data = dataTransferSender.GenMenssage(fileData, header1);
                Console.WriteLine("Mande paquete "+header1.GetCommand());
                DataTransferSuper.SendData(data, socket);      
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
            try
            {
                option = Int32.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Option");
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
        }
    }
}
