<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.AccountingPriceList.ArticleAccountingPriceEditViewModel>" %>

<script type="text/javascript">
    AccountingPriceList_PriceEdit.Init();

    function OnBeginArticlePriceEdit() {
        StartButtonProgress($("#btnSaveAccountingPriceList"));
    }
</script>

<% using (Ajax.BeginForm("EditArticle", "AccountingPriceList", new AjaxOptions() { OnBegin = "OnBeginArticlePriceEdit",
       OnSuccess = "AccountingPriceList_PriceEdit.OnSuccessArticlePriceEdit", OnFailure = "AccountingPriceList_PriceEdit.OnFailArticlePriceEdit" }))%>
<%{ %>
    <div class="modal_title"><%: Model.Title %></div>
    <div class="h_delim"></div>
    
    <div style="padding: 10px 10px 5px">        
        <div id="messageArticleAccountingPriceEdit"></div>

        <%: Html.HiddenFor(model => model.ArticleId)%>
        <%: Html.HiddenFor(model => model.Id)%>
        <%: Html.HiddenFor(model => model.AccountingPriceListId)%>
        <%: Html.HiddenFor(model => model.UsedDefaultRule) %>

        <table class='editor_table'>
            <tr>
                <td class='row_title' style="width: 178px;">                    
                    <%: Html.LabelFor(model => model.ArticleName)%>:
                </td>
                <td colspan="3">
                    <span <% if(Model.AllowToEdit) { %> class="select_link" <%} %> id="ArticleName"><%: Model.ArticleName%></span>
                    <%: Html.ValidationMessageFor(model => model.ArticleId)%>                
                </td>
            </tr> 
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.ArticleIdForDisplay)%>:
                </td>
                <td style = "min-width:80px">
                    <span id="ArticleIdForDisplay"><%: Model.ArticleIdForDisplay %></span>
                </td>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.ArticleNumber)%>:
                </td>
                <td style="min-width:150px">
                    <span id="ArticleNumber"><%: Model.ArticleNumber %></span>
                </td>
            </tr>
        </table>               

        <table class='display_table'>
            <tr>
                <td class='row_title' style="width: 178px;">
                    <%: Html.LabelFor(model => model.AverageAccountingPrice) %>:
                </td>
                <td>
                    <span id="AverageAccountingPrice"><%: Model.AverageAccountingPrice %></span>&nbsp;р.
                </td>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.AveragePurchaseCost) %>:
                </td>
                <td>
                    <span id="AveragePurchaseCost"><%: Model.AveragePurchaseCost %></span>&nbsp;р.
                </td>
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.MaxAccountingPrice) %>:
                </td>
                <td>
                    <span id="MaxAccountingPrice"><%: Model.MaxAccountingPrice %></span>&nbsp;р.
                </td>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.MaxPurchaseCost) %>:
                </td>
                <td>
                    <span id="MaxPurchaseCost"><%: Model.MaxPurchaseCost %></span>&nbsp;р.
                </td>
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.MinAccountingPrice) %>:
                </td>
                <td>
                    <span id="MinAccountingPrice"><%: Model.MinAccountingPrice %></span>&nbsp;р.
                </td>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.MinPurchaseCost) %>:
                </td>
                <td>
                    <span id="MinPurchaseCost"><%: Model.MinPurchaseCost %></span>&nbsp;р.
                </td>
            </tr>
             <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.DefaultMarkupPercent) %>:
                </td>
                <td>
                    <span id="DefaultMarkupPercent"><%: Model.DefaultMarkupPercent %></span> %
                </td>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.LastPurchaseCost)%>:
                </td>
                <td>
                    <span id="LastPurchaseCost"><%: Model.LastPurchaseCost%></span>&nbsp;р.
                </td>                
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.AccountingPriceRule) %>:
                </td>
                <td colspan="3">
                    <span id="AccountingPriceRule"><%: Model.AccountingPriceRule%></span>
                </td>                
            </tr>
        </table>

        <table class='editor_table'>              
            <tr>
                <td class='row_title' style="width: 178px;"><%: Html.LabelFor(model => model.CalculatedAccountingPrice)%>:</td>
                <td>
                    <%: Html.NumericTextBoxFor(model => model.CalculatedAccountingPrice, true)%>&nbsp;р.
                </td>
                <td class='row_title'><%: Html.LabelFor(model => model.AccountingPrice)%>:</td>
                <td style="width: 140px">
                    <span id="tbAccountingPrice"><%: Html.NumericTextBoxFor(model => model.AccountingPrice, new { style = "width:90px", maxlength = "19" }, !Model.AllowToEditPrice)%></span>&nbsp;р.
                    <%: Html.ValidationMessageFor(model => model.AccountingPrice)%>
                    <span class="field-validation-valid" id="lblDefaultRuleError"><%:Model.DefaultRuleErrorCaption %></span>
                </td>
            </tr>
        </table>
    </div>
    
    <div class="button_set">
        <%: Html.SubmitButton("btnSaveAccountingPriceList", "Сохранить", Model.AllowToEdit || Model.AllowToEditPrice, Model.AllowToEdit || Model.AllowToEditPrice)%>
        <input type="button" value="Закрыть" onclick="HideModal()" />         
    </div>    
<%} %>

<div id="articleSelector"></div>

