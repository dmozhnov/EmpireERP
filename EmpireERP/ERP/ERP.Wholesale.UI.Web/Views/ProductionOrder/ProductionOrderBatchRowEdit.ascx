<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderBatchRowEditViewModel>" %>

<script type="text/javascript">
    ProductionOrder_BatchRowEdit.Init();

    function OnFailProductionOrderBatchRowEdit(ajaxContext) {
        ProductionOrder_BatchRowEdit.OnFailProductionOrderBatchRowEdit(ajaxContext);
    }

    function OnSuccessManufacturerSave(ajaxContext) {
        ProductionOrder_BatchRowEdit.OnSuccessManufacturerSave(ajaxContext);
    }

    function OnSuccessCountrySave(ajaxContext) {
        ProductionOrder_BatchRowEdit.OnSuccessCountrySave(ajaxContext);
    }

    function OnBeginProductionOrderBatchRowEdit() {
        ProductionOrder_BatchRowEdit.OnBeginProductionOrderBatchRowEdit();
    }
</script>


<% using (Ajax.BeginForm("EditRow", "ProductionOrder", new AjaxOptions()
   {
       OnBegin = "OnBeginProductionOrderBatchRowEdit", 
       OnSuccess = "OnSuccessProductionOrderBatchRowEdit", OnFailure = "OnFailProductionOrderBatchRowEdit" }))%>
