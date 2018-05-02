<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ReceiptWaybill.ReceiptWaybillRowEditViewModel>" %>

<script type="text/javascript">
    ReceiptWaybill_RowForEdit.Init();

    function OnFailReceiptWaybillRowEdit(ajaxContext) {
        ReceiptWaybill_RowForEdit.OnFailReceiptWaybillRowEdit(ajaxContext);
    }

    function OnSuccessCountrySave(ajaxContext) {
        ReceiptWaybill_RowForEdit.OnSuccessCountrySave(ajaxContext);
    }

    function OnSuccessManufacturerSave(ajaxContext) {
        ReceiptWaybill_RowForEdit.OnSuccessManufacturerSave(ajaxContext);
    }

    function OnBeginReceiptWaybillRowEdit() {
        StartButtonProgress($("#btnSaveReceiptWaybillRow"));
    }
</script>


<% using (Ajax.BeginForm("EditRow", "ReceiptWaybill", new AjaxOptions() { OnBegin = "OnBeginReceiptWaybillRowEdit", 
       OnSuccess = "OnSuccessReceiptWaybillRowEdit", OnFailure = "OnFailReceiptWaybillRowEdit" }))%>
<%{ %>
    <div class="modal_title"><%: Model.Title %></div>
    <div class="h_delim"></div>
    
    <div style="padding: 5px 10px 5px">
        <div id="messageReceiptWaybillRowEdit"></div>

        <%: Html.HiddenFor(model => model.ArticleId)%>
        <%: Html.HiddenFor(model => model.Id)%>
        <%: Html.HiddenFor(model => model.ReceiptWaybillId)%>
        <%: Html.HiddenFor(model => model.MeasureUnitScale)%>
        <%: Html.HiddenFor(model => model.TotallyReserved)%>
        <%: Html.HiddenFor(model => model.PendingSumIsChangedLast)%>
        <%: Html.HiddenFor(model => model.AllowToViewPurchaseCosts)%>
        <table class='editor_table'>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.ArticleName)%>:                    
                </td>
                <td colspan="3">
                    <span <% if (Model.AllowToEdit){ %> id="ArticleName" class="select_link"<%} %>><%: Model.ArticleName%></span>
                    <%: Html.ValidationMessageFor(model => model.ArticleId)%>
                </td>                   
            </tr>        
            <tr>
                 <td class='row_title'>
                    <%: Html.LabelFor(model => model.ProductionCountryId)%>:
                </td>
                <td colspan="3">
                    <%: Html.DropDownListFor(model => model.ProductionCountryId, Model.ProductionCountryList, new { value = Model.ProductionCountryId }, !Model.AllowToEdit)%>
                    <%if (Model.AllowToEdit && Model.AllowToAddCountry)
                      { %>
                        &nbsp;&nbsp;<span class="edit_action" id="AddCountry">[ Добавить ]</span>
                    <%} %>
                    <%: Html.ValidationMessageFor(model => model.ProductionCountryId)%>
                </td>
            </tr>
            <tr>
                 <td class='row_title'>
                    <%: Html.LabelFor(model => model.ManufacturerId)%>:
                </td>
                <td colspan="3">
                    <%: Html.HiddenFor(x => x.ManufacturerId) %>

                    <span <%if (Model.AllowToEdit){ %> id="ManufacturerName" class="select_link" <%} %>><%: Model.ManufacturerName %></span>
                    <%: Html.ValidationMessageFor(model => model.ManufacturerId)%>
                </td>       
            </tr>    
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.PendingSum)%>:</td>
                <td style="width: 140px">
                    <%if (Model.AllowToViewPurchaseCosts){ %>
                        <%: Html.TextBoxFor(model => model.PendingSum, new { style = "width:90px" }, !Model.AllowToEdit)%>&nbsp;р.
                        <%: Html.ValidationMessageFor(model => model.PendingSum)%>
                    <%}else{%>
                        ---&nbsp;р.
                    <%}%>
                </td>
                <td class='row_title'><%: Html.LabelFor(model => model.PurchaseCost)%>:</td>
                <td style="width: 140px">
                    <%if (Model.AllowToViewPurchaseCosts){ %>
                        <%: Html.TextBoxFor(model => model.PurchaseCost, new { size = 8, tabindex = -1 }, !Model.AllowToEdit)%>&nbsp;p.<br />
                        <%: Html.ValidationMessageFor(model => model.PurchaseCost)%>
                    <%}else{%>
                        ---&nbsp;р.
                    <%}%>         
                </td>
            </tr>
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.PendingCount)%>:</td>
                <td>
                    <%: Html.TextBoxFor(model => model.PendingCount, new { style = "width:90px" }, !Model.AllowToEdit)%>
                    <span id="MeasureUnitName"><%: Model.MeasureUnitName %></span>
                    <%: Html.ValidationMessageFor(model => model.PendingCount) %>
                </td>
                <td></td>
            </tr>
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.PendingValueAddedTaxId)%>:</td>
                <td>
                    <%:Html.ParamDropDownListFor(model => model.PendingValueAddedTaxId, Model.PendingValueAddedTaxList, null, "Укажите ставку НДС", !Model.AllowToEdit)%>
                    <%:Html.ValidationMessageFor(model => model.PendingValueAddedTaxId) %>
                </td>
                <td class='row_title'><%: Html.LabelFor(model => model.PendingValueAddedTaxSum)%>:</td>
                <td>
                    <%if (Model.AllowToViewPurchaseCosts){ %>
                        <span id="ValueAddedTaxSum"><%: Model.PendingValueAddedTaxSum%></span>&nbsp;р.
                    <%}else{%>
                        ---&nbsp;р.
                    <%}%>   
                </td>
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.CustomsDeclarationNumber)%>:                    
                </td>
                <td colspan="3">
                    <%: Html.TextBoxFor(model => model.CustomsDeclarationNumber, new { style = "width:340px", maxlength = "33" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.CustomsDeclarationNumber)%>
                </td>                   
            </tr>
        </table>
    </div>
    
    <div class="button_set">
        <%: Html.SubmitButton("btnSaveReceiptWaybillRow", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%} %>

<div id="manufacturerAdd"></div>
<div id="countryAdd"></div>
<div id="articleSelector"></div>
