<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Report.Report0006.Report0006SettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройки отчета Report0006
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Report0006_Settings.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Ajax.BeginForm("Report0006Report", "Report0006", new AjaxOptions())) %>
    <%{ %>
        <%= Html.PageTitle("Report", "Настройки отчета Report0006", "Отчет по взаиморасчетам")%>
        <%: Html.HiddenFor(model => model.BackURL)%>
        <%= Html.PageBoxTop("Настраиваемые параметры отчета")%>
        <div style="background: #fff; padding: 10px 0;">
            <div class="button_set">
                <input id="btnRender2" type="button" value="Сформировать" />
                <input id="btnExportToExcel2" type="button" value="Выгрузить в Excel" />
                <input id="btnRestoreDefaults2" type="button" value="Вернуть по умолчанию" />
                <input type="button" id="btnBack2" value="Назад" />
            </div>
            <div id="messageReport0006Settings"></div>

            <div class="group_title">Период и детализация отчета</div>
            <div class="h_delim"></div>
            <br />
            <div style="max-width: 650px; min-width: 450px; min-height: 150px; padding-left: 10px;">
            <br />
            <table>
                <tr>
                    <td>
                        <table class="editor_table" id="ReportSourceType_Period">
                            <tr>
                                <td class='row_title'>Даты формирования отчета <%:Html.LabelFor(x => x.StartDate)%>:</td>
                                <td>
                                    <%= Html.DatePickerFor(model => model.StartDate)%>
                                    <%: Html.ValidationMessageFor(model => model.StartDate)%>
                                </td>
                                <td class='row_title'><%:Html.LabelFor(x => x.EndDate)%>:</td>
                                <td>
                                    <%= Html.DatePickerFor(model => model.EndDate)%>
                                    <%: Html.ValidationMessageFor(model => model.EndDate)%>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.GroupByCollection) %>: </td>
                                <td colspan="3">
                                    <%: Html.DropDownListFor(model => model.GroupByCollection, Model.GroupByCollection, new { style = "width:100%" })%>
                                </td>
                                <td><span class="link" id='btnAddGroupBy'>добавить в конец</span></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td colspan="3">
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
            <br />

            <table style="width: 100%">
                <tr>
                    <td>
                        <div class="group_title">Печать сводных таблиц</div>
                        <br />
                        <table class="editor_table">
                            <tr>
                                <td class='row_title' style="min-width: 200px; width: 60%">
                                    <%: Html.LabelFor(model => model.ShowClientSummary)%>:</td>
                                <td style="width: 40px">
                                    <%: Html.YesNoToggleFor(model => model.ShowClientSummary)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowClientSummary)%></td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowClientOrganizationSummary)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowClientOrganizationSummary)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowClientOrganizationSummary)%></td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowClientContractSummary)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowClientContractSummary)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowClientContractSummary)%></td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <div class="group_title">Учитывать</div>
                        <br />
                        <table class="editor_table">
                            <tr>
                                <td class='row_title' style="min-width: 200px; width: 60%">
                                    <%: Html.LabelFor(model => model.IncludeExpenditureWaybillsAndReturnFromClientWaybills)%>:</td>
                                <td style="width: 40px">
                                    <%: Html.YesNoToggleFor(model => model.IncludeExpenditureWaybillsAndReturnFromClientWaybills)%>
                                    <%: Html.ValidationMessageFor(model => model.IncludeExpenditureWaybillsAndReturnFromClientWaybills)%></td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.IncludeDealPayments)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.IncludeDealPayments)%>
                                    <%: Html.ValidationMessageFor(model => model.IncludeDealPayments)%></td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.IncludeDealInitialBalanceCorrections)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.IncludeDealInitialBalanceCorrections)%>
                                    <%: Html.ValidationMessageFor(model => model.IncludeDealInitialBalanceCorrections)%></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="group_title">Печать развернутых таблиц</div>
                        <br />
                        <table class="editor_table">
                            <tr>
                                <td class='row_title' style="min-width: 200px; width: 60%">
                                    <%: Html.LabelFor(model => model.ShowBalanceDocumentSummary)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowBalanceDocumentSummary)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowBalanceDocumentSummary)%></td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowBalanceDocumentFullInfo)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowBalanceDocumentFullInfo)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowBalanceDocumentFullInfo)%></td>
                            </tr>
                        </table>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>

            </div>

            <br />
            <br />
            <div style="max-width: 650px; min-width: 450px; padding-left: 10px;">
                <table class="editor_table">
                    <tr>
                        <td class='row_title' style="width: 240px;">
                            <%: Html.LabelFor(model => model.CreateByClient)%>:</td>
                        <td>
                            <%: Html.YesNoToggleFor(model => model.CreateByClient, affirmationString: "клиентам", negationString: "организациям клиента")%>
                            <%: Html.ValidationMessageFor(model => model.CreateByClient)%></td>
                    </tr>
                </table>
            </div>

            <br />
            <br />

            <% string clientSelectorDisplay = Model.CreateByClient != "0" ? "block" : "none"; %>
            <div id="clientSelector" style="display: <%: clientSelectorDisplay %>">
                <div class="group_title">Клиенты, по которым строить отчет</div>
                <div class="h_delim"></div>
                <br />
                <%= Html.MultipleSelector("multipleSelectorClient", Model.ClientList, "Список доступных клиентов", "Выбранные клиенты для отчета")%>
            </div>

            <% string clientOrganizationSelectorDisplay = Model.CreateByClient == "0" ? "block" : "none"; %>
            <div id="clientOrganizationSelector" style="display: <%: clientOrganizationSelectorDisplay %>">
                <div class="group_title">Организации клиентов, по которым строить отчет</div>
                <div class="h_delim"></div>
                <br />
                <%= Html.MultipleSelector("multipleSelectorClientOrganization", Model.ClientOrganizationList, "Список доступных организаций клиентов",
                    "Выбранные организации клиентов для отчета")%>
            </div>

            <div id="TeamSelector">
                <div class="group_title">Команды, по которым строить отчет</div>
                <div class="h_delim"></div>
                <br />
                <%= Html.MultipleSelector("multipleSelectorTeam", Model.TeamList, "Список доступных команд", "Выбранные команды для отчета")%>
            </div>

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
    <% } %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
