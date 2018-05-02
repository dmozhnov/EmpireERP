<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.BaseDictionary.BaseDictionaryEditViewModel>" %>

<% using (Ajax.BeginForm("Save", "ReturnFromClientReason", new AjaxOptions()
   {
       OnBegin = "ReturnFromClientReason_Edit.OnBeginReturnFromClientReasonSave",
       OnSuccess = "OnSuccessReturnFromClientReasonSave",
       OnFailure = "ReturnFromClientReason_Edit.OnFailReturnFromClientReasonSave"
   }))
{ %>
    <%: Html.HiddenFor(model => model.Id)%>
    
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ReturnFromClientReason_Edit") %></div>
    <div class="h_delim"></div>       
    
    <div style="padding: 10px 20px 5px; max-width:500px;">
        <div id="messageReturnFromClientReasonEdit"></div>

        <table class='editor_table'>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.Name)%>:
                </td>
                <td style="min-width: 70px;">
                    <%: Html.TextBoxFor(model => model.Name, new { style = "width:350px", maxlength = "200" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.Name)%>
                </td>
            </tr>
        </table>    
    
        <div class="button_set">
            <%: Html.SubmitButton("btnSaveReturnFromClientReason", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>        
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>    
    </div>
<%} %>
