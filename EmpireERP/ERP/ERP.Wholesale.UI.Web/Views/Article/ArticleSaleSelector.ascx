<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Article.ArticleSaleSelectViewModel>" %>

<div style="width: 700px; padding: 0px 10px;">
    <br />    
    <span style="margin-left: 30px; font-size: 16px;"><span class="greytext">Выбран товар: </span> <%: Model.ArticleId %> &nbsp;|&nbsp; <%: Model.ArticleNumber %> &nbsp;|&nbsp; <%: Model.ArticleName %></span>
    <br /><br />

    <table class="display_table">
        <tr>
            <td class="row_title" style="min-width: 200px">
                <%: Html.LabelFor(model => model.AccountingPrice)%>:
            </td>
            <td style="width: 70%">
                <span id="SenderAccountingPrice"><%: Model.AccountingPrice%></span> р.
            </td>            
            <td class="row_title">
                <%: Html.LabelFor(model => model.AvailableToReturnTotalCount)%>:
            </td>
            <td style="width: 30%">
                <span id="AvailableToMoveTotalCount"><%: Model.AvailableToReturnTotalCount%></span>
            </td>
        </tr>
    </table>

    <br />

    <% Html.RenderPartial("ArticleSaleGrid", Model.SaleGrid); %>
                        
    <div class="button_set">            
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>


