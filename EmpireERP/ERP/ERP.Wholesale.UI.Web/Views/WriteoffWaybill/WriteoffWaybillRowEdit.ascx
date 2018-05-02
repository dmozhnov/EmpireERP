<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.WriteoffWaybill.WriteoffWaybillRowEditViewModel>" %>

<script type="text/javascript">
    WriteoffWaybill_RowEdit.Init();

    function OnBeginWriteoffWaybillRowEdit() {
        StartButtonProgress($("#btnSaveWriteoffWaybillRow"));
    }
</script>

<% using (Ajax.BeginForm("EditRow", "WriteoffWaybill", new AjaxOptions() { OnBegin = "OnBeginWriteoffWaybillRowEdit",
       OnSuccess = "WriteoffWaybill_Details.OnSuccessWriteoffWaybillRowEdit", OnFailure = "WriteoffWaybill_RowEdit.OnFailWriteoffWaybillRowEdit" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.ArticleId) %>
    <%: Html.HiddenFor(model => model.MeasureUnitScale) %>
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.SenderId) %>
    <%: Html.HiddenFor(model => model.WriteoffWaybillId) %>
    <%: Html.HiddenFor(model => model.SenderStorageId) %>
    <%: Html.HiddenFor(model => model.ReceiptWaybillRowId) %>
    <%: Html.HiddenFor(model => model.CurrentReceiptWaybillRowId) %>
    <%: Html.HiddenFor(model => model.WriteoffWaybillDate) %>
    <%: Html.HiddenFor(model => model.AllowToViewPurchaseCost) %>
    <%: Html.HiddenFor(model => model.ManualSourcesInfo) %>

    <div class="modal_title"><%: Model.Title %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">
        <div id="messageWriteoffWaybillRowEdit"></div> 
    
        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 150px">
                    <%: Html.LabelFor(model => model.ArticleName) %>:                    
                </td>
                <td>
                    <span <% if (Model.AllowToEdit) { %>id="ArticleName" class="select_link" <%} %>><%: Model.ArticleName%></span>
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
                        <span id="BatchLink" class="select_link" style="display: none;">изменить партию</span>
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
                    <%: Html.LabelFor(model => model.MarkupPercent) %>:
                </td>
                <td>
                    <span id="MarkupPercent"><%: Model.MarkupPercent%></span> % &nbsp;||&nbsp; <span id="MarkupSum"><%: Model.MarkupSum%></span> р.
                </td>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.AvailableToReserveCount) %>:
                </td>
                <td>
                    <b  class="greentext"><span id="AvailableToReserveCount"><%: Model.AvailableToReserveCount%></span></b>
                </td>
            </tr>
        </table>

        <br />

        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 150px">
                    <%: Html.LabelFor(model => model.WritingoffCount) %>:
                </td>
                <td style="max-width: 340px">
                    <%: Html.TextBoxFor(model => model.WritingoffCount, new { @style = "width: 100px;" }, !Model.AllowToEdit)%>
                    <span id="MeasureUnitName"><%: Model.MeasureUnitName %></span>
                    <%: Html.ValidationMessageFor(model => model.WritingoffCount)%>
                </td>
            </tr>
        </table>
    </div>
    
    <div class="button_set">
        <%: Html.SubmitButton("btnSaveWriteoffWaybillRow", "Сохранить", false, Model.AllowToEdit) %>        
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%} %>

<div id="articleSelector"></div>
<div id="articleBatchSelector"></div>
<div id="sourceWaybillRowSelector"></div>