using NHibernate.Cfg;

namespace ERP.Infrastructure.NHibernate
{
    ///<summary>
    /// Инициализатор NHibernate
    ///</summary>
    public interface INHibernateInitializer
    {
        ///<summary>
        /// Строит и возвращает конфигурация NHibernate
        ///</summary>
        ///<returns>Объект конфигурации NHibernate</returns>
        Configuration GetConfiguration(string dbServer, string dbName);
    }
}
