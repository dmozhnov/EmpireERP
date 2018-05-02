var ArticleCertificate_ArticleCertificateSelector = {
    Init: function () {
        $(document).ready(function () {
            $("#createArticleCertificate").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/ArticleCertificate/Create/",
                    success: function (result) {
                        $("#articleCertificateEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#articleCertificateEdit"));
                        ShowModal("articleCertificateEdit");
                        $("#articleCertificateEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageSelectArticleCertificate");
                    }
                });
            });
        });
    }
};