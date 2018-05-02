<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.MovementWaybill.MovementWaybillDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Детали накладной перемещения
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Waybill_Details.Init();
        MovementWaybill_Details.Init();

        function OnSuccessMovementWaybillRowEdit(ajaxContext) {
            MovementWaybill_Details.OnSuccessMovementWaybillRowEdit(ajaxContext);
        }        
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.BackURL) %>
    <%: Html.HiddenFor(model => model.Id) %>

    <%= Html.PageTitle("MovementWaybill", "Детали накладной перемещения", "№ " + Model.MainDetails.Number + " от " + Model.MainDetails.Date)%>
    
    <div class="button_set">
        <%= Html.Button("btnPrepareToAcceptMovementWaybill", "Подготовить к проводке", Model.AllowToPrepareToAccept, Model.IsPossibilityToPrepareToAccept)%>
        <%= Html.Button("btnCancelReadinessToAcceptMovementWaybill", "Отменить готовность к проводке", Model.AllowToCancelReadinessToAccept, Model.AllowToCancelReadinessToAccept)%>
        <%= Html.Button("btnAcceptMovementWaybill", "Провести", Model.AllowToAccept, Model.IsPossibilityToAccept)%>
        <%= Html.Button("btnCancelAcceptanceMovementWaybill", "Отменить проводку", Model.AllowToCancelAcceptance, Model.AllowToCancelAcceptance)%>
        <%= Html.Button("btnShipMovementWaybill", "Отгрузить", Model.AllowToShip, Model.IsPossibilityToShip)%>
        <%= Html.Button("btnCancelShippingMovementWaybill", "Отменить отгрузку", Model.AllowToCancelShipping, Model.AllowToCancelShipping)%>
        <%= Html.Button("btnReceiptMovementWaybill", "Принять", Model.AllowToReceipt, Model.AllowToReceipt)%>
        <%= Html.Button("btnCancelReceiptMovementWaybill", "Отменить приемку", Model.AllowToCancelReceipt, Model.AllowToCancelReceipt)%>
        <%: Html.Button("btnAddRowsByList", "Добавить позиции списком", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnEditMovementWaybill", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnDeleteMovementWaybill", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <input id="btnBackTo" type="button" value="Назад" />
    </div>

    <div id="messageMovementWaybillDetails"></div>

    <div id="movementWaybill_main_details">
        <% Html.RenderPartial("MovementWaybillMainDetails", Model.MainDetails); %>
    </div>

    <br />

    <div id="messageMovementWaybillRowList"></div>
    <%: Html.WaybillTabPanelWithGrids("MovementWaybillRowGrid", Model.MovementWaybillRows,
                             "MovementWaybillArticleGroupGrid", Model.MovementWaybillArticleGroupsGridState)%>

    <% Html.RenderPartial("MovementWaybillDocGrid", Model.DocGrid); %>

    <div id="movementWaybillRowEdit"></div>
    <div id="movementWaybillChangeRecipientStorage"></div>
    <div id="movementWaybillPrintingForm"></div>
    <div id="movementWaybillSourceLink"></div>
    <div id="curatorSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
     <% var style = Model.AllowToPrintForms ? "'display: block'" : "'display: none'"; %>
     <div class="feature_menu_box" id="feature_menu_box" style=<%= style %> >
        <div class="feature_menu_box_title">Печатные формы</div>
        <% style = Model.AllowToPrintCashMemoForm ? "'display: block'" : "'display: none'";%> <div class="link" id="cashMemoPrintingForm" style=<%= style %>>Товарный чек</div>
        <% style = Model.AllowToPrintInvoiceForm ? "'display: block'" : "'display: none'";%> <div class="link" id="invoicePrintingForm" style=<%= style %>>Счет-фактура</div>
        <% style = Model.AllowToPrintWaybillFormInSenderPrices ? "'display: block'" : "'display: none'";%> <div class="link" id="printingFormSenderCost" style=<%= style %>>Накладная: цены отправителя</div>
        <% style = Model.AllowToPrintWaybillFormInRecipientPrices ? "'display: block'" : "'display: none'";%> <div class="link" id="printingFormReceiptCost" style=<%= style %>>Накладная: цены получателя</div>
        <% style = Model.AllowToPrintWaybillFormInBothPrices ? "'display: block'" : "'display: none'";%> <div class="link" id="printingFormReceiptAllCost" style=<%= style %>>Накладная: все цены</div>
        <% style = Model.AllowToPrintTORG12Form ? "'display: block'" : "'display: none'";%> <div class="link" id="printingFormTORG12" style=<%= style %>>ТОРГ - 12</div>
        <% style = Model.AllowToPrintT1Form ? "'display: block'" : "'display: none'";%> <div class="link" id="printingFormT1" style=<%= style %>>Т - 1</div>
    </div>
</asp:Content>

