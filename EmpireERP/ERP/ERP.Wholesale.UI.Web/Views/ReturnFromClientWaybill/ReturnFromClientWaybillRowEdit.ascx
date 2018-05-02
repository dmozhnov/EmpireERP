<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill.ReturnFromClientWaybillRowEditViewModel>" %>

<script type="text/javascript">
    ReturnFromClientWaybill_RowEdit.Init();

    function OnBeginReturnFromClientWaybillRowEdit() {
        StartButtonProgress($("#btnSaveReturnFromClientWaybillRow"));
    }

    function OnSuccessReturnFromClientWaybillRowEdit(ajaxContext) {
        ReturnFromClientWaybill_RowEdit.OnSuccessReturnFromClientWaybillRowEdit(ajaxContext);
    }
</script>

<% using (Ajax.BeginForm("SaveRow", "ReturnFromClientWaybill", new AjaxOptions() { OnBegin = "OnBeginReturnFromClientWaybillRowEdit",
       OnSuccess = "OnSuccessReturnFromClientWaybillRowEdit", OnFailure = "ReturnFromClientWaybill_RowEdit.OnFailReturnFromClientWaybillRowEdit" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.ArticleId) %>
    <%: Html.HiddenFor(model => model.MeasureUnitScale) %>
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.ReturnFromClientWaybillId)%>
    <%: Html.HiddenFor(model => model.ClientId) %>
    <%: Html.HiddenFor(model => model.DealId) %>
    <%: Html.HiddenFor(model => model.TeamId) %>
    <%: Html.HiddenFor(model => model.RecipientId) %>
    <%: Html.HiddenFor(model => model.RecipientStorageId)%>
    <%: Html.HiddenFor(model => model.SaleWaybillRowId)%>    
    <%: Html.HiddenFor(model => model.CurrentSaleWaybillRowId) %>
    <%: Html.HiddenFor(model => model.ReturnFromClientWaybillDate)%>
    <%: Html.HiddenFor(model => model.AllowToViewPurchaseCost) %>
    <%: Html.HiddenFor(model => model.AllowToViewAccountingPrice) %>

    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ReturnFromClientWaybill_RowEdit")%></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">
        <div id="messageReturnFromClientWaybillRowEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 150px">
                    <%: Html.LabelFor(model => model.ArticleName) %>:                    
                </td>
                <td>
                    <span <% if (Model.AllowToEdit) { %> id="ArticleName" class="select_link" <%} %>><%: Model.ArticleName%></span>                     
                    <%: Html.ValidationMessageFor(model => model.ArticleId)%>
                </td>                   
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.SaleWaybillName) %>:
                </td>
                <td>
                    <span id="SaleWaybillName"><%: Model.SaleWaybillName %></span> &nbsp;
                    <% if (Model.AllowToEdit) %>
                    <%{ %>
                        <span id="SaleLink" class="select_link" style="display: none;">изменить реализацию</span>
                    <%} %>
                </td>                
            </tr>
        </table>
        <br />
        <table class="display_table">
            <tr>
                <td class="row_title" style="width: 150px">
                    <%: Html.LabelFor(model => model.PurchaseCost) %>:
                </td>
                <td>
                    <span id="PurchaseCost"><%: Model.PurchaseCost%></span> р.
                </td>
                <td class="row_title" style="width: 200px">
                    <%: Html.LabelFor(model => model.TotalSoldCount)%>:
                </td>
                <td style="min-width: 60px">
                    <span id="TotalSoldCount"><%: Model.TotalSoldCount%></span>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.SalePrice) %>:
                </td>
                <td>
                    <span id="SalePrice"><%: Model.SalePrice%></span> р.
                </td>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.ReturnedCount) %>:
                </td>
                <td>
                    <span id="ReturnedCount"><%: Model.ReturnedCount%></span>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.AccountingPrice) %>:
                </td>
                <td>
                    <span id="AccountingPrice"><%: Model.AccountingPrice%></span> р.
                </td>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.AvailableToReturnCount)%>:
                </td>
                <td>
                    <b class="greentext"><span id="AvailableToReturnCount"><%: Model.AvailableToReturnCount%></span></b>
                </td>
            </tr>
        </table>
        
        <br />
        
        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 150px">
                    <%: Html.LabelFor(model => model.ReturningCount)%>:
                </td>
                <td style="max-width: 340px">
                    <%: Html.TextBoxFor(model => model.ReturningCount, new { @style = "width: 100px;" }, !Model.AllowToEdit)%>
                    <span id="MeasureUnitName"><%: Model.MeasureUnitName %></span>
                    <%: Html.ValidationMessageFor(model => model.ReturningCount)%>
                </td>
            </tr>
        </table>
    </div>
    
    <div class="button_set">
        <%: Html.SubmitButton("btnSaveReturnFromClientWaybillRow", "Сохранить", false, Model.AllowToEdit)%>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%} %>

<div id="articleSelector"></div>
<div id="articleSaleSelector"></div>