using Bizpulse.Admin.Domain.Entities;

namespace Bizpulse.Admin.Domain.AbstractServices
{
    public interface IClientService
    {
        /// <summary>
        /// Получение клиента с проверкой существования
        /// </summary>
        /// <param name="id">Код клиента</param>
        /// <param name="message">Текст сообщения об ошибке</param>
        Client CheckClientExistence(int id, string message = "");
    }
}
