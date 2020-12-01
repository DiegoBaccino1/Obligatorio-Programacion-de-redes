using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class MemoryRepository : IRepository
    {
        private static List<Log> SuccessLogs { get; set; }
        private static List<Log> WarningLogs { get; set; }
        public MemoryRepository()
        {
            SuccessLogs = new List<Log>();
            WarningLogs = new List<Log>();
        }
        public void AddLog(Log log)
        {
            if (log.Level.Equals(Log.SUCCESS_LEVEL))
                SuccessLogs.Add(log);
            else if (log.Level.Equals(Log.WARNING_LEVEL))
                WarningLogs.Add(log);
        }

        public IEnumerable<Log> GetAllLogs()
        {
            List<Log> retSuccess = SuccessLogs;
            List<Log> retWarning = WarningLogs;
            return retSuccess.Concat(retWarning);
        }

        public IEnumerable<Log> GetSuccessLogs()
        {
            return SuccessLogs;
        }

        public IEnumerable<Log> GetWarningLogs()
        {
            return WarningLogs;
        }
    }
}
