using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Сделка
    /// </summary>
    public class Deal : Entity<int>
    {
        #region Свойства

        #region Основные свойства

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        public virtual Client Client { get; protected internal set; }

        /// <summary>
        /// Договор по сделке
        /// </summary>
        public virtual ClientContract Contract
        {
            get { return contract; }
            set
            {
                ValidationUtils.Assert(value == null || Client == null || Client.Organizations.Contains(value.ContractorOrganization),
                    "Выбранная организация клиента больше не принадлежит данному клиенту. Возможно, она была удалена.");
            
                contract = value;
            }
        }
        private ClientContract contract;

        /// <summary>
        /// Ожидаемый бюджет
        /// </summary>
        public virtual decimal? ExpectedBudget { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Открыта ли сделка
        /// </summary>
        public virtual bool IsActive
        {
            get
            {
                return Stage.ContainsIn(DealStage.ClientInvestigation, DealStage.CommercialProposalPreparation, DealStage.Negotiations,
                    DealStage.ContractSigning, DealStage.ContractExecution, DealStage.ContractClosing, DealStage.DecisionMakerSearch);
            }
        }

        /// <summary>
        /// Закрыта ли сделка
        /// </summary>
        public virtual bool IsClosed { get; protected set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        /// <remarks>не более 4000 символов</remarks>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Куратор
        /// </summary>
        public virtual User Curator { get; protected set; }

        #endregion

        #region Этапы сделки

        /// <summary>
        /// Текущий этап сделки
        /// </summary>
        public virtual DealStage Stage { get; protected set; }

        /// <summary>
        /// Дата перехода на текущий этап
        /// </summary>
        public virtual DateTime StageDate { get; protected set; }

        /// <summary>
        /// Дата начала сделки (начала первого этапа)
        /// </summary>
        public virtual DateTime StartDate { get; protected set; }

        /// <summary>
        /// История этапов сделки
        /// </summary>
        public virtual IEnumerable<DealStageHistory> StageHistory
        {
            get { return new ImmutableSet<DealStageHistory>(stageHistory); }
        }
        private Iesi.Collections.Generic.ISet<DealStageHistory> stageHistory;

        /// <summary>
        /// Получить следующий этап сделки (не проверяет условия перехода, например, наличие договора)
        /// </summary>
        public virtual DealStage? NextStage
        {
            get
            {
                switch (Stage)
                {
                    // 0
                    case DealStage.DecisionMakerSearch: return null;
                    // 1
                    case DealStage.ClientInvestigation: return DealStage.CommercialProposalPreparation;
                    // 2
                    case DealStage.CommercialProposalPreparation: return DealStage.Negotiations;
                    // 3
                    case DealStage.Negotiations: return DealStage.ContractSigning;
                    // 4
                    case DealStage.ContractSigning: return DealStage.ContractExecution;
                    // 5
                    case DealStage.ContractExecution: return DealStage.ContractClosing;
                    // 6
                    case DealStage.ContractClosing: return DealStage.SuccessfullyClosed;
                    // 7.1
                    case DealStage.SuccessfullyClosed: return null;
                    // 7.2
                    case DealStage.ContractAbrogated: return null;
                    // 7.3
                    case DealStage.DealRejection: return null;

                    default:
                        throw new Exception("Неизвестный этап сделки.");
                }
            }
        }

        /// <summary>
        /// Получить предыдущий этап сделки (не проверяет условия перехода, например, наличие договора)
        /// </summary>
        public virtual DealStage? PreviousStage
        {
            get
            {
                switch (Stage)
                {
                    // 0
                    case DealStage.DecisionMakerSearch: return PreviousHistoryStage;
                    // 1
                    case DealStage.ClientInvestigation: return null;
                    // 2
                    case DealStage.CommercialProposalPreparation: return DealStage.ClientInvestigation;
                    // 3
                    case DealStage.Negotiations: return DealStage.CommercialProposalPreparation;
                    // 4
                    case DealStage.ContractSigning: return DealStage.Negotiations;
                    // 5
                    case DealStage.ContractExecution: return DealStage.ContractSigning;
                    // 6
                    case DealStage.ContractClosing: return DealStage.ContractExecution;
                    // 7.1
                    case DealStage.SuccessfullyClosed: return DealStage.ContractClosing;
                    // 7.2
                    case DealStage.ContractAbrogated: return PreviousHistoryStage;
                    // 7.3
                    case DealStage.DealRejection: return PreviousHistoryStage;

                    default:
                        throw new Exception("Неизвестный этап сделки.");
                }
            }
        }

        /// <summary>
        /// Получить этап сделки, соответствующий неуспешному закрытию (не проверяет условия перехода, например, наличие договора)
        /// </summary>
        public virtual DealStage? UnsuccessfulClosingStage
        {
            get
            {
                switch (Stage)
                {
                    // 0
                    case DealStage.DecisionMakerSearch: return null;
                    // 1
                    case DealStage.ClientInvestigation: return DealStage.DealRejection;
                    // 2
                    case DealStage.CommercialProposalPreparation: return DealStage.DealRejection;
                    // 3
                    case DealStage.Negotiations: return DealStage.DealRejection;
                    // 4
                    case DealStage.ContractSigning: return DealStage.DealRejection;
                    // 5
                    case DealStage.ContractExecution: return DealStage.ContractAbrogated;
                    // 6
                    case DealStage.ContractClosing: return DealStage.ContractAbrogated;
                    // 7.1
                    case DealStage.SuccessfullyClosed: return null;
                    // 7.2
                    case DealStage.ContractAbrogated: return null;
                    // 7.3
                    case DealStage.DealRejection: return null;

                    default:
                        throw new Exception("Неизвестный этап сделки.");
                }
            }
        }

        /// <summary>
        /// Получить из истории этапов предпоследний (тот, который был перед текущим)
        /// </summary>
        private DealStage PreviousHistoryStage
        {
            get
            {
                ValidationUtils.Assert(stageHistory.Count > 1, "Недостаточно пройденных этапов сделки.");

                return stageHistory.OrderByDescending(x => x.StartDate).ToArray()[1].DealStage;
            }
        }

        /// <summary>
        /// Продолжительность текущего этапа
        /// </summary>
        public virtual decimal CurrentStageDuration
        {
            get
            {
                return (decimal)Math.Floor((DateTime.Now - StageDate).TotalDays);
            }
        }

        #endregion

        #region Квоты по сделке

        /// <summary>
        /// Квоты по сделке
        /// </summary>
        public virtual IEnumerable<DealQuota> Quotas
        {
            get { return new ImmutableSet<DealQuota>(quotas); }
        }
        private Iesi.Collections.Generic.ISet<DealQuota> quotas;

        /// <summary>
        /// Количество квот по сделке
        /// </summary>
        public virtual int QuotaCount
        {
            get { return quotas.Count; }
        }

        #endregion

        #region Платежные документы по сделке

        /// <summary>
        /// Платежные документы по сделке
        /// </summary>
        public virtual IEnumerable<DealPaymentDocument> DealPaymentDocuments
        {
            get { return new ImmutableSet<DealPaymentDocument>(dealPaymentDocuments); }
        }
        private Iesi.Collections.Generic.ISet<DealPaymentDocument> dealPaymentDocuments;

        /// <summary>
        /// Подсчет суммы оплат
        /// </summary>
        /// <param name="dealPaymentDocuments"></param>
        /// <returns></returns>
        private decimal CalculatePaymentSum(IEnumerable<DealPaymentDocument> dealPaymentDocuments)
        {
            return dealPaymentDocuments.Sum(x =>
            {
                switch (x.Type)
                {
                    case DealPaymentDocumentType.DealPaymentFromClient:
                        return x.Sum;

                    case DealPaymentDocumentType.DealPaymentToClient:
                        return -x.Sum;

                    default:
                        return 0M;
                }
            });
        }

        /// <summary>
        /// Сумма оплат по сделке
        /// </summary>
        public virtual decimal DealPaymentSum
        {
            get
            {
                return CalculatePaymentSum(dealPaymentDocuments.Where(x => x.Is<DealPayment>()));
            }
        }

        /// <summary>
        /// Сумма оплат наличными денежными средствами по сделке
        /// </summary>
        public virtual decimal CashDealPaymentSum
        {
            get
            {
                return CalculatePaymentSum(dealPaymentDocuments.Where(x => x.Is<DealPayment>() && x.As<DealPayment>().DealPaymentForm == DealPaymentForm.Cash));
            }
        }

        /// <summary>
        /// Сумма неразнесенных оплат (корректировки не считаем). Неразнесенными могут быть только оплаты от клиента
        /// </summary>
        public virtual decimal UndistributedDealPaymentFromClientSum
        {
            get { return dealPaymentDocuments.Where(x => x.Type == DealPaymentDocumentType.DealPaymentFromClient).Sum(x => x.UndistributedSum); }
        }

        /// <summary>
        /// Сумма корректировок сальдо (без учета разнесения)
        /// </summary>
        public virtual decimal InitialBalance
        {
            get
            {
                return dealPaymentDocuments
                    .Where(x => x.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection || x.Type == DealPaymentDocumentType.DealCreditInitialBalanceCorrection)
                    .Sum(x => x.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ? x.Sum : -x.Sum);
            }   
        }

        /// <summary>
        /// Сумма неразнесенных корректировок сальдо клиента, т.е. сумма нераспределенных остатков по его кредитовых и дебетовым корректировок.
        /// (Дебеты берутся со знаком «+», кредиты - со знаком «-»)
        /// </summary>
        public virtual decimal UnpaidDebtToInitialBalance
        {
            get
            {
                return dealPaymentDocuments
                    .Where(x => x.Is<DealCreditInitialBalanceCorrection>())
                    .Sum(x => -x.UndistributedSum) +
                dealPaymentDocuments
                    .Where(x => x.Is<DealDebitInitialBalanceCorrection>())
                    .Sum(x => x.UndistributedSum);
            }
        }

        #endregion
                
        #endregion

        #region Конструкторы

        protected Deal() {}

        public Deal(string name, User curator)
        {
            CreationDate = StartDate = StageDate = DateTime.Now;
            stageHistory = new HashedSet<DealStageHistory>();
            quotas = new HashedSet<DealQuota>();
            dealPaymentDocuments = new HashedSet<DealPaymentDocument>();
            Comment = String.Empty;
            Curator = curator;
            
            Name = name;
            PerformStageChanging(DealStage.ClientInvestigation);
        }

        #endregion

        #region Методы

        #region Изменение этапа сделки

        public virtual void MoveToNextStage()
        {
            CheckPossibilityToMoveToNextStage();
            PerformStageChanging(NextStage.Value);
        }

        public virtual void MoveToPreviousStage()
        {
            CheckPossibilityToMoveToPreviousStage();
            PerformStageChanging(PreviousStage.Value);
        }

        public virtual void MoveToUnsuccessfulClosingStage()
        {
            CheckPossibilityToMoveToUnsuccessfulClosingStage();
            PerformStageChanging(UnsuccessfulClosingStage.Value);
        }

        public virtual void MoveToDecisionMakerSearchStage()
        {
            CheckPossibilityToMoveToDecisionMakerSearchStage();
            PerformStageChanging(DealStage.DecisionMakerSearch);
        }

        /// <summary>
        /// Метод вызывается при создании возврата по сделке. Если сделка была на "закрытом" этапе, она переводится на этап "Закрытие договора"
        /// </summary>
        protected internal virtual void MoveToContractClosingStage()
        {
            if (Stage.ContainsIn(DealStage.SuccessfullyClosed, DealStage.ContractAbrogated))
            {
                PerformStageChanging(DealStage.ContractClosing);
            }
        }

        /// <summary>
        /// Изменить (или установить начальный) этап сделки
        /// </summary>
        /// <param name="stage">Новый этап сделки</param>
        private void PerformStageChanging(DealStage stage)
        {
            var currentDate = DateTime.Now;

            // закрываем текущую строку истории
            var currentHist = stageHistory.Where(x => x.EndDate == null).SingleOrDefault();
            if (currentHist != null)
            {
                currentHist.EndDate = currentDate;
            }

            Stage = stage;
            StageDate = currentDate;
            IsClosed = !IsActive;

            // добавляем новую строку истории
            var hist = new DealStageHistory(stage);
            stageHistory.Add(hist);
            hist.Deal = this;
        }

        #endregion

        #region Добавление / удаление квоты

        /// <summary>
        /// Добавление квоты по сделке
        /// </summary>
        /// <param name="quota">Квота по сделке</param>
        public virtual void AddQuota(DealQuota quota)
        {
            CheckPossibilityToAddQuota();

            ValidationUtils.Assert(quota.IsActive, "Невозможно добавить недействующую квоту.");
            quotas.Add(quota);
        }

        /// <summary>
        /// Удаление квоты
        /// </summary>
        /// <param name="quota">Квота по сделке</param>
        public virtual void RemoveQuota(DealQuota quota, bool checkPossibility)
        {
            if (!quotas.Contains(quota))
            {
                throw new Exception("Данная квота не относится к этой сделке.");
            }

            if (checkPossibility)
            {
                CheckPossibilityToRemoveQuota(quota);
            }
            
            quotas.Remove(quota);
        }
        #endregion

        #region Добавление / удаление платежного документа

        public virtual void AddDealPaymentDocument(DealPaymentDocument dealPaymentDocument)
        {
            dealPaymentDocuments.Add(dealPaymentDocument);
            dealPaymentDocument.Deal = this;
        }

        public virtual void DeleteDealPaymentDocument(DealPaymentDocument dealPaymentDocument, DateTime currentDate)
        {
            dealPaymentDocuments.Remove(dealPaymentDocument);
            dealPaymentDocument.DeletionDate = currentDate;
        }

        #endregion

        #region Вспомогательные методы

        #region Проверки на возможность выполнения операций
        
        #region Переходы между этапами сделки

        public virtual void CheckPossibilityToMoveToNextStage()
        {
            ValidationUtils.NotNull(NextStage, String.Format("Этап сделки «{0}» не имеет следующего этапа.", Stage.GetDisplayName()));

            switch (Stage)
            {
                // 4
                case DealStage.ContractSigning:
                    ValidationUtils.NotNull(Contract, String.Format("Невозможно перевести сделку на этап «{0}», так как по ней отсутствует договор.", NextStage.GetDisplayName()));
                    break;
            }
        }

        public virtual void CheckPossibilityToMoveToPreviousStage()
        {
            ValidationUtils.NotNull(PreviousStage, String.Format("Этап сделки «{0}» не имеет предыдущего этапа.", Stage.GetDisplayName()));
        }

        public virtual void CheckPossibilityToMoveToUnsuccessfulClosingStage()
        {
            ValidationUtils.NotNull(UnsuccessfulClosingStage, String.Format("Невозможно перевести сделку на этап «Неуспешное закрытие» с этапа «{0}».", Stage.GetDisplayName()));
        }

        public virtual void CheckPossibilityToMoveToDecisionMakerSearchStage()
        {
            ValidationUtils.Assert(Stage.ContainsIn(DealStage.CommercialProposalPreparation, DealStage.Negotiations, DealStage.ContractSigning),
                String.Format("Невозможно перевести сделку на этап «{0}» с этапа «{1}».", DealStage.DecisionMakerSearch.GetDisplayName(), Stage.GetDisplayName()));
        }

        #endregion

        #region Прочие

        /// <summary>
        /// Проверка разрешения на добавление оплаты от клиента по сделке
        /// </summary>
        public virtual void CheckPossibilityToCreateDealPaymentFromClient()
        {
            ValidationUtils.Assert(Stage.ContainsIn(DealStage.ContractExecution, DealStage.ContractClosing),
                String.Format("Невозможно добавить оплату от клиента в сделку со статусом «{0}».", Stage.GetDisplayName()));
        }

        /// <summary>
        /// Проверка разрешения на удаление оплаты от клиента по сделке
        /// </summary>
        public virtual void CheckPossibilityToDeleteDealPaymentFromClient()
        {
            ValidationUtils.Assert(IsActive, "Невозможно удалить оплату от клиента по закрытой сделке.");
        }

        /// <summary>
        /// Проверка разрешения на добавление возврата оплаты клиенту по сделке
        /// </summary>
        public virtual void CheckPossibilityToCreateDealPaymentToClient()
        {
            ValidationUtils.Assert(Stage.ContainsIn(DealStage.ContractExecution, DealStage.ContractClosing),
                String.Format("Невозможно добавить возврат оплаты в сделку со статусом «{0}».", Stage.GetDisplayName()));
        }

        /// <summary>
        /// Проверка разрешения на удаление возврата оплаты клиенту по сделке
        /// </summary>
        public virtual void CheckPossibilityToDeleteDealPaymentToClient()
        {
            ValidationUtils.Assert(IsActive, "Невозможно удалить возврат оплаты по закрытой сделке.");
        }

        /// <summary>
        /// Проверка разрешения на добавление кредитовой корректировки сальдо по сделке
        /// </summary>
        public virtual void CheckPossibilityToCreateDealCreditInitialBalanceCorrection()
        {
            ValidationUtils.Assert(Stage.ContainsIn(DealStage.ContractExecution, DealStage.ContractClosing),
                String.Format("Невозможно добавить кредитовую корректировку сальдо в сделку со статусом «{0}».", Stage.GetDisplayName()));
        }

        /// <summary>
        /// Проверка разрешения на удаление кредитовой корректировки сальдо по сделке
        /// </summary>
        public virtual void CheckPossibilityToDeleteDealCreditInitialBalanceCorrection()
        {
            ValidationUtils.Assert(IsActive, "Невозможно удалить кредитовую корректировку сальдо по закрытой сделке.");
        }

        /// <summary>
        /// Проверка разрешения на добавление дебетовой корректировки сальдо по сделке
        /// </summary>
        public virtual void CheckPossibilityToCreateDealDebitInitialBalanceCorrection()
        {
            ValidationUtils.Assert(Stage.ContainsIn(DealStage.ContractExecution, DealStage.ContractClosing),
                String.Format("Невозможно добавить дебетовую корректировку сальдо в сделку со статусом «{0}».", Stage.GetDisplayName()));
        }

        /// <summary>
        /// Проверка разрешения на удаление дебетовой корректировки сальдо по сделке
        /// </summary>
        public virtual void CheckPossibilityToDeleteDealDebitInitialBalanceCorrection()
        {
            ValidationUtils.Assert(IsActive, "Невозможно удалить дебетовую корректировку сальдо по закрытой сделке.");
        }

        public virtual void CheckPossibilityToAddQuota()
        {
            if (!IsActive)
            {
                throw new Exception("Невозможно добавить квоту к закрытой сделке.");
            }     
        }

        public virtual void CheckPossibilityToRemoveQuota(DealQuota quota)
        {
            if (!IsActive)
            {
                throw new Exception("Невозможно удалить квоту из закрытой сделки.");
            }
        }

        public virtual void CheckPossibilityToEdit()
        {
            if (!IsActive)
            {
                throw new Exception("Невозможно редактировать закрытую сделку.");
            }
        }

        /// <summary>
        /// Проверка разрешения на добавление договора по сделке
        /// </summary>
        public virtual void CheckPossibilityToAddContract()
        {
            ValidationUtils.IsNull(Contract, "Договор по данной сделке уже создан.");
            ValidationUtils.Assert(Stage.ContainsIn(DealStage.DecisionMakerSearch, DealStage.ClientInvestigation, DealStage.CommercialProposalPreparation,
                DealStage.Negotiations, DealStage.ContractSigning), String.Format("Невозможно добавить договор в сделку со статусом «{0}».", Stage.GetDisplayName()));
        }

        /// <summary>
        /// Проверка разрешения на редактирование договора по сделке
        /// </summary>
        public virtual void CheckPossibilityToChangeContract()
        {
            ValidationUtils.NotNull(Contract, "Договор по данной сделке еще не создан.");
            ValidationUtils.Assert(
                Stage.ContainsIn(DealStage.ClientInvestigation, DealStage.CommercialProposalPreparation, DealStage.Negotiations, DealStage.ContractSigning), 
                String.Format("Невозможно отредактировать договор по сделке со статусом «{0}».", Stage.GetDisplayName()));
        }

        /// <summary>
        /// Проверка разрешения на редактирование организаций в договоре по сделке
        /// </summary>
        public virtual void CheckPossibilityToEditOrganization()
        {
            CheckPossibilityToChangeContract();
        }

        /// <summary>
        /// Проверка возможности создания накладной реализации по сделке
        /// </summary>
        public virtual void CheckPossibilityToCreateExpenditureWaybill()
        {
            ValidationUtils.Assert(Stage == DealStage.ContractExecution,
                String.Format("Невозможно создать накладную реализации товаров по сделке со статусом {0}.", Stage.GetDisplayName()));

            ValidationUtils.Assert(Quotas.Any(), "Невозможно создать накладную реализации товаров, т.к. по данной сделке отсутствуют квоты.");
            ValidationUtils.NotNull(Contract, "Невозможно создать накладную реализации товаров, т.к. по сделке отсутствует договор.");
        }

        /// <summary>
        /// Проверка возможности создания возврата по сделке
        /// </summary>
        public virtual void CheckPossibilityToCreateReturnFromClientWaybill()
        {
            ValidationUtils.Assert(Stage.ContainsIn(DealStage.ContractExecution, DealStage.ContractClosing, DealStage.SuccessfullyClosed, DealStage.ContractAbrogated),
                String.Format("Невозможно создать возврат от клиента по сделке со статусом «{0}».", Stage.GetDisplayName()));

            ValidationUtils.Assert(Quotas.Any(), "Невозможно создать возврат от клиента, т.к. по данной сделке отсутствуют квоты.");
            ValidationUtils.NotNull(Contract, "Невозможно создать возврат от клиента, т.к. по сделке отсутствует договор.");
        }

        #endregion

        #endregion
        
        #endregion

        #region Показатели по командам

        /// <summary>
        /// Сумма оплат по сделке
        /// </summary>
        public virtual decimal DealPaymentSumForTeam(Team team)
        {
            return CalculatePaymentSum(dealPaymentDocuments.Where(x => x.Is<DealPayment>() && x.Team == team));
        }

        /// <summary>
        /// Сумма корректировок сальдо (без учета разнесения)
        /// </summary>
        public virtual decimal InitialBalanceForTeam(Team team)
        {
            return dealPaymentDocuments
                .Where(x => x.Team == team && (x.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection || 
                    x.Type == DealPaymentDocumentType.DealCreditInitialBalanceCorrection))
                .Sum(x => x.Type == DealPaymentDocumentType.DealDebitInitialBalanceCorrection ? x.Sum : -x.Sum);
         
        }
        #endregion

        #endregion
    }
}
