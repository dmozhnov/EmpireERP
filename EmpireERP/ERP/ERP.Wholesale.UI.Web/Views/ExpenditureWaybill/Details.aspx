<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ExpenditureWaybill.ExpenditureWaybillDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали накладной реализации товаров
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Waybill_Details.Init();
        ExpenditureWaybill_Details.Init();                        
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.BackURL) %>

    <%= Html.PageTitle("ExpenditureWaybill", "Детали накладной реализации товаров", Model.Name, "/Help/GetHelp_ExpenditureWaybill_Details")%>

    <div class="button_set">
        <%: Html.Button("btnPrepareToAccept", "Подготовить к проводке", Model.AllowToPrepareToAccept, Model.IsPossibilityToPrepareToAccept)%>
        <%: Html.Button("btnCancelReadinessToAccept", "Отменить готовность к проводке", Model.AllowToCancelReadinessToAccept, Model.AllowToCancelReadinessToAccept)%>
        <%: Html.Button("btnAccept", "Провести", Model.AllowToAccept, Model.IsPossibilityToAccept)%>
        <%: Html.Button("btnAcceptRetroactively", "Провести задним числом", Model.AllowToAcceptRetroactively, Model.IsPossibilityToAcceptRetroactively)%>
        <%: Html.Button("btnCancelAcceptance", "Отменить проводку", Model.AllowToCancelAcceptance, Model.AllowToCancelAcceptance)%>
        <%: Html.Button("btnShip", "Отгрузить", Model.AllowToShip, Model.IsPossibilityToShip)%>
        <%: Html.Button("btnShipRetroactively", "Отгрузить задним числом", Model.AllowToShipRetroactively, Model.IsPossibilityToShipRetroactively)%>
        <%: Html.Button("btnCancelShipping", "Отменить отгрузку", Model.AllowToCancelShipping, Model.AllowToCancelShipping)%>
        <%: Html.Button("btnAddRowsByList", "Добавить позиции списком", Model.AllowToEdit, Model.AllowToEdit)%>
        <%: Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%: Html.Button("btnDelete", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <input id="btnBackTo" type="button" value="Назад" />
    </div>

    <div id="messageExpenditureWaybillEdit"></div>
    <% Html.RenderPartial("ExpenditureWaybillMainDetails", Model.MainDetails); %>
    <br />

    <div id="messageExpenditureWaybillRowList"></div>
    <%: Html.WaybillTabPanelWithGrids("ExpenditureWaybillRowGrid", Model.RowGrid, "ExpenditureWaybillArticleGroupGrid", Model.ArticleGroupGridState)%>

    <% Html.RenderPartial("ExpenditureWaybillDocumentGrid", Model.DocumentGrid); %>

    <div id="expenditureWaybillRowEdit"></div>
    <div id="expenditureWaybillSourceLink"></div>
    <div id="dealPaymentFormSelector"></div>
    <div id="dealQuotaSelector"></div>
    <div id="expenditureWaybillPrintingForm"></div>
    <div id="curatorSelector"></div>
    <div id="dateTimeSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    <% var style = Model.AllowToPrintForms ? "'display: block'" : "'display: none'"; %>
    <div class="feature_menu_box" id="feature_menu_box" style=<%= style %> >    
        <div class="feature_menu_box_title">Печатные формы</div>
        <div class="link" id="cashMemoPrintingForm">Товарный чек</div>
        <div class="link" id="paymentInvoicePrintingForm">Счет на оплату</div>
        <div class="link" id="invoicePrintingForm">Счет-фактура</div>
        <div class="link" id="printingForm">Расходная накладная</div>
        <div class="link" id="printingFormTORG12">ТОРГ - 12</div>
        <div class="link" id="printingFormT1">Т - 1</div>
    </div>
</asp:Content>
