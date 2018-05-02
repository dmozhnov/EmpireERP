using System;

namespace ERP.Infrastructure.UnitOfWork
{
    ///<summary>
    /// ������� ������
    ///</summary>
    public interface IUnitOfWork : IDisposable
    {
        ///<summary>
        /// ��������� ��������� � ����
        ///</summary>
        void Commit();
    }
}