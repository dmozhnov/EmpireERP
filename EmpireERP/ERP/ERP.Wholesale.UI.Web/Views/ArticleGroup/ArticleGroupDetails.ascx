<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ArticleGroup.ArticleGroupDetailsViewModel>" %>

<div style="width: 500px;">
    <%: Html.HiddenFor(model => model.Id) %>
    
    <div class="modal_title">Детали группы товаров<%: Html.Help("/Help/GetHelp_ArticleGroup_Details_ArticleGroup") %></div>
    
    <div style="padding: 10px 10px 5px">
        <table class="display_table">
            <tr>
                <td class="row_title" style="width: 100px">
                    <%: Html.LabelFor(model => model.Name) %>:
                </td>
                <td><%: Model.Name %></td>
            </tr>
             <tr>
                <td class="row_title" style="width: 100px">
                    <%: Html.LabelFor(model => model.NameFor1C) %>:
                </td>
                <td><%: Model.NameFor1C %></td>
            </tr>
            <tr>
                <td class="row_title"><%: Html.LabelFor(model => model.MarkupPercent)%>:</td>
                <td><%: Model.MarkupPercent%></td>
            </tr>
            <tr>
                <td class="row_title"><%: Html.LabelFor(model => model.SalaryPercent)%>:</td>
                <td><%: Model.SalaryPercent %></td>
            </tr>
       </table>

        <div style="font-size: 11px; padding-top: 8px;">            
            <%: Html.CommentFor(x => x.Comment, true) %>          
        </div>
    </div>

    <div class="button_set">
        <%= Html.Button("btnCreateSubGroup", "Новая подгруппа", Model.AllowToCreate, Model.AllowToCreate)%>
        <%= Html.Button("btnEditArticleGroup", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnDeleteArticleGroup", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <input type="button" value="Закрыть" onclick="HidePopup()" />
    </div>
</div>


