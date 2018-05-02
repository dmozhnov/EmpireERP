var Storage_Details = {
    Init: function () {
        $(document).ready(function () {
            $("#btnEditStorage").live('click', function () {
                var storage_id = $("#storage_id").val();

                $.ajax({
                    type: "GET",
                    url: "/Storage/Edit/",
                    data: { id: storage_id },
                    success: function (result) {
                        $('#storageEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#storageEdit"));
                        ShowModal("storageEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageStorageDetails");
                    }
                });
            });

            // создание реестра цен по умолчанию для данного места хранения
            $('#btnCreatePriceListByDefault').live('click', function () {
                var storage_id = $("#storage_id").val();
                window.location = "/AccountingPriceList/Create?reasonCode=3&additionalId=" + storage_id + GetBackUrl();
            });

            // удаление места хранения
            $("#btnDeleteStorage").live("click", function () {
                if (confirm('Вы действительно хотите удалить место хранения?')) {
                    var storage_id = $("#storage_id").val();

                    StartButtonProgress($(this));
                    $.ajax({
                        type: "POST",
                        url: "/Storage/Delete/",
                        data: { id: storage_id },
                        success: function (result) {
                            window.location = "/Storage/List/";
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageStorageDetails");
                        }
                    });
                };
            });

            // возврат к списку
            $("#btnBackToList").live('click', function () {
                window.location = $('#BackURL').val();
            });

            // добавление связанной организации
            $("#btnAddAccountOrganization").live('click', function () {
                var storage_id = $("#storage_id").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Storage/GetAvailableAccountOrganizations",
                    data: { storageId: storage_id },
                    success: function (result) {
                        $('#accountOrganizationSelectList').hide().html(result);
                        $.validator.unobtrusive.parse($("#accountOrganizationSelectList"));
                        ShowModal("accountOrganizationSelectList");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountOrganizationAdd");
                    }
                });
            });

            // удаление связанной организации
            $("#gridAccountOrganization .delete_accountOrganization_link").live('click', function () {
                if (confirm('Вы уверены?')) {
                    var accountOrganizationId = $(this).parent("td").parent("tr").find(".accountOrganizationId").text();
                    var storageId = $("#storage_id").val();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Storage/DeleteAccountOrganization/",
                        data: { accountOrganizationId: accountOrganizationId, storageId: storageId },
                        success: function (result) {
                            RefreshGrid("gridAccountOrganization", function () {
                                Storage_Details.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Организация удалена.", "messageAccountOrganizationAdd");                                
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountOrganizationAdd");
                        }
                    });
                }
            });

            // добавление секции места хранения
            $("#btnCreateStorageSection").live("click", function () {
                var storage_id = $("#storage_id").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Storage/AddSection",
                    data: { storageId: storage_id },
                    success: function (result) {
                        $('#storageSectionEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#storageSectionEdit"));
                        ShowModal("storageSectionEdit");
                        $("#storageSectionEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageStorageSectionList");
                    }
                });
            });

            // редактирование секции места хранения
            $("#gridStorageSection .name_link").live('click', function () {
                var storageId = $("#storage_id").val();
                var storageSectionId = $(this).parent("td").parent("tr").find(".storageSectionId").text();

                $.ajax({
                    type: "GET",
                    url: "/Storage/EditSection/",
                    data: { storageSectionId: storageSectionId, storageId: storageId },
                    success: function (result) {
                        $('#storageSectionEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#storageSectionEdit"));
                        ShowModal("storageSectionEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageStorageSectionList");
                    }
                });
            });


            // удаление секции места хранения
            $("#gridStorageSection .delete_storageSection_link").live('click', function () {
                if (confirm('Вы действительно хотите удалить секцию?')) {
                    var storageSectionId = $(this).parent("td").parent("tr").find(".storageSectionId").text();
                    var storageId = $("#storage_id").val();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Storage/DeleteStorageSection/",
                        data: { storageSectionId: storageSectionId, storageId: storageId },
                        success: function (result) {
                            RefreshGrid("gridStorageSection", function () {
                                Storage_Details.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Секция удалена.", "messageStorageSectionList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageStorageSectionList");
                        }
                    });
                }
            });
        });
    },

    // после успешной попытки редактирования места хранения
    OnSuccessStorageSave: function (result) {
        HideModal();
        Storage_Details.RefreshMainDetails(result.MainDetails);
        $(".page_title_item_name").text($("#Name").text());
        ShowSuccessMessage("Сохранено.", "messageStorageDetails");         
    },

    // при успешной попытке добавления связанной организации
    OnSuccessStorageAccountOrganizationAdd: function (result) {       
        HideModal();
        RefreshGrid("gridAccountOrganization", function () {
            Storage_Details.RefreshMainDetails(result.MainDetails);
            ShowSuccessMessage("Организация добавлена.", "messageAccountOrganizationAdd");            
        });
    },

    // при успешной попытке добавления/редактирования секции
    OnSuccessStorageSectionSave: function (result) {
        HideModal();
        RefreshGrid("gridStorageSection", function () {
            Storage_Details.RefreshMainDetails(result.MainDetails);
            ShowSuccessMessage("Сохранено.", "messageStorageSectionList");            
        });
    },

    // обновление основной информации
    RefreshMainDetails: function (details) {
        $("#SectionCount").text(details.SectionCount);
        $("#Name").text(details.Name);
        $("#AccountOrganizationCount").text(details.AccountOrganizationCount);
        $("#TypeName").text(details.TypeName);
        $("#Comment").html(details.Comment);
    },
}; 