<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Bank.RussianBankEditViewModel>" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#EditRussianBank #Name").focus();
    });

    function OnFailRussianBankEdit(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageRussianBankEdit");
    }

    function OnBeginRussianBankEdit() {
        StartButtonProgress($("#btnSaveRussianBank"));
    }
</script>

<% using (Ajax.BeginForm("EditRussianBank", "Bank", new AjaxOptions() { OnBegin = "OnBeginRussianBankEdit",
       OnSuccess = "OnSuccessRussianBankEdit", OnFailure = "OnFailRussianBankEdit" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.Id) %>

    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Bank_Edit_RussianBank") %></div>
    <div class="h_delim"></div>
        
    <div id="EditRussianBank" style="padding: 10px 10px 5px">
        <div id="messageRussianBankEdit"></div>  

        <table class='editor_table'>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.Name) %>:
                </td>
                <td>
                   <%: Html.TextBoxFor(model => model.Name, new { style = "width: 320px;", maxlength = "250" }, !Model.AllowToEdit)%>
                   <%: Html.ValidationMessageFor(model => model.Name) %>
                </td>                   
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.Address) %>:
                </td>
                <td>
                   <%: Html.TextBoxFor(model => model.Address, new { style = "width: 320px;", maxlength = "250" }, !Model.AllowToEdit)%>
                   <%: Html.ValidationMessageFor(model => model.Address) %>
                </td>                   
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.BIC) %>:
                </td>
                <td>
                   <%: Html.TextBoxFor(model => model.BIC, new { style = "width: 320px;", maxlength = "9" }, !Model.AllowToEdit)%>
                   <%: Html.ValidationMessageFor(model => model.BIC) %>
                </td>                   
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.CorAccount) %>:
                </td>
                <td>
                   <%: Html.TextBoxFor(model => model.CorAccount, new { style = "width: 320px;", maxlength = "20" }, !Model.AllowToEdit)%>
                   <%: Html.ValidationMessageFor(model => model.CorAccount) %>
                </td>                   
            </tr>
        </table>

        <div class="button_set">
            <%: Html.SubmitButton("btnSaveRussianBank", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>

    </div>
<%} %>






