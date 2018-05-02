using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc.ExportTo1CDataModels;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Transform;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    /// <summary>
    /// Репозиторий для выгрузки накладных для 1С
    /// </summary>
    public class ExportTo1CRepository : BaseRepository, IExportTo1CRepository
    {
        /// <summary>
        /// Получение списка накладных реализации для выгрузки в 1С
        /// </summary>
        /// <param name="startDate">Дата начала периода выгрузки</param>
        /// <param name="endDate">Дата окончания периода выгрузки</param>
        /// <param name="user">Пользователь</param>
        /// <param name="idSenderAccountingOrganizationList">Код собственных организаций-отправителей через запятую</param>        
        /// <param name="allSenderAccountOrganizations">Признак : брать все собственные организации-отправители (true) 
        /// или брать организации из параметра idSenderAccountOrganizationsList</param>
        /// <param name="idRecipientAccountOrganizationsList">Код собственных организаций-получателей  через запятую</param>        
        /// <param name="allRecipientAccountOrganizations">Признак : брать все собственные организации-получатели (true) или брать организации из параметра 
        /// idRecipientAccountOrganizationsList</param>
        /// <param name="addTransfersToCommission">Признак, того показывать ли передачу  на коммисию</param>        
        public IEnumerable<ExpenditureWaybillExportTo1CDataModel> GetSalesForExportTo1C(DateTime startDate, 
            DateTime endDate, User user, string idSenderAccountOrganizationsList, bool allSenderAccountOrganizations,
            bool addTransfersToCommission, string idRecipientAccountOrganizationsList, bool allRecipientAccountOrganizations)
        {
            return CurrentSession
                .GetNamedQuery("GetExpenditureWaybillsForExportTo1C")
                .SetDateTime("StartDate", startDate)
                .SetDateTime("EndDate", endDate)
                .SetInt32("UserId", user.Id)
                .SetString("IdSenderAccountOrganizationList", idSenderAccountOrganizationsList)
                .SetBoolean("AllSenderAccountOrganizations", allSenderAccountOrganizations)
                .SetString("IdRecipientAccountOrganizationList", idRecipientAccountOrganizationsList)
                .SetBoolean("AllRecipientAccountOrganizations", allRecipientAccountOrganizations)
                .SetBoolean("ShowTransferToCommission", addTransfersToCommission)
                .SetResultTransformer(new AliasToBeanResultTransformer(typeof(ExpenditureWaybillExportTo1CDataModel)))
                .List<ExpenditureWaybillExportTo1CDataModel>();
        }

        /// <summary>
        /// Получение списка накладных перемещения для выгрузки в 1С
        /// </summary>
        /// <param name="startDate">Дата начала периода выгрузки</param>
        /// <param name="endDate">Дата окончания периода выгрузки</param>
        /// <param name="user">Пользователь</param>
        /// <param name="idSenderAccountOrganizationsList">Код собственных организаций-отправителей через запятую</param>        
        /// <param name="allSenderAccountOrganizations">Признак : брать все собственные организации-отправители (true) 
        /// или брать организации из параметра idSenderAccountOrganizationsList</param>
        public IEnumerable<MovementWaybillExportTo1CDataModel> GetMovementsForExportTo1C(DateTime startDate,
            DateTime endDate, User user, string idSenderAccountOrganizationsList, bool allSenderAccountOrganizations)
        {
            return CurrentSession
                .GetNamedQuery("GetMovementWaybillsForExportTo1C")
                .SetDateTime("StartDate", startDate)
                .SetDateTime("EndDate", endDate)
                .SetInt32("UserId", user.Id)
                .SetString("IdSenderAccountOrganizationList", idSenderAccountOrganizationsList)
                .SetBoolean("AllSenderAccountOrganizations", allSenderAccountOrganizations)
                .SetResultTransformer(new AliasToBeanResultTransformer(typeof(MovementWaybillExportTo1CDataModel)))
                .List<MovementWaybillExportTo1CDataModel>();
        }

        /// <summary>
        /// Получение списка накладных возврата для выгрузки в 1С
        /// </summary>
        /// <param name="startDate">Дата начала периода выгрузки</param>
        /// <param name="endDate">Дата окончания периода выгрузки</param>
        /// <param name="user">Пользователь</param>
        /// <param name="idSenderAccountOrganizationsList">Код собственных организаций-отправителей через запятую</param>        
        /// <param name="allSenderAccountOrganizations">Признак : брать все собственные организации-отправители (true) 
        /// или брать организации из параметра idSenderAccountOrganizationsList</param>
        /// <param name="idRecipientAccountOrganizationsList">Код собственных организаций-получателей  через запятую</param>        
        /// <param name="allRecipientAccountOrganizations">Признак : брать все собственные организации-получатели (true) или брать организации из параметра 
        /// idRecipientAccountOrganizationsList</param> 
        /// <param name="showReturnsFromCommissionaires">Признак, того показывать ли возвраты от комиссионеров</param> 
        /// <param name="idRecipientCommissionaireOrganizationList">Список выбранных кодов организаций комиссионеров по которым необходимо учитывать возвраты от клиентов </param>
        /// <param name="allRecipientCommissionaireOrganizations">Признак выбора всех организаций комиссионеров по которым необходимо учитывать возвраты от клиентов</param>
        /// <param name="showReturnsAcceptedByCommissionaires">Показывать ли возвраты принятые комиссионерами от клиентов</param>
        /// <returns></returns>
        public IEnumerable<ReturnFromClientWaybillExportTo1CDataModel> GetReturnsForExportTo1C(DateTime startDate, DateTime endDate, User user, string idSenderAccountOrganizationsList, 
            bool allSenderAccountOrganizations, bool showReturnsFromCommissionaires, string idRecipientAccountOrganizationsList, bool allRecipientAccountOrganizations,
            string idRecipientCommissionaireOrganizationList, bool allRecipientCommissionaireOrganizations, bool showReturnsAcceptedByCommissionaires)
        {
            return CurrentSession
                .GetNamedQuery("GetReturnFromClientWaybillsForExportTo1C")
                .SetDateTime("StartDate", startDate)
                .SetDateTime("EndDate", endDate)
                .SetInt32("UserId", user.Id)
                .SetString("IdSenderAccountOrganizationList", idSenderAccountOrganizationsList)
                .SetBoolean("AllSenderAccountOrganizations", allSenderAccountOrganizations)
                .SetString("IdRecipientAccountOrganizationList", idRecipientAccountOrganizationsList)
                .SetBoolean("AllRecipientAccountOrganizations", allRecipientAccountOrganizations)
                .SetBoolean("ShowReturnsFromCommissionaires", showReturnsFromCommissionaires)
                .SetString("IdRecipientCommissionaireOrganizationList", idRecipientCommissionaireOrganizationList)
                .SetBoolean("AllRecipientCommissionaireOrganizations", allRecipientCommissionaireOrganizations)
                .SetBoolean("ShowReturnsAcceptedByCommissionaires", showReturnsAcceptedByCommissionaires)
                .SetResultTransformer(new AliasToBeanResultTransformer(typeof(ReturnFromClientWaybillExportTo1CDataModel)))
                .List<ReturnFromClientWaybillExportTo1CDataModel>();
        }


        /// <summary>
        /// Получение списка накладных поступления для выгрузки в 1С
        /// </summary>
        /// <param name="startDate">Дата начала периода выгрузки</param>
        /// <param name="endDate">Дата окончания периода выгрузки</param>
        /// <param name="user">Пользователь</param>
        /// <param name="idSenderAccountOrganizationsList">Код собственных организаций-отправителей через запятую</param>        
        /// <param name="allSenderAccountOrganizations">Признак : брать все собственные организации-отправители (true) 
        /// или брать организации из параметра idSenderAccountOrganizationsList</param>
        /// <param name="idRecipientAccountOrganizationsList">Код собственных организаций-получателей  через запятую</param>        
        /// <param name="allRecipientAccountOrganizations">Признак : брать все собственные организации-получатели (true) или брать организации из параметра 
        /// idRecipientAccountOrganizationsList</param>      
        public IEnumerable<IncomingWaybillExportTo1CDataModel> GetIncomingsForExportTo1C(DateTime startDate,
            DateTime endDate, User user, string idSenderAccountOrganizationsList, bool allSenderAccountOrganizations,
            string idRecipientAccountOrganizationsList, bool allRecipientAccountOrganizations)
        {
            return CurrentSession
            .GetNamedQuery("GetIncomingWaybillsForExportTo1C")
            .SetDateTime("StartDate", startDate)
            .SetDateTime("EndDate", endDate)
            .SetInt32("UserId", user.Id)
            .SetString("IdSenderAccountOrganizationList", idSenderAccountOrganizationsList)
            .SetBoolean("AllSenderAccountOrganizations", allSenderAccountOrganizations)
            .SetString("IdRecipientAccountOrganizationList", idRecipientAccountOrganizationsList)
            .SetBoolean("AllRecipientAccountOrganizations", allRecipientAccountOrganizations)
            .SetResultTransformer(new AliasToBeanResultTransformer(typeof(IncomingWaybillExportTo1CDataModel)))
            .List<IncomingWaybillExportTo1CDataModel>();
        
        }
    }
}
