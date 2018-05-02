<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ArticleGroup.ArticleGroupEditViewModel>" %>

<script type="text/javascript">
    ArticleGroup_DetailsForEdit.Init();
    
    function OnFailArticleGroupEdit(ajaxContext) {
        ArticleGroup_DetailsForEdit.OnFailArticleGroupEdit(ajaxContext);
    }

    function OnBeginArticleGroupEdit() {
        StartButtonProgress($("#btnSaveArticleGroup"));
    }    
</script>

<% using (Ajax.BeginForm("Edit", "ArticleGroup", new AjaxOptions() { OnSuccess = "OnSuccessArticleGroupEdit", OnFailure = "OnFailArticleGroupEdit",
   OnBegin = "OnBeginArticleGroupEdit" })) 
{ %>      
    <%: Html.HiddenFor(model => model.Id)%>
    <%: Html.HiddenFor(model => model.ParentArticleGroupId)%>
    
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ArticleGroup_Edit_ArticleGroup") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">
        <div id="messageArticleGroupEdit"></div>                               
                
        <table class="editor_table">
            <tr>
                <td class="row_title">                    
                    <%: Html.LabelFor(model => model.Name)%>:
                </td>
                <td>
                    <%: Html.TextBoxFor(model => model.Name, new { id="Name", style = "width: 400px;" }, !Model.AllowToEdit )%>
                    <%: Html.ValidationMessageFor(model => model.Name)%>
                </td>                    
            </tr>
            <tr>
                <td class='row_title'><%: Html.HelpLabelFor(model => model.NameFor1C, "/Help/GetHelp_ArticleGroup_Edit_NameFor1C")%>:</td>
                <td>
                    <%: Html.TextBoxFor(model => model.NameFor1C, new { id="NameFor1C", style = "width: 400px;" }, !Model.AllowToEdit )%>
                    <%: Html.ValidationMessageFor(model => model.NameFor1C)%>
                </td>                    
            </tr>    
            <tr>
                <td class="row_title"><%: Html.LabelFor(model => model.MarkupPercent)%>:</td>
                <td>
                    <%: Html.TextBoxFor(model => model.MarkupPercent, new { style = "width: 50px;", maxlength = "8" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.MarkupPercent)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%: Html.LabelFor(model => model.SalaryPercent)%>:</td>
                <td>
                    <%: Html.TextBoxFor(model => model.SalaryPercent, new { style = "width: 50px;", maxlength = "8" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.SalaryPercent)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:</td>
                <td>
                    <%: Html.CommentFor(model => model.Comment, new { style = "width: 400px; height: 100px;" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.Comment)%>
                </td>
            </tr>
        </table>
    </div>
        
    <div class="button_set">
        <%: Html.SubmitButton("btnSaveArticleGroup", "Сохранить", Model.AllowToEdit, Model.AllowToEdit) %>      
        <input type="button" value="Закрыть" onclick="HideModal()"/>
    </div>    
<%} %>  

