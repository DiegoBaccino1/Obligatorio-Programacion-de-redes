using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Server
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
                
            });
        }

        public override Task<Response> CreateUser(UserView user, ServerCallContext context)
        {
            string credentials = user.Credentials;
            return Task.FromResult(new Response
            {
                Ret = Server.SignUp(credentials)
            });
            
        }
      
        public override Task<Response> DeleteUser(UserName username, ServerCallContext context)
        {
            return Task.FromResult(new Response
            {
                Ret = Server.DeleteUser(username.Username)
            });
        }
        public override Task<Response> ModifyUser(UserDTO user, ServerCallContext context)
        {
            
            return Task.FromResult(new Response
            {

                Ret = Server.ModifyUser(user.Username,user.Credentials)
            });
        }
    }
}
