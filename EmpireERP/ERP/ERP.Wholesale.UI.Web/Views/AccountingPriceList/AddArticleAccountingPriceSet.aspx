<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.AccountingPriceList.ArticleAccountingPriceSetAddViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Добавление набора товаров
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        AccountingPriceList_AddArticleAccountingPriceSet.Init();

        function OnBeginAddArticleAccountingPriceSet() {
            StartButtonProgress($("#btnAddArticleAccountingPriceSet"));
        }
    </script> 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">  
    <%= Html.PageTitle("AccountingPriceList", Model.Title, Model.AccountingPriceListName)%>

    <% using (Ajax.BeginForm("SaveArticleAccountingPriceSet", "AccountingPriceList",
           new AjaxOptions()
           {
               OnBegin = "OnBeginAddArticleAccountingPriceSet",
               OnSuccess = "AccountingPriceList_AddArticleAccountingPriceSet.OnSuccessAddArticleAccountingPriceSet",
               OnFailure = "AccountingPriceList_AddArticleAccountingPriceSet.OnFailAddArticleAccountingPriceSet"
           })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.Id) %>
        <%: Html.HiddenFor(model => model.AccountingPriceListId) %>
        <%: Html.HiddenFor(model => model.BackURL) %>        
        <%: Html.HiddenFor(model => model.StorageIDs) %>
        <%: Html.HiddenFor(model => model.ArticleGroupsIDs)%>
        <%: Html.HiddenFor(model => model.AllStorages)%> 
        <%: Html.HiddenFor(model => model.AllArticleGroups)%>

        <div id="messageAddArticleAccountingPriceSet"></div>

        <div style="background: #fff; padding: 10px 0;">
            <div class="group_title">Группы товаров</div>
            <div class="h_delim"></div>
            <br />
            <%= Html.MultipleSelector("multipleSelectorArticleGroups", Model.ArticleGroups, "Список доступных групп товаров", "Выбранные группы товаров")  %>
            <br />

            <div style="min-width: 450px; max-width: 630px; padding-left: 16px;">
                <table>
                    <tr>
                        <td class='row_title'><%: Html.LabelFor(model => model.OnlyAvailability) %>:&nbsp;&nbsp;</td>
                        <td><%: Html.YesNoToggleFor(model => model.OnlyAvailability) %></td>
                    </tr>
                </table>            
            </div>
            <br />

            <div class="OnlyAvailabilityContainer" style="display: none">
                <div class="group_title">Места хранения, наличие на которых попадет в реестр цен</div>
                <div class="h_delim"></div>
                <br />
                <%= Html.MultipleSelector("multipleSelectorStorages", Model.Storages, "Список доступных мест хранения", "Выбранные места хранения для отчета")  %>
                <br />
            </div>

            <div class="button_set">
                <input id="btnAddArticleAccountingPriceSet" type="submit" value="Добавить" />
                <input type="button" id="btnBack" value="Назад" />
            </div>
        </div>
    <%} %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
