<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Report.Report0002.Report0002SettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройки отчета Report0002
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Content/Style/Treeview.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        Report0002_Settings.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("Report", "Настройки отчета Report0002", "Реализация товаров")%>
    <%: Html.HiddenFor(model => model.BackURL) %>
    <%= Html.PageBoxTop("Настраиваемые параметры отчета")%>
    <div style="background: #fff; padding: 10px 0;">
        <div class="button_set">
            <input id="btnRender2" type="button" value="Сформировать" />
            <input id="btnExportToExcel2" type="button" value="Выгрузить в Excel" />
            <input id="btnRestoreDefaults2" type="button" value="Вернуть по умолчанию" />
            <input type="button" id="btnBack2" value="Назад" />
        </div>
        <div id="messageReport0002Settings">
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
                        В каком диапазоне дат посчитать реализации?
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
                    <td>
                    </td>
                    <td colspan="2">
                        <%: Html.RadioButtonFor(model => model.WaybillStateId, 0, new { id = "rbWaybillState_0" })%>
                        <label for="rbWaybillState_0">
                            <%: Model.WaybillState_caption0 %></label>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td colspan="2">
                        <%: Html.RadioButtonFor(model => model.WaybillStateId, 1, new { id = "rbWaybillState_1" })%>
                        <label for="rbWaybillState_1">
                            <%: Model.WaybillState_caption1 %></label>
                        <br />
                        <br />
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
                        <table class="report_settings_table" id="tblGroupBy">
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
                            Вывод сводных таблиц </div>
                        <br />
                        <table class="editor_table table_show">
                            <tr>
                                <td class='row_title' style="min-width: 220px;">
                                    <%: Html.LabelFor(model => model.ShowStorageTable)%>:
                                </td>
                                <td style="min-width: 300px">
                                    <%: Html.YesNoToggleFor(model => model.ShowStorageTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowStorageTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowAccountOrganizationTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowAccountOrganizationTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowAccountOrganizationTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowClientTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowClientTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowClientTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowClientOrganizationTable )%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowClientOrganizationTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowClientOrganizationTable)%>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <div class="group_title">
                            </div>
                        <br />
                        <table class="editor_table table_show">
                            <tr>
                                <td class='row_title' style="min-width: 200px;" >
                                    <%: Html.LabelFor(model => model.ShowArticleGroupTable)%>:
                                </td>
                                <td style="min-width: 300px">
                                    <%: Html.YesNoToggleFor(model => model.ShowArticleGroupTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowArticleGroupTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowTeamTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowTeamTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowTeamTable)%>
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
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowProviderAndProducerTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowProviderAndProducerTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowProviderAndProducerTable)%>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="group_title">
                            Дополнительные настройки отчета</div>
                        <br />
                        <table class="editor_table">
                            <tr class="table_show">
                                <td class='row_title' style="min-width: 220px; ">
                                    <%: Html.LabelFor(model => model.ShowDetailsTable)%>:
                                </td>
                                <td style="min-width: 300px">
                                    <%: Html.YesNoToggleFor(model => model.ShowDetailsTable)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowDetailsTable)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.DevideByBatch)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.DevideByBatch, Model.ShowDetailsTable == "1") %>
                                    <%: Html.ValidationMessageFor(model => model.DevideByBatch)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.CalculateMarkup)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.CalculateMarkup, Model.AllowToViewPurchaseCost)%>
                                    <%: Html.ValidationMessageFor(model => model.CalculateMarkup)%>
                                </td>
                            </tr>                            
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.StoragesInColumns)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.StoragesInColumns)%>
                                    <%: Html.ValidationMessageFor(model => model.StoragesInColumns)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title' >
                                    <%: Html.LabelFor(model => model.ShowAdditionColumns)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowAdditionColumns)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowAdditionColumns)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.WithReturnFromClient)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.WithReturnFromClient)%>
                                    <%: Html.ValidationMessageFor(model => model.WithReturnFromClient)%>
                                </td>
                            </tr>
                            <tr class="ReturnFromClientTypeSelector">
                                <td class='row_title' style="vertical-align: top">
                                    <%: Html.LabelFor(model => model.ReturnFromClientType)%>:
                                </td>
                                <td>
                                    <%: Html.RadioButtonFor(model => model.ReturnFromClientType, 0, new { id = "rbReturnFromClientType_0" })%>
                                    <label for="rbReturnFromClientType_0"><%: Model.ReturnFromClientType_caption0 %></label>
                                    <br /><br />
                                    <%: Html.RadioButtonFor(model => model.ReturnFromClientType, 1, new { id = "rbReturnFromClientType_1" })%>
                                    <label for="rbReturnFromClientType_1"><%: Model.ReturnFromClientType_caption1 %></label>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowShortDetailsTable)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowShortDetailsTable,false)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowShortDetailsTable)%>
                                </td>
                            </tr>
                             <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.ShowSoldArticleCount)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.ShowSoldArticleCount)%>
                                    <%: Html.ValidationMessageFor(model => model.ShowSoldArticleCount)%>
                                </td>
                            </tr>
                        </table>                        
                    </td>
                    <td style="vertical-align: top">
                        <div class="group_title">
                            Вывод сумм</div>
                        <br />
                        <table class="editor_table">
                            <tr>
                                <td class='row_title' style="min-width: 200px;">
                                    <%: Html.LabelFor(model => model.InPurchaseCost)%>:
                                </td>
                                <td style="min-width: 300px">
                                    <%: Html.YesNoToggleFor(model => model.InPurchaseCost, Model.AllowToViewPurchaseCost)%>
                                    <%: Html.ValidationMessageFor(model => model.InPurchaseCost)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.InAccountingPrice)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.InAccountingPrice)%>
                                    <%: Html.ValidationMessageFor(model => model.InAccountingPrice)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.InSalePrice)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.InSalePrice)%>
                                    <%: Html.ValidationMessageFor(model => model.InSalePrice)%>
                                </td>
                            </tr>
                            <tr>
                                <td class='row_title'>
                                    <%: Html.LabelFor(model => model.InAvgPrice)%>:
                                </td>
                                <td>
                                    <%: Html.YesNoToggleFor(model => model.InAvgPrice, Model.ShowDetailsTable == "1")%>
                                    <%: Html.ValidationMessageFor(model => model.InAvgPrice)%>
                                </td>
                            </tr>
                            
                        </table>
                    </td>
                </tr>
            </table>            
        </div>
        <br />
        <div class="group_title">
            Места хранения, с которых учитывать реализацию товаров в отчете</div>
        <div class="h_delim">
        </div>
        <br />
        <div id='storageSelector'>
            <% Html.RenderPartial("/Views/Report/Report0002/Report0002StorageSelector.ascx", Model.Storages); %>
        </div>
        <br />
         <div style="max-width: 650px; min-width: 450px; padding-left: 10px;">
            <table class="editor_table">
                <tr>
                    <td class='row_title' style="width: 80px;">
                        <%: Html.LabelFor(model => model.CreateByArticleGroup)%>:</td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.CreateByArticleGroup, affirmationString: " группам товаров", negationString: "товарам")%>
                        <%: Html.ValidationMessageFor(model => model.CreateByArticleGroup)%></td>
                </tr>
            </table>
        </div>       
        <% string articlesSelectorDisplay = Model.CreateByArticleGroup == "0" ? "block" : "none"; %>
        <div id="articleGroupNameSelector" style="max-width: 650px; min-width: 450px; padding-left: 10px;">
            <table class="editor_table">
                <tr>
                    <td class='row_title' style="width: 240px;"><%:Html.LabelFor(x => x.ArticleGroupName)%>:</td>
                    <td>
                        <span id="ArticleGroupName" class="select_link"><%: Model.ArticleGroupName%></span>
                    </td>     
                </tr>
            </table>
        </div>
        <br />
        <% string articleGroupsSelectorDisplay = Model.CreateByArticleGroup != "0" ? "block" : "none"; %>
        <div id="articleGroupsSelector" style="display: <%: articleGroupsSelectorDisplay %>">
            <div class="group_title">Группы товаров, которые будут участвовать в формировании отчета</div>
            <div class="h_delim"></div>
            <br />
            <%= Html.MultipleSelector("multipleSelectorArticleGroups", Model.ArticleGroups, "Список доступных групп товаров", "Выбранные группы товаров для отчета")  %>
        </div>
        <br />
        <div id="articlesSelector" style="display: <%: articlesSelectorDisplay %>">
            <div class="group_title">Товары, которые будут участвовать в формировании отчета</div>
            <div class="h_delim"></div>
            <br />
            <%= Html.MultipleSelector("multipleSelectorArticles", Model.Articles, "Список доступных товаров", "Выбранные товары для отчета")%>
        </div>
        <br />
        <div class="group_title">
            Клиенты, реализацию товаров которым учитывать в отчете</div>
        <div class="h_delim">
        </div>
        <br />
        <%= Html.MultipleSelector("multipleSelectorClient", Model.Clients, "Список доступных клиентов", "Выбранные клиенты для отчета")  %>
        <br />
        <div class="group_title">
            Пользователи, реализацию товаров которых учитывать в отчете</div>
        <div class="h_delim">
        </div>
        <br />
        <%= Html.MultipleSelector("multipleSelectorUser", Model.Users, "Список доступных пользователей", "Выбранные пользователи для отчета")  %>
        <br />
        <div class="group_title">
            Собственные организации, чьи реализации товаров учитывать в отчете</div>
        <div class="h_delim">
        </div>
        <br />
        <%= Html.MultipleSelector("multipleSelectorAccountOrganization", Model.AccountOrganizations, "Список доступных собственных организаций", "Выбранные собственные организации для отчета")  %>
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
     <div id="articleGroupSelector"></div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
