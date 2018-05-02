<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#gridManufacturer #btnAddManufacturer").click(function () {
            StartButtonProgress($(this))
            $.ajax({
                type: "GET",
                url: "/Manufacturer/SelectManufacturerForProducer/",
                data: { producerId: $("#Id").val(), controller: "Producer", action: "SelectManufacturer" },
                success: function (result) {
                    $("#producerSelector").html(result);
                    $.validator.unobtrusive.parse($("#producerSelector"));
                    ShowModal("producerSelector");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageManufactureSelector");
                }
            });
        });

        //Обрабатываем выбор фабрики-изготовителя
        $("#producerSelector .select").live("click", function () {
            var manufacturerId = $(this).parent("td").parent("tr").find(".Id").html();

            $.ajax({
                type: "POST",
                url: "/Producer/AddManufacturer/",
                data: { producerId: $("#Id").val(), manufacturerId: manufacturerId },
                success: function (result) {
                    HideModal(function () {
                        RefreshGrid("gridManufacturer", function () {
                            ShowSuccessMessage("Фабрика-изготовитель добавлена", "messageManufactureSelector");
                        });
                    });
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageManufacturerSelectorListGrid");
                }
            });


        });

        $("#gridManufacturer .delete").click(function () {
            var manufacturerId = $(this).parent("td").parent("tr").find(".Id").html();
            if (confirm('Вы уверены?')) {
                $.ajax({
                    type: "POST",
                    url: "/Producer/RemoveManufacturer/",
                    data: { producerId: $("#Id").val(), manufacturerId: manufacturerId },
                    success: function (result) {
                        RefreshGrid("gridManufacturer", function () {
                            ShowSuccessMessage("Фабрика-изготовитель удалена", "messageManufactureSelector");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageManufactureSelector");
                    }
                });
            }
        });
    });

    function OnSuccessManufacturerSave(ajaxContext) {
        $.ajax({
            type: "POST",
            url: "/Producer/AddManufacturer/",
            data: { producerId: $("#Id").val(), manufacturerId: ajaxContext.Id },
            success: function (result) {
                HideModal(function () {
                    HideModal(function () {
                        RefreshGrid("gridManufacturer", function () {
                            ShowSuccessMessage("Фабрика-изготовитель добавлена", "messageManufactureSelector");
                        });
                    });
                });
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageManufacturerSelectorListGrid");
            }
        });
    }
</script>

<%= Html.GridHeader("Фабрики-изготовители", "gridManufacturer", "/Help/GetHelp_Producer_Details_ProducerManufacturerGrid")%>
    <div class="grid_buttons">
        <%= Html.Button("btnAddManufacturer", "Добавить")%>
    </div>
<%= Html.GridContent(Model, "/Producer/ShowManufacturerGrid/") %>