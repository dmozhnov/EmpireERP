using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.UI.ViewModels.Bank;
using ERP.UI.ViewModels.Grid;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IBankPresenter
    {
        BankListViewModel List(UserInfo currentUser);
        GridData GetRussianBankGrid(GridState state, UserInfo currentUser);
        GridData GetForeignBankGrid(GridState state, UserInfo currentUser);
        RussianBankEditViewModel EditRussianBank(int bankId, UserInfo currentUser);        
        void DeleteRussianBank(int bankId, UserInfo currentUser);
        void SaveRussianBank(RussianBankEditViewModel model, UserInfo currentUser);
        RussianBankEditViewModel AddRussianBank(UserInfo currentUser);

        ForeignBankEditViewModel AddForeignBank(UserInfo currentUser);
        ForeignBankEditViewModel EditForeignBank(int bankId, UserInfo currentUser);
        void SaveForeignBank(ForeignBankEditViewModel model, UserInfo currentUser);        
        void DeleteForeignBank(int bankId, UserInfo currentUser);
    }
}
