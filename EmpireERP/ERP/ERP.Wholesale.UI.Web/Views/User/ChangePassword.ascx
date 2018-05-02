﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.User.ChangePasswordViewModel>" %>

<% using (Ajax.BeginForm("PerformPasswordChange", "User", new AjaxOptions() { OnFailure = "User_ChangePassword.OnFailChangePassword", OnSuccess = "User_MainDetails.OnSuccessChangePassword" }))%>
<%{ %>
    <%: Html.HiddenFor(x => x.Id) %>

    <div class="modal_title"><%:Model.Title%></div>
    <div class="h_delim"></div>
    
    <div style="padding: 10px 20px 5px">
        <div id="messageUserChangePassword"></div>
        
        <table class="editor_table">
            <tr>
                <td class="row_title">
                    <%:Html.LabelFor(x => x.CurrentPassword) %>:
                </td>
                <td>
                    <%: Html.PasswordFor(x => x.CurrentPassword, new { style = "width:150px", maxlength = "20" })%>
                    <%: Html.ValidationMessageFor(x => x.CurrentPassword)%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%:Html.LabelFor(x => x.NewPassword) %>:
                </td>
                <td>
                    <%: Html.PasswordFor(x => x.NewPassword, new { style = "width:150px", maxlength = "20" })%>
                    <%: Html.ValidationMessageFor(x => x.NewPassword) %>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%:Html.LabelFor(x => x.NewPasswordConfirmation) %>:
                </td>
                <td>
                    <%: Html.PasswordFor(x => x.NewPasswordConfirmation, new { style = "width:150px", maxlength = "20" })%>
                    <%: Html.ValidationMessageFor(x => x.NewPasswordConfirmation)%>
                </td>
            </tr>
        </table>

        <div class='button_set'>
            <%: Html.SubmitButton("Сохранить") %>            
            <input type="button" value="Закрыть" onclick="HideModal();"/>
        </div>
    </div>
<%} %>