<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill.ChangeOwnerWaybillDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали накладной смены собственника
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">
        Waybill_Details.Init();
        ChangeOwnerWaybill_Details.Init();

        function OnSuccessChangeOwnerWaybillChangeRecipient(ajaxContext) {
            ChangeOwnerWaybill_Details.OnSuccessChangeOwnerWaybillChangeRecipient(ajaxContext);
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.BackURL) %>

    <%= Html.PageTitle("ChangeOwnerWaybill", Model.Title, Model.Name)%>
    
    <div class="button_set">
        <%= Html.Button("btnPrepareToAccept", "Подготовить к проводке", Model.AllowToPrepareToAccept, Model.IsPossibilityToPrepareToAccept)%>
        <%= Html.Button("btnCancelReadinessToAccept", "Отменить готовность к проводке", Model.AllowToCancelReadinessToAccept, Model.AllowToCancelReadinessToAccept)%>
        <%= Html.Button("btnAccept", "Провести", Model.AllowToAccept, Model.IsPossibilityToAccept)%>
        <%= Html.Button("btnCancelAcceptance", "Отменить проводку", Model.AllowToCancelAcceptance, Model.AllowToCancelAcceptance)%>
        <%: Html.Button("btnAddRowsByList", "Добавить позиции списком", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnDelete", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <input id="btnBackTo" type="button" value="Назад" />
    </div>

    <div id="messageChangeOwnerWaybillDetails"></div>

    <% Html.RenderPartial("ChangeOwnerWaybillMainDetails", Model.MainDetails); %>
    <br />

    <div id="messageChangeOwnerWaybillRowList"></div>
    <%: Html.WaybillTabPanelWithGrids("ChangeOwnerWaybillRowGrid", Model.ChangeOwnerWaybillRows, "ChangeOwnerWaybillArticleGroupGrid", Model.ChangeOwnerWaybillArticleGroupsGridState)%>

    <% Html.RenderPartial("ChangeOwnerWaybillDocsGrid", Model.DocGrid); %>
    
    <div id="changeOwnerWaybillRowForEdit"></div>
    <div id="changeOwnerWaybillChangeRecipient"></div>
    <div id="changeOwnerWaybillPrintingForm"></div>
    <div id="changeOwnerWaybillSourceLink"></div>
    <div id="curatorSelector"></div>
</asp:Content>



<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    <div class="feature_menu_box" id="feature_menu_box" <% if (!Model.AllowToPrintForms){%> style="display:none;" <%} %>>
        <div class="feature_menu_box_title">Печатные формы</div>
        <div class="link" id="cashMemoPrintingForm">Товарный чек</div>
        <div class="link" id="invoicePrintingForm">Счет-фактура</div>
        <div class="link" id="printingFormAllCost">Накладная</div>
        <div class="link" id="printingFormTORG12">ТОРГ - 12</div>
        <div class="link" id="printingFormT1">Т - 1</div>
    </div>
</asp:Content>
