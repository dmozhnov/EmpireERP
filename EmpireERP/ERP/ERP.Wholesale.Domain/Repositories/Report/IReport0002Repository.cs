using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Misc.Report.Report0002;

namespace ERP.Wholesale.Domain.Repositories.Report
{
    public interface IReport0002Repository
    {
        /// <summary>
        /// Получение данных отчета
        /// </summary>
        /// <param name="startDate">Дата начала диапазона</param>
        /// <param name="endDate">Дата завершения диапазона</param>
        /// <param name="onlyShippedWaybills">Признак выборки реализаций в финальной стадии (отгруженные). Если false, то берутся проведенные</param>
        /// <param name="devideByBatch">Признак разделения партий</param>
        /// <param name="getArticleAvailability">Признак необходимости подсчитать наличие товаров (остатки + УЦ)</param>
        /// <param name="inAccountingPrice">Признак вывода УЦ</param>
        /// <param name="considerReturnFromClient">Признак, нужно ли учитывать возвраты</param>
        /// <param name="considerReturnFromClientByDate">Признак того, что нужно учитывать возвраты из указанного интервала дат. Иначе из указанных возвратов.</param>
        /// <param name="storageIdList">Список кодов МХ</param>
        /// <param name="allStorages">Признак выбора всех МХ</param>
        /// <param name="takeArticlesFromArticleGroup">Признак того, откуда брать коды товаров (если true ,то из параметров articleGroupIdList или allArticleGroups, 
        /// если false , то из параметра articleIdList </param>
        /// <param name="articleGroupIdList">Список кодов групп товаров</param>
        /// <param name="allArticleGroups">Признак выбора всех групп товаров</param>
        /// <param name="articleIdList">Список кодов товаров</param>
        /// <param name="clientIdList">Список кодов клиентов</param>
        /// <param name="allClients">Признак выбора всех клиентов</param>
        /// <param name="userIdList">Список кодов групп пользователей</param>
        /// <param name="allUsers">Признак выбора всех пользователей</param>
        /// <param name="accountOrganizationIdList">Список кодов организаций аккаунта</param>
        /// <param name="allAccountOrganizations">Признак выбора всех организаций аккаунта</param>
        /// <param name="userId">Код пользователя, запросившего отчет</param>
        /// <returns>Строки плоской таблицы с данными</returns>
        IEnumerable<Report0002RowDataModel> GetData(DateTime startDate, DateTime endDate, bool onlyShippedWaybills, bool devideByBatch,
            bool getArticleAvailability, bool inAccountingPrice, bool considerReturnFromClient, bool considerReturnFromClientByDate,
            string storageIdList, bool allStorages, bool takeArticlesFromArticleGroup, string articleGroupIdList, bool allArticleGroups, 
            string articleIdList,string clientIdList,bool allClients, string userIdList, bool allUsers, string accountOrganizationIdList, 
            bool allAccountOrganizations, int userId);
    }
}
