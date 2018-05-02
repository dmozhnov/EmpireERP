<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderArticlePrimeCostSettingsViewModel>" %>

<div style="width:520px; padding-left:10px; padding-right:10px;">

<% using (Ajax.BeginForm("", "", new AjaxOptions() { }))%>
<%{ %>

    <%:Html.HiddenFor(model => model.ProductionOrderId)%>

    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrder_Edit_ProductionOrderArticlePrimeCostSettingsForm") %></div>
    <div class="h_delim"></div>

    <br />
    <div id="messageProductionOrderArticlePrimeCostForm"></div>

    <table class="editor_table">
        <tr>
            <td class="row_title" style="min-width: 265px"><%:Html.LabelFor(model => model.ArticlePrimeCostCalculationTypeId)%>:</td>
            <td style="width: 100%">
                <%: Html.DropDownListFor(model => model.ArticlePrimeCostCalculationTypeId, Model.ArticlePrimeCostCalculationTypeList, new { style = "width:235px" })%>
                <%: Html.ValidationMessageFor(model => model.ArticlePrimeCostCalculationTypeId)%>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.DivideCustomsExpenses)%>:</td>
            <td>
                <%: Html.YesNoToggleFor(model => model.DivideCustomsExpenses)%>
                <%: Html.ValidationMessageFor(model => model.DivideCustomsExpenses)%>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.ShowArticleVolumeAndWeight)%>:</td>
            <td>
                <%: Html.YesNoToggleFor(model => model.ShowArticleVolumeAndWeight)%>
                <%: Html.ValidationMessageFor(model => model.ShowArticleVolumeAndWeight)%>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.ArticleTransportingPrimeCostCalculationTypeId)%>:</td>
            <td>
                <%: Html.DropDownListFor(model => model.ArticleTransportingPrimeCostCalculationTypeId, Model.ArticleTransportingPrimeCostCalculationTypeList,
                    new { style = "width:235px" })%>
                <%: Html.ValidationMessageFor(model => model.ArticleTransportingPrimeCostCalculationTypeId)%>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.IncludeUnsuccessfullyClosedBatches)%>:</td>
            <td>
                <%: Html.YesNoToggleFor(model => model.IncludeUnsuccessfullyClosedBatches)%>
                <%: Html.ValidationMessageFor(model => model.IncludeUnsuccessfullyClosedBatches)%>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.IncludeUnapprovedBatches)%>:</td>
            <td>
                <%: Html.YesNoToggleFor(model => model.IncludeUnapprovedBatches)%>
                <%: Html.ValidationMessageFor(model => model.IncludeUnapprovedBatches)%>
            </td>
        </tr>
        <% if (Model.IsMarkupPercentEnabled) { %>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.MarkupPercent)%>:</td>
            <td>
                <%: Html.TextBoxFor(model => model.MarkupPercent, new { style = "width:90px", maxlength = "7" })%>
                <%: Html.ValidationMessageFor(model => model.MarkupPercent)%>
            </td>
        </tr>
        <% } %>
    </table>

    <div class="button_set">
        <%= Html.Button("btnCalculateArticlePrimeCost", "Рассчитать")%>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>

<% } %>

</div>
