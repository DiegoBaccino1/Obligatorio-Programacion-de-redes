using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IRepository
    {
        IEnumerable<Log> GetAllLogs();
        IEnumerable<Log> GetSuccessLogs();
        IEnumerable<Log> GetWarningLogs();
        void AddLog(Log log);
    }
}
