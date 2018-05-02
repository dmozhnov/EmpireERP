<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ValueAddedTax.ValueAddedTaxEditViewModel>" %>

<% using (Ajax.BeginForm("Save", "ValueAddedTax", new AjaxOptions()
   {
       OnBegin = "ValueAddedTax_Edit.OnBeginValueAddedTaxSave",
       OnSuccess = "OnSuccessValueAddedTaxSave",
       OnFailure = "ValueAddedTax_Edit.OnFailValueAddedTaxSave"
   }))
{ %>
    <%: Html.HiddenFor(model => model.Id)%>
    
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ValueAddedTax_Edit_ValueAddedTax") %></div>
    <div class="h_delim"></div>       
    
    <div style="padding: 10px 20px 5px; max-width:500px;">
        <div id="messageValueAddedTaxEdit"></div>

        <table class='editor_table'>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.Name)%>:
                </td>
                <td style="min-width: 70px;" colspan="3">
                    <%: Html.TextBoxFor(model => model.Name, new { style = "width:350px", maxlength = "100" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.Name)%>
                </td>
            </tr>
            <tr>
            <td class="row_title">
                    <%: Html.LabelFor(model => model.Value)%>:
                </td>
                <td style="min-width: 70px;">
                    <%: Html.TextBoxFor(model => model.Value, new { style = "width:70px", maxlength = "6" }, !(Model.AllowToEdit && Model.AllowToEditValue))%>
                    <%: Html.ValidationMessageFor(model => model.Value)%>
                </td>
            
            <td class="row_title">
                    <%: Html.LabelFor(model => model.IsDefault)%>:
                </td>
                <td style="min-width: 70px;">
                    <%: Html.YesNoToggleFor(model => model.IsDefault, Model.AllowToEdit && Model.IsDefault == "0")%>
                    <%: Html.ValidationMessageFor(model => model.IsDefault)%>
                </td>
            </tr>
        </table>    
    
        <div class="button_set">
            <%: Html.SubmitButton("btnSaveValueAddedTax", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>        
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>    
    </div>
<%} %>
