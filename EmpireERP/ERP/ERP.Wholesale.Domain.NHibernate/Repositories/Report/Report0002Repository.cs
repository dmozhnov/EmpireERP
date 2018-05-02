using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Misc.Report.Report0002;
using ERP.Wholesale.Domain.Repositories.Report;
using NHibernate.Transform;

namespace ERP.Wholesale.Domain.NHibernate.Repositories.Report
{
    public class Report0002Repository : BaseRepository, IReport0002Repository
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
        public IEnumerable<Report0002RowDataModel> GetData(DateTime startDate, DateTime endDate, bool onlyShippedWaybills, bool devideByBatch,
            bool getArticleAvailability, bool inAccountingPrice, bool considerReturnFromClient, bool considerReturnFromClientByDate,
            string storageIdList, bool allStorages, bool takeArticlesFromArticleGroup, string articleGroupIdList, bool allArticleGroups, string articleIdList, 
            string clientIdList,bool allClients, string userIdList, bool allUsers, string accountOrganizationIdList, bool allAccountOrganizations,
            int userId)
        {
            return CurrentSession
                .GetNamedQuery("Report0002")
                .SetDateTime("StartDate", startDate)
                .SetDateTime("EndDate", endDate)
                .SetBoolean("OnlyShippedWaybills", onlyShippedWaybills)
                .SetBoolean("DevideByBatch", devideByBatch)
                .SetBoolean("GetArticleAvailability", getArticleAvailability)
                .SetBoolean("InAccountingPrice", inAccountingPrice)
                .SetBoolean("ConsiderReturnFromClient", considerReturnFromClient)
                .SetBoolean("ConsiderReturnFromClientByDate", considerReturnFromClientByDate)
                .SetString("StorageIdList", storageIdList)
                .SetBoolean("AllStorages", allStorages)
                .SetBoolean("TakeArticlesFromArticleGroup", takeArticlesFromArticleGroup)
                .SetString("ArticleGroupIdList", articleGroupIdList)
                .SetBoolean("AllArticleGroups", allArticleGroups)
                .SetString("ArticleIdList", articleIdList)
                .SetString("ClientIdList", clientIdList)
                .SetBoolean("AllClients", allClients)
                .SetString("UserIdList", userIdList)
                .SetBoolean("AllUsers", allUsers)
                .SetString("AccountOrganizationIdList", accountOrganizationIdList)
                .SetBoolean("AllAccountOrganizations", allAccountOrganizations)
                .SetInt32("UserId", userId)
                .SetTimeout(600)
                .SetResultTransformer(new AliasToBeanResultTransformer(typeof(Report0002RowDataModel)))
                .List<Report0002RowDataModel>();
        }
    }
}
