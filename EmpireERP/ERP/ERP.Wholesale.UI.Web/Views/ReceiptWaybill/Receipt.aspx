<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ReceiptWaybill.ReceiptViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Приемка накладной
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
     <script type="text/javascript">
         ReceiptWaybill_Receipt.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.BackURL) %>
    <%: Html.HiddenFor(model => model.WaybillId) %>
    <%: Html.HiddenFor(model => model.AllowToViewPurchaseCosts) %>

    <%= Html.PageTitle("ReceiptWaybill", "Приемка приходной накладной на склад", "№ " + Model.Number + " от " + Model.Date)%>
        
    <div class="button_set">
        <input id="btnDoReceipt" type="button" value="Выполнить приемку" />
        <%: Html.Button("btnDoReceiptRetroactively", "Выполнить приемку задним числом", Model.AllowToReceiptRetroactively, Model.IsPossibilityToReceiptRetroactively)%>
        <input id="btnCloseReceipt" type="button" value="Вернуться к накладной" />
    </div>
    
    <% if (Model.AllowToViewPurchaseCosts) { %>
        <%= Html.PageBoxTop("Информация о накладной")%>
        <div style="background: #fff; padding: 5px 0;">

            <div id='messageReceiptWaybill'></div>

            <table class='editor_table'>
                <tr>
                    <td class='row_title' style="width: 230px;"><%: Html.LabelFor(model => model.TotalReceiptSum)%>:</td>
                    <td><%: Html.TextBoxFor(model => model.TotalReceiptSum, new { size = "12", maxlength = "19" })%>&nbsp;р.</td>
                    <td class='row_title' style="width: 230px;"><%: Html.LabelFor(model => model.TotalReceiptSumByRows)%>:</td>
                    <td><span id="TotalReceiptSumByRowsCol"><span id="TotalReceiptSumByRows"><%: Model.TotalReceiptSumByRows%></span>&nbsp;р.</span></td>
                </tr>
            </table>

        </div>
        <%= Html.PageBoxBottom() %>
    <% } else { %>
        <br />
    <% } %>

    <div id="messageEditReceiptCount"></div>
    
    <% Html.RenderPartial("ReceiptedArticlesGrid", Model.Articles); %>
                
    <div id="receiptArticleAdd"></div> 
    <div id="dateTimeSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
