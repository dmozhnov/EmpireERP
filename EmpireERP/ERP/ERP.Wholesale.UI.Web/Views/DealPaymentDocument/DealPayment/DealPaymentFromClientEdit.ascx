<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.DealPaymentDocument.DealPaymentFromClientEditViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type="text/javascript">
    DealPaymentDocument_DealPaymentFromClientEdit.Init();
</script>

<% using (Ajax.BeginForm("SelectDestinationDocumentsForDealPaymentFromClientDistribution", "DealPayment", new AjaxOptions()
   {
       OnBegin = "DealPaymentDocument_DealPaymentFromClientEdit.OnBeginSelectDestinationDocumentsButtonClick",
       OnFailure = "DealPaymentDocument_DealPaymentFromClientEdit.OnFailSelectDestinationDocumentsButtonClick",
       OnSuccess = "DealPaymentDocument_DealPaymentFromClientEdit.OnSuccessSelectDestinationDocumentsButtonClick"
   }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.DestinationDocumentSelectorControllerName)%>
    <%:Html.HiddenFor(model => model.DestinationDocumentSelectorActionName)%>
    <%:Html.HiddenFor(model => model.DealId)%>
    <%:Html.HiddenFor(model => model.ClientId)%>
    <%:Html.HiddenFor(model => model.MaxCashPaymentSum) %>

    <div class="modal_title"><%: Model.Title%><%: Html.Help("/Help/GetHelp_DealPaymentFromClient_Edit") %></div>
    <div class="h_delim"></div>
    
    <div style="padding: 10px 10px 5px">
        <div id="messageDealPaymentFromClientEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title"><%: Html.LabelFor(model => model.ClientName)%>:</td>
                <td>
                    <% if (String.IsNullOrEmpty(Model.ClientId)) {%>
                        <span class="select_link" id="ClientName"><%: Model.ClientName%></span>
                        <%: Html.ValidationMessageFor(model => model.ClientId)%>
                    <%} else {%>
                        <%: Model.ClientName%>
                    <%} %>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%: Html.LabelFor(model => model.DealName)%>:</td>
                <td>
                    <% if (String.IsNullOrEmpty(Model.DealId)) {%>
                        <span class="select_link no_auto_progress" id="DealName"><%: Model.DealName%></span>
                        <%: Html.ValidationMessageFor(model => model.DealId)%>
                    <%} else {%>
                        <%: Model.DealName%>
                    <%} %>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%: Html.LabelFor(model => model.PaymentDocumentNumber)%>:</td>
                <td>
                    <%: Html.TextBoxFor(model => model.PaymentDocumentNumber, new { maxlength = 50, size = 20 })%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%: Html.LabelFor(model => model.Date)%>:</td>
                <td>
                    <%= Html.DatePickerFor(model => model.Date, new { id = "DealPaymentFromClientEdit_Date" }, isDisabled: !Model.AllowToChangeDate)%>
                    <%: Html.ValidationMessageFor(model => model.Date)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.Sum)%>:</td>
                <td>
                    <%: Html.TextBoxFor(model => model.Sum, new { maxlength = 19, size = 20 })%> р.
                    <%: Html.ValidationMessageFor(model => model.Sum)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.DealPaymentForm)%>:</td>
                <td>
                    <%: Html.DropDownListFor(model => model.DealPaymentForm, Model.DealPaymentFormList)%>
                    <%: Html.ValidationMessageFor(model => model.DealPaymentForm)%>
                </td>
            </tr>
        </table>

        <div class="button_set">
            <%: Html.SubmitButton("btnSelectDestinationDocuments", "Начать разнесение")%>
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>        
<%} %>

<div id="clientSelector"></div>
<div id="dealSelector"></div>
