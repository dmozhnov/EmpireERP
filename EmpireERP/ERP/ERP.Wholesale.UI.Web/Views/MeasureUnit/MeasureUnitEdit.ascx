<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.MeasureUnit.MeasureUnitEditViewModel>" %>

<% using (Ajax.BeginForm("Save", "MeasureUnit", new AjaxOptions() { OnBegin = "MeasureUnit_Edit.OnBeginMeasureUnitSave",
       OnSuccess = "OnSuccessMeasureUnitSave", OnFailure = "MeasureUnit_Edit.OnFailMeasureUnitSave" }))
{ %>
    <%: Html.HiddenFor(model => model.Id)%>
    
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_MeasureUnit_Edit_MeasureUnit") %></div>
    <div class="h_delim"></div>       
    
    <div style="padding: 10px 20px 5px">
        <div id="messageMeasureUnitEdit"></div>

        <table class='editor_table'>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.ShortName)%>:
                </td>
                <td style="min-width: 70px;" colspan="3">
                    <%: Html.TextBoxFor(model => model.ShortName, new { style = "width:200px", maxlength = "7" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.ShortName)%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.FullName)%>:
                </td>
                <td  colspan="3">
                    <%: Html.TextBoxFor(model => model.FullName, new { style = "width:200px", maxlength = "20" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.FullName)%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                </td>
                <td style="max-width: 200px"  colspan="3">
                        <%: Html.CommentFor(model => model.Comment, new { style = "width:200px" }, !Model.AllowToEdit, rowsCount: 4)%>
                        <%: Html.ValidationMessageFor(model => model.Comment)%>                    
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.Scale)%>:
                    <%: Html.ValidationMessageFor(model => model.Scale)%>
                </td>
                <td>
                    <%: Html.TextBoxFor(model => model.Scale, new { style = "width:20px", maxlength = "2" }, !Model.AllowToEdit)%>                    
                </td>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.NumericCode)%>:
                    <%: Html.ValidationMessageFor(model => model.NumericCode)%>
                </td>
                <td>
                    <%: Html.TextBoxFor(model => model.NumericCode, new { style = "width:30px", maxlength = "3" }, !Model.AllowToEdit)%>                    
                </td>
            </tr>             
        </table>    
    
        <br />

        <div class="button_set">
            <%: Html.SubmitButton("btnSaveMeasureUnit", "Сохранить", Model.AllowToEdit, Model.AllowToEdit) %>        
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>    
    </div>
<%} %>
