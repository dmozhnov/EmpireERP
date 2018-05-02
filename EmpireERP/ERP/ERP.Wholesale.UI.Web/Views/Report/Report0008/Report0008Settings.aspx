<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Report.Report0008.Report0008SettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройки отчета Report0008
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Report0008_Settings.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <%= Html.PageTitle("Report", "Настройки отчета Report0008", "Реестр накладных")%>
    <%: Html.HiddenFor(model => model.BackURL) %>
    <%= Html.PageBoxTop("Настраиваемые параметры отчета")%>

    <div style="background: #fff; padding: 10px 0;">    
        <div class="button_set">
            <input id="btnRender2" type="button" value="Сформировать" />
            <input id="btnExportToExcel2" type="button" value="Выгрузить в Excel" />
            <input id="btnRestoreDefaults2" type="button" value="Вернуть по умолчанию" />
            <input type="button" id="btnBack2" value="Назад" />
        </div>
        
        <div id="messageReport0008Settings"></div>
        <div class="group_title">Настройка детализации отчета</div>
        <div class="h_delim"></div>
        <br />
        
         <div style="min-width: 450px; max-width: 630px; padding-left: 10px;">
         <table>
             <tr>
                 <td>
                    <table class="editor_table">
                        <tr>
                            <td class='row_title'><%: Html.LabelFor(model=>model.StartDate) %>:</td>
                            <td><%= Html.DatePickerFor(model => model.StartDate)%> - <%= Html.DatePickerFor(model => model.EndDate)%></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class='row_title'><%: Html.LabelFor(model => model.WaybillTypeId) %>:</td>
                            <td><%: Html.DropDownListFor(model => model.WaybillTypeId, Model.WaybillTypeList, new { style = "min-width: 240px;" })%></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class='row_title'><%: Html.LabelFor(model => model.DateTypeId) %>:</td>
                            <td><%: Html.DropDownListFor(model => model.DateTypeId, Model.DateTypeList, new { style = "min-width: 240px;", disabled = "disabled" })%></td>
                        </tr>
                        <tr>
                            <td class='row_title'><%: Html.LabelFor(model => model.WaybillOptionId) %>:</td>
                            <td><%: Html.DropDownListFor(model => model.WaybillOptionId, Model.WaybillOptionList, new { style = "min-width: 240px;", disabled = "disabled" })%></td>
                            <td class='row_title'><div id="PriorToDateLabel" style="display: none"><%: Html.LabelFor(model => model.PriorToDate)%>:</div></td>
                            <td><%= Html.DatePickerFor(model => model.PriorToDate, new { style = "display: none" })%></td>
                        </tr>
                        <tr>
                            <td class='row_title'><%: Html.LabelFor(model => model.SortDateTypeId) %>:</td>
                            <td><%: Html.DropDownListFor(model => model.SortDateTypeId, Model.SortDateTypeList, new { style = "min-width: 240px;", disabled = "disabled" })%></td>
                        </tr>
                        <tr>
                            <td class='row_title'><%: Html.LabelFor(model => model.GroupByCollection) %>:</td>
                            <td><%: Html.DropDownListFor(model => model.GroupByCollection, Model.GroupByCollection, new { style = "min-width: 240px;", disabled = "disabled" })%></td>
                            <td colspan="2" ><span class="link" id='btnAddGroupBy' style="display: none;">добавить в конец</span></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <table class="report_settings_table" id="tblGroupBy" style="display: none">
                                    <tbody>
                                        <tr>
                                            <th colspan="2">Порядок группировки информации</th>
                                        </tr>
                                    </tbody>
                                </table>
                                <%: Html.HiddenFor(x => x.GroupByCollectionIDs)%>                                    
                            </td>
                            <td><div id="basket" style="display: none"></div></td>
                        </tr>
                        <tr>
                           <td colspan="4">
                            <table>
                                <tr>
                                    <td class='row_title'><%: Html.LabelFor(model => model.ShowAdditionInfo) %>:</td>
                                    <td><%: Html.YesNoToggleFor(model => model.ShowAdditionInfo) %></td>
                                    <td>
                                        <table id="ExcludeDivergencesSetting" style="display: none">
                                            <tr>
                                                <td class='row_title'><%: Html.LabelFor(model => model.ExcludeDivergences)%>:</td>
                                                <td><%: Html.YesNoToggleFor(model => model.ExcludeDivergences)%></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                           </td> 
                        </tr>
                    </table>
                 </td>
             </tr>
         </table>
        </div>
        
        <br />
        <div class="group_title">Места хранения, по которым учитывать накладные в отчете</div>
        <div class="h_delim"></div>
        <br />
        <%= Html.MultipleSelector("multipleSelectorStorage", Model.StorageList, "Список доступных мест хранения", "Выбранные места хранения для отчета")  %>

        <br />
        <div class="group_title">Кураторы, по которым учитывать накладные в отчете</div>
        <div class="h_delim"></div>
        <br />
        <%= Html.MultipleSelector("multipleSelectorCurator", Model.CuratorList, "Список доступных кураторов", "Выбранные кураторы для отчета")%>
        
        <div id="clientSelectorProgress" style="padding-left: 20px;"></div>
        <div id="clientSelectorWrapper" style="display: none">
            <br />
            <div class="group_title">Клиенты, по которым учитывать накладные в отчете</div>
            <div class="h_delim"></div>
            <br />
            <div id="clientSelectorContainer"></div>
        </div>

        <div id="providerSelectorProgress" style="padding-left: 20px;"></div>
        <div id="providerSelectorWrapper" style="display: none">
            <br />
            <div class="group_title">Поставщики, по которым учитывать накладные в отчете</div>
            <div class="h_delim"></div>
            <br />
            <div id="providerSelectorContainer"></div>
        </div>

        <div class="button_set">
            <input id="btnRender" type="button" value="Сформировать" />
            <input id="btnExportToExcel" type="button" value="Выгрузить в Excel" />
            <input id="btnRestoreDefaults" type="button" value="Вернуть по умолчанию" />
            <input type="button" id="btnBack" value="Назад" />
        </div>
    </div>
    <%= Html.PageBoxBottom() %>
</asp:Content>


