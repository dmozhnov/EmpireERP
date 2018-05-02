<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill.ReturnFromClientWaybillDetailsViewModel>"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали накладной возврата товаров
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Waybill_Details.Init();
        ReturnFromClientWaybill_Details.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.BackURL) %>
    
    <%= Html.PageTitle("ReturnFromClientWaybill", "Детали накладной возврата от клиента", Model.Name, "/Help/GetHelp_ReturnFromClientWaybill_Details")%>

    <div class="button_set">
        <%= Html.Button("btnPrepareToAccept", "Подготовить к проводке", Model.AllowToPrepareToAccept, Model.IsPossibilityToPrepareToAccept)%>
        <%= Html.Button("btnCancelReadinessToAccept", "Отменить готовность к проводке", Model.AllowToCancelReadinessToAccept, Model.AllowToCancelReadinessToAccept)%>
        <%= Html.Button("btnAccept", "Провести", Model.AllowToAccept, Model.IsPossibilityToAccept)%>
        <%= Html.Button("btnCancelAcceptance", "Отменить проводку", Model.AllowToCancelAcceptance, Model.AllowToCancelAcceptance)%>
        <%= Html.Button("btnReceipt", "Принять", Model.AllowToReceipt, Model.AllowToReceipt)%>
        <%= Html.Button("btnCancelReceipt", "Отменить приемку", Model.AllowToCancelReceipt, Model.AllowToCancelReceipt)%>
        <%= Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnDelete", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>        
        <input id="btnBackTo" type="button" value="Назад" />
    </div>

    <div id="messageReturnFromClientWaybillEdit"></div>
    <% Html.RenderPartial("ReturnFromClientWaybillMainDetails", Model.MainDetails); %>
    <br />

    <div id="messageReturnFromClientWaybillRowList"></div>
    <%: Html.WaybillTabPanelWithGrids("ReturnFromClientWaybillRowGrid", Model.RowGrid, "ReturnFromClientWaybillArticleGroupGrid", Model.ArticleGroupGridState)%>

    <% Html.RenderPartial("ReturnFromClientWaybillDocumentGrid", Model.DocumentGrid); %>

    <div id="returnFromClientWaybillRowEdit"></div>
    <div id="returnFromClientWaybillPrintingForm"></div>
    <div id="curatorSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    <% var style = Model.AllowToPrintForms ? "'display: block'" : "'display: none'"; %>
    <div class="feature_menu_box" id="feature_menu_box" style=<%= style %> >    
        <div class="feature_menu_box_title">Печатные формы</div>        
        <div class="link" id="printingFormTORG12">ТОРГ - 12</div>
    </div>
</asp:Content>
