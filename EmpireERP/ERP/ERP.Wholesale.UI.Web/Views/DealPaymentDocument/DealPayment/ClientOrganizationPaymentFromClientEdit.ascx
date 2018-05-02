<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.DealPaymentDocument.ClientOrganizationPaymentFromClientEditViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type="text/javascript">
    DealPaymentDocument_ClientOrganizationPaymentFromClientEdit.Init();
</script>

<% using (Ajax.BeginForm("SelectDestinationDocumentsForClientOrganizationPaymentFromClientDistribution", "DealPayment", new AjaxOptions()
   {
       OnBegin = "DealPaymentDocument_ClientOrganizationPaymentFromClientEdit.OnBeginSelectDestinationDocumentsButtonClick",
       OnFailure = "DealPaymentDocument_ClientOrganizationPaymentFromClientEdit.OnFailSelectDestinationDocumentsButtonClick",
       OnSuccess = "DealPaymentDocument_ClientOrganizationPaymentFromClientEdit.OnSuccessSelectDestinationDocumentsButtonClick"
   }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.DestinationDocumentSelectorControllerName)%>
    <%:Html.HiddenFor(model => model.DestinationDocumentSelectorActionName)%>
    <%:Html.HiddenFor(model => model.ClientOrganizationId)%>
    
    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_DealPaymentFromClient_Edit") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">
        <div id="messageClientOrganizationPaymentFromClientEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ClientOrganizationName)%>:</td>
                <td>
                    <% if (String.IsNullOrEmpty(Model.ClientOrganizationId)) {%>
                        <span class="select_link" id="ClientOrganizationName"><%: Model.ClientOrganizationName%></span>
                        <%: Html.ValidationMessageFor(model => model.ClientOrganizationId)%>
                    <%} else {%>
                        <%: Model.ClientOrganizationName%>
                    <%} %>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PaymentDocumentNumber)%>:</td>
                <td>
                    <%:Html.TextBoxFor(model => model.PaymentDocumentNumber, new { maxlength = 50, size = 50 })%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.Date)%>:</td>
                <td>
                    <%=Html.DatePickerFor(model => model.Date, new { id = "ClientOrganizationPaymentFromClientEdit_Date" }, isDisabled: !Model.AllowToChangeDate)%>
                    <%:Html.ValidationMessageFor(model => model.Date)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.Sum)%>:</td>
                <td>
                    <%:Html.TextBoxFor(model => model.Sum, new { maxlength = 19, size = 20 })%>
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
            <%: Html.SubmitButton("btnSelectDestinationDocuments", "Начать разнесение")%>
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>
<%} %>

<div id="contractorOrganizationSelector"></div>
