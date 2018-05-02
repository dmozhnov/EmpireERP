<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ReceiptWaybill.ReceiptWaybillDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Детали приходной накладной
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Waybill_Details.Init();
        ReceiptWaybill_Details.Init();
        <% if (TempData["Message"] != null) { %>
            $(document).ready(function () {
                ShowSuccessMessage('<%: TempData["Message"].ToString() %>', "messageReceiptWaybillDetailsEdit");
            });
        <%} %>

        function OnSuccessReceiptWaybillRowEdit(ajaxContext) {
            ReceiptWaybill_Details.OnSuccessReceiptWaybillRowEdit(ajaxContext);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.BackURL) %>
    <%: Html.HiddenFor(model => model.Id) %>

    <%= Html.PageTitle("ReceiptWaybill", "Детали приходной накладной", "№ " + Model.MainDetails.Number + " от " + Model.MainDetails.Date)%>

    <div class="button_set">
        <%: Html.Button("btnAcceptReceiptWaybill", "Провести", Model.AllowToAccept, Model.AllowToAccept)%>
        <%: Html.Button("btnAcceptRetroactivelyReceiptWaybill", "Провести задним числом", Model.AllowToAcceptRetroactively, Model.AllowToAcceptRetroactively)%>
        <%: Html.Button("btnCancelAcceptanceReceiptWaybill", "Отменить проводку", Model.AllowToCancelAcceptance, Model.AllowToCancelAcceptance)%>
        
        <%: Html.Button("btnReceiptReceiptWaybill", "Принять", Model.AllowToReceipt, Model.IsPossibilityToReceipt)%>
        <%: Html.Button("btnCancelReceiptReceiptWaybill", "Отменить приемку", Model.AllowToCancelReceipt, Model.AllowToCancelReceipt)%>

        <%: Html.Button("btnApproveReceiptWaybill", "Согласовать", Model.AllowToApprove, Model.AllowToApprove)%>
        <%: Html.Button("btnCancelApprovementReceiptWaybill", Model.CancelApprovementButtonCaption, Model.AllowToCancelApprovement, Model.AllowToCancelApprovement)%>

        <%: Html.Button("btnEditReceiptWaybill", "Редактировать", Model.AllowToEdit || Model.AllowToEditProviderDocuments,
            Model.AllowToEdit || Model.AllowToEditProviderDocuments)%>
        <%: Html.Button("btnDeleteReceiptWaybill", "Удалить", Model.AllowToDelete, Model.AllowToDelete) %>
        <%: Html.Button("btnCreatePriceList", "Создать реестр цен", Model.AllowToCreateAccountingPriceList, Model.AllowToCreateAccountingPriceList) %>
        
        <input id="btnBackTo" type="button" value="Назад" />
    </div>
        
    <div id="messageReceiptWaybillDetailsEdit"></div>

    <div id="receiptWaybill_main_details">
        <% Html.RenderPartial("ReceiptWaybillMainDetails", Model.MainDetails); %>
    </div>

    <br />
    
    <div id="messageReceiptWaybillRowList"></div>
    
    <%if (!Model.IsReceipted)
      { %>
        <%: Html.WaybillTabPanelWithGrids("ReceiptWaybillRowGrid", Model.ReceiptWaybillRows, "ReceiptWaybillArticleGroupGrid", Model.ReceiptArticleGroupsGridState)%>
    <%} %>

    <!-- Гриды накладной с расхождениями -->
    <%if (Model.IsReceipted && !Model.IsApproved){ %>
         <%: Html.WaybillTabPanelWithGrids("ReceiptWaybillWithDifDetailsAddedRowGrid", Model.ReceiptWaybillAddedRowsGrid,
                     "ReceiptWaybillWithDifDetailsAddedArticleGroupGrid", Model.ReceiptWaybillAddedArticleGroupsGridState)%>

         <%: Html.WaybillTabPanelWithGrids("ReceiptWaybillWithDifDetailsDifRowGrid", Model.ReceiptWaybillDifRowsGrid,
                   "ReceiptWaybillWithDifDetailsDifArticleGroupGrid", Model.ReceiptWaybillDifArticleGroupsGridState)%>

         <%: Html.WaybillTabPanelWithGrids("ReceiptWaybillWithDifDetailsMatchRowGrid", Model.ReceiptWaybillMatchRowsGrid,
                  "ReceiptWaybillWithDifDetailsMatchArticleGroupGrid", Model.ReceiptWaybillMatchArticleGroupsGridState)%>
    <%} %>

    <!-- Грид окончательно согласованной накладной-->
    <%if (Model.IsApproved){%>
        <%: Html.WaybillTabPanelWithGrids("ReceiptWaybillApprovementDetailsGrid", Model.ReceiptWaybillApproveRowsGrid, 
                    "ReceiptWaybillApprovementArticleGroupGrid", Model.ReceiptWaybillApproveArticleGroupsGridState)%>
    <%}%>

    <!-- Грид с документами -->
    <% Html.RenderPartial("ReceiptWaybillDocumentsGrid", Model.DocumentsGrid); %>
              
    <div id="receiptWaybillRowForEdit"></div>
    <div id="receiptWaybillProviderForChange"></div>
    <div id="receiptWaybillStorageChange"></div>
    <div id="receiptWaybillPrintingForm"></div>
    <div id="curatorSelector"></div>
    <div id="dateTimeSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
<%  var needToPrint = false;
    if (Model.MainDetails.AllowToPrintForms || Model.MainDetails.AllowToPrintDivergenceAct) needToPrint = true;
    var styleDisplayFeatureMenuPrintingForm = needToPrint ? "'display: block'" : "'display: none'"; 
    var stylePrintingForm = Model.MainDetails.AllowToPrintForms ? "'display: block'" : "'display: none'";
    var styleDivergenceAct = Model.MainDetails.AllowToPrintDivergenceAct ? "'display: block'" : "'display: none'"; %>
     <div class="feature_menu_box" id="feature_menu_box" style=<%= styleDisplayFeatureMenuPrintingForm %> >
        <div class="feature_menu_box_title">Печатные формы</div>    
            <div class="link" id="printingForm" style=<%= stylePrintingForm %>>Приходная накладная</div>
            <div class="link" id="divergenceActPrintingForm" style=<%= styleDivergenceAct %>>Акт расхождений</div>
            <div class="link" id="printingFormTORG12" style=<%= stylePrintingForm %>>ТОРГ - 12</div>
    </div>
</asp:Content>
