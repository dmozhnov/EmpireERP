<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Organization.RussianBankAccountEditViewModel>" %>

<script type="text/javascript">
    Organization_RussianBankAccountEdit.Init();

    function OnFailRussianBankAccountEdit(ajaxContext) {
        Organization_RussianBankAccountEdit.OnFailBankAccountEdit(ajaxContext);
    }

    function OnBeginRussianBankAccountEdit() {
        StartButtonProgress($("#btnSaveRussianBankAccount"));
    }
</script>

<% using (Ajax.BeginForm(Model.ActionName, Model.ControllerName, new AjaxOptions() { OnBegin = "OnBeginRussianBankAccountEdit",
       OnFailure = "OnFailRussianBankAccountEdit", OnSuccess = Model.SuccessFunctionName }))%>
<%{ %>
    <%:Html.HiddenFor(x => x.BankAccountId)%>
    <%:Html.HiddenFor(x => x.OrganizationId) %>
        
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Organization_Edit_RussianBankAccount") %></div>    
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">
        <div id='messageRussianBankAccountEdit'></div>

        <table class='editor_table'>
            <tr>
                <td class='row_title' style="width: 120px"><%:Html.LabelFor(x => x.BankAccountNumber)%>:</td>
                <td>
                    <%:Html.TextBoxFor(x => x.BankAccountNumber, new { maxlength = 20, size = 40}, !Model.AllowToEdit)%>
                    <%:Html.ValidationMessageFor(x => x.BankAccountNumber)%>
                </td>
            </tr>
            <tr>
                <td class='row_title'><%:Html.LabelFor(x => x.CurrencyId)%>:</td>
                <td>
                    <%:Html.DropDownListFor(model => model.CurrencyId, Model.CurrencyList, !Model.AllowToEdit)%>                    
                    <%:Html.ValidationMessageFor(x => x.CurrencyId)%>
                </td>
            </tr>            
            <tr>
                <td class='row_title'><%:Html.LabelFor(x => x.BIC) %>:</td>
                <td>
                    <%:Html.TextBoxFor(x => x.BIC, new { maxlength = 9, size = 40 }, !Model.AllowToEdit)%>
                    <%:Html.ValidationMessageFor(x => x.BIC) %>
                </td>
            </tr>
            <tr>
                <td class='row_title'><%:Html.LabelFor(x => x.BankName) %>:</td>
                <td>
                    <span id='BankName'><%:Model.BankName %></span>
                </td>
            </tr>
            <tr>
                <td class='row_title'><%:Html.LabelFor(x =>x .CorAccount) %>:</td>
                <td>
                    <span id='CorAccount'><%:Model.CorAccount %></span>
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
            <%: Html.SubmitButton("btnSaveRussianBankAccount", "Сохранить", Model.BankAccountId != 0, Model.AllowToEdit)%>
            <input type="button" value="Закрыть" onclick="HideModal()" />     
        </div>
    </div>
<%} %>