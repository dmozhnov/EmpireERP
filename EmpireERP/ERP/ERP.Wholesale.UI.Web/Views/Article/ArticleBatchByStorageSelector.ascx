<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Article.ArticleBatchSelectViewModel>" %>

<div style="width: 750px; padding: 0px 10px;">
    <br />    
    <span style="margin-left: 30px; font-size: 16px;"><span class="greytext">Выбран товар: </span> <%: Model.ArticleId %> &nbsp;|&nbsp; <%: Model.ArticleNumber %> &nbsp;|&nbsp; <%: Model.ArticleName %></span>
    <br /><br />

    <table class="display_table">
        <tr>
            <td class="row_title" style="width: 200px">
                <%: Html.LabelFor(model => model.SenderAccountingPrice) %>:
            </td>
            <td>
                <span id="SenderAccountingPrice"><%: Model.SenderAccountingPrice %></span> р.
            </td>            
            <td class="row_title">
                <%: Html.LabelFor(model => model.AvailableToMoveTotalCount)%>:
            </td>
            <td>
                <span id="AvailableToMoveTotalCount"><%: Model.AvailableToMoveTotalCount%></span>
            </td>
        </tr>
    </table>

    <br />

    <% Html.RenderPartial("ArticleBatchGrid", Model.BatchGrid); %>
                        
    <div class="button_set">            
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>