<%{ %>
    <div class="modal_title"><%: Model.Title %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">
        <div id="messageProductionOrderBatchRowEdit"></div>

        <%: Html.HiddenFor(model => model.Id)%>
        <%: Html.HiddenFor(model => model.BatchId)%>
        <%: Html.HiddenFor(model => model.ProducerId)%>
        <%: Html.HiddenFor(model => model.MeasureUnitScale)%>

        <table class='editor_table'>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.ArticleName)%>:                    
                </td>
                <td colspan="3">               
                    <span <% if (Model.AllowToEditRow) { %>id="ArticleName" class="select_link"<%} %>><%: Model.ArticleName%></span>
                    <%: Html.HiddenFor(model => model.ArticleId)%>
                    <%: Html.ValidationMessageFor(model => model.ArticleId)%>
                </td>                   
            </tr> 
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.ManufacturerName)%>:                    
                </td>
                <td colspan="3">
                    <span <% if (Model.AllowToEditRow) { %> id="ManufacturerName" class="select_link"<%} %>><%:Model.ManufacturerName %></span>
                    <%: Html.HiddenFor(model => model.ManufacturerId)%>
                    <%: Html.ValidationMessageFor(model => model.ManufacturerId)%>
                </td>                   
            </tr>        
            <tr>
                 <td class='row_title'>
                    <%: Html.LabelFor(model => model.ProductionCountryId)%>:                    
                </td>
                <td colspan="3">
                    <%: Html.DropDownListFor(model => model.ProductionCountryId, Model.ProductionCountryList, new { value = Model.ProductionCountryId }, !Model.AllowToEditRow)%>
                    <%if (Model.AllowToEditRow && Model.AllowToAddCountry)
                      { %>
                        &nbsp;&nbsp;<span class="edit_action" id="AddCountry">[ Добавить ]</span>
                    <%} %>
                    <%: Html.ValidationMessageFor(model => model.ProductionCountryId)%>
                    

                </td>       
            </tr>   
            </table>            
            
            <table class="display_table">
                <tr>
                    <td class='row_title'>
                        <%:Html.LabelFor(model => model.PackLength) %>:
                    </td>
                    <td>
                        <%: Html.TextBoxFor(x => x.PackHeight, new { style = "width:40px" }, !Model.AllowToEditRow)%>
                        &nbsp;x&nbsp;
                        <%: Html.TextBoxFor(x => x.PackWidth, new { style = "width:40px" }, !Model.AllowToEditRow)%>
                        &nbsp;x&nbsp;
                        <%: Html.TextBoxFor(x => x.PackLength, new { style = "width:40px" }, !Model.AllowToEditRow)%>
                        <%: Html.ValidationMessageFor(x => x.PackHeight) %>
                        <%: Html.ValidationMessageFor(x => x.PackWidth) %>
                        <%: Html.ValidationMessageFor(x => x.PackLength) %>
                    </td>
                    <td class="row_title">
                        <%:Html.LabelFor(model => model.PackSize) %>:
                    </td>
                    <td>
                        <span id="PackSize"><%: Model.PackSize %></span>&nbsp;<span id="MeasureUnitNameForPackSize"><%: Model.MeasureUnitName %></span>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%:Html.LabelFor(model => model.PackWeight) %>:
                    </td>
                    <td>
                        <%:Html.TextBoxFor(x => x.PackWeight, new { style = "width:128px" }, !Model.AllowToEditRow)%>&nbsp;кг
                        <%:Html.ValidationMessageFor(model => model.PackWeight)%>
                    </td>
                    <td class="row_title">
                        <%:Html.LabelFor(model => model.PackVolume)%>:                        
                    </td>
                    <td valign="bottom">
                        <%: Html.TextBoxFor(x => x.PackVolume, new { maxlength = 16 }, !Model.AllowToEditRow)%>&nbsp;м<sup>3</sup>
                        <%:Html.ValidationMessageFor(model => model.PackVolume)%>
                    </td>
                </tr>
            </table>

            <table class="editor_table">
                <tr>
                    <td class="row_title" >
                        <%:Html.LabelFor(model => model.ProductionCost)%>:
                    </td>
                    <td style="width:170px">
                        <%:Html.TextBoxFor(model => model.ProductionCost, new { style = "width:70px" }, !Model.AllowToEditRow)%>
                        <%:Html.ValidationMessageFor(model => model.ProductionCost) %>
                    </td>
                    <td class="row_title">
                        <%:Html.LabelFor(model => model.CurrencyName) %>:
                    </td>
                    <td>
                        <%: Model.CurrencyName %>
                    </td>
                </tr>
                 <tr>
                    <td class="row_title">
                        <%:Html.LabelFor(model => model.Count) %>:
                    </td>
                    <td>
                        <%:Html.TextBoxFor(model => model.Count, new { style = "width:70px" }, !Model.AllowToEditRow)%>&nbsp;
                        <span id="MeasureUnitName"><%: Model.MeasureUnitName %></span>
                        <%:Html.ValidationMessageFor(model => model.Count) %>
                    </td>
                    <td class="row_title">
                        <%:Html.LabelFor(model => model.CurrencyRate) %>:
                    </td>
                    <td>
                        <%: Model.CurrencyRate %>&nbsp;р.
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <% if (Model.AllowToEditRow) { %>или&nbsp;&nbsp;&nbsp;<% } %><%:Html.LabelFor(model => model.PackCount) %>:
                    </td>
                    <td>
                        <%:Html.TextBoxFor(model => model.PackCount, new { style = "width:70px" }, !Model.AllowToEditRow)%>
                        <%:Html.ValidationMessageFor(model => model.PackCount) %>
                    </td>
                    <td colspan="2">
                        <% if (Model.AllowToEditRow) { %><span class="select_link" id="calculateOptimalPlacement">Посчитать кол-во по размещению</span><% } %>
                    </td>                   
                </tr>
                <tr>
                    <td class="row_title">
                        <% if (Model.AllowToEditRow) { %>или&nbsp;&nbsp;&nbsp;<% } %><%:Html.LabelFor(model => model.TotalCost) %>:
                    </td>
                    <td>
                        <%:Html.TextBoxFor(model => model.TotalCost, new { style = "width:128px" }, !Model.AllowToEditRow)%>
                        <%:Html.ValidationMessageFor(model => model.TotalCost) %>
                    </td>                    
                </tr>
            </table>

            <table class="display_table">
                <tr>
                    <td class="row_title" valign="bottom">
                        <%:Html.LabelFor(model => model.TotalWeight) %>:
                    </td>
                    <td valign="bottom">
                        <span id="TotalWeight"><%:Model.TotalWeight %></span>&nbsp;кг
                    </td>
                    <td class="row_title" valign="bottom">
                        <%:Html.LabelFor(model => model.TotalVolume) %>:
                    </td>
                    <td valign="bottom">
                        <span id="TotalVolume"><%:Model.TotalVolume %></span>&nbsp;м<sup>3</sup>
                    </td>
                </tr>
                <tr>
                    <td class="row_title" valign="bottom">
                        <%:Html.LabelFor(model => model.BatchWeight) %>:
                    </td>
                    <td valign="bottom">
                        <span id="BatchWeight"><%:Model.BatchWeight %></span>&nbsp;кг
                    </td>
                    <td class="row_title" valign="bottom">
                        <%:Html.LabelFor(model => model.BatchVolume) %>:
                    </td>
                    <td valign="bottom">
                        <span id="BatchVolume"><%:Model.BatchVolume %></span>&nbsp;м<sup>3</sup>
                    </td>
                </tr>
                <tr>
                    <td class="row_title" valign="bottom">
                        <%:Html.LabelFor(model => model.OptimalPlacement) %>:
                    </td>
                    <td valign="bottom">
                        <span id="OptimalPlacement"><%:Model.OptimalPlacement %></span>
                    </td>
                    <td class="row_title" valign="bottom">
                        <%:Html.LabelFor(model => model.FreeVolume) %>:
                    </td>
                    <td valign="bottom">
                        <span id="FreeVolume"><%:Model.FreeVolume %></span>&nbsp;м<sup>3</sup>
                    </td>
                </tr>
            </table>            
    </div>
    
    <div class="button_set">
        <%=Html.SubmitButton("btnSaveProductionOrderBatchRow", "Сохранить", Model.AllowToEditRow, Model.AllowToEditRow)%>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%} %>

<div id="manufacturerSelector"></div>
<div id="articleSelector"></div>
<div id="countryAdd"></div>
