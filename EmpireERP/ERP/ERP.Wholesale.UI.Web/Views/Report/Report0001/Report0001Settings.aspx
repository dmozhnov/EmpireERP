<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Report.Report0001.Report0001SettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройки отчета Report0001
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Content/Style/Treeview.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        Report0001_Settings.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.BackURL) %>
    
    <%= Html.PageTitle("Report", "Настройки отчета Report0001", "Наличие товаров на местах хранения")%>    
    <%= Html.PageBoxTop("Настраиваемые параметры отчета")%>

    <div style="background: #fff; padding: 10px 0;">
        <div class="button_set">
            <input id="btnRender2" type="button" value="Сформировать" />
            <input id="btnExportToExcel2" type="button" value="Выгрузить в Excel" />
            <input id="btnRestoreDefaults2" type="button" value="Вернуть по умолчанию" />
            <input id="btnBack2" type="button" value="Назад" />
        </div>
        <div id="messageReport0001Settings"></div>
        
        <div class="group_title">Настройка детализации отчета</div>
        <div class="h_delim"></div>
        <br />

        <div style="max-width: 500px; min-width: 450px;">
            <table class="editor_table">
                <tr>
                    <td class='row_title' style="min-width: 110px; width: 20%">
                        <%: Html.LabelFor(model => model.Date)%>: </td>
                    <td style="width: 40%" colspan="3">
                        <%= Html.DatePickerFor(model => model.Date)%>
                        <%: Html.ValidationMessageFor(model => model.Date)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.SortTypeId)%>:</td>
                    <td colspan="3">
                        <%: Html.DropDownListFor(model => model.SortTypeId, Model.SortTypeList) %>
                        <%: Html.ValidationMessageFor(model => model.SortTypeId)%>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <div class="group_title">
                            Вывод сводных таблиц </div>
                    </td>
                </tr>
                <tr class="table_show">
                    <td class='row_title' style="min-width: 220px;">
                        <%: Html.LabelFor(model => model.ShowStorageTable)%>:
                    </td>
                    <td style="min-width: 300px">
                        <%: Html.YesNoToggleFor(model => model.ShowStorageTable)%>
                        <%: Html.ValidationMessageFor(model => model.ShowStorageTable)%>
                    </td>
                    <td class='row_title' style="min-width: 220px;">
                        <%: Html.LabelFor(model => model.ShowArticleGroupTable)%>:
                    </td>
                    <td style="min-width: 300px">
                        <%: Html.YesNoToggleFor(model => model.ShowArticleGroupTable)%>
                        <%: Html.ValidationMessageFor(model => model.ShowArticleGroupTable)%>
                    </td>
                </tr>
                <tr class="table_show">
                    <td class='row_title' style="min-width: 220px;">
                        <%: Html.LabelFor(model => model.ShowAccountOrganizationTable)%>:
                    </td>
                    <td style="min-width: 300px" >
                        <%: Html.YesNoToggleFor(model => model.ShowAccountOrganizationTable)%>
                        <%: Html.ValidationMessageFor(model => model.ShowAccountOrganizationTable)%>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="4">
                        <div class="group_title">
                            Дополнительные настройки отчета </div>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.ShowAccountingPrices)%>: </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.ShowAccountingPrices)%>
                        <%: Html.ValidationMessageFor(model => model.ShowAccountingPrices)%>
                    </td>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.ShowPurchaseCosts)%>: </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.ShowPurchaseCosts, Model.AllowToViewPurchaseCost)%>
                        <%: Html.ValidationMessageFor(model => model.ShowPurchaseCosts)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.DevideByStorages)%>: </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.DevideByStorages)%>
                        <%: Html.ValidationMessageFor(model => model.DevideByStorages)%>
                    </td>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.ShowAveragePurchaseCost)%>: </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.ShowAveragePurchaseCost, Model.AllowToViewPurchaseCost)%>
                        <%: Html.ValidationMessageFor(model => model.ShowAveragePurchaseCost)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.DevideByAccountOrganizations)%>: </td>
                    <td style="width: 40%">
                        <%: Html.YesNoToggleFor(model => model.DevideByAccountOrganizations) %>
                        <%: Html.ValidationMessageFor(model => model.DevideByAccountOrganizations) %>
                    </td>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.ShowAverageAccountingPrice)%>: </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.ShowAverageAccountingPrice)%>
                        <%: Html.ValidationMessageFor(model => model.ShowAverageAccountingPrice)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.StoragesInRows)%>: </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.StoragesInRows)%>
                        <%: Html.ValidationMessageFor(model => model.StoragesInRows)%>
                    </td>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.ShowExtendedAvailability)%>: </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.ShowExtendedAvailability)%>
                        <%: Html.ValidationMessageFor(model => model.ShowExtendedAvailability)%>
                    </td>
                </tr>
                <tr class="table_show">
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.ShowDetailsTable)%>: </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.ShowDetailsTable)%>
                        <%: Html.ValidationMessageFor(model => model.ShowDetailsTable)%>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="table_show">
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.ShowShortDetailsTable)%>: </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.ShowShortDetailsTable)%>
                        <%: Html.ValidationMessageFor(model => model.ShowShortDetailsTable)%>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
            </table>            
        </div>
        <br />
        <div class="group_title">Места хранения, по которым будет построено наличие товаров</div>
        <div class="h_delim"></div>
        <br />
        <%= Html.MultipleSelector("multipleSelectorStorages", Model.Storages, "Список доступных мест хранения", "Выбранные места хранения для отчета")  %>
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
