using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc.ExportTo1CDataModels;

namespace ERP.Wholesale.Domain.Repositories
{
    /// <summary>
    /// Интерфейс репозитория выгрузки накладных в 1С
    /// </summary>
    public interface IExportTo1CRepository
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
        IEnumerable<ExpenditureWaybillExportTo1CDataModel> GetSalesForExportTo1C(DateTime startDate,
            DateTime endDate, User user, string idSenderAccountOrganizationsList, bool allSenderAccountOrganizations,
            bool addTransfersToCommission, string idRecipientAccountOrganizationsList, bool allRecipientAccountOrganizations);

        /// <summary>
        /// Получение списка накладных перемещения для выгрузки в 1С
        /// </summary>
        /// <param name="startDate">Дата начала периода выгрузки</param>
        /// <param name="endDate">Дата окончания периода выгрузки</param>
        /// <param name="user">Пользователь</param>
        /// <param name="idSenderAccountOrganizationsList">Код собственных организаций-отправителей через запятую</param>        
        /// <param name="allSenderAccountOrganizations">Признак : брать все собственные организации-отправители (true) 
        /// или брать организации из параметра idSenderAccountOrganizationsList</param>
        IEnumerable<MovementWaybillExportTo1CDataModel> GetMovementsForExportTo1C(DateTime startDate,
            DateTime endDate, User user, string idSenderAccountOrganizationsList, bool allSenderAccountOrganizations);

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
        IEnumerable<ReturnFromClientWaybillExportTo1CDataModel> GetReturnsForExportTo1C(DateTime startDate, DateTime endDate, User user, string idSenderAccountOrganizationsList,
            bool allSenderAccountOrganizations, bool showReturnsFromCommissionaires, string idRecipientAccountOrganizationsList, bool allRecipientAccountOrganizations,
            string idRecipientCommissionaireOrganizationList, bool allRecipientCommissionaireOrganizations, bool showReturnsAcceptedByCommissionaires);

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
        IEnumerable<IncomingWaybillExportTo1CDataModel> GetIncomingsForExportTo1C(DateTime startDate,
            DateTime endDate, User user, string idSenderAccountOrganizationsList, bool allSenderAccountOrganizations,
            string idRecipientAccountOrganizationsList, bool allRecipientAccountOrganizations);
    }

}
