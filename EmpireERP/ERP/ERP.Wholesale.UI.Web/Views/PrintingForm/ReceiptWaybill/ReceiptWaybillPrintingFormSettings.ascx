<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill.ReceiptWaybillPrintingFormSettingsViewModel>" %>
<script type="text/javascript">
    $(document).ready(function () {
        $("#btnPrint").click(function () {
            window.open("/ReceiptWaybill/ShowPrintingForm?PrintPurchaseCost=" + $("#PrintPurchaseCost").attr('checked') +
            "&PrintAccountingPrice=" + $("#PrintAccountingPrice").attr('checked') + "&PrintMarkup=" + $("#PrintMarkup").attr('checked') + "&WaybillId=" + $("#Id").val());
            HideModal();
        });

        revalid();

        // Опасный код, но работает. Если CheckBox disabled, то $("#PrintPurchaseCost") возвращает вовсе не CheckBox, а Hidden (у которого, правда, никогда нет checked).
        // Надо написать новую общую функцию получения значения CheckBox по Id или квалификатору jquery, null, если не найдено поле
        function revalid() {
            if ($("#PrintPurchaseCost").attr("checked") && $("#PrintAccountingPrice").attr("checked")) {
                $("#PrintMarkup").removeAttr("disabled");
            }
            else {
                $("#PrintMarkup").attr("disabled", "disabled");
                $("#PrintMarkup").attr("checked", false);
            }
        };


        $("#PrintPurchaseCost").change(function () {
            revalid();
        });

        $("#PrintAccountingPrice").change(function () {
            revalid();
        });
    });
</script>

<div class="modal_title">Настройки печатной формы</div>
<div class="h_delim"></div>

<div style="padding: 10px 10px 5px">
    <%: Html.CheckBoxFor(x => x.PrintPurchaseCost, !Model.AllowToViewPurchaseCosts)%>
    <%: Html.LabelFor(x => x.PrintPurchaseCost)%>
    <%: Html.CheckBoxFor(x => x.PrintAccountingPrice, !Model.AllowToViewAccountingPrices )%>
    <%: Html.LabelFor(x => x.PrintAccountingPrice )%>
    <%: Html.CheckBoxFor(x => x.PrintMarkup)%>
    <%: Html.LabelFor(x => x.PrintMarkup)%>
</div>

<div class="button_set">
    <input type="button" id="btnPrint" value="Печать" />
    <input type="button" value="Закрыть" onclick="HideModal();" />
</div>
