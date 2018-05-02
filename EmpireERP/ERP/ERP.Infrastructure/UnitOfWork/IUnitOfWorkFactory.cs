using System.Data;

namespace ERP.Infrastructure.UnitOfWork
{
    ///<summary>
    /// Фабрика UnitOfWork
    ///</summary>
    public interface IUnitOfWorkFactory
    {
        ///<summary>
        /// Создает UnitOfWork, если у UnitOfWork не будет вызван метод <see cref="IUnitOfWork.Commit"/>, то автоматически будет выполнен rollback
        ///</summary>
        ///<param name="isolationLevel"></param>
        ///<returns></returns>
        IUnitOfWork Create(IsolationLevel isolationLevel);

        ///<summary>
        /// Создает UnitOfWork, если у UnitOfWork не будет вызван метод <see cref="IUnitOfWork.Commit"/>, то автоматически будет выполнен rollback
        ///</summary>
        ///<returns></returns>
        IUnitOfWork Create();
    }
}