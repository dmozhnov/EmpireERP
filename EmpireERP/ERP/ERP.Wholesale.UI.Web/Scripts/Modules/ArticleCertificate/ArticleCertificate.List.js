var ArticleCertificate_List = {
    OnSuccessArticleCertificateSave: function () {
        HideModal();
        RefreshGrid("gridArticleCertificates", function () {
            ShowSuccessMessage("Сертификат товара сохранен.", "messageArticleCertificateList");
        });
    }
};
