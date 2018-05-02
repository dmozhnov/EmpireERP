using System;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.AccountingPriceList;
using ERP.Wholesale.UI.ViewModels.PrintingForm.AccountingPriceList;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Security;
using System.Collections.Generic;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IAccountingPriceListPresenter
    {
        ArticleAccountingPriceEditViewModel GetTipsForArticle(Guid accountingPriceListId, int articleId, UserInfo currentUser);        
        AccountingPriceListPrintingFormViewModel GetAccountingPriceListPrintingForm(Guid id, bool detailedMode, UserInfo currentUser);

        AccountingPriceListViewModel List(UserInfo currentUser);
        GridData GetNewAccountingPriceListGrid(GridState state, UserInfo currentUser);
        GridData GetAcceptedAccountingPriceListGrid(GridState state, UserInfo currentUser);
        AccountingPriceListDetailsViewModel Details(Guid id, UserInfo currentUser, string message = "", string backURL = "");
        GridData GetAccountingPriceArticlesGrid(GridState state, UserInfo currentUser);
        GridData GetAccountingPriceStoragesGrid(GridState state, UserInfo currentUser);
        ArticleAccountingPriceEditViewModel AddArticle(Guid accountingPriceListId, UserInfo currentUser);
        ArticleAccountingPriceEditViewModel EditArticle(Guid accountingPriceListId, Guid articleAccountingPriceId, UserInfo currentUser);
        object SaveArticle(ArticleAccountingPriceEditViewModel model, UserInfo currentUser);
        object DeleteArticle(Guid accountingPriceListId, Guid articleAccountingPriceId, UserInfo currentUser);
        AccountingPriceListEditViewModel Create(string additionalId, AccountingPriceListReason reasonCode, UserInfo currentUser, string backURL = "");
        AccountingPriceListEditViewModel Edit(Guid accountingPriceListId, UserInfo currentUser, string backURL = "");
        KeyValuePair<object,string> Save(AccountingPriceListEditViewModel model, UserInfo currentUser);
        ArticleAccountingPriceSetAddViewModel AddArticleAccountingPriceSet(Guid accountingPriceListId, string backURL, UserInfo currentUser);
        object SaveArticleAccountingPriceSet(ArticleAccountingPriceSetAddViewModel model, UserInfo currentUser);
        void Delete(Guid id, UserInfo currentUser);
        string Accept(Guid id, UserInfo currentUser);
        void CancelAcceptance(Guid id, UserInfo currentUser);

        AccountingPriceListAddStorageViewModel StoragesList(Guid priceListId, UserInfo currentUser);
        object StoragesList(AccountingPriceListAddStorageViewModel model, UserInfo currentUser);
        object GetListOfStorages(Guid priceListId, UserInfo currentUser);
        object StoragesAddAll(Guid priceListId, UserInfo currentUser);
        object StoragesAddTradePoint(Guid priceListId, UserInfo currentUser);

        object DeleteStorage(Guid accPriceListId, short storageId, UserInfo currentUser);

        bool IsNumberUnique(string number);
    }
}
