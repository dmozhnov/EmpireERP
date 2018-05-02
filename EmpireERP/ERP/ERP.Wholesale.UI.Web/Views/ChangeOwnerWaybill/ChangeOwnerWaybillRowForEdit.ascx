<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill.ChangeOwnerWaybillRowEditViewModel>" %>

<script type="text/javascript">
    ChangeOwnerWaybill_RowForEdit.Init();

    function OnBeginChangeOwnerWaybillRowEdit() {
        StartButtonProgress($("#btnSaveChangeOwnerWaybillRow"));
    }
</script>

<% using (Ajax.BeginForm("SaveRow", "ChangeOwnerWaybill", new AjaxOptions() { OnBegin = "OnBeginChangeOwnerWaybillRowEdit",
       OnSuccess = "ChangeOwnerWaybill_Details.OnSuccessChangeOwnerWaybillRowEdit", OnFailure = "ChangeOwnerWaybill_RowForEdit.OnFailChangeOwnerWaybillRowEdit" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.ArticleId) %>
    <%: Html.HiddenFor(model => model.MeasureUnitScale) %>
    <%: Html.HiddenFor(model => model.ChangeOwnerWaybillRowId)%>
    <%: Html.HiddenFor(model => model.ChangeOwnerWaybillId) %>
    <%: Html.HiddenFor(model => model.StorageId) %>
    <%: Html.HiddenFor(model => model.SenderId) %>
    <%: Html.HiddenFor(model => model.ReceiptWaybillRowId) %>
    <%: Html.HiddenFor(model => model.CurrentReceiptWaybillRowId) %>
    <%: Html.HiddenFor(model => model.ChangeOwnerWaybillDate) %>
    <%: Html.HiddenFor(model => model.AllowToViewPurchaseCost) %>
    <%: Html.HiddenFor(model => model.ManualSourcesInfo) %>

    <div class="modal_title"><%: Model.Title %></div>
    <div class="h_delim"></div>            

    <div style="padding: 10px 10px 5px; min-width: 550px;">
        <div id="messageChangeOwnerWaybillRowEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 150px">
                    <%: Html.LabelFor(model => model.ArticleName) %>:                    
                </td>
                <td>
                    <span <% if (Model.AllowToEdit) { %> class="select_link"  id="ArticleName"<% }%> ><%: Model.ArticleName%></span>                    
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
                    <%: Html.LabelFor(model => model.AccountingPriceString) %>:
                </td>
                <td>
                    <span id="AccountingPrice"><%: Model.AccountingPriceString%></span> р.
                    <%: Html.HiddenFor(model => model.AccountingPriceValue)%>
                </td>
                <td class="row_title" style="width: 170px">
                    <%: Html.LabelFor(model => model.AvailableToReserveFromStorageCount)%>:
                </td>
                <td>
                    <span id="AvailableToReserveFromStorageCount"><%: Model.AvailableToReserveFromStorageCount%></span>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.PurchaseCost) %>:
                </td>
                <td>
                    <span id="PurchaseCost"><%: Model.PurchaseCost %></span>&nbsp;р.
                </td>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.AvailableToReserveFromPendingCount) %>:
                </td>
                <td>
                    <span id="AvailableToReserveFromPendingCount"><%: Model.AvailableToReserveFromPendingCount %></span>
                </td>
            </tr>           
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.ValueAddedTaxId)%>:
                </td>
                <td>
                    <%:Html.ParamDropDownListFor(model => model.ValueAddedTaxId, Model.ValueAddedTaxList, null, "Укажите ставку НДС", !Model.AllowToEdit)%>
                    <%:Html.ValidationMessageFor(model => model.ValueAddedTaxId)%>
                </td>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.AvailableToReserveCount) %>:
                </td>
                <td>
                    <b class="greentext"><span id="AvailableToReserveCount"><%: Model.AvailableToReserveCount%></span></b>
                </td>
            </tr>
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.ValueAddedTaxSum)%>:</td>
                <td>
                    <span id="ValueAddedTaxSum"><%: Model.ValueAddedTaxSum%></span>&nbsp;р.
                </td>
                <td colspan="2">
                </td>
            </tr>
        </table>
        
        <br />

        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 155px">
                    <%: Html.LabelFor(model => model.MovingCount) %>:
                </td>
                <td style="max-width: 340px">
                    <%= Html.TextBoxFor(model => model.MovingCount, new { @style = "width: 100px;" }, !Model.AllowToEdit)%>
                    <span id="MeasureUnitName"><%: Model.MeasureUnitName %></span>
                    <%: Html.ValidationMessageFor(model => model.MovingCount) %>
                </td>
            </tr>
        </table>
    </div>
                
    <div class="button_set">
        <%: Html.SubmitButton("btnSaveChangeOwnerWaybillRow", "Сохранить", false, Model.AllowToEdit)%>        
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
               
<%} %>

<div id="articleSelector"></div>
<div id="articleBatchSelector"></div>
<div id="sourceWaybillRowSelector"></div>