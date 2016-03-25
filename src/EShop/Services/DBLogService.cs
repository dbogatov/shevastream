using System;
using System.Threading.Tasks;
using EShop.Models.Enitites;

namespace EShop.Services
{
    public enum DBLogEntryType
    {
        OrderReceived,
        OrderChanged,
        OrderDeleted,
        UserLoggedIn,
        UserLoggedOut,
        UserPulledOrders
    }

    public interface IDBLogService
    {
        Task LogActionAsync(DBLogEntryType type, int targetId);
    }

    public class DBLogService : IDBLogService
    {
        private readonly DataContext _context;

        public DBLogService(DataContext context)
        {
            _context = context;
        }

        public Task LogActionAsync(DBLogEntryType type, int targetId)
        {
            _context.LogEntries.Add(
                new LogEntry
                {
                    Type = (int)type,
                    TargetId = targetId,
                    Timestamp = DateTime.Now.Ticks
                }
            );

            return _context.SaveChangesAsync();
        }
    }
}
