<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.MovementWaybill.MovementWaybillRowEditViewModel>" %>

<script type="text/javascript">
    MovementWaybill_RowEdit.Init();

    function OnBeginMovementWaybillRowEdit() {
        StartButtonProgress($("#btnSaveMovementWaybillRow"));
    }

    function OnFailMovementWaybillRowEdit(ajaxContext) {
        MovementWaybill_RowEdit.OnFailMovementWaybillRowEdit(ajaxContext);
    }
</script>

<% using (Ajax.BeginForm("EditRow", "MovementWaybill", new AjaxOptions() { OnBegin = "OnBeginMovementWaybillRowEdit",
       OnSuccess = "MovementWaybill_Details.OnSuccessMovementWaybillRowEdit", OnFailure = "OnFailMovementWaybillRowEdit" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.MovementWaybillId) %>
    <%: Html.HiddenFor(model => model.ArticleId) %>
    <%: Html.HiddenFor(model => model.MeasureUnitScale) %>
    <%: Html.HiddenFor(model => model.SenderStorageId) %>
    <%: Html.HiddenFor(model => model.SenderId) %>
    <%: Html.HiddenFor(model => model.RecipientStorageId) %>
    <%: Html.HiddenFor(model => model.ReceiptWaybillRowId) %>
    <%: Html.HiddenFor(model => model.CurrentReceiptWaybillRowId) %>
    <%: Html.HiddenFor(model => model.MovementWaybillDate) %>
    <%: Html.HiddenFor(model => model.AllowToViewPurchaseCost) %>
    <%: Html.HiddenFor(model => model.ManualSourcesInfo) %>

    <div class="modal_title"><%: Model.Title %></div>
    <div class="h_delim"></div>            

    <div style="padding: 10px 10px 5px">
        <div id="messageMovementWaybillRowEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 160px">
                    <%: Html.LabelFor(model => model.ArticleName) %>:
                </td>
                <td>
                    <span <% if (Model.AllowToEdit) { %> id="ArticleName" class="select_link"<%} %>><%: Model.ArticleName%></span>
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
                <td class="row_title" style="width: 160px">
                    <%: Html.LabelFor(model => model.PurchaseCost) %>:
                </td>
                <td>
                    <span id="PurchaseCost"><%: Model.PurchaseCost %></span>&nbsp;р.
                </td>
                <td class="row_title" style="width: 200px">
                    <%: Html.LabelFor(model => model.PurchaseMarkupPercent) %>:
                </td>
                <td>
                    <span id="PurchaseMarkupPercent"><%: Model.PurchaseMarkupPercent%></span>&nbsp;%&nbsp;&nbsp;||&nbsp;&nbsp;<span id="PurchaseMarkupSum"><%: Model.PurchaseMarkupSum%></span>&nbsp;р.
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.SenderAccountingPriceString) %>:
                </td>
                <td>
                    <span id="SenderAccountingPrice"><%: Model.SenderAccountingPriceString%></span>&nbsp;р.
                    <%: Html.HiddenFor(model => model.SenderAccountingPriceValue)%>
                </td>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.AvailableToReserveFromStorageCount)%>:
                </td>
                <td>
                    <span id="AvailableToReserveFromStorageCount"><%: Model.AvailableToReserveFromStorageCount%></span>
                </td>
            </tr>           
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.RecipientAccountingPriceString) %>:
                </td>
                <td>
                    <span id="RecipientAccountingPrice"><%: Model.RecipientAccountingPriceString%></span>&nbsp;р.
                    <%: Html.HiddenFor(model => model.RecipientAccountingPriceValue)%>
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
                    <%: Html.LabelFor(model => model.MovementMarkupPercent) %>:
                </td>
                <td>
                    <span id="MovementMarkupPercent"><%: Model.MovementMarkupPercent%></span>&nbsp;%&nbsp;&nbsp;||&nbsp;&nbsp;<span id="MovementMarkupSum"><%: Model.MovementMarkupSum%></span>&nbsp;р.
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
                    <%: Html.LabelFor(model => model.ValueAddedTaxId)%>:
                </td>
                <td>
                    <%:Html.ParamDropDownListFor(model => model.ValueAddedTaxId, Model.ValueAddedTaxList, null, "Укажите ставку НДС", !Model.AllowToChangeValueAddedTax)%>
                    <%:Html.ValidationMessageFor(model => model.ValueAddedTaxId)%>
                </td>
                <td class='row_title'><%: Html.LabelFor(model => model.SenderValueAddedTaxSum)%>:</td>
                <td>
                    <span id="SenderValueAddedTaxSum"><%: Model.SenderValueAddedTaxSum%></span>&nbsp;р.&nbsp;&nbsp;||&nbsp;&nbsp;<span id="RecipientValueAddedTaxSum"><%: Model.RecipientValueAddedTaxSum%></span>&nbsp;р.
                </td>
            </tr>
        </table>
        
        <br />

        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 160px">
                    <%: Html.LabelFor(model => model.MovingCount) %>:
                </td>
                <td style="max-width: 340px">
                    <%: Html.TextBoxFor(model => model.MovingCount, new { @style = "width: 100px;" }, !Model.AllowToEdit)%>
                    <span id="MeasureUnitName"><%: Model.MeasureUnitName %></span>
                    <%: Html.ValidationMessageFor(model => model.MovingCount) %>
                </td>
            </tr>
        </table>
    </div>
                
    <div class="button_set">
        <%= Html.SubmitButton("btnSaveMovementWaybillRow", "Сохранить", false, Model.AllowToEdit)%>        
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
               
<%} %>

<div id="articleSelector"></div>
<div id="articleBatchSelector"></div>
<div id="sourceWaybillRowSelector"></div>


