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
using System.Security.Cryptography.X509Certificates;
using MyMessaging.DataTransference;

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
        public static List<Socket> _conectedClients = new List<Socket>();

        public static List<User> Users = new List<User>();
        

        public void StartServer()

        {

            IPEndPoint endPoint = new IPEndPoint(address, port);

            isServerUp = true;

            _server.Bind(endPoint);

            _server.Listen(CONNECTIONS);

            Console.WriteLine("Listening.....");


            User user = new User()
            {
                Username = "tato",
                Password = "tato",
            };

            Users.Add(user);

            while (isServerUp)
            {
                Socket _clientSocket = _server.Accept();

                _conectedClients.Add(_clientSocket);

                new Thread(() => HandleClient(_clientSocket)).Start();
            }

        }

        public void HandleClient(Socket socket)
        {
            DataTransformSuper transfer = new StringDataTransform();
            //DataTransference transfer = new DataTransference();
            Response response = new StringResponse();
            User user = new User();
            long fileSize = 0;
            string fileName = "";
            while (true)
            {

                DataTransferResult result = DataTransference.RecieveData(socket);

                int command =result.Header.GetCommand();
                int dataLength= result.Header.GetDataLength();
                string direction= result.Header.GetDirection();
                string word = "";
                
                if (command != 31)
                {
                    transfer = new StringDataTransform();
                    result.objectResult = transfer.DecodeMessage((byte[])result.objectResult);
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
                            break;
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(e.Message);
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
                        int responseDataLength = ListStringDataTransform.ListLength(usersList);
                        response.SendResponse(command, responseData, socket, responseDataLength);
                        break;

                    case CommandConstants.ListFiles:
                        User userPhoto = new User(); //TO DO ESTO HAY QUE MOVERLO DE ACA
                        userPhoto.Username = word;
                        List<string> fileList = GetUserPhotos(userPhoto);
                        response = new ListStringResponse();
                        responseData = fileList;
                        responseDataLength = ListStringDataTransform.ListLength(fileList);
                        response.SendResponse(command, responseData, socket, responseDataLength);
                        break;

                    case CommandConstants.UploadFile:
                        ReciveFileData(word,out fileName,out fileSize);
                        transfer = new ByteDataTransform();
                        Photo photo1 = new Photo();
                        photo1.Name = fileName;
                        user.AddPhoto(photo1);
                        break;

                    case CommandConstants.UploadFileSignal:

                        //transfer = new ByteDataTransform();
                        byte[] fileBytes = (byte[])result.objectResult;
                        IFileSenderHandler senderHandler = new FileSenderHandler();
                        senderHandler.Write(fileName, fileBytes);
                        //ReciveFile(fileSize, fileName, socket);
                        break;
                    case CommandConstants.AddComent:
                        try
                        {
                            AddComment(word);
                        }catch(Exception)
                        {
                        }
                        break;
                    case CommandConstants.ViewComents:
                        List<string> comments;
                        string userName, photo;
                        GetCredentials(word,out userName, out photo);
                        Photo photoForComments = GetPhoto(userName, photo);
                        comments = photoForComments.Comments;

                        response = new ListStringResponse();
                        responseData = comments;
                        responseDataLength = ListStringDataTransform.ListLength(comments);
                        response.SendResponse(command, responseData, socket, responseDataLength);
                        break;

                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
        }

        private void AddComment(string word)
        {
            lock (Users)
            {
                string userName, photo, comment;
                GetDataComment(word, out userName, out photo, out comment);
                Photo photoToComment = GetPhoto(userName, photo);
                photoToComment.Comments.Add(comment);
            }
        }

        private void GetDataComment(string word, out string userName,out string photo,out string comment)
        {
            var data = word.Split('%');
            userName = data[0];
            photo = data[1];
            comment = data[2];
        }


        private void ReciveFileData(string data,out string fileName,out long fileSize)
        {
            string fileSizeAux;
            GetCredentials(data, out fileName, out fileSizeAux);
            fileSize = long.Parse(fileSizeAux);
        }

        private void ReciveFile(long fileSize,string fileName, Socket socket)
        {
            var segments = (fileSize / FileSenderHandler.FileSegmentSize);
            segments = segments * FileSenderHandler.FileSegmentSize == fileSize ? segments : segments + 1;

            long offset = 0;
            long currentSegments = 1;

            IFileSenderHandler senderHandler = new FileSenderHandler();
            while (fileSize > offset)
            {
                ByteDataTransform transfer = new ByteDataTransform();
                DataTransferResult result = DataTransference.RecieveData(socket);
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
            bool fullRecived = true;

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
            if(fileList.Count == 0)
            {
                return null;
            }
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

        private static User GetUserByName (string userName)
        {
            lock (Users)
            {
                return Users.Where
                    (x => x.Username.Equals(userName)).FirstOrDefault();
            }
        }

        private static Photo GetPhoto (string userName, string photo)
        {
            
            lock (Users)
            {
                User user = GetUserByName(userName);
                List<Photo> photos = user.Photos;
                foreach(Photo p in photos)
                {
                    if (p.Equals(photo))
                    {
                        return p;
                    }
                }
                throw new Exception("No existe foto");
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

        //private static void Response(int command,string message,Socket socket)
        //{
        //    Header header = new Header(HeaderConstants.Response, command, message.Length);
        //    var byteMessage=DataTransfer.GenMenssage(message, header);
        //    DataTransfer.SendData(byteMessage,socket);
        //}

        //private static bool BoolResponse(int command)
        //{
        //    return command == CommandConstants.Login || command == CommandConstants.AddComent
        //|| command == CommandConstants.SignUp || command == CommandConstants.UploadFile;
        //}
    }
}
