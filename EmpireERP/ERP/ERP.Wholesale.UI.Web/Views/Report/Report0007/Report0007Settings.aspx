<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Report.Report0007.Report0007SettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройки отчета Report0007
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Report0007_Settings.Init();
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

 <% using (Ajax.BeginForm("Report0006Report", "Report0006", new AjaxOptions())) %>
    <%{ %>
        <%= Html.PageTitle("Report", "Настройки отчета Report0007", "Отчет «Взаиморасчеты по реализациям»")%>
        <%: Html.HiddenFor(model => model.BackURL)%>
        <input type="hidden" id="groupByCollectionCount" value="<%: Model.GroupByCollection.Count() %>"/>

        <%= Html.PageBoxTop("Настраиваемые параметры отчета")%>
        <div style="background: #fff; padding: 10px 0;">
            <div class="button_set">
                <input id="btnRender2" type="button" value="Сформировать" />
                <input id="btnExportToExcel2" type="button" value="Выгрузить в Excel" />
                <input id="btnRestoreDefaults2" type="button" value="Вернуть по умолчанию" />
                <input id="btnBack2" type="button" value="Назад" />
            </div>

            <div id="messageReport0007Settings"></div>

            <div class="group_title">Дата и детализация отчета</div>
            <div class="h_delim"></div>
            <br />
            <div style="max-width: 650px; min-width: 450px; min-height: 60px; padding-left: 10px;">
                <br />
                <table>
                    <tr>
                        <td>
                            <table class="editor_table" id="ReportSourceType_Period">
                                <tr>
                                    <td class='row_title'><%: Html.LabelFor(model=>model.Date) %>:</td>
                                    <td colspan="2">
                                        <%: Html.DatePickerFor(model => model.Date,isDisabled: !Model.AllowToChangeDataTime)%>
                                        <%: Html.ValidationMessageFor(model => model.Date)%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='row_title'><%: Html.LabelFor(model=>model.ShowOnlyDelayDebt)%>:</td>
                                    <td colspan="2"> 
                                        <%: Html.YesNoToggleFor(model => model.ShowOnlyDelayDebt)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowOnlyDelayDebt)%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='row_title'>
                                        <%: Html.LabelFor(model => model.GroupByCollection) %>: </td>
                                    <td>
                                        <%: Html.DropDownListFor(model => model.GroupByCollection, Model.GroupByCollection, new { style = "width:100%;min-width:220px;" })%>
                                    </td>
                                    <td><span class="link" id='btnAddGroupBy'>добавить в конец</span></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <table class="report_settings_table" id="tblGroupBy">
                                            <tbody>
                                                <tr>
                                                    <th colspan="2">
                                                        Порядок группировки информации
                                                    </th>
                                                </tr>
                                            </tbody>
                                        </table>
                                        <%: Html.HiddenFor(x => x.GroupByCollectionIDs)%>
                                        <input id="AllowToAddGrouping" type="hidden" value="true" />
                                    </td>
                                    <td></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
 
            <div class="group_title">Печать таблиц</div>
            <div class="h_delim"></div>
            <br />

            <div style="max-width: 650px; min-width: 450px;  padding-left: 10px;">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <table class="editor_table">
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowStorageTable)%>:
                                    </td>
                                    <td style="width: 40px">
                                        <%: Html.YesNoToggleFor(model => model.ShowStorageTable)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowStorageTable)%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowAccountOrganizationTable)%>:
                                    </td>
                                    <td style="width: 40px">
                                        <%: Html.YesNoToggleFor(model => model.ShowAccountOrganizationTable)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowAccountOrganizationTable)%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowClientTable)%>:
                                    </td>
                                    <td style="width: 40px">
                                        <%: Html.YesNoToggleFor(model => model.ShowClientTable)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowClientTable)%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowClientOrganizationTable)%>:
                                    </td>
                                    <td style="width: 40px">
                                        <%: Html.YesNoToggleFor(model => model.ShowClientOrganizationTable, Model.AllowCheckClientOrganizationTable)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowClientOrganizationTable)%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                           <table class="editor_table">
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowDealTable)%>:
                                    </td>
                                    <td style="width: 40px">
                                        <%: Html.YesNoToggleFor(model => model.ShowDealTable, Model.AllowCheckDealTable)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowDealTable)%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowTeamTable)%>:
                                    </td>
                                    <td style="width: 40px">
                                        <%: Html.YesNoToggleFor(model => model.ShowTeamTable, Model.AllowCheckTeamTable)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowTeamTable)%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowUserTable)%>:
                                    </td>
                                    <td style="width: 40px">
                                        <%: Html.YesNoToggleFor(model => model.ShowUserTable)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowUserTable)%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowExpenditureWaybillTable)%>:
                                    </td>
                                    <td style="width: 40px">
                                        <%: Html.YesNoToggleFor(model => model.ShowExpenditureWaybillTable)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowExpenditureWaybillTable)%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            
            <br />
            <br />
            
            <div id="storageSelector">
                <div class="group_title">Места хранения, по которым строить отчет</div>
                <div class="h_delim"></div>
                <br />
                <%= Html.MultipleSelector("multipleSelectorStorage", Model.StorageList, "Список доступных мест хранения", "Выбранные места хранения для отчета")%>
            </div>

            <div id="clientSelector">
                <div class="group_title">Клиенты, по которым строить отчет</div>
                <div class="h_delim"></div>
                <br />
                <%= Html.MultipleSelector("multipleSelectorClient", Model.ClientList, "Список доступных клиентов", "Выбранные клиенты для отчета")%>
            </div>

            <div id="accountOrganizationSelector">
                <div class="group_title">Собственные организации, по которым строить отчет</div>
                <div class="h_delim"></div>
                <br />
                <%= Html.MultipleSelector("multipleSelectorAccountOrganization", Model.AccountOrganizationList, "Список собственных организаций", "Выбранные собственные организации для отчета")%>
            </div>

            <div id="userSelector">
                <div class="group_title">Пользователи, по которым строить отчет</div>
                <div class="h_delim"></div>
                <br />
                <%= Html.MultipleSelector("multipleSelectorUser", Model.UserList, "Список доступных пользователей", "Выбранные пользователи для отчета")%>
            </div>

            <br />
            <br />
            <div class="button_set">
                <input id="btnRender" type="button" value="Сформировать" />
                <input id="btnExportToExcel" type="button" value="Выгрузить в Excel" />
                <input id="btnRestoreDefaults" type="button" value="Вернуть по умолчанию" />
                <input id="btnBack" type="button" value="Назад" />
            </div>
        </div>
        <%= Html.PageBoxBottom() %>
    <% } %>
</asp:Content>


