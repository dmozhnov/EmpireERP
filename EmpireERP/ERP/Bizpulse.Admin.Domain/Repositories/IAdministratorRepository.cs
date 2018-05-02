using Bizpulse.Admin.Domain.Entities;
using ERP.Infrastructure.Repositories;

namespace Bizpulse.Admin.Domain.Repositories
{
    public interface IAdministratorRepository : IRepository<Administrator, short>
    {
        /// <summary>
        /// Получение администратора по логину
        /// </summary>
        /// <param name="login">Логин</param>
        Administrator GetByLogin(string login);
    }
}
