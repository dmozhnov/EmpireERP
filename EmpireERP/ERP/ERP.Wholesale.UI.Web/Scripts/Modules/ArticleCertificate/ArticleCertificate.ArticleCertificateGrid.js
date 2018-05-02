var ArticleCertificate_ArticleCertificateGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateArticleCertificate').click(function () {
                StartButtonProgress($(this));

                var id = 0;
                ArticleCertificate_ArticleCertificateGrid.ShowArticleCertificateDetailsForEdit(id);
            });

            $('#gridArticleCertificates .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                ArticleCertificate_ArticleCertificateGrid.ShowArticleCertificateDetailsForEdit(id);
            });

            $('#gridArticleCertificates .delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/ArticleCertificate/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridArticleCertificates", function () {
                                ShowSuccessMessage("Сертификат товара удален.", "messageArticleCertificateList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleCertificateList");
                        }
                    });
                }
            });
        });
    },

    ShowArticleCertificateDetailsForEdit: function (id) {
        var method = IsDefaultOrEmpty(id) ? "Create" : "Edit";

        $.ajax({
            type: "GET",
            url: "/ArticleCertificate/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#articleCertificateEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#articleCertificateEdit"));
                ShowModal("articleCertificateEdit");
                $("#articleCertificateEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageArticleCertificateList");
            }
        });
    }
};
