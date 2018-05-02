<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.PrintingForm.WriteoffWaybill.WriteoffWaybillPrintingFormSettingsViewModel>" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#btnPrint").click(function () {
            window.open("/WriteoffWaybill/ShowWriteoffWaybillPrintingForm?PrintPurchaseCost=" + $("#PrintPurchaseCost").attr('checked') +
            "&PrintAccountingPrice=" + $("#PrintAccountingPrice").attr('checked') + "&WaybillId=" + $("#Id").val());
            HideModal();
        });
    });
</script>

<div class="modal_title">Настройки печатной формы</div>
<div class="h_delim"></div>

<div style="padding: 10px 10px 5px">    
    <%: Html.CheckBoxFor(x => x.PrintAccountingPrice, !Model.AllowToViewAccountingPrices )%>
    <%: Html.LabelFor(x => x.PrintAccountingPrice )%>

    <%: Html.CheckBoxFor(x => x.PrintPurchaseCost, !Model.AllowToViewPurchaseCosts)%>
    <%: Html.LabelFor(x => x.PrintPurchaseCost)%>    
</div>

<div class="button_set">
    <input type="button" id="btnPrint" value="Печать" />
    <input type="button" value="Закрыть" onclick="HideModal();" />
</div>