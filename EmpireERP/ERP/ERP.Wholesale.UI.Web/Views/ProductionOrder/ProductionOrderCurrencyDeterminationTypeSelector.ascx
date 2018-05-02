<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderCurrencyDeterminationTypeSelectorViewModel>" %>

<% using (Ajax.BeginForm(Model.ActionName, "ProductionOrder", new AjaxOptions() { OnBegin = "OnBeginProductionOrderCurrencyDeterminationTypeSelect", 
       OnFailure = "OnFailProductionOrderCurrencyDeterminationTypeSelect", OnSuccess = "OnSuccessProductionOrderCurrencyDeterminationTypeSelect" }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.ProductionOrderId)%>
    <%:Html.HiddenFor(model => model.ProductionOrderCurrencyDocumentType)%>
    
    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrder_Select_ProductionOrderCurrencyDeterminationType") %></div>
    <div class="h_delim"></div>       

    <div style="padding: 10px 10px 5px">
        <div id="messageProductionOrderCurrencyDeterminationTypeSelect"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title" style="min-width: 155px"><%:Html.LabelFor(model => model.ProductionOrderCurrencyDeterminationType)%>:</td>
                <td style="min-width: 255px">
                    <%:Html.DropDownListFor(model => model.ProductionOrderCurrencyDeterminationType, Model.ProductionOrderCurrencyDeterminationTypeList)%>
                    <%:Html.ValidationMessageFor(model => model.ProductionOrderCurrencyDeterminationType)%>
                </td>
            </tr>
        </table>

        <div class="button_set">
            <%= Html.SubmitButton("btnProductionOrderCurrencyDeterminationTypeSelect", "Перейти к вводу параметров")%>
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>
<%} %>
