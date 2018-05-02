<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ReceiptWaybill.ApprovementViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Окончательное согласование приходной накладной
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ReceiptWaybill_Approve.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.BackURL) %>
    <%: Html.HiddenFor(model => model.WaybillId) %>
    <%: Html.HiddenFor(model => model.AllowToViewPurchaseCosts) %>

    <%= Html.PageTitle("ReceiptWaybill", "Окончательное согласование приходной накладной", Model.Name)%>

    <div class="button_set">
        <input id="btnApprove" type="button" value="Согласовать" />
        <%: Html.Button("btnApproveRetroactively", "Согласовать задним числом", Model.AllowToApproveRetroactively, Model.AllowToApproveRetroactively)%>
        <input id="btnCloseApprovement" type="button" value="Вернуться к накладной" />
    </div>

    <%= Html.PageBoxTop("Информация из накладной")%>
    <div style="background: #fff; padding: 5px 0;">

        <div id='messageApproveWaybill'></div>
    
        <div class="group_title">Ранее введенная информация о накладной</div>
        <div class="h_delim"></div>
        <br />

        <table class='editor_table' style="margin-left: 10px;">
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.PendingSum)%>:</td>
                <td width="50%" align="left">
                    <%: Model.PendingSum %>&nbsp;р.
                </td>
                <td class='row_title'><%: Html.LabelFor(model => model.ReceiptedSum)%>:</td>
                <td width="50%" align="left">
                    <%: Model.ReceiptedSum %>&nbsp;р.</td>
            </tr>
        </table>

        <br />
        <div class="group_title">Итоговая принимаемая информация о накладной</div>
        <div class="h_delim"></div>
        <br />

        <table class='editor_table' style="margin-left: 10px;">
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.TotalApprovedSum)%>:</td>
                <td width="50%">
                    <% if (Model.AllowToViewPurchaseCosts){ %>
                    <%: Html.TextBoxFor(model => model.TotalApprovedSum, new { size = "12", style = "display:inline", maxlength = "19" })%>&nbsp;р.
                    <span class="field-validation-valid" data-valmsg-for="TotalApprovedSum" data-valmsg-replace="true">
                        <span htmlfor="TotalApprovedSum" generated="true" class="">Укажите положительное десятичное число</span>
                    </span>
                    <%}else{%>
                        <%: Model.TotalApprovedSum %>&nbsp;р.
                    <%}%>
                </td>
                <td class='row_title'><%: Html.LabelFor(model => model.ApprovedRowsSum)%>:</td>
                <td width="50%">
                    <span id="ApprovedRowsSum"><%: Model.ApprovedRowsSum%></span>&nbsp;р.
                </td>
            </tr>                        
        </table>
        <br />
    </div>
    <%= Html.PageBoxBottom() %>
            
    <div id='messageEditApproveCount'></div>
    <div id="dateTimeSelector"></div>
    <% Html.RenderPartial("ReceiptWaybillApprovementArticlesGrid", Model.ReceiptWaybillRowGrid); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
