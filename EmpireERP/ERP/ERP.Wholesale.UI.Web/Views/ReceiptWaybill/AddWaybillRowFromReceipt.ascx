<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ReceiptWaybill.ReceiptRowAddViewModel>" %>

<script type="text/javascript">
    ReceiptWaybill_AddWaybillRowFromReceipt.Init();

    function OnSuccessReceiptWaybillRowAdd(ajaxContext) {
        ReceiptWaybill_AddWaybillRowFromReceipt.OnSuccessReceiptWaybillRowAdd(ajaxContext);
    }

    function OnFailReceiptWaybillRowAdd(ajaxContext) {
        ReceiptWaybill_AddWaybillRowFromReceipt.OnFailReceiptWaybillRowAdd(ajaxContext);
    }

    function OnSuccessCountrySave(ajaxContext) {
        ReceiptWaybill_AddWaybillRowFromReceipt.OnSuccessCountrySave(ajaxContext);
    }

    function OnSuccessManufacturerSave(ajaxContext) {
        ReceiptWaybill_AddWaybillRowFromReceipt.OnSuccessManufacturerSave(ajaxContext);
    }

    function OnBeginReceiptWaybillRowAdd() {
        StartButtonProgress($("#btnSaveWaybillRow"));
    }
</script>

<% using (Ajax.BeginForm("AddWaybillRowFromReceipt", "ReceiptWaybill", new AjaxOptions() { OnBegin = "OnBeginReceiptWaybillRowAdd", 
       OnFailure = "OnFailReceiptWaybillRowAdd", OnSuccess = "OnSuccessReceiptWaybillRowAdd" }))%>
<%{ %>
    <%: Html.HiddenFor(model=> model.WaybillId) %>
    <%: Html.HiddenFor(model=> model.ArticleId) %>

    <div class='modal_title'>Добавление позиции в накладную</div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 0px">
        <div id='messageReceiptWaybillRowAdd'></div>

        <table class='editor_table'>
            <tr>
                <td class='row_label'><%: Html.LabelFor(model => model.ArticleId)%>:</td>
                <td>
                    <span id="ArticleName" class="select_link">Выберите товар</span>
                    <%: Html.ValidationMessageFor(model => model.ArticleId)%>
                </td>
            </tr>
            <tr>
                 <td class='row_title'>
                    <%: Html.LabelFor(model => model.ProductionCountryId)%>:
                </td>
                <td colspan="3">
                    <%: Html.DropDownListFor(model => model.ProductionCountryId, Model.ProductionCountryList, new { style = "width:280px", value = Model.ProductionCountryId })%>
                    &nbsp;&nbsp;<span class="edit_action" id="AddCountry">[ Добавить ]</span>
                    <%: Html.ValidationMessageFor(model => model.ProductionCountryId)%>
                </td>
            </tr>
            <tr>
                 <td class='row_title'>
                    <%: Html.LabelFor(model => model.ManufacturerId)%>:
                </td>
                <td colspan="3">
                    <%: Html.HiddenFor(x => x.ManufacturerId) %>

                    <span class="select_link" id="ManufacturerName"><%: Model.ManufacturerName %></span>
                    <%: Html.ValidationMessageFor(model => model.ManufacturerId)%>
                </td>
            </tr>
            <tr>
                <td class='row_label'><%: Html.LabelFor(model=>model.ReceiptedCount) %>:</td>
                <td>
                    <%: Html.TextBoxFor(model => model.ReceiptedCount, new { size = "8" })%>
                    <span id='measureUnitNameOfReceiptedCount'></span>
                    <%: Html.ValidationMessageFor(model => model.ReceiptedCount)%>
                </td>
            </tr>
            <tr>
                <td class='row_label'><%: Html.LabelFor(model=>model.ProviderCount) %>:</td>
                <td>
                    <%: Html.TextBoxFor(model => model.ProviderCount, new { size = "8" })%>
                    <span id='measureUnitNameOfProviderCount'></span>
                    <%: Html.ValidationMessageFor(model => model.ProviderCount)%>
                </td>
            </tr>
            <tr>
                <td class='row_label'><%: Html.LabelFor(model=>model.ProviderSum) %>:</td>
                <td>
                    <% if (Model.AllowToViewPurchaseCosts){ %> 
                            <%: Html.TextBoxFor(model => model.ProviderSum, new { size = "8" }, !Model.AllowToViewPurchaseCosts)%>&nbsp;р.
                            <%: Html.ValidationMessageFor(model => model.ProviderSum)%>
                    <% }else{ %> 
                        ---&nbsp;р.
                    <% } %>        
                </td>
            </tr>
            <tr>
                <td class='row_label'><%: Html.LabelFor(model=>model.CustomsDeclarationNumber) %>:</td>
                <td><%: Html.TextBoxFor(model => model.CustomsDeclarationNumber, new { style = "width:280px", maxlength = "33" })%></td>
            </tr>
        </table>
                
        <div class="button_set">
            <input id="btnSaveWaybillRow"type="submit" value="Добавить" />
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>
<%} %>

<div id="articleSelector"></div>
<div id="manufacturerAdd"></div>
<div id="countryAdd"></div>