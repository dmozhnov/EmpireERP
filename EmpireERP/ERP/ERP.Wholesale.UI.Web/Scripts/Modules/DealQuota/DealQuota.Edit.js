var DealQuota_Edit = {
    Init: function () {
        $(document).ready(function () {
            if (IsFalse($('#AllowToEdit').val())) {
                $("[name='IsPrepayment']").attr("disabled", "disabled");
            }

            DealQuota_Edit.SetPrepayment($("#IsPrepayment").val() != "0");
        });

        $('#rbIsPrepayment_false').click(function () {
            DealQuota_Edit.SetPrepayment(false);
        });

        $('#rbIsPrepayment_true').click(function () {
            DealQuota_Edit.SetPrepayment(true);
        });
    },

    SetPrepayment: function (status) {
        if (IsTrue(status)) {
            $("#PostPaymentGroup *").attr("disabled", "disabled");            
            $('#dealQuotaEdit #PostPaymentDays').ValidationValid();
            $("#CreditLimitSumGroup *").attr("disabled", "disabled");            
            $('#dealQuotaEdit #CreditLimitSum').ValidationValid();
            $("#dealQuotaEdit #IsPrepayment").val("1");
        }
        else {
            $("#PostPaymentGroup *").removeAttr("disabled");
            $("#CreditLimitSumGroup *").removeAttr("disabled");
            $("#dealQuotaEdit #IsPrepayment").val("0");
        }
    },

    OnFailSaveDealQuota: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageDealQuotaEdit");
    }
};