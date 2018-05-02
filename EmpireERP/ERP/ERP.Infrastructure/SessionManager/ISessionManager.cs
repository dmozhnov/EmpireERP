
namespace ERP.Infrastructure.SessionManager
{
    public interface ISessionManager
    {
        /// <summary>
        /// Создание сессии
        /// </summary>
        /// <param name="dbServer">Имя сервера БД</param>
        /// <param name="dbName">Название БД</param>
        void CreateSession(string dbServer, string dbName);

        /// <summary>
        /// Удаление сессии
        /// </summary>
        void DisposeSession();
    }
}
