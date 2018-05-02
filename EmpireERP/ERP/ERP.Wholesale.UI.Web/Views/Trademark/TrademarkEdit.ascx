<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.BaseDictionary.BaseDictionaryEditViewModel>" %>

<% using (Ajax.BeginForm("Save", "Trademark", new AjaxOptions()
   {
       OnBegin = "Trademark_Edit.OnBeginTrademarkSave",
       OnSuccess = "OnSuccessTrademarkSave",
       OnFailure = "Trademark_Edit.OnFailTrademarkSave"
   }))
{ %>
    <%: Html.HiddenFor(model => model.Id)%>
    
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Trademark_Edit_Trademark") %></div>
    <div class="h_delim"></div>       
    
    <div style="padding: 10px 20px 5px; max-width:500px;">
        <div id="messageTrademarkEdit"></div>

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
            <%: Html.SubmitButton("btnSaveTrademark", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>        
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>    
    </div>
<%} %>
