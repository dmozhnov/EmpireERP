var ArticleCertificate_Edit = {    
    OnFailArticleCertificateSave : function (ajaxContext) {        
        ShowErrorMessage(ajaxContext.responseText, "messageArticleCertificateEdit");        
    }
};
