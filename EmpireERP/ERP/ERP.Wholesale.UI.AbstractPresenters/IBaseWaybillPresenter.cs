using ERP.Infrastructure.Security;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IBaseWaybillPresenter<T> where T: BaseWaybill
    {
        /// <summary>
        /// Смена куратора
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        /// <param name="curatorId">Код нового куратора</param>
        /// <param name="currentUser">Текущий пользователь</param>
        void ChangeCurator(string waybillId, string curatorId, UserInfo currentUser);
    }
}
