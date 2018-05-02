using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Bank;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class BankPresenter : IBankPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IUserService userService;

        private readonly IRussianBankService russianBankService;
        private readonly IForeignBankService foreignBankService;

        #endregion

        #region Конструктор

        public BankPresenter(IUnitOfWorkFactory unitOfWorkFactory, IRussianBankService russianBankService, IForeignBankService foreignBankService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.userService = userService;
            
            this.russianBankService = russianBankService;
            this.foreignBankService = foreignBankService;
        }

        #endregion

        #region Методы

        #region Список

        public BankListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = new BankListViewModel();
                
                var user = userService.CheckUserExistence(currentUser.Id);

                model.Filter = new FilterData();
                model.Filter.Items.Add(new FilterTextEditor("Name", "Название"));
                model.Filter.Items.Add(new FilterTextEditor("BIC", "БИК"));
                model.Filter.Items.Add(new FilterTextEditor("SWIFT", "SWIFT-код"));

                model.RussianBankGrid = GetRussianBankGridLocal(new GridState() { Sort = "Name=Asc" }, user);
                model.ForeignBankGrid = GetForeignBankGridLocal(new GridState() { Sort = "Name=Asc" }, user);

                return model;
            }
        }

        /// <summary>
        /// Формирование грида российских банков
        /// </summary>
        public GridData GetRussianBankGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetRussianBankGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование грида российских банков
        /// </summary>
        private GridData GetRussianBankGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                state = new GridState();
            }

            var model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(70), GridCellStyle.Action);
            model.AddColumn("RussianBankName", "Название", Unit.Percentage(100));
            model.AddColumn("BIC", "БИК", Unit.Pixel(120));
            model.AddColumn("CorAccount", "Кор. / счет", Unit.Pixel(120));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.Bank_Create);

            var action = new GridActionCell("Action");

            if (user.HasPermission(Permission.Bank_Edit))
            {
                action.AddAction("Ред.", "editRussianBank");
            }
            else
            {
                action.AddAction("Дет.", "editRussianBank");
            }

            if (user.HasPermission(Permission.Bank_Delete))
            {
                action.AddAction("Удал.", "deleteRussianBank");
            }

            ParameterString ps = new ParameterString(state.Filter);
            state.Filter = "";  // Сбрасываем фильтр, т.к. при мержинге ParameterString и этой строки возвращается удаленный параметр SWIFT

            if (!(ps.Keys.Contains("SWIFT") && !String.IsNullOrEmpty(ps["SWIFT"].Value as string)))
            {
                ps.Delete("SWIFT");
                var list = russianBankService.GetFilteredList(state, ps);

                foreach (var val in list.OrderBy(x => x.Name))
                {
                    model.AddRow(new GridRow(
                        action,
                        new GridLabelCell("RussianBankName") { Value = val.Name },
                        new GridLabelCell("BIC") { Value = val.BIC },
                        new GridLabelCell("CorAccount") { Value = val.CorAccount },
                        new GridHiddenCell("Id") { Value = val.Id.ToString() }));
                }
            }
            else
            {
                state.TotalRow = 0;
                state.CurrentPage = 1;
            }

            model.State = state;

            return model;
        }

        /// <summary>
        /// Формирвание грида иностранных банков
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public GridData GetForeignBankGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetForeignBankGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование грида иностранных банков
        /// </summary>
        private GridData GetForeignBankGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                state = new GridState();
            }

            var model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(70), GridCellStyle.Action);
            model.AddColumn("ForeignBankName", "Название", Unit.Percentage(100));
            model.AddColumn("SWIFT", "SWIFT-код", Unit.Pixel(120));
            model.AddColumn("ClearingCodeType", "Тип клирингового кода", Unit.Pixel(120));
            model.AddColumn("ClearingCode", "Клиринговый код", Unit.Pixel(120));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.Bank_Create);

            var action = new GridActionCell("Action");

            if (user.HasPermission(Permission.Bank_Edit))
            {
                action.AddAction("Ред.", "editForeignBank");
            }
            else
            {
                action.AddAction("Дет.", "editForeignBank");
            }

            if (user.HasPermission(Permission.Bank_Delete))
            {
                action.AddAction("Удал.", "deleteForeignBank");
            }

            ParameterString ps = new ParameterString(state.Filter);
            state.Filter = "";  // Сбрасываем фильтр, т.к. при мержинге ParameterString и этой строки возвращается удаленный параметр BIC

            if (!(ps.Keys.Contains("BIC") && !String.IsNullOrEmpty(ps["BIC"].Value as string)))
            {
                ps.Delete("BIC");
                var list = foreignBankService.GetFilteredList(state, ps);

                foreach (var val in list.OrderBy(x => x.Name))
                {
                    model.AddRow(new GridRow(
                        action,
                        new GridLabelCell("ForeignBankName") { Value = val.Name },
                        new GridLabelCell("SWIFT") { Value = val.SWIFT },
                        new GridLabelCell("ClearingCodeType") { Value = val.ClearingCodeType != null ? val.ClearingCodeType.Value.GetDisplayName() : "" },
                        new GridLabelCell("ClearingCode") { Value = val.ClearingCode },
                        new GridHiddenCell("Id") { Value = val.Id.ToString() }));
                }
            }
            else
            {
                state.CurrentPage = 1;
                state.TotalRow = 0;
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Редактирование

        #region Российский банк

        public RussianBankEditViewModel AddRussianBank(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Bank_Create);

                return new RussianBankEditViewModel()
                {
                    Title = "Добавление российского банка",
                    AllowToEdit = true,
                    Id = "0"
                };
            }
        }

        public RussianBankEditViewModel EditRussianBank(int bankId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var bank = russianBankService.CheckBankExistence(bankId);
                var allowToEdit = user.HasPermission(Permission.Bank_Edit);

                return new RussianBankEditViewModel()
                {
                    Name = bank.Name,
                    Address = bank.Address,
                    BIC = bank.BIC,
                    CorAccount = bank.CorAccount,
                    Title = allowToEdit ? "Редактирование российского банка" : "Детали российского банка",
                    AllowToEdit = allowToEdit
                };
            }
        }

        public void SaveRussianBank(RussianBankEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var id = ValidationUtils.TryGetInt(model.Id);
                var user = userService.CheckUserExistence(currentUser.Id);

                RussianBank bank;

                if (id == 0)
                {
                    user.CheckPermission(Permission.Bank_Create);
                    bank = new RussianBank(model.Name, model.BIC, model.CorAccount);
                }
                else
                {
                    user.CheckPermission(Permission.Bank_Edit);
                    bank = russianBankService.CheckBankExistence(id);
                    bank.Name = model.Name;
                    bank.BIC = model.BIC;
                    bank.CorAccount = model.CorAccount;
                }

                bank.Address = model.Address;

                russianBankService.Save(bank);

                uow.Commit();
            }
        }

        public void DeleteRussianBank(int bankId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Bank_Delete);

                var bank = russianBankService.CheckBankExistence(bankId);

                russianBankService.Delete(bank);

                uow.Commit();
            }
        }

        #endregion

        #region Иностранный банк

        public ForeignBankEditViewModel AddForeignBank(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Bank_Create);

                return new ForeignBankEditViewModel()
                {
                    Title = "Добавление иностранного банка",
                    ClearingCodeTypeList = ComboBoxBuilder.GetComboBoxItemList<ClearingCodeType>(true),
                    AllowToEdit = true,
                    Id = "0"
                };
            }
        }

        public ForeignBankEditViewModel EditForeignBank(int bankId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var bank = foreignBankService.CheckBankExistence(bankId);
                var allowToEdit = user.HasPermission(Permission.Bank_Edit);

                return new ForeignBankEditViewModel()
                {
                    Name = bank.Name,
                    Address = bank.Address,
                    SWIFT = bank.SWIFT,
                    ClearingCode = bank.ClearingCode,
                    ClearingCodeType = bank.ClearingCodeType != null ? bank.ClearingCodeType.Value.ValueToString() : "",
                    ClearingCodeTypeList = ComboBoxBuilder.GetComboBoxItemList<ClearingCodeType>(true),
                    Title = allowToEdit ? "Редактирование иностранного банка" : "Детали иностранного банка",
                    AllowToEdit = allowToEdit
                };
            }
        }

        public void SaveForeignBank(ForeignBankEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var id = ValidationUtils.TryGetInt(model.Id);
                var user = userService.CheckUserExistence(currentUser.Id);

                if (model.ClearingCode == null)
                {
                    model.ClearingCode = "";
                }

                ForeignBank bank;

                if (id == 0)
                {
                    user.CheckPermission(Permission.Bank_Create);
                    bank = new ForeignBank(model.Name, model.SWIFT);
                }
                else
                {
                    user.CheckPermission(Permission.Bank_Edit);
                    bank = foreignBankService.CheckBankExistence(id);

                    bank.Name = model.Name;
                    bank.SWIFT = model.SWIFT;
                }

                bank.Address = model.Address;
                bank.ClearingCode = model.ClearingCode;
                bank.ClearingCodeType = model.ClearingCodeType != null ? (ClearingCodeType?)Convert.ToByte(model.ClearingCodeType) : (ClearingCodeType?)null;

                foreignBankService.Save(bank);

                uow.Commit();
            }
        }

        public void DeleteForeignBank(int bankId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Bank_Delete);

                var bank = foreignBankService.CheckBankExistence(bankId);
                foreignBankService.Delete(bank);

                uow.Commit();
            }
        }

        #endregion

        #endregion

        #endregion
    }
}