<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.PrintingForm.MovementWaybill.MovementWaybillPrintingFormSettingsViewModel>" %>
<script type="text/javascript">
    $(document).ready(function () {

        $("#btnPrint").click(function () {
            window.open("/MovementWaybill/ShowPrintingForm?DevideByBatch=" + $("#DevideByBatch").attr('checked') + "&PrintPurchaseCost=" + $("#PrintPurchaseCost").attr('checked') +
            "&PrintSenderPrice=" + $("#PrintSenderPrice").val() + "&PrintMarkup=" + $("#PrintMarkup").val() + "&PrintRecepientPrice=" + $("#PrintRecepientPrice").val() + "&WaybillId=" + $("#Id").val());
            HideModal();
        });

        $("#PrintPurchaseCost").attr("disabled", "disabled");

        $("#DevideByBatch").change(function () {
            if ($(this).attr("checked") && IsTrue($("#AllowToViewPurchaseCosts").val())) {
                $("#PrintPurchaseCost").removeAttr("disabled");
            }
            else {
                $("#PrintPurchaseCost").attr("disabled", "disabled");
                $("#PrintPurchaseCost").attr("checked", false);
            }
        });
    });
            
</script>

<div class="modal_title">Настройки печатной формы</div>
<div class="h_delim"></div>

<div style="padding: 10px 10px 5px">
    <%: Html.CheckBoxFor(x => x.DevideByBatch) %>
    <%: Html.LabelFor(x => x.DevideByBatch) %>
    <%: Html.CheckBoxFor(x => x.PrintPurchaseCost) %>
    <%: Html.LabelFor(x => x.PrintPurchaseCost) %>
    <%: Html.HiddenFor(x => x.PrintSenderPrice) %>
    <%: Html.HiddenFor(x => x.PrintRecepientPrice) %>
    <%: Html.HiddenFor(x => x.PrintMarkup) %>
    <%: Html.HiddenFor(x => x.AllowToViewPurchaseCosts) %>
</div>
<div class="button_set">
    <input type="button" id="btnPrint" value="Печать" />
    <input type="button" value="Закрыть" onclick="HideModal();" />
</div>
