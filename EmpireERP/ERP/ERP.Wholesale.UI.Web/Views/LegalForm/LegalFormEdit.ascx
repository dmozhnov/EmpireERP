<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.LegalForm.LegalFormEditViewModel>" %>

<% using (Ajax.BeginForm("Save", "LegalForm", new AjaxOptions()
   {
       OnBegin = "LegalForm_Edit.OnBeginLegalFormSave",
       OnSuccess = "OnSuccessLegalFormSave",
       OnFailure = "LegalForm_Edit.OnFailLegalFormSave"
   }))
{ %>
    <%: Html.HiddenFor(model => model.Id)%>
    
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_LegalForm_Edit_LegalForm") %></div>
    <div class="h_delim"></div>       
    
    <div style="padding: 10px 20px 5px; max-width:500px;">
        <div id="messageLegalFormEdit"></div>

        <table class='editor_table'>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.Name)%>:
                </td>
                <td style="min-width: 70px;">
                    <%: Html.TextBoxFor(model => model.Name, new { style = "width:150px", maxlength = "15" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.Name)%>
                </td>
            </tr>
            <tr>
            <td class="row_title">
                    <%: Html.LabelFor(model => model.EconomicAgentType)%>:
                </td>
                <td style="min-width: 70px;">
                    <%: Html.DropDownListFor(model => model.EconomicAgentType, Model.EconomicAgentTypeList, new { id = "economicAgentTypeList" }, !Model.AllowToEditEconomicAgentType)%>
                    <%:Html.ValidationMessageFor(model => model.EconomicAgentType)%>
                </td>
            </tr>
        </table>    
    
        <div class="button_set">
            <%: Html.SubmitButton("btnSaveLegalForm", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>        
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>    
    </div>
<%} %>
