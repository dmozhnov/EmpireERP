<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Report.Report0003.Report0003SettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройки отчета Report0003
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Report0003_Settings.Init();
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("Report", "Настройки отчета Report0003", "Финансовый отчет")%>
    <%: Html.HiddenFor(model => model.BackURL) %>
    <%= Html.PageBoxTop("Настраиваемые параметры отчета")%>
    <div style="background: #fff; padding: 10px 0;">
        <div class="button_set">
            <input id="btnRender2" type="button" value="Сформировать" />
            <input id="btnExportToExcel2" type="button" value="Выгрузить в Excel" />
            <input id="btnRestoreDefaults2" type="button" value="Вернуть по умолчанию" />
            <input type="button" id="btnBack2" value="Назад" />
        </div>
        <div id="messageReport0003Settings"></div>
        <div class="group_title">Настройка детализации отчета</div>
        <div class="h_delim"></div>
        <br />
        <div style="max-width: 500px; min-width: 450px;">
            <table class="editor_table">
                <tr>
                    <td class='row_title' style="min-width: 115px; width: 25%">Период для отчета: </td>
                    <td style="width: 75%">
                        <table>
                            <tr>
                                <td>c:</td>
                                <td>
                                    <%= Html.DatePickerFor(model => model.StartDate)%>
                                    <%: Html.ValidationMessageFor(model => model.StartDate)%></td>
                                <td>по:</td>
                                <td>
                                    <%= Html.DatePickerFor(model => model.EndDate)%>
                                    <%: Html.ValidationMessageFor(model => model.EndDate)%></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table class="editor_table" style="width:1%;">
                <tr>
                    <!--<td class='row_title'>
                        <%: Html.LabelFor(model => model.DevideByAccountOrganizations)%>: </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.DevideByAccountOrganizations)%>
                        <%: Html.ValidationMessageFor(model => model.DevideByAccountOrganizations)%>
                    </td>-->
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.DevideByInnerOuterMovement)%>: </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.DevideByInnerOuterMovement)%>
                        <%: Html.ValidationMessageFor(model => model.DevideByInnerOuterMovement)%>
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <div class="group_title">Места хранения, по которым будет построен отчет</div>
        <div class="h_delim"></div>
        <br />
        <%= Html.MultipleSelector("multipleSelectorStorages", Model.Storages, "Список доступных мест хранения", "Выбранные места хранения для отчета")  %>
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
<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
