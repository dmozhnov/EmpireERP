var Report0006PrintingForm_Settings = {
    Init: function () {
        $(document).ready(function () {
            $("#btnPrint").click(function () {
                if (Report0006_Settings.ValidateDate($("#StartDate").val(), $("#EndDate").val(), "messageReport0006PrintingFormSettings", false)) {
                    window.open("/Report/Report0006PrintingForm?printingFormClientId=" + $("#PrintingFormClientId").val() +
                    "&printingFormClientOrganizationId=" + $("#PrintingFormClientOrganizationId").val() +
                    "&startDate=" + $("#StartDate").val() +
                    "&endDate=" + $("#EndDate").val());
                    HideModal();
                }
            });
        });
    }
};