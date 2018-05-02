using System.Data;
using ERP.Infrastructure.UnitOfWork;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.LogItem;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class LogItemPresenter : ILogItemPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly ILogItemRepository logItemRepository;

        #endregion

        #region Конструкторы

        public LogItemPresenter(IUnitOfWorkFactory unitOfWorkFactory, ILogItemRepository logItemRepository)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.logItemRepository = logItemRepository;
        }

        #endregion

        #region Методы

        public void Save(LogItemEditViewModel model)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var logItem = new LogItem(model.Time, model.UserId, model.Url, model.UserMessage, model.SystemMessage);
                
                logItemRepository.Save(logItem);

                uow.Commit();
            }
        }

        #endregion
    }
}
