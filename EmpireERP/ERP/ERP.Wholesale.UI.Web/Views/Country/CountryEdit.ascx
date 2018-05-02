<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Country.CountryEditViewModel>" %>

<% using (Ajax.BeginForm("Save", "Country", new AjaxOptions()
   {
       OnBegin = "Country_Edit.OnBeginCountrySave",
       OnSuccess = "OnSuccessCountrySave",
       OnFailure = "Country_Edit.OnFailCountrySave"
   }))
{ %>
    <%: Html.HiddenFor(model => model.Id)%>
    
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Country_Edit_Country") %></div>
    <div class="h_delim"></div>       
    
    <div style="padding: 10px 10px 5px; max-width:500px;">
        <div id="messageCountryEdit"></div>

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
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.NumericCode)%>:
                </td>
                <td style="min-width: 70px;">
                    <%: Html.TextBoxFor(model => model.NumericCode, new { style = "width:30px", maxlength = "3" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.NumericCode)%>
                </td>
            </tr>
        </table>    

        <div class="button_set">
            <%: Html.SubmitButton("btnSaveCountry", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>        
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>    
    </div>
<%} %>
