<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Report.Report0004.Report0004SettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройки отчета Report0004
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Report0004_Settings.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Ajax.BeginForm("Report0004", "Report0004", new AjaxOptions())) %>
    <%{ %>
        <%= Html.PageTitle("Report", "Настройки отчета Report0004", "Движение товара за период")%>
        <%: Html.HiddenFor(model => model.BackURL)%>    
        <%= Html.PageBoxTop("Настраиваемые параметры отчета")%>
        <div style="background: #fff; padding: 10px 0;">
            <div class="button_set">
                <input id="btnRender2" type="button" value="Сформировать" />
                <input id="btnExportToExcel2" type="button" value="Выгрузить в Excel" />
                <input id="btnRestoreDefaults2" type="button" value="Вернуть по умолчанию" />
                <input type="button" id="btnBack2" value="Назад" />
            </div>
            <div id="messageReport0004Settings"></div>
            <div class="group_title">Настройка детализации отчета</div>
            <div class="h_delim"></div>
        
            <br />
        
            <div style="max-width: 650px; min-width: 450px; padding-left: 10px;">
                <table class="editor_table">
                    <tr>
                        <td class='row_title'><%:Html.LabelFor(x => x.ArticleName)%>:</td>
                        <td colspan="3">
                            <span id="ArticleName" class="select_link"><%: Model.ArticleName%></span>                            
                            <%: Html.HiddenFor(model => model.ArticleId)%>
                            <%: Html.ValidationMessageFor(x => x.ArticleId)%>
                        </td>     
                    </tr>
                    <tr>
                        <td class='row_title' style="min-width: 110px; width: 45%">Период:</td>
                        <td style="min-width:25%">
                            <table>
                                <tr>
                                    <td><%:Html.LabelFor(x => x.StartDate)%>:</td>
                                    <td>
                                        <%= Html.DatePickerFor(model => model.StartDate)%>
                                        <%: Html.ValidationMessageFor(model => model.StartDate)%></td>                                    
                                    <td><%:Html.LabelFor(x => x.EndDate)%>:</td>
                                    <td>
                                        <%= Html.DatePickerFor(model => model.EndDate)%>
                                        <%: Html.ValidationMessageFor(model => model.EndDate)%></td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 30%"></td>
                    </tr>
                </table>
                <br />
                <br />
                <table style="width: 100%">
                    <tr>
                        <td>
                            <div class="group_title">Вывод сводных таблиц слева</div>
                            <br />
                            <table class="editor_table">
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowStartQuantityByStorage)%>:</td>
                                    <td style="width: 40px">
                                        <%: Html.YesNoToggleFor(model => model.ShowStartQuantityByStorage)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowStartQuantityByStorage)%></td>
                                </tr>
                                <tr>
                                    <td class='row_title'>
                                        <%: Html.LabelFor(model => model.ShowStartQuantityByOrganization)%>:</td>
                                    <td>
                                        <%: Html.YesNoToggleFor(model => model.ShowStartQuantityByOrganization)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowStartQuantityByOrganization)%></td>
                                </tr>                            
                            </table>
                        </td>
                        <td>
                            <div class="group_title">Вывод сводных таблиц справа</div>
                            <br />
                            <table class="editor_table">
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowEndQuantityByStorage)%>:</td>
                                    <td style="width: 40px">
                                        <%: Html.YesNoToggleFor(model => model.ShowEndQuantityByStorage)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowEndQuantityByStorage)%></td>
                                </tr>
                                <tr>
                                    <td class='row_title'>
                                        <%: Html.LabelFor(model => model.ShowEndQuantityByOrganization)%>:</td>
                                    <td>
                                        <%: Html.YesNoToggleFor(model => model.ShowEndQuantityByOrganization)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowEndQuantityByOrganization)%></td>
                                </tr>         
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div class="group_title">Дополнительные настройки отчета</div>
                            <br />
                            <table class="editor_table">
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowBatches)%>:</td>
                                    <td>
                                        <%: Html.YesNoToggleFor(model => model.ShowBatches)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowBatches)%></td>
                                </tr>
                                <tr>
                                    <td class='row_title'>
                                        <%: Html.LabelFor(model => model.ShowPurchaseCosts)%>:</td>
                                    <td>
                                        <%: Html.YesNoToggleFor(model => model.ShowPurchaseCosts, Model.AllowToViewPurchaseCost)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowPurchaseCosts)%></td>
                                </tr>
                                <tr>
                                    <td class='row_title'>
                                        <%: Html.LabelFor(model => model.ShowOnlyExactAvailability)%>:</td>
                                    <td>
                                        <%: Html.YesNoToggleFor(model => model.ShowOnlyExactAvailability)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowOnlyExactAvailability)%></td>
                                </tr>                            
                            </table>
                        </td>
                        <td>
                            <div class="group_title"></div>
                            <br />
                            <table class="editor_table">
                                <tr>
                                    <td class='row_title' style="min-width: 200px; width: 60%">
                                        <%: Html.LabelFor(model => model.ShowRecipientAccountingPrices)%>:</td>
                                    <td>
                                        <%: Html.YesNoToggleFor(model => model.ShowRecipientAccountingPrices, Model.AllowToViewRecipientAccountingPrices)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowRecipientAccountingPrices)%></td>
                                </tr>
                                <tr>
                                    <td class='row_title'>
                                        <%: Html.LabelFor(model => model.ShowSenderAccountingPrices)%>:</td>
                                    <td>
                                        <%: Html.YesNoToggleFor(model => model.ShowSenderAccountingPrices, Model.AllowToViewSenderAccountingPrices)%>
                                        <%: Html.ValidationMessageFor(model => model.ShowSenderAccountingPrices)%></td>
                                </tr>                            
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <div class="group_title">Места хранения, по которым построить движение товара</div>
            <div class="h_delim"></div>
            <br />
            <%= Html.MultipleSelector("multipleSelectorStorages", Model.Storages, "Список доступных мест хранения", "Выбранные места хранения для отчета")%>
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

    <div id="articleSelector"></div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
