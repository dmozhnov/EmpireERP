<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Organization.ForeignBankAccountEditViewModel>" %>

<script type="text/javascript">
    Organization_ForeignBankAccountEdit.Init();

    function OnBeginForeignBankAccountEdit() {
        StartButtonProgress($("#btnSaveForeignBankAccount"));
    }
</script>

<% using (Ajax.BeginForm(Model.ActionName, Model.ControllerName, new AjaxOptions() { OnBegin = "OnBeginForeignBankAccountEdit",
       OnFailure = "Organization_ForeignBankAccountEdit.OnFailForeignBankAccountEdit", OnSuccess = Model.SuccessFunctionName }))%>
<%{ %>
    <%:Html.HiddenFor(x => x.BankAccountId) %>
    <%:Html.HiddenFor(x => x.OrganizationId) %>
        
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Organization_Edit_ForeignBankAccount") %></div>
    <div class="h_delim"></div>
    
    <div id="foreignBankAccountEdit" style="padding: 10px 10px 5px">
        <div id='messageForeignBankAccountEdit'></div>

        <table class='editor_table'>            
            <tr>
                <td class='row_title'><%:Html.LabelFor(x => x.BankAccountNumber)%>:</td>
                <td>
                    <%:Html.TextBoxFor(x => x.BankAccountNumber, new { maxlength = 20, size = 60 }, !Model.AllowToEdit)%>
                    <%:Html.ValidationMessageFor(x => x.BankAccountNumber)%>
                </td>
            </tr>
            <tr>
                <td class='row_title'><%:Html.LabelFor(x => x.CurrencyId)%>:</td>
                <td>
                    <%:Html.DropDownListFor(model => model.CurrencyId, Model.CurrencyList, !Model.AllowToEdit)%>
                    <%--&nbsp;&nbsp;&nbsp;&nbsp;<span class="edit_action" id="addCurrency">[ Добавить ]</span>  --%>
                    <%:Html.ValidationMessageFor(x => x.CurrencyId)%>
                </td>
            </tr>
            <tr>
                <td class='row_title'><%:Html.LabelFor(x=>x.IBAN) %>:</td>
                <td>
                    <%:Html.TextBoxFor(x => x.IBAN, new { maxlength = 34, size = 60 }, !Model.AllowToEdit)%>
                    <%:Html.ValidationMessageFor(x => x.IBAN)%>
                </td>
            </tr>
            <tr>
                <td class='row_title'><%:Html.LabelFor(x=>x.SWIFT) %>:</td>
                <td>
                    <%:Html.TextBoxFor(x => x.SWIFT, new { maxlength = 11, size = 60 }, !Model.AllowToEdit)%>
                    <%:Html.ValidationMessageFor(x => x.SWIFT)%>
                </td>
            </tr>
             <tr>
                <td class='row_title'><%:Html.LabelFor(x=>x.BankName) %>:</td>
                <td>
                    <span id="BankName"><%: Model.BankName %></span>
                </td>
            </tr>
            <tr>
                <td class='row_title'><%:Html.LabelFor(x=>x.BankAddress) %>:</td>
                <td>
                    <span id="BankAddress"><%: Model.BankAddress %></span>
                </td>
            </tr>
            <tr>
                <td class='row_title'><%:Html.LabelFor(x => x.ClearingCode)%>:</td>
                <td>
                    <span id="ClearingCodeType"><%: Model.ClearingCodeType %></span>&nbsp;&nbsp;&nbsp;&nbsp;
                    <span id="ClearingCode"><%: Model.ClearingCode %></span>
                </td>
            </tr>
            <tr>
                <td class='row_title'><%:Html.LabelFor(x => x.IsMaster)%>:</td>
                <td>
                    <% if (Model.IsMaster == "1" && Model.BankAccountId != 0)
                       { %>
                        Да
                        <input id="IsMaster" name="IsMaster" type="hidden" value="1" />
                    <%} else { %>
                        <%:Html.YesNoToggleFor(model => model.IsMaster, Model.AllowToEdit)%>
                    <%} %>
                </td>
            </tr>
        </table>

        <div class='button_set'>
            <%: Html.SubmitButton("btnSaveForeignBankAccount", "Сохранить", Model.BankAccountId != 0, Model.AllowToEdit)%>            
            <input type="button" value="Закрыть" onclick="HideModal()" />     
        </div>
    </div>
<%} %>