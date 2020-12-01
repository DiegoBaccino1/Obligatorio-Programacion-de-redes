using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace AdminConsumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencrypetedSupport", true);
            Console.WriteLine("Starting GRPC Service....");
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var clientAdmin = new Greeter.GreeterClient(channel);
            var response= clientAdmin.CreateUser(new UserView() { Credentials="pepe%120" });
            Console.WriteLine(response.Ret);
            var response2 = clientAdmin.ModifyUser(new UserDTO() { Username= "pepe", Credentials="NewName%85588633" });
            Console.WriteLine(response.Ret);
            var response3 = clientAdmin.DeleteUser(new UserName() { Username = "aspdkpasd" });
            Console.WriteLine(response3.Ret);
            var response4 = clientAdmin.DeleteUser(new UserName() { Username = "NewName" });
            Console.WriteLine(response4.Ret);
            Console.ReadLine();
        }
    }
}
