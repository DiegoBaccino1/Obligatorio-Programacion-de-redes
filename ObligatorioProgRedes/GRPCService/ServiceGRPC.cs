using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace AdminConsumer
{
    public class ServiceGRPC:IService
    {
        private readonly Greeter.GreeterClient clientAdmin;
        public ServiceGRPC()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencrypetedSupport", true);
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            this.clientAdmin = new Greeter.GreeterClient(channel);
        }
        public bool CreateUser(string username, string password)
        {
            var response = this.clientAdmin.CreateUser(new UserView() { Credentials = username+"%"+password });
            return response.Ret;
        }
        public bool ModifyUser(string username, string newCredentials)
        {
            var response2 = this.clientAdmin.ModifyUser(new UserDTO() { Username = username, Credentials = newCredentials });
            return response2.Ret;
        }
        public bool DeleteUser(string username)
        {
            var response3 = this.clientAdmin.DeleteUser(new UserName() { Username = username });
            return response3.Ret;
        }
    }
}
