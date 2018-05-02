<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ExpenditureWaybill.ExpenditureWaybillRowEditViewModel>" %>

<script type="text/javascript">
    ExpenditureWaybill_RowEdit.Init();

    function OnSuccessExpenditureWaybillRowEdit(ajaxContext) {
        ExpenditureWaybill_RowEdit.OnSuccessExpenditureWaybillRowEdit(ajaxContext);
    }

    function OnBeginExpenditureWaybillRowEdit() {
        StartButtonProgress($("#btnSaveExpenditureWaybillRow"));
    }
</script>

<% using (Ajax.BeginForm("SaveRow", "ExpenditureWaybill", new AjaxOptions() { OnBegin = "OnBeginExpenditureWaybillRowEdit",
       OnSuccess = "OnSuccessExpenditureWaybillRowEdit", OnFailure = "ExpenditureWaybill_RowEdit.OnFailExpenditureWaybillRowEdit" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.ArticleId) %>
    <%: Html.HiddenFor(model => model.MeasureUnitScale) %>
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.ExpenditureWaybillId) %>
    <%: Html.HiddenFor(model => model.SenderStorageId) %>
    <%: Html.HiddenFor(model => model.SenderId) %>
    <%: Html.HiddenFor(model => model.ReceiptWaybillRowId) %>
    <%: Html.HiddenFor(model => model.CurrentReceiptWaybillRowId) %>
    <%: Html.HiddenFor(model => model.ExpenditureWaybillDate) %>
    <%: Html.HiddenFor(model => model.RoundSalePrice) %>
    <%: Html.HiddenFor(model => model.AllowToViewPurchaseCost) %>
    <%: Html.HiddenFor(model => model.ManualSourcesInfo) %>

    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ExpenditureWaybill_RowEdit") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">
        <div id="messageExpenditureWaybillRowEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 150px">
                    <%: Html.LabelFor(model => model.ArticleName) %>:
                </td>
                <td>
                    <span <% if (Model.AllowToEdit) { %> class="select_link" id="ArticleName" <%} %>><%: Model.ArticleName%></span>
                    <%: Html.ValidationMessageFor(model => model.ArticleId)%>
                </td>                   
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.BatchName) %>:
                </td>
                <td>
                    <span id="BatchName"><%: Model.BatchName%></span> &nbsp;
                    <% if (Model.AllowToEdit) %>
                    <%{ %>
                        <span class="select_link" id="BatchLink" style="display: none;">изменить партию</span>
                        <span class="select_link" id="ManualSourcesLink" style="display: none;">изменить источники</span>
                    <%} %>
                </td>
            </tr>
        </table>
        
        <br />
    
        <table class="display_table">
            <tr>
                <td class="row_title" style="width: 150px">
                    <%: Html.LabelFor(model => model.PurchaseCost) %>:
                </td>
                <td>
                    <span id="PurchaseCost"><%: Model.PurchaseCost%></span> р.
                </td>
                <td class="row_title" style="width: 180px">
                    <%: Html.LabelFor(model => model.AvailableToReserveFromStorageCount)%>:
                </td>
                <td style="min-width: 60px">
                    <span id="AvailableToReserveFromStorageCount"><%: Model.AvailableToReserveFromStorageCount%></span>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.SenderAccountingPrice) %>:
                </td>
                <td>
                    <span id="SenderAccountingPrice"><%: Model.SenderAccountingPrice%></span> р.
                </td>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.AvailableToReserveFromPendingCount) %>:
                </td>
                <td>
                    <span id="AvailableToReserveFromPendingCount"><%: Model.AvailableToReserveFromPendingCount%></span>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.SalePriceString) %>:
                </td>
                <td>
                    <span id="SalePrice"><%: Model.SalePriceString%></span> р.
                    <%: Html.HiddenFor(model => model.SalePriceValue)%>
                </td>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.AvailableToReserveCount) %>:
                </td>
                <td>
                    <b class="greentext"><span id="AvailableToReserveCount"><%: Model.AvailableToReserveCount%></span></b>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.MarkupPercent) %>:
                </td>
                <td>
                    <span id="MarkupPercent"><%: Model.MarkupPercent%></span> % &nbsp;||&nbsp; <span id="MarkupSum"><%: Model.MarkupSum%></span> р.
                </td>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.DealQuotaDiscountPercent) %>:
                </td>
                <td>
                    <span id="DealQuotaDiscountPercent"><%: Model.DealQuotaDiscountPercent%></span> %
                </td>
            </tr>
        </table>

        <br />

        <table class="editor_table">
            <tr>    
                <td class="row_title">
                    <%: Html.LabelFor(model => model.ValueAddedTaxId)%>:
                </td>
                <td style="width: 160px">
                    <%:Html.ParamDropDownListFor(model => model.ValueAddedTaxId, Model.ValueAddedTaxList, null, "Укажите ставку НДС", !Model.AllowToEdit)%>
                    <%:Html.ValidationMessageFor(model => model.ValueAddedTaxId)%>
                </td>
                <td class='row_title' style="width: 90px"><%: Html.LabelFor(model => model.ValueAddedTaxSum)%>:</td>
                <td>
                    <span id="ValueAddedTaxSum"><%: Model.ValueAddedTaxSum%></span>&nbsp;р.
                </td>
            </tr>
            <tr>
                <td class="row_title" style="width: 150px">
                    <%: Html.LabelFor(model => model.SellingCount) %>:
                </td>
                <td style="max-width: 340px">
                    <%: Html.TextBoxFor(model => model.SellingCount, new { @style = "width: 100px;" }, !Model.AllowToEdit)%>
                    <span id="MeasureUnitName"><%: Model.MeasureUnitName %></span>
                    <%: Html.ValidationMessageFor(model => model.SellingCount)%>
                </td>
            </tr>
        </table>
    </div>
    
    <div class="button_set">
        <%: Html.SubmitButton("btnSaveExpenditureWaybillRow", "Сохранить", false, Model.AllowToEdit) %>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%} %>

<div id="articleSelector"></div>
<div id="articleBatchSelector"></div>
<div id="sourceWaybillRowSelector"></div>