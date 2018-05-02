<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.AccountingPriceList.AccountingPriceListDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали реестра цен
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        AccountingPriceList_Details.Init();
    </script>

    <% if (TempData["Message"] != null) { %>
        <script type="text/javascript">
            $(document).ready(function () {
                ShowSuccessMessage('<%: TempData["Message"].ToString() %>', "messageAccountingPriceListDetails");
            });
        </script>
    <%} %>   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.MainDetails.Id) %>    
    <%: Html.HiddenFor(model => model.BackURL) %>
    
    <%= Html.PageTitle("AccountingPriceList", "Детали реестра цен", Model.MainDetails.Name )%>
    
    <div class="button_set">
        <%: Html.Button("btnAccept", "Провести", Model.AllowToAccept, Model.AllowToAccept) %>
        <%: Html.Button("btnCancelAcceptance", "Отменить проводку", Model.AllowToCancelAcceptance, Model.AllowToCancelAcceptance)%>
        <%: Html.Button("btnEditAccountingPriceList", "Редактировать", Model.AllowToEdit, Model.AllowToEdit) %>
        <%: Html.Button("btnDeleteAccountingPriceList", "Удалить", Model.AllowToDelete, Model.AllowToDelete) %>
         <input id="btnAccountingPriceListBack" type="button" value="Назад" />
    </div>
    
    <div id="messageAccountingPriceListDetails"></div>
    
    <div id="accountingPriceList_main_details">
        <% Html.RenderPartial("AccountingPriceListMainDetails", Model.MainDetails); %>
    </div>
    
    <br />
    <div id="messageAccountingPriceListDetailsArticleList"></div>
    
    <% Html.RenderPartial("AccountingPriceArticlesGrid", Model.ArticleGrid); %>
    <div id="messageAccountingPriceListDetailsStorageList"></div>

    <% Html.RenderPartial("AccountingPriceStoragesGrid", Model.StorageGrid); %>
    <div id="articleSelectList"></div>
    
    <div id="storageSelectList"></div>

    <div id="AccountingPriceListPrintingForm"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    <% var style = Model.AllowToPrintForms ? "display: block" : "display: none"; %>
    <div class="feature_menu_box" id="feature_menu_box" style="<%= style %>" >
        <div class="feature_menu_box_title">
            Печатные формы</div>
        <div class="link" id="lnkAccountingPriceListPrintingForm">Переоценка</div>
        <div class="link" id="lnkAccountingPriceListPrintingFormExpanded">Переоценка: развернуто</div>
    </div>
</asp:Content>
