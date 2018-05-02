
namespace ERP.Infrastructure.SessionManager
{
    public interface ISingleDBSessionManager
    {
        /// <summary>
        /// Создание сессии
        /// </summary>        
        void CreateSession();

        /// <summary>
        /// Удаление сессии
        /// </summary>
        void DisposeSession();
    }
}
