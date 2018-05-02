using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Infrastructure.Repositories
{
    /// <summary>
    /// ��������� �����������
    /// </summary>
    /// <typeparam name="T">��� ��������</typeparam>
    /// <typeparam name="IdT">��� �������������� ��������</typeparam>
    public interface IRepository<T, IdT>
    {
        /// <summary>
        /// ��������� �������� �� Id
        /// </summary>
        /// <param name="id">������������� ��������</param>
        /// <returns>��������</returns>
        T GetById(IdT id);

        /// <summary>
        /// ���������� ��������
        /// </summary>
        /// <param name="entity">��������</param>
        void Save(T entity);

        /// <summary>
        /// �������� ������
        /// </summary>
        /// <param name="entity">��������</param>
        void Delete(T entity);

        /// <summary>
        /// ���������� ������� �� ���������
        /// </summary>
        /// <typeparam name="T">��� ������������� ������</typeparam>
        /// <returns></returns>
        ICriteria<TResult> Query<TResult>(bool ignoreDeletedRows = true, string alias = "") where TResult : class;

        /// <summary>
        /// ���������
        /// </summary>
        ISubCriteria<TResult> SubQuery<TResult>(bool ignoreDeletedRows = true) where TResult : class;
    }
}