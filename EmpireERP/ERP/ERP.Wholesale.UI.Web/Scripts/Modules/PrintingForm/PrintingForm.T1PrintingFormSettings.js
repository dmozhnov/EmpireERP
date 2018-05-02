var PrintingForm_T1PrintingFormSettings = {
    Init: function () {
        $(document).ready(function () {
            $("#btnPrint").click(function () {
                var actionUrl = $("#ActionUrl").val();
                var url = actionUrl + '?WaybillId=' + $("#WaybillId").val();

                if (IsTrue($("#IsNeedSelectPriceType").val())) {
                    url += "&PriceTypeId=" + $("#PriceTypeId option:selected").val();
                }

                // Временная проверка на выбранный раздел для печати
                if (IsTrue($("#IsPrintProductSection:checked").val())) {
                    window.open(url);
                }

                HideModal();
            });
        });
    }
};