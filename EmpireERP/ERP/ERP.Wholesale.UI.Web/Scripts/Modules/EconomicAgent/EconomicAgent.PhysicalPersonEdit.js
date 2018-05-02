var EconomicAgent_PhysicalPersonEdit = {
    Init: function () {
        $("#ShortName").focus();        
     },

     OnFailPhysicalPersonEdit:function(ajaxContext) {
         ShowErrorMessage(ajaxContext.responseText, "messageOrganizationEdit");
     }
};