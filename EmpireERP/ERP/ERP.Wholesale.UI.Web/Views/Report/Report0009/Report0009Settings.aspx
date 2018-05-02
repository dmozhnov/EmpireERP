<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Report.Report0009.Report0009SettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройки отчета Report0009
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Report0009_Settings.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("Report", "Настройки отчета Report0009", "Поставки")%>
    <%: Html.HiddenFor(model => model.BackURL) %>
    <%= Html.PageBoxTop("Настраиваемые параметры отчета")%>
    <div style="background: #fff; padding: 10px 0;">
        <div class="button_set">
            <input id="btnRender2" type="button" value="Сформировать" />
            <input id="btnExportToExcel2" type="button" value="Выгрузить в Excel" />
            <input id="btnRestoreDefaults2" type="button" value="Вернуть по умолчанию" />
            <input type="button" id="btnBack2" value="Назад" />
        </div>
        <div id="messageReport0009Settings">
        </div>
        <div class="group_title">
            Настройка детализации отчета</div>
        <div class="h_delim">
        </div>
        <br />
        <div style="max-width: 650px; min-width: 450px; padding-left: 10px;">
            <table class="editor_table">
                <tr>
                    <td class='row_title' style="min-width: 110px; width: 45%">
                        В каком диапазоне дат посчитать поставки?
                    </td>
                    <td style="width: 25%">
                        <table>
                            <tr>
                                <td>
                                    c:
                                </td>
                                <td>
                                    <%= Html.DatePickerFor(model => model.StartDate)%>
                                    <%: Html.ValidationMessageFor(model => model.StartDate)%>
                                </td>
                                <td>
                                    по:
                                </td>
                                <td>
                                    <%= Html.DatePickerFor(model => model.EndDate)%>
                                    <%: Html.ValidationMessageFor(model => model.EndDate)%>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 30%">
                    </td>
                </tr>
                <tr>
                    <td class='row_title'><%: Html.LabelFor(model => model.DateTypeId) %>:</td>
                    <td colspan="2">
                        <%: Html.DropDownListFor(model => model.DateTypeId, Model.DateTypeList)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.GroupByCollection) %>:
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.GroupByCollection, Model.GroupByCollection, new { style = "width:100%" })%>
                    </td>
                    <td>
                        <span class="link" id='btnAddGroupBy'>добавить в конец</span>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <table class="report_settings_table" id="tblGroupBy" style="display: none;">
                            <tbody>
                                <tr>
                                    <th colspan="2">
                                        Порядок группировки информации
                                    </th>
                                </tr>
                            </tbody>
                        </table>
                        <%: Html.HiddenFor(x=>x.GroupByCollectionIDs) %>
                        <input type="hidden" id="GroupByCollectionIds" />
                        <div id="option_storage" style="display: none"></div>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <br />
            <table style="width: 100%">
                <tr>
                    <td>
                        <div class="group_title">
                            Вывод таблиц</div>
                        <br />
                        <table class="editor_table table_show">
                            <tr>
                                <td class='row_title' style="min-width: 300px;">
                                    <%: Html.LabelFor(model => model.ShowDetailsTable)%>:
                                </td>
                                <td style="min-width: 300px">
                                    <%: Html.YesNoToggleFor(model => model.ShowDetailsTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowDetailsTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowDetailReceiptWaybillRowsWithDivergencesTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowDetailReceiptWaybillRowsWithDivergencesTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowDetailReceiptWaybillRowsWithDivergencesTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowStorageTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowStorageTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowStorageTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowOrganizationTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowOrganizationTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowOrganizationTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowArticleGroupTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowArticleGroupTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowArticleGroupTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowProviderTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowProviderTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowProviderTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowProviderOrganizationTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowProviderOrganizationTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowProviderOrganizationTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowUserTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowUserTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowUserTable)%>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="vertical-align: top">
                        <div class="group_title">
                            Дополнительные настройки</div>
                        <br />
                        <table class="editor_table">
                            <tr>
                                <td class='row_title' style="min-width: 200px;">
                                    <%: Html.LabelFor(model => model.ShowBatch)%>:
                                </td>
                                <td style="min-width: 300px">
                                    <%: Html.YesNoToggleFor(model => model.ShowBatch)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowBatch)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowCountArticleInPack)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowCountArticleInPack)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowCountArticleInPack)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowCountryOfProduction)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowCountryOfProduction)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowCountryOfProduction)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowManufacturer)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowManufacturer)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowManufacturer)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowCustomsDeclarationNumber)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowCustomsDeclarationNumber)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowCustomsDeclarationNumber)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.CalculateMarkup)%>:
                                </td>
                                <% if (Model.AllowToViewPurchaseCost){ %>
                                    <td>
                                        <%: Html.YesNoToggleFor(model => model.CalculateMarkup, Model.AllowToViewPurchaseCost)%>
                                        <%: Html.ValidationMessageFor(model => model.CalculateMarkup)%>
                                    </td>
                                <%}else{ %>
                                <td style="min-width: 300px">Нет</td>
                                <%} %>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                     <div class="group_title">
                            Вывод цен</div>
                        <br />
                        <table class="editor_table">
                            <tr>
                                <td class='row_title' style="min-width: 300px;">
                                    <%: Html.LabelFor(model => model.InPurchaseCost)%>:
                                </td>
                                <% if (Model.AllowToViewPurchaseCost){ %>
                                    <td style="min-width: 300px">
                                        <%: Html.YesNoToggleFor(model => model.InPurchaseCost)%>
                                        <%: Html.ValidationMessageFor(model => model.InPurchaseCost)%>
                                    </td>
                                <%}else{ %>
                                <td style="min-width: 300px">Нет</td>
                                <%} %>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.InRecipientWaybillAccountingPrice)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.InRecipientWaybillAccountingPrice)%>
                                    <%: Html.ValidationMessageFor(model => model.InRecipientWaybillAccountingPrice)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.InCurrentAccountingPrice)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.InCurrentAccountingPrice)%>
                                    <%: Html.ValidationMessageFor(model => model.InCurrentAccountingPrice)%>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                </tr>
            </table>            
        </div>
        <br />
        <div class="group_title">
            Места хранения, приходы на которые учитывать в отчете</div>
        <div class="h_delim">
        </div>
        <br />
        <%= Html.MultipleSelector("multipleSelectorStorages", Model.Storages, "Список доступных мест хранения", "Выбранные М.Х. для отчета")  %>
        <br />
        <div class="group_title">
            Группы товаров, которые будут участвовать в формировании отчета</div>
        <div class="h_delim">
        </div>
        <br />
        <%= Html.MultipleSelector("multipleSelectorArticleGroups", Model.ArticleGroups, "Список доступных групп товаров", "Выбранные группы товаров для отчета")  %>
        <br />
        <div class="group_title">
            Поставщики, от которых учитывать приходы в отчете</div>
        <div class="h_delim">
        </div>
        <br />
        <%= Html.MultipleSelector("multipleSelectorProviders", Model.Providers, "Список доступных поставщиков", "Выбранные поставщики для отчета")%>
        <br />
        <div class="group_title">
            Пользователи, приходы которых учитывать в отчете</div>
        <div class="h_delim">
        </div>
        <br />
        <%= Html.MultipleSelector("multipleSelectorUser", Model.Users, "Список доступных пользователей", "Выбранные пользователи для отчета")  %>
        <br />
        <br />
        <div class="button_set">
            <input id="btnRender" type="button" value="Сформировать" />
            <input id="btnExportToExcel" type="button" value="Выгрузить в Excel" />
            <input id="btnRestoreDefaults" type="button" value="Вернуть по умолчанию" />
            <input type="button" id="btnBack" value="Назад" />
        </div>
    </div>
    <%= Html.PageBoxBottom() %>
</asp:Content>

