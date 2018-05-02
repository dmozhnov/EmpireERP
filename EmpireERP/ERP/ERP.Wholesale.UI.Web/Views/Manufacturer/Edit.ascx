<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Manufacturer.ManufacturerEditViewModel>" %>

<script type="text/javascript">
    function OnFailManufacturerSave(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageManufacturerEdit");
    }

    function OnBeginManufacturerSave() {
        StartButtonProgress($("#btnSaveManufacturer"));
    }
</script>

<% using (Ajax.BeginForm("Edit", "Manufacturer", new AjaxOptions() { OnFailure = "OnFailManufacturerSave", OnSuccess = "OnSuccessManufacturerSave", OnBegin = "OnBeginManufacturerSave" }))%>
<%{ %>
    <%:Html.HiddenFor(x => x.Id) %>
    <%:Html.HiddenFor(x => x.ProducerId) %>

    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Manufacturer_Edit_Manufacturer") %></div>
    <div class="h_delim"></div>
    
    <div id="AddManufacturer" style="padding: 10px 20px 5px;">
        <div id="messageManufacturerEdit"></div>
        <table class="editor_table">
            <tr>
                <td class="row_title">
                    <%:Html.LabelFor(x=>x.Name) %>:
                </td>
                <td>
                    <%: Html.TextBoxFor(x=>x.Name, new { maxlength=200, size = 50}, !Model.AllowToEdit) %>
                    <%: Html.ValidationMessageFor(x => x.Name) %>
                </td>
            </tr>
        </table>

        <div class='button_set'>
            <%: Html.SubmitButton("btnSaveManufacturer", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>        
            <input type="button" value="Закрыть" onclick="HideModal();"/>
        </div>
    </div>
<%} %>
