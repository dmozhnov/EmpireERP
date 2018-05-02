var Deal_PaymentEdit = {
    Init: function () {
        $(document).ready(function () {
            // Убрать данный документ. Перенести проверку в js-файлы по созданию оплат от клиента по 1 сделке.
            // Т.е. будет клиентская проверка на максимальный нал (надо позаботиться, чтобы поле правильно заполнялось -
            // если пока не выбрана сделка, но разнесение будет идти обязательно на 1 сделку, то чтобы там было 100,000).
            // 866
            $("#btnSelectDestinationDocuments").click(function () {
                if (IsTrue($("#DealPaymentForm").val() == "1")) {
                    var maxSum = TryGetDecimal($("#MaxCashPaymentSum").val());

                    if ($("#Sum").val() > maxSum) {
                        ShowErrorMessage("Чтобы не превышать максимально допустимой суммы наличных оплат, сумма оплаты должна быть не больше " + ValueForDisplay(maxSum),
                            "messageDealPaymentFromClientEdit");
                        return false;
                    }
                }
            });
        });
    }
};