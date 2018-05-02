<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Report.Report0010.Report0010SettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройки отчета Report0010
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Report0010_Settings.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Ajax.BeginForm("Report0010Report", "Report0010", new AjaxOptions())) %>
    <%{ %>
        <%= Html.PageTitle("Report", "Настройки отчета Report0010", "Принятые платежи")%>
        <%: Html.HiddenFor(model => model.BackURL)%>
        <%= Html.PageBoxTop("Настраиваемые параметры отчета")%>
        <div style="background: #fff; padding: 10px 0;">

            <div class="button_set">
                <input id="btnRender2" type="button" value="Сформировать" />
                <input id="btnExportToExcel2" type="button" value="Выгрузить в Excel" />
                <input id="btnRestoreDefaults2" type="button" value="Вернуть по умолчанию" />
                <input id="btnBack2" type="button" value="Назад" />
            </div>

            <div id="messageReport0010Settings"></div>

            <div class="group_title">Основные параметры</div>
            <div class="h_delim"></div>
            <br />
            
            <div style="min-width: 450px; min-height: 150px; padding-left: 10px;" >
            
            <br />
            <table>
                <tr>
                    <td>
                        <table class="editor_table" id="ReportSourceType_Period">
                            <tr>
                                <td class='row_title'>Даты оплат для отчета <%:Html.LabelFor(x => x.StartDate)%>:</td>
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
                                    <table class="report_settings_table" id="tblGroupBy" style="display: none">
                                        <tbody>
                                            <tr>
                                                <th colspan="2">Порядок группировки информации</th>
                                            </tr>
                                        </tbody>
                                    </table>
                                    <%: Html.HiddenFor(x => x.GroupByCollectionIDs)%>                                    
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            
            <br />
            <table>
                <tr style="vertical-align: top">
                    <td>
                        <div class="group_title">Печать сводных таблиц</div>
                        <div class="h_delim"></div>
                        <br />
                        <table class="editor_table" style="text-align: left;">
                            <tr>
                                <td class='row_title' style="width: 200px;">
                                    <%: Html.LabelFor(model => model.ShowAccountOrganizationSummary)%>:</td>
                                <td style="width: 25px">
                                    <%: Html.YesNoToggleFor(model => model.ShowAccountOrganizationSummary)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowAccountOrganizationSummary)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title' style="width: 200px;">
                                    <%: Html.LabelFor(model => model.ShowClientSummary)%>:</td>
                                <td style="width: 25px">
                                    <%: Html.YesNoToggleFor(model => model.ShowClientSummary)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowClientSummary)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowClientOrganizationSummary)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowClientOrganizationSummary)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowClientOrganizationSummary)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowClientContractSummary)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowClientContractSummary)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowClientContractSummary)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowTeamSummary)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowTeamSummary)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowTeamSummary)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowUserSummary)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowUserSummary)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowUserSummary)%>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 30px"></td>
                    <td>
                        <div class="group_title">Печать развернутых таблиц</div>
                        <div class="h_delim"></div>
                        <br />
                        <table class="editor_table">
                            <tr>
                                <td class='row_title' style="width: 420px;">
                                    <%: Html.LabelFor(model => model.ShowDetailsTable)%>:</td>
                                <td style="width: 25px">
                                    <%: Html.YesNoToggleFor(model => model.ShowDetailsTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowDetailsTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.SeparateByDealPaymentForm)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.SeparateByDealPaymentForm)%>
                                    <%: Html.ValidationMessageFor(model => model.SeparateByDealPaymentForm)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowDistributedAndUndistributedSums)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowDistributedAndUndistributedSums)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowDistributedAndUndistributedSums)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowDistributionDetails)%>:</td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowDistributionDetails)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowDistributionDetails)%>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            </div>

            <br /><br />
                        
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

            <div id="teamSelector">
                <div class="group_title">Команды, по которым строить отчет</div>
                <div class="h_delim"></div>
                <br />
                <%= Html.MultipleSelector("multipleSelectorTeam", Model.TeamList, "Список доступных команд", "Выбранные команды для отчета")%>
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
                <input type="button" id="btnBack" value="Назад" />
            </div>
        </div>
    <%= Html.PageBoxBottom() %>
    <% } %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
