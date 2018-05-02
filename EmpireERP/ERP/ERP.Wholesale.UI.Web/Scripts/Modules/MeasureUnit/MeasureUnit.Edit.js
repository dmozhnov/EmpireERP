var MeasureUnit_Edit = {    
    OnBeginMeasureUnitSave : function () {
        StartButtonProgress($("#btnSaveMeasureUnit"));
    },
        
    OnFailMeasureUnitSave : function (ajaxContext) {        
        ShowErrorMessage(ajaxContext.responseText, "messageMeasureUnitEdit");
    }
};
