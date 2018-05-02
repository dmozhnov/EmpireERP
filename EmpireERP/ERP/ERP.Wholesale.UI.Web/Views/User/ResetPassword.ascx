<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.User.ResetPasswordViewModel>" %>

<% using (Ajax.BeginForm("PerformPasswordReset", "User", new AjaxOptions()
   {
       OnBegin = "User_MainDetails.OnBeginResetPassword", 
       OnFailure = "User_ResetPassword.OnFailResetPassword", OnSuccess = "User_MainDetails.OnSuccessResetPassword" }))%>
<%{ %>
    <%: Html.HiddenFor(x => x.Id) %>

    <div class="modal_title"><%:Model.Title%></div>
    <div class="h_delim"></div>
    
    <div style="padding: 10px 20px 5px">
        <div id="messageUserResetPassword"></div>
        
        <table class="editor_table">
            <tr>                
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
            <%: Html.SubmitButton("btnResetPassword", "Сохранить")%>            
            <input type="button" value="Закрыть" onclick="HideModal();"/>
        </div>
    </div>
<%} %>