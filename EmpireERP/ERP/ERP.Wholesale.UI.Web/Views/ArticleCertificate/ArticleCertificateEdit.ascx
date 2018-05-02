<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ArticleCertificate.ArticleCertificateEditViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type="text/javascript">
    function OnBeginArticleCertificateSave() {
        StartButtonProgress($("#btnSaveArticleCertificate"));
    }
</script>

<div style="width: 600px;">

<% using (Ajax.BeginForm("Save", "ArticleCertificate", new AjaxOptions() { OnBegin = "OnBeginArticleCertificateSave",
       OnSuccess = "OnSuccessArticleCertificateSave", OnFailure = "ArticleCertificate_Edit.OnFailArticleCertificateSave" }))
{ %>
    <%: Html.HiddenFor(model => model.Id)%>
    
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ArticleCertificate_Edit_ArticleCertificate") %></div>
    <div class="h_delim"></div>       
    
    <div style="padding: 10px 20px 5px">
        <div id="messageArticleCertificateEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title" valign="top" style="width: 65px;">
                    <%: Html.LabelFor(model => model.Name)%>:
                </td>
                <td style="width: 475px;">
                    <%: Html.TextAreaFor(model => model.Name, new { maxlength = "500", rows = 5, style = "width: 465px;" }, !Model.AllowToEdit)%> 
                    <%: Html.ValidationMessageFor(model => model.Name) %>                   
                </td>
            </tr>
        </table>
        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 135px;">
                    <%: Html.LabelFor(model => model.StartDate)%>:
                </td>
                <td align="left" style="width: 90px;">
                    <%= Html.DatePickerFor(model => model.StartDate, new { id = "ArticleCertificateEdit_StartDate" }, isDisabled: !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.StartDate)%>
                </td>
                <td class="row_title" style="width: 205px;">
                    <%: Html.LabelFor(model => model.EndDate)%>:
                </td>
                <td align="left" style="width: 90px;">
                    <%= Html.DatePickerFor(model => model.EndDate, new { id = "ArticleCertificateEdit_EndDate" }, isDisabled: !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.EndDate)%>
                </td>
            </tr>
        </table>
    
        <br />

        <div class="button_set">
            <%: Html.SubmitButton("btnSaveArticleCertificate", "Сохранить", Model.AllowToEdit, Model.AllowToEdit) %>
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>    
    </div>
<%} %>

</div>
