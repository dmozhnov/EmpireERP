<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.DealPaymentDocument.DealPaymentToClientEditViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type ="text/javascript">
    DealPaymentDocument_DealPaymentToClientEdit.Init();
</script>

<% using (Ajax.BeginForm(Model.ActionName, Model.ControllerName, new AjaxOptions()
   {
       OnBegin = "DealPaymentDocument_DealPaymentToClientEdit.OnBeginDealPaymentToClientSave",
       OnFailure = "DealPaymentDocument_DealPaymentToClientEdit.OnFailDealPaymentToClientSave",
       OnSuccess = "OnSuccessDealPaymentToClientSave"
   }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.IsDealSelectedByClient)%>
    
    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_DealPaymentToClient_Edit") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px; max-width: 499px;">
        <div id="messageDealPaymentToClientEdit"></div>

        <table class="editor_table">
            <% if (Model.AllowToViewClientOrganization) {%>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ClientOrganizationName)%>:</td>
                <td>
                    <%: Model.ClientOrganizationName%>
                    <%: Html.HiddenFor(model => model.ClientOrganizationId)%>
                    <%: Html.ValidationMessageFor(model => model.ClientOrganizationId)%>
                </td>
            </tr>
            <%} %>
            <% if (Model.AllowToViewClient) {%>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ClientName)%>:</td>
                <td>
                    <% if (Model.AllowToChooseClient) { %>
                        <span class="select_link" id="ClientName"><%:Model.ClientName%></span>
                    <%} else { %>
                        <%:Model.ClientName%>
                    <%} %>                    
                    <%: Html.HiddenFor(model => model.ClientId)%>
                    <%: Html.ValidationMessageFor(model => model.ClientId)%>
                </td>
            </tr>
            <% } %>
            <tr>                
                <td class="row_title"><%:Html.LabelFor(model => model.DealName)%>:</td>
                <td>
                    <% if (Model.AllowToChooseDeal) { %>
                        <span class="select_link" id="DealName"><%:Model.DealName%></span>
                    <%} else { %>
                        <%:Model.DealName%>
                    <%} %>
                    <%: Html.HiddenFor(model => model.DealId)%>
                    <%: Html.ValidationMessageFor(model => model.DealId)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.TeamId)%>:</td>
                <td>
                    <%: Html.DropDownListFor(x => x.TeamId, Model.TeamList, new { style = "min-width:160px;" })%>
                    <%: Html.ValidationMessageFor(model => model.TeamId)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ReturnedById)%>:</td>
                <td>
                    <%: Html.DropDownListFor(x => x.ReturnedById, Model.ReturnedByList, new { style = "min-width:160px;" })%>
                    <%: Html.ValidationMessageFor(model => model.ReturnedById)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PaymentDocumentNumber)%>:</td>
                <td>
                    <%:Html.TextBoxFor(model => model.PaymentDocumentNumber, new { maxlength = 50, size = 20 })%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.Date)%>:</td>
                <td>
                    <%=Html.DatePickerFor(model => model.Date, new { id = "DealPaymentToClientEdit_Date" }, isDisabled: !Model.AllowToChangeDate)%>
                    <%:Html.ValidationMessageFor(model => model.Date)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.Sum)%>:</td>
                <td>
                    <%:Html.TextBoxFor(model => model.Sum, new { maxlength = 19, size = 20 })%> р.
                    <%:Html.ValidationMessageFor(model => model.Sum)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.DealPaymentForm)%>:</td>
                <td>
                    <%:Html.DropDownListFor(model => model.DealPaymentForm, Model.DealPaymentFormList)%>
                    <%:Html.ValidationMessageFor(model => model.DealPaymentForm)%>
                </td>
            </tr>
        </table>

        <div class="button_set">
            <%= Html.SubmitButton("btnSave", "Сохранить")%>
            <input type="button" value="Закрыть" onclick="HideModal()" />     
        </div>
    </div>
<%} %>

<div id="clientSelector"></div>
<div id="dealSelector"></div>
