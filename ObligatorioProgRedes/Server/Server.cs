using Common;
using Domain;
using MyMessaging.Responses;
using Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MyMessaging;
using MyMessaging.DataTransfer;
using Common.Interfaces;
using System.IO;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;
using LogServerImp;
using LogServerImp.LogServerRabbitMQ;
using Entities;

namespace Server
{
    public class Server
    {
        private static Socket _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static IPAddress address = IPAddress.Parse("127.0.0.1");

        private static int port = 6000;

        private const int BUFFER_SIZE = 1024;

        private const int CONNECTIONS = 10;

        private static bool isServerUp = false;

        private static List<User> Users = new List<User>();
        

        public void StartServer()

        {

            IPEndPoint endPoint = new IPEndPoint(address, port);

            isServerUp = true;

            _server.Bind(endPoint);

            _server.Listen(CONNECTIONS);

            Console.WriteLine("Listening.....");

            while (isServerUp)
            {
                Socket _clientSocket = _server.Accept();

                new Thread(() => HandleClient(_clientSocket)).Start();
            }
        }

        public void HandleClient(Socket socket)
        {

            DataTransferSuper transfer = new StringDataTransfer();
            Response response = new StringResponse();
            User user;
            string fileName = "";
                while (true)
                {
                    DataTransferResult result = transfer.RecieveData(socket);

                    int command = result.Header.GetCommand();
                    int dataLength = result.Header.GetDataLength();
                    string direction = result.Header.GetDirection();
                    string word = "";

                    if (command != 31)
                    {
                        word = (string)result.objectResult;
                    };
                    dynamic responseData;
                    switch (command)
                    {
                        case CommandConstants.Login:
                            try
                            {
                                user = Login(word);
                                response = new StringResponse();
                                responseData = "true";
                                response.SendResponse(command, responseData, socket, responseData.Length);
                                SendLog("Ok");
                                break;
                            }
                            catch (Exception)
                            {
                                response = new StringResponse();
                                responseData = "false";
                                response.SendResponse(command, responseData, socket, responseData.Length);
                                break;
                            }
                        case CommandConstants.SignUp:
                            try
                            {
                                SignUp(word);
                                //response = new StringResponse();
                                //responseData = "true";
                                //response.SendResponse(command, responseData, socket, responseData.Length);
                                break;
                            }
                            catch (UserAlreadyExistException)
                            {
                                break;
                            }
                        case CommandConstants.ListUsers:
                            List<string> usersList = GetUsers();
                            response = new ListStringResponse();
                            responseData = usersList;
                            int responseDataLength = ListStringDataTransfer.ListLength(usersList);
                            response.SendResponse(command, responseData, socket, responseDataLength);
                            break;
                        case CommandConstants.ListFiles:
                            User userPhoto = new User();
                            userPhoto.Username = word;
                            List<string> fileList = GetUserPhotos(userPhoto);
                            response = new ListStringResponse();
                            responseData = fileList;
                            responseDataLength = ListStringDataTransfer.ListLength(fileList);
                            response.SendResponse(command, responseData, socket, responseDataLength);
                            break;
                        case CommandConstants.UploadFile:
                            long fileSize;
                            ReciveFileData(word, out fileName, out fileSize);
                            transfer = new ByteDataTransfer();
                            break;
                        case CommandConstants.UploadFileSignal:
                            byte[] fileBytes = (byte[])result.objectResult;
                            IFileSenderHandler senderHandler = new FileSenderHandler();
                            senderHandler.Write(fileName, fileBytes);
                            Console.WriteLine(Directory.GetCurrentDirectory());
                            break;
                        default:
                            Console.WriteLine("Invalid command");
                            break;
                    }
                }
            }

        private static void SendLog(string message)
        {
            ILogServer logServer = new LogServerRabbitMQ();
            Log log = new Log() { Message=message};
            logServer.PublishLog(log);
        }

        private void ReciveFileData(string data,out string fileName,out long fileSize)
        {
            string fileSizeAux;
            GetCredentials(data, out fileName, out fileSizeAux);
            fileSize = long.Parse(fileSizeAux);
        }

        private void ReciveFile(long fileSize,string fileName, Socket socket,ref bool fullRecived)
        {
            var segments = (fileSize / FileSenderHandler.FileSegmentSize);
            segments = segments * FileSenderHandler.FileSegmentSize == fileSize ? segments : segments + 1;

            long offset = 0;
            long currentSegments = 1;

            IFileSenderHandler senderHandler = new FileSenderHandler();
            while (fileSize > offset)
            {
                ByteDataTransfer transfer = new ByteDataTransfer();
                DataTransferResult result = transfer.RecieveData(socket);
                if (result.Header.GetCommand() == 31)
                {
                    byte[] fileDataRecived = (byte[])result.objectResult;
                    int size = 0;
                    if (currentSegments == segments)
                    {
                        size = (int)(fileSize - offset);
                    }
                    else
                    {
                        size = (int)FileSenderHandler.FileSegmentSize;
                    }

                    senderHandler.Write(fileName, fileDataRecived);
                    offset += size;
                    currentSegments++;
                }
            }
           Console.WriteLine(Directory.GetCurrentDirectory());

            //  /home/ltato                         /    miFoto.png
            fullRecived = true;
        }

        private void GetFileData(string fileData, out string fileName, out string fileSize)
        {
            throw new NotImplementedException();
        }

        private List<string> GetUserPhotos(User userPhoto)
        {
            List<Photo> fileList = new List<Photo>();
            lock (Users)
            {
                foreach(User user in Users)
                {
                    if (userPhoto.Equals(user))
                    {
                        fileList = user.Photos;
                    }
                }
            }
            List<string> result = new List<string>();
            foreach(Photo photo in fileList)
            {
                result.Add(photo.ToString());
            }
            return result;
        }

        private List<string> GetUsers()
        {
            List<string> users = new List<string>();
            lock (Users)
            {
                foreach(User user in Users) 
                {
                        users.Add(user.Username);
                }
            }
            return users;
        }

        private static void GetCredentials(string word, out string username, out string password)
        {
            var credentials = word.Split('%');
            username = credentials[0];
            password = credentials[1];
        }

        private static User Login(string credentials)
        {
            string username, password;
            GetCredentials(credentials, out username, out password);
            User user = GetUserByCredentials(username, password);
            lock (Users)
            {
                if (Users.Contains(user))
                {
                    if (!user.IsLogged)
                    {
                        user.IsLogged = true;
                        return user;
                    }
                    else
                        throw new IsLoggedException();
                }
                else
                    throw new UserNotExistException();
            }
        }

        private static User GetUserByCredentials(string username, string password)
        {
            lock (Users)
            {
                return Users.Where
                    (x => x.Username.Equals(username) && x.Password.Equals(password)).FirstOrDefault();
            }
        }
        
        private static User CreateUser(string username, string password)
        {
            User user = new User();
            user.Username = username;
            user.Password = password;
            user.Photos = new List<Photo>();
            return user;
        }

        private static void SignUp(string credentials)
        {
            string username, password;
            GetCredentials(credentials, out username, out password);

            User user = CreateUser(username, password);
            lock (Users)
            {
                if (!Users.Contains(user))
                {
                    Users.Add(user);
                }
                else
                    throw new UserAlreadyExistException();
            }
        }
    }
}
