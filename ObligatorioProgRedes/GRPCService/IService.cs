using System;
using System.Collections.Generic;
using System.Text;

namespace AdminConsumer
{
    public interface IService
    {
        bool CreateUser(string username, string password);
        bool DeleteUser(string username);
        bool ModifyUser(string username, string newCredentials);
    }
}
