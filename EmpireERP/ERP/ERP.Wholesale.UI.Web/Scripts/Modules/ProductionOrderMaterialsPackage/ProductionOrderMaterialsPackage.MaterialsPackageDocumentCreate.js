var ProductionOrderMaterialsPackage_MaterialsPackageDocumentCreate = {
    Init: function () {
        $(document).ready(function () {
            var upload = new AjaxUpload($('#file_upload_button'),
                {
                    // Отправляем 
                    autoSubmit: false,               // Отправлять ли файл сразу после выбора
                    action: '/ProductionOrderMaterialsPackage/ProductionOrderMaterialsPackageDocumentSave',    // Куда отправлять
                    name: 'myfile',                 // Имя переменной для хранения файла.
                    response: 'json',                // Ответ сервера.

                    // Срабатывает перед загрузкой файла
                    // Тоже можно вернуть false для отмены.
                    onSubmit: function (file, extension) {
                        $("#btnClose").addClass("disabled").attr("disabled", "disabled");
                        $("#Description").addClass("disabled").attr("disabled", "disabled");
                        $("#FileName").addClass("disabled").attr("disabled", "disabled");
                        $("#file_upload_button").addClass("hidden");

                        upload.setData({ "DocumentId": $("#DocumentId").val(), "PackageId": $("#PackageId").val(), "Description": $("#Description").val() });
                    },

                    onChange: function (file, extension) {
                        $("#FileName").val(file);
                        $("#btnSave").removeClass("disabled").removeAttr("disabled");
                    },

                    // Выполняется после получения ответа от сервера.
                    // file - имя файла, который указал клиент.
                    // response - ответ сервера.
                    onComplete: function (file, response) {
                        var resultObject = eval('(' + response + ')');

                        upload.destroy();
                        OnSuccessProductionOrderMaterialsPackageDocumentCreate(resultObject.obj);
                    }
                });

            $("#btnSave").click(function () {
                StartButtonProgress($("#btnSave"));
                upload.submit();
            });
        });
    }
};